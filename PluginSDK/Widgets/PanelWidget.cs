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

using System;
using System.Drawing;

namespace WorldWind.Widgets
{
	/// <summary>
	/// PanelWidget - This class implements a basic panel with no layout management whatsoever.  
	/// PanelWidget can take any widget as a child.  This widget is used to group other widgets
	/// as a single widget.  
	/// 
	/// No scroll bars are created as this widget doesn't clip subwidgets.  If
	/// you specify a child widget to be outside the panel space it renders outside the panel.
	/// 
	/// Panels cannot be closed, resized or moved (dragged).
	/// </summary>
	public class PanelWidget : WidgetCollection, IWidget, IInteractive
	{
		#region Protected Members

		#region IWidget support variables

		/// <summary>
		/// Name property value
		/// </summary>
		protected string m_name = "";

		/// <summary>
		/// Location property value
		/// </summary>
		protected Point m_location = new Point(0,0);

		/// <summary>
		/// ClientLocation property value
		/// </summary>
		protected Point m_clientLocation = new Point(0,23);

		/// <summary>
		/// WidgetSize property value
		/// </summary>
		protected Size m_size = new Size(200, 300);

		/// <summary>
		/// ClientSize property value
		/// </summary>
		protected Size m_clientSize = new Size(300,177);

		/// <summary>
		/// Visible property value
		/// </summary>
		protected bool m_visible = true;

		/// <summary>
		/// Enabled property value
		/// </summary>
		protected bool m_enabled = true;

		/// <summary>
		/// CountHeight property value
		/// </summary>
		protected bool m_countHeight;

		/// <summary>
		/// CountWidth property value
		/// </summary>
		protected bool m_countWidth;

		/// <summary>
		/// Parent widget property value
		/// </summary>
		protected IWidget m_parentWidget;

		/// <summary>
		/// ChildWidget property value
		/// </summary>
		protected IWidgetCollection m_ChildWidgets = new WidgetCollection();

		/// <summary>
		/// Tag property
		/// </summary>
		protected object m_tag;

		/// <summary>
		/// Flag indicating if initialization is required
		/// </summary>
		protected bool m_isInitialized;

		#endregion

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

		#region Color Values

		/// <summary>
		/// Background color
		/// </summary>
		protected Color m_BackgroundColor = Color.FromArgb(
			170,
			40,
			40,
			40);

		/// <summary>
		/// Border Color
		/// </summary>
		protected Color m_BorderColor = Color.GhostWhite;

		/// <summary>
		/// Header Background Color
		/// </summary>
		protected Color m_HeaderColor = Color.FromArgb(
			170,
//		96,
			Color.DarkKhaki.R,
			Color.DarkKhaki.G,
			Color.DarkKhaki.B);

		/// <summary>
		/// Text color
		/// </summary>
		protected Color m_TextColor = Color.GhostWhite;

		#endregion

		/// <summary>
		/// Height of title bar
		/// </summary>
		protected int m_headerHeight = 23;
		protected int m_currHeaderHeight;

		protected int m_leftPadding = 2;
		protected int m_rightPadding = 1;
		protected int m_topPadding = 2;
		protected int m_bottomPadding = 1;

		/// <summary>
		/// Whether or not to render the body.
		/// </summary>
		protected bool m_renderBody = true;

		protected SharpDX.Direct3D9.Font m_TextFont; 
		protected SharpDX.Direct3D9.Font m_TitleFont; 
		protected SharpDX.Direct3D9.Font m_wingdingsFont;
		protected SharpDX.Direct3D9.Font m_worldwinddingsFont;

		protected Vector2[] m_OutlineVertsHeader = new Vector2[5];
		protected Vector2[] m_OutlineVerts = new Vector2[5];

		/// <summary>
		/// Last point where the mouse was clicked (mousedown).
		/// </summary>
		protected Point m_LastMousePosition = new Point(0,0);

		/// <summary>
		/// Last time the mouse clicked on this widget (header area mostly) - used to implement double click
		/// </summary>
		protected DateTime m_LastClickTime;

		#endregion

		#region Public Members

		/// <summary>
		/// The text to render when the body is hidden
		/// </summary>
		public string Text = "";

		/// <summary>
		/// Whether or not to ever render the header
		/// </summary>
		public bool HeaderEnabled = true;

		#endregion 

		#region Properties

		public SharpDX.Direct3D9.Font TextFont
		{
			get { return this.m_TextFont; }
			set { this.m_TextFont = value; }
		}

		public Color HeaderColor
		{
			get { return this.m_HeaderColor; }
			set { this.m_HeaderColor = value; }
		}

		public int HeaderHeight
		{
			get { return this.m_headerHeight; }
			set { this.m_headerHeight = value; }
		}

		public Color BorderColor
		{
			get { return this.m_BorderColor; }
			set { this.m_BorderColor = value; }
		}

		public Color BackgroundColor
		{
			get { return this.m_BackgroundColor; }
			set { this.m_BackgroundColor = value; }
		}


		/// <summary>
		/// The top edge of this widget.
		/// </summary>
		public int Top
		{
			get
			{
				if (this.HeaderEnabled)
					return this.AbsoluteLocation.Y;
				else
					return this.AbsoluteLocation.Y + this.m_currHeaderHeight;
			}
		}


		/// <summary>
		/// The bottom edge of this widget
		/// </summary>
		public int Bottom
		{
			get 
			{
				if (this.m_renderBody)
					return this.AbsoluteLocation.Y + this.m_size.Height;
				else
					return this.AbsoluteLocation.Y + this.m_currHeaderHeight;
			}
		}


		/// <summary>
		/// The left edge of this widget
		/// </summary>
		public int Left
		{
			get
			{
				return this.AbsoluteLocation.X;
			}
		}


		/// <summary>
		/// The right edge of this widget
		/// </summary>
		public int Right
		{
			get
			{
				return this.AbsoluteLocation.X + this.m_size.Width;
			}
		}


		/// <summary>
		/// Location within the form of where the client area is
		/// </summary>
		public Point BodyLocation
		{
			get
			{
				Point bodyLocation;
				bodyLocation = this.AbsoluteLocation;
				if (this.HeaderEnabled)
					bodyLocation.Y += this.m_currHeaderHeight;
				return bodyLocation;
			}
		}


		#endregion


		/// <summary>
		/// Form Widget Constructor
		/// </summary>
		/// <param name="name">Name of this form.  Name is displayed in header.</param>
		public PanelWidget(string name)
		{
            this.m_name = name;
		}

		/// <summary>
		/// Adds a new child widget
		/// </summary>
		/// <param name="widget">The widget to be added</param>
		new public void Add(IWidget widget)
		{
            this.m_ChildWidgets.Add(widget);
			widget.ParentWidget = this;
		}

		/// <summary>
		/// Removes a child widget
		/// </summary>
		/// <param name="widget">The widget to be removed</param>
		new public void Remove(IWidget widget)
		{
            this.m_ChildWidgets.Remove(widget);
		}

		/// <summary>
		/// Try to clean up everything.
		/// </summary>
		public void Dispose()
		{
			if(this.m_ChildWidgets != null)
			{
				for(int i = 0; i < this.m_ChildWidgets.Count; i++)
				{
					// get rid of child widget
				}

                this.m_ChildWidgets.Clear();
			}

            this.m_isInitialized = false;
		}

		/// <summary>
		/// Computes the height and width of children as laid out.  This value is
		/// used to determine if scrolling is required.
		/// 
		/// HACK - Uses the fields CountHeight and CountWidth in the child widgets 
		/// to determine if they should be counted in the total height/width.
		/// </summary>
		/// <param name="childrenHeight">The total children height.</param>
		/// <param name="childrenWidth">The total children width</param>
		protected void getChildrenSize(out int childrenHeight, out int childrenWidth)
		{
			childrenHeight = 0;
			childrenWidth = 0;

			int biggestHeight = 0;
			int biggestWidth = 0;

			for(int i = 0; i < this.m_ChildWidgets.Count; i++)
			{
				if (this.m_ChildWidgets[i].CountHeight)
					childrenHeight += this.m_ChildWidgets[i].WidgetSize.Height;

				if (this.m_ChildWidgets[i].CountWidth)
					childrenWidth += this.m_ChildWidgets[i].WidgetSize.Width;

				if (this.m_ChildWidgets[i].WidgetSize.Height > biggestHeight)
					biggestHeight = this.m_ChildWidgets[i].WidgetSize.Height;

				if (this.m_ChildWidgets[i].WidgetSize.Width > biggestWidth)
					biggestWidth = this.m_ChildWidgets[i].WidgetSize.Width;
			}
			if (biggestHeight > childrenHeight)
				childrenHeight = biggestHeight;

			if (biggestWidth > childrenWidth)
				childrenWidth = biggestWidth;
		}

		#region IWidget Members

		#region Properties

		/// <summary>
		/// Name of this widget
		/// </summary>
		public string Name
		{
			get { return this.m_name; }
			set { this.m_name = value; }
		}


		/// <summary>
		/// Location of this widget relative to the client area of the parent
		/// </summary>
		public Point Location
		{
			get { return this.m_location; }
			set { this.m_location = value; }
		}


		/// <summary>
		/// Where this widget is on the window
		/// </summary>
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


		/// <summary>
		/// The top left corner of this widget's client area offset by scrolling.
		/// This area is is masked by the ViewPort so objects outside the client
		/// area is clipped and not shown.
		/// </summary>
		public Point ClientLocation
		{
			get
			{
                this.m_clientLocation = this.BodyLocation;
				return this.m_clientLocation;
			}
		}


		/// <summary>
		/// Size of widget in pixels
		/// </summary>
		public Size WidgetSize
		{
			get { return this.m_size; }
			set { this.m_size = value; }
		}


		/// <summary>
		/// Size of the client area in pixels.  This area is the 
		/// widget area minus header and scrollbar areas.
		/// </summary>
		public Size ClientSize
		{
			get 
			{
                this.m_clientSize = this.m_size;

				// deduct header height
                this.m_clientSize.Height -= this.m_currHeaderHeight;

				return this.m_clientSize; 
			}
			set 
			{
                this.m_size = value;
                this.m_size.Height += this.m_currHeaderHeight; 
			}
		}


		/// <summary>
		/// Whether this widget is enabled
		/// </summary>
		public bool Enabled
		{
			get { return this.m_enabled; }
			set { this.m_enabled = value; }
		}


		/// <summary>
		/// Whether this widget is visible
		/// </summary>
		public bool Visible
		{
			get { return this.m_visible; }
			set { this.m_visible = value; }
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


		/// <summary>
		/// The parent widget of this widget.
		/// </summary>
		public IWidget ParentWidget
		{
			get { return this.m_parentWidget; }
			set { this.m_parentWidget = value; }
		}


		/// <summary>
		/// List of children widgets - None in the case of button widgets.
		/// </summary>
		public IWidgetCollection ChildWidgets
		{
			get { return this.m_ChildWidgets; }
			set { this.m_ChildWidgets = value; }
		}


		/// <summary>
		/// A link to an object.
		/// </summary>
		public object Tag
		{
			get { return this.m_tag; }
			set { this.m_tag = value; }
		}


		#endregion

		#region Methods

		/// <summary>
		/// Initializes the button by loading the texture, creating the sprite and figure out the scaling.
		/// 
		/// Called on the GUI thread.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		public void Initialize(DrawArgs drawArgs)
		{
			if(!this.m_enabled)
				return;

			if (this.m_TitleFont == null)
			{
				Font localHeaderFont = new Font("Arial", 12.0f, FontStyle.Italic | FontStyle.Bold);
                this.m_TitleFont = new SharpDX.Direct3D9.Font(drawArgs.device, localHeaderFont);

				Font wingdings = new Font("Wingdings", 12.0f);
                this.m_wingdingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, wingdings);

				Font worldwinddings = new Font("World Wind dings", 12.0f);
                this.m_worldwinddingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, worldwinddings);
			}

            this.m_TextFont = drawArgs.defaultDrawingFont;

            this.m_isInitialized = true;
		}


		/// <summary>
		/// The render method to draw this widget on the screen.
		/// 
		/// Called on the GUI thread.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		public void Render(DrawArgs drawArgs)
		{
			if ((!this.m_visible) || (!this.m_enabled))
				return;

			if (!this.m_isInitialized)
			{
                this.Initialize(drawArgs);
			}

			int widgetTop = this.Top;
			int widgetBottom = this.Bottom;
			int widgetLeft = this.Left;
			int widgetRight = this.Right;

            this.m_currHeaderHeight = 0;

			#region Header Rendering

			// If we should render the header
			if(this.HeaderEnabled)
			{
                this.m_currHeaderHeight = this.m_headerHeight;

				WidgetUtilities.DrawBox(
					this.AbsoluteLocation.X,
					this.AbsoluteLocation.Y, this.m_size.Width, this.m_currHeaderHeight,
					0.0f, this.m_HeaderColor.ToArgb(),
					drawArgs.device);

				Rectangle nameBounds = this.m_TitleFont.MeasureString(
					null, this.m_name,
					DrawTextFormat.None,
					0);

				int widthLeft = this.m_size.Width;

                this.m_TitleFont.DrawText(
					null, this.m_name,
					new Rectangle(this.AbsoluteLocation.X + 2, this.AbsoluteLocation.Y + 2, widthLeft, this.m_currHeaderHeight),
					DrawTextFormat.None, this.m_TextColor.ToArgb());


				// if we don't render the body add whatever is in the text field as annotation
				if (!this.m_renderBody)
				{

					widthLeft -= nameBounds.Width + 10;
					if (widthLeft > 20)
					{
                        this.m_TextFont.DrawText(
							null, this.Text,
							new Rectangle(this.AbsoluteLocation.X + 10 + nameBounds.Width, this.AbsoluteLocation.Y + 3, widthLeft, this.m_currHeaderHeight),
							DrawTextFormat.None, this.m_TextColor.ToArgb());
					}
				}

				// Render border
                this.m_OutlineVertsHeader[0].X = this.AbsoluteLocation.X;
                this.m_OutlineVertsHeader[0].Y = this.AbsoluteLocation.Y;

                this.m_OutlineVertsHeader[1].X = this.AbsoluteLocation.X + this.m_size.Width;
                this.m_OutlineVertsHeader[1].Y = this.AbsoluteLocation.Y;

                this.m_OutlineVertsHeader[2].X = this.AbsoluteLocation.X + this.m_size.Width;
                this.m_OutlineVertsHeader[2].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

                this.m_OutlineVertsHeader[3].X = this.AbsoluteLocation.X;
                this.m_OutlineVertsHeader[3].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

                this.m_OutlineVertsHeader[4].X = this.AbsoluteLocation.X;
                this.m_OutlineVertsHeader[4].Y = this.AbsoluteLocation.Y;

				WidgetUtilities.DrawLine(this.m_OutlineVertsHeader, this.m_BorderColor.ToArgb(), drawArgs.device);
				
			}

			#endregion
			
			#region Body Rendering

			if (this.m_renderBody)
			{
				
				// Draw the interior background
				WidgetUtilities.DrawBox(
					this.AbsoluteLocation.X,
					this.AbsoluteLocation.Y + this.m_currHeaderHeight, this.m_size.Width, this.m_size.Height - this.m_currHeaderHeight,
					0.0f, this.m_BackgroundColor.ToArgb(),
					drawArgs.device);

				int childrenHeight = 0;
				int childrenWidth = 0;

				int bodyHeight = this.m_size.Height - this.m_currHeaderHeight;
				int bodyWidth = this.m_size.Width;

                this.getChildrenSize(out childrenHeight, out childrenWidth);

				// Render each child widget

				int bodyLeft = this.BodyLocation.X;
				int bodyRight = this.BodyLocation.X + this.ClientSize.Width;
				int bodyTop = this.BodyLocation.Y;
				int bodyBottom = this.BodyLocation.Y + this.ClientSize.Height;
				int childLeft = 0;
				int childRight = 0;
				int childTop = 0;
				int childBottom = 0;

				for(int index = this.m_ChildWidgets.Count - 1; index >= 0; index--)
				{
					IWidget currentChildWidget = this.m_ChildWidgets[index] as IWidget;
					if(currentChildWidget != null)
					{
						if(currentChildWidget.ParentWidget == null || currentChildWidget.ParentWidget != this)
						{
							currentChildWidget.ParentWidget = this;
						}
						Point childLocation = currentChildWidget.AbsoluteLocation;

						// if any portion is visible try to render
						childLeft = childLocation.X;
						childRight = childLocation.X + currentChildWidget.WidgetSize.Width;
						childTop = childLocation.Y;
						childBottom = childLocation.Y + currentChildWidget.WidgetSize.Height;

						if ( ( ( (childLeft >= bodyLeft) && (childLeft <= bodyRight) ) ||
							( (childRight >= bodyLeft) && (childRight <= bodyRight) ) ||
							( (childLeft <= bodyLeft) && (childRight >= bodyRight) ) ) 
							&&
							( ( (childTop >= bodyTop) && (childTop <= bodyBottom) ) ||
							( (childBottom >= bodyTop) && (childBottom <= bodyBottom) ) ||
							( (childTop <= bodyTop) && (childBottom >= bodyBottom) ) )  
							)
						{
							currentChildWidget.Visible = true;
							currentChildWidget.Render(drawArgs);
						}
						else
							currentChildWidget.Visible = false;
					}
				}

                this.m_OutlineVerts[0].X = this.AbsoluteLocation.X;
                this.m_OutlineVerts[0].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

                this.m_OutlineVerts[1].X = this.AbsoluteLocation.X + this.m_size.Width;
                this.m_OutlineVerts[1].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

                this.m_OutlineVerts[2].X = this.AbsoluteLocation.X + this.m_size.Width;
                this.m_OutlineVerts[2].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                this.m_OutlineVerts[3].X = this.AbsoluteLocation.X;
                this.m_OutlineVerts[3].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                this.m_OutlineVerts[4].X = this.AbsoluteLocation.X;
                this.m_OutlineVerts[4].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

				WidgetUtilities.DrawLine(this.m_OutlineVerts, this.m_BorderColor.ToArgb(), drawArgs.device);
			}

			#endregion
		}


		#endregion

		#endregion

		#region IInteractive Members

		#region Properties

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

		#region Methods

		/// <summary>
		/// Mouse down event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			// Whether or not the event was handled
			bool handled = false;

			// Whether or not we're in the form
			bool inClientArea = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			int widgetTop = this.Top;
			int widgetBottom = this.Bottom;
			int widgetLeft = this.Left;
			int widgetRight = this.Right;

			// if we're in the client area bring to front
			if(e.X >= widgetLeft &&
				e.X <= widgetRight &&
				e.Y >= widgetTop &&
				e.Y <= widgetBottom)
			{
				if (this.m_parentWidget != null) this.m_parentWidget.ChildWidgets.BringToFront(this);

				inClientArea = true;
			}

			// if its the left mouse button check for UI actions (resize, drags, etc) 
			if(e.Button == System.Windows.Forms.MouseButtons.Left)
			{

				#region header dbl click

					// Check for header double click (if its shown)
				if (this.HeaderEnabled &&
                    e.X >= this.m_location.X &&
                    e.X <= this.AbsoluteLocation.X + this.m_size.Width &&
                    e.Y >= this.AbsoluteLocation.Y &&
                    e.Y <= this.AbsoluteLocation.Y + this.m_currHeaderHeight)
				{
					if (DateTime.Now > this.m_LastClickTime.AddSeconds(0.5))
					{
						handled = true;
					}
					else
					{
                        this.m_renderBody = !this.m_renderBody;
					}

                    this.m_LastClickTime = DateTime.Now;

				}

				#endregion
			}

			// Store the current position
            this.m_LastMousePosition = new Point(e.X, e.Y);

			// If we aren't handling this then let the children try if they are rendered
			if(!handled && inClientArea && this.m_renderBody)
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

			// If we resized or inside the form then consider it handled anyway.
			if(inClientArea)
			{
				handled = true;
			}

			return handled;			 
		}


		/// <summary>
		/// Mouse up event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			int widgetTop = this.Top;
			int widgetBottom = this.Bottom;
			int widgetLeft = this.Left;
			int widgetRight = this.Right;

			// if we're in the client area handle let the children try 
			if(e.X >= widgetLeft &&
				e.X <= widgetRight &&
				e.Y >= widgetTop &&
				e.Y <= widgetBottom)
			{
				for(int i = 0; i < this.m_ChildWidgets.Count; i++)
				{
					if(this.m_ChildWidgets[i] is IInteractive)
					{
						IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
						if (currentInteractive.OnMouseUp(e))
							return true;
					}
				}
			}

			return false;
		}


		/// <summary>
		/// Mouse move event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			int deltaX = e.X - this.m_LastMousePosition.X;
			int deltaY = e.Y - this.m_LastMousePosition.Y;

            this.m_LastMousePosition = new Point(e.X, e.Y);

			int widgetTop = this.Top;
			int widgetBottom = this.Bottom;
			int widgetLeft = this.Left;
			int widgetRight = this.Right;

			// Handle each child if we're in the client area
			if(e.X >= widgetLeft &&
				e.X <= widgetRight &&
				e.Y >= widgetTop &&
				e.Y <= widgetBottom)
			{
				for(int i = 0; i < this.m_ChildWidgets.Count; i++)
				{
					if(this.m_ChildWidgets[i] is IInteractive)
					{
						IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
						
						if (currentInteractive.OnMouseMove(e))
							return true;
					}
				}
			}

			return false;
		}


		/// <summary>
		/// Mouse wheel event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			int widgetTop = this.Top;
			int widgetBottom = this.Bottom;
			int widgetLeft = this.Left;
			int widgetRight = this.Right;

			// Handle each child if we're in the client area
			if(e.X >= widgetLeft &&
				e.X <= widgetRight &&
				e.Y >= widgetTop &&
				e.Y <= widgetBottom)
			{
				for(int i = 0; i < this.m_ChildWidgets.Count; i++)
				{
					if(this.m_ChildWidgets[i] is IInteractive)
					{
						IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
						
						if (currentInteractive.OnMouseWheel(e))
							return true;
					}
				}
			}

			return false;
		}


		/// <summary>
		/// Mouse entered this widget event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseEnter(EventArgs e)
		{
			return false;
		}


		/// <summary>
		/// Mouse left this widget event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseLeave(EventArgs e)
		{
			return false;
		}


		/// <summary>
		/// Key down event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			return false;
		}


		/// <summary>
		/// Key up event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Key press event handler.
		/// This widget does nothing with key presses.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public bool OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			return false;
		}


		#endregion

		#endregion
	}
}
