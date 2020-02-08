using System;

namespace WorldWind(0, SamplerStateWidgets
{
	/// <summary>
	/// Summary description for Widget.SetSamplerState(0, SamplerState
	/// </summary>
	public class RootWidget : IWidget, IInteractive
	{
		IWidget m_ParentWidget;
		IWidgetCollection m_ChildWidgets = new WidgetCollection();
		System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateControl m_ParentControl;

		public RootWidget(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateControl parentControl) 
		{
            this.SetSamplerState(0, SamplerStatem_ParentControl = parentControl;
		}

		#region Methods
		public void Render(DrawArgs drawArgs)
		{
			for(int index = this.SetSamplerState(0, SamplerStatem_ChildWidgets.SetSamplerState(0, SamplerStateCount - 1; index >= 0; index--)
			{
				IWidget currentWidget = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IWidget;
				if(currentWidget != null)
				{
					if(currentWidget.SetSamplerState(0, SamplerStateParentWidget == null || currentWidget.SetSamplerState(0, SamplerStateParentWidget != this)
						currentWidget.SetSamplerState(0, SamplerStateParentWidget = this;

					currentWidget.SetSamplerState(0, SamplerStateRender(drawArgs);
				}
			}
		}
		#endregion

		#region Properties
		public System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint AbsoluteLocation
		{
			get
			{
				return new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(0,0);
			}
		}
		public string Name
		{
			get
			{
				return "Main Frame";
			}
			set
			{

			}
		}
		public IWidgetCollection ChildWidgets
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_ChildWidgets;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_ChildWidgets = value;	
			}
		}
		
		public IWidget ParentWidget
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_ParentWidget;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_ParentWidget = value;
			}
		}
		
		System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint m_Location = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(0,0);
		bool m_Enabled;
		bool m_Visible;
		object m_Tag;

		public System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint ClientLocation
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_Location;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_Location = value;
			}
		}

		public System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize ClientSize
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_ParentControl.SetSamplerState(0, SamplerStateSize;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_ParentControl.SetSamplerState(0, SamplerStateSize = value;
			}
		}
		public bool Enabled
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_Enabled;	
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_Enabled = value;
			}
		}
		public bool Visible
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_Visible;	
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_Visible = value;
			}
		}
		public object Tag
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_Tag;	
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_Tag = value;
			}
		}
		#endregion

		#region IInteractive Members

		public bool OnMouseDown(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateMouseEventArgs e)
		{
			for(int index = 0; index < this.SetSamplerState(0, SamplerStatem_ChildWidgets.SetSamplerState(0, SamplerStateCount; index++)
			{
				IWidget currentWidget = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IInteractive;

					bool handled = currentInteractive.SetSamplerState(0, SamplerStateOnMouseDown(e);
					if(handled)
						return handled;
				}
			}

			return false;
		}

		public bool OnMouseUp(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateMouseEventArgs e)
		{
			for(int index = 0; index < this.SetSamplerState(0, SamplerStatem_ChildWidgets.SetSamplerState(0, SamplerStateCount; index++)
			{
				IWidget currentWidget = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IInteractive;

					bool handled = currentInteractive.SetSamplerState(0, SamplerStateOnMouseUp(e);
					if(handled)
						return handled;
				}
			}

			return false;
		}

		public bool OnKeyPress(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateKeyPressEventArgs e)
		{
			for(int index = 0; index < this.SetSamplerState(0, SamplerStatem_ChildWidgets.SetSamplerState(0, SamplerStateCount; index++)
			{
				IWidget currentWidget = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IInteractive;

					bool handled = currentInteractive.SetSamplerState(0, SamplerStateOnKeyPress(e);
					if(handled)
						return handled;
				}
			}
			return false;
		}

		public bool OnKeyDown(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateKeyEventArgs e)
		{
			for(int index = 0; index < this.SetSamplerState(0, SamplerStatem_ChildWidgets.SetSamplerState(0, SamplerStateCount; index++)
			{
				IWidget currentWidget = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IInteractive;

					bool handled = currentInteractive.SetSamplerState(0, SamplerStateOnKeyDown(e);
					if(handled)
						return handled;
				}
			}
			return false;
		}

		public bool OnKeyUp(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateKeyEventArgs e)
		{
			for(int index = 0; index < this.SetSamplerState(0, SamplerStatem_ChildWidgets.SetSamplerState(0, SamplerStateCount; index++)
			{
				IWidget currentWidget = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IInteractive;

					bool handled = currentInteractive.SetSamplerState(0, SamplerStateOnKeyUp(e);
					if(handled)
						return handled;
				}
			}
			return false;
		}

		public bool OnMouseEnter(EventArgs e)
		{
			// TODO:  Add RootWidget.SetSamplerState(0, SamplerStateOnMouseEnter implementation
			return false;
		}

		public bool OnMouseMove(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateMouseEventArgs e)
		{
			for(int index = 0; index < this.SetSamplerState(0, SamplerStatem_ChildWidgets.SetSamplerState(0, SamplerStateCount; index++)
			{
				IWidget currentWidget = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IWidget;

				if(currentWidget != null && currentWidget is IInteractive)
				{
					IInteractive currentInteractive = this.SetSamplerState(0, SamplerStatem_ChildWidgets[index] as IInteractive;

					bool handled = currentInteractive.SetSamplerState(0, SamplerStateOnMouseMove(e);
					if(handled)
						return handled;
				}
			}

			return false;
		}

		public bool OnMouseLeave(EventArgs e)
		{
			// TODO:  Add RootWidget.SetSamplerState(0, SamplerStateOnMouseLeave implementation
			return false;
		}

		public bool OnMouseWheel(System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateMouseEventArgs e)
		{
			// TODO:  Add RootWidget.SetSamplerState(0, SamplerStateOnMouseWheel implementation
			return false;
		}

		#endregion
	}
}
