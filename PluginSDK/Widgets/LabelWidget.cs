//========================= (UNCLASSIFIED) ==============================
// Copyright © 2005-2006 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2006 The Johns Hopkins University. 
//
// This software was developed at The Johns Hopkins University/Applied 
// Physics Laboratory (“JHU/APL”) that is the author thereof under the 
// “work made for hire” provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the “Software”), to use the 
// Software without restriction, including without limitation the rights 
// to copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit others to do so, subject to the 
// following conditions: 
//
// 1.  This LICENSE AND DISCLAIMER, including the copyright notice, shall 
//     be included in all copies of the Software, including copies of 
//     substantial portions of the Software; 
//
// 2.  JHU/APL assumes no obligation to provide support of any kind with 
//     regard to the Software.  This includes no obligation to provide 
//     assistance in using the Software nor to provide updated versions of 
//     the Software; and 
//
// 3.  THE SOFTWARE AND ITS DOCUMENTATION ARE PROVIDED AS IS AND WITHOUT 
//     ANY EXPRESS OR IMPLIED WARRANTIES WHATSOEVER.  ALL WARRANTIES 
//     INCLUDING, BUT NOT LIMITED TO, PERFORMANCE, MERCHANTABILITY, FITNESS
//     FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT ARE HEREBY DISCLAIMED.  
//     USERS ASSUME THE ENTIRE RISK AND LIABILITY OF USING THE SOFTWARE.  
//     USERS ARE ADVISED TO TEST THE SOFTWARE THOROUGHLY BEFORE RELYING ON 
//     IT.  IN NO EVENT SHALL THE JOHNS HOPKINS UNIVERSITY BE LIABLE FOR 
//     ANY DAMAGES WHATSOEVER, INCLUDING, WITHOUT LIMITATION, ANY LOST 
//     PROFITS, LOST SAVINGS OR OTHER INCIDENTAL OR CONSEQUENTIAL DAMAGES, 
//     ARISING OUT OF THE USE OR INABILITY TO USE THE SOFTWARE. 
//

using System.Drawing;

namespace WorldWind.Widgets
{
	/// <summary>
	/// Summary description for TextLabel.
	/// </summary>
	public class LabelWidget : IWidget
	{
		string m_Text = "";
		Point m_location = new Point(0,0);
		Size m_size = new Size(0,20);
		bool m_visible = true;
		bool m_enabled = true;
		IWidget m_parentWidget;
		object m_tag;
		Color m_ForeColor = Color.White;
		string m_name = "";
		DrawTextFormat m_Format = DrawTextFormat.NoClip;

		protected int m_borderWidth = 5;

		protected bool m_clearOnRender;

		protected bool m_autoSize = true;

		protected bool m_useParentWidth;

		protected bool m_useParentHeight;

		protected bool m_isInitialized;

		public LabelWidget()
		{
            this.m_location.X = this.m_borderWidth;
            this.m_location.Y = this.m_borderWidth;
		}

		public LabelWidget(string text)
		{
            this.Text = text;
            this.m_location.X = this.m_borderWidth;
            this.m_location.Y = this.m_borderWidth;
		}

        public LabelWidget(string text, Color color, int xLoc, int yLoc, int xSize, int ySize)
        {
            this.Text = text;
            this.m_ForeColor = color;
            this.m_location.X = xLoc;
            this.m_location.Y = yLoc;
            this.m_size = new Size(xSize, ySize);
        }

		#region Properties
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
                this.m_name = value;
			}
		}
		public Color ForeColor
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
                this.m_isInitialized = false;
			}
		}

		public DrawTextFormat Format
		{
			get { return this.m_Format; }
			set { this.m_Format = value; }
		}

		public bool ClearOnRender
		{
			get { return this.m_clearOnRender; }
			set { this.m_clearOnRender = value; }
		}

		public bool AutoSize
		{
			get { return this.m_autoSize; }
			set { this.m_autoSize = value; }
		}

		public bool UseParentWidth
		{
			get { return this.m_useParentWidth; }
			set { this.m_useParentWidth = value; }
		}

		public bool UseParentHeight
		{
			get { return this.m_useParentHeight; }
			set { this.m_useParentHeight = value; }
		}

		#endregion

		#region IWidget Members

		public IWidget ParentWidget
		{
			get
			{
				return this.m_parentWidget;
			}
			set
			{
                this.m_parentWidget = value;
			}
		}

		public bool Visible
		{
			get
			{
				return this.m_visible;
			}
			set
			{
                this.m_visible = value;
			}
		}
		protected bool m_countHeight = true;
		protected bool m_countWidth = true;
		public bool CountHeight
		{
			get { return this.m_countHeight; }
			set { this.m_countHeight = value; }
		}

		public bool CountWidth		
		{
			get { return this.m_countWidth; }
			set { this.m_countWidth = value; }
		}

		public object Tag
		{
			get
			{
				return this.m_tag;
			}
			set
			{
                this.m_tag = value;
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

		public Size ClientSize
		{
			get { return this.m_size; }
			set { this.m_size = value; }
		}

		public Size WidgetSize
		{
			get 
			{ 
				if (this.m_parentWidget != null)
				{
					if (this.m_useParentWidth) this.m_size.Width = this.m_parentWidget.ClientSize.Width - (this.m_borderWidth + this.m_location.X);
					if (this.m_useParentHeight) this.m_size.Height = this.m_parentWidget.ClientSize.Height - (this.m_borderWidth + this.m_location.Y);	
				}
				return this.m_size; 
			}
			set { this.m_size = value; }
		}

		public bool Enabled
		{
			get { return this.m_enabled; }
			set { this.m_enabled = value; }
		}

		public Point Location
		{
			get { return this.m_location; }
			set { this.m_location = value; }
		}

		public Point ClientLocation
		{
			get { return this.AbsoluteLocation; }
		}

		public Point AbsoluteLocation
		{
			get
			{
				if(this.m_parentWidget != null)
				{
					return new Point(this.m_location.X + this.m_parentWidget.ClientLocation.X, this.m_location.Y + this.m_parentWidget.ClientLocation.Y);
					
				}
				else
				{
					return this.m_location;
				}
			}
		}

		public void ComputeAutoSize (DrawArgs drawArgs)
		{
			SharpDX.Direct3D9.Font font = drawArgs.defaultDrawingFont;
			if(font==null)
				font = drawArgs.CreateFont( "", 10 );
			Rectangle bounds = font.MeasureString(null, this.m_Text, this.m_Format, 0);
			if(this.m_useParentWidth)
			{
                this.m_size.Width = this.WidgetSize.Width - this.m_location.X;
                this.m_size.Height = bounds.Height * ( (int)(bounds.Width/ this.m_size.Width) + 1);
			}
			else
			{
                this.m_size.Width = bounds.Width + this.m_borderWidth;
                this.m_size.Height = bounds.Height + this.m_borderWidth;
			}

			if(this.m_useParentHeight) this.m_size.Height = this.WidgetSize.Height - this.m_location.Y;

			// This code is iffy - no idea why Y is offset by more than specified.
			if (this.m_location.X == 0)
			{
                this.m_location.X = this.m_borderWidth;
                this.m_size.Width += this.m_borderWidth;
			}
			if (this.m_location.Y == 0)
			{
                this.m_location.Y = this.m_borderWidth;
                this.m_size.Height += this.m_borderWidth;
			}
		}
			
		public void Initialize(DrawArgs drawArgs)
		{
			if (this.m_autoSize) this.ComputeAutoSize (drawArgs);
            this.m_isInitialized = true;
		}

		public void Render(DrawArgs drawArgs)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return;			

			if (!this.m_isInitialized) this.Initialize(drawArgs);

			drawArgs.defaultDrawingFont.DrawText(
				null, this.m_Text,
				new Rectangle(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.m_size.Width, this.m_size.Height), this.m_Format, this.m_ForeColor);

			if (this.m_clearOnRender)
			{
                this.m_Text = "";
                this.m_isInitialized = false;
			}
		}

		#endregion
	}
}
