using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ImageProcessing
{
    public class YDataFloat
    {
        public YDataFloat(float[,] data, float maximumValue)
        {
            Width = data.GetLength(0);
            Height = data.GetLength(1);
            Data = data;
            MaximumValue = maximumValue;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float[,] Data { get; private set; }
        public float MaximumValue { get; private set; }

        public static YDataFloat FromBitmap(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            var data = new float[width, height];

            var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            unsafe
            {
                Parallel.For(0, height, (y) =>
                {
                    fixed (float* pData = data)
                    {
                        int* row = (int*)imageData.Scan0 + (y * imageData.Stride / sizeof(int));

                        int yDstOffset = y;
                        int xMax = width;

                        int x = 0;
                        while (x < xMax)
                        {
                            int value = row[x++];

                            var color = Color.FromArgb(value);
                            pData[yDstOffset] = color.GetBrightness() * 255.0f;
                            yDstOffset += height;
                        }
                    }
                });
            }

            image.UnlockBits(imageData);

            return new YDataFloat(data, 255.0f);
        }

        public Bitmap ToBitmap()
        {
            long alpha1 = 255L << 24;
            long alpha2 = 255L << 56;

            var image = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);

            unsafe
            {
                Parallel.For(0, Height, (y) =>
                {
                    fixed (float* pData = Data)
                    {
                        long* row = (long*)imageData.Scan0 + (y * imageData.Stride / sizeof(long));

                        int ySrcOffset = y;
                        int xMax = Width / 2;

                        int x = 0;
                        while (x < xMax)
                        {
                            long value;

                            float fColor1 = (pData[ySrcOffset] / MaximumValue);
                            ySrcOffset += Height;
                            float fColor2 = (pData[ySrcOffset] / MaximumValue);
                            ySrcOffset += Height;

                            long color1 = (long)(fColor1 * byte.MaxValue);
                            long color2 = (long)(fColor2 * byte.MaxValue);

                            if (color1 < 0)
                                color1 = 0;
                            if (color1 > byte.MaxValue)
                                color1 = byte.MaxValue;
                            if (color2 < 0)
                                color2 = 0;
                            if (color2 > byte.MaxValue)
                                color2 = byte.MaxValue;

                            value = color1 | (color1 << 8) | (color1 << 16) | alpha1 | (color2 << 32) | (color2 << 40) | (color2 << 48) | alpha2;

                            row[x++] = value;
                        }
                    }
                });
            }

            image.UnlockBits(imageData);

            return image;
        }

        public static YDataFloat NewData(int width, int height, float maximumValue, float defaultValue)
        {
            var result = new YDataFloat(new float[width, height], maximumValue);

            unsafe
            {
                fixed (float* pData = result.Data)
                {
                    int dataLength = result.Width * result.Height;

                    for (int i = 0; i < dataLength; i++)
                    {
                        pData[i] = defaultValue;
                    }
                }
            }

            return result;
        }

    }

}
