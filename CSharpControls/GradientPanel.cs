using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CSharpControls
{
	/// <summary>
	/// Summary description for Gradient.
	/// </summary>
	public class GradientPanel : ScrollableControl
	{
		private Color _color1 = Color.Black;
		private Color _color2 = Color.White;
		private bool _vertical = false;
		private Color _borderColor = Color.Transparent;

		[Category("Appearance"),
		DefaultValue(typeof(Color), "Black"),
		Description("The first colour of the gradient. (top or left)")]
		public Color Color1
		{
			get { return _color1; }
			set { _color1 = value; this.Invalidate(); }
		}

		[Category("Appearance"),
		DefaultValue(typeof(Color), "White"),
		Description("The second colour of the gradient. (bottom or right)")]
		public Color Color2
		{
			get { return _color2; }
			set { _color2 = value; this.Invalidate(); }
		}

		[Category("Appearance"),
		DefaultValue(false),
		Description("Specifies if the direction of the gradient should be vertical")]
		public bool Vertical
		{
			get { return _vertical; }
			set { _vertical = value; this.Invalidate(); }
		}

		[Category("Appearance"),
		DefaultValue(typeof(Color), "Transparent"),
		Description("The colour of the border.")]
		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; this.Invalidate(); }
		}


		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GradientPanel()
		{
			InitializeComponent();

			this.TabStop = false;

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.Selectable, false);
			//this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.UserPaint, true);

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// GradientPanel
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Name = "GradientPanel";
			this.Size = new System.Drawing.Size(150, 8);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.GradientPanel_Paint);

		}
		#endregion

		private void GradientPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (this.BackgroundImage == null)
			{
				Rectangle rectBounds = this.ClientRectangle;

				LinearGradientMode direction = (_vertical) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
				using (LinearGradientBrush lg = new LinearGradientBrush(rectBounds, _color1, _color2, direction))
				{
					e.Graphics.FillRectangle(lg, this.ClientRectangle);
				}

				if (_borderColor.A > 0)
				{
					rectBounds.Width -= 1;
					rectBounds.Height -= 1;
					e.Graphics.DrawRectangle(new Pen(_borderColor), rectBounds);
				}
			}
		}
	}
}
