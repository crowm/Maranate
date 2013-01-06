using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.ParallelArrays;

namespace ImageProcessing
{
    public class YData
    {
        public YData(int width, int height, int stride, byte[] data)
        {
            Width = width;
            Height = height;
            Stride = stride;
            Data = data;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public byte[] Data { get; private set; }

        private YDataFloat _floatData = null;
        private object _floatDataLock = new object();
        public YDataFloat GetFloatData()
        {
            if (_floatData == null)
            {
                lock (_floatDataLock)
                {
                    if (_floatData == null)
                    {
                        var floatArray = new float[Width, Height];
                        int height = Height;

                        unsafe
                        {
                            //for (int y = 0; y < Height; y++)
                            Parallel.For(0, height, (y) =>
                            {
                                fixed (byte* pData = Data)
                                {
                                    fixed (float* pFloat = floatArray)
                                    {
                                        int* pDataInt = (int*)pData;

                                        int ySrcOffset = (y * Stride) / sizeof(int);
                                        int xMax = Width / 2;

                                        int yDstOffset = y;

                                        int x = 0;
                                        while (x < xMax)
                                        {
                                            int color1, color2, color3, color4;

                                            int color = pDataInt[ySrcOffset++];
                                            color1 = (color & 0xFF);
                                            color2 = (color >> 8) & 0xFF;
                                            color3 = (color >> 16) & 0xFF;
                                            color4 = (color >> 24) & 0xFF;

                                            pFloat[yDstOffset] = color1;
                                            yDstOffset += height;
                                            pFloat[yDstOffset] = color2;
                                            yDstOffset += height;
                                            pFloat[yDstOffset] = color3;
                                            yDstOffset += height;
                                            pFloat[yDstOffset] = color4;
                                            yDstOffset += height;

                                            x += 2;
                                        }
                                    }
                                }
                            });
                        }

                        _floatData = new YDataFloat(floatArray, byte.MaxValue);
                    }
                }
            }
            return _floatData;
        }

        public Bitmap GetBitmap()
        {
            long alpha1 = 255L << 24;
            long alpha2 = 255L << 56;

            var image = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);

            unsafe
            {
                Parallel.For(0, Height, (y) =>
                {
                    fixed (byte* pData = Data)
                    {
                        int* pDataInt = (int*)pData;
                        long* row = (long*)imageData.Scan0 + (y * imageData.Stride / sizeof(long));

                        int ySrcOffset = (y * Stride) / sizeof(int);
                        int xMax = Width / 2;

                        int x = 0;
                        while (x < xMax)
                        {
                            long color1, color2, value;

                            int color = pDataInt[ySrcOffset++];
                            color1 = color & 0xFF;
                            color2 = (color >> 8) & 0xFF;

                            value = color1 | (color1 << 8) | (color1 << 16) | alpha1 | (color2 << 32) | (color2 << 40) | (color2 << 48) | alpha2;

                            row[x++] = value;

                            color1 = (color >> 16) & 0xFF;
                            color2 = (color >> 24) & 0xFF;

                            value = color1 | (color1 << 8) | (color1 << 16) | alpha1 | (color2 << 32) | (color2 << 40) | (color2 << 48) | alpha2;

                            row[x++] = value;
                        }
                    }
                });
            }

            image.UnlockBits(imageData);

            return image;
        }

    }

}
