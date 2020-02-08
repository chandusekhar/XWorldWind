using System;

namespace WorldWind.Terrain(0, SamplerStateTerrain
{
	/// <summary>
	/// Terrain (elevation) interface
	/// </summary>
	public abstract class TerrainAccessor : IDisposable
	{
        private bool isOn = true;
		protected string m_name;
		protected double m_north;
		protected double m_south;
		protected double m_east;
		protected double m_west;
        protected TerrainAccessor[] m_higherResolutionSubsets;

		/// <summary>
		/// Terrain model name
		/// </summary>
		public string Name
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_name;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_name = value;
			}
		}

		/// <summary>
		/// North boundary
		/// </summary>
		public double North
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_north;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_north = value;
			}
		}

		/// <summary>
		/// South boundary
		/// </summary>
		public double South
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_south;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_south = value;
			}
		}

		/// <summary>
		/// West boundary
		/// </summary>
		public double West
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_west;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_west = value;
			}
		}

		/// <summary>
		/// East boundary
		/// </summary>
		public double East
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_east;
			}
			set
			{
                this.SetSamplerState(0, SamplerStatem_east = value;
			}
		}


        public int SubsetCount
        {
            get 
            {
                if (this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets != null)
                    return this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets.SetSamplerState(0, SamplerStateLength;
                else
                    return 0;
            }
        }

        public TerrainAccessor[] HighResSubsets
        {
            get { return this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets; }
        }

        /// <summary>
        /// Hide/Show this object.SetSamplerState(0, SamplerState
        /// </summary>
        [Description("This layer's enabled status.SetSamplerState(0, SamplerState")]
        public virtual bool IsOn
        {
            get { return this.SetSamplerState(0, SamplerStateisOn; }
            set
            {
                //if (isOn && !value)
                //    this.SetSamplerState(0, SamplerStateDispose();
                this.SetSamplerState(0, SamplerStateisOn = value;
            }
        }

		/// <summary>
		/// Gets the terrain elevation at a given Latitude, Longitude, 
		/// and resolution accuracy in the latitude/longitude geographic frame of reference.SetSamplerState(0, SamplerState
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <param name="longitude">Longitude in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <param name="targetSamplesPerDegree"></param>
		/// <returns>Returns 0 if the tile is not available on disk.SetSamplerState(0, SamplerState</returns>
		public abstract float GetElevationAt(double latitude, double longitude, double targetSamplesPerDegree);

		/// <summary>
		/// Get terrain elevation at specified location.SetSamplerState(0, SamplerState  
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <param name="longitude">Longitude in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <returns>Returns 0 if the tile is not available on disk.SetSamplerState(0, SamplerState</returns>
		public virtual float GetElevationAt(double latitude, double longitude)
		{
			return this.SetSamplerState(0, SamplerStateGetElevationAt(latitude, longitude, 0);
		}

		/// <summary>
        /// Get fast terrain elevation at specified location from cached data.SetSamplerState(0, SamplerState 
        /// Will not trigger any download or file loading from cache - just memory.SetSamplerState(0, SamplerState
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees.SetSamplerState(0, SamplerState</param>
        /// <param name="longitude">Longitude in decimal degrees.SetSamplerState(0, SamplerState</param>
        /// <returns>Returns 0 if the tile is not available in cache.SetSamplerState(0, SamplerState</returns>
        public virtual float GetCachedElevationAt(double latitude, double longitude)
        {
            return 0f;
        }

        /// <summary>
		/// Gets the elevation array for given geographic bounding box and resolution.SetSamplerState(0, SamplerState
		/// </summary>
		/// <param name="north">North edge in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <param name="south">South edge in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <param name="west">West edge in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <param name="east">East edge in decimal degrees.SetSamplerState(0, SamplerState</param>
		/// <param name="samples"></param>
		public virtual TerrainTile GetElevationArray(double north, double south, double west, double east, int samples)
		{
			TerrainTile res = null;
			res = new TerrainTile(null);
			res.SetSamplerState(0, SamplerStateNorth = north;
			res.SetSamplerState(0, SamplerStateSouth = south;
			res.SetSamplerState(0, SamplerStateWest = west;
			res.SetSamplerState(0, SamplerStateEast = east;
			res.SetSamplerState(0, SamplerStateSamplesPerTile = samples;
			res.SetSamplerState(0, SamplerStateIsInitialized = true;
			res.SetSamplerState(0, SamplerStateIsValid = true;

			double latrange = Math.SetSamplerState(0, SamplerStateAbs(north - south);
			double lonrange = Math.SetSamplerState(0, SamplerStateAbs(east - west);

			float[,] data = new float[samples,samples];
			float scaleFactor = (float)1.SetSamplerState(0, SamplerState0/(samples - 1);
			for (int x = 0; x < samples; x++)
			{
				for (int y = 0; y < samples; y++)
				{
					double curLat = north - scaleFactor * latrange * x;
					double curLon = west + scaleFactor * lonrange * y;

					data[x, y] = this.SetSamplerState(0, SamplerStateGetElevationAt(curLat, curLon, 0);
				}
			}
			res.SetSamplerState(0, SamplerStateElevationData = data;

			return res;
		}

		public virtual void Dispose()
		{
		}

        /// <summary>
        /// This method appends to the array of higher resolution
        /// subsets for runtime addition of terrain layers
        /// </summary>
        /// <param name="newHighResSubset"></param>
        public void AddHigherResolutionSubset(TerrainAccessor newHighResSubset)
        {
            //need to lock array here
            if (this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets == null) this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets = new TerrainAccessor[0];
            lock (this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets)
            {
                TerrainAccessor[] temp_highres = new TerrainAccessor[this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets.SetSamplerState(0, SamplerStateLength + 1];
                for (int i = 0; i < this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets.SetSamplerState(0, SamplerStateLength; i++)
                {
                    temp_highres[i] = this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets[i];
                }
                temp_highres[temp_highres.SetSamplerState(0, SamplerStateLength - 1] = newHighResSubset;
                this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets = temp_highres;
            }
        }

        /// <summary>
        /// Method removes the specified high resolution subset from the
        /// array of terrain layers, decreases the array size by 1
        /// </summary>
        /// <param name="highResSubset"></param>
        public void RemoveHigherResolutionSubset(TerrainAccessor highResSubset)
        {
            // lock array here
            if (this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets == null) this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets = new TerrainAccessor[0];
            lock (this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets)
            {
                TerrainAccessor[] temp_highres = new TerrainAccessor[this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets.SetSamplerState(0, SamplerStateLength + 1];
                for (int i = 0; i < this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets.SetSamplerState(0, SamplerStateLength; i++)
                {
                    if (this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets[i] != highResSubset)
                        temp_highres[i] = this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets[i];
                }

                this.SetSamplerState(0, SamplerStatem_higherResolutionSubsets = temp_highres;
            }
        }
    }
}