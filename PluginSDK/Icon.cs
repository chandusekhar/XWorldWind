//
// Copyright © 2005 NASA.  Available under the NOSA License
//
// Portions copied from JHU_Icon - Copyright © 2005-2006 The Johns Hopkins University 
// Applied Physics Laboratory.  Available under the JHU/APL Open Source Agreement.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind
{
	/// <summary>
	/// One icon in an icon layer
	/// </summary>
	public class Icon : RenderableObject
    {
        # region private variables
        
        /// <summary>
        /// Indicates if an update is required.  If isUpdated is false then the update will run.
        /// </summary>
        protected bool m_isUpdated;

        /// <summary>
        /// Indicates if a new texture needs to be loaded
        /// </summary>
        protected bool m_newTexture;

        protected Vector3 m_groundPoint;
        protected Line m_groundStick;
        protected Vector2[] m_groundStickVector;

        protected static int hotColor = Color.White.ToArgb();
        protected static int normalColor = Color.FromArgb(150, 255, 255, 255).ToArgb();
        protected static int nameColor = Color.White.ToArgb();
        protected static int descriptionColor = Color.White.ToArgb();

        // used only in default render
        protected Sprite m_sprite;

        // used only in default render
        protected List<Rectangle> m_labelRectangles;

        protected Matrix lastView = Matrix.Identity;

        /// <summary>
        /// The context menu for this icon
        /// </summary>
        protected ContextMenu m_contextMenu;

        /// <summary>
        /// This value is computed by anyone that would try to render this icon.
        /// It isn't a parameter because it is often compared to in order to 
        /// see how this icon should be rendered (PointSprite, Sprite, Mesh).
        /// </summary>
        protected internal float DistanceToIcon
        {
            get { return this.m_distanceToIcon; }
            set { this.m_distanceToIcon = value; }
        }
        private float m_distanceToIcon = float.MaxValue;

        #endregion

        #region Properties

        /// <summary>
		/// Whether the name of this icon should always be rendered
		/// </summary>
		public bool NameAlwaysVisible
		{
			get{ return this.m_nameAlwaysVisible; }
			set{ this.m_nameAlwaysVisible = value; }
		}
        private bool m_nameAlwaysVisible;

        /// <summary>
        /// Whether or not this icon should be rotated.
        /// </summary>
		public bool IsRotated
		{
			get { return this.m_isRotated; }
			set { this.m_isRotated = value; }
		}
        private bool m_isRotated;
	
        /// <summary>
        /// The angle of rotation to display the icon's texture in Degrees.
        /// </summary>
		public Angle Rotation
		{
			get { return this.m_rotation; }
			set { this.m_rotation = value; }
		}
        private Angle m_rotation = Angle.Zero;

        /// <summary>
        /// Latitude (North/South) in decimal degrees
        /// </summary>
        public double Latitude
        {
            get { return this.m_latitude.Degrees; }
            set
            {
                this.m_latitude.Degrees = value;
                this.m_isUpdated = false;
            }
        }
        private Angle m_latitude = Angle.Zero;

        /// <summary>
        /// Longitude (East/West) in decimal degrees
        /// </summary>
        public double Longitude
        {
            get { return this.m_longitude.Degrees; }
            set
            {
                this.m_longitude.Degrees = value;
                this.m_isUpdated = false;
            }

        }
        private Angle m_longitude = Angle.Zero;

        /// <summary>
        /// Latitude as an Angle class
        /// </summary>
        public Angle LatitudeAngle
        {
            get { return this.m_latitude; }
            set
            {
                this.m_latitude = value;
                this.m_isUpdated = false;
            }
        }

        /// <summary>
        /// Longitude as an Angle class
        /// </summary>
        public Angle LongitudeAngle
        {
            get { return this.m_longitude; }
            set
            {
                this.m_longitude = value;
                this.m_isUpdated = false;
            }
        }

        /// <summary>
        /// Latitude as a radian
        /// </summary>
        public double LatitudeRadians
        {
            get { return this.m_latitude.Radians; }
            set
            {
                this.m_latitude.Radians = value;
                this.m_isUpdated = false;
            }
        }

        /// <summary>
        /// Longitude as a radian
        /// </summary>
        public double LongitudeRadians
        {
            get { return this.m_longitude.Radians; }
            set
            {
                this.m_longitude.Radians = value;
                this.m_isUpdated = false;
            }
        }

        /// <summary>
        /// The icon altitude above the surface 
        /// </summary>
        public double Altitude
        {
            get { return this.m_altitude; }
            set
            {
                this.m_altitude = value;
                this.m_isUpdated = false;
            }
        }
        private double m_altitude;

        /// <summary>
        /// The cartesian coordinates of this icon.  
        /// Used to be settable but never actually updated the position of the icon.
        /// </summary>
        public Point3d PositionD
        {
            get { return this.m_positionD; }
        }
        private Point3d m_positionD = new Point3d();

        /// <summary>
        /// Icon bitmap path. (Overrides Image)
        /// </summary>
        public string TextureFileName
        {
            get { return this.m_textureFileName; }
            set
            {
                this.m_textureFileName = value;
                this.m_newTexture = true;
                this.m_isUpdated = false;
            }
        }
        private string m_textureFileName;

        /// <summary>
        /// The icon's texture
        /// </summary>
        public IconTexture IconTexture
        {
            get { return this.m_iconTexture; }
        }
        private IconTexture m_iconTexture;

        /// <summary>
        /// On-Click browse to location
        /// </summary>
        public string ClickableActionURL
        {
            get
            {
                return this.m_clickableActionURL;
            }
            set
            {
                this.isSelectable = value != null;
                this.m_clickableActionURL = value;
            }
        }
        private string m_clickableActionURL;

        /// <summary>
        /// Whether or not a groundstick should be drawn
        /// </summary>
        public bool DrawGroundStick
        {
            get { return this.m_drawGroundStick; }
            set
            {
                this.m_drawGroundStick = value;
                this.m_isUpdated = false;
            }
        }
        private bool m_drawGroundStick;

        /// <summary>
        /// Maximum distance at which to render ground sticks.  Default = max.
        /// </summary>
        public double MaxGroundStickDistance
        {
            get { return this.m_maxGroundStickDistance; }
            set { this.m_maxGroundStickDistance = value; }
        }
        private double m_maxGroundStickDistance = double.MaxValue;

        /// <summary>
        /// Whether or not the labels should be decluttered
        /// </summary>
        public bool Declutter
        {
            get { return this.m_declutter; }
            set { this.m_declutter = value; }
        }
        private bool m_declutter;

        /// <summary>
        /// An ID for this icon.  Depends on the plugin to assign but can be used to uniquely identify this icon
        /// without having an incomprehensible name.
        /// 
        /// TODO: Name is not always unique...determine if this is a problem since it may be used as a key in things.
        /// </summary>
        public string Id
        {
            get { return this.m_id; }
            set { this.m_id = value; }
        }
        protected string m_id;

        /// <summary>
        /// Any user defined icon related data to hang onto.
        /// 
        /// TODO:  Should this be in RO?
        /// </summary>
        public object Tag
        {
            get { return this.m_tag; }
            set { this.m_tag = value; }
        }
        protected object m_tag;

        /// <summary>
        /// If true the icon will autoscale based on altitude
        /// </summary>
        public bool AutoScaleIcon
        {
            get { return this.m_autoScaleIcon; }
            set { this.m_autoScaleIcon = value; }
        }
        protected bool m_autoScaleIcon;

        /// <summary>
        /// At what distance to start autoscaling
        /// </summary>
        public int MinIconZoomDistance
        {
            get { return this.m_minIconZoomDistance; }
            set { this.m_minIconZoomDistance = value; }
        }
        private int m_minIconZoomDistance = 2500000;

        /// <summary>
        /// The smallest to scale this icon based on zoom (default = 20%)
        /// </summary>
        public float MinScaleFactor
        {
            get { return this.m_minScaleFactor; }
            set { this.m_minScaleFactor = value; }
        }
        private float m_minScaleFactor = 0.20f;

        /// <summary>
        /// True if altitude is in AGL, False if ASL.  Default is AGL.
        /// </summary>
        public bool IsAGL
        {
            get { return this.m_isAGL; }
            set { this.m_isAGL = value; }
        }
        protected bool m_isAGL = true;

        /// <summary>
        /// True if Vertical Exaggeration should be used in computing altitude.  
        /// Default is true.
        /// </summary>
        public bool UseVE
        {
            get { return this.m_useVE; }
            set { this.m_useVE = value; }
        }
        protected bool m_useVE = true;        
        
        /// <summary>
        /// True if a zero Vertical Exaggeration should be used in computing altitude.  
        /// If set then a VE of 0 forces altitude to 0 since its multiplied.  Ignored if UseVE is false.
        /// Default is true.
        /// </summary>
        public bool UseZeroVE
        {
            get { return this.m_useZeroVE; }
            set { this.m_useZeroVE = value; }
        }
        protected bool m_useZeroVE = true;

        /// <summary>
        /// Whether or not this will change color on mouseover (default = true)
        /// </summary>
        public bool AlwaysHighlight
        {
            get
            {
                return this.m_alwaysHighlight;
            }
            set
            {
                this.m_alwaysHighlight = value;
            }
        }
        protected bool m_alwaysHighlight;

        /// <summary>
        /// Whether or not this will change color on mouseover (default = true)
        /// </summary>
        public bool DisableMouseoverHighlight
        {
            get
            {
                return this.m_disableMouseoverHighlight;
            }
            set
            {
                this.m_disableMouseoverHighlight = value;
            }
        }
        protected bool m_disableMouseoverHighlight;

        public bool OnClickZoomTo
        {
            get { return this.m_onClickZoomTo; }
            set { this.m_onClickZoomTo = value; }
        }
        protected bool m_onClickZoomTo = true;

        /// <summary>
        /// Whether or not we should render as a point sprite above a certain camera altitude.
        /// </summary>
        public bool UsePointSprite
        {
            get { return this.m_usePointSprite; }
            set { this.m_usePointSprite = value; }
        }
        private bool m_usePointSprite;

        /// <summary>
        /// Distance at which to render as PointSprite rather than as a sprite.  Default = 15000000m
        /// </summary>
        public double PointSpriteDistance
        {
            get { return this.m_pointSpriteDistance; }
            set { this.m_pointSpriteDistance = value; }
        }
        private double m_pointSpriteDistance = 1500000;

        /// <summary>
        /// Color to render the PointSprite.  Just sets lighting color.
        /// </summary>
        public Color PointSpriteColor
        {
            get { return this.m_pointSpriteColor; }
            set { this.m_pointSpriteColor = value; }
        }
        private Color m_pointSpriteColor = Color.Violet;

        /// <summary>
        /// The size to render this pointsprite
        /// </summary>
        public float PointSpriteSize
        {
            get { return this.m_pointSpriteSize; }
            set { this.m_pointSpriteSize = value; }
        }
        private float m_pointSpriteSize = .01f;

        /// <summary>
        /// Whether or not to render the texture.  Useful if you want to render groundstick with pointsprites 
        /// but not actually show this icon as a sprite when you do.  
        /// Set MaxGroundStickDistance to PointSpriteDistance, TrailShowDistance or ModelShowDistance 
        /// AlwaysRenderPointSprite to true and this to false.
        /// </summary>
        public bool TextureRenderEnabled
        {
            get { return this.m_textureRenderEnabled; }
            set { this.m_textureRenderEnabled = value; }
        }
        private bool m_textureRenderEnabled = true;

        /// <summary>
        /// Whether to always render as a pointsprite even when we render as a sprite
        /// </summary>
        public bool AlwaysRenderPointSprite
        {
            get { return this.m_alwaysRenderPointSprite; }
            set { this.m_alwaysRenderPointSprite = value; }
        }
        private bool m_alwaysRenderPointSprite;

        /// <summary>
        /// Whether or not this icon has been "hooked" to show descrption all of the time.
        /// </summary>
        /// <remarks>
        /// Is never set to true for base Icon objects but only those the implement hooking (TrackIcon) but
        /// this is here to allow for use by Icons to behave differently (specifically PointSprite behaviors).
        /// </remarks>
        public bool IsHooked
        {
            get { return this.m_isHooked; }
            set { this.m_isHooked = value; }
        }
        private bool m_isHooked;

        #endregion

        public double OnClickZoomAltitude = double.NaN;
        public double OnClickZoomHeading = double.NaN;
        public double OnClickZoomTilt = double.NaN;
        public string SaveFilePath = null;
        public DateTime LastRefresh = DateTime.MinValue;
        public TimeSpan RefreshInterval = TimeSpan.MaxValue;

		ArrayList overlays = new ArrayList();
		
		//not a good way to handle this
		public void OverlayOnOpen(object o, EventArgs e)
		{
			MenuItem mi = (MenuItem)o;

			foreach(ScreenOverlay overlay in this.overlays)
			{
				if(overlay == null)
					continue;

				if(overlay.Name.Equals(mi.Text))
				{
					if(!overlay.IsOn)
						overlay.IsOn = true;
				}
			}
		}

		public ScreenOverlay[] Overlays
		{
			get
			{
				if(this.overlays == null)
				{
					return null;
				}
				else
				{
					return (ScreenOverlay[]) this.overlays.ToArray(typeof(ScreenOverlay));
				}
			}
		}

		public void AddOverlay(ScreenOverlay overlay)
		{
			if(overlay != null) this.overlays.Add(overlay);
		}

		public void RemoveOverlay(ScreenOverlay overlay)
		{
			for(int i = 0; i < this.overlays.Count; i++)
			{
				ScreenOverlay curOverlay = (ScreenOverlay) this.overlays[i];
				if(curOverlay.IconImagePath == overlay.IconImagePath && overlay.Name == curOverlay.Name)
				{
                    this.overlays.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Icon image.  Leave TextureFileName=null if using Image.  
		/// Caller is responsible for disposing the Bitmap when the layer is removed, 
		/// either by calling Dispose on Icon or on the Image directly.
		/// </summary>
        public Bitmap Image
        {
            get { return this.m_image; }
            set 
            {
                this.m_image = value;
                this.m_newTexture = true;
                this.m_isUpdated = false;
            }
        }
        private Bitmap m_image;

		/// <summary>
		/// Icon on-screen rendered width (pixels).  Defaults to icon image width.  
		/// If source image file is not a valid GDI+ image format, width may be increased to closest power of 2.
		/// </summary>
        public int Width
        {
            get { return this.m_width; }
            set
            {
                this.m_width = value;
                this.m_newTexture = true;
                this.m_isUpdated = false;
            }
        }
        private int m_width;

		/// <summary>
		/// Icon on-screen rendered height (pixels).  Defaults to icon image height.  
		/// If source image file is not a valid GDI+ image format, height may be increased to closest power of 2.
		/// </summary>
        public int Height
        {
            get { return this.m_height; }
            set
            {
                this.m_height = value;
                this.m_newTexture = true;
                this.m_isUpdated = false;
            }
        }
        private int m_height;

        /// <summary>
        ///  Icon X scaling computed by dividing icon width by texture width
        /// </summary>
        public float XScale;

        /// <summary>
        ///  Icon Y scaling computed by dividing icon height by texture height 
        /// </summary>
        public float YScale;

		/// <summary>
		/// The maximum distance (meters) the icon will be visible from
		/// </summary>
		public double MaximumDisplayDistance = double.MaxValue;

		/// <summary>
		/// The minimum distance (meters) the icon will be visible from
		/// </summary>
		public double MinimumDisplayDistance;

		/// <summary>
		/// Bounding box centered at (0,0) used to calculate whether mouse is over icon/label
		/// </summary>
		public Rectangle SelectionRectangle;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		public Icon(string name,
			double latitude, 
			double longitude) : base( name )
		{
            this.m_latitude.Degrees = latitude;
            this.m_longitude.Degrees = longitude;
			this.RenderPriority = RenderPriority.Icons;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		public Icon(string name,
			double latitude, 
			double longitude,
            double heightAboveSurface) : this(name, latitude, longitude)
		{
            this.m_altitude = heightAboveSurface;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Icon"/> class 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="heightAboveSurface"></param>
        /// <param name="TextureFileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="actionURL"></param>
        public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			string TextureFileName,
			int width,
			int height,
			string actionURL) : this( name, latitude, longitude, heightAboveSurface )
		{
			this.Description = description;

			this.TextureFileName = TextureFileName;
			this.Width = width;
			this.Height = height;
            this.m_clickableActionURL = actionURL;
            this.isSelectable = actionURL != null;
		}

		#region Obsolete

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		[Obsolete]
		public Icon(string name,
			double latitude, 
			double longitude,
			double heightAboveSurface, 
			World parentWorld ) : base( name )
		{
            this.m_latitude.Degrees = latitude;
            this.m_longitude.Degrees = longitude;
			this.Altitude = heightAboveSurface;
			this.RenderPriority = RenderPriority.Icons;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		[Obsolete]
		public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			World parentWorld, 
			Bitmap image,
			int width,
			int height,
			string actionURL) : base( name )
		{
			this.Description = description;
            this.m_latitude.Degrees = latitude;
            this.m_longitude.Degrees = longitude;
			this.Altitude = heightAboveSurface;
			this.m_image = image;
			this.Width = width;
			this.Height = height;
            this.ClickableActionURL = actionURL;
			this.RenderPriority = RenderPriority.Icons;
            this.isSelectable = actionURL != null;

		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Icon"/> class 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="heightAboveSurface"></param>
		/// <param name="parentWorld"></param>
		/// <param name="TextureFileName"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="actionURL"></param>
		[Obsolete]
		public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			World parentWorld, 
			string TextureFileName,
			int width,
			int height,
			string actionURL) : base( name )
		{
			this.Description = description;
            this.m_latitude.Degrees = latitude;
            this.m_longitude.Degrees = longitude;
			this.Altitude = heightAboveSurface;
			this.TextureFileName = TextureFileName;
			this.Width = width;
			this.Height = height;
            this.ClickableActionURL = actionURL;
			this.RenderPriority = RenderPriority.Icons;
            this.isSelectable = actionURL != null;

		}

		#endregion

		/// <summary>
		/// Sets the geographic position of the icon.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
        public virtual void SetPosition(double latitude, double longitude)
		{
            this.m_latitude.Degrees = latitude;
            this.m_longitude.Degrees = longitude;

			// Recalculate XYZ coordinates
            this.m_isUpdated = false;
        }

		/// <summary>
		/// Sets the geographic position of the icon.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="altitude">The icon altitude above sea level.</param>
        public virtual void SetPosition(double latitude, double longitude, double altitude)
		{
            this.m_latitude.Degrees = latitude;
            this.m_longitude.Degrees = longitude;
            this.m_altitude = altitude;

			// Recalculate XYZ coordinates
            this.m_isUpdated = false;
        }

		#region RenderableObject methods

		public override void Initialize(DrawArgs drawArgs)
		{
            // get icon texture
            this.m_iconTexture = null;

            this.BuildIconTexture(drawArgs);

            if (this.m_drawGroundStick)
            {
                if (this.m_groundStick == null) this.m_groundStick = new Line(drawArgs.device);

                if (this.m_groundStickVector == null) this.m_groundStickVector = new Vector2[2];

                this.m_groundStick.Antialias = true;
            }

            this.isInitialized = true;
		}

		/// <summary>
		/// Disposes the icon (when disabled)
		/// </summary>
		public override void Dispose()
		{
            try
            {
                IconTexture iconTexture = null;
                // decrement our count from textures - the icons class will clean up
                if ((this.TextureFileName != null) && (this.TextureFileName.Trim() != String.Empty))
                    iconTexture = (IconTexture)DrawArgs.Textures[this.TextureFileName];
                else if (this.m_image != null)
                    iconTexture = (IconTexture)DrawArgs.Textures[this.m_image];

                if (iconTexture != null)
                {
                    iconTexture.ReferenceCount--;
                }

                if (this.m_sprite != null)
                {
                    this.m_sprite.Dispose();
                    this.m_sprite = null;
                }

                this.isInitialized = false;

            }
            finally
            {
                // base.Dispose();
            }

		}

        /// <summary>
        /// If LMB pressed calls PerformLMBAction, if RMB pressed calls PerformRMBAction
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <returns></returns>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
            if (!this.isSelectable)
                return false;

            if (DrawArgs.IsLeftMouseButtonDown)
                return this.PerformLMBAction(drawArgs);

            if (DrawArgs.IsRightMouseButtonDown)
                return this.PerformRMBAction(drawArgs);

			return false;
		}

        /// <summary>
        /// Goes to icon if camera positions set.  Also opens URL if it exists
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <returns></returns>
        protected virtual bool PerformLMBAction(DrawArgs drawArgs)
        {
            try
            {
                // Goto icon
                if (this.OnClickZoomTo && (this.OnClickZoomAltitude != double.NaN 
                                           || this.OnClickZoomHeading != double.NaN || this.OnClickZoomTilt != double.NaN))
                {
                    drawArgs.WorldCamera.SetPosition(this.Latitude, this.Longitude, this.OnClickZoomHeading, this.OnClickZoomAltitude, this.OnClickZoomTilt);
                }

                // Goto to URL if we have one
                if (this.ClickableActionURL!=null && !this.ClickableActionURL.Contains(@"worldwind://"))
                {
                    if (World.Settings.UseInternalBrowser && this.ClickableActionURL.StartsWith("http"))
                    {
                        SplitContainer sc = (SplitContainer)drawArgs.parentControl.Parent.Parent;
                        InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                        browser.NavigateTo(this.ClickableActionURL);
                    }
                    else
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = this.ClickableActionURL;
                        psi.Verb = "open";
                        psi.UseShellExecute = true;

                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return false;
        }

        /// <summary>
        /// Does something with overlays...
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <returns></returns>
        protected virtual bool PerformRMBAction(DrawArgs drawArgs)
        {
            try
            {
                if (this.m_contextMenu == null)
                {
                    this.m_contextMenu = new ContextMenu();
                    this.BuildContextMenu(this.m_contextMenu);
                }

                this.m_contextMenu.Show(DrawArgs.ParentControl, DrawArgs.LastMousePosition);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return false;
        }

        /// <summary>
        /// Adds to the default context menu any screen overlays and user defined context menus
        /// </summary>
        /// <param name="menu"></param>
        public override void BuildContextMenu(ContextMenu menu)
        {
            base.BuildContextMenu(menu);

            // Add screen overlay items
            ScreenOverlay[] overlays = this.Overlays;
            if (overlays != null && overlays.Length > 0)
            {
                foreach (ScreenOverlay curOverlay in overlays)
                {
                    menu.MenuItems.Add(curOverlay.Name, new EventHandler(this.OverlayOnOpen));
                }
            }
        }

        /// <summary>
        /// Adds a new context menu item to this icon.
        /// </summary>
        /// <param name="newItem">The menu item to add</param>
        public void AddContextMenuItem(MenuItem newItem)
        {
            if (this.m_contextMenu == null)
            {
                this.m_contextMenu = new ContextMenu();
                this.BuildContextMenu(this.m_contextMenu);
            }

            this.m_contextMenu.MenuItems.Add(newItem);
        }

        /// <summary>
        /// Updates where we are if the camera has changed position (and thereby might be using higher resolution terrain
        /// </summary>
        /// <param name="drawArgs"></param>
		public override void Update(DrawArgs drawArgs)
		{
            if (!this.IsOn)
                return;

            if (!this.isInitialized)
                this.Initialize(drawArgs);

            if (!this.m_isUpdated || (drawArgs.WorldCamera.ViewMatrix != this.lastView))
            {
                double elevation = drawArgs.WorldCamera.WorldRadius;
                double altitude;

                // altitude = World.Settings.VerticalExaggeration * Altitude;

                // Added this because if VE is set to zero then all floating icons fall to the earth.
                if (this.UseVE && (this.UseZeroVE || World.Settings.VerticalExaggeration > 0.1))
                    altitude = World.Settings.VerticalExaggeration * this.Altitude;
                else
                    altitude = this.Altitude;

                if (drawArgs.CurrentWorld.TerrainAccessor != null && drawArgs.WorldCamera.Altitude < 300000)
                {
                    double samplesPerDegree = 50.0 / drawArgs.WorldCamera.ViewRange.Degrees;
                    elevation += drawArgs.CurrentWorld.TerrainAccessor.GetElevationAt(this.m_latitude.Degrees, this.m_longitude.Degrees, samplesPerDegree) * World.Settings.VerticalExaggeration;
                }

                // we do this rather than zero out elevation because ground stick needs elevation if it exists.
                if (this.IsAGL)
                {
                    this.Position = MathEngine.SphericalToCartesian(this.m_latitude, this.m_longitude, altitude + elevation);

                    this.m_positionD = MathEngine.SphericalToCartesianD(this.m_latitude, this.m_longitude,
                        altitude + elevation);
                }
                else
                {
                    this.Position = MathEngine.SphericalToCartesian(this.m_latitude, this.m_longitude, altitude + drawArgs.WorldCamera.WorldRadius);

                    this.m_positionD = MathEngine.SphericalToCartesianD(this.m_latitude, this.m_longitude,
                        altitude + drawArgs.WorldCamera.WorldRadius);
                }

                if (this.m_drawGroundStick)
                {
                    if (this.m_groundStick == null) this.m_groundStick = new Line(drawArgs.device);

                    if (this.m_groundStickVector == null) this.m_groundStickVector = new Vector2[2];

                    this.m_groundPoint = MathEngine.SphericalToCartesian(this.Latitude, this.Longitude, elevation);
                }

                this.lastView = drawArgs.WorldCamera.ViewMatrix;
			}

            // should overlays update every time?
            if (this.overlays != null)
			{
                for (int i = 0; i < this.overlays.Count; i++)
				{
					ScreenOverlay curOverlay = (ScreenOverlay) this.overlays[i];
                    if (curOverlay != null)
					{
						curOverlay.Update(drawArgs);
					}
				}
			}

            if (this.m_newTexture)
            {
                this.BuildIconTexture(drawArgs);
            }

            this.m_isUpdated = true;
		}


        /// <summary>
        /// Builds the icon texture based on the saved texturefile name
        /// </summary>
        /// <param name="drawArgs"></param>
        protected virtual void BuildIconTexture(DrawArgs drawArgs)
        {
            try
            {
                object key = null;

                if (this.m_iconTexture != null)
                {
                    this.m_iconTexture.ReferenceCount--;
                }

                if ((this.TextureFileName != null) && (this.TextureFileName.Trim() != String.Empty))
                {
                    // Icon image from file
                    this.m_iconTexture = (IconTexture)DrawArgs.Textures[this.TextureFileName];
                    if (this.m_iconTexture == null)
                    {
                        key = this.TextureFileName;
                        this.m_iconTexture = new IconTexture(drawArgs.device, this.TextureFileName);
                    }
                  
                }
                else
                {
                    // Icon image from bitmap
                    if (this.m_image != null)
                    {
                        this.m_iconTexture = (IconTexture)DrawArgs.Textures[this.m_image];
                        if (this.m_iconTexture == null)
                        {
                            // Create new texture from image
                            key = this.m_image;
                            this.m_iconTexture = new IconTexture(drawArgs.device, this.m_image);
                        }
                    }
                }
                
                if (this.m_iconTexture != null)
                {
                    this.m_iconTexture.ReferenceCount++;

                    if (key != null)
                    {
                        // New texture, cache it
                        DrawArgs.Textures.Add(key, this.m_iconTexture);
                    }

                    // Use default dimensions if not set
                    if (this.Width == 0)
                        this.Width = this.m_iconTexture.Width;
                    if (this.Height == 0)
                        this.Height = this.m_iconTexture.Height;
                }

                // Compute mouse over bounding boxes
                if (this.m_iconTexture == null)
                {
                    // Label only 
                    this.SelectionRectangle = drawArgs.defaultDrawingFont.MeasureString(null, this.Name, DrawTextFormat.None, 0);
                }
                else
                {
                    // Icon only
                    this.SelectionRectangle = new Rectangle(0, 0, this.Width, this.Height);
                }

                // Center the box at (0,0)
                this.SelectionRectangle.Offset(-this.SelectionRectangle.Width / 2, -this.SelectionRectangle.Height / 2);

                if (this.m_iconTexture != null)
                {
                    this.XScale = (float)this.Width / this.m_iconTexture.Width;
                    this.YScale = (float)this.Height / this.m_iconTexture.Height;

                }
                else
                {
                    this.XScale = 1.0f;
                    this.YScale = 1.0f;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            this.m_newTexture = false;

        }

        /// <summary>
        /// Render the icon.  This can be pretty slow so you should only stick an Icon on an Icons layer.
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Render(DrawArgs drawArgs)
        {
            this.DistanceToIcon = Vector3.Length(this.Position - drawArgs.WorldCamera.Position);

            // Do whatever pre-rendering we have to do
            this.PreRender(drawArgs, false);

            // If we're in view render
            if ((drawArgs.WorldCamera.ViewFrustum.ContainsPoint(this.Position)) &&
                (this.DistanceToIcon <= this.MaximumDisplayDistance) &&
                (this.DistanceToIcon >= this.MinimumDisplayDistance))
            {
                Vector3 translationVector = new Vector3(
                    (float)(this.PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                    (float)(this.PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                    (float)(this.PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

                if (this.m_sprite == null) this.m_sprite = new Sprite(drawArgs.device);

                if (this.m_labelRectangles == null) this.m_labelRectangles = new List<Rectangle>();

                // Clear or we never redraw our label
                this.m_labelRectangles.Clear();

                // Check icons for within "visual" range
                this.m_sprite.Begin(SpriteFlags.AlphaBlend);

                this.FastRender(drawArgs, this.m_sprite, projectedPoint, false, this.m_labelRectangles);

                this.m_sprite.End();
            }

            // do whatever post rendering stuff we have to do
            this.PostRender(drawArgs, false);
        }

        /// <summary>
        /// Fast render is used to batch the renders of all icons on a layer into a single Sprite.Begin and End block.
        /// </summary>
        /// <param name="drawArgs">The drawing arguments</param>
        /// <param name="sprite">The sprite to use for drawing</param>
        /// <param name="projectedPoint">Where we are</param>
        /// <param name="isMouseOver">Whether we should render as a mouseover icon</param>
        public void FastRender(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, bool isMouseOver, List<Rectangle> labelRectangles)
        {
            if (!this.isInitialized)
            {
                this.m_isUpdated = false;
                return;
            }

            int color = normalColor;

            if ((!this.m_disableMouseoverHighlight && isMouseOver) || this.m_alwaysHighlight)
                color = hotColor;

            // Render the label if necessary
            this.RenderLabel(drawArgs, sprite, projectedPoint, color, labelRectangles, isMouseOver);
            
            // render the icon image
            if (this.TextureRenderEnabled && (this.m_iconTexture != null))
            {
                this.RenderTexture(drawArgs, sprite, projectedPoint, color, isMouseOver);
            }

            this.RenderGroundStick(drawArgs, sprite, projectedPoint, color, isMouseOver);

            if (isMouseOver) this.RenderDescription(drawArgs, sprite, projectedPoint, color);

        }

        /// <summary>
        /// Renders the overlays for this icon
        /// </summary>
        /// <param name="drawArgs"></param>
        public virtual void RenderOverlay(DrawArgs drawArgs)
        {
            if (this.overlays != null)
            {
                for (int i = 0; i < this.overlays.Count; i++)
                {
                    ScreenOverlay curOverlay = (ScreenOverlay) this.overlays[i];
                    if (curOverlay != null && curOverlay.IsOn)
                    {
                        curOverlay.Render(drawArgs);
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to render icon label.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color"></param>
        /// <param name="labelRectangles"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected virtual void RenderLabel(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, List<Rectangle> labelRectangles, bool isMouseOver)
        {
            if ((this.Name != null) && ((this.m_iconTexture == null) || isMouseOver || this.NameAlwaysVisible))
            {
                if (this.m_iconTexture == null)
                {
                    // Original Icon Label Render code

                    // Center over target as we have no bitmap
                    Rectangle realrect = drawArgs.defaultDrawingFont.MeasureString(this.m_sprite, this.Name, DrawTextFormat.WordBreak, color);
                    realrect.X = (int)projectedPoint.X - (realrect.Width >> 1);
                    realrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    bool bDraw = true;

                    // Only not show if declutter is turned on and we aren't always supposed to be seen
                    if (this.Declutter && !this.NameAlwaysVisible)
                    {
                        foreach (Rectangle drawnrect in labelRectangles)
                        {
                            if (realrect.IntersectsWith(drawnrect))
                            {
                                bDraw = false;
                                break;
                            }
                        }
                    }

                    if (bDraw)
                    {
                        labelRectangles.Add(realrect);

                        drawArgs.defaultDrawingFont.DrawText(this.m_sprite, this.Name, realrect, DrawTextFormat.WordBreak, color);
                    }
                }
                else
                {
                    // KML Label Render Code with Declutter

                    // Adjust text to make room for icon
                    int spacing = (int)(this.Width * 0.3f);
                    if (spacing > 5)
                        spacing = 5;
                    int offsetForIcon = (this.Width >> 1) + spacing;

                    // Text to the right
                    Rectangle rightrect = drawArgs.defaultDrawingFont.MeasureString(this.m_sprite, this.Name, DrawTextFormat.WordBreak, color);
                    rightrect.X = (int)projectedPoint.X + offsetForIcon;
                    rightrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    // Text to the left
                    Rectangle leftrect = drawArgs.defaultDrawingFont.MeasureString(this.m_sprite, this.Name, DrawTextFormat.WordBreak, color);
                    leftrect.X = (int)projectedPoint.X - offsetForIcon - rightrect.Width;
                    leftrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    bool bDrawRight = true;
                    bool bDrawLeft = true;

                    // Only not show if declutter is turned on and we aren't always supposed to be seen
                    if (this.Declutter && !this.NameAlwaysVisible)
                    {
                        foreach (Rectangle drawnrect in labelRectangles)
                        {
                            if (rightrect.IntersectsWith(drawnrect))
                            {
                                bDrawRight = false;
                            }
                            if (leftrect.IntersectsWith(drawnrect))
                            {
                                bDrawLeft = false;
                            }
                            if (!bDrawRight && !bDrawLeft)
                            {
                                break;
                            }
                        }
                    }

                    // draw either right or left if we have space.  If we don't too bad.
                    if (bDrawRight)
                    {
                        labelRectangles.Add(rightrect);
                        //drawArgs.defaultDrawingFont.DrawText(m_sprite, Name, rightrect, DrawTextFormat.WordBreak, color);
                        drawArgs.defaultDrawingFont.DrawText(null, this.Name, rightrect, DrawTextFormat.WordBreak, color);
                    }
                    else if (bDrawLeft)
                    {
                        labelRectangles.Add(leftrect);
                        drawArgs.defaultDrawingFont.DrawText(this.m_sprite, this.Name, leftrect, DrawTextFormat.WordBreak, color);
                    }
                }

            }
        }

        /// <summary>
        /// Helper function to render icon texture.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color">the color to render the icon</param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected virtual void RenderTexture(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, bool isMouseOver)
        {
            Matrix scaleTransform;
            Matrix rotationTransform;

            //Do Altitude depedent scaling for KMLIcons
            if (this.AutoScaleIcon)
            {
                float factor = 1;
                if (this.DistanceToIcon > this.MinIconZoomDistance)
                    factor -= (float)((this.DistanceToIcon - this.MinIconZoomDistance) / this.DistanceToIcon);
                if (factor < this.MinScaleFactor) factor = this.MinScaleFactor;

                this.XScale = factor * ((float) this.Width / this.m_iconTexture.Width);
                this.YScale = factor * ((float) this.Height / this.m_iconTexture.Height);
            }

            //scale and rotate image
            scaleTransform = Matrix.Scaling(this.XScale, this.YScale, 0);

            if (this.m_isRotated)
                rotationTransform = Matrix.RotationZ((float) this.m_rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
            else
                rotationTransform = Matrix.Identity;

            sprite.Transform = scaleTransform * rotationTransform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
            sprite.Draw(this.m_iconTexture.Texture,
                new Vector3(this.m_iconTexture.Width >> 1, this.m_iconTexture.Height >> 1, 0),
                Vector3.Zero,
                color);

            // Reset transform to prepare for text rendering later
            sprite.Transform = Matrix.Identity;

        }

        /// <summary>
        /// Helper function to render the groundstick
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected virtual void RenderGroundStick(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, bool isMouseOver)
        {
            if (this.m_drawGroundStick)
            {
                // distance to icon is set by the renderer
                if ((this.DistanceToIcon < this.MaxGroundStickDistance) || isMouseOver)
                {
                    Vector3 referenceCenter = new Vector3(
                        (float)drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                    Vector3 projectedGroundPoint = drawArgs.WorldCamera.Project(Vector3.Subtract(this.m_groundPoint, referenceCenter));

                    this.m_groundStick.Begin();
                    this.m_groundStickVector[0].X = projectedPoint.X;
                    this.m_groundStickVector[0].Y = projectedPoint.Y;
                    this.m_groundStickVector[1].X = projectedGroundPoint.X;
                    this.m_groundStickVector[1].Y = projectedGroundPoint.Y;

                    this.m_groundStick.Draw(this.m_groundStickVector, color);
                    this.m_groundStick.End();
                }
            }
        }

        /// <summary>
        /// Helper function to render icon description.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        protected virtual void RenderDescription(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color)
        {
            string description = this.Description;

            if (description == null)
                description = this.ClickableActionURL;

            if (description != null)
            {
                // Render description field
                DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Bottom;
                int left = 10;
                if (World.Settings.showLayerManager)
                    left += World.Settings.layerManagerWidth;
                Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10);

                // Draw outline
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                rect.Offset(2, 0);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                rect.Offset(0, 2);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                rect.Offset(-2, 0);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, 0xb0 << 24);

                // Draw description
                rect.Offset(1, -1);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, descriptionColor);
            }
        }

        /// <summary>
        /// Does whatever you need to do before you render the icon.  Occurs even if the icon isn't visible!
        /// Occurs OUTSIDE of Sprite.begin.  Other RO's can be rendered here.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public virtual void PreRender(DrawArgs drawArgs, bool isMouseOver)
        {
            this.RenderOverlay(drawArgs);
        }

        /// <summary>
        /// Does whatever you need to do after you render the icon.  Occurs even if the icon isn't visible!
        /// Occurs INSIDE of Sprite.begin.  Other RO's wont render here.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public virtual void PostRender(DrawArgs drawArgs, bool isMouseOver)
        {
        }

        /// <summary>
        /// Do this if we don't actually get rendered (not in view, too far, etc)
        /// </summary>
        /// <param name="drawArgs"></param>
        public virtual void NoRender(DrawArgs drawArgs)
        {
        }

		#endregion

        protected void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
		}
	}
}
