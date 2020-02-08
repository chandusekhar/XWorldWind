namespace WorldWind
{
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

		public TextLabel()
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

		public void Render(DrawArgs drawArgs)
		{
			if(this.m_Visible)
			{

				drawArgs.defaultDrawingFont.DrawText(
					null, this.m_Text,
					new System.Drawing.Rectangle(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.m_Size.Width, this.m_Size.Height),
					DrawTextFormat.NoClip, this.m_ForeColor);
			}
				
		}

		#endregion
	}
}