using System;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;

namespace WorldWind
{
	public class SurfaceTile : IDisposable
	{
		#region Private Members
		int m_Level;
		double m_North;
		double m_South;
		double m_West;
		double m_East;
		bool m_Initialized;
		Device m_Device;
		Texture m_RenderTexture;
	//	Surface m_RenderSurface = null;
	//	bool m_InitTexture = true;
	//	bool m_DeviceBindingMade = false;
		float[,] m_HeightData;
		CustomVertex.TransformedColoredTextured[] m_RenderToTextureVertices = new CustomVertex.TransformedColoredTextured[4];
		DynamicTexture m_DynamicTexture;
		bool m_RequiresUpdate;
		float m_VerticalExaggeration = float.NaN;

		DateTime m_LastUpdate = DateTime.Now;

		WorldSurfaceRenderer m_ParentWorldSurfaceRenderer;
		BoundingBox m_BoundingBox;
		short[] m_NwIndices = null;
		short[] m_NeIndices = null;
		short[] m_SwIndices = null;
		short[] m_SeIndices = null;

		SurfaceTile m_NorthWestChild;
		SurfaceTile m_NorthEastChild;
		SurfaceTile m_SouthWestChild;
		SurfaceTile m_SouthEastChild;

		short[] m_IndicesElevated;
		
		#endregion

		/// <summary>
		/// Creates a new <see cref="SurfaceTile"/> instance.
		/// </summary>
		/// <param name="north">North. (in degrees)</param>
		/// <param name="south">South. (in degrees)</param>
		/// <param name="west">West. (in degrees)</param>
		/// <param name="east">East. (in degrees)</param>
		/// <param name="level">Level.</param>
		/// <param name="parentWorldSurfaceRenderer">Parent world surface renderer.</param>
		public SurfaceTile(double north, double south, double west, double east, int level,
			WorldSurfaceRenderer parentWorldSurfaceRenderer)
		{
            this.m_North = north;
            this.m_South = south;
            this.m_West = west;
            this.m_East = east;
            this.m_Level = level;
            this.m_ParentWorldSurfaceRenderer = parentWorldSurfaceRenderer;
		
			float scale = 1.1f;

			Vector3 v = MathEngine.SphericalToCartesian(
				Angle.FromDegrees(south),
				Angle.FromDegrees(west), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel);

			Vector3 v0 = new Vector3((float)v.X, (float)v.Y, (float)v.Z);
			Vector3 v1 = Vector3.Multiply(v0, scale);

			v = MathEngine.SphericalToCartesian(
				Angle.FromDegrees(south),
				Angle.FromDegrees(east), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel);

			Vector3 v2 = new Vector3((float)v.X, (float)v.Y, (float)v.Z);
			Vector3 v3 = Vector3.Multiply(v2, scale);

			v = MathEngine.SphericalToCartesian(
				Angle.FromDegrees(north),
				Angle.FromDegrees(west), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel);

			Vector3 v4 = new Vector3((float)v.X, (float)v.Y, (float)v.Z);
			Vector3 v5 = Vector3.Multiply(v4, scale);

			v = MathEngine.SphericalToCartesian(
				Angle.FromDegrees(north),
				Angle.FromDegrees(east), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel);

			Vector3 v6 = new Vector3((float)v.X, (float)v.Y, (float)v.Z);
			Vector3 v7 = Vector3.Multiply(v6, scale);

            this.m_BoundingBox = new BoundingBox(v0, v1, v2, v3, v4, v5, v6, v7);

			int thisVertexDensityElevatedPlus2 = ((int) this.m_ParentWorldSurfaceRenderer.SamplesPerTile / 2 + 2);
            this.m_IndicesElevated = new short[2 * thisVertexDensityElevatedPlus2 * thisVertexDensityElevatedPlus2 * 3]; 

			for(int i = 0; i < thisVertexDensityElevatedPlus2; i++)
			{
				int elevated_idx = (2*3*i*thisVertexDensityElevatedPlus2);
				for(int j = 0; j < thisVertexDensityElevatedPlus2; j++)
				{
                    this.m_IndicesElevated[elevated_idx] = (short)(i*(thisVertexDensityElevatedPlus2 + 1) + j);
                    this.m_IndicesElevated[elevated_idx + 1] = (short)((i+1)*(thisVertexDensityElevatedPlus2 + 1) + j);
                    this.m_IndicesElevated[elevated_idx + 2] = (short)(i*(thisVertexDensityElevatedPlus2 + 1) + j+1);

                    this.m_IndicesElevated[elevated_idx + 3] = (short)(i*(thisVertexDensityElevatedPlus2 + 1) + j+1);
                    this.m_IndicesElevated[elevated_idx + 4] = (short)((i+1)*(thisVertexDensityElevatedPlus2 + 1) + j);
                    this.m_IndicesElevated[elevated_idx + 5] = (short)((i+1)*(thisVertexDensityElevatedPlus2 + 1) + j+1);

					elevated_idx += 6;
				}
			}
		}

		/// <summary>
		/// Gets the render texture.
		/// </summary>
		/// <value></value>
		public Texture RenderTexture
		{
			get
			{
				return this.m_RenderTexture;
			}
		}

		private void OnDeviceReset(object sender, EventArgs e)
		{
			Device dev = (Device)sender;

            this.m_Device = dev;

            this.m_RenderTexture = new Texture(
				dev,
				WorldSurfaceRenderer.RenderSurfaceSize,
				WorldSurfaceRenderer.RenderSurfaceSize,
				1,
				Usage.RenderTarget,
				Format.X8R8G8B8,
				Pool.Default);

            this.m_RequiresUpdate = true;
		}

		private void OnDeviceDispose(object sender, EventArgs e)
		{
			if(this.m_RenderTexture != null && !this.m_RenderTexture.IsDisposed)
			{
                this.m_RenderTexture.Dispose();
			}
		}

		/// <summary>
		/// Initializes the specified draw args.
		/// </summary>
		/// <param name="drawArgs">DrawArgs.</param>
		public void Initialize(DrawArgs drawArgs)
		{
			try
			{
				if(this.m_HeightData == null)
				{
					if(this.m_Level > 2)
					{
						Terrain.TerrainTile tt = this.m_ParentWorldSurfaceRenderer.ParentWorld.TerrainAccessor.GetElevationArray(this.m_North, this.m_South, this.m_West, this.m_East, 
							(int) this.m_ParentWorldSurfaceRenderer.SamplesPerTile + 1);
						
						if(tt.ElevationData != null)
						{
                            this.m_HeightData = tt.ElevationData;
						}
						else
						{
                            this.m_HeightData = new float[(uint) this.m_ParentWorldSurfaceRenderer.SamplesPerTile + 1, (uint) this.m_ParentWorldSurfaceRenderer.SamplesPerTile + 1];
						}
					}
					else
					{
                        this.m_HeightData = new float[(uint) this.m_ParentWorldSurfaceRenderer.SamplesPerTile + 1, (uint) this.m_ParentWorldSurfaceRenderer.SamplesPerTile + 1];
					}
				}

				if(this.m_DynamicTexture == null)
				{
                    this.m_DynamicTexture = new DynamicTexture();
                    this.buildTerrainMesh();
				}
			}
			catch(Exception ex)
			{	
				Log.Write(ex);
			}

            this.m_Initialized = true;
		}
				
		/// <summary>
		/// Updates the render surface.
		/// </summary>
		/// <param name="drawArgs">Draw args.</param>
		public void UpdateRenderSurface(DrawArgs drawArgs)
		{
			if(this.m_RenderTexture == null)
			{
				drawArgs.device.DeviceReset += new EventHandler(this.OnDeviceReset);
                this.OnDeviceReset(drawArgs.device, null);
			}
		
			Viewport view = new Viewport();
			view.Width = WorldSurfaceRenderer.RenderSurfaceSize;
			view.Height = WorldSurfaceRenderer.RenderSurfaceSize;
			
			if(drawArgs.RenderWireFrame)
			{
				drawArgs.device.SetRenderState(RenderState.FillMode , FillMode.Solid);
			}
			using(Surface renderSurface = this.m_RenderTexture.GetSurfaceLevel(0))
			{
                this.m_ParentWorldSurfaceRenderer.RenderToSurface.BeginScene(renderSurface, view);
					drawArgs.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.Black, 1.0f, 0);

					drawArgs.device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
					drawArgs.device.SetRenderState(RenderState.ZEnable , false);

					double latRange = this.m_North - this.m_South;
					double lonRange = this.m_East - this.m_West;

					lock(this.m_ParentWorldSurfaceRenderer.SurfaceImages.SyncRoot)
					{
						for(int i = 0; i < this.m_ParentWorldSurfaceRenderer.SurfaceImages.Count; i++)
						{
							SurfaceImage currentSurfaceImage = this.m_ParentWorldSurfaceRenderer.SurfaceImages[i] as SurfaceImage;
					
							if(currentSurfaceImage == null || 
								currentSurfaceImage.ImageTexture == null || 
								currentSurfaceImage.ImageTexture.Disposed || 
								!currentSurfaceImage.Enabled ||
								(currentSurfaceImage.North > this.m_North && currentSurfaceImage.South >= this.m_North) ||
								(currentSurfaceImage.North <= this.m_South && currentSurfaceImage.South < this.m_South) ||
								(currentSurfaceImage.West < this.m_West && currentSurfaceImage.East <= this.m_West) ||
								(currentSurfaceImage.West >= this.m_East && currentSurfaceImage.East > this.m_East)
								)
							{
								continue;
							}

							currentSurfaceImage.Opacity = currentSurfaceImage.ParentRenderable.Opacity;

							Vector2 tex = currentSurfaceImage.GetTextureCoordinate(this.m_North, this.m_West);
                            this.m_RenderToTextureVertices[0].X = 0;
                            this.m_RenderToTextureVertices[0].Y = 0;
                            this.m_RenderToTextureVertices[0].Z = 0.0f;
                            this.m_RenderToTextureVertices[0].Tu = tex.Y;
                            this.m_RenderToTextureVertices[0].Tv = tex.X;
                            this.m_RenderToTextureVertices[0].Color = System.Drawing.Color.FromArgb(
								currentSurfaceImage.ParentRenderable.Opacity,
								0,0,0).ToArgb();

							tex = currentSurfaceImage.GetTextureCoordinate(this.m_South, this.m_West);
                            this.m_RenderToTextureVertices[1].X = 0;
                            this.m_RenderToTextureVertices[1].Y = WorldSurfaceRenderer.RenderSurfaceSize;
                            this.m_RenderToTextureVertices[1].Z = 0.0f;
                            this.m_RenderToTextureVertices[1].Tu = tex.Y;
                            this.m_RenderToTextureVertices[1].Tv = tex.X;
                            this.m_RenderToTextureVertices[1].Color = System.Drawing.Color.FromArgb(
								currentSurfaceImage.ParentRenderable.Opacity,
								0,0,0).ToArgb();

							tex = currentSurfaceImage.GetTextureCoordinate(this.m_North, this.m_East);
                            this.m_RenderToTextureVertices[2].X = WorldSurfaceRenderer.RenderSurfaceSize;
                            this.m_RenderToTextureVertices[2].Y = 0;
                            this.m_RenderToTextureVertices[2].Z = 0.0f;
                            this.m_RenderToTextureVertices[2].Tu = tex.Y;
                            this.m_RenderToTextureVertices[2].Tv = tex.X;
                            this.m_RenderToTextureVertices[2].Color = System.Drawing.Color.FromArgb(
								currentSurfaceImage.ParentRenderable.Opacity,
								0,0,0).ToArgb();

							tex = currentSurfaceImage.GetTextureCoordinate(this.m_South, this.m_East);
                            this.m_RenderToTextureVertices[3].X = WorldSurfaceRenderer.RenderSurfaceSize;
                            this.m_RenderToTextureVertices[3].Y = WorldSurfaceRenderer.RenderSurfaceSize;
                            this.m_RenderToTextureVertices[3].Z = 0.0f;
                            this.m_RenderToTextureVertices[3].Tu = tex.Y;
                            this.m_RenderToTextureVertices[3].Tv = tex.X;
                            this.m_RenderToTextureVertices[3].Color = System.Drawing.Color.FromArgb(
								currentSurfaceImage.ParentRenderable.Opacity,
								0,0,0).ToArgb();
						
							drawArgs.device.SetRenderState(RenderState.AlphaBlendEnable , true);
							drawArgs.device.SetRenderState(RenderState.SourceBlend , Blend.SourceAlpha);
							drawArgs.device.SetRenderState(RenderState.DestinationBlend , Blend.InverseSourceAlpha);

							drawArgs.device.SetSamplerState(0, SamplerState.BorderColor = System.Drawing.Color.FromArgb(0,0,0,0);
							drawArgs.device.SamplerState[1].BorderColor = System.Drawing.Color.FromArgb(0,0,0,0);
							drawArgs.device.SetTexture(0, currentSurfaceImage.ImageTexture);
							drawArgs.device.SetTexture(1, currentSurfaceImage.ImageTexture);
							drawArgs.device.SetTextureStageState(1, TextureStage.TexCoordIndex , 0);
							
							drawArgs.device.SetSamplerState(0, SamplerState.MinFilter = TextureFilter.Linear;
							drawArgs.device.SetSamplerState(0, SamplerState.MagFilter = TextureFilter.Linear;
							drawArgs.device.SetSamplerState(0, SamplerState.AddressU = TextureAddress.Clamp;
							drawArgs.device.SetSamplerState(0, SamplerState.AddressV = TextureAddress.Clamp;
					
							drawArgs.device.SamplerState[1].MinFilter = TextureFilter.Point;
							drawArgs.device.SamplerState[1].MagFilter = TextureFilter.Point;
							drawArgs.device.SamplerState[1].AddressU = TextureAddress.Border;
							drawArgs.device.SamplerState[1].AddressV = TextureAddress.Border;

                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
							drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
							drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg2,TextureArgument.Diffuse);
							drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.Modulate);
							drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.Diffuse);
							drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg2,TextureArgument.Texture);

							drawArgs.device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.SelectArg1);						
							drawArgs.device.SetTextureStageState(1, TextureStage.ColorArg1, TextureArgument.Current);
							drawArgs.device.SetTextureStageState(1, TextureStage.AlphaOperation,  TextureOperation.Modulate);
							drawArgs.device.SetTextureStageState(1, TextureStage.AlphaArg1,TextureArgument.Texture);
							drawArgs.device.SetTextureStageState(1, TextureStage.AlphaArg2,TextureArgument.Diffuse);
							
							drawArgs.device.SetTextureStageState(2, TextureStage.ColorOperation, TextureOperation.Disable);
							drawArgs.device.SetTextureStageState(2, TextureStage.AlphaOperation,  TextureOperation.Disable);

							drawArgs.device.DrawUserPrimitives(
								PrimitiveType.TriangleStrip,
								2, this.m_RenderToTextureVertices);

							drawArgs.device.SetTexture(0, null);
							drawArgs.device.SetTexture(1, null);

                            drawArgs.device.SetTextureStageState(1, TextureStage.TexCoordIndex, 1);
						}

                        this.m_ParentWorldSurfaceRenderer.RenderToSurface.EndScene(Filter.Box);
						drawArgs.device.SetRenderState(RenderState.ZEnable , true);
					}
			}

			if(drawArgs.RenderWireFrame)
			{
				drawArgs.device.SetRenderState(RenderState.FillMode , FillMode.Wireframe);
			}

            this.m_LastUpdate = DateTime.Now;
            this.m_RequiresUpdate = false;
		}

		private void ComputeChildrenTiles(DrawArgs drawArgs)
		{
			if(this.m_NorthWestChild == null)
			{
				SurfaceTile nwc = new SurfaceTile(this.m_North,
					0.5f*(this.m_South+ this.m_North), this.m_West,
					0.5f*(this.m_West+ this.m_East), this.m_Level + 1, this.m_ParentWorldSurfaceRenderer
					);
			
				nwc.Initialize(drawArgs);
                this.m_NorthWestChild = nwc;
			}

			if(this.m_NorthEastChild == null)
			{
				SurfaceTile nec = new SurfaceTile(this.m_North,
					0.5f*(this.m_South+ this.m_North),
					0.5f*(this.m_West+ this.m_East), this.m_East, this.m_Level + 1, this.m_ParentWorldSurfaceRenderer
					);
			
				nec.Initialize(drawArgs);
                this.m_NorthEastChild = nec;
			}

			if(this.m_SouthWestChild == null)
			{
				SurfaceTile swc = new SurfaceTile(
					0.5f*(this.m_South+ this.m_North), this.m_South, this.m_West,
					0.5f*(this.m_West+ this.m_East), this.m_Level + 1, this.m_ParentWorldSurfaceRenderer
					);
			
				swc.Initialize(drawArgs);
                this.m_SouthWestChild = swc;
			}

			if(this.m_SouthEastChild == null)
			{
				SurfaceTile sec = new SurfaceTile(
					0.5f*(this.m_South+ this.m_North), this.m_South,
					0.5f*(this.m_West+ this.m_East), this.m_East, this.m_Level + 1, this.m_ParentWorldSurfaceRenderer
					);
				
				sec.Initialize(drawArgs);
                this.m_SouthEastChild = sec;
			}
		}
		
		private Vector2 GetTextureCoordinate(SurfaceImage surfaceImage, double latitude, double longitude)
		{
			double deltaLat = surfaceImage.North - latitude;
			double deltaLon = longitude - surfaceImage.West;

			double latRange = surfaceImage.North - surfaceImage.South;
			double lonRange = surfaceImage.East - surfaceImage.West;

			Vector2 v = new Vector2(
				(float)(deltaLat / latRange),
				(float)(deltaLon / lonRange));

			return v;
		}

		/// <summary>
		/// Builds the terrain mesh. Also re-builds the terrain mesh, such as when vertical exaggeration has changed.
		/// </summary>
		private void buildTerrainMesh()
		{
			int thisVertexDensityElevatedPlus3 = ((int) this.m_ParentWorldSurfaceRenderer.SamplesPerTile / 2 + 3);

			double scaleFactor = (float)1/(this.m_ParentWorldSurfaceRenderer.SamplesPerTile);
			double latrange = (float)Math.Abs(this.m_North - this.m_South );
			double lonrange = (float)Math.Abs(this.m_East - this.m_West );

            this.m_DynamicTexture.nwVerts = new CustomVertex.PositionNormalTextured[thisVertexDensityElevatedPlus3 * thisVertexDensityElevatedPlus3];
            this.m_DynamicTexture.neVerts = new CustomVertex.PositionNormalTextured[thisVertexDensityElevatedPlus3 * thisVertexDensityElevatedPlus3];
            this.m_DynamicTexture.swVerts = new CustomVertex.PositionNormalTextured[thisVertexDensityElevatedPlus3 * thisVertexDensityElevatedPlus3];
            this.m_DynamicTexture.seVerts = new CustomVertex.PositionNormalTextured[thisVertexDensityElevatedPlus3 * thisVertexDensityElevatedPlus3];

			int base_idx;
			for(int i = 0; i < thisVertexDensityElevatedPlus3; i++)
			{
				base_idx = i*thisVertexDensityElevatedPlus3;
                    
				for(int j = 0; j < thisVertexDensityElevatedPlus3; j++)
				{
					float height = -30000;
					if(i == 0 || j == 0 || i == thisVertexDensityElevatedPlus3 - 1 || j == thisVertexDensityElevatedPlus3 - 1)
					{
						double latitude = 0;
						double longitude = 0;
						float tu = 0.0f;
						float tv = 0.0f;

						if(j == 0)
						{
							longitude = this.m_West;
							tu = 0.0f;
						}
						else if(j == thisVertexDensityElevatedPlus3 - 1)
						{
							longitude = 0.5*(this.m_West + this.m_East);
							tu = 0.5f;
						}
						else
						{
							longitude = this.m_West + (float)scaleFactor*lonrange*(j-1);
							tu = (float)(scaleFactor * (j - 1));
						}

						if(i == 0)
						{
							latitude = this.m_North;
							tv = 0.0f;
						}
						else if(i == thisVertexDensityElevatedPlus3 - 1)
						{
							latitude = 0.5*(this.m_North + this.m_South);
							tv = 0.5f;
						}
						else
						{
							latitude = this.m_North - scaleFactor*latrange*(i-1);
							tv = (float)(scaleFactor * (i - 1));
						}
						
						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(latitude),
							Angle.FromDegrees(longitude), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.nwVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.nwVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.nwVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.nwVerts[base_idx].Tu = tu;
                        this.m_DynamicTexture.nwVerts[base_idx].Tv = tv;


					}
					else
					{

						height = (float) this.m_HeightData[(i - 1), (j - 1)];

						height *= World.Settings.VerticalExaggeration;

						
						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(this.m_North - scaleFactor * latrange * (i - 1)),
							Angle.FromDegrees(this.m_West + scaleFactor * lonrange * (j - 1)), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.nwVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.nwVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.nwVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.nwVerts[base_idx].Tu = (float)(scaleFactor * (j - 1));
                        this.m_DynamicTexture.nwVerts[base_idx].Tv = (float)(scaleFactor * (i - 1));
					}

					base_idx += 1;
				}
			}

			for(int i = 0; i < thisVertexDensityElevatedPlus3; i++)
			{
				base_idx = i*thisVertexDensityElevatedPlus3;
                    
				for(int j = 0; j < thisVertexDensityElevatedPlus3; j++)
				{
					float height = -30000;
					if(i == 0 || j == 0 || i == thisVertexDensityElevatedPlus3 - 1 || j == thisVertexDensityElevatedPlus3 - 1)
					{
						double latitude = 0;
						double longitude = 0;
						float tu = 0.0f;
						float tv = 0.0f;

						if(j == 0)
						{
							longitude = 0.5*(this.m_West + this.m_East);
							tu = 0.5f;
						}
						else if(j == thisVertexDensityElevatedPlus3 - 1)
						{
							longitude = this.m_East;
							tu = 1.0f;
						}
						else
						{
							longitude = 0.5*(this.m_West+ this.m_East) + (float)scaleFactor*lonrange*(j-1);
							tu = (float)(0.5f + scaleFactor * (j - 1));
						}

						if(i == 0)
						{
							latitude = this.m_North;
							tv = 0.0f;
						}
						else if(i == thisVertexDensityElevatedPlus3 - 1)
						{
							latitude = 0.5*(this.m_North + this.m_South);
							tv = 0.5f;
						}
						else
						{
							latitude = this.m_North - scaleFactor*latrange*(i-1);
							tv = (float)(scaleFactor * (i - 1));
						}

						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(latitude),
							Angle.FromDegrees(longitude), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.neVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.neVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.neVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.neVerts[base_idx].Tu = tu;
                        this.m_DynamicTexture.neVerts[base_idx].Tv = tv;
					}
					else
					{
						height = (float) this.m_HeightData[(i - 1), (j-1)+(this.m_ParentWorldSurfaceRenderer.SamplesPerTile / 2)];
						height *= World.Settings.VerticalExaggeration;
						 
						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(this.m_North - scaleFactor*latrange*(i-1)),
							Angle.FromDegrees(0.5f*(this.m_West+ this.m_East) + (float)scaleFactor*lonrange*(j-1)), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.neVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.neVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.neVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.neVerts[base_idx].Tu = (float)(0.5f + scaleFactor * (j - 1));
                        this.m_DynamicTexture.neVerts[base_idx].Tv = (float)(scaleFactor * (i - 1));
					}
					base_idx += 1;
				}
			}

			for(int i = 0; i < thisVertexDensityElevatedPlus3; i++)
			{
				base_idx = i*thisVertexDensityElevatedPlus3;
                    
				for(int j = 0; j < thisVertexDensityElevatedPlus3; j++)
				{
					float height = -30000;
					if(i == 0 || j == 0 || i == thisVertexDensityElevatedPlus3 - 1 || j == thisVertexDensityElevatedPlus3 - 1)
					{
						double latitude = 0;
						double longitude = 0;
						float tu = 0.0f;
						float tv = 0.0f;

						if(j == 0)
						{
							longitude = this.m_West;
							tu = 0.0f;
						}
						else if(j == thisVertexDensityElevatedPlus3 - 1)
						{
							longitude = 0.5*(this.m_West + this.m_East);
							tu = 0.5f;
						}
						else
						{
							longitude = this.m_West + (float)scaleFactor*lonrange*(j-1);
							tu = (float)(scaleFactor * (j - 1));
						}

						if(i == 0)
						{
							latitude = 0.5*(this.m_North + this.m_South);
							tv = 0.5f;
						}
						else if(i == thisVertexDensityElevatedPlus3 - 1)
						{
							latitude = this.m_South;
							tv = 1.0f;
						}
						else
						{
							latitude = 0.5*(this.m_North+ this.m_South) - scaleFactor*latrange*(i-1);
							tv = (float)(0.5f * scaleFactor * (i - 1));
						}

						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(latitude),
							Angle.FromDegrees(longitude), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.swVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.swVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.swVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.swVerts[base_idx].Tu = tu;
                        this.m_DynamicTexture.swVerts[base_idx].Tv = tv;
					}
					else
					{
						height = (float) this.m_HeightData[this.m_ParentWorldSurfaceRenderer.SamplesPerTile / 2 + (i - 1), (j-1)];
						height *= World.Settings.VerticalExaggeration;

						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(0.5*(this.m_North+ this.m_South) - scaleFactor*latrange*(i-1)),
							Angle.FromDegrees(this.m_West + (float)scaleFactor*lonrange*(j-1)), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.swVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.swVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.swVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.swVerts[base_idx].Tu = (float)(scaleFactor * (j - 1));
                        this.m_DynamicTexture.swVerts[base_idx].Tv = (float)(0.5f + scaleFactor * (i - 1));
					}
					base_idx += 1;
				}
			}

			for(int i = 0; i < thisVertexDensityElevatedPlus3; i++)
			{
				base_idx = i*thisVertexDensityElevatedPlus3;
                    
				for(int j = 0; j < thisVertexDensityElevatedPlus3; j++)
				{
					float height = -30000;
					if(i == 0 || j == 0 || i == thisVertexDensityElevatedPlus3 - 1 || j == thisVertexDensityElevatedPlus3 - 1)
					{
						double latitude = 0;
						double longitude = 0;
						float tu = 0.0f;
						float tv = 0.0f;

						if(j == 0)
						{
							longitude = 0.5*(this.m_West + this.m_East);
							tu = 0.5f;
						}
						else if(j == thisVertexDensityElevatedPlus3 - 1)
						{
							longitude = this.m_East;
							tu = 1.0f;
						}
						else
						{
							longitude = 0.5*(this.m_West+ this.m_East) + (float)scaleFactor*lonrange*(j-1);
							tu = (float)(0.5f + scaleFactor * (j - 1));
						}

						if(i == 0)
						{
							latitude = 0.5*(this.m_North + this.m_South);
							tv = 0.5f;
						}
						else if(i == thisVertexDensityElevatedPlus3 - 1)
						{
							latitude = this.m_South;
							tv = 1.0f;
						}
						else
						{
							latitude = 0.5*(this.m_North+ this.m_South) - scaleFactor*latrange*(i-1);
							tv = (float)(0.5f + scaleFactor * (i - 1));
						}

						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(latitude),
							Angle.FromDegrees(longitude), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.seVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.seVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.seVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.seVerts[base_idx].Tu = tu;
                        this.m_DynamicTexture.seVerts[base_idx].Tv = tv;
					}
					else
					{
						height = (float) this.m_HeightData[this.m_ParentWorldSurfaceRenderer.SamplesPerTile / 2 + (i - 1), (j-1)+(this.m_ParentWorldSurfaceRenderer.SamplesPerTile / 2)];
								
						height *= World.Settings.VerticalExaggeration;

						Vector3 v = MathEngine.SphericalToCartesian(
							Angle.FromDegrees(0.5*(this.m_North+ this.m_South) - scaleFactor*latrange*(i-1)),
							Angle.FromDegrees(0.5*(this.m_West+ this.m_East) + (float)scaleFactor*lonrange*(j-1)), this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius + this.m_ParentWorldSurfaceRenderer.DistanceAboveSeaLevel + height);

                        this.m_DynamicTexture.seVerts[base_idx].X = v.X;
                        this.m_DynamicTexture.seVerts[base_idx].Y = v.Y;
                        this.m_DynamicTexture.seVerts[base_idx].Z = v.Z;
                        this.m_DynamicTexture.seVerts[base_idx].Tu = (float)(0.5f + scaleFactor * (j - 1));
                        this.m_DynamicTexture.seVerts[base_idx].Tv = (float)(0.5f + scaleFactor * (i - 1));
					}
					base_idx += 1;
				}
				
			}
			double equatoralRadius = this.m_ParentWorldSurfaceRenderer.ParentWorld.EquatorialRadius;

			try
			{
				if(this.m_VerticalExaggeration != World.Settings.VerticalExaggeration)
				{
				//	m_NwIndices = CreateTriangleIndicesBTT(m_DynamicTexture.nwVerts, (int)m_ParentWorldSurfaceRenderer.SamplesPerTile/2, 1, equatoralRadius); 
				//	m_NeIndices = CreateTriangleIndicesBTT(m_DynamicTexture.neVerts, (int)m_ParentWorldSurfaceRenderer.SamplesPerTile/2, 1, equatoralRadius); 
				//	m_SwIndices = CreateTriangleIndicesBTT(m_DynamicTexture.swVerts, (int)m_ParentWorldSurfaceRenderer.SamplesPerTile/2, 1, equatoralRadius); 
				//	m_SeIndices = CreateTriangleIndicesBTT(m_DynamicTexture.seVerts, (int)m_ParentWorldSurfaceRenderer.SamplesPerTile/2, 1, equatoralRadius); 
				
				//	calculate_normals(ref m_DynamicTexture.nwVerts, m_IndicesElevated);
				//	calculate_normals(ref m_DynamicTexture.neVerts, m_IndicesElevated);
				//	calculate_normals(ref m_DynamicTexture.swVerts, m_IndicesElevated);
				//	calculate_normals(ref m_DynamicTexture.seVerts, m_IndicesElevated);

                this.m_VerticalExaggeration = World.Settings.VerticalExaggeration;
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}
		
		// Create indice list (PM)
		private short[] CreateTriangleIndicesBTT(CustomVertex.PositionTextured[] ElevatedVertices, int VertexDensity, int Margin, double LayerRadius)
		{
			BinaryTriangleTree Tree = new BinaryTriangleTree(ElevatedVertices, VertexDensity, Margin, LayerRadius);
			Tree.BuildTree(7); // variance 0 = best fit
			Tree.BuildIndices();
			return Tree.Indices;
		}

		private short[] CreateTriangleIndicesRegular(CustomVertex.PositionTextured[] ElevatedVertices, int VertexDensity, int Margin, double LayerRadius)
		{
			short[] indices;
			int thisVertexDensityElevatedPlus2 = (VertexDensity + (Margin * 2));
			indices = new short[2 * thisVertexDensityElevatedPlus2 * thisVertexDensityElevatedPlus2 * 3]; 

			for(int i = 0; i < thisVertexDensityElevatedPlus2; i++)
			{
				int elevated_idx = (2*3*i*thisVertexDensityElevatedPlus2);
				for(int j = 0; j < thisVertexDensityElevatedPlus2; j++)
				{
					indices[elevated_idx] = (short)(i*(thisVertexDensityElevatedPlus2 + 1) + j);
					indices[elevated_idx + 1] = (short)((i+1)*(thisVertexDensityElevatedPlus2 + 1) + j);
					indices[elevated_idx + 2] = (short)(i*(thisVertexDensityElevatedPlus2 + 1) + j+1);

					indices[elevated_idx + 3] = (short)(i*(thisVertexDensityElevatedPlus2 + 1) + j+1);
					indices[elevated_idx + 4] = (short)((i+1)*(thisVertexDensityElevatedPlus2 + 1) + j);
					indices[elevated_idx + 5] = (short)((i+1)*(thisVertexDensityElevatedPlus2 + 1) + j+1);

					elevated_idx += 6;
				}
			}
			return indices;
		}

		/// <summary>
		/// Updates the specified draw args.
		/// </summary>
		/// <param name="drawArgs">Draw args.</param>
		public void Update(DrawArgs drawArgs)
		{
			try
			{
				if(!this.m_Initialized) this.Initialize(drawArgs);
				
				float scaleFactor = 1f/(this.m_ParentWorldSurfaceRenderer.SamplesPerTile);
				float latrange = (float)Math.Abs(this.m_North - this.m_South );
				float lonrange = (float)Math.Abs(this.m_East - this.m_West );
				
				int thisVertexDensityElevatedPlus3 = ((int) this.m_ParentWorldSurfaceRenderer.SamplesPerTile / 2 + 3);

				scaleFactor = (float)1/(this.m_ParentWorldSurfaceRenderer.SamplesPerTile);
				latrange = (float)Math.Abs(this.m_North - this.m_South );
				lonrange = (float)Math.Abs(this.m_East - this.m_West );

				double centerLatitude = 0.5*(this.m_North + this.m_South);
				double centerLongitude = 0.5*(this.m_East + this.m_West);
				double tileSize = (this.m_North - this.m_South);

				if(this.m_VerticalExaggeration != World.Settings.VerticalExaggeration)
				{
				//	buildTerrainMesh();
				}

				if(drawArgs.WorldCamera.TrueViewRange < Angle.FromDegrees(3.0f*tileSize) 
						&& MathEngine.SphericalDistance( Angle.FromDegrees(centerLatitude), Angle.FromDegrees(centerLongitude), 
						drawArgs.WorldCamera.Latitude, drawArgs.WorldCamera.Longitude) < Angle.FromDegrees( 2.9f*tileSize )
						&& drawArgs.WorldCamera.ViewFrustum.Intersects(this.m_BoundingBox))
				{
					if(this.m_NorthWestChild == null || this.m_NorthEastChild == null || this.m_SouthWestChild == null || this.m_SouthEastChild == null)
					{
                        this.ComputeChildrenTiles(drawArgs);
					}
					else
					{
						if(this.m_NorthEastChild != null)
						{
                            this.m_NorthEastChild.Update(drawArgs);
						}
						if(this.m_NorthWestChild != null)
						{
                            this.m_NorthWestChild.Update(drawArgs);
						}
						if(this.m_SouthEastChild != null)
						{
                            this.m_SouthEastChild.Update(drawArgs);
						}
						if(this.m_SouthWestChild != null)
						{
                            this.m_SouthWestChild.Update(drawArgs);
						}
					}			
				}
				else 
				{
					if(this.m_NorthWestChild != null)
					{
                        this.m_NorthWestChild.Dispose();
                        this.m_NorthWestChild = null;
					}

					if(this.m_NorthEastChild != null)
					{
                        this.m_NorthEastChild.Dispose();
                        this.m_NorthEastChild = null;
					}

					if(this.m_SouthEastChild != null)
					{
                        this.m_SouthEastChild.Dispose();
                        this.m_SouthEastChild = null;
					}

					if(this.m_SouthWestChild != null)
					{
                        this.m_SouthWestChild.Dispose();
                        this.m_SouthWestChild = null;
					}
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

		/// <summary>
		/// Disposes this instance. Releases any resources from the graphics device, also disposes of "child" surface tiles.
		/// </summary>
		public void Dispose()
		{
            this.m_Initialized = false;
            this.m_BoundingBox = null;

			if(this.m_Device != null)
			{
                this.m_Device.DeviceReset -= new EventHandler(this.OnDeviceReset);
                this.m_Device.Disposing -= new EventHandler(this.OnDeviceDispose);

                this.OnDeviceDispose(this.m_Device, null);
			}
	
			if(this.m_NorthWestChild != null)
			{
                this.m_NorthWestChild.Dispose();
                this.m_NorthWestChild = null;
			}
			if(this.m_NorthEastChild != null)
			{
                this.m_NorthEastChild.Dispose();
                this.m_NorthEastChild = null;
			}
			if(this.m_SouthWestChild != null)
			{
                this.m_SouthWestChild.Dispose();
                this.m_SouthWestChild = null;
			}
			if(this.m_SouthEastChild != null)
			{
                this.m_SouthEastChild.Dispose();
                this.m_SouthEastChild = null;
			}
			
		}

		private void calculate_normals(ref CustomVertex.PositionNormalTextured[] vertices, short[] indices)
		{
			System.Collections.ArrayList[] normal_buffer = new System.Collections.ArrayList[vertices.Length];
			for(int i = 0; i < vertices.Length; i++)
			{
				normal_buffer[i] = new System.Collections.ArrayList();
			}
			for(int i = 0; i < this.m_IndicesElevated.Length; i += 3)
			{
				Vector3 p1 = vertices[indices[i+0]].Position;
				Vector3 p2 = vertices[indices[i+1]].Position;
				Vector3 p3 = vertices[indices[i+2]].Position;

				Vector3 v1 = p2 - p1;
				Vector3 v2 = p3 - p1;
				Vector3 normal = Vector3.Cross(v1, v2);

				normal.Normalize();

				// Store the face's normal for each of the vertices that make up the face.
				normal_buffer[indices[i+0]].Add( normal );
				normal_buffer[indices[i+1]].Add( normal );
				normal_buffer[indices[i+2]].Add( normal );
			}


			// Now loop through each vertex vector, and avarage out all the normals stored.
			for( int i = 0; i < vertices.Length; ++i )
			{
				for( int j = 0; j < normal_buffer[i].Count; ++j )
				{
					Vector3 curNormal = (Vector3)normal_buffer[i][j];
					
					if(vertices[i].Normal == Vector3.Zero)
						vertices[i].Normal = curNormal;
					else
						vertices[i].Normal += curNormal;
				}
    
				vertices[i].Normal.Multiply(1.0f / normal_buffer[i].Count);
				
			}
		}

		/// <summary>
		/// Determines whether this surface tile is renderable.
		/// </summary>
		/// <param name="drawArgs">Draw args.</param>
		/// <returns>
		/// 	<c>true</c> if the surface tile is renderable; otherwise, <c>false</c>.
		/// </returns>
		public bool IsRenderable(DrawArgs drawArgs)
		{
			double _centerLat = 0.5*(this.m_North + this.m_South);
			double _centerLon = 0.5*(this.m_East + this.m_West);
			double m_DynamicTextureSize = (this.m_North - this.m_South);

			if(!this.m_Initialized || 
				drawArgs.WorldCamera.TrueViewRange / 2 > Angle.FromDegrees( 3.0*m_DynamicTextureSize*1.5f ) || 
				MathEngine.SphericalDistance(Angle.FromDegrees(_centerLat), Angle.FromDegrees(_centerLon),
				drawArgs.WorldCamera.Latitude, drawArgs.WorldCamera.Longitude) > Angle.FromDegrees( 3.0*m_DynamicTextureSize*1.5f ) || this.m_BoundingBox == null || 
				!drawArgs.WorldCamera.ViewFrustum.Intersects(this.m_BoundingBox) || this.m_DynamicTexture == null //||
				//m_RenderTexture == null ||
				//m_RenderTexture.Disposed
				)
			{
			//	if(m_Level == 0)
			//	{
			//		return true;
			//	}
			//	else
			//	{
					return false;
			//	}
			}
			else
			{
				return true;
			}
		}

		private bool checkSurfaceImageChange()
		{
			//TODO: Make this smart enough to check only *this* surface tile
			if(this.m_ParentWorldSurfaceRenderer.LastChange > this.m_LastUpdate)
			{
				return true;
			}

			lock(this.m_ParentWorldSurfaceRenderer.SurfaceImages.SyncRoot)
			{
				for(int i = 0; i < this.m_ParentWorldSurfaceRenderer.SurfaceImages.Count; i++)
				{
					SurfaceImage currentSurfaceImage = this.m_ParentWorldSurfaceRenderer.SurfaceImages[i] as SurfaceImage;
					
					if(currentSurfaceImage.LastUpdate > this.m_LastUpdate || currentSurfaceImage.Opacity != currentSurfaceImage.ParentRenderable.Opacity)
					{
						if(currentSurfaceImage == null || 
							currentSurfaceImage.ImageTexture == null || 
							currentSurfaceImage.ImageTexture.Disposed || 
							!currentSurfaceImage.Enabled ||
							(currentSurfaceImage.North > this.m_North && currentSurfaceImage.South >= this.m_North) ||
							(currentSurfaceImage.North <= this.m_South && currentSurfaceImage.South < this.m_South) ||
							(currentSurfaceImage.West < this.m_West && currentSurfaceImage.East <= this.m_West) ||
							(currentSurfaceImage.West >= this.m_East && currentSurfaceImage.East > this.m_East)
							)
						{
							continue;
						}
						else
						{
							return true;
						}
					}
					else
					{
						continue;
					}
				}
			}

			return false;
		}

		int m_FramesSinceLastUpdate;
		public static int SurfaceTileRefreshHz = 35;

		/// <summary>
		/// Renders the surface tile.
		/// </summary>
		/// <param name="drawArgs">Draw args.</param>
		public void Render(DrawArgs drawArgs)
		{
			try
			{
				if(!this.IsRenderable(drawArgs))
				{
					if(this.m_RenderTexture != null)
					{
						return;
					}
				}

				bool nwRendered = false;
				bool neRendered = false;
				bool swRendered = false;
				bool seRendered = false;

				if(this.m_NorthWestChild != null && this.m_NorthWestChild.m_Initialized && this.m_NorthWestChild.IsRenderable(drawArgs))
				{
					if(this.m_NorthWestChild.RenderTexture != null ||
                       (this.m_NorthWestChild.RenderTexture == null && this.m_ParentWorldSurfaceRenderer.NumberTilesUpdated < 2))
					{
                        this.m_NorthWestChild.Render(drawArgs);
						nwRendered = true;
					}
				}
				
				if(this.m_NorthEastChild != null && this.m_NorthEastChild.m_Initialized && this.m_NorthEastChild.IsRenderable(drawArgs))
				{
					if(this.m_NorthEastChild.RenderTexture != null ||
                       (this.m_NorthEastChild.RenderTexture == null && this.m_ParentWorldSurfaceRenderer.NumberTilesUpdated < 2))
					{
                        this.m_NorthEastChild.Render(drawArgs);
						neRendered = true;
					}
				}
				
				if(this.m_SouthWestChild != null && this.m_SouthWestChild.m_Initialized && this.m_SouthWestChild.IsRenderable(drawArgs))
				{
					if(this.m_SouthWestChild.RenderTexture != null ||
                       (this.m_SouthWestChild.RenderTexture == null && this.m_ParentWorldSurfaceRenderer.NumberTilesUpdated < 2))
					{
                        this.m_SouthWestChild.Render(drawArgs);
						swRendered = true;
					}
				}	
				
				if(this.m_SouthEastChild != null && this.m_SouthEastChild.m_Initialized && this.m_SouthEastChild.IsRenderable(drawArgs))
				{
					if(this.m_SouthEastChild.RenderTexture != null ||
                       (this.m_SouthEastChild.RenderTexture == null && this.m_ParentWorldSurfaceRenderer.NumberTilesUpdated < 2))
					{
                        this.m_SouthEastChild.Render(drawArgs);
						seRendered = true;
					}
				}

				if(nwRendered && neRendered && swRendered && seRendered)
					return;

				if(this.m_RenderTexture == null || this.m_RequiresUpdate || this.checkSurfaceImageChange() || this.m_FramesSinceLastUpdate++ > SurfaceTileRefreshHz)
				{
					drawArgs.device.EndScene();
                    this.UpdateRenderSurface(drawArgs);
					drawArgs.device.BeginScene();

                    this.m_FramesSinceLastUpdate = 0;
                    this.m_ParentWorldSurfaceRenderer.NumberTilesUpdated++;
				}

				if(this.m_RenderTexture == null)
					return;
				
				drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
				drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.SelectArg1);
				drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
				drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
				drawArgs.device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Disable);
				drawArgs.device.SetTextureStageState(1, TextureStage.AlphaOperation,  TextureOperation.Disable);
				drawArgs.device.SetRenderState(RenderState.ZEnable, true);
				
				drawArgs.device.SetTexture(0, this.m_RenderTexture);

				drawArgs.device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
				drawArgs.device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
				drawArgs.device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
				drawArgs.device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);

				if(!nwRendered && this.m_DynamicTexture.nwVerts != null)
				{
					drawArgs.device.DrawIndexedUserPrimitives(
						PrimitiveType.TriangleList,
						0, this.m_DynamicTexture.nwVerts.Length,
						(this.m_NwIndices != null ? this.m_NwIndices.Length / 3 : this.m_IndicesElevated.Length / 3),
						(this.m_NwIndices != null ? this.m_NwIndices : this.m_IndicesElevated),
                        Format.Index16, this.m_DynamicTexture.nwVerts);
				}
	
				if(!neRendered && this.m_DynamicTexture.neVerts != null)
				{
					drawArgs.device.DrawIndexedUserPrimitives(
						PrimitiveType.TriangleList,
						0, this.m_DynamicTexture.neVerts.Length,
						(this.m_NeIndices != null ? this.m_NeIndices.Length / 3 : this.m_IndicesElevated.Length / 3),
						(this.m_NeIndices != null ? this.m_NeIndices : this.m_IndicesElevated),
                        Format.Index16, this.m_DynamicTexture.neVerts);
				}	

				if(!swRendered && this.m_DynamicTexture.swVerts != null)
				{
					drawArgs.device.DrawIndexedUserPrimitives(
						PrimitiveType.TriangleList,
						0, this.m_DynamicTexture.nwVerts.Length,
						(this.m_SwIndices != null ? this.m_SwIndices.Length / 3 : this.m_IndicesElevated.Length / 3),
						(this.m_SwIndices != null ? this.m_SwIndices : this.m_IndicesElevated),
                        Format.Index16, this.m_DynamicTexture.swVerts);
				}	
						
				if(!seRendered && this.m_DynamicTexture.seVerts != null)
				{
					drawArgs.device.DrawIndexedUserPrimitives(
						PrimitiveType.TriangleList,
						0, this.m_DynamicTexture.seVerts.Length,
						(this.m_SeIndices != null ? this.m_SeIndices.Length / 3 : this.m_IndicesElevated.Length / 3),
						(this.m_SeIndices != null ? this.m_SeIndices : this.m_IndicesElevated),
                        Format.Index16, this.m_DynamicTexture.seVerts);
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				//TODO: Remove this
				Log.Write(ex);
			}
		}
	}

	class DynamicTexture : IDisposable
	{		
		public CustomVertex.PositionNormalTextured[] nwVerts;
		public CustomVertex.PositionNormalTextured[] neVerts;
		public CustomVertex.PositionNormalTextured[] swVerts;
		public CustomVertex.PositionNormalTextured[] seVerts;

		public DynamicTexture()
		{
		}
		
		#region IDisposable Members

		public void Dispose()
		{
		}
		#endregion
	}
}
