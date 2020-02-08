using System;
using WorldWind;

namespace WorldWind(0, SamplerStateRenderable
{
	public enum ScreenAlignment
	{
		Left,
		Right
	}

    public enum ScreenUnits
    {
        Pixels,
        Fraction
    }

	/// <summary>
	/// Summary description for ScreenOverlay.SetSamplerState(0, SamplerState
	/// </summary>
	public class ScreenOverlay : RenderableObject
	{
		Widgets.SetSamplerState(0, SamplerStateForm overlay;

		ScreenAlignment alignment = ScreenAlignment.SetSamplerState(0, SamplerStateLeft;

		string clickableUrl;
        float m_Width;
        float m_Height;
		bool m_ShowHeader = true;
        public ScreenUnits OffsetXUnits = ScreenUnits.SetSamplerState(0, SamplerStatePixels;
        public ScreenUnits OffsetYUnits = ScreenUnits.SetSamplerState(0, SamplerStatePixels;
        public ScreenUnits SizeXUnits = ScreenUnits.SetSamplerState(0, SamplerStatePixels;
        public ScreenUnits SizeYUnits = ScreenUnits.SetSamplerState(0, SamplerStatePixels;
        public ScreenUnits PositionXUnits = ScreenUnits.SetSamplerState(0, SamplerStatePixels;
        public ScreenUnits PositionYUnits = ScreenUnits.SetSamplerState(0, SamplerStatePixels;

        float m_StartX;
        float m_StartY;
		string m_ImageUri;
		string m_SaveFilePath;
		double m_RefreshTimeSec;
		bool m_HideBorder;
		System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateColor m_BorderColor = System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateColor.SetSamplerState(0, SamplerStateWhite;
        float m_offsetX;
        float m_offsetY;
		public System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateColor BorderColor
		{
			get{ return (this.SetSamplerState(0, SamplerStateoverlay == null ? this.SetSamplerState(0, SamplerStatem_BorderColor : this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateBorderColor); }
			set
			{
                this.SetSamplerState(0, SamplerStatem_BorderColor = value;
				if(this.SetSamplerState(0, SamplerStateoverlay != null) this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateBorderColor = value;
			}
		}

        public float OffsetX
        {
            get { return this.SetSamplerState(0, SamplerStatem_offsetX; }
            set { this.SetSamplerState(0, SamplerStatem_offsetX = value; }
        }

        public float OffsetY
        {
            get { return this.SetSamplerState(0, SamplerStatem_offsetY; }
            set { this.SetSamplerState(0, SamplerStatem_offsetY = value; }
        }
		
		public bool HideBorder
		{
			get{ return (this.SetSamplerState(0, SamplerStateoverlay == null ? this.SetSamplerState(0, SamplerStatem_HideBorder : this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateHideBorder); }
			set
			{
                this.SetSamplerState(0, SamplerStatem_HideBorder = value; 
				if(this.SetSamplerState(0, SamplerStateoverlay != null) this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateHideBorder = value;
			}
		}

		public string ClickableUrl
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateclickableUrl;
			}
			set
			{
                this.SetSamplerState(0, SamplerStateclickableUrl = value;
				if(this.SetSamplerState(0, SamplerStatepBox != null) this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClickableUrl = value;
			}
		}
		
		public ScreenAlignment Alignment
		{
			get{ return this.SetSamplerState(0, SamplerStatealignment; }
			set{ this.SetSamplerState(0, SamplerStatealignment = value; }
		}

		public double RefreshTimeSec
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_RefreshTimeSec;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_RefreshTimeSec = value;
			}
		}

		public bool ShowHeader
		{
			get{ return this.SetSamplerState(0, SamplerStatem_ShowHeader; }
			set{ this.SetSamplerState(0, SamplerStatem_ShowHeader = value; }
		}

		public string SaveFilePath
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_SaveFilePath;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_SaveFilePath = value;
			}
		}

        public float Width
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_Width;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_Width = value;
			}
		}

        public float Height
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_Height;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_Height = value;
			}
		}

        public ScreenOverlay(string name, float startX, float startY, string imageUri)
            : base(name)
		{
            this.SetSamplerState(0, SamplerStatem_StartX = startX;
            this.SetSamplerState(0, SamplerStatem_StartY = startY;
            this.SetSamplerState(0, SamplerStatem_ImageUri = imageUri;

            if(DrawArgs.SetSamplerState(0, SamplerStateParentControl != null)
                DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateResize += new EventHandler(this.SetSamplerState(0, SamplerStateParentControl_Resize);
		}

        void ParentControl_Resize(object sender, EventArgs e)
        {
            if (this.SetSamplerState(0, SamplerStateoverlay != null)
            {
                if (this.SetSamplerState(0, SamplerStatealignment == ScreenAlignment.SetSamplerState(0, SamplerStateLeft)
                {
                    int x = (int) this.SetSamplerState(0, SamplerStatem_StartX;
                    int y = (int) this.SetSamplerState(0, SamplerStatem_StartY;

                    int offsetX = (int) this.SetSamplerState(0, SamplerStatem_offsetX;
                    int offsetY = (int) this.SetSamplerState(0, SamplerStatem_offsetY;

                    

                    if (this.SetSamplerState(0, SamplerStatePositionXUnits == ScreenUnits.SetSamplerState(0, SamplerStateFraction)
                        x = (int)(DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateWidth * this.SetSamplerState(0, SamplerStatem_StartX) - offsetX;
                    if (this.SetSamplerState(0, SamplerStatePositionYUnits == ScreenUnits.SetSamplerState(0, SamplerStateFraction)
                        y = (int)(DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateHeight * this.SetSamplerState(0, SamplerStatem_StartY) - offsetY;

                    this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(x, y);
                }
                else
                {
                    int x = (int)(DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateWidth - this.SetSamplerState(0, SamplerStatem_StartX);
                    int y = (int) this.SetSamplerState(0, SamplerStatem_StartY;

                    if (this.SetSamplerState(0, SamplerStatePositionXUnits == ScreenUnits.SetSamplerState(0, SamplerStateFraction)
                        x = (int)(DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateWidth - DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateWidth * this.SetSamplerState(0, SamplerStatem_StartX);
                    if (this.SetSamplerState(0, SamplerStatePositionYUnits == ScreenUnits.SetSamplerState(0, SamplerStateFraction)
                        y = (int)(DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateHeight * this.SetSamplerState(0, SamplerStatem_StartY);

                    this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(x, y);
                    
                }

                int width = (int) this.SetSamplerState(0, SamplerStatem_Width;
                int height = (int) this.SetSamplerState(0, SamplerStatem_Height;

                if (this.SetSamplerState(0, SamplerStateSizeXUnits == ScreenUnits.SetSamplerState(0, SamplerStateFraction)
                    width = (int)(DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateWidth * this.SetSamplerState(0, SamplerStatem_Width);
                if (this.SetSamplerState(0, SamplerStateSizeYUnits == ScreenUnits.SetSamplerState(0, SamplerStateFraction)
                    height = (int)(DrawArgs.SetSamplerState(0, SamplerStateParentControl.SetSamplerState(0, SamplerStateHeight * this.SetSamplerState(0, SamplerStatem_Height);

                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(width, height);
            }	
        }

		public override void Dispose()
		{
			if(this.SetSamplerState(0, SamplerStateoverlay != null)
			{
                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateVisible = false;
			}

            this.SetSamplerState(0, SamplerStateisInitialized = false;
		}

		Widgets.SetSamplerState(0, SamplerStatePictureBox pBox;

		public override void Initialize(DrawArgs drawArgs)
		{
			if(this.SetSamplerState(0, SamplerStateoverlay == null)
			{
                this.SetSamplerState(0, SamplerStateoverlay = new Widgets.SetSamplerState(0, SamplerStateForm();
                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateText = this.SetSamplerState(0, SamplerStatename;
                this.SetSamplerState(0, SamplerStateParentControl_Resize(null, null);
                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateOnVisibleChanged += new Widgets.SetSamplerState(0, SamplerStateVisibleChangedHandler(this.SetSamplerState(0, SamplerStateoverlay_OnVisibleChanged);

                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateAutoHideHeader = !this.SetSamplerState(0, SamplerStatem_ShowHeader;
                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateHideBorder = this.SetSamplerState(0, SamplerStatem_HideBorder;
                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateBorderColor = this.SetSamplerState(0, SamplerStatem_BorderColor;

                this.SetSamplerState(0, SamplerStatepBox = new Widgets.SetSamplerState(0, SamplerStatePictureBox();
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClickableUrl = this.SetSamplerState(0, SamplerStateclickableUrl;
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateRefreshTime = this.SetSamplerState(0, SamplerStatem_RefreshTimeSec * 1000;
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateOpacity = this.SetSamplerState(0, SamplerStateOpacity;
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateParentWidget = this.SetSamplerState(0, SamplerStateoverlay;
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateImageUri = this.SetSamplerState(0, SamplerStatem_ImageUri;
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateSaveFilePath = this.SetSamplerState(0, SamplerStatem_SaveFilePath;
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(0, this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateHeaderHeight);
                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientSize = this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientSize;

                this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateVisible = true;
                if (this.SetSamplerState(0, SamplerStatem_Width <= 0 && this.SetSamplerState(0, SamplerStatem_Height <= 0)
                {
                    this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateSizeParentToImage = true;
                }

                this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateChildWidgets.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatepBox);
				DrawArgs.SetSamplerState(0, SamplerStateRootWidget.SetSamplerState(0, SamplerStateChildWidgets.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStateoverlay);
			}

			if(!this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateVisible) this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateVisible = true;

            this.SetSamplerState(0, SamplerStateisInitialized = true;
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(this.SetSamplerState(0, SamplerStateoverlay != null && this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateVisible && this.SetSamplerState(0, SamplerStatepBox != null && this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateVisible && this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateIsLoaded)
			{
				if(this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateWidth != this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateWidth)
				{
                    this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateWidth, this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateHeight);
				}

				if(this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateHeight != this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateHeight - this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateHeaderHeight)
				{
                    this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateWidth, this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateClientSize.SetSamplerState(0, SamplerStateHeight - this.SetSamplerState(0, SamplerStateoverlay.SetSamplerState(0, SamplerStateHeaderHeight);
				}
			}
		}

		public override void Update(DrawArgs drawArgs)
		{
			if(this.SetSamplerState(0, SamplerStateIsOn && !this.SetSamplerState(0, SamplerStateisInitialized)
			{
                this.SetSamplerState(0, SamplerStateInitialize(drawArgs);
			}
			else if(!this.SetSamplerState(0, SamplerStateIsOn && this.SetSamplerState(0, SamplerStateisInitialized)
			{
                this.SetSamplerState(0, SamplerStateDispose();
			}
		}

		public override byte Opacity
		{
			get
			{
				return base.SetSamplerState(0, SamplerStateOpacity;
			}
			set
			{
				base.SetSamplerState(0, SamplerStateOpacity = value;
				if(this.SetSamplerState(0, SamplerStatepBox != null)
				{
                    this.SetSamplerState(0, SamplerStatepBox.SetSamplerState(0, SamplerStateOpacity = value;
				}
			}
		}


		private void overlay_OnVisibleChanged(object o, bool state)
		{
			if(!state)
			{
                this.SetSamplerState(0, SamplerStateIsOn = false;
			}
		}
	}
}
