using System;
using System.Collections;
using System.IO;
using Utility;

namespace WorldWind
{
	/// <summary>
	/// Placename Layer that uses a Directory of xml files and extract place names 
	/// based on current lat/lon/viewrange
	/// </summary>
	public class TiledPlacenameSet : RenderableObject
	{
		protected World m_parentWorld;

		/// <summary>
		/// Minimum distance from camera to label squared
		/// </summary>
		protected double m_minimumDistanceSq; 

		/// <summary>
		/// Maximum distance from camera to label squared
		/// </summary>
		protected double m_maximumDistanceSq;
		protected string m_placenameListFilePath;

		protected string m_iconFilePath;
		protected Sprite m_sprite;
		
		protected Font m_drawingFont;
		protected int m_color;
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
			get{ return this.m_color; }
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
		/// <param name="placenameListFilePath"></param>
		/// <param name="fontDescription"></param>
		/// <param name="color"></param>
		/// <param name="iconFilePath"></param>
		public TiledPlacenameSet(
			string name, 
			World parentWorld,
			double altitude,
			double maximumDisplayAltitude,
			double minimumDisplayAltitude,
			string placenameListFilePath,
			FontDescription fontDescription,
			System.Drawing.Color color,
			string iconFilePath
			) : base(name, parentWorld.Position, Quaternion.RotationYawPitchRoll(0,0,0))
		{
            this.m_parentWorld = parentWorld;
            this.m_altitude = altitude;
            this.m_maximumDistanceSq = maximumDisplayAltitude*maximumDisplayAltitude;
            this.m_minimumDistanceSq = minimumDisplayAltitude*minimumDisplayAltitude;
            this.m_placenameListFilePath = placenameListFilePath;
            this.m_fontDescription = fontDescription;
            this.m_color = color.ToArgb();
            this.m_iconFilePath = iconFilePath;
			
			// Set default render priority
            this.RenderPriority = RenderPriority.Placenames;
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

            this.m_drawingFont = drawArgs.CreateFont(this.m_fontDescription);
			if(!File.Exists(this.m_placenameListFilePath))
			{
				this.isInitialized = true;
				Log.Write(Log.Levels.Error, "PLACE", this.m_placenameListFilePath + " not found.");
				return;
			}

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

			using( BufferedStream placenameFileListStream = new BufferedStream(File.Open(this.m_placenameListFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
			using( BinaryReader placenameFileListReader = new BinaryReader(placenameFileListStream, System.Text.Encoding.ASCII))
			{
				int count = placenameFileListReader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					WorldWindPlacenameFile wwpf = new WorldWindPlacenameFile();
					wwpf.dataFilename = placenameFileListReader.ReadString();
					wwpf.west = placenameFileListReader.ReadSingle();
					wwpf.south = placenameFileListReader.ReadSingle();
					wwpf.east = placenameFileListReader.ReadSingle();
					wwpf.north = placenameFileListReader.ReadSingle();
                    this.m_placenameFileList.Add(wwpf);
				}
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

		public override void Update(DrawArgs drawArgs)
		{
			try
			{
				if(!this.isInitialized)
					this.Initialize(drawArgs);

				if(this.lastView != drawArgs.WorldCamera.ViewMatrix)
				{
					ArrayList tempPlacenames = new ArrayList();
					if((this.m_minimumDistanceSq == 0 && this.m_maximumDistanceSq == 0) ||
						drawArgs.WorldCamera.Altitude*drawArgs.WorldCamera.Altitude <= this.m_maximumDistanceSq)
					{
                        this.curPlaceNameIndex=0; 
						foreach(WorldWindPlacenameFile placenameFileDescriptor in this.m_placenameFileList)
						{
                            this.UpdateNames(placenameFileDescriptor, tempPlacenames, drawArgs);
						}
					}

					lock(this)
					{
                        this.m_placeNames = new WorldWindPlacename[tempPlacenames.Count];
						tempPlacenames.CopyTo(this.m_placeNames);
					}

                    this.lastView = drawArgs.WorldCamera.ViewMatrix;
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Loads visible place names from one file.
		/// </summary>
		void UpdateNames(WorldWindPlacenameFile placenameFileDescriptor, ArrayList tempPlacenames, DrawArgs drawArgs)
		{
			// TODO: Replace with bounding box frustum intersection test
			double viewRange = drawArgs.WorldCamera.TrueViewRange.Degrees;
			double north = drawArgs.WorldCamera.Latitude.Degrees + viewRange;
			double south = drawArgs.WorldCamera.Latitude.Degrees - viewRange;
			double west = drawArgs.WorldCamera.Longitude.Degrees - viewRange;
			double east = drawArgs.WorldCamera.Longitude.Degrees + viewRange;
		
			if(placenameFileDescriptor.north < south)
				return;
			if(placenameFileDescriptor.south > north)
				return;
			if(placenameFileDescriptor.east < west)
				return;
			if(placenameFileDescriptor.west > east)
				return;

			string dataFilePath = Path.Combine( Path.GetDirectoryName(this.m_placenameListFilePath), placenameFileDescriptor.dataFilename );
			using( BufferedStream dataFileStream = new BufferedStream(File.Open(dataFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) )
			using( BinaryReader dataFileReader = new BinaryReader(dataFileStream, System.Text.Encoding.ASCII) )
			{
				WorldWindPlacenameFile dataFile = new WorldWindPlacenameFile();
				dataFile.dataFilename = placenameFileDescriptor.dataFilename;
				dataFile.north = placenameFileDescriptor.north;
				dataFile.south = placenameFileDescriptor.south;
				dataFile.west = placenameFileDescriptor.west;
				dataFile.east = placenameFileDescriptor.east;

				int numberPlacenames = dataFileReader.ReadInt32();
				tempPlacenames.Capacity = tempPlacenames.Count + numberPlacenames;
				WorldWindPlacename curPlace = new WorldWindPlacename();
				for (int i = 0; i < numberPlacenames; i++)
				{
					if(this.m_placeNames != null && this.curPlaceNameIndex< this.m_placeNames.Length)
						curPlace = this.m_placeNames[this.curPlaceNameIndex];

					string name = dataFileReader.ReadString();
					float lat = dataFileReader.ReadSingle();
					float lon = dataFileReader.ReadSingle();
					int c = dataFileReader.ReadInt32();

					// Not in use, removed for speed
					// Hashtable metaData = new Hashtable(c);

					for (int n = 0; n < c; n++)
					{
						string key = dataFileReader.ReadString();
						string keyData = dataFileReader.ReadString();

						// Not in use, removed for speed
						//metaData.Add(key, keyData);
					}

					// for easier hit testing
					float lonRanged = lon;
					if (lonRanged < west)
						lonRanged += 360; // add a revolution

					if (lat > north || lat < south || lonRanged > east || lonRanged < west)
						continue;

					WorldWindPlacename pn = new WorldWindPlacename();
					pn.Lat = lat;
					pn.Lon = lon;
					pn.Name = name;
					// Not in use, removed for speed
					//pn.metaData = metaData;

					float elevation = 0;
					if(this.m_parentWorld.TerrainAccessor != null && drawArgs.WorldCamera.Altitude < 300000)
						elevation = (float) this.m_parentWorld.TerrainAccessor.GetElevationAt(lat, lon);
					float altitude = (float)(this.m_parentWorld.EquatorialRadius + World.Settings.VerticalExaggeration * this.m_altitude + World.Settings.VerticalExaggeration * elevation);
					pn.cartesianPoint = MathEngine.SphericalToCartesian(lat, lon, altitude);
					float distanceSq = Vector3.LengthSq(pn.cartesianPoint - drawArgs.WorldCamera.Position);
					if(distanceSq > this.m_maximumDistanceSq)
						continue;
					if(distanceSq < this.m_minimumDistanceSq)
						continue;

					if(!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(pn.cartesianPoint))
						continue;

					tempPlacenames.Add(pn);
				}
			}
		}

		public override void Render(DrawArgs drawArgs)
		{
			try
			{
				lock(this)
				{
					Vector3 cameraPosition = drawArgs.WorldCamera.Position;
					if(this.m_placeNames==null)
						return;

					// Black outline for light text, white outline for dark text
					int outlineColor = unchecked((int)0x80ffffff);
					int brightness = (this.m_color & 0xff) + 
						((this.m_color >> 8) & 0xff) + 
						((this.m_color >> 16) & 0xff);
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

						System.Drawing.Rectangle rect = this.m_drawingFont.MeasureString(null, label, this.m_textFormat, this.m_color );

						pv.Y -= rect.Height/2;
						if(this.m_sprite==null)
							// Center horizontally
							pv.X -= rect.Width/2;

						rect.Inflate(3,1);
						int x = (int)Math.Round(pv.X);
						int y = (int)Math.Round(pv.Y);

						rect.Offset(x,y);

						if(World.Settings.outlineText)
						{
                            this.m_drawingFont.DrawText(null,label, x-1, y-1, outlineColor );
                            this.m_drawingFont.DrawText(null,label, x-1, y+1, outlineColor );
                            this.m_drawingFont.DrawText(null,label, x+1, y-1, outlineColor );
                            this.m_drawingFont.DrawText(null,label, x+1, y+1, outlineColor );
						}

                        this.m_drawingFont.DrawText(null,label, x, y, this.m_color );

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

	public class WorldWindPlacenameList
	{
		public ArrayList m_placenameFiles = new ArrayList();
		public WorldWindPlacenameList()
		{
			WorldWindPlacenameFile rootPlacenameFile = new WorldWindPlacenameFile();
			rootPlacenameFile.dataFilename = "0000_0000.wwp";
			rootPlacenameFile.north = 90.0f;
			rootPlacenameFile.south = -90.0f;
			rootPlacenameFile.west = -180.0f;
			rootPlacenameFile.east = 180.0f;
            this.m_placenameFiles.Add(rootPlacenameFile);
		}

		public void SavePlacenameList(string filename)
		{
			if(!Directory.Exists(Path.GetDirectoryName(filename)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filename));
			}

			using( FileStream fs = File.Open(filename, FileMode.Create) )
			using (BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.ASCII))
			{
				bw.Write(this.m_placenameFiles.Count);
				foreach (WorldWindPlacenameFile pf in this.m_placenameFiles)
				{
					bw.Write(pf.dataFilename);
					bw.Write(pf.west);
					bw.Write(pf.south);
					bw.Write(pf.east);
					bw.Write(pf.north);

					string dataFilePath = Path.Combine( Path.GetDirectoryName(filename), pf.dataFilename );
					using( FileStream placenameFileStream = File.Open(dataFilePath, FileMode.Create))
					using (BinaryWriter placenameFileWriter = new BinaryWriter(placenameFileStream, System.Text.Encoding.ASCII))
					{
						placenameFileWriter.Write(pf.m_placeNames.Count);

						foreach (WorldWindPlacename wwp in pf.m_placeNames)
						{
							placenameFileWriter.Write(wwp.Name);
							placenameFileWriter.Write(wwp.Lat);
							placenameFileWriter.Write(wwp.Lon);
							if (wwp.metaData != null)
							{
								placenameFileWriter.Write(wwp.metaData.Count);
								foreach (string key in wwp.metaData.Keys)
								{
									string keyData = (string)wwp.metaData[key];
									placenameFileWriter.Write(key);
									placenameFileWriter.Write(keyData);
								}
							}
							else
							{
								int c = 0;
								placenameFileWriter.Write(c);
							}
						}
					}
				}
			}
		}

		public void AddPlacename(string name, float lat, float lon, Hashtable metaData)
		{
			WorldWindPlacenameFile p = null;
			foreach(WorldWindPlacenameFile tempFile in this.m_placenameFiles)
			{
				if(lat < tempFile.north && lat >= tempFile.south && lon >= tempFile.west && lon < tempFile.east)
				{
					p = tempFile;
					break;
				}
			}

			if(p == null)
				return;
			
			if(p.m_placeNames.Count == 50000)
			{
				//split
				WorldWindPlacenameFile[] splitFiles = p.SplitPlacenameFiles();
                this.m_placenameFiles.Remove(p);
				foreach(WorldWindPlacenameFile newFile in splitFiles) this.m_placenameFiles.Add(newFile);
				this.AddPlacename(name, lat, lon, metaData);
				
			}
			else
				p.AddPlacename(name, lat, lon, metaData);
		}
	}

	public class WorldWindPlacenameFile
	{
		public string dataFilename;
		public float north;
		public float south;
		public float west;
		public float east;
		public ArrayList m_placeNames = new ArrayList();

		public WorldWindPlacenameFile()
		{
		}

		public void AddPlacename(string name, float lat, float lon)
		{
			WorldWindPlacename newPlacename;
			newPlacename.Name = name;
			newPlacename.Lat = lat;
			newPlacename.Lon = lon;
			newPlacename.cartesianPoint = Vector3.Zero;
			newPlacename.ID = this.dataFilename + "_" + this.m_placeNames.Count.ToString();
			newPlacename.metaData = null;
            this.m_placeNames.Add(newPlacename);
		}

		public void AddPlacename(string name, float lat, float lon, Hashtable metaData)
		{
			WorldWindPlacename newPlacename;
			newPlacename.Name = name;
			newPlacename.Lat = lat;
			newPlacename.Lon = lon;
			newPlacename.metaData = metaData;
			newPlacename.cartesianPoint = Vector3.Zero;
			newPlacename.ID = this.dataFilename + "_" + this.m_placeNames.Count.ToString();
            this.m_placeNames.Add(newPlacename);
		}

		public WorldWindPlacenameFile[] SplitPlacenameFiles()
		{
			//split
			WorldWindPlacenameFile northWest = new WorldWindPlacenameFile();
			northWest.north = this.north;
			northWest.south = 0.5f * (this.north + this.south);
			northWest.west = this.west;
			northWest.east = 0.5f * (this.west + this.east);
			northWest.dataFilename = GetRowFromLatitude(northWest.south, northWest.north - northWest.south).ToString().PadLeft(4,'0') + "_" + GetColFromLongitude(northWest.west, northWest.east - northWest.west).ToString().PadLeft(4,'0') + ".wwp";

			WorldWindPlacenameFile northEast = new WorldWindPlacenameFile();
			northEast.north = this.north;
			northEast.south = 0.5f * (this.north + this.south);
			northEast.west = 0.5f * (this.west + this.east);
			northEast.east = this.east;
			northEast.dataFilename = GetRowFromLatitude(northEast.south, northEast.north - northEast.south).ToString().PadLeft(4,'0') + "_" + GetColFromLongitude(northEast.west, northEast.east - northEast.west).ToString().PadLeft(4,'0') + ".wwp";

			WorldWindPlacenameFile southWest = new WorldWindPlacenameFile();
			southWest.north = 0.5f * (this.north + this.south);
			southWest.south = this.south;
			southWest.west = this.west;
			southWest.east = 0.5f * (this.west + this.east);
			southWest.dataFilename = GetRowFromLatitude(southWest.south, southWest.north - southWest.south).ToString().PadLeft(4,'0') + "_" + GetColFromLongitude(southWest.west, southWest.east - southWest.west).ToString().PadLeft(4,'0') + ".wwp";

			WorldWindPlacenameFile southEast = new WorldWindPlacenameFile();
			southEast.north = 0.5f * (this.north + this.south);
			southEast.south = this.south;
			southEast.west = 0.5f * (this.west + this.east);
			southEast.east = this.east;
			southEast.dataFilename = GetRowFromLatitude(southEast.south, southEast.north - southEast.south).ToString().PadLeft(4,'0') + "_" + GetColFromLongitude(southEast.west, southEast.east - southEast.west).ToString().PadLeft(4,'0') + ".wwp";

			foreach(WorldWindPlacename p in this.m_placeNames)
			{
				if(p.Lat >= 0.5f * (this.north + this.south))
				{
					if(p.Lon >= 0.5 * (this.west + this.east))
					{
						northEast.AddPlacename(p.Name, p.Lat, p.Lon, p.metaData);	
					}
					else
					{
						northWest.AddPlacename(p.Name, p.Lat, p.Lon, p.metaData);
					}
				}
				else
				{
					if(p.Lon >= 0.5 * (this.west + this.east))
					{
						southEast.AddPlacename(p.Name, p.Lat, p.Lon, p.metaData);

					}
					else
					{
						southWest.AddPlacename(p.Name, p.Lat, p.Lon, p.metaData);
					}
				}
			}

			WorldWindPlacenameFile[] returnArray = new WorldWindPlacenameFile[] {northWest, northEast, southWest, southEast};
			return returnArray;
		}

		
		public static int GetRowFromLatitude(float latitude, float tileSize)
		{
			return (int)Math.Round((Math.Abs(-90.0 - latitude)%180)/tileSize);
		}
			
		public static int GetColFromLongitude(float longitude, float tileSize)
		{
			return (int)Math.Round((Math.Abs(-180.0 - longitude)%360)/tileSize);
		}
	}

	public struct WorldWindPlacename
	{
		public string ID;
		public string Name;
		public float Lat;
		public float Lon;
		public Vector3 cartesianPoint;
		public Hashtable metaData;
	}
}