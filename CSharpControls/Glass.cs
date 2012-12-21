using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace CSharpControls
{
    public class Glass
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        public static bool IsDesktopCompositionEnabled()
        {
            try
            {
                if (Environment.OSVersion.Version.Major < 6)
                    return false;
                return DwmIsCompositionEnabled();
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }
        public static void ExtendGlassFrameIntoClientArea(Form form, MARGINS margins)
        {
            DwmExtendFrameIntoClientArea(form.Handle, ref margins);
        }

    }

    public class GlassText
    {
        private const int DTT_COMPOSITED = (int)(1UL << 13);
        private const int DTT_GLOWSIZE = (int)(1UL << 11);

        //Text format consts
        private const int DT_SINGLELINE = 0x00000020;
        private const int DT_CENTER = 0x00000001;
        private const int DT_VCENTER = 0x00000004;
        private const int DT_NOPREFIX = 0x00000800;

        //Const for BitBlt
        private const int SRCCOPY = 0x00CC0020;


        //Consts for CreateDIBSection
        private const int BI_RGB = 0;
        private const int DIB_RGB_COLORS = 0;//color table in RGBs

        private struct POINTAPI
        {
            public int x;
            public int y;
        };

        private struct DTTOPTS
        {
            public uint dwSize;
            public uint dwFlags;
            public uint crText;
            public uint crBorder;
            public uint crShadow;
            public int iTextShadowType;
            public POINTAPI ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public int fApplyOverlay;
            public int iGlowSize;
            public IntPtr pfnDrawTextCallback;
            public int lParam;
        };

        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;


        };

        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        };

        private struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        };

        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        } 


        //API declares

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern int SaveDC(IntPtr hdc);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hdc, int state);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("UxTheme.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);
        [DllImport("UxTheme.dll", ExactSpelling = true, SetLastError = true)]
        private static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags1, int dwFlags2, ref RECT pRect);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr FillRect(IntPtr hDC, [In] ref RECT rect, IntPtr hbr);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, EntryPoint = "GetObject")]
        public static extern int GetObjectBitmap(IntPtr hObject, int nCount, ref BITMAP lpObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);


        public void FillBlackRegion(Graphics gph, Rectangle rgn)
        {
            var destdc = gph.GetHdc();
            FillBlackRegionInternal(destdc, rgn);
            gph.ReleaseHdc(destdc);
        }
        public void FillBlackRegion(IntPtr hwnd, Rectangle rgn)
        {
            IntPtr destdc = GetDC(hwnd);    //hwnd must be the handle of form,not control
            FillBlackRegionInternal(destdc, rgn);
        }

        private void FillBlackRegionInternal(IntPtr destdc, Rectangle rgn)
        {
            RECT rc = new RECT();
            rc.left = rgn.Left;
            rc.right = rgn.Right;
            rc.top = rgn.Top;
            rc.bottom = rgn.Bottom;

            IntPtr Memdc = CreateCompatibleDC(destdc);
            IntPtr bitmap;
            IntPtr bitmapOld = IntPtr.Zero;

            BITMAPINFO dib = new BITMAPINFO();
            dib.bmiHeader.biHeight = -(rc.bottom - rc.top);
            dib.bmiHeader.biWidth = rc.right - rc.left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;
            if (!(SaveDC(Memdc) == 0))
            {
                bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                if (!(bitmap == IntPtr.Zero))
                {
                    bitmapOld = SelectObject(Memdc, bitmap);
                    BitBlt(destdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, Memdc, 0, 0, SRCCOPY);
                }

                //Remember to clean up
                SelectObject(Memdc, bitmapOld);

                DeleteObject(bitmap);

                ReleaseDC(Memdc, -1);
                DeleteDC(Memdc);


            }
        }

        public void DrawTextOnGlass(Graphics gph, String text, Font font, Rectangle ctlrct, ContentAlignment textAlign, int iglowSize)
        {
            var destdc = gph.GetHdc();
            DrawTextOnGlassInternal(destdc, text, font, ctlrct, textAlign, iglowSize);
            gph.ReleaseHdc(destdc);
        }
        public void DrawTextOnGlass(IntPtr hwnd, String text, Font font, Rectangle ctlrct, ContentAlignment textAlign, int iglowSize)
        {
            IntPtr destdc = GetDC(hwnd);    //hwnd must be the handle of form,not control
            DrawTextOnGlassInternal(destdc, text, font, ctlrct, textAlign, iglowSize);
        }

        private void DrawTextOnGlassInternal(IntPtr destdc, String text, Font font, Rectangle ctlrct, ContentAlignment textAlign, int iglowSize)
        {
            if (Glass.IsDesktopCompositionEnabled())
            {
                RECT rc = new RECT();
                RECT rc2 = new RECT();

                var iglowExpand = (int)(iglowSize * 0.5);
                //iglowExpand = 0;

                //make it larger to contain the glow effect
                rc.left = ctlrct.Left - iglowExpand;
                rc.right = ctlrct.Right;// + iglowExpand;
                rc.top = ctlrct.Top - iglowExpand;
                rc.bottom = ctlrct.Bottom + iglowExpand;

                //Just the same rect with rc,but (0,0) at the lefttop
                rc2.left = iglowExpand;
                rc2.top = iglowExpand;
                rc2.right = rc.right - rc.left;// - iglowExpand;
                rc2.bottom = rc.bottom - rc.top - iglowExpand;

                IntPtr Memdc = CreateCompatibleDC(destdc);   // Set up a memory DC where we'll draw the text.
                IntPtr bitmap;
                IntPtr bitmapOld = IntPtr.Zero;
                IntPtr logfnotOld;

                //int uFormat = DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX;   //text format
                TextFormatFlags uFormat = TextFormatFlags.NoPrefix;   //text format
                //if ((textAlign == ContentAlignment.TopCenter) ||
                //    (textAlign == ContentAlignment.MiddleCenter) ||
                //    (textAlign == ContentAlignment.BottomCenter))
                //    uFormat |= TextFormatFlags.HorizontalCenter;

                //if ((textAlign == ContentAlignment.TopRight) ||
                //    (textAlign == ContentAlignment.MiddleRight) ||
                //    (textAlign == ContentAlignment.BottomRight))
                //    uFormat |= TextFormatFlags.Right;

                //if ((textAlign == ContentAlignment.MiddleLeft) ||
                //    (textAlign == ContentAlignment.MiddleCenter) ||
                //    (textAlign == ContentAlignment.MiddleRight))
                //    uFormat |= TextFormatFlags.VerticalCenter;

                //if ((textAlign == ContentAlignment.BottomLeft) ||
                //    (textAlign == ContentAlignment.BottomCenter) ||
                //    (textAlign == ContentAlignment.BottomRight))
                //    uFormat |= TextFormatFlags.Bottom;

                BITMAPINFO dib = new BITMAPINFO();
                dib.bmiHeader.biHeight = -(rc.bottom - rc.top);         // negative because DrawThemeTextEx() uses a top-down DIB
                dib.bmiHeader.biWidth = rc.right - rc.left;
                dib.bmiHeader.biPlanes = 1;
                dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                dib.bmiHeader.biBitCount = 32;
                dib.bmiHeader.biCompression = BI_RGB;
                if (!(SaveDC(Memdc) == 0))
                {
                    bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);   // Create a 32-bit bmp for use in offscreen drawing when glass is on
                    if (!(bitmap == IntPtr.Zero))
                    {
                        bitmapOld = SelectObject(Memdc, bitmap);
                        IntPtr hFont = font.ToHfont();
                        logfnotOld = SelectObject(Memdc, hFont);
                        try
                        {

                            System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);

                            DTTOPTS dttOpts = new DTTOPTS();
                            dttOpts.dwSize = (uint)Marshal.SizeOf(typeof(DTTOPTS));
                            dttOpts.dwFlags = DTT_COMPOSITED | DTT_GLOWSIZE;
                            dttOpts.iGlowSize = iglowSize;

                            DrawThemeTextEx(renderer.Handle, Memdc, 0, 0, text, -1, (int)uFormat, ref rc2, ref dttOpts);

                            BitBlt(destdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, Memdc, 0, 0, SRCCOPY);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                        }

                        //using (var image = GetImageFromHBitmap(bitmap))
                        //{
                        //    image.Save(@"d:\temp\sqlupdate.png", System.Drawing.Imaging.ImageFormat.Png);
                        //}

                        //Remember to clean up
                        SelectObject(Memdc, bitmapOld);
                        SelectObject(Memdc, logfnotOld);
                        DeleteObject(bitmap);
                        DeleteObject(hFont);

                        ReleaseDC(Memdc, -1);
                        DeleteDC(Memdc);
                    }
                }
            }
        }

        public Image GetImageFromHBitmap(IntPtr hBitmap)
        {
            var bitmapStruct = new BITMAP();
            GetObjectBitmap(hBitmap, Marshal.SizeOf(bitmapStruct), ref bitmapStruct);
            var image = new Bitmap(bitmapStruct.bmWidth, bitmapStruct.bmHeight, bitmapStruct.bmWidth * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, bitmapStruct.bmBits);
            return image;
        }

        public void SaveGraphics(Form form, Graphics g)
        {
            var handle = form.Handle;
            IntPtr hdcSrc = GetDC(handle);    //hwnd must be the handle of form,not control

            var width = form.Width;
            var height = form.Height;

            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);   // Set up a memory DC where we'll draw the text.

            // create a bitmap we can copy it to,
            BITMAPINFO dib = new BITMAPINFO();
            dib.bmiHeader.biHeight = -height;         // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = width;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;
            var hBitmap = CreateDIBSection(hdcDest, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);   // Create a 32-bit bmp for use in offscreen drawing when glass is on


            // select the bitmap object
            IntPtr hOld = SelectObject(hdcDest, hBitmap);

            // bitblt over
            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, SRCCOPY);

            //using (var img = GetImageFromHBitmap(hBitmap))
            //{
            //    img.Save(@"D:\Temp\sqlupdate2.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            // restore selection
            SelectObject(hdcDest, hOld);

            // clean up 
            DeleteDC(hdcDest);
            ReleaseDC(hdcSrc, -1);


            // free up the Bitmap object
            DeleteObject(hBitmap);
        }
    }
}
