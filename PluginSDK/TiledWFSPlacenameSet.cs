using System;
using System.Collections;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.GZip;
using Utility;
using WorldWind.Net;

namespace WorldWind
{
	/// <summary>
	/// Placename Layer that uses a Directory of xml files and extract place names 
	/// based on current lat/lon/viewrange
	/// </summary>
	public class TiledWFSPlacenameSet : RenderableObject
	{

        protected string m_name;
	    
		protected World m_parentWorld;
		protected Cache m_cache;

		/// <summary>
		/// Minimum distance from camera to label squared
		/// </summary>
		protected double m_minimumDistanceSq; 

		/// <summary>
		/// Maximum distance from camera to label squared
		/// </summary>
		protected double m_maximumDistanceSq;
        /// <summary>
        /// Wfs Base Url from which to fetch placenames
        /// Typically http://host/wfs?service=WFS&amp;request=GetFeature&amp;FeatureID=cite:BrasilCid
        /// </summary>
		protected string m_placenameBaseUrl;
        protected string m_labelfield;
        protected string m_typename;

		protected string m_iconFilePath;
		protected Sprite m_sprite;
		
		protected Font m_drawingFont;
        protected float m_fontScaling;
		protected int m_defaultColor;
        protected int m_renderColor;
		protected ArrayList m_placenameFileList = new ArrayList();
		protected Hashtable m_placenameFiles = new Hashtable();
		protected Hashtable m_renderablePlacenames = new Hashtable();
		protected WorldWindPlacename[] m_placeNames;
		protected double m_altitude;
		protected Texture m_iconTexture;
		protected System.Drawing.Rectangle m_spriteSize;
		protected FontDescription m_fontDescription;
		protected DrawTextFormat m_textFormat = DrawTextFormat.None;
		
		protected static int IconWidth = 48;
		protected static int IconHeight = 48;

		public WorldWindPlacename[] PlaceNames
		{
			get{ return this.m_placeNames; }
		}

		public int Color
		{
			get{ return this.m_defaultColor; }
		}
		public FontDescription FontDescription
		{
			get{ return this.m_fontDescription; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.TiledPlacenameSet"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parentWorld"></param>
		/// <param name="altitude"></param>
		/// <param name="maximumDisplayAltitude"></param>
		/// <param name="minimumDisplayAltitude"></param>
		/// <param name="placenameBaseUrl"></param>
        /// <param name="labelfield">Field in Feature type used as PlacenameLabel</param>
		/// <param name="fontDescription"></param>
		/// <param name="color"></param>
		/// <param name="iconFilePath"></param>
		public TiledWFSPlacenameSet(
			string name, 
			World parentWorld,
			double altitude,
			double maximumDisplayAltitude,
			double minimumDisplayAltitude,
			string placenameBaseUrl,
            string typename,
            string labelfield,
			FontDescription fontDescription,
			System.Drawing.Color color,
			string iconFilePath,
			Cache cache
			) : base(name, parentWorld.Position, Quaternion.RotationYawPitchRoll(0,0,0))
		{
            this.m_name = name;
            this.m_parentWorld = parentWorld;
            this.m_altitude = altitude;
            this.m_maximumDistanceSq = maximumDisplayAltitude*maximumDisplayAltitude;
            this.m_minimumDistanceSq = minimumDisplayAltitude*minimumDisplayAltitude;
            this.m_placenameBaseUrl = placenameBaseUrl;
            this.m_typename = typename;
            this.m_labelfield = labelfield;
            this.m_fontDescription = fontDescription;
            this.m_defaultColor = color.ToArgb();
            this.m_iconFilePath = iconFilePath;
            this.m_cache = cache;
			
			// Set default render priority
            this.RenderPriority = RenderPriority.Placenames;

            this.m_fontScaling = World.Settings.WFSNameSizeMultiplier;
		}

		public override bool IsOn
		{
			get
			{
				return this.isOn;
			}
			set
			{
				if(this.isOn && !value) this.Dispose();
                this.isOn = value;
				
				// HACK: We need a flag telling which layers are "Place names"
				if(this.Name=="Placenames")
					World.Settings.showPlacenames = value;
			}
		}

		public override void Initialize(DrawArgs drawArgs)
		{
			this.isInitialized = true;

            FontDescription scaledDescription = this.m_fontDescription;
            scaledDescription.Height = (int)(this.m_fontDescription.Height * this.m_fontScaling);

            this.m_drawingFont = drawArgs.CreateFont(scaledDescription);

            //Validate URL
            /*
            if(System.Text.RegularExpressions.Regex.IsMatch(m_placenameBaseUrl, "(http|ftp|https)://([/w-]+/.)+(/[/w- ./?%&=]*)?"))
            {
                this.isInitialized = true;
            }
            */
            //Generate Initial File List
            //WorldWindWFSPlacenameFile root_file = new WorldWindWFSPlacenameFile(m_placenameBaseUrl, m_typename, m_labelfield);
            //m_placenameFileList = new ArrayList(root_file.SplitPlacenameFiles());

            //TODO:Download and validate capabitilities

			if(this.m_iconFilePath!=null)
			{
                this.m_iconTexture = ImageHelper.LoadIconTexture(this.m_iconFilePath );
			
				using(Surface s = this.m_iconTexture.GetSurfaceLevel(0))
				{
					SurfaceDescription desc = s.Description;
                    this.m_spriteSize = new System.Drawing.Rectangle(0,0, desc.Width, desc.Height);
				}

                this.m_sprite = new Sprite(drawArgs.device);
			}
		}

		public override void Dispose()
		{
			this.isInitialized = false;

			if(this.m_placenameFileList != null)
			{
				lock(this.m_placenameFileList.SyncRoot)
				{
                    this.m_placenameFileList.Clear();
				}
			}

			if(this.m_placenameFiles != null)
			{
				lock(this.m_placenameFiles.SyncRoot)
				{
                    this.m_placenameFiles.Clear();
				}
			}

			if(this.m_placeNames != null)
			{
				lock(this) this.m_placeNames = null;
			}

			if(this.m_iconTexture != null)
			{
                this.m_iconTexture.Dispose();
                this.m_iconTexture = null;
			}

			if(this.m_sprite != null)
			{
                this.m_sprite.Dispose();
                this.m_sprite = null;
			}
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		/// <summary>
		/// // Index into currently loaded array for already loaded test
		/// </summary>
		int curPlaceNameIndex; 

		Matrix lastView = Matrix.Identity;
        Hashtable m_fileHash = new Hashtable();

		public override void Update(DrawArgs drawArgs)
		{
			try
			{
				if(!this.isInitialized)
					this.Initialize(drawArgs);

				if(this.lastView != drawArgs.WorldCamera.ViewMatrix &&
                   ((this.m_minimumDistanceSq == 0 && this.m_maximumDistanceSq == 0) ||
                    (drawArgs.WorldCamera.Altitude*drawArgs.WorldCamera.Altitude >= this.m_minimumDistanceSq &&
                     drawArgs.WorldCamera.Altitude*drawArgs.WorldCamera.Altitude <= this.m_maximumDistanceSq)))
				{
                    //Log.Write(Log.Levels.Debug, "Updating WFS: " + this.name);
                    double viewNorth = drawArgs.WorldCamera.Latitude.Degrees + drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;
                    double viewSouth = drawArgs.WorldCamera.Latitude.Degrees - drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;
                    double viewWest = drawArgs.WorldCamera.Longitude.Degrees - drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;
                    double viewEast = drawArgs.WorldCamera.Longitude.Degrees + drawArgs.WorldCamera.TrueViewRange.Degrees * 0.5;

                    WorldWindWFSPlacenameFile[] placenameFiles = this.GetWFSFiles(viewNorth, viewSouth, viewWest, viewEast);
					
                    ArrayList tempPlacenames = new ArrayList();
                    this.curPlaceNameIndex=0;
                    foreach (WorldWindWFSPlacenameFile placenameFileDescriptor in placenameFiles)
					{
                        this.UpdateNames(placenameFileDescriptor, tempPlacenames, drawArgs);
					}
					
                    lock(this)
					{
                        this.m_placeNames = new WorldWindPlacename[tempPlacenames.Count];
						tempPlacenames.CopyTo(this.m_placeNames);
					}

                    ArrayList deletionKeys = new ArrayList();
                    foreach (int key in this.m_fileHash.Keys)
                    {
                        bool found = false;
                        foreach (WorldWindWFSPlacenameFile placenameFile in placenameFiles)
                        {
                            if (key == placenameFile.GetHashCode())
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            deletionKeys.Add(key);
                    }

                    foreach (int deletionKey in deletionKeys) this.m_fileHash.Remove(deletionKey);

                    this.lastView = drawArgs.WorldCamera.ViewMatrix;
				}
			}
			catch(Exception ex)
			{
                Log.Write(ex);
			}
		}

        WorldWindWFSPlacenameFile[] GetWFSFiles(double north, double south, double west, double east)
        {
            double tileSize = 0;
            
            //base the tile size on the max viewing distance
            double maxDistance = Math.Sqrt(this.m_maximumDistanceSq);
            
            double factor = maxDistance / this.m_parentWorld.EquatorialRadius;
            // True view range 
			if(factor < 1)
				tileSize = Angle.FromRadians(Math.Abs(Math.Asin(maxDistance / this.m_parentWorld.EquatorialRadius))*2).Degrees;
			else
				tileSize = Angle.FromRadians(Math.PI).Degrees;

            tileSize = (180 / (int)(180 / tileSize));

            if (tileSize == 0)
                tileSize = 0.1;
            //Log.Write(Log.Levels.Debug, string.Format("TS: {0} -> {1}", name, tileSize));
            //not working for some reason...
            //int startRow = MathEngine.GetRowFromLatitude(south, tileSize);
            //int endRow = MathEngine.GetRowFromLatitude(north, tileSize);
            //int startCol = MathEngine.GetColFromLongitude(west, tileSize);
            //int endCol = MathEngine.GetColFromLongitude(east, tileSize);
            
            ArrayList placenameFiles = new ArrayList();

            double currentSouth = -90;
            //for (int row = 0; row <= endRow; row++)
            while(currentSouth < 90)
            {
                double currentNorth = currentSouth + tileSize;
                if (currentSouth > north || currentNorth < south)
                {
                    currentSouth += tileSize;
                    continue;
                }

                double currentWest = -180;
                while(currentWest < 180)
            //    for (int col = startCol; col <= endCol; col++)
                {
                    double currentEast = currentWest + tileSize;
                    if (currentWest > east || currentEast < west)
                    {
                        currentWest += tileSize;
                        continue;
                    }
                    
                    WorldWindWFSPlacenameFile placenameFile = new WorldWindWFSPlacenameFile(this.m_name, this.m_placenameBaseUrl, this.m_typename, this.m_labelfield, currentNorth, currentSouth, currentWest, currentEast, this.m_parentWorld, this.m_cache);
                    
                    int key = placenameFile.GetHashCode();
                    if (!this.m_fileHash.Contains(key))
                    {
                        this.m_fileHash.Add(key, placenameFile);
                    }
                    else
                    {
                        placenameFile = (WorldWindWFSPlacenameFile) this.m_fileHash[key];
                    }
                    placenameFiles.Add(placenameFile);
                    
                    currentWest += tileSize;
                }
                currentSouth += tileSize;
            }

            return (WorldWindWFSPlacenameFile[])placenameFiles.ToArray(typeof(WorldWindWFSPlacenameFile));
        }

        /// <summary>
        /// Loads visible place names from one file.
        /// If cache files are appropriately named only files in view are hit
        /// </summary>
        void UpdateNames(WorldWindWFSPlacenameFile placenameFileDescriptor, ArrayList tempPlacenames, DrawArgs drawArgs)
		{
			// TODO: Replace with bounding box frustum intersection test
			double viewRange = drawArgs.WorldCamera.TrueViewRange.Degrees;
			double north = drawArgs.WorldCamera.Latitude.Degrees + viewRange;
			double south = drawArgs.WorldCamera.Latitude.Degrees - viewRange;
			double west = drawArgs.WorldCamera.Longitude.Degrees - viewRange;
			double east = drawArgs.WorldCamera.Longitude.Degrees + viewRange;

            //TODO: Implement GML parsing
			if(placenameFileDescriptor.north < south)
				return;
			if(placenameFileDescriptor.south > north)
				return;
			if(placenameFileDescriptor.east < west)
				return;
			if(placenameFileDescriptor.west > east)
				return;

            WorldWindPlacename[] tilednames = placenameFileDescriptor.PlaceNames;
            if (tilednames == null)
                return;
            
            tempPlacenames.Capacity = tempPlacenames.Count + tilednames.Length;
            WorldWindPlacename curPlace = new WorldWindPlacename();
            for (int i = 0; i < tilednames.Length; i++)
            {
                if (this.m_placeNames != null && this.curPlaceNameIndex < this.m_placeNames.Length)
                    curPlace = this.m_placeNames[this.curPlaceNameIndex];
                
                WorldWindPlacename pn = tilednames[i];
                float lat = pn.Lat;
                float lon = pn.Lon;

                // for easier hit testing
                
                float lonRanged = lon;
                if (lonRanged < west)
                    lonRanged += 360; // add a revolution
                

                if (lat > north || lat < south || lonRanged > east || lonRanged < west)
                    continue;
                
                float elevation = 0;
                if (this.m_parentWorld.TerrainAccessor != null && drawArgs.WorldCamera.Altitude < 300000)
                    elevation = (float) this.m_parentWorld.TerrainAccessor.GetElevationAt(lat, lon);
                float altitude = (float)(this.m_parentWorld.EquatorialRadius + World.Settings.VerticalExaggeration * this.m_altitude + World.Settings.VerticalExaggeration * elevation);
                pn.cartesianPoint = MathEngine.SphericalToCartesian(lat, lon, altitude);
                float distanceSq = Vector3.LengthSq(pn.cartesianPoint - drawArgs.WorldCamera.Position);
                if (distanceSq > this.m_maximumDistanceSq)
                    continue;
                if (distanceSq < this.m_minimumDistanceSq)
                    continue;
                
                if (!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(pn.cartesianPoint))
                    continue;
                
                tempPlacenames.Add(pn);
            }
            
		}

		public override void Render(DrawArgs drawArgs)
		{
			try
			{
				lock(this)
				{
                    //override colors for visibility
                    switch (World.Settings.WFSNameColors)
                    {
                        case WFSNameColors.Black:
                            this.m_renderColor = System.Drawing.Color.Black.ToArgb();
                            break;

                        case WFSNameColors.White:
                            this.m_renderColor = System.Drawing.Color.White.ToArgb();
                            break;
                            
                        case WFSNameColors.Gray:
                            this.m_renderColor = System.Drawing.Color.DarkGray.ToArgb();
                            break;

                        case WFSNameColors.Default:
                            this.m_renderColor = this.m_defaultColor;
                            break;
                    }

                    if (World.Settings.WFSNameSizeMultiplier != this.m_fontScaling)
                    {
                        this.m_fontScaling = World.Settings.WFSNameSizeMultiplier;
                        // scale font size based on settings
                        FontDescription scaledDescription = this.m_fontDescription;
                        scaledDescription.Height = (int)(this.m_fontDescription.Height * this.m_fontScaling);
                        this.m_drawingFont = drawArgs.CreateFont(scaledDescription);
                    }

					Vector3 cameraPosition = drawArgs.WorldCamera.Position;
					if(this.m_placeNames==null)
						return;

					// Black outline for light text, white outline for dark text
					int outlineColor = unchecked((int)0x80ffffff);
					int brightness = (this.m_renderColor & 0xff) + 
						((this.m_renderColor >> 8) & 0xff) + 
						((this.m_renderColor >> 16) & 0xff);
					if(brightness > 255*3/2)
						outlineColor = unchecked((int)0x80000000);

					if(this.m_sprite != null) this.m_sprite.Begin(SpriteFlags.AlphaBlend);
					int count = 0;
					Vector3 referenceCenter = new Vector3(
						(float)drawArgs.WorldCamera.ReferenceCenter.X,
						(float)drawArgs.WorldCamera.ReferenceCenter.Y,
						(float)drawArgs.WorldCamera.ReferenceCenter.Z);

					for(int index=0; index< this.m_placeNames.Length; index++)
					{
						Vector3 v = this.m_placeNames[index].cartesianPoint;
						float distanceSquared = Vector3.LengthSq(v-cameraPosition);
						if(distanceSquared > this.m_maximumDistanceSq)
							continue;

						if(distanceSquared < this.m_minimumDistanceSq)
							continue;
						
						if(!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(v))
							continue;

						Vector3 pv = drawArgs.WorldCamera.Project(v - referenceCenter);

						// Render text only
						string label = this.m_placeNames[index].Name;
					
						
						/*
						if(m_sprite != null && 1==0)
						{
							m_sprite.Draw(m_iconTexture, 
								m_spriteSize,
								new Vector3(m_spriteSize.Width/2,m_spriteSize.Height/2,0), 
								new Vector3(pv.X, pv.Y, 0),
								System.Drawing.Color.White);
							pv.X += m_spriteSize.Width/2 + 4;
						}
						*/

						System.Drawing.Rectangle rect = this.m_drawingFont.MeasureString(null, label, this.m_textFormat, this.m_renderColor );

						pv.Y -= rect.Height/2;
						if(this.m_sprite==null)
							// Center horizontally
							pv.X -= rect.Width/2;

						rect.Inflate(3,1);
						int x = (int)Math.Round(pv.X);
						int y = (int)Math.Round(pv.Y);

						rect.Offset(x,y);

						if(World.Settings.WFSOutlineText)
						{
                            this.m_drawingFont.DrawText(null,label, x-1, y-1, outlineColor );
                            this.m_drawingFont.DrawText(null,label, x-1, y+1, outlineColor );
                            this.m_drawingFont.DrawText(null,label, x+1, y-1, outlineColor );
                            this.m_drawingFont.DrawText(null,label, x+1, y+1, outlineColor );
						}

                        this.m_drawingFont.DrawText(null,label, x, y, this.m_renderColor );

						count++;
						if(count>30)
							break;
					}

					if(this.m_sprite != null) this.m_sprite.End();

				}
			}
			catch(Exception caught)
			{
				Log.Write( caught );
			}
		}
	}


	public class WorldWindWFSPlacenameFile
	{
        public string name;
		public string wfsURL;
		public double north = 90.0f;
        public double south = -90.0f;
        public double west = -180.0f;
        public double east = 180.0f;
		protected WorldWindPlacename[] m_placeNames;
        protected string wfsBaseUrl;
        protected string labelfield;
        protected string typename;
		protected World m_world;
		protected Cache m_cache;

        protected bool m_dlInProcess;

		public WorldWindWFSPlacenameFile(
		    string name,
            String wfsBaseUrl,
            string typename,
            String labelfield,
            double north,
            double south,
            double west,
            double east,
			World world,
			Cache cache)
		{
            //TODO:Validate basewfsurl
            this.name = name;
            this.wfsBaseUrl = wfsBaseUrl;
            this.typename = typename;
            this.labelfield = labelfield;
            this.north = north;
            this.south = south;
            this.west = west;
            this.east = east;
            this.wfsURL = wfsBaseUrl + "&OUTPUTFORMAT=GML2-GZIP&BBOX=" + west + "," + south + "," + east + "," + north;
			this.m_world = world;
			this.m_cache = cache;
        }

        public WorldWindPlacename[] PlaceNames
        {
            get {
                if (this.m_placeNames == null)
                {
                    this.DownloadParsePlacenames();
                }
                return this.m_placeNames; 
            }
        }

        /*public WorldWindWFSPlacenameFile[] SplitPlacenameFiles()
		{
			//split
			WorldWindWFSPlacenameFile northWest = new WorldWindWFSPlacenameFile(this.wfsBaseUrl,this.typename,this.labelfield);
			northWest.north = this.north;
			northWest.south = 0.5f * (this.north + this.south);
			northWest.west = this.west;
			northWest.east = 0.5f * (this.west + this.east);
            northWest.wfsURL = northWest.wfsBaseUrl + "&OUTPUTFORMAT=GML2-GZIP&BBOX=" + northWest.west + "," + northWest.south + "," + northWest.east + ","+northWest.north;

            WorldWindWFSPlacenameFile northEast = new WorldWindWFSPlacenameFile(this.wfsBaseUrl,this.typename, this.labelfield);
			northEast.north = this.north;
			northEast.south = 0.5f * (this.north + this.south);
			northEast.west = 0.5f * (this.west + this.east);
			northEast.east = this.east;
            northEast.wfsURL = northEast.wfsBaseUrl + "&OUTPUTFORMAT=GML2-GZIP&BBOX=" + northEast.west + "," + northEast.south + "," + northEast.east + "," + northEast.north;

            WorldWindWFSPlacenameFile southWest = new WorldWindWFSPlacenameFile(this.wfsBaseUrl,this.typename, this.labelfield);
			southWest.north = 0.5f * (this.north + this.south);
			southWest.south = this.south;
			southWest.west = this.west;
			southWest.east = 0.5f * (this.west + this.east);
            southWest.wfsURL = southWest.wfsBaseUrl + "&OUTPUTFORMAT=GML2-GZIP&BBOX=" + southWest.west + "," + southWest.south + "," + southWest.east + "," + southWest.north;

            WorldWindWFSPlacenameFile southEast = new WorldWindWFSPlacenameFile(this.wfsBaseUrl,this.typename, this.labelfield);
			southEast.north = 0.5f * (this.north + this.south);
			southEast.south = this.south;
			southEast.west = 0.5f * (this.west + this.east);
			southEast.east = this.east;
            southEast.wfsURL = southEast.wfsBaseUrl + "&OUTPUTFORMAT=GML2-GZIP&BBOX=" + southEast.west + "," + southEast.south + "," + southEast.east + "," + southEast.north;

			WorldWindWFSPlacenameFile[] returnArray = new WorldWindWFSPlacenameFile[] {northWest, northEast, southWest, southEast};
			return returnArray;
		}*/

        public override int GetHashCode()
        {
            return this.wfsURL.GetHashCode();
        }
        bool m_failed;

        //TODO: Implement Downloading + Uncompressing + Caching 
        private void DownloadParsePlacenames()
        {
            try
            {
                if (this.m_failed)
                {
                    return;
                }

                if (this.m_dlInProcess)
                    return;

				//hard coded cache location
                //string cachefilename = 
                //    Directory.GetParent(System.Windows.Forms.Application.ExecutablePath) +
                //    string.Format("Cache//WFS//Placenames//{0}//{1}_{2}_{3}_{4}.xml.gz", 
                //    this.name, this.west, this.south, this.east, this.north);
				
				
				//let's use the location from settings instead
				string cachefilename = this.m_cache.CacheDirectory +
                                       string.Format("\\{0}\\WFS\\{1}\\{2}_{3}_{4}_{5}.xml.gz", 
                                       this.m_world.Name, this.name, this.west, this.south, this.east, this.north);

                if (!File.Exists(cachefilename))
                {
                    this.m_dlInProcess = true;

                    // Offline check
                    if (World.Settings.WorkOffline)
                        throw new Exception("Offline mode active.");

                    WebDownload wfsdl = new WebDownload(this.wfsURL);
                    wfsdl.BackgroundDownloadFile(cachefilename, new DownloadCompleteHandler(this.DownloadComplete));
                }
                else
                    this.ProcessCacheFile();
            }
            catch //(Exception ex)
            {
                //Log.Write(ex);
                if (this.m_placeNames == null) this.m_placeNames = new WorldWindPlacename[0];
                this.m_failed = true;
            }
        }

        private void DownloadComplete(WebDownload dl)
        {
            this.ProcessCacheFile();
        }

        private void ProcessCacheFile()
        {
            try
            {
                string cachefilename = this.m_cache.CacheDirectory +
                                       string.Format("\\{0}\\WFS\\{1}\\{2}_{3}_{4}_{5}.xml.gz",
                                       this.m_world.Name, this.name, this.west, this.south, this.east, this.north);

                GZipInputStream instream = new GZipInputStream(new FileStream(cachefilename, FileMode.Open));
                XmlDocument gmldoc = new XmlDocument();
                gmldoc.Load(instream);
                XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(gmldoc.NameTable);
                xmlnsManager.AddNamespace("gml", "http://www.opengis.net/gml");
                //HACK: Create namespace using first part of Label Field
                string labelnmspace = this.labelfield.Split(':')[0];

                if (labelnmspace == "cite")
                {
                    xmlnsManager.AddNamespace(labelnmspace, "http://www.opengeospatial.net/cite");
                }
                else if (labelnmspace == "topp")
                {
                    xmlnsManager.AddNamespace(labelnmspace, "http://www.openplans.org/topp");
                }

                XmlNodeList featureList = gmldoc.SelectNodes("//gml:featureMember", xmlnsManager);
                if (featureList != null)
                {

                    ArrayList placenameList = new ArrayList();
                    foreach (XmlNode featureTypeNode in featureList)
                    {
                        XmlNode typeNameNode = featureTypeNode.SelectSingleNode(this.typename, xmlnsManager);
                        if (typeNameNode == null)
                        {
                            Log.Write(Log.Levels.Debug, "No typename node: " + this.typename);
                            continue;
                        }
                        XmlNode labelNode = typeNameNode.SelectSingleNode(this.labelfield, xmlnsManager);
                        if (labelNode == null)
                        {
                            Log.Write(Log.Levels.Debug, "No label node: " + this.labelfield);
                            continue;
                        }

                        XmlNodeList gmlCoordinatesNodes = featureTypeNode.SelectNodes(".//gml:Point/gml:coordinates", xmlnsManager);
                        if (gmlCoordinatesNodes != null)
                        {
                            foreach (XmlNode gmlCoordinateNode in gmlCoordinatesNodes)
                            {
                                //Log.Write(Log.Levels.Debug, "FOUND " + gmlCoordinatesNode.Count.ToString() + " POINTS");
                                string coordinateNodeText = gmlCoordinateNode.InnerText;
                                string[] coords = coordinateNodeText.Split(',');
                                WorldWindPlacename pn = new WorldWindPlacename();
                                pn.Lon = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
                                pn.Lat = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                                pn.Name = labelNode.InnerText;
                                placenameList.Add(pn);
                            }
                        }
                    }

                    this.m_placeNames = (WorldWindPlacename[])placenameList.ToArray(typeof(WorldWindPlacename));
                }

                if (this.m_placeNames == null) this.m_placeNames = new WorldWindPlacename[0];
            }
            catch //(Exception ex)
            {
                //Log.Write(ex);
                if (this.m_placeNames == null) this.m_placeNames = new WorldWindPlacename[0];
                this.m_failed = true;
            }
            finally
            {
                this.m_dlInProcess = false;
            }
        }
	}
}