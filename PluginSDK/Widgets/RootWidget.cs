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

//using WorldWind.Renderable;

namespace WorldWind.Widgets
{
	/// <summary>
	/// Summary description for Widget.
	/// </summary>
	public class RootWidget : WidgetCollection, IWidget, IInteractive
	{
		IWidget m_parentWidget;
		IWidgetCollection m_ChildWidgets = new WidgetCollection();
		System.Windows.Forms.Control m_ParentControl;
		bool m_Initialized;

		public RootWidget(System.Windows.Forms.Control parentControl) 
		{
            this.m_ParentControl = parentControl;
		}

		#region Methods

		public void Initialize(DrawArgs drawArgs)
		{
		}

		public void Render(DrawArgs drawArgs)
		{
			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return;

			for(int index = this.m_ChildWidgets.Count - 1; index >= 0; index--)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;
				if(currentWidget != null)
				{
					if(currentWidget.ParentWidget == null || currentWidget.ParentWidget != this)
						currentWidget.ParentWidget = this;

					currentWidget.Render(drawArgs);
				}
			}
		}
		#endregion

		#region Properties

		System.Drawing.Point m_location = new System.Drawing.Point(0,0);
		System.Drawing.Point m_ClientLocation = new System.Drawing.Point(0,0);

		public System.Drawing.Point AbsoluteLocation
		{
			get { return this.m_location; }
			set { this.m_location = value; }		
		}

		public string Name
		{
			get { return "Main Frame"; }
			set { }
		}
		
		public IWidget ParentWidget
		{
			get { return this.m_parentWidget; }
			set { this.m_parentWidget = value; }
		}

		public IWidgetCollection ChildWidgets
		{
			get { return this.m_ChildWidgets; }
			set { this.m_ChildWidgets = value; }
		}		

		bool m_enabled = true;
		bool m_visible = true;
		object m_tag;

		public System.Drawing.Point ClientLocation
		{
			get { return this.m_ClientLocation; }
			set { }
		}

		public System.Drawing.Size ClientSize
		{
			get
			{
				System.Drawing.Size mySize = this.m_ParentControl.Size;
				return mySize;
			}
            set
            {
                // ignore attempts to resize the root widget
            }
		}

		public System.Drawing.Size WidgetSize
		{
			get { return this.m_ParentControl.Size; }
			set { }
		}

		public bool Enabled
		{
			get { return this.m_enabled; }
			set { this.m_enabled = value; }
		}

		public bool Visible
		{
			get { return this.m_visible; }
			set { this.m_visible = value; }
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
		public System.Drawing.Point Location
		{
			get { return this.m_location; }
			set { this.m_location = value; }
		}

		public object Tag
		{
			get { return this.m_tag; }
			set { this.m_tag = value; }
		}
		public bool IsInitialized
		{
			get { return this.m_Initialized;}
			set { this.m_Initialized = value; }
		}
		#endregion

		#region IInteractive Members

		MouseClickAction m_leftClickAction;
		public MouseClickAction LeftClickAction
		{
			get { return this.m_leftClickAction; }
			set { this.m_leftClickAction = value; }
		}	

		MouseClickAction m_rightClickAction;
		public MouseClickAction RightClickAction
		{
			get { return this.m_rightClickAction; }
			set { this.m_rightClickAction = value; }
		}	

		public bool OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnMouseDown(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnMouseUp(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnKeyDown(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnKeyUp(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

        public bool OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            for (int index = 0; index < this.m_ChildWidgets.Count; index++)
            {
                IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

                if (currentWidget != null && currentWidget is IInteractive)
                {
                    IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

                    bool handled = currentInteractive.OnKeyPress(e);
                    if (handled)
                        return handled;
                }
            }
            return false;
        }

		public bool OnMouseEnter(EventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnMouseEnter(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

            for (int index = this.m_ChildWidgets.Count - 1; index >= 0; index--)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnMouseMove(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnMouseLeave(EventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnMouseLeave(e);
					if(handled)
						return handled;
				}
			}
			
			return handled;
		}

		public bool OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!this.m_visible) || (!this.m_enabled))
				return false;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.m_ChildWidgets[index] as IInteractive;

					handled = currentInteractive.OnMouseWheel(e);
					if(handled)
						return handled;
				}
			}
			
			return handled;
		}

		#endregion

		new public void Add(IWidget widget)
		{
            this.m_ChildWidgets.Add(widget);
			widget.ParentWidget = this;
		}		
		
		new public void Remove(IWidget widget)
		{
            this.m_ChildWidgets.Remove(widget);
		}
	}
}
