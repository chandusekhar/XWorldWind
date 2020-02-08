using System;
using System.Collections;
using System.Globalization;

namespace WorldWind.Terrain
{
	/// <summary>
	/// Reads NLT terrain/elevation data (BIL files).
	/// </summary>
	public class NltTerrainAccessor : TerrainAccessor
	{
		public static int CacheSize = 100;
		protected TerrainTileService m_terrainTileService;
		protected WmsImageStore m_wmsElevationSet;
		protected Hashtable m_tileCache = new Hashtable();

		#region Properties

		public WmsImageStore WmsElevationStore
		{
			get
			{
				return this.m_wmsElevationSet;
			}
			set
			{
                this.m_wmsElevationSet = value;
			}
		}

		public TerrainAccessor this[int index]
		{
			get
			{
				return this.m_higherResolutionSubsets[index];
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Terrain.NltTerrainAccessor"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="west"></param>
		/// <param name="south"></param>
		/// <param name="east"></param>
		/// <param name="north"></param>
		/// <param name="terrainTileService"></param>
		/// <param name="higherResolutionSubsets"></param>
		public NltTerrainAccessor(string name, double west, double south, double east, double north,
			TerrainTileService terrainTileService, TerrainAccessor[] higherResolutionSubsets)
		{
            this.m_name = name;
            this.m_west = west;
            this.m_south = south;
            this.m_east = east;
            this.m_north = north;
            this.m_terrainTileService = terrainTileService;
            this.m_higherResolutionSubsets = higherResolutionSubsets;
		}

		/// <summary>
		/// Get terrain elevation at specified location.  
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="targetSamplesPerDegree"></param>
		/// <returns>Returns 0 if the tile is not available on disk.</returns>
		public override float GetElevationAt(double latitude, double longitude, double targetSamplesPerDegree)
		{
			try
			{
				if (this.m_terrainTileService == null || targetSamplesPerDegree < World.Settings.MinSamplesPerDegree)
					return 0;

				if (this.m_higherResolutionSubsets != null)
				{
					foreach (TerrainAccessor higherResSub in this.m_higherResolutionSubsets)
					{
						if (latitude > higherResSub.South && latitude < higherResSub.North &&
							longitude > higherResSub.West && longitude < higherResSub.East)
						{
							return higherResSub.GetElevationAt(latitude, longitude, targetSamplesPerDegree);
						}
					}
				}

				TerrainTile tt = this.m_terrainTileService.GetTerrainTile(latitude, longitude, targetSamplesPerDegree);
				TerrainTileCacheEntry ttce = (TerrainTileCacheEntry) this.m_tileCache[tt.TerrainTileFilePath];
				if (ttce == null)
				{
					ttce = new TerrainTileCacheEntry(tt);
                    this.AddToCache(ttce);
				}

				if (!ttce.TerrainTile.IsInitialized)
					ttce.TerrainTile.Initialize();
				ttce.LastAccess = DateTime.Now;
				return ttce.TerrainTile.GetElevationAt(latitude, longitude);
			}
			catch (Exception)
			{
			}
			return 0;
		}
        /// <summary>
        /// Get fast terrain elevation at specified location from already loaded data.
        /// Will not trigger any download or data loading from files in cache - just memory.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees.</param>
        /// <param name="longitude">Longitude in decimal degrees.</param>
        /// <returns>Returns NaN if no tile is available in cache.</returns>
        public override float GetCachedElevationAt(double latitude, double longitude)
        {
            try
            {
                if (this.m_terrainTileService == null)
                    return 0;
                // Use higher res subset if any
                if (this.m_higherResolutionSubsets != null)
                {
                    foreach (TerrainAccessor higherResSub in this.m_higherResolutionSubsets)
                    {
                        if (!higherResSub.IsOn)
                            continue;
                        if (latitude > higherResSub.South && latitude < higherResSub.North &&
                            longitude > higherResSub.West && longitude < higherResSub.East)
                        {
                            return higherResSub.GetCachedElevationAt(latitude, longitude);
                        }
                    }
                }
                // Look for a tile starting from higher res level, moving down the levels
                TerrainTileCacheEntry ttce = null;
                for (int targetLevel = this.m_terrainTileService.NumberLevels - 1; targetLevel >= 0; targetLevel--)
                {
                    // File name and path for that level
                    double tileSize = this.m_terrainTileService.LevelZeroTileSizeDegrees * Math.Pow(0.5, targetLevel);
                    int row = TerrainTileService.GetRowFromLatitude(latitude, tileSize);
                    int col = TerrainTileService.GetColFromLongitude(longitude, tileSize);
                    string terrainTileFilePath = string.Format(CultureInfo.InvariantCulture,
                        @"{0}\{4}\{1:D4}\{1:D4}_{2:D4}.{3}", this.m_terrainTileService.TerrainTileDirectory, row, col, this.m_terrainTileService.FileExtension, targetLevel);
                    // Look in cache
                    ttce = (TerrainTileCacheEntry) this.m_tileCache[terrainTileFilePath];
                    if (ttce != null)
                    {
                        // Tile found, get elevation from it
                        ttce.LastAccess = DateTime.Now;
                        return ttce.TerrainTile.GetElevationAt(latitude, longitude);
                    }
                }
            }
            catch (Exception)
            {
            }
            // No tile found - sorry.
            return float.NaN;
        }

		/// <summary>
		/// Get terrain elevation at specified location.  
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <returns>Returns 0 if the tile is not available on disk.</returns>
		public override float GetElevationAt(double latitude, double longitude)
		{
			return this.GetElevationAt(latitude, longitude, this.m_terrainTileService.SamplesPerTile / this.m_terrainTileService.LevelZeroTileSizeDegrees);
		}

		/// <summary>
		/// Builds a terrain array with specified boundaries
		/// </summary>
		/// <param name="north">North edge in decimal degrees.</param>
		/// <param name="south">South edge in decimal degrees.</param>
		/// <param name="west">West edge in decimal degrees.</param>
		/// <param name="east">East edge in decimal degrees.</param>
		/// <param name="samples"></param>
		public override TerrainTile GetElevationArray(double north, double south, double west, double east,
			int samples)
		{
			TerrainTile res = null;
			
			if (this.m_higherResolutionSubsets != null)
			{
				// TODO: Support more than 1 level of higher resolution sets and allow user selections
				foreach (TerrainAccessor higherResSub in this.m_higherResolutionSubsets)
				{
                    if (!higherResSub.IsOn)
                        continue;
					if (north <= higherResSub.North && south >= higherResSub.South &&
						west >= higherResSub.West && east <= higherResSub.East)
					{
						res = higherResSub.GetElevationArray(north, south, west, east, samples);
						return res;
					}
				}
			}

			res = new TerrainTile(this.m_terrainTileService);
			res.North = north;
			res.South = south;
			res.West = west;
			res.East = east;
			res.SamplesPerTile = samples;
			res.IsInitialized = true;
			res.IsValid = true;

			double samplesPerDegree = (double)samples / (double)(north - south);
			double latrange = Math.Abs(north - south);
			double lonrange = Math.Abs(east - west);
			TerrainTileCacheEntry ttce = null;

			float[,] data = new float[samples, samples];

			if(samplesPerDegree < World.Settings.MinSamplesPerDegree)
			{
				res.ElevationData = data;
				return res;
			}

			double scaleFactor = (double)1 / (samples - 1);
			for (int x = 0; x < samples; x++)
			{
				for (int y = 0; y < samples; y++)
				{
					double curLat = north - scaleFactor * latrange * x;
					double curLon = west + scaleFactor * lonrange * y;

                    // Wrap lat/lon to fit range 90/-90 and -180/180 (PM 2006-11-17)
                    if (curLat > 90)
                    {
                        curLat = 90 - (curLat - 90);
                        curLon += 180;
                    }
                    if (curLat < -90)
                    {
                        curLat = -90 - (curLat + 90);
                        curLon += 180;
                    }
                    if (curLon > 180)
                    {
                        curLon -= 360;
                    }
                    if (curLon < -180)
                    {
                        curLon += 360;
                    }

					if (ttce == null ||
						curLat < ttce.TerrainTile.South ||
						curLat > ttce.TerrainTile.North ||
						curLon < ttce.TerrainTile.West ||
						curLon > ttce.TerrainTile.East)
					{
						TerrainTile tt = this.m_terrainTileService.GetTerrainTile(curLat, curLon, samplesPerDegree);
						ttce = (TerrainTileCacheEntry) this.m_tileCache[tt.TerrainTileFilePath];
						if (ttce == null)
						{
							ttce = new TerrainTileCacheEntry(tt);
                            this.AddToCache(ttce);
						}
						if (!ttce.TerrainTile.IsInitialized)
							ttce.TerrainTile.Initialize();
						ttce.LastAccess = DateTime.Now;
						if (!tt.IsValid)
							res.IsValid = false;
					}

					data[x, y] = ttce.TerrainTile.GetElevationAt(curLat, curLon);
				}
			}
			res.ElevationData = data;

			return res;
		}

		#endregion

		protected void AddToCache(TerrainTileCacheEntry ttce)
		{
			if (!this.m_tileCache.ContainsKey(ttce.TerrainTile.TerrainTileFilePath))
			{
				if (this.m_tileCache.Count >= CacheSize)
				{
					// Remove least recently used tile
					TerrainTileCacheEntry oldestTile = null;
					foreach (TerrainTileCacheEntry curEntry in this.m_tileCache.Values)
					{
						if (oldestTile == null)
							oldestTile = curEntry;
						else
						{
							if (curEntry.LastAccess < oldestTile.LastAccess)
								oldestTile = curEntry;
						}
					}

                    this.m_tileCache.Remove(oldestTile.TerrainTile.TerrainTileFilePath);
				}

                this.m_tileCache.Add(ttce.TerrainTile.TerrainTileFilePath, ttce);
			}
		}

		public class TerrainTileCacheEntry
		{
			DateTime m_lastAccess = DateTime.Now;
			TerrainTile m_terrainTile;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="tile">TerrainTile to be cached.</param>
			public TerrainTileCacheEntry(TerrainTile tile)
			{
                this.m_terrainTile = tile;
			}

			public TerrainTile TerrainTile
			{
				get
				{
					return this.m_terrainTile;
				}
				set
				{
                    this.m_terrainTile = value;
				}
			}

			public DateTime LastAccess
			{
				get
				{
					return this.m_lastAccess;
				}
				set
				{
                    this.m_lastAccess = value;
				}
			}
		}

		public override void Dispose()
		{
			if (this.m_terrainTileService != null)
			{
                this.m_terrainTileService.Dispose();
                this.m_terrainTileService = null;
			}
		}
	}
}