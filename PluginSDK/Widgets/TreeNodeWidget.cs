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
// Copyright (c) 2005 The Johns Hopkins University. 
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

namespace WorldWind.Widgets
{
    public delegate void CheckStateChangedHandler(object o, bool state);

	/// <summary>
	/// Summary description for TextLabel.
	/// </summary>
	public abstract class TreeNodeWidget : WidgetCollection, IWidget, IInteractive
	{
		public const int NODE_OFFSET = 5;

		public const int NODE_HEIGHT = 20;

		public const int NODE_INDENT = 15;

		public const int NODE_ARROW_SIZE = 15;

		public const int NODE_CHECKBOX_SIZE = 15;

		protected const int DEFAULT_OPACITY = 150;

		#region icon members

		// sprite to use for render of icon
		protected Sprite m_sprite;

		// icon texture to use
		protected IconTexture m_iconTexture;

		// image name to load
		protected string m_imageName = "";

		// whether or not this node has an icon
		protected bool m_hasIcon = false;

		// Icon X scaling computed by dividing icon area width by texture width
		protected float XScale;
 
		// Icon Y scaling computed by dividing icon area height by texture height 
		protected float YScale;

		#endregion icon members

		#region color members

		protected int m_itemOnColor = Color.White.ToArgb();
		protected int m_itemOffColor = Color.Gray.ToArgb();

		protected int m_mouseOverColor = Color.FromArgb(DEFAULT_OPACITY,160,160,160).ToArgb();
		protected int m_mouseOverOnColor = Color.White.ToArgb(); 
		protected int m_mouseOverOffColor = Color.Black.ToArgb(); 

		#endregion color members

		#region iWidget members

		protected IWidget m_parentWidget;

		protected Point m_location = new Point(0,0);

		// size is used within this class for most size calculations
		protected Size m_size = new Size(0,0);

		// consumed size is passed back to parent for the widget size.  Usually same as m_size except if top level tree widget
		protected Size m_ConsumedSize = new Size(0,0);

		protected IWidgetCollection m_subNodes = new WidgetCollection();

		protected bool m_enabled = true;

		protected bool m_visible = true;

		// Menu tree items always count height and width wise.
		protected bool m_countHeight = true;
		protected bool m_countWidth = true;

		protected object m_tag;
		protected string m_name = "";

		#endregion IWidget members

		// true if we've initialized this widget for proper rendering
		protected bool m_isInitialized;

		// true if this widget is expanded
		protected bool m_isExpanded;

		// true if the mouse is over this widget (detected from move events)
		protected bool m_isMouseOver;

		// true if we saw the mousedown event for this widget
		protected bool m_isMouseDown;

		// the associated renderable object (used in layer manager)
		protected RenderableObject m_renderableObject;
		
		// true if we are using radio style checkbox
		protected bool m_isRadioButton;

		// true if check box should be checked
		protected bool m_isChecked = true;

		// true if check box should be mapped to if this tree node is enabled or not
		protected bool m_enableCheck = true;

		// fonts to use in render
		protected static SharpDX.Direct3D9.Font m_drawingFont;
		protected static SharpDX.Direct3D9.Font m_wingdingsFont;
		protected static SharpDX.Direct3D9.Font m_worldwinddingsFont;

		protected int m_xOffset = 0;

		#region Properties

		public string ImageName
		{
			get { return this.m_imageName; }
			set 
			{
                this.m_imageName = value;
                this.m_isInitialized = false;
			}
		}


		public bool Expanded
		{
			get { return this.m_isExpanded; }
			set { this.m_isExpanded = value; }
		}

		public bool IsRadioButton
		{
			get { return this.m_isRadioButton; }
			set { this.m_isRadioButton = value; }
		}

		public bool EnableCheck
		{
			get { return this.m_enableCheck; }
			set { this.m_enableCheck = value; }
		}

		public bool IsChecked
		{
			get
			{
				if (this.m_enableCheck) this.m_isChecked = this.Enabled;

				return this.m_isChecked;
			}
			set
			{
				if (this.m_enableCheck)
					this.Enabled = value;

                this.m_isChecked = value;
			}
		}

		public RenderableObject RenderableObject
		{
			get { return this.m_renderableObject; }
			set 
			{
                this.m_renderableObject = value;
				if (this.m_renderableObject != null)
				{
					// Use radio check
					if(this.m_renderableObject.ParentList != null && this.m_renderableObject.ParentList.ShowOnlyOneLayer) this.m_isRadioButton = true;
				}

			}
		}

		#endregion

		#region IWidget Properties

		public IWidget ParentWidget
		{
			get { return this.m_parentWidget; }
			set { this.m_parentWidget = value; }
		}


		public Point Location
		{
			get { return this.m_location; }
			set { this.m_location = value; }
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

		
		public Point ClientLocation
		{
			get { return this.AbsoluteLocation; }
		}


		public Size ClientSize
		{
			get 
			{
				Size clientSize = new Size();
				if (this.m_parentWidget != null)
				{
                    this.m_size.Width = this.m_parentWidget.ClientSize.Width;
				}

				clientSize.Width = this.m_size.Width;
				clientSize.Height = this.m_ConsumedSize.Height;

				return clientSize; 
			}
			set { this.m_size = value; }
		}


		public Size WidgetSize
		{
			get { return this.m_ConsumedSize; }
			set { this.m_ConsumedSize = value; }
		}


		public IWidgetCollection ChildWidgets
		{
			get { return this.m_subNodes; }
			set { this.m_subNodes = value; }
		}


		public bool Enabled
		{
			get 
			{ 
				if (this.m_renderableObject != null) this.m_enabled = this.m_renderableObject.IsOn;

				return this.m_enabled; 
			}
			set 
			{
                this.m_enabled = value; 
				if (this.m_renderableObject != null) this.m_renderableObject.IsOn = value;

                if (this.OnCheckStateChanged != null) this.OnCheckStateChanged(this, value);
			}
		}


		public bool Visible
		{
			get { return this.m_visible; }
			set { this.m_visible = value; }
		}


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
			get { return this.m_tag; }
			set { this.m_tag = value; }
		}


		public string Name
		{
			get
			{
				// if the name isn't set return the RO's name if it exists
				if ( (this.m_name.Length <= 0) && (this.m_renderableObject != null) )
				{
					return this.m_renderableObject.Name;
				} 

				return this.m_name;
			}
			set
			{
                this.m_name = value;
			}
		}


		#endregion IWidget Properties

        public event CheckStateChangedHandler OnCheckStateChanged;

		public TreeNodeWidget()
		{
            this.m_location.X = 0;
            this.m_location.Y = 0;
            this.m_size.Height = NODE_HEIGHT;
            this.m_size.Width = 100;
            this.m_ConsumedSize = this.m_size;
            this.m_isInitialized = false;
		}

		public TreeNodeWidget(string name) : this()
		{
            this.m_name = name;
		}

		public void Initialize (DrawArgs drawArgs)
		{
			// Initialize fonts
            if (m_drawingFont == null)
            {
                Font localHeaderFont = new Font("Arial", 12.0f, FontStyle.Italic | FontStyle.Bold);
                m_drawingFont = new SharpDX.Direct3D9.Font(drawArgs.device, localHeaderFont);
            }

            if (m_wingdingsFont == null)
            {
                Font wingdings = new Font("Wingdings", 12.0f);
                m_wingdingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, wingdings);
            }

            if (m_worldwinddingsFont == null)
            {
                AddFontResource(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                System.Drawing.Text.PrivateFontCollection fpc = new System.Drawing.Text.PrivateFontCollection();
                fpc.AddFontFile(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                Font m_worldwinddings = new Font(fpc.Families[0], 12.0f);
                m_worldwinddingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, m_worldwinddings);
            }

			// Initialize icon if any
			if (this.m_imageName.Trim() != string.Empty)
			{
				object key = null;

				// Icon image from file
                this.m_iconTexture = (IconTexture)DrawArgs.Textures[this.m_imageName];
				if(this.m_iconTexture==null)
				{
					key = this.m_imageName;
                    this.m_iconTexture = new IconTexture( drawArgs.device, this.m_imageName );
				}

				if(this.m_iconTexture!=null)
				{
                    this.m_iconTexture.ReferenceCount++;

					if(key!=null)
					{
						// New texture, cache it
						DrawArgs.Textures.Add(key, this.m_iconTexture);
					}

					if (this.m_size.Width == 0) this.m_size.Width = this.m_iconTexture.Width;

					if (this.m_size.Height == 0) this.m_size.Height = this.m_iconTexture.Height;

					this.XScale = (float) this.m_size.Width / this.m_iconTexture.Width;
					this.YScale = (float) this.m_size.Height / this.m_iconTexture.Height;
				}

				if (this.m_sprite == null) this.m_sprite = new Sprite(drawArgs.device);
			}

            this.m_isInitialized = true;
		}

        [DllImport("gdi32.dll")]
        static extern int AddFontResource(string lpszFilename);

		protected MouseClickAction m_leftClickAction;
		public MouseClickAction LeftClickAction
		{
			get { return this.m_leftClickAction; }
			set { this.m_leftClickAction = value; }
		}	

		protected MouseClickAction m_rightClickAction;
		public MouseClickAction RightClickAction
		{
			get { return this.m_rightClickAction; }
			set { this.m_rightClickAction = value; }
		}	

		protected MouseClickAction m_checkClickAction;
		public MouseClickAction CheckClickAction
		{
			get { return this.m_checkClickAction; }
			set { this.m_checkClickAction = value; }
		}

		protected MouseClickAction m_expandClickAction;
		public MouseClickAction ExpandClickAction
		{
			get { return this.m_expandClickAction; }
			set { this.m_expandClickAction = value; }
		}

        protected MouseClickAction m_enterAction;
        public MouseClickAction EnterAction
        {
            get { return this.m_enterAction; }
            set { this.m_enterAction = value; }
        }

        protected MouseClickAction m_leaveAction;
        public MouseClickAction LeaveAction
        {
            get { return this.m_leaveAction; }
            set { this.m_leaveAction = value; }
        }

		#region IInteractive Methods

		public bool OnMouseDown(MouseEventArgs e)
		{
			bool handled = false;
			if (this.m_visible)
			{
				// if we're in the client area and lmb
				if( e.X >= this.AbsoluteLocation.X &&
					e.X <= this.AbsoluteLocation.X + this.ClientSize.Width &&
					e.Y >= this.AbsoluteLocation.Y &&
					e.Y <= this.AbsoluteLocation.Y + NODE_HEIGHT)
				{
                    this.m_isMouseDown = true;
					handled = true;
				}
				else
				{
                    this.m_isMouseDown = false;
				}
			}

			// if mouse didn't come down on us, let the children try as we should be inside the top form client area
			if (!handled)
			{
				for(int i = 0; i < this.m_subNodes.Count; i++)
				{
					if(this.m_subNodes[i] is IInteractive)
					{
						IInteractive currentInteractive = this.m_subNodes[i] as IInteractive;
						handled = currentInteractive.OnMouseDown(e);
					}

					// if anyone has handled this, we're done
					if (handled)
						continue;	
				}
			}

			return handled;			 
		}


		public bool OnMouseUp(MouseEventArgs e)
		{
			bool handled = false;
			if (this.m_visible)
			{
				// if we're in the client area
				if( e.X >= this.AbsoluteLocation.X &&
					e.X <= this.AbsoluteLocation.X + this.ClientSize.Width &&
					e.Y >= this.AbsoluteLocation.Y &&
					e.Y <= this.AbsoluteLocation.Y + NODE_HEIGHT)
				{
					if (this.m_isMouseDown)
					{
						// if we're in the expand arrow region
						if ((e.X > this.AbsoluteLocation.X + this.m_xOffset) &&
							(e.X < this.AbsoluteLocation.X + this.m_xOffset + NODE_ARROW_SIZE))
						{
							this.Expanded = !this.Expanded;

							// call helper routine if it exists
							if (this.m_expandClickAction != null) this.m_expandClickAction(e);
						}
						// else if we're supposed to use checkmark and we're in the activate/deactivate region
						else if (this.m_enableCheck &&
                                 (e.X > this.AbsoluteLocation.X + this.m_xOffset + NODE_ARROW_SIZE) &&
                                 (e.X < this.AbsoluteLocation.X + this.m_xOffset + NODE_ARROW_SIZE + NODE_CHECKBOX_SIZE))
						{
							this.Enabled = !this.Enabled;

							// call helper routine if it exists
							if (this.m_checkClickAction != null) this.m_checkClickAction(e);
						}
							// Otherwise perform general LMB, RMB action
						else if ((e.Button == MouseButtons.Left) && (this.m_leftClickAction != null))
						{
                            this.m_leftClickAction(e);
						} 
						else if ((e.Button == MouseButtons.Right) && (this.m_rightClickAction != null))
						{
                            this.m_rightClickAction(e);
						}

						handled = true;
					}
				}
			}

			// if mouse isn't over us, let the children try as we should be inside the top form client area
			if (!handled)
			{
				for(int i = 0; i < this.m_subNodes.Count; i++)
				{
                    if (this.m_subNodes[i] is UpdateWidget)
                    {
                        UpdateWidget currentWidget = this.m_subNodes[i] as UpdateWidget;
                        bool subHandled = currentWidget.OnMouseUp(e);
                        if (subHandled)
                            handled = true;
                    }
                    if(this.m_subNodes[i] is IInteractive)
					{
						IInteractive currentInteractive = this.m_subNodes[i] as IInteractive;
						bool subHandled = currentInteractive.OnMouseUp(e);
                        if (subHandled)
                            handled = true;
                    }

					// if anyone has handled this, we're done
					//if (handled)
					//	break;	
				}
			}

			// regardless reset mousedown point 
            this.m_isMouseDown = false;
            
            return handled;
		}


		public bool OnMouseMove(MouseEventArgs e)
		{
			bool handled = false;
			if (this.m_visible)
			{
				// if we're in the client area
				if( e.X >= this.AbsoluteLocation.X &&
					e.X <= this.AbsoluteLocation.X + this.ClientSize.Width &&
					e.Y >= this.AbsoluteLocation.Y &&
					e.Y <= this.AbsoluteLocation.Y + NODE_HEIGHT)
				{
					if (!this.m_isMouseOver)
                        if (this.m_enterAction != null)
                        {
                            this.m_enterAction(e);
                        }
						this.OnMouseEnter(e);

//					m_isMouseOver = true;
					handled = true;
				}
				else
				{
					if (this.m_isMouseOver)
                        if (this.m_leaveAction != null)
                        {
                            this.m_leaveAction(e);
                        }
						this.OnMouseLeave(e);

//					m_isMouseOver = false;
				}
			}
			else
			{
                this.m_isMouseOver = false;
			}

			// call all the children because they need to have a chance to detect mouse leaving.
			for(int i = 0; i < this.m_subNodes.Count; i++)
			{
				if(this.m_subNodes[i] is IInteractive)
				{
					IInteractive currentInteractive = this.m_subNodes[i] as IInteractive;
					if (currentInteractive.OnMouseMove(e))
						handled = true;
				}
			}

			return handled;
		}


		public bool OnMouseWheel(MouseEventArgs e)
		{
			return false;
		}

		public bool OnMouseEnter(EventArgs e)
		{
            if (this.m_parentWidget != null && this.m_parentWidget is TreeNodeWidget)
            {
                TreeNodeWidget parentNode = (TreeNodeWidget) this.m_parentWidget;
                if (!parentNode.Expanded)
                {
                    this.m_isMouseOver = false;
                }
                else
                {
                    this.m_isMouseOver = true;
                }
            }
            else
            {
                this.m_isMouseOver = true;
            }

            return false;
		}


		public bool OnMouseLeave(EventArgs e)
		{
            this.m_isMouseOver = false;

            

			return false;
		}


		public bool OnKeyDown(KeyEventArgs e)
		{
			return false;
		}


		public bool OnKeyUp(KeyEventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Key press event handler.
		/// This widget does nothing with key presses.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public bool OnKeyPress(KeyPressEventArgs e)
		{
			return false;
		}


		#endregion

		#region IWidget Methods

		public void Render(DrawArgs drawArgs)
		{
			int consumedPixels = 0;

			consumedPixels = this.Render(drawArgs, NODE_OFFSET, 0);

			// TODO: set size to consumed pixels?
            this.m_ConsumedSize.Height = consumedPixels;
		}

		#endregion

		#region IWidgetCollection methods

		new public void Add(IWidget widget)
		{
            this.m_subNodes.Add(widget);
			widget.ParentWidget = this;
		}

		new public void Remove(IWidget widget)
		{
            this.m_subNodes.Remove(widget);
		}

		#endregion IWidgetCollection methods

		/// <summary>
		/// Specialized render for tree nodes
		/// </summary>
		/// <param name="drawArgs"></param>
		/// <param name="xOffset">The offset from the left based on how deep this node is nested</param>
		/// <param name="yOffset">The offset from the top based on how many treenodes are above this one</param>
		/// <returns>Total pixels consumed by this widget and its children</returns>
		public abstract int Render(DrawArgs drawArgs, int xOffset, int yOffset);
	}
}
