using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace WorldWind
{
    public delegate void VisibleChangedHandler(object o, bool state);

	public class Form : IWidget, IInteractive
	{
		Point m_Location = new Point(0, 0);
		Size m_Size = new Size(300, 200);
		IWidget m_ParentWidget;
		IWidgetCollection m_ChildWidgets = new WidgetCollection();
		string m_Name = "";
        Alignment m_alignment = Alignment.None;

		Color m_BackgroundColor = Color.FromArgb(
			100, 0, 0, 0);

		bool m_HideBorder;
		Color m_BorderColor = Color.GhostWhite;
		Color m_HeaderColor =  Color.FromArgb(
			120,
			Color.Coral.R,
			Color.Coral.G,
			Color.Coral.B);
		
		int m_HeaderHeight = 20;

		Color m_TextColor = Color.GhostWhite;
		SharpDX.Direct3D9.Font m_WorldWindDingsFont;
		SharpDX.Direct3D9.Font m_TextFont;

        bool m_HideHeader;
		bool m_AutoHideHeader;
		bool m_Visible = true;
		bool m_Enabled = true;
		object m_Tag;
		string m_Text = "";
		
		public Form()
		{
		}

		#region Properties
        public Alignment Alignment
        {
            get { return this.m_alignment; }
            set { this.m_alignment = value; }
        }
		public bool HideBorder
		{
			get{ return this.m_HideBorder; }
			set{ this.m_HideBorder = value; }
		}
        public bool HideHeader
        {
            get { return this.m_HideHeader; }
            set { this.m_HideHeader = value; }
        }

		public SharpDX.Direct3D9.Font TextFont
		{
			get
			{
				return this.m_TextFont;
			}
			set
			{
                this.m_TextFont = value;
			}
		}
		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
                this.m_Name = value;
			}
		}
		public bool AutoHideHeader
		{
			get
			{
				return this.m_AutoHideHeader;
			}
			set
			{
                this.m_AutoHideHeader = value;
			}
		}
		public Color HeaderColor
		{
			get
			{
				return this.m_HeaderColor;
			}
			set
			{
                this.m_HeaderColor = value;
			}
		}
		public int HeaderHeight
		{
			get
			{
				return this.m_HeaderHeight;
			}
			set
			{
                this.m_HeaderHeight = value;
			}
		}
		public Color BorderColor
		{
			get
			{
				return this.m_BorderColor;
			}
			set
			{
                this.m_BorderColor = value;
			}
		}
		public Color BackgroundColor
		{
			get
			{
				return this.m_BackgroundColor;
			}
			set
			{
                this.m_BackgroundColor = value;
			}
		}

		public string Text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
                this.m_Text = value;
			}
		}
		#endregion

		#region IWidget Members

		public IWidget ParentWidget
		{
			get
			{
				return this.m_ParentWidget;
			}
			set
			{
                this.m_ParentWidget = value;
			}
		}

		public bool Visible
		{
			get
			{
				return this.m_Visible;
			}
			set
			{
				if(this.m_Visible != value)
				{
                    this.m_Visible = value;
					if(this.OnVisibleChanged != null) this.OnVisibleChanged(this, value);
				}
			}
		}

		public object Tag
		{
			get
			{
				return this.m_Tag;
			}
			set
			{
                this.m_Tag = value;
			}
		}

		public IWidgetCollection ChildWidgets
		{
			get
			{
				return this.m_ChildWidgets;
			}
			set
			{
                this.m_ChildWidgets = value;
			}
		}

		public Size ClientSize
		{
			get
			{
				return this.m_Size;
			}
			set
			{
                this.m_Size = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
                this.m_Enabled = value;
			}
		}

		public Point ClientLocation
		{
			get
			{
                Point location = this.m_Location;

                if (this.m_ParentWidget != null)
                {
                    if (this.m_alignment == Alignment.Right || this.m_alignment == (Alignment.Top | Alignment.Right) || this.m_alignment == (Alignment.Bottom | Alignment.Right))
                    {
                        location.X = this.m_ParentWidget.ClientSize.Width - this.ClientSize.Width - location.X;
                    }
                    if (this.m_alignment == Alignment.Bottom || this.m_alignment == (Alignment.Left | Alignment.Bottom) || this.m_alignment == (Alignment.Right | Alignment.Bottom))
                    {
                        location.Y = this.m_ParentWidget.ClientSize.Height - this.ClientSize.Height - location.Y;
                    }
                }
                return location;
			}
			set
			{
                this.m_Location = value;
			}
		}

		public Point AbsoluteLocation
		{
			get
			{
				if(this.m_ParentWidget != null)
				{
					return new Point(this.ClientLocation.X + this.m_ParentWidget.AbsoluteLocation.X, this.ClientLocation.Y + this.m_ParentWidget.AbsoluteLocation.Y);
				}
				else
				{
					return this.ClientLocation;		
				}
			}
		}

		[DllImport("gdi32.dll")]
		static extern int AddFontResource(string lpszFilename);

		int resizeBuffer = 5;

		public virtual void Render(DrawArgs drawArgs)
		{
			if(!this.Visible)
				return;

			if(this.m_TextFont == null)
			{
				Font localHeaderFont = new Font("Arial", 12.0f, FontStyle.Italic | FontStyle.Bold);
                this.m_TextFont = new SharpDX.Direct3D9.Font(drawArgs.device, localHeaderFont);
			}

			if(this.m_WorldWindDingsFont == null)
			{
				AddFontResource(Path.Combine(System.Windows.Forms.Application.StartupPath, "World Wind Dings 1.04.ttf"));
				System.Drawing.Text.PrivateFontCollection fpc = new System.Drawing.Text.PrivateFontCollection();
				fpc.AddFontFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "World Wind Dings 1.04.ttf"));
				Font worldwinddings = new Font(fpc.Families[0], 12.0f);

                this.m_WorldWindDingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, worldwinddings);
			}

			if(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer)
			{
				DrawArgs.MouseCursor = CursorType.SizeNWSE;
			}
			else if(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer)
			{
				DrawArgs.MouseCursor = CursorType.SizeNESW;
			}
			else if(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer + this.ClientSize.Height &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height)
			{
				DrawArgs.MouseCursor = CursorType.SizeNESW;
			}
			else if(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer + this.ClientSize.Height &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height)
			{
				DrawArgs.MouseCursor = CursorType.SizeNWSE;
			}
			else if(
				(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height) ||
				(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height))
			{
				DrawArgs.MouseCursor = CursorType.SizeWE;
			}
			else if(
				(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer) ||
				(DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y - this.resizeBuffer + this.ClientSize.Height &&
				DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height))
			{
				DrawArgs.MouseCursor = CursorType.SizeNS;
			}

			if(this.ClientSize.Height > drawArgs.parentControl.Height)
			{
                this.ClientSize = new Size(this.ClientSize.Width, drawArgs.parentControl.Height);
			}

			if(this.ClientSize.Width > drawArgs.parentControl.Width)
			{
                this.ClientSize = new Size(drawArgs.parentControl.Width, this.ClientSize.Height);
			}

            if (!this.m_HideHeader && 
                (!this.m_AutoHideHeader || (DrawArgs.LastMousePosition.X >= this.ClientLocation.X &&
                                            DrawArgs.LastMousePosition.X <= this.ClientLocation.X + this.m_Size.Width &&
                                            DrawArgs.LastMousePosition.Y >= this.ClientLocation.Y &&
                                            DrawArgs.LastMousePosition.Y <= this.ClientLocation.Y + this.m_Size.Height)))
			{
				
				Utilities.DrawBox(this.ClientLocation.X, this.ClientLocation.Y, this.m_Size.Width, this.m_HeaderHeight,
					0.0f, this.m_HeaderColor.ToArgb(),
					drawArgs.device);

                this.m_TextFont.DrawText(
					null, this.m_Text,
                    new Rectangle(this.ClientLocation.X + 2, this.ClientLocation.Y, this.m_Size.Width, this.m_HeaderHeight),
					DrawTextFormat.None, this.m_TextColor.ToArgb());

                this.m_WorldWindDingsFont.DrawText(
					null,
					"E",
                    new Rectangle(this.ClientLocation.X + this.m_Size.Width - 15, this.ClientLocation.Y + 2, this.m_Size.Width, this.m_Size.Height),
					DrawTextFormat.NoClip,
					Color.White.ToArgb());

                this.m_OutlineVertsHeader[0].X = this.AbsoluteLocation.X;
                this.m_OutlineVertsHeader[0].Y = this.AbsoluteLocation.Y;

                this.m_OutlineVertsHeader[1].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                this.m_OutlineVertsHeader[1].Y = this.AbsoluteLocation.Y;

                this.m_OutlineVertsHeader[2].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                this.m_OutlineVertsHeader[2].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;

                this.m_OutlineVertsHeader[3].X = this.AbsoluteLocation.X;
                this.m_OutlineVertsHeader[3].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;

                this.m_OutlineVertsHeader[4].X = this.AbsoluteLocation.X;
                this.m_OutlineVertsHeader[4].Y = this.AbsoluteLocation.Y;

				if(!this.m_HideBorder)
					Utilities.DrawLine(this.m_OutlineVertsHeader, this.m_BorderColor.ToArgb(), drawArgs.device);

			}

			Utilities.DrawBox(this.ClientLocation.X, this.ClientLocation.Y + this.m_HeaderHeight, this.m_Size.Width, this.m_Size.Height - this.m_HeaderHeight,
				0.0f, this.m_BackgroundColor.ToArgb(),
				drawArgs.device);
			
			for(int index = this.m_ChildWidgets.Count - 1; index >= 0; index--)
			{
				IWidget currentChildWidget = this.m_ChildWidgets[index] as IWidget;
				if(currentChildWidget != null)
				{
					if(currentChildWidget.ParentWidget == null || currentChildWidget.ParentWidget != this)
					{
						currentChildWidget.ParentWidget = this;
					}
					currentChildWidget.Render(drawArgs);
				}
			}

            this.m_OutlineVerts[0].X = this.AbsoluteLocation.X;
            this.m_OutlineVerts[0].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;

            this.m_OutlineVerts[1].X = this.AbsoluteLocation.X + this.ClientSize.Width;
            this.m_OutlineVerts[1].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;

            this.m_OutlineVerts[2].X = this.AbsoluteLocation.X + this.ClientSize.Width;
            this.m_OutlineVerts[2].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;

            this.m_OutlineVerts[3].X = this.AbsoluteLocation.X;
            this.m_OutlineVerts[3].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;

            this.m_OutlineVerts[4].X = this.AbsoluteLocation.X;
            this.m_OutlineVerts[4].Y = this.AbsoluteLocation.Y + this.m_HeaderHeight;

			if(!this.m_HideBorder)
				Utilities.DrawLine(this.m_OutlineVerts, this.m_BorderColor.ToArgb(), drawArgs.device);			
		}

		private Vector2[] m_OutlineVerts = new Vector2[5];
		private Vector2[] m_OutlineVertsHeader = new Vector2[5];

		#endregion

		bool m_IsDragging;
		Point m_LastMousePosition = new Point(0,0);

		bool isResizingLeft;
		bool isResizingRight;
		bool isResizingBottom;
		bool isResizingTop;
		bool isResizingUL;
		bool isResizingUR;
		bool isResizingLL;
		bool isResizingLR;

		#region IInteractive Members

		public bool OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
            if (!this.Visible)
                return false;

			bool handled = false;

			bool inClientArea = false;

            if (e.X >= this.ClientLocation.X &&
                e.X <= this.ClientLocation.X + this.m_Size.Width &&
                e.Y >= this.ClientLocation.Y &&
                e.Y <= this.ClientLocation.Y + this.m_Size.Height)
			{
                this.m_ParentWidget.ChildWidgets.BringToFront(this);
				inClientArea = true;
			}

			if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer)
			{
                this.isResizingUL = true;
			}
			else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer + this.ClientSize.Width &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer)
			{
                this.isResizingUR = true;
			}
			else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer + this.ClientSize.Height &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height)
			{
                this.isResizingLL = true;
			}
			else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer + this.ClientSize.Width &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer + this.ClientSize.Height &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height)
			{
                this.isResizingLR = true;
			}
			else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height )
			{
                this.isResizingLeft = true;
			}
			else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer + this.ClientSize.Width &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height)
			{
                this.isResizingRight = true;
			}
			else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer
				)
			{
                this.isResizingTop = true;
			}
			else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
				e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.ClientSize.Width &&
				e.Y > this.AbsoluteLocation.Y - this.resizeBuffer + this.ClientSize.Height &&
				e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.ClientSize.Height)
			{
                this.isResizingBottom = true;
			}
            else if (e.X >= this.ClientLocation.X &&
				e.X <= this.AbsoluteLocation.X + this.ClientSize.Width &&
				e.Y >= this.AbsoluteLocation.Y &&
				e.Y <= this.AbsoluteLocation.Y + this.m_HeaderHeight)
			{
                this.m_IsDragging = true;
				handled = true;
			}

            this.m_LastMousePosition = new Point(e.X, e.Y);

			if(!handled)
			{
				for(int i = 0; i < this.m_ChildWidgets.Count; i++)
				{
					if(!handled)
					{
						if(this.m_ChildWidgets[i] is IInteractive)
						{
							IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
							handled = currentInteractive.OnMouseDown(e);
						}
					}
				}
			}

			if(!handled && inClientArea)
			{
				handled = true;
			}

			return handled;
			 
		}

		public bool OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
            if (!this.Visible)
                return false;

			bool handled = false;
			if(e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if(this.m_IsDragging)
				{
                    this.m_IsDragging = false;
				}
			}

			bool inClientArea = false;

            if (e.X >= this.ClientLocation.X &&
                e.X <= this.ClientLocation.X + this.m_Size.Width &&
                e.Y >= this.ClientLocation.Y &&
                e.Y <= this.ClientLocation.Y + this.m_Size.Height)
			{
				inClientArea = true;
			}

			if(inClientArea)
			{
				if(!this.m_HideHeader && this.isPointInCloseBox(new Point(e.X, e.Y)))
				{
                    this.Visible = false;
					handled = true;
				}
			}

			for(int i = 0; i < this.m_ChildWidgets.Count; i++)
			{
				if(this.m_ChildWidgets[i] is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
					handled = currentInteractive.OnMouseUp(e);
				}
			}

			if(!handled && inClientArea)
			{
				handled = true;
			}

			if(this.isResizingTop)
			{
                this.isResizingTop = false;
			}
			if(this.isResizingBottom)
			{
                this.isResizingBottom = false;
			}
			if(this.isResizingLeft)
			{
                this.isResizingLeft = false;
			}
			if(this.isResizingRight)
			{
                this.isResizingRight = false;
			}
			if(this.isResizingUL)
			{
                this.isResizingUL = false;
			}
			if(this.isResizingUR)
			{
                this.isResizingUR = false;
			}
			if(this.isResizingLL)
			{
                this.isResizingLL = false;
			}
			if(this.isResizingLR)
			{
                this.isResizingLR = false;
			}

			return handled;
		}

		public bool OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			return false;
		}

		public bool OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			return false;
		}

		public bool OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			return false;
		}

		public bool OnMouseEnter(EventArgs e)
		{
			return false;
		}

		public event VisibleChangedHandler OnVisibleChanged;

		Size minSize = new Size(20, 20);

		public bool OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
            if (!this.Visible)
                return false;

			bool handled = false;
			int deltaX = e.X - this.m_LastMousePosition.X;
			int deltaY = e.Y - this.m_LastMousePosition.Y;

			if(this.isResizingTop || this.isResizingUL || this.isResizingUR)
			{
                this.ClientLocation = new Point(this.ClientLocation.X, this.ClientLocation.Y + deltaY);
                this.m_Size.Height -= deltaY;
			}
			else if(this.isResizingBottom || this.isResizingLL || this.isResizingLR)
			{
                this.m_Size.Height += deltaY;
			}
			else if(this.isResizingRight || this.isResizingUR || this.isResizingLR)
			{
                this.m_Size.Width += deltaX;
			}
			else if(this.isResizingLeft || this.isResizingUL || this.isResizingLL)
			{
                this.ClientLocation = new Point(this.ClientLocation.X + deltaX, this.ClientLocation.Y);
                this.m_Size.Width -= deltaX;
			}
			else if(this.m_IsDragging)
			{
                this.ClientLocation = new Point(this.ClientLocation.X + deltaX, this.ClientLocation.Y + deltaY);

                if (this.ClientLocation.X < 0) this.ClientLocation = new Point(0, this.ClientLocation.Y);
                if (this.ClientLocation.Y < 0) this.ClientLocation = new Point(this.ClientLocation.X, 0);
                if (this.ClientLocation.Y + this.m_Size.Height > DrawArgs.ParentControl.Height) this.ClientLocation = new Point(this.ClientLocation.X , DrawArgs.ParentControl.Height - this.m_Size.Height);
                if (this.ClientLocation.X + this.m_Size.Width > DrawArgs.ParentControl.Width) this.ClientLocation = new Point(DrawArgs.ParentControl.Width - this.m_Size.Width, this.ClientLocation.Y);

				handled = true;
			}

			if(this.m_Size.Width < this.minSize.Width)
			{
                this.m_Size.Width = this.minSize.Width;
			}

			if(this.m_Size.Height < this.minSize.Height)
			{
                this.m_Size.Height = this.minSize.Height;
			}

            this.m_LastMousePosition = new Point(e.X, e.Y);

			for(int i = 0; i < this.m_ChildWidgets.Count; i++)
			{
				if(this.m_ChildWidgets[i] is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
					handled = currentInteractive.OnMouseMove(e);
				}
			}

			bool inClientArea = false;
            if (e.X >= this.ClientLocation.X &&
                e.X <= this.ClientLocation.X + this.m_Size.Width &&
                e.Y >= this.ClientLocation.Y &&
                e.Y <= this.ClientLocation.Y + this.m_Size.Height)
			 {
				inClientArea = true;
			 }


			if(!handled && inClientArea)
			{
				handled = true;
			}
			return handled;
		}

		private bool isPointInCloseBox(Point absolutePoint)
		{
			int closeBoxSize = 10;
			int closeBoxYOffset = 2;
			int closeBoxXOffset = this.m_Size.Width - 15;

			if(absolutePoint.X >= this.ClientLocation.X + closeBoxXOffset &&
                absolutePoint.X <= this.ClientLocation.X + closeBoxXOffset + closeBoxSize &&
                absolutePoint.Y >= this.ClientLocation.Y + closeBoxYOffset &&
                absolutePoint.Y <= this.ClientLocation.Y + closeBoxYOffset + closeBoxSize)
			{
				return true;
			}
			
			return false;
		}

		public bool OnMouseLeave(EventArgs e)
		{
			return false;
		}

		public bool OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			return false;
		}

		#endregion
	}

    public enum Alignment
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }
}
