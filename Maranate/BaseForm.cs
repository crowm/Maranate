using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using CSharpControls;

namespace Maranate
{
    public class BaseForm : Form
    {
        private List<Thread> _threads = new List<Thread>();

        public BaseForm() : base()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);

            string exeFilename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            if (exeFilename.EndsWith(".vshost.exe"))
                exeFilename = exeFilename.Substring(0, exeFilename.Length - 11) + ".exe";
            try
            {
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(exeFilename);
            }
            catch (Exception)
            { }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (this.Owner != null)
            {
                Point pt = this.Owner.Location;
                pt.Offset(6, 6);
                this.Location = pt;
            }

            if (Glass.IsDesktopCompositionEnabled())
            {
                if (this.MainMenuStrip != null)
                {
                    //this.TransparencyKey = Color.FromArgb(121, 122, 123);
                    //this.BackColor = this.TransparencyKey;
                    //this.MainMenuStrip.BackColor = this.TransparencyKey;
                    //foreach (ToolStripMenuItem menuItem in this.MainMenuStrip.Items)
                    //{
                    //    menuItem.BackColor = Color.FromArgb(153, 180, 209);
                    //}
                    //var margins = new Glass.MARGINS()
                    //{
                    //    Left = 0,
                    //    Right = 0,
                    //    Top = this.MainMenuStrip.Height,
                    //    Bottom = 0
                    //};
                    //ExtendGlassFrameIntoClientArea(margins);
                }
            }

            base.OnLoad(e);
        }

        bool IsClosing { get; set; }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            IsClosing = true;
            foreach (var thread in _threads)
            {
                if (thread.IsAlive)
                {
                    thread.Abort();
                    thread.Join();
                }
            }
            _threads.Clear();

            base.OnFormClosing(e);
        }

        protected void RunOnBackgroundThread(Action target)
        {
            RunOnBackgroundThread(null, target);
        }
        protected void RunOnBackgroundThread(string threadName, Action target)
        {
            var thread = new Thread(new ThreadStart(() => {
                try
                {
                    target();
                }
                catch (Exception e)
                {
                    this.Invoke(new Action(() =>
                    {
                        ErrorForm.Show(this, e.ToString(), this.Text + " - Unhandled exception");
                    }));
                }
            }));
            if (threadName != null)
                thread.Name = threadName;
            _threads.Add(thread);
            thread.Start();
        }

        protected void Invoke(Action target)
        {
            if (IsClosing)
                return;
            base.Invoke(target);
        }

        public void ExtendGlassFrameIntoClientArea(CSharpControls.Glass.MARGINS margins)
        {
            CSharpControls.Glass.ExtendGlassFrameIntoClientArea(this, margins);
        }

        public void ExtendGlassFrameToControl(Control control)
        {
            if (Glass.IsDesktopCompositionEnabled())
            {
                var clientBounds = this.RectangleToScreen(this.ClientRectangle);
                var rect = control.RectangleToScreen(control.ClientRectangle);
                rect.Offset(-clientBounds.X, -clientBounds.Y);

                var margins = new Glass.MARGINS()
                {
                    Left = rect.Left,
                    Right = this.ClientRectangle.Width - rect.Right,
                    Top = rect.Top,
                    Bottom = this.ClientRectangle.Height - rect.Bottom
                };
                ExtendGlassFrameIntoClientArea(margins);

                this.Paint += (sender2, e2) =>
                {
                    var glasstxt = new GlassText();
                    glasstxt.FillBlackRegion(e2.Graphics, this.ClientRectangle);
                };
            }
        }

        private void FixScrollableControl(ScrollableControl scrollableControl)
        {
            Type scrollableControlType = typeof(ScrollableControl);
            System.Reflection.MethodInfo setScrollStateMethod = scrollableControlType.GetMethod("SetScrollState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setScrollStateMethod.Invoke(scrollableControl, new object[] { 0x10, true });
        }



    }
}
