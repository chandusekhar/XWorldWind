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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX;

namespace WorldWind.Widgets
{
    public delegate void VisibleChangedHandler(object o, bool state);

	/// <summary>
	/// FormWidget - This class implements a basic form with no layout management whatsoever but
	/// will resize and generate scrollbars.  FormWidget can take any widget as a child including
	/// other form widgets.  Typically all other widgets reside in a form widget.
	/// 
	/// Note:  You can nest form widgets (maybe) but not put form widgets inside other widgets 
	/// because each form widget creates a new ViewPort.  Typically a PanelWidget should work
	/// fine instead.
	/// </summary>
	public class FormWidget : WidgetCollection, IWidget, IInteractive
	{
		/// <summary>
		/// Possible Resize Directions
		/// </summary>
		[Flags]
		public enum ResizeDirection : ushort
		{
			None = 0x00,
			Left = 0x01,
			Right = 0x02,
			Up = 0x04,
			Down = 0x08,
			UL = 0x05,
			UR = 0x06,
			DL = 0x09,
			DR = 0x0A
		}

		#region Protected Members

		#region IWidget support variables

		/// <summary>
		/// Name property value
		/// </summary>
		protected string m_name = "";

		/// <summary>
		/// Location property value
		/// </summary>
		protected System.Drawing.Point m_location = new System.Drawing.Point(0,0);

		/// <summary>
		/// ClientLocation property value
		/// </summary>
		protected System.Drawing.Point m_clientLocation = new System.Drawing.Point(0,23);

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
		protected System.Drawing.Color m_BackgroundColor = System.Drawing.Color.FromArgb(
			170,
			40,
			40,
			40);
//		96,
//		32,
//		32,
//		32);

		/// <summary>
		/// Border Color
		/// </summary>
		protected System.Drawing.Color m_BorderColor = System.Drawing.Color.GhostWhite;

		/// <summary>
		/// Header Background Color
		/// </summary>
		protected System.Drawing.Color m_HeaderColor = System.Drawing.Color.FromArgb(
			170,
//		96,
			System.Drawing.Color.DarkKhaki.R,
			System.Drawing.Color.DarkKhaki.G,
			System.Drawing.Color.DarkKhaki.B);

		/// <summary>
		/// Text color
		/// </summary>
		protected System.Drawing.Color m_TextColor = System.Drawing.Color.GhostWhite;

		#endregion

		/// <summary>
		/// Height of title bar
		/// </summary>
		protected int m_headerHeight = 15;
		protected int m_currHeaderHeight;

		protected int m_leftPadding = 2;
		protected int m_rightPadding = 1;
		protected int m_topPadding = 2;
		protected int m_bottomPadding = 1;

		#region vertical scroll bar members

		/// <summary>
		/// Width of vertical scrollbar
		/// </summary>
		protected int m_scrollbarWidth = 20;

		protected int m_vScrollbarPos;
		protected int m_vScrollbarHeight;
		protected double m_vScrollbarPercent;

		// where we grabbed the scroll bar in a drag
		protected int m_vScrollbarGrabPosition;

		// True if the vertical scroll bar must be visible because the client height is too small
		protected bool m_showVScrollbar;

		// True if we are currently dragging the scroll bar
		protected bool m_isVScrolling;

		#endregion

		#region Horizontal scroll bar members

		/// <summary>
		/// Height of horizontal scrollbar
		/// </summary>
		protected int m_scrollbarHeight = 20;

		protected int m_hScrollbarPos;
		protected int m_hScrollbarWidth;
		protected double m_hScrollbarPercent;

		// where we grabbed the scroll bar in a drag
		protected int m_hScrollbarGrabPosition;

		// True if the horizontal scroll bar must be visible because the client width is too small
		protected bool m_showHScrollbar;

		// True if we are currently dragging the scroll bar
		protected bool m_isHScrolling;

		#endregion

		/// <summary>
		/// Whether or not to render the body.
		/// </summary>
		protected bool m_renderBody = true;

		protected SharpDX.Direct3D9.Font m_TextFont; 
		protected SharpDX.Direct3D9.Font m_TitleFont; 
		protected SharpDX.Direct3D9.Font m_wingdingsFont;
		protected SharpDX.Direct3D9.Font m_worldwinddingsFont;

		/// <summary>
		/// Region around widget that counts for grabbing when trying to resize the widget.
		/// </summary>
		protected int resizeBuffer = 5;

		protected Vector2[] m_OutlineVertsHeader = new Vector2[5];
		protected Vector2[] m_OutlineVerts = new Vector2[5];

		/// <summary>
		/// True if we're dragging the form around
		/// </summary>
		protected bool m_isDragging;

		/// <summary>
		/// Last point where the mouse was clicked (mousedown).
		/// </summary>
		protected System.Drawing.Point m_LastMousePosition = new System.Drawing.Point(0,0);

		/// <summary>
		/// Last time the mouse clicked on this widget (header area mostly) - used to implement double click
		/// </summary>
		protected DateTime m_LastClickTime;

		/// <summary>
		/// Current resizing direction
		/// </summary>
		protected ResizeDirection m_resize = ResizeDirection.None;

		protected int m_distanceFromTop;
		protected int m_distanceFromBottom;
		protected int m_distanceFromLeft;
		protected int m_distanceFromRight;

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

        /// <summary>
        /// Whether or not to ever render the border
        /// </summary>
        public bool BorderEnabled = true;

		/// <summary>
		/// Whether or not to hide the header when form doesn't have focus.
		/// </summary>
		public bool AutoHideHeader = false;

		/// <summary>
		/// Minimum drawing size
		/// </summary>
		public Size MinSize = new Size(20, 60);

		/// <summary>
		/// Flag that indicates whether the user can resize vertically
		/// </summary>
		public bool VerticalResizeEnabled = true;

		/// <summary>
		/// Flag that indicates whether the user can resize horizontally
		/// </summary>
		public bool HorizontalResizeEnabled = true;

		/// <summary>
		/// True if we allow the showing of the vertical scroll bar (clips otherwise)
		/// </summary>
		public bool VerticalScrollbarEnabled = true;

		/// <summary>
		/// True if we allow the showing of the horizontal scroll bar (clips otherwise)
		/// </summary>
		public bool HorizontalScrollbarEnabled = true;

		/// <summary>
		/// Flag that indicates this form should get deleted on close
		/// </summary>
		public bool DestroyOnClose = false;

		public WidgetEnums.AnchorStyles Anchor = WidgetEnums.AnchorStyles.None;

		#endregion 

		#region Properties

		public SharpDX.Direct3D9.Font TextFont
		{
			get { return this.m_TextFont; }
			set { this.m_TextFont = value; }
		}

		public System.Drawing.Color HeaderColor
		{
			get { return this.m_HeaderColor; }
			set { this.m_HeaderColor = value; }
		}

		public int HeaderHeight
		{
			get { return this.m_headerHeight; }
			set { this.m_headerHeight = value; }
		}

		public System.Drawing.Color BorderColor
		{
			get { return this.m_BorderColor; }
			set { this.m_BorderColor = value; }
		}

		public System.Drawing.Color BackgroundColor
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
		public System.Drawing.Point BodyLocation
		{
			get
			{
				System.Drawing.Point bodyLocation;
				bodyLocation = this.AbsoluteLocation;
				if ((this.HeaderEnabled) || (this.AutoHideHeader))
					bodyLocation.Y += this.m_headerHeight;
				return bodyLocation;
			}
		}

		#endregion


		/// <summary>
		/// Form Widget Constructor
		/// </summary>
		/// <param name="name">Name of this form.  Name is displayed in header.</param>
		public FormWidget(string name)
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
		public System.Drawing.Point Location
		{
			get 
			{
				// multiple anchors not supported.
				// ignore top and left anchors
				if ( ((this.Anchor & WidgetEnums.AnchorStyles.Bottom) != 0) && (this.m_parentWidget != null))
				{
					// if the distance has changed then reset the location.
					if (this.m_location.Y - this.m_parentWidget.ClientSize.Height != this.m_distanceFromBottom)
					{
                        this.m_location.Y = this.m_parentWidget.ClientSize.Height - this.m_distanceFromBottom;
					}
				}
				if ( ((this.Anchor & WidgetEnums.AnchorStyles.Right) != 0) && (this.m_parentWidget != null))
				{
					// if the distance has changed then reset the location.
					if (this.m_location.X - this.m_parentWidget.ClientSize.Width != this.m_distanceFromRight)
					{
                        this.m_location.X = this.m_parentWidget.ClientSize.Width - this.m_distanceFromRight;
					}
				}
				return this.m_location; 
			}
			set 
			{
                this.m_location = value;
                this.UpdateLocation();
			}
		}


		/// <summary>
		/// Where this widget is on the window
		/// </summary>
		public System.Drawing.Point AbsoluteLocation
		{
			get
			{
				if(this.m_parentWidget != null)
				{
					return new System.Drawing.Point(this.Location.X + this.m_parentWidget.ClientLocation.X, this.Location.Y + this.m_parentWidget.ClientLocation.Y);
				}
				else
				{
					return this.Location;
				}
			}
		}


		/// <summary>
		/// The top left corner of this widget's client area offset by scrolling.
		/// This area is is masked by the ViewPort so objects outside the client
		/// area is clipped and not shown.
		/// </summary>
		public System.Drawing.Point ClientLocation
		{
			get
			{
                this.m_clientLocation = this.BodyLocation;
				if (this.m_showVScrollbar)
				{
					if (this.m_vScrollbarPercent < .01) this.m_vScrollbarPercent = .01;
                    this.m_clientLocation.Y -= (int) (this.m_vScrollbarPos / this.m_vScrollbarPercent);
				}
				if (this.m_showHScrollbar)
				{
					if (this.m_hScrollbarPercent < .01) this.m_hScrollbarPercent = .01;
                    this.m_clientLocation.X -= (int) (this.m_hScrollbarPos / this.m_hScrollbarPercent);
				}
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

				// if scroll bars deduct those sizes
				if (this.m_showHScrollbar) this.m_clientSize.Height -= this.m_scrollbarHeight;

				if (this.m_showVScrollbar) this.m_clientSize.Width -= this.m_scrollbarWidth;

				return this.m_clientSize; 
			}
            set
            {
                // Reset the client size
                this.m_clientSize = value;

                // Reset the widget size and add back in decoration sizes
                this.m_size = value;
                this.m_size.Height += this.m_currHeaderHeight;

                // Reset the scrollbars.  Should be reset on next render anyway but just in case.
                // TODO Requires testing to see if we get weird behavior.
                this.m_showVScrollbar = false;
                this.m_showHScrollbar = false;
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

        public event VisibleChangedHandler OnVisibleChanged;

		/// <summary>
		/// Whether this widget is visible
		/// </summary>
		public bool Visible
		{
			get { return this.m_visible; }
			set 
            {
                this.m_visible = value;
                if (this.OnVisibleChanged != null) this.OnVisibleChanged(this, value);
            }
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
				Font localHeaderFont = new Font("Arial", 8.0f, FontStyle.Bold);
                this.m_TitleFont = new SharpDX.Direct3D9.Font(drawArgs.device, localHeaderFont);
            }

            if (this.m_wingdingsFont == null)
            {
				Font wingdings = new Font("Wingdings", 12.0f);
                this.m_wingdingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, wingdings);
            }

            if (this.m_worldwinddingsFont == null)
            {
                AddFontResource(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                System.Drawing.Text.PrivateFontCollection fpc = new System.Drawing.Text.PrivateFontCollection();
                fpc.AddFontFile(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                Font m_worldwinddings = new Font(fpc.Families[0], 12.0f);
                this.m_worldwinddingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, m_worldwinddings);
			}

            if (this.m_TextFont == null)
            {
                // m_TextFont = drawArgs.defaultDrawingFont;
                this.m_TextFont = this.m_TitleFont;
            }

            this.UpdateLocation();

            this.m_isInitialized = true;
		}

        [DllImport("gdi32.dll")]
        static extern int AddFontResource(string lpszFilename);


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

			#region Resize Crosshair rendering
            if (this.VerticalResizeEnabled || this.HorizontalResizeEnabled)
            {
                if (DrawArgs.LastMousePosition.X > widgetLeft - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetLeft + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetTop - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetTop + this.resizeBuffer)
                {
                    DrawArgs.MouseCursor = CursorType.Cross;
                }
                else if (DrawArgs.LastMousePosition.X > widgetRight - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetRight + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetTop - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetTop + this.resizeBuffer)
                {
                    DrawArgs.MouseCursor = CursorType.Cross;
                }
                else if (DrawArgs.LastMousePosition.X > widgetLeft - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetLeft + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetBottom - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetBottom + this.resizeBuffer)
                {
                    DrawArgs.MouseCursor = CursorType.Cross;
                }
                else if (DrawArgs.LastMousePosition.X > widgetRight - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetRight + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetBottom - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetBottom + this.resizeBuffer)
                {
                    DrawArgs.MouseCursor = CursorType.Cross;
                }
                else if (
                    (DrawArgs.LastMousePosition.X > widgetLeft - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetLeft + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetTop - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetBottom + this.resizeBuffer) ||
                    (DrawArgs.LastMousePosition.X > widgetRight - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetRight + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetTop - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetBottom + this.resizeBuffer))
                {
                    DrawArgs.MouseCursor = CursorType.SizeWE;
                }
                else if (
                    (DrawArgs.LastMousePosition.X > widgetLeft - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetRight + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetTop - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetTop + this.resizeBuffer) ||
                    (DrawArgs.LastMousePosition.X > widgetLeft - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.X < widgetRight + this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y > widgetBottom - this.resizeBuffer &&
                    DrawArgs.LastMousePosition.Y < widgetBottom + this.resizeBuffer))
                {
                    DrawArgs.MouseCursor = CursorType.SizeNS;
                }
            }
			#endregion

            this.m_currHeaderHeight = 0;

			#region Header Rendering

			// If we should render the header or if we're in the body (when autohide enabled) then render the header
			if( (this.HeaderEnabled && !this.AutoHideHeader) || 
                (this.HeaderEnabled && (this.AutoHideHeader &&
                                        DrawArgs.LastMousePosition.X >= widgetLeft &&
                                        DrawArgs.LastMousePosition.X <= widgetRight &&
                                        DrawArgs.LastMousePosition.Y >= widgetTop &&
                                        DrawArgs.LastMousePosition.Y <= widgetBottom)) ||
                !this.m_renderBody)
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

				int widthLeft = this.m_size.Width - 20;  // account for close box

                this.m_TitleFont.DrawText(
					null, this.m_name,
					new System.Drawing.Rectangle(this.AbsoluteLocation.X + 2, this.AbsoluteLocation.Y + 2, widthLeft, this.m_currHeaderHeight),
					DrawTextFormat.None, this.m_TextColor.ToArgb());


				// if we don't render the body add whatever is in the text field as annotation
				if (!this.m_renderBody)
				{

					widthLeft -= nameBounds.Width + 10;
					if (widthLeft > 20)
					{
                        this.m_TextFont.DrawText(
							null, this.Text,
							new System.Drawing.Rectangle(this.AbsoluteLocation.X + 10 + nameBounds.Width, this.AbsoluteLocation.Y, widthLeft, this.m_currHeaderHeight),
							DrawTextFormat.None, this.m_TextColor.ToArgb());
					}
				}

                this.m_worldwinddingsFont.DrawText(
					null,
					"E",
					new System.Drawing.Rectangle(this.AbsoluteLocation.X + this.m_size.Width - 18, this.AbsoluteLocation.Y, 20, this.m_currHeaderHeight),
					DrawTextFormat.None, this.m_TextColor.ToArgb());

				// Render border
                if (this.BorderEnabled)
                {
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
			}

			#endregion
			
			#region Body Rendering

			if (this.m_renderBody)
			{
				
				// Draw the interior background
				WidgetUtilities.DrawBox(
					this.AbsoluteLocation.X,
					this.AbsoluteLocation.Y + this.m_headerHeight,//m_currHeaderHeight,
                    this.m_size.Width, this.m_size.Height - this.m_headerHeight,//m_currHeaderHeight,
					0.0f, this.m_BackgroundColor.ToArgb(),
					drawArgs.device);
                
				// Render scrollbars
				int childrenHeight = 0;
				int childrenWidth = 0;

				int bodyHeight = this.m_size.Height - this.m_currHeaderHeight;
				int bodyWidth = this.m_size.Width;

                this.getChildrenSize(out childrenHeight, out childrenWidth);

				// reset the scroll bars flag so we can retest if we need them
                this.m_showVScrollbar = false;
                this.m_showHScrollbar = false;

				// if the children are too high turn on the verticle scrollbar
				if ( (childrenHeight > bodyHeight) && (this.VerticalScrollbarEnabled) )
				{
					// deduct the vertical scrollbar width
                    this.m_showVScrollbar = true;
					bodyWidth -= this.m_scrollbarWidth;

					// if the children are too wide turn on the horizontal
					if ( (childrenWidth > bodyWidth) && (this.HorizontalScrollbarEnabled) )
					{
                        this.m_showHScrollbar = true;
						bodyHeight -= this.m_scrollbarHeight;
					}
				}
				else
				{
					// if children are too wide turn on horizontal scrollbar
					if ( (childrenWidth > this.m_size.Width) && (this.HorizontalScrollbarEnabled) )
					{
                        this.m_showHScrollbar = true;
						bodyHeight -= this.m_scrollbarHeight;

						// if the horizontal scrollbar takes up too much room turn on the verticle too
						if ( (childrenHeight > bodyHeight) && (this.VerticalScrollbarEnabled) )
						{
                            this.m_showVScrollbar = true;
							bodyWidth -= this.m_scrollbarWidth;
						}
					}
				}

				// Render verticle scrollbar if there is one
				if (this.m_showVScrollbar)
				{
                    this.m_vScrollbarPercent = (double)bodyHeight/(double)childrenHeight;
                    this.m_vScrollbarHeight = (int)(bodyHeight * this.m_vScrollbarPercent);

					if (this.m_vScrollbarPos < 0)
					{
                        this.m_vScrollbarPos = 0;
					} 
					else if (this.m_vScrollbarPos > bodyHeight - this.m_vScrollbarHeight)
					{
                        this.m_vScrollbarPos = bodyHeight - this.m_vScrollbarHeight;
					}

					int color = (this.m_isVScrolling ? System.Drawing.Color.White.ToArgb() : System.Drawing.Color.Gray.ToArgb());
					WidgetUtilities.DrawBox(this.BodyLocation.X + bodyWidth + 2, this.BodyLocation.Y + this.m_vScrollbarPos + 1, this.m_scrollbarWidth - 3, this.m_vScrollbarHeight - 2,
						0.0f,
						color,
						drawArgs.device);

                    this.m_OutlineVerts[0].X = this.AbsoluteLocation.X + this.m_size.Width - this.m_scrollbarWidth;
                    this.m_OutlineVerts[0].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

                    this.m_OutlineVerts[1].X = this.AbsoluteLocation.X + this.m_size.Width;
                    this.m_OutlineVerts[1].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

                    this.m_OutlineVerts[2].X = this.AbsoluteLocation.X + this.m_size.Width ;
                    this.m_OutlineVerts[2].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                    this.m_OutlineVerts[3].X = this.AbsoluteLocation.X + this.m_size.Width - this.m_scrollbarWidth;
                    this.m_OutlineVerts[3].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                    this.m_OutlineVerts[4].X = this.AbsoluteLocation.X + this.m_size.Width - this.m_scrollbarWidth;
                    this.m_OutlineVerts[4].Y = this.AbsoluteLocation.Y + this.m_currHeaderHeight;

					WidgetUtilities.DrawLine(this.m_OutlineVerts, this.m_BorderColor.ToArgb(), drawArgs.device);

				}
				else
				{
                    this.m_vScrollbarPos = 0;
				}

				if (this.m_showHScrollbar)
				{
                    this.m_hScrollbarPercent = (double)bodyWidth/(double)childrenWidth;
                    this.m_hScrollbarWidth = (int)(bodyWidth * this.m_hScrollbarPercent);

					if (this.m_hScrollbarPos < 0)
					{
                        this.m_hScrollbarPos = 0;
					} 
					else if (this.m_hScrollbarPos > bodyWidth - this.m_hScrollbarWidth)
					{
                        this.m_hScrollbarPos = bodyWidth - this.m_hScrollbarWidth;
					}

					int color = (this.m_isHScrolling ? System.Drawing.Color.White.ToArgb() : System.Drawing.Color.Gray.ToArgb());
					WidgetUtilities.DrawBox(this.BodyLocation.X + this.m_hScrollbarPos + 1, this.BodyLocation.Y + bodyHeight + 2, this.m_hScrollbarWidth - 3, this.m_scrollbarHeight - 2,
						0.0f,
						color,
						drawArgs.device);

                    this.m_OutlineVerts[0].X = this.AbsoluteLocation.X;
                    this.m_OutlineVerts[0].Y = this.AbsoluteLocation.Y + bodyHeight + this.m_currHeaderHeight;

                    this.m_OutlineVerts[1].X = this.AbsoluteLocation.X + this.m_size.Width;
                    this.m_OutlineVerts[1].Y = this.AbsoluteLocation.Y + bodyHeight + this.m_currHeaderHeight;

                    this.m_OutlineVerts[2].X = this.AbsoluteLocation.X + this.m_size.Width;
                    this.m_OutlineVerts[2].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                    this.m_OutlineVerts[3].X = this.AbsoluteLocation.X;
                    this.m_OutlineVerts[3].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                    this.m_OutlineVerts[4].X = this.AbsoluteLocation.X ;
                    this.m_OutlineVerts[4].Y = this.AbsoluteLocation.Y + bodyHeight + this.m_currHeaderHeight;

					WidgetUtilities.DrawLine(this.m_OutlineVerts, this.m_BorderColor.ToArgb(), drawArgs.device);
				}
				else
				{
                    this.m_hScrollbarPos = 0;
				}

				// Render each child widget

				// create the client viewport to clip child objects
				Viewport clientViewPort = new Viewport();

				clientViewPort.X = this.BodyLocation.X;
				clientViewPort.Y = this.BodyLocation.Y;

				clientViewPort.Width = this.ClientSize.Width;
				clientViewPort.Height = this.ClientSize.Height;

				if (this.m_parentWidget != null)
				{
					if (this.BodyLocation.X + this.ClientSize.Width > this.m_parentWidget.ClientSize.Width + this.m_parentWidget.ClientLocation.X)
						clientViewPort.Width = (this.m_parentWidget.ClientSize.Width + this.m_parentWidget.ClientLocation.X) - this.BodyLocation.X;

					if (this.BodyLocation.Y + this.ClientSize.Height > this.m_parentWidget.ClientSize.Height + this.m_parentWidget.ClientLocation.Y)
						clientViewPort.Height = (this.m_parentWidget.ClientSize.Height + this.m_parentWidget.ClientLocation.Y) - this.BodyLocation.Y;
				}

				// save the original viewport
				Viewport defaultViewPort = drawArgs.device.Viewport;

				// replace with client viewport
				drawArgs.device.Viewport = clientViewPort;

				int bodyLeft = this.BodyLocation.X;
				int bodyRight = this.BodyLocation.X + this.ClientSize.Width;
				int bodyTop = this.BodyLocation.Y;
				int bodyBottom = this.BodyLocation.Y + this.ClientSize.Height;
				int childLeft = 0;
				int childRight = 0;
				int childTop = 0;
				int childBottom = 0;

				//for(int index = m_ChildWidgets.Count - 1; index >= 0; index--)
                for (int index = 0; index < this.m_ChildWidgets.Count;  index++)
                {
                    IWidget currentChildWidget = this.m_ChildWidgets[index] as IWidget;
                    if (currentChildWidget != null)
                    {
                        if (currentChildWidget.ParentWidget == null || currentChildWidget.ParentWidget != this)
                        {
                            currentChildWidget.ParentWidget = this;
                        }
                        System.Drawing.Point childLocation = currentChildWidget.AbsoluteLocation;

                        // if any portion is visible try to render
                        childLeft = childLocation.X;
                        childRight = childLocation.X + currentChildWidget.WidgetSize.Width;
                        childTop = childLocation.Y;
                        childBottom = childLocation.Y + currentChildWidget.WidgetSize.Height;

                        if (currentChildWidget.Visible &&
                            (((childLeft >= bodyLeft) && (childLeft <= bodyRight)) ||
                            ((childRight >= bodyLeft) && (childRight <= bodyRight)) ||
                            ((childLeft <= bodyLeft) && (childRight >= bodyRight)))
                            &&
                            (((childTop >= bodyTop) && (childTop <= bodyBottom)) ||
                            ((childBottom >= bodyTop) && (childBottom <= bodyBottom)) ||
                            ((childTop <= bodyTop) && (childBottom >= bodyBottom)))
                            )
                        {
                            currentChildWidget.Render(drawArgs);
                        }
                    }
                }

				// restore normal viewport
				drawArgs.device.Viewport = defaultViewPort;

                if (this.BorderEnabled)
                {
                    this.m_OutlineVerts[0].X = this.AbsoluteLocation.X;
                    this.m_OutlineVerts[0].Y = this.AbsoluteLocation.Y + this.m_headerHeight;//m_currHeaderHeight;

                    this.m_OutlineVerts[1].X = this.AbsoluteLocation.X + this.m_size.Width;
                    this.m_OutlineVerts[1].Y = this.AbsoluteLocation.Y + this.m_headerHeight;//m_currHeaderHeight;

                    this.m_OutlineVerts[2].X = this.AbsoluteLocation.X + this.m_size.Width;
                    this.m_OutlineVerts[2].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                    this.m_OutlineVerts[3].X = this.AbsoluteLocation.X;
                    this.m_OutlineVerts[3].Y = this.AbsoluteLocation.Y + this.m_size.Height;

                    this.m_OutlineVerts[4].X = this.AbsoluteLocation.X;
                    this.m_OutlineVerts[4].Y = this.AbsoluteLocation.Y + this.m_headerHeight;// m_currHeaderHeight;

                    WidgetUtilities.DrawLine(this.m_OutlineVerts, this.m_BorderColor.ToArgb(), drawArgs.device);
                }
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
		public bool OnMouseDown(MouseEventArgs e)
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

            this.m_lastMouseDownPosition = new Point(e.X, e.Y);

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
			if(e.Button == MouseButtons.Left)
			{
				// Reset dragging and resizing
                this.m_isDragging = false;
                this.m_resize = ResizeDirection.None;

				// Reset scrolling
                this.m_isVScrolling = false;
                this.m_isHScrolling = false;

				#region resize and dragging

				// Check for resize (pointer is just outside the form)
				if(e.X > widgetLeft - this.resizeBuffer &&
					e.X < widgetLeft + this.resizeBuffer &&
					e.Y > widgetTop - this.resizeBuffer &&
					e.Y < widgetTop + this.resizeBuffer)
				{
					if ((this.HorizontalResizeEnabled) && (this.VerticalResizeEnabled))
                        this.m_resize = ResizeDirection.UL;
					else if (this.HorizontalResizeEnabled)
                        this.m_resize = ResizeDirection.Left;
					else if (this.VerticalResizeEnabled) this.m_resize = ResizeDirection.Up;
				}
				else if(e.X > widgetRight - this.resizeBuffer &&
					e.X < widgetRight + this.resizeBuffer &&
					e.Y > widgetTop - this.resizeBuffer &&
					e.Y < widgetTop + this.resizeBuffer)
				{
					if ((this.HorizontalResizeEnabled) && (this.VerticalResizeEnabled))
                        this.m_resize = ResizeDirection.UR;
					else if (this.HorizontalResizeEnabled)
                        this.m_resize = ResizeDirection.Right;
					else if (this.VerticalResizeEnabled) this.m_resize = ResizeDirection.Up;
				}
				else if(e.X > widgetLeft - this.resizeBuffer &&
					e.X < widgetLeft + this.resizeBuffer &&
					e.Y > widgetBottom - this.resizeBuffer &&
					e.Y < widgetBottom + this.resizeBuffer)
				{
					if ((this.HorizontalResizeEnabled) && (this.VerticalResizeEnabled))
                        this.m_resize = ResizeDirection.DL;
					else if (this.HorizontalResizeEnabled)
                        this.m_resize = ResizeDirection.Left;
					else if (this.VerticalResizeEnabled) this.m_resize = ResizeDirection.Down;
				}
				else if(e.X > widgetRight - this.resizeBuffer &&
					e.X < widgetRight + this.resizeBuffer &&
					e.Y > widgetBottom - this.resizeBuffer &&
					e.Y < widgetBottom + this.resizeBuffer )
				{
					if ((this.HorizontalResizeEnabled) && (this.VerticalResizeEnabled))
                        this.m_resize = ResizeDirection.DR;
					else if (this.HorizontalResizeEnabled)
                        this.m_resize = ResizeDirection.Right;
					else if (this.VerticalResizeEnabled) this.m_resize = ResizeDirection.Down;
				}
				else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
					e.X < this.AbsoluteLocation.X + this.resizeBuffer &&
					e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
					e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.m_size.Height && this.HorizontalResizeEnabled)
				{
                    this.m_resize = ResizeDirection.Left;
				}
				else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer + this.m_size.Width &&
					e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.m_size.Width &&
					e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
					e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.m_size.Height && this.HorizontalResizeEnabled)
				{
                    this.m_resize = ResizeDirection.Right;
				}
				else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
					e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.m_size.Width &&
					e.Y > this.AbsoluteLocation.Y - this.resizeBuffer &&
					e.Y < this.AbsoluteLocation.Y + this.resizeBuffer && this.VerticalResizeEnabled)
				{
                    this.m_resize = ResizeDirection.Up;
				}
				else if(e.X > this.AbsoluteLocation.X - this.resizeBuffer &&
					e.X < this.AbsoluteLocation.X + this.resizeBuffer + this.m_size.Width &&
					e.Y > this.AbsoluteLocation.Y - this.resizeBuffer + this.m_size.Height &&
					e.Y < this.AbsoluteLocation.Y + this.resizeBuffer + this.m_size.Height && this.VerticalResizeEnabled)
				{
                    this.m_resize = ResizeDirection.Down;
				}
					
					// Check for header double click (if its shown)
				else if(this.HeaderEnabled &&
                        e.X >= this.Location.X &&
                        e.X <= this.AbsoluteLocation.X + this.m_size.Width &&
                        e.Y >= this.AbsoluteLocation.Y &&
                        e.Y <= this.AbsoluteLocation.Y + this.m_currHeaderHeight)
				{
					if (DateTime.Now > this.m_LastClickTime.AddSeconds(0.5))
					{
                        this.m_isDragging = true;
						handled = true;
					}
					else
					{
                        this.m_renderBody = !this.m_renderBody;
                        //if (AutoHideHeader && m_renderBody)
                        //{
                        //    HeaderEnabled = false;
                        //}
                        //else
                        //{
                        //    HeaderEnabled = true;
                        //}
					}

                    this.m_LastClickTime = DateTime.Now;

				}

				#endregion

				#region scrolling

				if (inClientArea && this.m_renderBody)
				{
					// Check to see if we're in vertical scroll region
					if(this.m_showVScrollbar &&
                       e.X > this.Right - this.m_scrollbarWidth &&
                       (!this.m_showHScrollbar || e.Y < this.Bottom - this.m_scrollbarWidth) )
					{
						// set scroll position to e.Y offset from top of client area
						if (e.Y < this.BodyLocation.Y + this.m_vScrollbarPos)
						{
                            this.m_vScrollbarPos -= this.m_clientSize.Height / 10;
						}
						else if (e.Y > this.BodyLocation.Y + this.m_vScrollbarPos + this.m_vScrollbarHeight)
						{
                            this.m_vScrollbarPos += this.m_clientSize.Height / 10;
						}
						else
						{
                            this.m_vScrollbarGrabPosition = e.Y - this.BodyLocation.Y;
                            this.m_isVScrolling = true;
						}
						handled = true;
					}
					else if(this.m_showHScrollbar &&
                            e.Y > this.Bottom - this.m_scrollbarWidth &&
                            (!this.m_showVScrollbar || e.X < this.Right - this.m_scrollbarWidth) )
					{
						// set scroll position to e.Y offset from top of client area
						if (e.X < this.BodyLocation.X + this.m_hScrollbarPos)
						{
                            this.m_hScrollbarPos -= this.m_clientSize.Width / 10;
						}
						else if (e.X > this.BodyLocation.X + this.m_hScrollbarPos + this.m_hScrollbarWidth)
						{
                            this.m_hScrollbarPos += this.m_clientSize.Width / 10;
						}
						else
						{
                            this.m_hScrollbarGrabPosition = e.X - this.BodyLocation.X;
                            this.m_isHScrolling = true;
						}
						handled = true;
					}
				}

				#endregion
			}

			// Store the current position
            this.m_LastMousePosition = new System.Drawing.Point(e.X, e.Y);

			// If we aren't handling this then let the children try if they are rendered
			if(!handled && inClientArea && this.m_renderBody)
			{
                for (int i = this.m_ChildWidgets.Count - 1; i >= 0; i--)
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
			if(inClientArea || (this.m_resize != ResizeDirection.None))
			{
				handled = true;
			}

			return handled;			 
		}

        System.Drawing.Point m_lastMouseDownPosition = System.Drawing.Point.Empty;

		/// <summary>
		/// Mouse up event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseUp(MouseEventArgs e)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			int widgetTop = this.Top;
			int widgetBottom = this.Bottom;
			int widgetLeft = this.Left;
			int widgetRight = this.Right;

            // Check for close if header is rendered
			if(this.HeaderEnabled && this.m_lastMouseDownPosition.X >= this.Location.X + this.m_size.Width - 18 && this.m_lastMouseDownPosition.X <= this.AbsoluteLocation.X + this.m_size.Width && this.m_lastMouseDownPosition.Y >= this.AbsoluteLocation.Y && this.m_lastMouseDownPosition.Y <= this.AbsoluteLocation.Y + this.m_currHeaderHeight - 2)
			{
                this.Visible = false;
                this.m_isDragging = false;
                
                if (this.DestroyOnClose)
				{
                    this.Enabled = false;

					WidgetCollection parentCollection = (WidgetCollection) this.m_parentWidget;
					if (parentCollection != null)
						parentCollection.Remove(this);

					this.Dispose();
				}

                this.m_lastMouseDownPosition = System.Drawing.Point.Empty;
                return true;
			}

			// reset scrolling flags (don't care what up button event it is)
            this.m_isVScrolling = false;
            this.m_isHScrolling = false;

			// if its the left mouse button then reset dragging and resizing
			if (((this.m_isDragging) || (this.m_resize != ResizeDirection.None)) && 
				(e.Button == MouseButtons.Left))
			{
				// reset dragging flags
                this.m_isDragging = false;
                this.m_resize = ResizeDirection.None;
                this.m_lastMouseDownPosition = System.Drawing.Point.Empty;
				return true;
			}

			// if we're in the client area handle let the children try 
			if(e.X >= widgetLeft &&
				e.X <= widgetRight &&
				e.Y >= widgetTop &&
				e.Y <= widgetBottom)
			{
                for(int i = this.m_ChildWidgets.Count - 1; i >= 0; i--)
				{
					if(this.m_ChildWidgets[i] is IInteractive)
					{
						IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
                        if (currentInteractive.OnMouseUp(e))
                        {
                            this.m_lastMouseDownPosition = System.Drawing.Point.Empty;
                            return true;
                        }
					}
				}
			}

            this.m_lastMouseDownPosition = System.Drawing.Point.Empty;
			return false;
		}

        public delegate void ResizeHandler(object IWidget, Size size);
        public event ResizeHandler OnResizeEvent;

		/// <summary>
		/// Mouse move event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseMove(MouseEventArgs e)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			int deltaX = e.X - this.m_LastMousePosition.X;
			int deltaY = e.Y - this.m_LastMousePosition.Y;

            this.m_LastMousePosition = new System.Drawing.Point(e.X, e.Y);

			int widgetTop = this.Top;
			int widgetBottom = this.Bottom;
			int widgetLeft = this.Left;
			int widgetRight = this.Right;

			if(this.m_isDragging && this.m_headerHeight > 0)
			{
                this.m_location.X += deltaX;
                this.m_location.Y += deltaY;

				// Since we've modified m_location update the anchor calcs.
                this.UpdateLocation();

				return true;
			}

			// If we're resizing handle it
			if (this.m_resize != ResizeDirection.None)
			{
				if ((ResizeDirection.Up & this.m_resize) > 0)
				{
                    this.m_location.Y += deltaY;
                    this.m_size.Height -= deltaY;
				}
				if ((ResizeDirection.Down & this.m_resize) > 0)
				{
                    this.m_size.Height += deltaY;
				}
				if ((ResizeDirection.Right & this.m_resize) > 0)
				{
                    this.m_size.Width += deltaX;
				}
				if ((ResizeDirection.Left & this.m_resize) > 0)
				{
                    this.m_location.X += deltaX;
                    this.m_size.Width -= deltaX;
				}

				if(this.m_size.Width < this.MinSize.Width)
				{
                    this.m_size.Width = this.MinSize.Width;
				}

				if(this.m_size.Height < this.MinSize.Height)
				{
                    this.m_size.Height = this.MinSize.Height;
				}

				// TODO - Resize all the child widgets

                // Since we've modified m_location update the anchor calcs.
                this.UpdateLocation();
                if (this.OnResizeEvent != null)
                {
                    this.OnResizeEvent(this, this.m_size);
                }
				
				return true;
			}

			if (this.m_isVScrolling)
			{
                this.m_vScrollbarPos = e.Y - this.m_vScrollbarGrabPosition - this.BodyLocation.Y;
				return true;
			}

			if (this.m_isHScrolling)
			{
                this.m_hScrollbarPos = e.X - this.m_hScrollbarGrabPosition - this.BodyLocation.X;
				return true;
			}

			// Handle each child if we're in the client area
			if(e.X >= widgetLeft &&
				e.X <= widgetRight &&
				e.Y >= widgetTop &&
				e.Y <= widgetBottom)
            {
                for (int i = this.m_ChildWidgets.Count - 1; i >= 0; i--)
				{
					if(this.m_ChildWidgets[i] is IInteractive)
					{
						IInteractive currentInteractive = this.m_ChildWidgets[i] as IInteractive;
						
						if (currentInteractive.OnMouseMove(e))
							return true;
					}
				}
                return true;
			}

			return false;
		}


		/// <summary>
		/// Mouse wheel event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnMouseWheel(MouseEventArgs e)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			// if we're not in the client area
			if(e.X > this.Right || e.X < this.Left  || e.Y < this.Top || e.Y > this.Bottom)
				return false;

			// Mouse wheel scroll
			this.m_vScrollbarPos -= (e.Delta/10);

			return true;
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
		public bool OnKeyDown(KeyEventArgs e)
		{
			return false;
		}


		/// <summary>
		/// Key up event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		public bool OnKeyUp(KeyEventArgs e)
		{
			return false;
		}

        /// <summary>
        /// Key press event handler.  
        /// This widget doesn't do anything with key presses.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool OnKeyPress(KeyPressEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Helper method to recalculate position from the edge of the 
        /// parent widget for anchors.
        /// </summary>
		protected void UpdateLocation()
		{
			int height;
			int width;

			if (this.m_parentWidget != null)
			{
				height = this.m_parentWidget.ClientSize.Height;
				width = this.m_parentWidget.ClientSize.Width;
			}
			else
			{
                height = DrawArgs.ParentControl.Height;
                width = DrawArgs.ParentControl.Width;
			}

			// if anchors are active recompute distances from anchor location
			if (this.Anchor != WidgetEnums.AnchorStyles.None)
			{
				// Compute our distance from the edges
                this.m_distanceFromTop = this.m_location.Y;
                this.m_distanceFromLeft = this.m_location.X;
                this.m_distanceFromBottom = height - this.m_location.Y;
                this.m_distanceFromRight = width - this.m_location.X;

				// Make sure the distance makes sense
				if (this.m_distanceFromTop < 0) this.m_distanceFromTop = 0;
				if (this.m_distanceFromBottom < this.m_currHeaderHeight) this.m_distanceFromBottom = this.m_currHeaderHeight;
				if (this.m_distanceFromLeft < 0) this.m_distanceFromLeft = 0;
				if (this.m_distanceFromRight < this.m_currHeaderHeight) this.m_distanceFromRight = this.m_currHeaderHeight;
			}

			// If we're off the top or left edge
			if(this.m_location.X < 0) this.m_location.X = 0;
			if(this.m_location.Y < 0) this.m_location.Y = 0;

			// If we're off the bottom or right edge
			if(this.m_location.Y + this.m_headerHeight > height) this.m_location.Y = height - this.m_currHeaderHeight;
			if(this.m_location.X + this.m_headerHeight > width) this.m_location.X = width - this.m_currHeaderHeight;
		}

		#endregion

		#endregion
	}
}
