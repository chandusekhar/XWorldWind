using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;
using WorldWind.Configuration;
using WorldWind.Menu;

namespace WorldWind
{

	public enum MeasureMode
	{
		Single,
		Multi
	}

	/// <summary>
	/// World user configurable settings
	/// TODO: Group settings
	/// </summary>
	public class WorldSettings : SettingsBase
	{
		#region Atmosphere
		internal bool enableAtmosphericScattering;

		[Browsable(true), Category("Atmosphere")]
		[Description("Enable Atmospheric Scattering")]
		public bool EnableAtmosphericScattering
		{
			get { return this.enableAtmosphericScattering; }
			set { this.enableAtmosphericScattering = value; }
		}

		internal bool forceCpuAtmosphere = true;
		[Browsable(true), Category("Atmosphere")]
		[Description("Forces CPU calculation instead of GPU for Atmospheric Scattering")]
		public bool ForceCpuAtmosphere
		{
			get { return this.forceCpuAtmosphere; }
			set { this.forceCpuAtmosphere = value; }
		}

		#endregion

		#region UI

		/// <summary>
		/// Show the top tool button bar
		/// </summary>
		internal bool showToolbar = true;

        /// <summary>
        /// Where the tool bar should be anchored
        /// </summary>
        internal MenuAnchor toolbarAnchor = MenuAnchor.Top;

		/// <summary>
		/// Display the layer manager window
		/// </summary>
		internal bool showLayerManager;

		/// <summary>
		/// Display cross-hair symbol on screen
		/// </summary>
		internal bool showCrosshairs;
        public int crosshairColor = Color.Beige.ToArgb();
        internal int crosshairSize = 10;

		/// <summary>
		/// Font name for the default font used in UI
		/// </summary>
		internal string defaultFontName = "Tahoma";

		/// <summary>
		/// Font size (em) for the default font used in UI
		/// </summary>
		internal float defaultFontSize = 9.0f;

		/// <summary>
		/// Font style for the default font used in UI
		/// </summary>
		internal FontStyle defaultFontStyle = FontStyle.Regular;

		/// <summary>
		/// Font name used in the toolbar 
		/// </summary>
		internal string toolbarFontName = "Tahoma";

		/// <summary>
		/// Font size (em) for the font used in UI
		/// </summary>
		internal float toolbarFontSize = 8;

		/// <summary>
		/// Font style for the font used in UI
		/// </summary>
		internal FontStyle toolbarFontStyle = FontStyle.Bold;

		/// <summary>
		/// Menu bar background color
		/// </summary>
		public int menuBarBackgroundColor = Color.FromArgb(128, 128, 128, 128).ToArgb();

		/// <summary>
		/// Font name used in the layer manager 
		/// </summary>
		internal string layerManagerFontName = "Tahoma";

		/// <summary>
		/// Font size (em) for the font used in UI
		/// </summary>
		internal float layerManagerFontSize = 9;

		/// <summary>
		/// Font style for the font used in layer manager
		/// </summary>
		internal FontStyle layerManagerFontStyle = FontStyle.Regular;

		/// <summary>
		/// Layer manager width (pixels)
		/// </summary>
		internal int layerManagerWidth = 200;

		/// <summary>
		/// Draw anti-aliased text
		/// </summary>
		internal bool antiAliasedText;

		/// <summary>
		/// Maximum frames-per-second setting
		/// </summary>
		internal int throttleFpsHz = 50;

		/// <summary>
		/// Vsync on/off (Wait for vertical retrace)
		/// </summary>
		internal bool vSync = true;

		/// <summary>
		/// Rapid Fire MODIS icon size
		/// </summary>
		internal int modisIconSize = 60;

		internal int m_FpsFrameCount = 300;
		internal bool m_ShowFpsGraph;

		internal int downloadTerrainRectangleColor = Color.FromArgb(50, 0, 0, 255).ToArgb();
		internal int downloadProgressColor = Color.FromArgb(50, 255, 0, 0).ToArgb();
		internal int downloadLogoColor = Color.FromArgb(180, 255, 255, 255).ToArgb();
		public int menuBackColor = Color.FromArgb(170, 40, 40, 40).ToArgb();
		internal int menuOutlineColor = Color.FromArgb(150, 160, 160, 160).ToArgb();
		internal int widgetBackgroundColor = Color.FromArgb(0, 0, 0, 255).ToArgb();
		internal int scrollbarColor = Color.FromArgb(170, 100, 100, 100).ToArgb();
		internal int scrollbarHotColor = Color.FromArgb(170, 255, 255, 255).ToArgb();
		public int toolBarBackColor = Color.FromArgb(100, 255, 255, 255).ToArgb();
		internal bool showDownloadIndicator = true;
		internal bool outlineText;
		internal bool showCompass;
        internal WFSNameColors nameColors = WFSNameColors.Default;
        internal float nameSizeMultiplier = 1.0f;

		internal bool browserVisible;
		internal bool browserOrientationHorizontal;
		internal int browserSize = 300;
		internal bool useInternalBrowser = true;
		internal bool useOfflineSearch;

		[Browsable(true), Category("UI")]
		[Description("Show Compass Indicator.")]
		public bool ShowCompass
		{
			get { return this.showCompass; }
			set { this.showCompass = value; }
		}


		[Browsable(true), Category("UI")]
		[Description("Draw outline around WFS text to improve visibility.")]
		public bool WFSOutlineText
		{
			get { return this.outlineText; }
			set { this.outlineText = value; }
		}

        [Browsable(true), Category("UI")]
        [Description("Change name colors for visibility.")]
        public WFSNameColors WFSNameColors
        {
            get { return this.nameColors; }
            set { this.nameColors = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Factor by which default text size will be multiplied")]
        public float WFSNameSizeMultiplier
        {
            get { return this.nameSizeMultiplier; }
            set { 
                if (value <0.1f || value > 10f)
                    throw new ArgumentException("WFSNameSize out of range: " + value);
                else
                    this.nameSizeMultiplier = value;
            }
        }

		[Browsable(true), Category("UI")]
		[Description("Display download progress and rectangles.")]
		public bool ShowDownloadIndicator
		{
			get { return this.showDownloadIndicator; }
			set { this.showDownloadIndicator = value; }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Toolbar Background color.")]
		public Color ToolBarBackColor
		{
			get { return Color.FromArgb(this.toolBarBackColor); }
			set { this.toolBarBackColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color of scrollbar when scrolling.")]
		public Color ScrollbarHotColor
		{
			get { return Color.FromArgb(this.scrollbarHotColor); }
			set { this.scrollbarHotColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color of scrollbar.")]
		public Color ScrollbarColor
		{
			get { return Color.FromArgb(this.scrollbarColor); }
			set { this.scrollbarColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Menu border color.")]
		public Color MenuOutlineColor
		{
			get { return Color.FromArgb(this.menuOutlineColor); }
			set { this.menuOutlineColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Widget background color.")]
		public Color WidgetBackgroundColor
		{
			get { return Color.FromArgb(this.widgetBackgroundColor); }
			set { this.widgetBackgroundColor = value.ToArgb(); }
		}

        [XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Background color of the menu.")]
		public Color MenuBackColor
		{
			get { return Color.FromArgb(this.menuBackColor); }
			set { this.menuBackColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color/transparency of the download progress icon.")]
		public Color DownloadLogoColor
		{
			get { return Color.FromArgb(this.downloadLogoColor); }
			set { this.downloadLogoColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color of the download progress bar.")]
		public Color DownloadProgressColor
		{
			get { return Color.FromArgb(this.downloadProgressColor); }
			set { this.downloadProgressColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color of the terrain download in progress rectangle.")]
		public Color DownloadTerrainRectangleColor
		{
			get { return Color.FromArgb(this.downloadTerrainRectangleColor); }
			set { this.downloadTerrainRectangleColor = value.ToArgb(); }
		}

		[Browsable(true), Category("UI")]
		[Description("Show the top tool button bar.")]
		public bool ShowToolbar
		{
			get { return this.showToolbar; }
			set { this.showToolbar = value; }
		}

        [Browsable(true), Category("UI")]
        [Description("Where the toolbar is anchored.")]
        public MenuAnchor ToolbarAnchor
        {
            get { return this.toolbarAnchor; }
            set { this.toolbarAnchor = value; }
        }

		[Browsable(true), Category("UI")]
		[Description("Display the layer manager window.")]
		public bool ShowLayerManager
		{
			get { return this.showLayerManager; }
			set { this.showLayerManager = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Display cross-hair symbol on screen.")]
		public bool ShowCrosshairs
		{
			get { return this.showCrosshairs; }
			set { this.showCrosshairs = value; }
		}

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Cross-hair symbol color.")]
        public Color CrosshairColor
        {
            get { return Color.FromArgb(this.crosshairColor); }
            set { this.crosshairColor = value.ToArgb(); }
        }

        [Browsable(true), Category("UI")]
        [Description("Size of cross-hair.")]
        public int CrosshairSize
        {
            get { return this.crosshairSize; }
            set { this.crosshairSize = value; }
        }

		[Browsable(true), Category("UI")]
		[Description("Font name for the default font used in UI.")]
		public string DefaultFontName
		{
			get { return this.defaultFontName; }
			set { this.defaultFontName = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Font size for the default font used in UI.")]
		public float DefaultFontSize
		{
			get { return this.defaultFontSize; }
			set { this.defaultFontSize = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Font style for the default font used in UI.")]
		public FontStyle DefaultFontStyle
		{
			get { return this.defaultFontStyle; }
			set { this.defaultFontStyle = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Font name for the toolbar font used in UI.")]
		public string ToolbarFontName
		{
			get { return this.toolbarFontName; }
			set { this.toolbarFontName = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Font size (em) for the toolbar font used in UI.")]
		public float ToolbarFontSize
		{
			get { return this.toolbarFontSize; }
			set { this.toolbarFontSize = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Font style for the toolbar font used in UI.")]
		public FontStyle ToolbarFontStyle
		{
			get { return this.toolbarFontStyle; }
			set { this.toolbarFontStyle = value; }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Menu bar background color.")]
		public Color MenuBarBackgroundColor
		{
			get { return Color.FromArgb(this.menuBarBackgroundColor); }
			set { this.menuBarBackgroundColor = value.ToArgb(); }
		}

		[Browsable(true), Category("UI")]
		[Description("Font name for the layer manager font.")]
		public string LayerManagerFontName
		{
			get { return this.layerManagerFontName; }
			set { this.layerManagerFontName = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Font size for the layer manager font.")]
		public float LayerManagerFontSize
		{
			get { return this.layerManagerFontSize; }
			set { this.layerManagerFontSize = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Font style for the layer manager font used in UI.")]
		public FontStyle LayerManagerFontStyle
		{
			get { return this.layerManagerFontStyle; }
			set { this.layerManagerFontStyle = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Layer manager width (pixels)")]
		public int LayerManagerWidth
		{
			get { return this.layerManagerWidth; }
			set { this.layerManagerWidth = value; }
		}

		/// <summary>
		/// Draw anti-aliased text
		/// </summary>
		[Browsable(true), Category("UI")]
		[Description("Enable anti-aliased text rendering. Change active only after program restart.")]
		public bool AntiAliasedText
		{
			get { return this.antiAliasedText; }
			set { this.antiAliasedText = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Maximum frames-per-second setting. Optionally throttles the frame rate (to get consistent frame rates or reduce CPU usage. 0 = Disabled")]
		public int ThrottleFpsHz
		{
			get { return this.throttleFpsHz; }
			set { this.throttleFpsHz = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Synchronize render buffer swaps with the monitor's refresh rate (vertical retrace). Change active only after program restart.")]
		public bool VSync
		{
			get { return this.vSync; }
			set { this.vSync = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Changes the size of the Rapid Fire Modis icons.")]
		public int ModisIconSize
		{
			get { return this.modisIconSize; }
			set { this.modisIconSize = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Enables the Frames Per Second Graph")]
		public bool ShowFpsGraph
		{
			get { return this.m_ShowFpsGraph; }
			set { this.m_ShowFpsGraph = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Changes length of the Fps Graph History")]
		public int FpsFrameCount
		{
			get { return this.m_FpsFrameCount; }
			set { this.m_FpsFrameCount = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Initial visiblity of browser.")]
		public bool BrowserVisible
		{
			get { return this.browserVisible; }
			set { this.browserVisible = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Browser orientation.")]
		public bool BrowserOrientationHorizontal
		{
			get { return this.browserOrientationHorizontal; }
			set { this.browserOrientationHorizontal = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Size of browser panel.")]
		public int BrowserSize
		{
			get { return this.browserSize; }
			set { this.browserSize = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Use Internal Browser?")]
		public bool UseInternalBrowser
		{
			get { return this.useInternalBrowser; }
			set { this.useInternalBrowser = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Use Offline Placename Search?")]
		public bool UseOfflineSearch
		{
			get { return this.useOfflineSearch; }
			set { this.useOfflineSearch = value; }
		}

        internal int downloadQueuedColor = Color.FromArgb(50, 128, 168, 128).ToArgb();

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of queued for download image tile rectangles.")]
        public Color DownloadQueuedColor
        {
            get { return Color.FromArgb(this.downloadQueuedColor); }
            set { this.downloadQueuedColor = value.ToArgb(); }
        }

		#endregion

		#region Grid

		/// <summary>
		/// Display the latitude/longitude grid
		/// </summary>
		internal bool showLatLonLines;

		/// <summary>
		/// The color of the latitude/longitude grid
		/// </summary>
		public int latLonLinesColor = Color.FromArgb(200, 160, 160, 160).ToArgb();

		/// <summary>
		/// The color of the equator latitude line
		/// </summary>
		public int equatorLineColor = Color.FromArgb(160, 64, 224, 208).ToArgb();

		/// <summary>
		/// Display the tropic of capricorn/cancer lines
		/// </summary>
		internal bool showTropicLines = true;

		/// <summary>
		/// The color of the latitude/longitude grid
		/// </summary>
		public int tropicLinesColor = Color.FromArgb(160, 176, 224, 230).ToArgb();

		[Browsable(true), Category("Grid Lines")]
		[Description("Display the latitude/longitude grid.")]
		public bool ShowLatLonLines
		{
			get { return this.showLatLonLines; }
			set { this.showLatLonLines = value; }
		}

		[XmlIgnore]
		[Browsable(true), Category("Grid Lines")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("The color of the latitude/longitude grid.")]
		public Color LatLonLinesColor
		{
			get { return Color.FromArgb(this.latLonLinesColor); }
			set { this.latLonLinesColor = value.ToArgb(); }
		}

		[XmlIgnore]
		[Browsable(true), Category("Grid Lines")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("The color of the equator latitude line.")]
		public Color EquatorLineColor
		{
			get { return Color.FromArgb(this.equatorLineColor); }
			set { this.equatorLineColor = value.ToArgb(); }
		}

		[Browsable(true), Category("Grid Lines")]
		[Description("Display the tropic latitude lines.")]
		public bool ShowTropicLines
		{
			get { return this.showTropicLines; }
			set { this.showTropicLines = value; }
		}

		[XmlIgnore]
		[Browsable(true), Category("Grid Lines")]
		[Description("The color of the latitude/longitude grid.")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public Color TropicLinesColor
		{
			get { return Color.FromArgb(this.tropicLinesColor); }
			set { this.tropicLinesColor = value.ToArgb(); }
		}

		#endregion

		#region World

        /// <summary>
        /// Index of blue marble version to show
        /// </summary>
        internal int bmngVersion = 1;

		/// <summary>
		/// Whether to display the planet axis line (through poles)
		/// </summary>
		internal bool showPlanetAxis;

		/// <summary>
		/// Whether place name labels should display
		/// </summary>
		internal bool showPlacenames = true;

		/// <summary>
		/// Whether country borders and other boundaries should display
		/// </summary>
		internal bool showBoundaries;

		/// <summary>
		/// Displays coordinates of current position
		/// </summary>
		internal bool showPosition;


		/// <summary>
		/// Color of the sky at sea level
		/// </summary>
		internal int skyColor = Color.FromArgb(115, 155, 185).ToArgb();

		[XmlIgnore]
		[Browsable(true), Category("World")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color of the sky at sea level.")]
		public Color SkyColor
		{
			get { return Color.FromArgb(this.skyColor); }
			set { this.skyColor = value.ToArgb(); }
		}

		/// <summary>
		/// Keep the original (unconverted) NASA SVS image files on disk (in addition to converted files). 
		/// </summary>
		internal bool keepOriginalSvsImages;

		[Browsable(true), Category("World")]
		[Description("Whether to display the planet axis line (through poles).")]
		public bool ShowPlanetAxis
		{
			get { return this.showPlanetAxis; }
			set { this.showPlanetAxis = value; }
		}

		internal bool showClouds = true;
		[Browsable(true), Category("World")]
		[Description("Whether to show clouds.")]
		public bool ShowClouds
		{
			get { return this.showClouds; }
			set { this.showClouds = value; }
		}

		[Browsable(true), Category("World")]
		[Description("Whether place name labels should display")]
		public bool ShowPlacenames
		{
			get { return this.showPlacenames; }
			set { this.showPlacenames = value; }
		}

		[Browsable(true), Category("World")]
		[Description("Whether country borders and other boundaries should display")]
		public bool ShowBoundaries
		{
			get { return this.showBoundaries; }
			set { this.showBoundaries = value; }
		}

		[Browsable(true), Category("World")]
		[Description("Displays coordinates of current position.")]
		public bool ShowPosition
		{
			get { return this.showPosition; }
			set { this.showPosition = value; }
		}

		[Browsable(true), Category("World")]
		[Description("Keep the original (unconverted) NASA SVS image files on disk (in addition to converted files). ")]
		public bool KeepOriginalSvsImages
		{
			get { return this.keepOriginalSvsImages; }
			set { this.keepOriginalSvsImages = value; }
		}

        [Browsable(false), Category("World")]
        [Description("Index of Blue Marble version to show.")]
        public int BmngVersion
        {
            get { return this.bmngVersion; }
            set { this.bmngVersion = value; }
        }

		#endregion

		#region Camera

		internal bool cameraResetsAtStartup = true;
		internal Angle cameraLatitude = Angle.FromDegrees(0.0);
		internal Angle cameraLongitude = Angle.FromDegrees(0.0);
		internal double cameraAltitudeMeters = 20000000;
		internal Angle cameraHeading = Angle.FromDegrees(0.0);
        internal Angle cameraTilt = Angle.FromDegrees(0.0);
        internal Angle cameraSwivel = Angle.FromDegrees(0.0);

        internal bool cameraHeadTracking;
        internal Angle cameraHeadTilt = Angle.FromDegrees(0.0);
        internal Angle cameraHeadSwivel = Angle.FromDegrees(0.0);
        internal double cameraHeadZoom = 0.0;

		internal bool cameraIsPointGoto = true;
		internal bool cameraHasInertia = true;
		internal bool cameraSmooth = true;
		internal bool cameraHasMomentum;
		internal bool cameraTwistLock = true;
        internal bool cameraSwivelLock = true;
		internal bool cameraBankLock = true;
		internal float cameraSlerpStandard = 0.35f;
		internal float cameraSlerpInertia = 0.05f;

		// Set to either Inertia or Standard slerp value
		internal float cameraSlerpPercentage = 0.05f;

		internal Angle cameraFov = Angle.FromRadians(Math.PI * 0.25f);
		internal Angle cameraFovMin = Angle.FromDegrees(5);
		internal Angle cameraFovMax = Angle.FromDegrees(150);
		internal float cameraZoomStepFactor = 0.015f;
		internal float cameraZoomAcceleration = 10f;
		internal float cameraZoomAnalogFactor = 1f;
		internal float cameraZoomStepKeyboard = 0.15f;
		internal float cameraRotationSpeed = 3.5f;
        internal bool elevateCameraLookatPoint = true;
        internal bool allowNegativeAltitude;
        internal bool allowNegativeTilt;

        [Browsable(true), Category("Camera")]
        public bool ElevateCameraLookatPoint
        {
            get { return this.elevateCameraLookatPoint; }
            set { this.elevateCameraLookatPoint = value; }
        }

        [Browsable(true), Category("Camera")]
        [Description("Allow camera to go below sea level - experimental.")]
        public bool AllowNegativeAltitude
        {
            get { return this.allowNegativeAltitude; }
            set { this.allowNegativeAltitude = value; }
        }

		[Browsable(true), Category("Camera")]
		public bool CameraResetsAtStartup
		{
			get { return this.cameraResetsAtStartup; }
			set { this.cameraResetsAtStartup = value; }
		}

		//[Browsable(true),Category("Camera")]
		public Angle CameraLatitude
		{
			get { return this.cameraLatitude; }
			set { this.cameraLatitude = value; }
		}

		//[Browsable(true),Category("Camera")]
		public Angle CameraLongitude
		{
			get { return this.cameraLongitude; }
			set { this.cameraLongitude = value; }
		}

		public double CameraAltitude
		{
			get { return this.cameraAltitudeMeters; }
			set { this.cameraAltitudeMeters = value; }
		}

		//[Browsable(true),Category("Camera")]
		public Angle CameraHeading
		{
			get { return this.cameraHeading; }
			set { this.cameraHeading = value; }
		}

		public Angle CameraTilt
		{
			get { return this.cameraTilt; }
			set { this.cameraTilt = value; }
		}

        [Browsable(true), Category("Camera")]
        public bool CameraIsPointGoto
		{
			get { return this.cameraIsPointGoto; }
			set { this.cameraIsPointGoto = value; }
		}

		[Browsable(true), Category("Camera")]
		[Description("Smooth camera movement.")]
		public bool CameraSmooth
		{
			get { return this.cameraSmooth; }
			set { this.cameraSmooth = value; }
		}

		[Browsable(true), Category("Camera")]
		[Description("See CameraSlerp settings for responsiveness adjustment.")]
		public bool CameraHasInertia
		{
			get { return this.cameraHasInertia; }
			set
			{
                this.cameraHasInertia = value;
                this.cameraSlerpPercentage = this.cameraHasInertia ? this.cameraSlerpInertia : this.cameraSlerpStandard;
			}
		}

		[Browsable(true), Category("Camera")]
		public bool CameraHasMomentum
		{
			get { return this.cameraHasMomentum; }
			set { this.cameraHasMomentum = value; }
		}

		[Browsable(true), Category("Camera")]
		public bool CameraTwistLock
		{
			get { return this.cameraTwistLock; }
			set { this.cameraTwistLock = value; }
		}

		[Browsable(true), Category("Camera")]
		public bool CameraBankLock
		{
			get { return this.cameraBankLock; }
			set { this.cameraBankLock = value; }
		}

        [Browsable(true), Category("Camera")]
        [Description("Whether or not to allow camera swivel as well as tilting.")]
        public bool CameraSwivelLock
        {
            get { return this.cameraSwivelLock; }
            set { this.cameraSwivelLock = value; }
        }

        [Browsable(true), Category("Camera")]
        [Description("Whether or not to allow camera swivel as well as tilting.")]
        public bool AllowNegativeTilt
        {
            get { return this.allowNegativeTilt; }
            set { this.allowNegativeTilt = value; }
        }

		[Browsable(true), Category("Camera")]
		[Description("Responsiveness of movement when inertia is enabled.")]
		public float CameraSlerpInertia
		{
			get { return this.cameraSlerpInertia; }
			set
			{
                this.cameraSlerpInertia = value;
				if (this.cameraHasInertia) this.cameraSlerpPercentage = this.cameraSlerpInertia;
			}
		}

		[Browsable(true), Category("Camera")]
		[Description("Responsiveness of movement when inertia is disabled.")]
		public float CameraSlerpStandard
		{
			get { return this.cameraSlerpStandard; }
			set
			{
                this.cameraSlerpStandard = value;
				if (!this.cameraHasInertia) this.cameraSlerpPercentage = this.cameraSlerpStandard;
			}
		}

		[Browsable(true), Category("Camera")]
		public Angle CameraFov
		{
			get { return this.cameraFov; }
			set { this.cameraFov = value; }
		}

		[Browsable(true), Category("Camera")]
		public Angle CameraFovMin
		{
			get { return this.cameraFovMin; }
			set { this.cameraFovMin = value; }
		}

		[Browsable(true), Category("Camera")]
		public Angle CameraFovMax
		{
			get { return this.cameraFovMax; }
			set { this.cameraFovMax = value; }
		}

		[Browsable(true), Category("Camera")]
		public float CameraZoomStepFactor
		{
			get { return this.cameraZoomStepFactor; }
			set
			{
				const float maxValue = 0.3f;
				const float minValue = 1e-4f;

				if (value >= maxValue)
					value = maxValue;
				if (value <= minValue)
					value = minValue;
                this.cameraZoomStepFactor = value;
			}
		}

		[Browsable(true), Category("Camera")]
		public float CameraZoomAcceleration
		{
			get { return this.cameraZoomAcceleration; }
			set
			{
				const float maxValue = 50f;
				const float minValue = 1f;

				if (value >= maxValue)
					value = maxValue;
				if (value <= minValue)
					value = minValue;

                this.cameraZoomAcceleration = value;
			}
		}

		[Browsable(true), Category("Camera")]
		[Description("Analog zoom factor (Mouse LMB+RMB)")]
		public float CameraZoomAnalogFactor
		{
			get { return this.cameraZoomAnalogFactor; }
			set { this.cameraZoomAnalogFactor = value; }
		}

		[Browsable(true), Category("Camera")]
		public float CameraZoomStepKeyboard
		{
			get { return this.cameraZoomStepKeyboard; }
			set
			{
				const float maxValue = 0.3f;
				const float minValue = 1e-4f;

				if (value >= maxValue)
					value = maxValue;
				if (value <= minValue)
					value = minValue;

                this.cameraZoomStepKeyboard = value;
			}
		}

		float m_cameraDoubleClickZoomFactor = 2.0f;
		[Browsable(true), Category("Camera")]
		public float CameraDoubleClickZoomFactor
		{
			get { return this.m_cameraDoubleClickZoomFactor; }
			set
			{
                this.m_cameraDoubleClickZoomFactor = value;
			}
		}

		[Browsable(true), Category("Camera")]
		public float CameraRotationSpeed
		{
			get { return this.cameraRotationSpeed; }
			set { this.cameraRotationSpeed = value; }
		}

        [Browsable(true), Category("Camera")]
        [Description("Enables and disables headtracking support in Camera. If false all headtracking values are zero'd every update")]
        public bool CameraHeadTracking
        {
            get { return this.cameraHeadTracking; }
            set { this.cameraHeadTracking = value; }
        }

		#endregion

		#region Time

		[Browsable(true), Category("Time")]
		[Description("Controls the time multiplier for the Time Keeper.")]
		[XmlIgnore]
		public float TimeMultiplier
		{
			get { return TimeKeeper.TimeMultiplier; }
			set { TimeKeeper.TimeMultiplier = value; }
		}

		#endregion

		#region 3D

		private Format textureFormat = Format.Dxt3;
		private bool m_UseBelowNormalPriorityUpdateThread;
		private bool m_AlwaysRenderWindow;

		private bool convertDownloadedImagesToDds = true;
		[Browsable(true), Category("3D settings")]
		[Description("Enables image conversion to DDS files when loading images. TextureFormat controls the sub-format of the DDS file.")]
		public bool ConvertDownloadedImagesToDds
		{
			get
			{
				return this.convertDownloadedImagesToDds;
			}
			set
			{
                this.convertDownloadedImagesToDds = value;
			}
		}

		[Browsable(true), Category("3D settings")]
		[Description("Always Renders the 3D window even form is unfocused.")]
		public bool AlwaysRenderWindow
		{
			get
			{
				return this.m_AlwaysRenderWindow;
			}
			set
			{
                this.m_AlwaysRenderWindow = value;
			}
		}

		[Browsable(true), Category("3D settings")]
		[Description("In-memory texture format.  Also used for converted files on disk when image conversion is enabled.")]
		public Format TextureFormat
		{
			get
			{
				//	return Format.Dxt3;
				return this.textureFormat;
			}
			set
			{
                this.textureFormat = value;
			}
		}

		private bool m_enableSunShading;
		[Browsable(true), Category("3D settings")]
		[Description("Shade the Earth according to the Sun's position at a certain time.")]
		public bool EnableSunShading
		{
			get
			{
				return this.m_enableSunShading;
			}
			set
			{
                this.m_enableSunShading = value;
			}
		}

		private bool m_sunSynchedWithTime = true;
		[Browsable(true), Category("3D settings")]
		[Description("Sun position is computed according to time.")]
		public bool SunSynchedWithTime
		{
			get
			{
				return this.m_sunSynchedWithTime;
			}
			set
			{
                this.m_sunSynchedWithTime = value;
			}
		}

		private double m_sunElevation = Math.PI / 4;
		[Browsable(true), Category("3D settings")]
		[Description("Sun elevation when not synched to time.")]
		public double SunElevation
		{
			get
			{
				return this.m_sunElevation;
			}
			set
			{
                this.m_sunElevation = value;
			}
		}

		private double m_sunHeading = -Math.PI / 4;
		[Browsable(true), Category("3D settings")]
		[Description("Sun direction when not synched to time.")]
		public double SunHeading
		{
			get
			{
				return this.m_sunHeading;
			}
			set
			{
                this.m_sunHeading = value;
			}
		}

		private double m_sunDistance = 150000000000;
		[Browsable(true), Category("3D settings")]
		[Description("Sun distance in meter.")]
		public double SunDistance
		{
			get
			{
				return this.m_sunDistance;
			}
			set
			{
                this.m_sunDistance = value;
			}
		}
        private int m_LightColor = Color.FromArgb(255, 255, 255).ToArgb();
        [Browsable(true), Category("3D settings")]
        [Description("The light color when sun shading is enabled.")]
        [XmlIgnore]
        public Color LightColor
        {
            get
            {
                return Color.FromArgb(this.m_LightColor);
            }
            set
            {
                this.m_LightColor = value.ToArgb();
            }
        }

		private int m_shadingAmbientColor = Color.FromArgb(50, 50, 50).ToArgb();
		[Browsable(true), Category("3D settings")]
		[Description("The background ambient color when sun shading is enabled.")]
		[XmlIgnore]
		public Color ShadingAmbientColor
		{
			get
			{
				return Color.FromArgb(this.m_shadingAmbientColor);
			}
			set
			{
                this.m_shadingAmbientColor = value.ToArgb();
			}
		}

		private int m_standardAmbientColor = Color.FromArgb(64, 64, 64).ToArgb();
		[Browsable(true), Category("3D settings")]
		[Description("The background ambient color only ambient lighting is used.")]
		[XmlIgnore]
		public Color StandardAmbientColor
		{
			get
			{
				return Color.FromArgb(this.m_standardAmbientColor);
			}
			set
			{
                this.m_standardAmbientColor = value.ToArgb();
			}
		}

		[Browsable(true), Category("3D settings")]
		[Description("Use lower priority update thread to allow smoother rendering at the expense of data update frequency.")]
		public bool UseBelowNormalPriorityUpdateThread
		{
			get
			{
				return this.m_UseBelowNormalPriorityUpdateThread;
			}
			set
			{
                this.m_UseBelowNormalPriorityUpdateThread = value;
			}
		}

		#endregion

		#region Terrain

		private float minSamplesPerDegree = 3.0f;

		[Browsable(true), Category("Terrain")]
		[Description("Sets the minimum samples per degree for which elevation is applied.")]
		public float MinSamplesPerDegree
		{
			get
			{
				return this.minSamplesPerDegree;
			}
			set
			{
                this.minSamplesPerDegree = value;
			}
		}

		private bool useWorldSurfaceRenderer = true;

		[Browsable(true), Category("Terrain")]
		[Description("Use World Surface Renderer for the visualization of multiple terrain-mapped layers.")]
		public bool UseWorldSurfaceRenderer
		{
			get
			{
				return this.useWorldSurfaceRenderer;
			}
			set
			{
                this.useWorldSurfaceRenderer = value;
			}
		}

		private float verticalExaggeration = 3.0f;

		[Browsable(true), Category("Terrain")]
		[Description("Terrain height multiplier.")]
		public float VerticalExaggeration
		{
			get
			{
				return this.verticalExaggeration;
			}
			set
			{
				if (value > 20)
					throw new ArgumentException("Vertical exaggeration out of range: " + value);
				if (value <= 0)
                    this.verticalExaggeration = Single.Epsilon;
				else
                    this.verticalExaggeration = value;
			}
        }

        private TimeSpan terrainTileRetryInterval = TimeSpan.FromMinutes(30);

        [Browsable(true), Category("Terrain")]
        [Description("Retry Interval for missing terrain tiles.")]
        [XmlIgnore]
        public TimeSpan TerrainTileRetryInterval
        {
            get
            {
                return this.terrainTileRetryInterval;
            }
            set
            {
                TimeSpan minimum = TimeSpan.FromMinutes(1);
                if (value < minimum)
                    value = minimum;
                this.terrainTileRetryInterval = value;
            }
        }

		#endregion

		#region Measure tool

		internal MeasureMode measureMode;

		internal bool measureShowGroundTrack;

		internal int measureLineGroundColor = Color.FromArgb(222, 0, 255, 0).ToArgb();
		internal int measureLineLinearColor = Color.FromArgb(255, 255, 0, 0).ToArgb();

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color of the linear distance measure line.")]
		public Color MeasureLineLinearColor
		{
			get { return Color.FromArgb(this.measureLineLinearColor); }
			set { this.measureLineLinearColor = value.ToArgb(); }
		}

		[Browsable(false)]
		public int MeasureLineLinearColorXml
		{
			get { return this.measureLineLinearColor; }
			set { this.measureLineLinearColor = value; }
		}

		[XmlIgnore]
		[Browsable(true), Category("UI")]
		[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[Description("Color of the ground track measure line.")]
		public Color MeasureLineGroundColor
		{
			get { return Color.FromArgb(this.measureLineGroundColor); }
			set { this.measureLineGroundColor = value.ToArgb(); }
		}

		[Browsable(false)]
		public int MeasureLineGroundColorXml
		{
			get { return this.measureLineGroundColor; }
			set { this.measureLineGroundColor = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Display the ground track column in the measurement statistics table.")]
		public bool MeasureShowGroundTrack
		{
			get { return this.measureShowGroundTrack; }
			set { this.measureShowGroundTrack = value; }
		}

		[Browsable(true), Category("UI")]
		[Description("Measure tool operation mode.")]
		public MeasureMode MeasureMode
		{
			get { return this.measureMode; }
			set { this.measureMode = value; }
		}

		#endregion

		#region Units
		private Units m_displayUnits = Units.Metric;
		[Browsable(true), Category("Units")]
		[Description("The target display units for measurements.")]
		public Units DisplayUnits
		{
			get
			{
				return this.m_displayUnits;
			}
			set
			{
                this.m_displayUnits = value;
			}
		}
		#endregion

        private bool workOffline;

        [Browsable(true), Category("World")]
        public bool WorkOffline
        {
            get { return this.workOffline; }
            set
            {
                this.workOffline = value;
                this.showDownloadIndicator = !value;
            }
        }

		#region Layers
		internal System.Collections.ArrayList loadedLayers = new System.Collections.ArrayList();
		internal bool useDefaultLayerStates = true;
		internal int maxSimultaneousDownloads = 1;

		[Browsable(true), Category("Layers")]
		public bool UseDefaultLayerStates
		{
			get { return this.useDefaultLayerStates; }
			set { this.useDefaultLayerStates = value; }
		}

		[Browsable(true), Category("Layers")]
		public int MaxSimultaneousDownloads
		{
			get { return this.maxSimultaneousDownloads; }
			set
			{
				if (value > 20)
                    this.maxSimultaneousDownloads = 20;
				else if (value < 1)
                    this.maxSimultaneousDownloads = 1;
				else
                    this.maxSimultaneousDownloads = value;
			}
		}

		[Browsable(true), Category("Layers")]
		public System.Collections.ArrayList LoadedLayers
		{
			get { return this.loadedLayers; }
			set { this.loadedLayers = value; }
		}
		#endregion

		[Browsable(true), Category("Logging")]
		public bool Log404Errors
		{
			get { return Net.WebDownload.Log404Errors; }
			set { Net.WebDownload.Log404Errors = value; }
		}

		// comment out ToString() to have namespace+class name being used as filename
		public override string ToString()
		{
			return "World";
		}
	}
}