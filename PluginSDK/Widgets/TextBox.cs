using System;

namespace WorldWind.Widgets
{
	/// <summary>
	/// Summary description for TextLabel.
	/// </summary>
	public class TextBox : IWidget, IInteractive
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
		System.Drawing.Point m_LastMouseClickPosition = System.Drawing.Point.Empty;

        /// <summary>
        /// CountHeight property value
        /// </summary>
        protected bool m_countHeight = true;

        /// <summary>
        /// CountWidth property value
        /// </summary>
        protected bool m_countWidth = true;

        #region IInteractive support variables

        /// <summary>
        /// LeftClickAction value - holds method to call on left mouse click
        /// </summary>
        protected MouseClickAction m_leftClickAction;

        /// <summary>
        /// RightClickAction value - holds method to call on right mouse click
        /// </summary>
        protected MouseClickAction m_rightClickAction;

        #endregion

		public TextBox()
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


		int m_SelectionStart = -1;
		int m_SelectionEnd = -1;
		int m_CaretPos = -1;

		bool m_RecalculateCaretPos;

		public void Render(DrawArgs drawArgs)
		{
			if(this.m_Visible)
			{
				string displayText = this.m_Text;
				string caretText = "|";

				if(this.m_MouseDownPosition != System.Drawing.Point.Empty)
				{
					int startX = (this.m_LastMousePosition.X >= this.m_MouseDownPosition.X ? this.m_MouseDownPosition.X : this.m_LastMousePosition.X);
					int endX = (this.m_LastMousePosition.X < this.m_MouseDownPosition.X ? this.m_MouseDownPosition.X : this.m_LastMousePosition.X);

					int prevWidth = 0;
					bool startXFound = false;
					bool endXFound = false;
					for(int i = 1; i <= displayText.Length; i++)
					{
						System.Drawing.Rectangle rect = drawArgs.defaultDrawingFont.MeasureString(
							null,
							displayText.Substring(0, i).Replace(" ", "I"),
							DrawTextFormat.None, this.m_ForeColor);

						if(!startXFound && startX <= rect.Width)
						{
							startX = prevWidth;
                            this.m_SelectionStart = i - 1;
							startXFound = true;
						}

						if(!endXFound && endX <= rect.Width)
						{
							endX = prevWidth;
                            this.m_SelectionEnd = i - 1;
							endXFound = true;
						}

						if(startXFound && endXFound)
							break;

						prevWidth = rect.Width;
					}

					if(!endXFound)
					{
                        this.m_SelectionEnd = displayText.Length;
						endX = prevWidth;
					}

					WidgetUtilities.DrawBox(this.AbsoluteLocation.X + startX, this.AbsoluteLocation.Y,
						endX - startX,
						this.ClientSize.Height, 0.0f, System.Drawing.Color.FromArgb(200,200,200,200).ToArgb(),
						drawArgs.device);
				}

				drawArgs.defaultDrawingFont.DrawText(
					null, this.m_Text,
					new System.Drawing.Rectangle(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.m_Size.Width, this.m_Size.Height),
					DrawTextFormat.NoClip, this.m_ForeColor);

				if(DateTime.Now.Millisecond < 500)
				{
					string space = " W";

					System.Drawing.Rectangle spaceRect = drawArgs.defaultDrawingFont.MeasureString(
						null,
						space,
						DrawTextFormat.None, this.m_ForeColor);

					space = "W";

					System.Drawing.Rectangle spaceRect1 = drawArgs.defaultDrawingFont.MeasureString(
						null,
						space,
						DrawTextFormat.None, this.m_ForeColor);

					int spaceWidth = spaceRect.Width - spaceRect1.Width;

					if(this.m_RecalculateCaretPos)
					{
						if(this.m_LastMouseClickPosition == System.Drawing.Point.Empty)
                            this.m_CaretPos = displayText.Length;
						else if(displayText.Length == 0)
                            this.m_CaretPos = 0;
						else
						{
							for(int i = 1; i < displayText.Length; i++)
							{
								System.Drawing.Rectangle rect = drawArgs.defaultDrawingFont.MeasureString(
									null,
									displayText.Substring(0, i).Replace(" ", "i"),
									DrawTextFormat.None, this.m_ForeColor);

								if(this.m_LastMouseClickPosition.X <= rect.Width)
								{
                                    this.m_CaretPos = i - 1;
									break;
								}
							}

                            this.m_RecalculateCaretPos = false;
						}
					}


					if(this.m_CaretPos >= 0)
					{
						System.Drawing.Rectangle caretRect = drawArgs.defaultDrawingFont.MeasureString(
							null,
							caretText,
							DrawTextFormat.None, this.m_ForeColor);

						System.Drawing.Rectangle textRect = drawArgs.defaultDrawingFont.MeasureString(
							null,
							displayText.Substring(0, this.m_CaretPos),
							DrawTextFormat.None, this.m_ForeColor);

						int caretOffset = 0;
						if(this.m_CaretPos != 0 && this.m_CaretPos == displayText.Length && displayText[displayText.Length - 1] == ' ')
							caretOffset = spaceWidth;
						else if(this.m_CaretPos < displayText.Length && this.m_CaretPos > 0 && displayText[this.m_CaretPos - 1] == ' ')
							caretOffset = spaceWidth;

						drawArgs.defaultDrawingFont.DrawText(
							null,
							caretText,
							new System.Drawing.Rectangle(this.AbsoluteLocation.X + textRect.Width - caretRect.Width / 2 + caretOffset, this.AbsoluteLocation.Y, this.m_Size.Width, this.m_Size.Height),
							DrawTextFormat.NoClip,
							System.Drawing.Color.Cyan);//m_ForeColor);
					}
				}
			}
				
		}

		#endregion

		#region IInteractive Members

		public bool OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			if(this.m_MouseDownPosition != System.Drawing.Point.Empty)
			{
				return true;
			}
			return false;
		}

		public bool OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			if(this.m_MouseDownPosition != System.Drawing.Point.Empty)
			{
				return true;
			}
			return false;
		}

		public bool OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			if(this.m_MouseDownPosition != System.Drawing.Point.Empty || this.m_LastMouseClickPosition != System.Drawing.Point.Empty)
			{
				switch(e.KeyChar)
				{
					case '\b':
						if(this.m_MouseDownPosition != System.Drawing.Point.Empty &&
							this.m_SelectionStart != this.m_SelectionEnd)
						{
							this.Text = this.Text.Remove(this.m_SelectionStart, this.m_SelectionEnd - this.m_SelectionStart);
                            this.m_CaretPos = this.m_SelectionStart;
                            this.m_MouseDownPosition = System.Drawing.Point.Empty;
						}
						else if(this.m_CaretPos > 0)
						{
							this.Text = this.Text.Remove(this.m_CaretPos - 1, 1);
                            this.m_CaretPos--;

						}
						
						break;
					default:
						this.Text += e.KeyChar;
						break;
				
				}
				return true;
			}
			return false;
		}

		System.Drawing.Point m_MouseDownPosition = System.Drawing.Point.Empty;
		System.Drawing.Point m_LastMousePosition = System.Drawing.Point.Empty;

		public bool OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == System.Windows.Forms.MouseButtons.Left && this.IsInClientArea(e))
			{
                this.m_MouseDownPosition = new System.Drawing.Point(e.X - this.AbsoluteLocation.X, e.Y - this.AbsoluteLocation.Y);
				this.m_LastMousePosition = this.m_MouseDownPosition;
                this.m_RecalculateCaretPos = true;
				return true;
			}
			else
			{
                this.m_MouseDownPosition = System.Drawing.Point.Empty;
                this.m_RecalculateCaretPos = true;
				return false;
			}
		}

		public bool OnMouseEnter(EventArgs e)
		{
			// TODO:  Add TextBox.OnMouseEnter implementation
			return false;
		}

		public bool OnMouseLeave(EventArgs e)
		{
			// TODO:  Add TextBox.OnMouseLeave implementation
			return false;
		}

		public bool OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == System.Windows.Forms.MouseButtons.Left && this.m_MouseDownPosition != System.Drawing.Point.Empty)
			{
                this.m_LastMousePosition = new System.Drawing.Point(e.X - this.AbsoluteLocation.X, e.Y - this.AbsoluteLocation.Y);
                this.m_RecalculateCaretPos = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool IsInClientArea(System.Drawing.Point p)
		{
			if(p.X >= this.AbsoluteLocation.X &&
				p.X <= this.AbsoluteLocation.X + this.ClientSize.Width &&
				p.Y >= this.AbsoluteLocation.Y &&
				p.Y <= this.AbsoluteLocation.Y + this.ClientSize.Height)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool IsInClientArea(System.Windows.Forms.MouseEventArgs e)
		{
			if(e.X >= this.AbsoluteLocation.X &&
				e.X <= this.AbsoluteLocation.X + this.ClientSize.Width &&
				e.Y >= this.AbsoluteLocation.Y &&
				e.Y <= this.AbsoluteLocation.Y + this.ClientSize.Height)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			if(this.IsInClientArea(e))
			{
				this.m_LastMouseClickPosition = new System.Drawing.Point(
					e.X - this.AbsoluteLocation.X,
					e.Y - this.AbsoluteLocation.Y);
                this.m_RecalculateCaretPos = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			// TODO:  Add TextBox.OnMouseWheel implementation
			return false;
		}


        // TODO: Refactor so that URL click is done on LeftClickAction
        // and OnMouseUp (or Down) the relevant Left and Right click actions
        // are called.

        /// <summary>
        /// Action to perform when the left mouse button is clicked
        /// </summary>
        public MouseClickAction LeftClickAction
        {
            get { return this.m_leftClickAction; }
            set { this.m_leftClickAction = value; }
        }


        /// <summary>
        /// Action to perform when the right mouse button is clicked
        /// </summary>
        public MouseClickAction RightClickAction
        {
            get { return this.m_rightClickAction; }
            set { this.m_rightClickAction = value; }
        }	


		#endregion
	}
}