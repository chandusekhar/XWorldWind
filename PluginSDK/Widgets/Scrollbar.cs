namespace WorldWind.Widgets
{
    public class Scrollbar : IWidget
    {
        System.Drawing.Point m_Location = new System.Drawing.Point(0,0);
		System.Drawing.Size m_Size = new System.Drawing.Size(0,20);
		bool m_Visible = true;
		bool m_Enabled = true;
		IWidget m_ParentWidget;
		object m_Tag;
		System.Drawing.Color m_ForeColor = System.Drawing.Color.White;
		string m_Name = "";

        public bool Outline = true;
        public double Value = 0;
        public double Minimum = 0;
        public double Maximum = 1;

        /// <summary>
        /// CountHeight property value
        /// </summary>
        protected bool m_countHeight = true;

        /// <summary>
        /// CountWidth property value
        /// </summary>
        protected bool m_countWidth = true;

        public Scrollbar()
		{
			
		}
		
		#region Properties
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

        SharpDX.Vector2[] m_outlinePoints = new SharpDX.Vector2[5];

		public void Render(DrawArgs drawArgs)
        {
            if(this.m_Visible)
			{
                float percent = 0;
                if (this.Value < this.Minimum)
                    percent = 0;
                else if (this.Value > this.Maximum)
                    percent = 1.0f;
                else
                {
                    percent = (float)((this.Value - this.Minimum) / (this.Maximum - this.Minimum));
                }

                if (this.Outline)
                {
                    this.m_outlinePoints[0].X = this.AbsoluteLocation.X;
                    this.m_outlinePoints[0].Y = this.AbsoluteLocation.Y;

                    this.m_outlinePoints[1].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                    this.m_outlinePoints[1].Y = this.AbsoluteLocation.Y;

                    this.m_outlinePoints[2].X = this.AbsoluteLocation.X + this.ClientSize.Width;
                    this.m_outlinePoints[2].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;

                    this.m_outlinePoints[3].X = this.AbsoluteLocation.X;
                    this.m_outlinePoints[3].Y = this.AbsoluteLocation.Y + this.ClientSize.Height;

                    this.m_outlinePoints[4].X = this.AbsoluteLocation.X;
                    this.m_outlinePoints[4].Y = this.AbsoluteLocation.Y;
                    
                    WidgetUtilities.DrawLine(this.m_outlinePoints, this.m_ForeColor.ToArgb(), drawArgs.device);
                }

                WidgetUtilities.DrawBox(this.AbsoluteLocation.X, this.AbsoluteLocation.Y,
                    (int)(percent * this.ClientSize.Width), this.ClientSize.Height,
                    0.5f, this.m_ForeColor.ToArgb(),
                    drawArgs.device);
			}
				
		}

		#endregion
    }
}
