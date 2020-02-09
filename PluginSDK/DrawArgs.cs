using System;
using System.Diagnostics;
using System.Collections;
using SharpDX;
using SharpDX.Direct3D9;
using WorldWind.Net;
using Utility;

namespace WorldWind
{
    /// <summary>
    /// 
    /// </summary>
    public class DrawArgs : IDisposable
    {
        public Device device;
        public System.Windows.Forms.Control parentControl;
        public static System.Windows.Forms.Control ParentControl;
        public int numBoundaryPointsTotal;
        public int numBoundaryPointsRendered;
        public int numBoundariesDrawn;
        public Font defaultDrawingFont;
        public System.Drawing.Font defaultSubTitleFont;
        public Font defaultSubTitleDrawingFont;
        public Font toolbarFont;
        public int screenWidth;
        public int screenHeight;
        public static System.Drawing.Point LastMousePosition;
        public int numberTilesDrawn;
        public System.Drawing.Point CurrentMousePosition;
        public string UpperLeftCornerText = "";
        CameraBase m_WorldCamera;
        public World m_CurrentWorld;
        public static bool IsLeftMouseButtonDown = false;
        public static bool IsRightMouseButtonDown = false;
        public static DownloadQueue DownloadQueue = new DownloadQueue();
        public static Widgets.RootWidget RootWidget = null;
        public static Widgets.RootWidget NewRootWidget = null;
        public int TexturesLoadedThisFrame;
        private static System.Drawing.Bitmap bitmap;
        public static System.Drawing.Graphics Graphics;

        public bool RenderWireFrame = false;

        /// <summary>
        /// Table of all icon textures
        /// </summary>
        protected static Hashtable m_textures = new Hashtable();
        public static Hashtable Textures
        {
            get { return m_textures; }
        }

        public static CameraBase Camera;
        public CameraBase WorldCamera
        {
            get
            {
                return this.m_WorldCamera;
            }
            set
            {
                this.m_WorldCamera = value;
                Camera = value;
            }
        }

        public static World CurrentWorldStatic;
        public World CurrentWorld
        {
            get
            {
                return this.m_CurrentWorld;
            }
            set
            {
                this.m_CurrentWorld = value;
                CurrentWorldStatic = value;
            }
        }

        public static WorldWindow WorldWindow
        {
            get
            {
                if (ParentControl is WorldWindow)
                    return (WorldWindow)ParentControl;
                else
                    return null;
            }
        }

        /*
		public Device ReferenceDevice
		{
			get
			{
				return m_Device3dReference;
			}
		}
		*/
        /// <summary>
        /// Absolute time of current frame render start (ticks)
        /// </summary>
        public static long CurrentFrameStartTicks;

        /// <summary>
        /// Seconds elapsed between start of previous frame and start of current frame.
        /// </summary>
        public static float LastFrameSecondsElapsed;

        static CursorType mouseCursor;
        static CursorType lastCursor;
        bool repaint = true;
        bool isPainting;
        Hashtable fontList = new Hashtable();

        public static Device Device;
        System.Windows.Forms.Cursor measureCursor;

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.DrawArgs"/> class.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="parentForm"></param>
        public DrawArgs(Device device, System.Windows.Forms.Control parentForm)
        {
            this.parentControl = parentForm;
            ParentControl = parentForm;
            Device = device;
            this.device = device;
            this.defaultDrawingFont = this.CreateFont(World.Settings.defaultFontName, World.Settings.defaultFontSize);
            if (this.defaultDrawingFont == null) this.defaultDrawingFont = this.CreateFont("", 10);

            this.defaultSubTitleFont = new System.Drawing.Font("Ariel", 8.0f);
            this.defaultSubTitleDrawingFont = new Font(device, this.defaultSubTitleFont);
            if (this.defaultSubTitleDrawingFont == null) this.defaultSubTitleDrawingFont = this.CreateFont("", 8);

            this.toolbarFont = this.CreateFont(World.Settings.ToolbarFontName, World.Settings.ToolbarFontSize, World.Settings.ToolbarFontStyle);

            bitmap = new System.Drawing.Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics = System.Drawing.Graphics.FromImage(bitmap);
            //	InitializeReference();
        }

        System.Windows.Forms.Control m_ReferenceForm;
        private void InitializeReference()
        {
            PresentParameters presentParameters = new PresentParameters();
            presentParameters.Windowed = true;
            presentParameters.SwapEffect = SwapEffect.Discard;
            presentParameters.AutoDepthStencilFormat =Format.D16;
            presentParameters.EnableAutoDepthStencil = true;

            this.m_ReferenceForm = new System.Windows.Forms.Control("Reference", 0, 0, 1, 1);
            this.m_ReferenceForm.Visible = false;

            int adapterOrdinal = 0;
            try
            {
                // Store the default adapter
                adapterOrdinal = Manager.Adapters.Default.Adapter;
            }
            catch
            {
                // User probably needs to upgrade DirectX or install a 3D capable graphics adapter
                throw new NotAvailableException();
            }


            CreateFlags flags = CreateFlags.SoftwareVertexProcessing;

            flags |= CreateFlags.Multithreaded | CreateFlags.FpuPreserve;
        }

        private void OnDeviceReset(object sender, EventArgs e)
        {
            using (new DirectXProfilerEvent("DrawArgs::OnDeviceReset"))
            {
                // Can we use anisotropic texture minify filter?
                if ((this.m_Device3dReference.Capabilities.TextureFilterCaps & FilterCaps.MinAnisotropic) != 0)
                {
                    this.m_Device3dReference.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
                }
                else if ((this.m_Device3dReference.Capabilities.TextureFilterCaps & FilterCaps.MinLinear) != 0)
                {
                    this.m_Device3dReference.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                }

                // What about magnify filter?
                if ((this.m_Device3dReference.Capabilities.TextureFilterCaps & FilterCaps.MagAnisotropic) != 0)
                {
                    this.m_Device3dReference.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
                }
                else if ((this.m_Device3dReference.Capabilities.TextureFilterCaps & FilterCaps.MagLinear) != 0)
                {
                    this.m_Device3dReference.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
                }

                this.m_Device3dReference.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
                this.m_Device3dReference.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);

                this.m_Device3dReference.SetRenderState(RenderState.Clipping, true);
                this.m_Device3dReference.SetRenderState(RenderState.CullMode, Cull.Clockwise);
                this.m_Device3dReference.SetRenderState(RenderState.Lighting,  false);
                this.m_Device3dReference.SetRenderState(RenderState.Ambient, Color.FromAbgr(0x40, 0x40, 0x40));

                this.m_Device3dReference.SetRenderState(RenderState.ZEnable, true);
                this.m_Device3dReference.SetRenderState(RenderState.AlphaBlendEnable, true);
                this.m_Device3dReference.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                this.m_Device3dReference.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            }
        }

        Device m_Device3dReference = null;
        public void BeginRender()
        {
            // Development variable to see the number of tiles drawn - Added for frustum culling testing
            this.numberTilesDrawn = 0;

            this.TexturesLoadedThisFrame = 0;

            this.UpperLeftCornerText = "";
            this.numBoundaryPointsRendered = 0;
            this.numBoundaryPointsTotal = 0;
            this.numBoundariesDrawn = 0;

            this.isPainting = true;
        }

        public void EndRender()
        {
            Debug.Assert(this.isPainting);
            this.isPainting = false;
        }

        /// <summary>
        /// Displays the rendered image (call after EndRender)
        /// </summary>
        public void Present()
        {
            // Calculate frame time
            long previousFrameStartTicks = CurrentFrameStartTicks;
            PerformanceTimer.QueryPerformanceCounter(ref CurrentFrameStartTicks);
            LastFrameSecondsElapsed = (CurrentFrameStartTicks - previousFrameStartTicks) /
                (float)PerformanceTimer.TicksPerSecond;

            // Display the render
            this.device.Present();
        }

        /// <summary>
        /// Creates a font.
        /// </summary>
        public Font CreateFont(string familyName, float emSize)
        {
            return this.CreateFont(familyName, emSize, System.Drawing.FontStyle.Regular);
        }

        /// <summary>
        /// Creates a font.
        /// </summary>
        public Font CreateFont(string familyName, float emSize, System.Drawing.FontStyle style)
        {
            try
            {
                FontDescription description = new FontDescription
                {
                    FaceName = familyName, Height = (int) (1.9 * emSize)
                };

                if (style == System.Drawing.FontStyle.Regular)
                    return this.CreateFont(description);
                if ((style & System.Drawing.FontStyle.Italic) != 0)
                    description.Italic = true;
                if ((style & System.Drawing.FontStyle.Bold) != 0)
                    description.Weight = FontWeight.Heavy;
                description.Quality = FontQuality.Antialiased;
                return this.CreateFont(description);
            }
            catch
            {
                Log.Write(Log.Levels.Error, "FONT", string.Format("Unable to load '{0}' {2} ({1}em)",
                    familyName, emSize, style));
                return this.defaultDrawingFont;
            }
        }

        /// <summary>
        /// Creates a font.
        /// </summary>
        public Font CreateFont(FontDescription description)
        {
            try
            {
                if (World.Settings.AntiAliasedText)
                    description.Quality = FontQuality.ClearTypeNatural;
                else
                    description.Quality = FontQuality.Default;

                // TODO: Improve font cache
                string hash = description.ToString();//.GetHashCode(); returned hash codes are not correct

                Font font = (Font)this.fontList[hash];
                if (font != null)
                    return font;

                font = new Font(this.device, description);
                //newDrawingFont.PreloadText("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXRZ");
                this.fontList.Add(hash, font);
                return font;
            }
            catch
            {
                Log.Write(Log.Levels.Error, "FONT", string.Format("Unable to load '{0}' (Height: {1})", description.FaceName, description.Height));
                return this.defaultDrawingFont;
            }
        }

        /// <summary>
        /// Active mouse cursor
        /// </summary>
        public static CursorType MouseCursor
        {
            get
            {
                return mouseCursor;
            }
            set
            {
                mouseCursor = value;
            }
        }

        public void UpdateMouseCursor(System.Windows.Forms.Control parent)
        {
            if (lastCursor == mouseCursor)
                return;

            switch (mouseCursor)
            {
                case CursorType.Hand:
                    parent.Cursor = System.Windows.Forms.Cursors.Hand;
                    break;
                case CursorType.Cross:
                    parent.Cursor = System.Windows.Forms.Cursors.Cross;
                    break;
                case CursorType.Measure:
                    if (this.measureCursor == null) this.measureCursor = ImageHelper.LoadCursor("measure.cur");
                    parent.Cursor = this.measureCursor;
                    break;
                case CursorType.SizeWE:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeWE;
                    break;
                case CursorType.SizeNS:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeNS;
                    break;
                case CursorType.SizeNESW:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeNESW;
                    break;
                case CursorType.SizeNWSE:
                    parent.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
                    break;
                default:
                    parent.Cursor = System.Windows.Forms.Cursors.Arrow;
                    break;
            }
            lastCursor = mouseCursor;
        }

        /// <summary>
        /// Returns the time elapsed since last frame render operation started.
        /// </summary>
        public static float SecondsSinceLastFrame
        {
            get
            {
                long curTicks = 0;
                PerformanceTimer.QueryPerformanceCounter(ref curTicks);
                float elapsedSeconds = (curTicks - CurrentFrameStartTicks) / (float)PerformanceTimer.TicksPerSecond;
                return elapsedSeconds;
            }
        }

        public bool IsPainting
        {
            get
            {
                return this.isPainting;
            }
        }

        public bool Repaint
        {
            get
            {
                return this.repaint;
            }
            set
            {
                this.repaint = value;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IDisposable font in this.fontList.Values)
            {
                if (font != null)
                {
                    font.Dispose();
                }
            }

            this.fontList.Clear();

            if (this.measureCursor != null)
            {
                this.measureCursor.Dispose();
                this.measureCursor = null;
            }

            if (DownloadQueue != null)
            {
                DownloadQueue.Dispose();
                DownloadQueue = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Mouse cursor
    /// </summary>
    public enum CursorType
    {
        Arrow = 0,
        Hand,
        Cross,
        Measure,
        SizeWE,
        SizeNS,
        SizeNESW,
        SizeNWSE
    }
}
