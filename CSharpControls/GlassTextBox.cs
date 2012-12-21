using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace CSharpControls
{
    public class GlassTextBox : TextBox
    {
        private Bitmap _bitmap;
        private bool _paintedFirstTime = false;

        const int WM_PAINT = 0x000F;
        const int WM_PRINT = 0x0317;

        const int PRF_CLIENT = 0x00000004;
        const int PRF_ERASEBKGND = 0x00000008;

        [DllImport("USER32.DLL", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        public GlassTextBox()
        {
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (_bitmap != null)
                _bitmap.Dispose();

            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PAINT)
            {
                _paintedFirstTime = true;
                CaptureBitmap();
                SetStyle(ControlStyles.UserPaint, true);
                base.WndProc(ref m);
                SetStyle(ControlStyles.UserPaint, false);
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SetStyle(ControlStyles.UserPaint, true);
            if (_bitmap != null)
                e.Graphics.DrawImageUnscaled(_bitmap, 0, 0);
            else
                e.Graphics.FillRectangle(Brushes.CornflowerBlue, ClientRectangle);
            SetStyle(ControlStyles.UserPaint, false);
            //base.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Invalidate();
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            if (_paintedFirstTime)
                SetStyle(ControlStyles.UserPaint, false);
            base.OnFontChanged(e);
            if (_paintedFirstTime)
                SetStyle(ControlStyles.UserPaint, true);
        }

        public new BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set
            {
                if (_paintedFirstTime)
                    SetStyle(ControlStyles.UserPaint, false);
                base.BorderStyle = value;
                if (_paintedFirstTime)
                    SetStyle(ControlStyles.UserPaint, true);
            }
        }
        public override bool Multiline
        {
            get { return base.Multiline; }
            set
            {
                if (_paintedFirstTime)
                    SetStyle(ControlStyles.UserPaint, false);
                base.Multiline = value;
                if (_paintedFirstTime)
                    SetStyle(ControlStyles.UserPaint, true);
            }
        }


        private void CaptureBitmap()
        {
            if (_bitmap != null)
                _bitmap.Dispose();

            _bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height, PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(_bitmap))
            {
                int lParam = PRF_CLIENT | PRF_ERASEBKGND;

                System.IntPtr hdc = graphics.GetHdc();
                SendMessage(this.Handle, WM_PRINT, hdc, new IntPtr(lParam));
                graphics.ReleaseHdc(hdc);
            }
            //_bitmap.Save(@"D:\Temp\sqlupdate3.png", ImageFormat.Png);
        }
    }
}
