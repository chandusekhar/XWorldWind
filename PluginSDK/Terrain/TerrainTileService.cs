using System;
using System.Globalization;
using System.IO;

namespace WorldWind.Terrain
{
	/// <summary>
	/// Provides elevation data (BIL format).
	/// </summary>
	public class TerrainTileService : IDisposable
	{
		#region Private Members
		string m_serverUrl;
		string m_dataSet;
		double m_levelZeroTileSizeDegrees;
		int m_samplesPerTile;
		int m_numberLevels;
		string m_fileExtension;
		string m_terrainTileDirectory;
		TimeSpan m_terrainTileRetryInterval;
		#endregion

        #region Protected Members
        protected string m_dataType;
        #endregion

        #region Properties
        public string ServerUrl
		{
			get
			{
				return this.m_serverUrl;
			}
		}

		public string DataSet
		{
			get
			{
				return this.m_dataSet;
			}
		}

		public double LevelZeroTileSizeDegrees
		{
			get
			{
				return this.m_levelZeroTileSizeDegrees;
			}
		}

        public int NumberLevels
        {
            get
            {
                return this.m_numberLevels;
            }
        }

		public int SamplesPerTile
		{
			get
			{
				return this.m_samplesPerTile;
			}
		}

		public string FileExtension
		{
			get
			{
				return this.m_fileExtension;
			}
		}

		public string TerrainTileDirectory
		{
			get
			{
				return this.m_terrainTileDirectory;
			}
		}

		public TimeSpan TerrainTileRetryInterval
		{
			get
			{
				return this.m_terrainTileRetryInterval;
			}
		}

		public string DataType
		{
			get
			{
				return this.m_dataType;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Terrain.TerrainTileService"/> class.
		/// </summary>
		/// <param name="serverUrl"></param>
		/// <param name="dataSet"></param>
		/// <param name="levelZeroTileSizeDegrees"></param>
		/// <param name="samplesPerTile"></param>
		/// <param name="fileExtension"></param>
		/// <param name="numberLevels"></param>
		/// <param name="terrainTileDirectory"></param>
		/// <param name="terrainTileRetryInterval"></param>
		/// <param name="dataType">Terrain Tiles Data type</param>
		public TerrainTileService(
			string serverUrl,
			string dataSet,
			double levelZeroTileSizeDegrees,
			int samplesPerTile,
			string fileExtension,
			int numberLevels,
			string terrainTileDirectory,
			TimeSpan terrainTileRetryInterval,
			string dataType)
		{
            this.m_serverUrl = serverUrl;
            this.m_dataSet = dataSet;
            this.m_levelZeroTileSizeDegrees = levelZeroTileSizeDegrees;
            this.m_samplesPerTile = samplesPerTile;
            this.m_numberLevels = numberLevels;
            this.m_fileExtension = fileExtension.Replace(".","");
            this.m_terrainTileDirectory = terrainTileDirectory;
			if(!Directory.Exists(this.m_terrainTileDirectory))
				Directory.CreateDirectory(this.m_terrainTileDirectory);
            this.m_terrainTileRetryInterval = terrainTileRetryInterval;
            this.m_dataType = dataType;
		}

		/// <summary>
		/// Builds terrain tile containing the specified coordinates.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="samplesPerDegree"></param>
		/// <returns>Uninitialized terrain tile (no elevation data)</returns>
		public TerrainTile GetTerrainTile(double latitude, double longitude, double samplesPerDegree)
		{
			TerrainTile tile = new TerrainTile(this);

			tile.TargetLevel = this.m_numberLevels - 1;
			for(int i = 0; i < this.m_numberLevels; i++)
			{
				if(samplesPerDegree <= this.m_samplesPerTile / (this.m_levelZeroTileSizeDegrees * Math.Pow(0.5, i)))
				{
					tile.TargetLevel = i;
					break;
				}
			}

			tile.Row = GetRowFromLatitude(latitude, this.m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel));
			tile.Col = GetColFromLongitude(longitude, this.m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel));
			tile.TerrainTileFilePath = string.Format( CultureInfo.InvariantCulture,
				@"{0}\{4}\{1:D4}\{1:D4}_{2:D4}.{3}", this.m_terrainTileDirectory, tile.Row, tile.Col, this.m_fileExtension, tile.TargetLevel);
			tile.SamplesPerTile = this.m_samplesPerTile;
			tile.TileSizeDegrees = this.m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel);
			tile.North = -90.0 + tile.Row * tile.TileSizeDegrees + tile.TileSizeDegrees;
			tile.South = -90.0 + tile.Row * tile.TileSizeDegrees;
			tile.West = -180.0 + tile.Col * tile.TileSizeDegrees;
			tile.East = -180.0 + tile.Col * tile.TileSizeDegrees + tile.TileSizeDegrees;

			return tile;
		}

		// Hack: newer methods in MathEngine class cause problems
		public static int GetColFromLongitude(double longitude, double tileSize)
		{
			return (int)Math.Floor((Math.Abs(-180.0 - longitude)%360)/tileSize);
		}

		public static int GetRowFromLatitude(double latitude, double tileSize)
		{
			return (int)Math.Floor((Math.Abs(-90.0 - latitude)%180)/tileSize);
		}
		#region IDisposable Members

		public void Dispose()
		{
			if(DrawArgs.DownloadQueue!=null)
				DrawArgs.DownloadQueue.Clear(this);
		}

		#endregion
	}


	public class TerrainTile : IDisposable
	{
		public string TerrainTileFilePath;
		public double TileSizeDegrees;
		public int SamplesPerTile;
		public double South;
		public double North;
		public double West;
		public double East;
		public int Row;
		public int Col;
		public int TargetLevel;
		public TerrainTileService m_owner;
		public bool IsInitialized;
		public bool IsValid;

		public float[,] ElevationData;
		protected TerrainDownloadRequest request;

		public TerrainTile( TerrainTileService owner )
		{
            this.m_owner = owner;
		}
		/// <summary>
		/// This method initializes the terrain tile add switches to
		/// Initialize floating point/int 16 tiles
		/// </summary>
        public void Initialize()
        {
            if (this.IsInitialized)
                return;

            if (!File.Exists(this.TerrainTileFilePath))
            {
                // Download elevation
                if (this.request == null)
                {
                    using (this.request = new TerrainDownloadRequest(this, this.m_owner, this.Row, this.Col, this.TargetLevel))
                    {
                        this.request.SaveFilePath = this.TerrainTileFilePath;
                        this.request.DownloadInForeground();
                    }
                }
            }

            if (this.ElevationData == null) this.ElevationData = new float[this.SamplesPerTile, this.SamplesPerTile];

            if (File.Exists(this.TerrainTileFilePath))
            {
                // Load elevation file
                try
                {
                    // TerrainDownloadRequest's FlagBadTile() creates empty files
                    // as a way to flag "bad" terrain tiles.
                    // Remove the empty 'flag' files after preset time.
                    try
                    {
                        FileInfo tileInfo = new FileInfo(this.TerrainTileFilePath);
                        if (tileInfo.Length == 0)
                        {
                            TimeSpan age = DateTime.Now.Subtract(tileInfo.LastWriteTime);
                            if (age < this.m_owner.TerrainTileRetryInterval)
                            {
                                // This tile is still flagged bad
                                this.IsInitialized = true;
                            }
                            else
                            {
                                // remove the empty 'flag' file
                                File.Delete(this.TerrainTileFilePath);
                            }
                            return;
                        }
                    }
                    catch
                    {
                        // Ignore any errors in the above block, and continue.
                        // For example, if someone had the empty 'flag' file
                        // open, the delete would fail.
                    }

                    using (Stream s = File.OpenRead(this.TerrainTileFilePath))
                    {
                        BinaryReader reader = new BinaryReader(s);
                        if (this.m_owner.DataType == "Int16")
                        {
                            /*
                            byte[] tfBuffer = new byte[SamplesPerTile*SamplesPerTile*2];
                            if (s.Read(tfBuffer,0,tfBuffer.Length) < tfBuffer.Length)
                                throw new IOException(string.Format("End of file error while reading terrain file '{0}'.", TerrainTileFilePath) );

                            int offset = 0;
                            for(int y = 0; y < SamplesPerTile; y++)
                                for(int x = 0; x < SamplesPerTile; x++)
                                    ElevationData[x,y] = tfBuffer[offset++] + (short)(tfBuffer[offset++]<<8);
                            */
                            for (int y = 0; y < this.SamplesPerTile; y++)
                                for (int x = 0; x < this.SamplesPerTile; x++)
                                    this.ElevationData[x, y] = reader.ReadInt16();
                        }
                        if (this.m_owner.DataType == "Float32")
                        {
                            /*
                            byte[] tfBuffer = new byte[SamplesPerTile*SamplesPerTile*4];
                            if (s.Read(tfBuffer,0,tfBuffer.Length) < tfBuffer.Length)
                                    throw new IOException(string.Format("End of file error while reading terrain file '{0}'.", TerrainTileFilePath) );
                            */
                            for (int y = 0; y < this.SamplesPerTile; y++)
                                for (int x = 0; x < this.SamplesPerTile; x++)
                                {
                                    this.ElevationData[x, y] = reader.ReadSingle();
                                }
                        }

                        this.IsInitialized = true;
                        this.IsValid = true;
                    }
                    return;
                }
                catch (IOException)
                {
                    // If there is an IO exception when reading the terrain tile,
                    // then either something is wrong with the file, or with
                    // access to the file, so try and remove it.
                    try
                    {
                        File.Delete(this.TerrainTileFilePath);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(String.Format("Error while trying to delete corrupt terrain tile {0}", this.TerrainTileFilePath), ex);
                    }
                }
                catch (Exception ex)
                {
                    // Some other type of error when reading the terrain tile.
                    throw new ApplicationException(String.Format("Error while trying to read terrain tile {0}", this.TerrainTileFilePath), ex);
                }
            }
        }

        public float GetElevationAt(double latitude, double longitude)
        {
            if (!this.IsInitialized)
                return 0;

            try
            {
                double deltaLat = this.North - latitude;
                double deltaLon = longitude - this.West;

                double df2 = (this.SamplesPerTile - 1) / this.TileSizeDegrees;
                float lat_pixel = (float)(deltaLat * df2);
                float lon_pixel = (float)(deltaLon * df2);

                int lat_min = (int)lat_pixel;
                int lat_max = (int)Math.Ceiling(lat_pixel);
                int lon_min = (int)lon_pixel;
                int lon_max = (int)Math.Ceiling(lon_pixel);

                if (lat_min >= this.SamplesPerTile)
                    lat_min = this.SamplesPerTile - 1;
                if (lat_max >= this.SamplesPerTile)
                    lat_max = this.SamplesPerTile - 1;
                if (lon_min >= this.SamplesPerTile)
                    lon_min = this.SamplesPerTile - 1;
                if (lon_max >= this.SamplesPerTile)
                    lon_max = this.SamplesPerTile - 1;

                if (lat_min < 0)
                    lat_min = 0;
                if (lat_max < 0)
                    lat_max = 0;
                if (lon_min < 0)
                    lon_min = 0;
                if (lon_max < 0)
                    lon_max = 0;

                float delta = lat_pixel - lat_min;
                float westElevation = this.ElevationData[lon_min, lat_min] * (1 - delta) + this.ElevationData[lon_min, lat_max] * delta;

                float eastElevation = this.ElevationData[lon_max, lat_min] * (1 - delta) + this.ElevationData[lon_max, lat_max] * delta;

                delta = lon_pixel - lon_min;
                float interpolatedElevation =
                    westElevation * (1 - delta) +
                    eastElevation * delta;

                return interpolatedElevation;
            }
            catch
            {
            }

            return 0;
        }
		#region IDisposable Members

		public void Dispose()
		{
			if(this.request != null)
			{
                this.request.Dispose();
                this.request = null;
			}
			
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
