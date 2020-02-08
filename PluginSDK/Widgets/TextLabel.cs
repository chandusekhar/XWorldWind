namespace WorldWind.Widgets
{
    public enum Alignment
    {
        Left,
        Center,
        Right
    }

	/// <summary>
	/// Summary description for TextLabel.
	/// </summary>
	public class TextLabel : IWidget
	{
		string m_Text = "";
		System.Drawing.Point m_Location = new System.Drawing.Point(0,0);
		System.Drawing.Size m_Size = new System.Drawing.Size(0,20);
		bool m_Visible = true;
		bool m_Enabled = true;
		IWidget m_ParentWidget;
		object m_Tag;
		System.Drawing.Color m_ForeColor = System.Drawing.Color.White;
		string m_Name = "";
        System.Drawing.Font m_localFont;
        Font m_drawingFont = null;

        public Alignment Alignment = Alignment.Left;
        public bool WordBreak = false;

        /// <summary>
        /// CountHeight property value
        /// </summary>
        protected bool m_countHeight = true;

        /// <summary>
        /// CountWidth property value
        /// </summary>
        protected bool m_countWidth = true;

		public TextLabel()
		{
			
		}
		
		#region Properties
        public System.Drawing.Font Font
        {
            get { return this.m_localFont; }
            set
            {
                this.m_localFont = value;
                if (this.m_drawingFont != null)
                {
                    this.m_drawingFont.Dispose();
                    this.m_drawingFont = new Font(DrawArgs.Device, this.m_localFont);
                }
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
		public System.Drawing.Color ForeColor
		{
			get
			{
				return this.m_ForeColor;
			}
			set
			{
                this.m_ForeColor = value;
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
                this.m_Visible = value;
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
				// TODO:  Add TextLabel.ChildWidgets getter implementation
				return null;
			}
			set
			{
				// TODO:  Add TextLabel.ChildWidgets setter implementation
			}
		}

		public System.Drawing.Size ClientSize
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

		public System.Drawing.Point ClientLocation
		{
			get
			{
				return this.m_Location;
			}
			set
			{
                this.m_Location = value;
			}
		}

		public System.Drawing.Point AbsoluteLocation
		{
			get
			{
				if(this.m_ParentWidget != null)
				{
					return new System.Drawing.Point(this.m_Location.X + this.m_ParentWidget.AbsoluteLocation.X, this.m_Location.Y + this.m_ParentWidget.AbsoluteLocation.Y);
					
				}
				else
				{
					return this.m_Location;
				}
			}
		}


        // New IWidget properties

        /// <summary>
        /// Location of this widget relative to the client area of the parent
        /// </summary>
        public System.Drawing.Point Location
        {
            get { return this.m_Location; }
            set { this.m_Location = value; }
        }

        /// <summary>
        /// Size of widget in pixels
        /// </summary>
        public System.Drawing.Size WidgetSize
        {
            get { return this.m_Size; }
            set { this.m_Size = value; }
        }


        /// <summary>
        /// Whether this widget should count for height calculations - HACK until we do real layout
        /// </summary>
        public bool CountHeight
        {
            get { return this.m_countHeight; }
            set { this.m_countHeight = value; }
        }


        /// <summary>
        /// Whether this widget should count for width calculations - HACK until we do real layout
        /// </summary>
        public bool CountWidth
        {
            get { return this.m_countWidth; }
            set { this.m_countWidth = value; }
        }

        public void Initialize(DrawArgs drawArgs)
        {
        }

		public void Render(DrawArgs drawArgs)
		{
			if(this.m_Visible)
			{
                if (this.m_localFont != null && this.m_drawingFont == null)
                {
                    this.m_drawingFont = new Font(drawArgs.device, this.m_localFont);
                }

                DrawTextFormat drawTextFormat = (this.WordBreak ? DrawTextFormat.WordBreak : DrawTextFormat.SingleLine);

                switch(this.Alignment)
                {
                    case Alignment.Left:
                        drawTextFormat |= DrawTextFormat.Left;
                        break;
                    case Alignment.Center:
                        drawTextFormat |= DrawTextFormat.Center;
                        break;
                    case Alignment.Right:
                        drawTextFormat |= DrawTextFormat.Right;
                        break;
                }

                if (this.m_drawingFont == null)
                {
                    drawArgs.defaultDrawingFont.DrawText(
                        null, this.m_Text,
                        new System.Drawing.Rectangle(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.m_Size.Width, this.m_Size.Height),
                        drawTextFormat, this.m_ForeColor);
                }
                else
                {
                    this.m_drawingFont.DrawText(
                        null, this.m_Text,
                        new System.Drawing.Rectangle(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.m_Size.Width, this.m_Size.Height),
                        drawTextFormat, this.m_ForeColor);
                }
			}
				
		}

		#endregion
	}
}