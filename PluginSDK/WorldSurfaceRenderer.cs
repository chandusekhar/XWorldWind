using System;
using Utility;

namespace WorldWind
{
	/// <summary>
	/// Summary description for SurfaceRenderer.
	/// </summary>
	public class WorldSurfaceRenderer
	{
		public const int RenderSurfaceSize = 256;
		
		#region Private Members
		RenderToSurface m_Rts = null;
		const int m_NumberRootTilesHigh = 5;
		uint m_SamplesPerTile;
		World m_ParentWorld;
		SurfaceTile[] m_RootSurfaceTiles;
		double m_DistanceAboveSeaLevel;
		bool m_Initialized;
		System.Collections.ArrayList m_SurfaceImages = new System.Collections.ArrayList();
		System.Collections.Queue m_TextureLoadQueue = new System.Collections.Queue();
		#endregion

		#region Properties
		/// <summary>
		/// Gets the surface images.
		/// </summary>
		/// <value></value>
		public System.Collections.ArrayList SurfaceImages
		{
			get
			{
				return this.m_SurfaceImages;
			}
		}
		
		/// <summary>
		/// Gets the distance above sea level in meters.
		/// </summary>
		/// <value></value>
		public double DistanceAboveSeaLevel
		{
			get
			{
				return this.m_DistanceAboveSeaLevel;
			}
		}
		
		/// <summary>
		/// Gets the samples per tile.  Also can be considered the Vertex Density or Mesh Density of each SurfaceTile
		/// </summary>
		/// <value></value>
		public uint SamplesPerTile
		{
			get
			{
				return this.m_SamplesPerTile;
			}
		}
		
		/// <summary>
		/// Gets the parent world.
		/// </summary>
		/// <value></value>
		public World ParentWorld
		{
			get
			{
				return this.m_ParentWorld;
			}
		}
		
		#endregion

		public WorldSurfaceRenderer(
			uint samplesPerTile,
			double distanceAboveSeaLevel,
			World parentWorld
			)
		{
            this.m_SamplesPerTile = samplesPerTile;
            this.m_ParentWorld = parentWorld;
            this.m_DistanceAboveSeaLevel = distanceAboveSeaLevel;

			double tileSize = 180.0f / m_NumberRootTilesHigh;

            this.m_RootSurfaceTiles = new SurfaceTile[m_NumberRootTilesHigh * (m_NumberRootTilesHigh * 2)];
			for(int i = 0; i < m_NumberRootTilesHigh; i++)
			{
				for(int j = 0; j < m_NumberRootTilesHigh * 2; j++)
				{
                    this.m_RootSurfaceTiles[i * m_NumberRootTilesHigh * 2 + j] = new SurfaceTile(
						(i + 1) * tileSize - 90.0f,
						i * tileSize - 90.0f,
						j * tileSize - 180.0f,
						(j + 1) * tileSize - 180.0f,
						0,
						this);

				}
			}
		}

		public DateTime LastChange = DateTime.Now;

		public void AddSurfaceImage(SurfaceImage surfaceImage)
		{
			if(surfaceImage.ImageTexture != null)
			{
				lock(this.m_SurfaceImages.SyncRoot)
				{
                    this.m_SurfaceImages.Add(surfaceImage);
                    this.m_SurfaceImages.Sort();
				}

                this.LastChange = DateTime.Now;
			}
			else
			{
				lock(this.m_TextureLoadQueue.SyncRoot)
				{
                    this.m_TextureLoadQueue.Enqueue(surfaceImage);
				}
			}
		}

		public RenderToSurface RenderToSurface
		{
			get
			{
				return this.m_Rts;
			}
		}

		Device m_Device = null;
		private void OnDeviceReset(object sender, EventArgs e)
		{
			Device dev = (Device)sender;

            this.m_Device = dev;

            this.m_Rts = new RenderToSurface(
				dev,
				RenderSurfaceSize,
				RenderSurfaceSize,
				Format.X8R8G8B8,
				true,
				DepthFormat.D16);
		}

		public void RemoveSurfaceImage(string imageResource)
		{
			try
			{
				lock(this.m_SurfaceImages.SyncRoot)
				{
					for(int i = 0; i < this.m_SurfaceImages.Count; i++)
					{
						SurfaceImage current = this.m_SurfaceImages[i] as SurfaceImage;
						if(current != null && current.ImageFilePath == imageResource)
						{
                            this.m_SurfaceImages.RemoveAt(i);
							break;
						}
					}

                    this.m_SurfaceImages.Sort();
				}

                this.LastChange = DateTime.Now;
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				Log.DebugWrite(ex);
			}
		}

		public int NumberTilesUpdated;

		public void Dispose()
		{
            this.m_Initialized = false;
			if(this.m_Device != null)
			{
                this.m_Device.DeviceReset -= new EventHandler(this.OnDeviceReset);
			}

			if(this.m_Rts != null && !this.m_Rts.Disposed)
			{
                this.m_Rts.Dispose();
			}
			lock(this.m_RootSurfaceTiles.SyncRoot)
			{
				foreach(SurfaceTile st in this.m_RootSurfaceTiles)
					st.Dispose();
			}
		}

		public void Initialize(DrawArgs drawArgs)
		{
		//	drawArgs.device.DeviceReset += new EventHandler(OnDeviceReset);
		//	OnDeviceReset(drawArgs.device, null);
			foreach(SurfaceTile st in this.m_RootSurfaceTiles)
			{
				st.Initialize(drawArgs);
			}

            this.m_Initialized = true;
		}

		public void Update(DrawArgs drawArgs)
		{
			try
			{
				if(!this.m_Initialized)
				{
                    this.Initialize(drawArgs);
				}

				foreach(SurfaceTile tile in this.m_RootSurfaceTiles)
				{
					tile.Update(drawArgs);
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}	

		public void RenderSurfaceImages(DrawArgs drawArgs)
		{
			if(this.m_Rts == null)
			{
				drawArgs.device.DeviceReset += new EventHandler(this.OnDeviceReset);
                this.OnDeviceReset(drawArgs.device, null);
			}
			if(!this.m_Initialized)
				return;

			if(this.m_TextureLoadQueue.Count > 0)
			{
				SurfaceImage si = this.m_TextureLoadQueue.Dequeue() as SurfaceImage;
				if(si != null)
				{
					si.ImageTexture = ImageHelper.LoadTexture( si.ImageFilePath );

					lock(this.m_SurfaceImages.SyncRoot)
					{
                        this.m_SurfaceImages.Add(si);
                        this.m_SurfaceImages.Sort();
					}
				}
				drawArgs.TexturesLoadedThisFrame++;
			}

            this.NumberTilesUpdated = 0;
			foreach(SurfaceTile tile in this.m_RootSurfaceTiles)
			{
				if(tile != null)
					tile.Render(drawArgs);
			}
		}
	}


}
