using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace CSharpControls
{
	/// <summary>
	/// A replacement for the Windows Button Control.
	/// </summary>
	[DefaultEvent("Click")]
	public class RoundedPanel : UserControl
	{

		#region -  Designer  -

			private System.ComponentModel.Container components = null;

			/// <summary>
			/// Initialize the component with it's
			/// default settings.
			/// </summary>
			public RoundedPanel()
			{
				InitializeComponent();

				this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				this.SetStyle(ControlStyles.DoubleBuffer, true);
				this.SetStyle(ControlStyles.ResizeRedraw, true);
				this.SetStyle(ControlStyles.Selectable, true);
				this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				this.SetStyle(ControlStyles.UserPaint, true);
				this.BackColor = Color.Transparent;
				this.TabStop = false;
			}

			/// <summary>
			/// Release resources used by the control.
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

			#region -  Component Designer generated code  -

				private void InitializeComponent()
				{
					// 
					// RoundedPanel
					// 
					this.Name = "RoundedPanel";
					this.Size = new System.Drawing.Size(100, 32);
					this.Paint += new System.Windows.Forms.PaintEventHandler(this.RoundedPanel_Paint);
					this.Resize +=new EventHandler(RoundedPanel_Resize);
				}

			#endregion

		#endregion
		
		#region -  Properties  -

			#region -  Image  -

				private Image mImage;
				/// <summary>
				/// The image displayed on the button that 
				/// is used to help the user identify
				/// it's function if the text is ambiguous.
				/// </summary>
				[Category("Image"), 
				 DefaultValue(null),
				 Description("The image displayed on the button that " +
							 "is used to help the user identify" + 
							 "it's function if the text is ambiguous.")]
				public Image Image
				{
					get { return mImage; }
					set { mImage = value; this.InvalidateEx(); }
				}

				private ContentAlignment mImageAlign = ContentAlignment.MiddleLeft;
				/// <summary>
				/// The alignment of the image 
				/// in relation to the button.
				/// </summary>
				[Category("Image"), 
				 DefaultValue(typeof(ContentAlignment),"MiddleLeft"),
				 Description("The alignment of the image " + 
							 "in relation to the button.")]
				public ContentAlignment ImageAlign
				{
					get { return mImageAlign; }
					set { mImageAlign = value; this.InvalidateEx(); }
				}

				private Size mImageSize = new Size(24,24);
				/// <summary>
				/// The size of the image to be displayed on the
				/// button. This property defaults to 24x24.
				/// </summary>
				[Category("Image"), 
				 DefaultValue(typeof(Size),"24, 24"),
				 Description("The size of the image to be displayed on the" + 
							 "button. This property defaults to 24x24.")]
				public Size ImageSize
				{
					get { return mImageSize; }
					set { mImageSize = value; this.InvalidateEx(); }
				}
	
			#endregion

			#region -  Appearance  -
							
				private int mCornerRadius = 6;
				/// <summary>
				/// The radius for the button corners. The 
				/// greater this value is, the more 'smooth' 
				/// the corners are. This property should
				///  not be greater than half of the 
				///  controls height.
				/// </summary>
				[Category("Appearance"), 
				 DefaultValue(6),
				 Description("The radius for the button corners. The " +
							 "greater this value is, the more 'smooth' " +
							 "the corners are. This property should " +
							 "not be greater than half of the " +
							 "controls height.")]
				public int CornerRadius
				{
					get { return mCornerRadius; }
					set { mCornerRadius = value; this.InvalidateEx(); }
				}

				private Color mHighlightColor = Color.White;
				/// <summary>
				/// The colour of the highlight on the top of the button.
				/// </summary>
				[Category("Appearance"), 
				 DefaultValue(typeof(Color), "White"),
				 Description("The colour of the highlight on the top of the button.")]
				public Color HighlightColor
				{
					get { return mHighlightColor; }
					set { mHighlightColor = value; this.InvalidateEx(); }
				}

				private Color mButtonColor = Color.Black;
				/// <summary>
				/// The bottom color of the button that 
				/// will be drawn over the base color.
				/// </summary>
				[Category("Appearance"), 
				 DefaultValue(typeof(Color), "Black"), 
				 Description("The bottom color of the button that " + 
							 "will be drawn over the base color.")]
				public Color ButtonColor
				{
					get { return mButtonColor; }
					set { mButtonColor = value; this.InvalidateEx(); }
				}

				private Image mBackImage;
				/// <summary>
				/// The background image for the button, 
				/// this image is drawn over the base 
				/// color of the button.
				/// </summary>
				[Category("Appearance"), 
				 DefaultValue(null), 
				 Description("The background image for the button, " + 
							 "this image is drawn over the base " + 
							 "color of the button.")]
				public Image BackImage
				{
					get { return mBackImage; }
					set { mBackImage = value; this.InvalidateEx(); }
				}

				private Color mBaseColor = Color.Black;
				/// <summary>
				/// The backing color that the rest of 
				/// the button is drawn. For a glassier 
				/// effect set this property to Transparent.
				/// </summary>
				[Category("Appearance"), 
				 DefaultValue(typeof(Color), "Black"), 
				 Description("The backing color that the rest of" + 
							 "the button is drawn. For a glassier " + 
							 "effect set this property to Transparent.")]
				public Color BaseColor
				{
					get { return mBaseColor; }
					set { mBaseColor = value; this.InvalidateEx(); }
				}

				private Color mDropShadowColor = Color.FromArgb(90, 0, 0, 0);
				/// <summary>
				/// The bottom color of the button that 
				/// will be drawn over the base color.
				/// </summary>
				[Category("Appearance"), 
				DefaultValue(typeof(Color), "90, 0, 0, 0"), 
				Description("The drop shadow color of the button that " + 
					"will be drawn behind the right and bottom sides.")]
				public Color DropShadowColor
				{
					get { return mDropShadowColor; }
					set { mDropShadowColor = value; this.InvalidateEx(); }
				}


			#endregion

		#endregion

		#region -  Functions  -

			private GraphicsPath RoundRect(RectangleF r, float r1, float r2, float r3, float r4)
			{
				float x = r.X, y = r.Y, w = r.Width, h = r.Height;
				GraphicsPath rr = new GraphicsPath();
				rr.AddBezier(x, y + r1, x, y, x + r1, y, x + r1, y);
				rr.AddLine(x + r1, y, x + w - r2, y);
				rr.AddBezier(x + w - r2, y, x + w, y, x + w, y + r2, x + w, y + r2);
				rr.AddLine(x + w, y + r2, x + w, y + h - r3);
				rr.AddBezier(x + w, y + h - r3, x + w, y + h, x + w - r3, y + h, x + w - r3, y + h);
				rr.AddLine(x + w - r3, y + h, x + r4, y + h);
				rr.AddBezier(x + r4, y + h, x, y + h, x, y + h - r4, x, y + h - r4);
				rr.AddLine(x, y + h - r4, x, y + r1);
				return rr;
			}

		#endregion

		#region -  Drawing  -

			/// <summary>
			/// Draws the drop shadow border for the control
			/// </summary>
			/// <param name="g">The graphics object used in the paint event.</param>
			private void DrawDropShadow(Graphics g)
			{
				Rectangle r = this.ClientRectangle;

				//g.FillRectangle(Brushes.CornflowerBlue, r);

				//r.Width -= 1; r.Height -= 1;
				//r.Offset(1, 1);
				//r.Inflate(-1, -1);
				float radius = CornerRadius + 2.0f;
				using (GraphicsPath rr = RoundRect(r, radius, radius, radius, radius))
				{
					//using (Pen p = new Pen(Color.FromArgb(90, 0, 0, 0)))
					using (Brush br = new SolidBrush(mDropShadowColor))
					{
						g.FillPath(br, rr);
					}
				}
			}

			/// <summary>
			/// Draws the outer border for the control
			/// using the ButtonColor property.
			/// </summary>
			/// <param name="g">The graphics object used in the paint event.</param>
			private void DrawOuterStroke(Graphics g)
			{
				Rectangle r = this.ClientRectangle;
				r.Width -= 2; r.Height -= 2;
				using (GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius))
				{
					using (Pen p = new Pen(Color.FromArgb(64, this.ButtonColor)))
					{
						g.DrawPath(p, rr);
					}
				}
			}

			/// <summary>
			/// Draws the inner border for the control
			/// using the HighlightColor property.
			/// </summary>
			/// <param name="g">The graphics object used in the paint event.</param>
			private void DrawInnerStroke(Graphics g)
			{
				Rectangle r = this.ClientRectangle;
				r.Width -= 2; r.Height -= 2;
				r.Inflate(-2, -2);
				using (GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius))
				{
					using (Pen p = new Pen(this.HighlightColor))
					{
						g.DrawPath(p, rr);
					}
				}
			}

			/// <summary>
			/// Draws the background for the control
			/// using the background image and the 
			/// BaseColor.
			/// </summary>
			/// <param name="g">The graphics object used in the paint event.</param>
			private void DrawBackground(Graphics g)
			{
				int alpha = 255;
				Rectangle r = this.ClientRectangle;
				r.Inflate(-1, -1);
				r.Width -= 1; r.Height -= 1;
				using (GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius))
				{
					using (SolidBrush sb = new SolidBrush(this.BaseColor))
					{
						g.FillPath(sb, rr);
					}
					if (this.BackImage != null)
					{
						SetClip(g);
						g.DrawImage(this.BackImage, this.ClientRectangle);
						g.ResetClip();
					}
					using (SolidBrush sb = new SolidBrush(Color.FromArgb(alpha, this.ButtonColor)))
					{
						g.FillPath(sb, rr);
					}
				}
			}

			/// <summary>
			/// Draws the Highlight over the top of the
			/// control using the HightlightColor.
			/// </summary>
			/// <param name="g">The graphics object used in the paint event.</param>
			private void DrawHighlight(Graphics g)
			{
				int alpha = 150;
				Rectangle rect = new Rectangle(0, 0, this.Width-2, this.Height / 2);
				using (GraphicsPath r = RoundRect(rect, CornerRadius, CornerRadius, 0, 0))
				{
					RectangleF rectBounds = r.GetBounds();
					rectBounds.Height += 1.0f;
					using (LinearGradientBrush lg = new LinearGradientBrush(rectBounds, 
								Color.FromArgb(alpha, this.HighlightColor),
								Color.FromArgb(alpha / 3, this.HighlightColor), 
								LinearGradientMode.Vertical))
					{
						g.FillPath(lg, r);
					}
				}
			}

			/// <summary>
			/// Draws the image for the button
			/// </summary>
			/// <param name="g">The graphics object used in the paint event.</param>
			private void DrawImage(Graphics g)
			{
				if (this.Image == null) {return;}
				Rectangle r = new Rectangle(8,8,this.ImageSize.Width,this.ImageSize.Height);
				switch (this.ImageAlign)
				{
					case ContentAlignment.TopCenter:
						r = new Rectangle(this.Width / 2 - this.ImageSize.Width / 2,8,this.ImageSize.Width,this.ImageSize.Height);
						break;
					case ContentAlignment.TopRight:
						r = new Rectangle(this.Width - 8 - this.ImageSize.Width,8,this.ImageSize.Width,this.ImageSize.Height);
						break;
					case ContentAlignment.MiddleLeft:
						r = new Rectangle(8,this.Height / 2 - this.ImageSize.Height / 2,this.ImageSize.Width,this.ImageSize.Height);
						break;
					case ContentAlignment.MiddleCenter:
						r = new Rectangle(this.Width / 2 - this.ImageSize.Width / 2,this.Height / 2 - this.ImageSize.Height / 2,this.ImageSize.Width,this.ImageSize.Height);
						break;
					case ContentAlignment.MiddleRight:
						r = new Rectangle(this.Width - 8 - this.ImageSize.Width,this.Height / 2 - this.ImageSize.Height / 2,this.ImageSize.Width,this.ImageSize.Height);
						break;
					case ContentAlignment.BottomLeft:
						r = new Rectangle(8,this.Height - 8 - this.ImageSize.Height,this.ImageSize.Width,this.ImageSize.Height);
						break;
					case ContentAlignment.BottomCenter:
						r = new Rectangle(this.Width / 2 - this.ImageSize.Width / 2,this.Height - 8 - this.ImageSize.Height,this.ImageSize.Width,this.ImageSize.Height);
						break;
					case ContentAlignment.BottomRight:
						r = new Rectangle(this.Width - 8 - this.ImageSize.Width,this.Height - 8 - this.ImageSize.Height,this.ImageSize.Width,this.ImageSize.Height);
						break;
				}
				g.DrawImage(this.Image,r);
			}

			private void SetClip(Graphics g)
			{
				Rectangle r = this.ClientRectangle;
				r.X++; r.Y++; r.Width-=3; r.Height-=3;
				using (GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius))
				{
					g.SetClip(rr);
				}		
			}

		private void InvalidateEx()
		{
			if (this.Parent == null)
				return;
			Rectangle rc = new Rectangle(this.Location, this.Size);
			this.Parent.Invalidate(rc, true);
		}

		#endregion

		#region -  Private Subs  -

			private void RoundedPanel_Paint(object sender, PaintEventArgs e)
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				DrawDropShadow(e.Graphics);
				DrawBackground(e.Graphics);
				DrawHighlight(e.Graphics);
				DrawImage(e.Graphics);
				DrawOuterStroke(e.Graphics);
				//DrawInnerStroke(e.Graphics);
			}

			private void RoundedPanel_Resize(object sender, EventArgs e)
			{
				Rectangle r = this.ClientRectangle;
				//r.X -= 1; r.Y -= 1;
				//r.Width += 2; r.Height += 2;
				//r.Width -= 2; r.Height -= 2;
				using (GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius))
				{
					this.Region = new Region(rr);
				}
			}

		#endregion

	}
}