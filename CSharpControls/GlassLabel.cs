using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace CSharpControls
{
    public class GlassLabel : Label
    {
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                if (Glass.IsDesktopCompositionEnabled())
                {
                    var form = this.FindForm();
                    var glass = new GlassText();
                    glass.DrawTextOnGlass(this.FindForm().Handle, Text, Font, Bounds, TextAlign, 8);
                    glass.DrawTextOnGlass(e.Graphics, Text, Font, ClientRectangle, TextAlign, 8);

                    return;
                }
            }
            catch (DllNotFoundException)
            {
            }
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Glass.IsDesktopCompositionEnabled())
                return;

            base.OnPaintBackground(pevent);
        }
    }

}
