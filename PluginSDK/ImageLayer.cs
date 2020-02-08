using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;
using WorldWind.Net;
using WorldWind.VisualControl;

namespace WorldWind
{
	/// <summary>
	/// Use this class to map a single image to the planet at a desired altitude.
	/// Source image must be in Plate Carree (geographic) map projection:
	/// http://en.wikipedia.org/wiki/Plate_Carr%E9e_Projection
	/// TODO: Update this code to take advantage of the Texture Manager
	/// </summary>
	public class ImageLayer : RenderableObject
	{
		#region Private Members

		protected double layerRadius;
		protected double minLat;
		protected double maxLat;
		protected double minLon;
		protected double maxLon;
		World m_ParentWorld;
		Stream m_TextureStream;

		protected bool _disableZbuffer;
		protected CustomVertex.PositionNormalTextured[] vertices;
		protected static CustomVertex.TransformedColored[] progressBarOutline = new CustomVertex.TransformedColored[5];
		protected static CustomVertex.TransformedColored[] progressBar = new CustomVertex.TransformedColored[4];
		protected short[] indices;

		protected Texture texture;
		protected Device device;
		protected string _imageUrl;
		protected string _imagePath;

		// The triangular mesh density for the rendered sector
		protected int meshPointCount = 64;
		protected TerrainAccessor _terrainAccessor;

		protected int progressBarBackColor = System.Drawing.Color.FromArgb(100, 255, 255, 255).ToArgb();
		protected int progressBarOutlineColor = System.Drawing.Color.SlateGray.ToArgb();
		protected int textColor = System.Drawing.Color.Black.ToArgb();

		protected float downloadPercent;
		protected Thread downloadThread;
		protected float verticalExaggeration;
		protected string m_legendImagePath;
		protected Colorbar legendControl;

		int m_TransparentColor;
		#endregion

        bool m_renderGrayscale;

		TimeSpan cacheExpiration = TimeSpan.MaxValue;
		System.Timers.Timer refreshTimer;
		#region Properties
		
		/// <summary>
		/// Gets or sets the color used for transparent areas.
		/// </summary>
		/// <value></value>
		public int TransparentColor
		{
			get
			{
				return this.m_TransparentColor;
			}
			set
			{
                this.m_TransparentColor = value;
			}
		}
		/// <summary>
		/// The url of the image (when image is on network)
		/// </summary>
		public string ImageUrl
		{
			get
			{
				return this._imageUrl;
			}
			set
			{
				this._imageUrl = value;
			}
		}

		/// <summary>
		/// The Path of the image (local file)
		/// </summary>
		public string ImagePath
		{
			get
			{
				return this._imagePath;
			}
			set
			{
				if(value!=null)
					value = value.Trim();
				if(value==null || value.Trim().Length<=0)
                    this._imagePath = null;
				else
                    this._imagePath = value.Trim();
			}
		}

		/// <summary>
		/// Opacity (0..1)
		/// </summary>
		public float OpacityPercent
		{
			get
			{
				return (float)this.m_opacity / 255;
			}
		}

		public bool DisableZBuffer
		{
			get
			{
				return this._disableZbuffer;
			}
			set
			{
				this._disableZbuffer = value;
			}
		}

		/// <summary>
		/// Longitude at left edge of image
		/// </summary>
		public double MinLon
		{
			get
			{
				return this.minLon;
			}
			set
			{
				this.minLon= value;
			}
		}

		/// <summary>
		/// Latitude at lower edge of image
		/// </summary>
		public double MinLat
		{
			get
			{
				return this.minLat;
			}
			set
			{
				this.minLat= value;
			}
		}

		/// <summary>
		/// Longitude at upper edge of image
		/// </summary>
		public double MaxLon
		{
			get
			{
				return this.maxLon;
			}
			set
			{
				this.maxLon= value;
			}
		}

		/// <summary>
		/// Latitude at upper edge of image
		/// </summary>
		public double MaxLat
		{
			get
			{
				return this.maxLat;
			}
			set
			{
				this.maxLat= value;
			}
		}

		/// <summary>
		/// Path or URL of layer legend image
		/// </summary>
		public string LegendImagePath
		{
			get
			{
				return this.m_legendImagePath;
			}
			set
			{
                this.m_legendImagePath = value;
			}
		}

		#endregion

        float m_grayscaleBrightness;

        public float GrayscaleBrightness
        {
            get { return this.m_grayscaleBrightness; }
            set { this.m_grayscaleBrightness = value; }
        }

        public bool RenderGrayscale
        {
            get { return this.m_renderGrayscale; }
            set { this.m_renderGrayscale = value; }
        }

		public TimeSpan CacheExpiration
		{
			get{ return this.cacheExpiration; }
			set{ this.cacheExpiration = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.ImageLayer"/> class.
		/// </summary>
		public ImageLayer( string name, float layerRadius ) : base (name)
		{
			this.layerRadius = layerRadius;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ImageLayer(
			string name,
			World parentWorld,
			double distanceAboveSurface,
			string imagePath,
			double minLatitude,
			double maxLatitude,
			double minLongitude,
			double maxLongitude,
			byte opacity,
			TerrainAccessor terrainAccessor)
			: base(name, parentWorld.Position, parentWorld.Orientation) 
		{
			this.m_ParentWorld = parentWorld;
			this.layerRadius = (float)parentWorld.EquatorialRadius + distanceAboveSurface;
			this._imagePath = imagePath;
            this.minLat = minLatitude;
            this.maxLat = maxLatitude;
            this.minLon = minLongitude;
            this.maxLon = maxLongitude;
			this.m_opacity = opacity;
			this._terrainAccessor = terrainAccessor;
			this._imagePath = imagePath;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ImageLayer(
			string name,
			World parentWorld,
			double distanceAboveSurface,
			Stream textureStream,
			int transparentColor,
			double minLatitude,
			double maxLatitude,
			double minLongitude,
			double maxLongitude,
			double opacityPercent,
			TerrainAccessor terrainAccessor)
			: this(name,parentWorld,distanceAboveSurface,null, 
			minLatitude, maxLatitude, minLongitude, maxLongitude, 
			(byte)(255*opacityPercent), terrainAccessor)
		{
            this.m_TextureStream = textureStream;
            this.m_TransparentColor = transparentColor;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ImageLayer(
			string name,
			World parentWorld,
			double distanceAboveSurface,
			string imagePath,
			double minLatitude,
			double maxLatitude,
			double minLongitude,
			double maxLongitude,
			double opacityPercent,
			TerrainAccessor terrainAccessor)
			: this(name,parentWorld,distanceAboveSurface,imagePath, 
			minLatitude, maxLatitude, minLongitude, maxLongitude, 
			(byte)(255*opacityPercent), terrainAccessor)
		{
		}

		/// <summary>
		/// Layer initialization code
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				this.device = drawArgs.device;

				if(this._imagePath == null && this._imageUrl != null && this._imageUrl.ToLower().StartsWith("http://"))
				{
                    this._imagePath = getFilePathFromUrl(this._imageUrl);
					
				}

				FileInfo imageFileInfo = null;
				if(this._imagePath != null)
					imageFileInfo = new FileInfo(this._imagePath);

				if(this.downloadThread != null && this.downloadThread.IsAlive)
					return;

				if(this._imagePath != null && this.cacheExpiration != TimeSpan.MaxValue && this.cacheExpiration.TotalMilliseconds > 0 && this._imageUrl.ToLower().StartsWith("http://") &&
                   imageFileInfo != null && 
                   imageFileInfo.Exists &&
                   imageFileInfo.LastWriteTime < DateTime.Now - this.cacheExpiration)
				{
					//attempt to redownload it
                    this.downloadThread = new Thread(new ThreadStart(this.DownloadImage));
                    this.downloadThread.Name = "ImageLayer.DownloadImage";

                    this.downloadThread.IsBackground = true;
                    this.downloadThread.Start();

					return;
				}

				if(this.m_TextureStream != null)
				{
                    this.UpdateTexture(this.m_TextureStream, this.m_TransparentColor);
                    this.verticalExaggeration = World.Settings.VerticalExaggeration;
                    this.CreateMesh();
                    this.isInitialized = true;
					return;
				}
				else if(imageFileInfo != null && imageFileInfo.Exists)
				{
                    this.UpdateTexture(this._imagePath);
                    this.verticalExaggeration = World.Settings.VerticalExaggeration;
                    this.CreateMesh();
                    this.isInitialized = true;
					return;
				}

				if(this._imageUrl != null && this._imageUrl.ToLower().StartsWith("http://"))
				{
					//download it...
                    this.downloadThread = new Thread(new ThreadStart(this.DownloadImage));
                    this.downloadThread.Name = "ImageLayer.DownloadImage";

                    this.downloadThread.IsBackground = true;
                    this.downloadThread.Start();

					return;
				}

				// No image available
                this.Dispose();
                this.isOn = false;
				return;
			}
			catch
			{
			}
		}

		/// <summary>
		/// Downloads image from web
		/// </summary>
		protected void DownloadImage()
		{
			try
			{
				if(this._imagePath!=null)
					Directory.CreateDirectory(Path.GetDirectoryName(this._imagePath));

                // Offline check
                if (World.Settings.WorkOffline)
                    return;

				using(WebDownload downloadReq = new WebDownload(this._imageUrl))
				{
					downloadReq.ProgressCallback += new DownloadProgressHandler(this.UpdateDownloadProgress);
					string filePath = getFilePathFromUrl(this._imageUrl);
					
					if(this._imagePath==null)
					{
						// Download to RAM
						downloadReq.DownloadMemory();
                        this.texture = ImageHelper.LoadTexture(downloadReq.ContentStream);
					}
					else
					{
						downloadReq.DownloadFile(this._imagePath);
                        this.UpdateTexture(this._imagePath);
					}

                    this.CreateMesh();
                    this.isInitialized = true;
				}
			}
			catch(ThreadAbortException)
			{}
			catch(Exception caught)
			{
				if(!this.showedError)
				{
					string msg = string.Format("Image download of file\n\n{1}\n\nfor layer '{0}' failed:\n\n{2}", this.name, this._imageUrl, caught.Message );
					System.Windows.Forms.MessageBox.Show(msg, "Image download failed.", 
						System.Windows.Forms.MessageBoxButtons.OK, 
						System.Windows.Forms.MessageBoxIcon.Error );
                    this.showedError = true;
				}

				if(this._imagePath != null)
				{
					FileInfo imageFile = new FileInfo(this._imagePath);
					if(imageFile.Exists)
					{
                        this.UpdateTexture(this._imagePath);
                        this.CreateMesh();
                        this.isInitialized = true;
					}
				}
				else
				{
                    this.isOn = false;
				}
			}
		}

		bool showedError;

		/// <summary>
		/// Download progress callback 
		/// </summary>
		protected void UpdateDownloadProgress(int bytesRead, int bytesTotal)
		{
			if(bytesRead < bytesTotal) this.downloadPercent = (float)bytesRead / bytesTotal;
		}

		/// <summary>
		/// Update layer (called from worker thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
			try
			{
				if(!this.isInitialized)
				{
					this.Initialize(drawArgs);
					if(!this.isInitialized)
						return;
				}

				if(this.m_SurfaceImage == null && this.isInitialized && Math.Abs(this.verticalExaggeration - World.Settings.VerticalExaggeration)>0.01f)
				{
					// Vertical exaggeration changed - rebuild mesh
					this.verticalExaggeration = World.Settings.VerticalExaggeration;
					this.isInitialized = false;
                    this.CreateMesh();
					this.isInitialized = true;
				}

				if(this.cacheExpiration != TimeSpan.MaxValue && this.cacheExpiration.TotalMilliseconds > 0)
				{
					if(this.refreshTimer == null)
					{
                        this.refreshTimer = new System.Timers.Timer(this.cacheExpiration.TotalMilliseconds);
                        this.refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.refreshTimer_Elapsed);
                        this.refreshTimer.Start();
					}

					if(this.refreshTimer.Interval != this.cacheExpiration.TotalMilliseconds)
					{
                        this.refreshTimer.Interval = this.cacheExpiration.TotalMilliseconds;
					}
				}
				else if(this.refreshTimer != null && this.refreshTimer.Enabled) this.refreshTimer.Stop();
			}
			catch
			{
			}
		}

		/// <summary>
		/// Handle mouse click
		/// </summary>
		/// <returns>true if click was handled.</returns>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		public override byte Opacity
		{
			get
			{
				return this.m_opacity;
			}
			set
			{
                this.m_opacity = value;

			//	if(vertices==null)
			//		return;

				// Update mesh opacity
			//	int opacityColor = m_opacity << 24;
			//	for(int index = 0; index < vertices.Length; index++)
			//		vertices[index].Color = opacityColor;
			}
		}

		SurfaceImage m_SurfaceImage;

		/// <summary>
		/// Builds the image's mesh 
		/// </summary>
		protected virtual void CreateMesh()
		{
			int upperBound = this.meshPointCount - 1;
			float scaleFactor = (float)1/upperBound;
			double latrange = Math.Abs(this.maxLat - this.minLat);
			double lonrange;
			if(this.minLon < this.maxLon)
				lonrange = this.maxLon - this.minLon;
			else
				lonrange = 360.0f + this.maxLon - this.minLon;

			int opacityColor = System.Drawing.Color.FromArgb(this.m_opacity,0,0,0).ToArgb();
            this.vertices = new CustomVertex.PositionNormalTextured[this.meshPointCount * this.meshPointCount];
			for(int i = 0; i < this.meshPointCount; i++)
			{
				for(int j = 0; j < this.meshPointCount; j++)
				{	
					double height = 0;
					if(this._terrainAccessor != null)
						height = this.verticalExaggeration * this._terrainAccessor.GetElevationAt(
							(double) this.maxLat - scaleFactor * latrange * i,
							(double) this.minLon + scaleFactor * lonrange * j,
							(double)upperBound / latrange);

					Vector3 pos = MathEngine.SphericalToCartesian(this.maxLat - scaleFactor*latrange*i, this.minLon + scaleFactor*lonrange*j, this.layerRadius + height);

                    this.vertices[i* this.meshPointCount + j].X = pos.X;
                    this.vertices[i* this.meshPointCount + j].Y = pos.Y;
                    this.vertices[i* this.meshPointCount + j].Z = pos.Z;

                    this.vertices[i* this.meshPointCount + j].Tu = j*scaleFactor;
                    this.vertices[i* this.meshPointCount + j].Tv = i*scaleFactor;
				//	vertices[i*meshPointCount + j].Color = opacityColor;
				}
			}

            this.indices = new short[2 * upperBound * upperBound * 3];
			for(int i = 0; i < upperBound; i++)
			{
				for(int j = 0; j < upperBound; j++)
				{
                    this.indices[(2*3*i*upperBound) + 6*j] = (short)(i* this.meshPointCount + j);
                    this.indices[(2*3*i*upperBound) + 6*j + 1] = (short)((i+1)* this.meshPointCount + j);
                    this.indices[(2*3*i*upperBound) + 6*j + 2] = (short)(i* this.meshPointCount + j+1);

                    this.indices[(2*3*i*upperBound) + 6*j + 3] = (short)(i* this.meshPointCount + j+1);
                    this.indices[(2*3*i*upperBound) + 6*j + 4] = (short)((i+1)* this.meshPointCount + j);
                    this.indices[(2*3*i*upperBound) + 6*j + 5] = (short)((i+1)* this.meshPointCount + j+1);
				}
			}

            this.calculate_normals(ref this.vertices, this.indices);
		}

		private void calculate_normals(ref CustomVertex.PositionNormalTextured[] vertices, short[] indices)
		{
			System.Collections.ArrayList[] normal_buffer = new System.Collections.ArrayList[vertices.Length];
			for(int i = 0; i < vertices.Length; i++)
			{
				normal_buffer[i] = new System.Collections.ArrayList();
			}
			for(int i = 0; i < indices.Length; i += 3)
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

		//Blend m_sourceBlend = Blend.BlendFactor;
		//Blend m_destinationBlend = Blend.InvBlendFactor;
        static Effect grayscaleEffect;
		/// <summary>
		/// Draws the layer
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			if(this.downloadThread != null && this.downloadThread.IsAlive) this.RenderProgress(drawArgs);

			if(!this.isInitialized)
				return;

			try
			{
				
				if(this.texture == null || this.m_SurfaceImage != null)
					return;

				drawArgs.device.SetTexture(0, this.texture);

				if(this._disableZbuffer)
				{
					if(drawArgs.device.SetRenderState(RenderState.ZBufferEnable)
						drawArgs.device.SetRenderState(RenderState.ZBufferEnable = false;
				}
				else
				{
					if(!drawArgs.device.SetRenderState(RenderState.ZBufferEnable)
						drawArgs.device.SetRenderState(RenderState.ZBufferEnable = true;
				}

				drawArgs.device.SetRenderState(RenderState.ZBufferEnable = true;
				drawArgs.device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);

			/*	if (m_opacity < 255 && device.Capabilities.DestinationBlendCaps.SupportsBlendFactor)
				{
					// Blend
					device.SetRenderState(RenderState.AlphaBlendEnable = true;
					device.SetRenderState(RenderState.SourceBlend = m_sourceBlend;
					device.SetRenderState(RenderState.DestinationBlend = m_destinationBlend;
					// Set Red, Green and Blue = opacity
					device.SetRenderState(RenderState.BlendFactorColor = (m_opacity << 16) | (m_opacity << 8) | m_opacity;
				}*/
			//	else if (EnableColorKeying && device.Capabilities.TextureCaps.SupportsAlpha)
			//	{
			//		device.SetRenderState(RenderState.AlphaBlendEnable = true;
			//		device.SetRenderState(RenderState.SourceBlend = Blend.SourceAlpha;
			//		device.SetRenderState(RenderState.DestinationBlend = Blend.InvSourceAlpha;
			//	}

                drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
                        (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                        );
                this.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                if (!this.RenderGrayscale || (this.device.Capabilities.PixelShaderVersion.Major < 1))
                {
                    if (World.Settings.EnableSunShading)
                    {
                        Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                        Vector3 sunVector = new Vector3(
                            (float)sunPosition.X,
                            (float)sunPosition.Y,
                            (float)sunPosition.Z);

                        this.device.SetRenderState(RenderState.Lighting = true;
                        Material material = new Material();
                        material.Diffuse = System.Drawing.Color.White;
                        material.Ambient = System.Drawing.Color.White;

                        this.device.Material = material;
                        this.device.SetRenderState(RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                        this.device.SetRenderState(RenderState.NormalizeNormals = true;
                        this.device.SetRenderState(RenderState.AlphaBlendEnable = true;

                        this.device.Lights[0].Enabled = true;
                        this.device.Lights[0].Type = LightType.Directional;
                        this.device.Lights[0].Diffuse = System.Drawing.Color.White;
                        this.device.Lights[0].Direction = sunVector;

                        this.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.Modulate;
                        this.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse;
                        this.device.SetTextureStageState(0, TextureStage.ColorArg2,TextureArgument.TextureColor;
                    }
                    else
                    {
                        this.device.SetRenderState(RenderState.Lighting = false;
                        this.device.SetRenderState(RenderState.Ambient = World.Settings.StandardAmbientColor;

                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.SelectArg1;
                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.TextureColor;
                    }

                    this.device.SetRenderState(RenderState.TextureFactor = System.Drawing.Color.FromArgb(this.m_opacity, 255, 255, 255).ToArgb();
                    this.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.Modulate;
                    this.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.TextureColor;
                    this.device.SetTextureStageState(0, TextureStage.AlphaArg2,TextureArgument.TFactor;

                    drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                    drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, this.vertices.Length, this.indices.Length / 3, this.indices, true, this.vertices);

                    
                }
                else
                {
                    if (grayscaleEffect == null)
                    {
                        this.device.DeviceReset += new EventHandler(this.device_DeviceReset);
                        this.device_DeviceReset(this.device, null);
                    }

                    grayscaleEffect.Technique = "RenderGrayscaleBrightness";
                    grayscaleEffect.SetValue("WorldViewProj", Matrix.Multiply(this.device.Transform.World, Matrix.Multiply(this.device.Transform.View, this.device.Transform.Projection)));
                    grayscaleEffect.SetValue("Tex0", this.texture);
                    grayscaleEffect.SetValue("Brightness", this.GrayscaleBrightness);
                    float opacity = (float) this.m_opacity / 255.0f;
                    grayscaleEffect.SetValue("Opacity", opacity);

                    int numPasses = grayscaleEffect.Begin(0);
                    for (int i = 0; i < numPasses; i++)
                    {
                        grayscaleEffect.BeginPass(i);

                        drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, this.vertices.Length, this.indices.Length / 3, this.indices, true, this.vertices);

                        grayscaleEffect.EndPass();
                    }

                    grayscaleEffect.End();
                }

                drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;
				
			}
			finally
			{
				if (this.m_opacity < 255)
				{
					// Restore alpha blend state
                    this.device.SetRenderState(RenderState.SourceBlend = Blend.SourceAlpha;
                    this.device.SetRenderState(RenderState.DestinationBlend = Blend.InvSourceAlpha;
				}

				if(this._disableZbuffer)
					drawArgs.device.SetRenderState(RenderState.ZBufferEnable = true;
			}
		}

        void device_DeviceReset(object sender, EventArgs e)
        {
            Device device = (Device)sender;

            string outerrors = "";
            grayscaleEffect = Effect.FromFile(
                device,
                "shaders\\grayscale.fx",
                null,
                null,
                ShaderFlags.None,
                null,
                out outerrors);

            if (outerrors != null && outerrors.Length > 0)
                Log.Write(Log.Levels.Error, outerrors);
        }

		protected virtual void RenderProgress(DrawArgs drawArgs)
		{

			drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
				(float)-drawArgs.WorldCamera.ReferenceCenter.X,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Y,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Z
			);

            this.device.SetRenderState(RenderState.ZBufferEnable = false;
			double centerLat = 0.5 * (this.maxLat + this.minLat);
			double centerLon = 0.5 * (this.maxLon + this.minLon);

			Vector3 v = MathEngine.SphericalToCartesian(centerLat, centerLon, this.layerRadius);

			if(drawArgs.WorldCamera.ViewFrustum.ContainsPoint(v) && 
				MathEngine.SphericalDistanceDegrees(centerLat, centerLon, drawArgs.WorldCamera.Latitude.Degrees, drawArgs.WorldCamera.Longitude.Degrees) < 2 * drawArgs.WorldCamera.ViewRange.Degrees
				)
			{
				v.Project(drawArgs.device.Viewport, drawArgs.device.Transform.Projection, drawArgs.device.Transform.View, drawArgs.device.Transform.World);

				MenuUtils.DrawBox((int)v.X, (int)v.Y, 200, 40, 0.0f, this.progressBarBackColor, drawArgs.device);
				Vector2[] boxOutline = new Vector2[5];
				boxOutline[0].X = (int)v.X;
				boxOutline[0].Y = (int)v.Y;

				boxOutline[1].X = (int)v.X + 200;
				boxOutline[1].Y = (int)v.Y;
					
				boxOutline[2].X = (int)v.X + 200;
				boxOutline[2].Y = (int)v.Y + 40;

				boxOutline[3].X = (int)v.X;
				boxOutline[3].Y = (int)v.Y + 40;

				boxOutline[4].X = (int)v.X;
				boxOutline[4].Y = (int)v.Y;

				MenuUtils.DrawLine(boxOutline, this.progressBarOutlineColor, drawArgs.device);

				drawArgs.defaultDrawingFont.DrawText(null,
					"Downloading Remote Image...",
					new System.Drawing.Rectangle((int)v.X + 5, (int)v.Y + 5, 200, 50),
					DrawTextFormat.NoClip, this.textColor);

                this.DrawProgressBar(drawArgs, v.X + 100, v.Y + 30, 180, 10, World.Settings.downloadProgressColor);
			}

            this.device.SetRenderState(RenderState.ZBufferEnable = true;
			drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;
		}

		/// <summary>
		/// Displays a progress bar
		/// </summary>
		void DrawProgressBar(DrawArgs drawArgs, float x, float y, float width, float height, int color)
		{
			float halfWidth = width/2;
			float halfHeight = height/2;
			progressBarOutline[0].X = x - halfWidth;
			progressBarOutline[0].Y = y - halfHeight;
			progressBarOutline[0].Z = 0.5f;
			progressBarOutline[0].Color = color;

			progressBarOutline[1].X = x + halfWidth;
			progressBarOutline[1].Y = y - halfHeight;
			progressBarOutline[1].Z = 0.5f;
			progressBarOutline[1].Color = color;
					
			progressBarOutline[2].X = x + halfWidth;
			progressBarOutline[2].Y = y + halfHeight;
			progressBarOutline[2].Z = 0.5f;
			progressBarOutline[2].Color = color;
					
			progressBarOutline[3].X = x - halfWidth;
			progressBarOutline[3].Y = y + halfHeight;
			progressBarOutline[3].Z = 0.5f;
			progressBarOutline[3].Color = color;

			progressBarOutline[4].X = x - halfWidth;
			progressBarOutline[4].Y = y - halfHeight;
			progressBarOutline[4].Z = 0.5f;
			progressBarOutline[4].Color = color;

			drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
			drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.Disable;
			drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, progressBarOutline);
				
			int barlength = (int)(this.downloadPercent * 2 * halfWidth);

			progressBar[0].X = x - halfWidth;
			progressBar[0].Y = y - halfHeight;
			progressBar[0].Z = 0.5f;
			progressBar[0].Color = color;

			progressBar[1].X = x - halfWidth;
			progressBar[1].Y = y + halfHeight;
			progressBar[1].Z = 0.5f;
			progressBar[1].Color = color;
					
			progressBar[2].X = x - halfWidth + barlength;
			progressBar[2].Y = y - halfHeight;
			progressBar[2].Z = 0.5f;
			progressBar[2].Color = color;
					
			progressBar[3].X = x - halfWidth + barlength;
			progressBar[3].Y = y + halfHeight;
			progressBar[3].Z = 0.5f;
			progressBar[3].Color = color;

			drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, progressBar);
		}

		private static string getFilePathFromUrl(string url)
		{
			if(url.ToLower().StartsWith("http://"))
			{
				url = url.Substring(7);
			}

			// ShockFire: Remove any illegal characters from the path
			foreach (char invalidChar in Path.GetInvalidPathChars())
			{
				url = url.Replace(invalidChar.ToString(), "");
			}

			// ShockFire: Also remove other illegal chars that are not included in InvalidPathChars for no good reason
			url = url.Replace(":", "").Replace("*", "").Replace("?", "");

			return Path.Combine(
				Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), Path.Combine("Cache\\ImageUrls", url));
		}


		/// <summary>
		/// Switch to a different image
		/// </summary>
		public void UpdateTexture(string fileName)
		{
			try
			{
				if(this.device != null)
				{
					Texture oldTexture = this.texture;

					this._imagePath = fileName;
					Texture newTexture = ImageHelper.LoadTexture(fileName);
					this.texture = newTexture;

					if(oldTexture != null)
						oldTexture.Dispose();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Switch to a different image
		/// </summary>
		public void UpdateTexture(Stream textureStream)
		{
            this.UpdateTexture(textureStream, 0);
		}

		/// <summary>
		/// Switch to a different image
		/// </summary>
		public void UpdateTexture(Stream textureStream, int transparentColor)
		{
			try
			{
				if(this.device != null)
				{
					Texture oldTexture = this.texture;

					Texture newTexture = ImageHelper.LoadTexture(textureStream);
					this.texture = newTexture;

					if(oldTexture != null)
						oldTexture.Dispose();
				}
			}
			catch
			{
			}
		}


		/// <summary>
		/// Cleanup when layer is disabled
		/// </summary>
		public override void Dispose()
		{
			this.isInitialized = false;

			if(this.downloadThread != null)
			{
				if(this.downloadThread.IsAlive)
				{
                    this.downloadThread.Abort();
				}

                this.downloadThread = null;
			}

			if(this.m_SurfaceImage != null)
			{
                this.m_ParentWorld.WorldSurfaceRenderer.RemoveSurfaceImage(this.m_SurfaceImage.ImageFilePath);
                this.m_SurfaceImage = null;
			}
			if (this.texture!=null)
			{
				this.texture.Dispose();
				this.texture = null;
			}

			if(this.legendControl != null)
			{
                this.legendControl.Dispose();
                this.legendControl = null;
			}

			if(this.refreshTimer != null && this.refreshTimer.Enabled)
			{
                this.refreshTimer.Stop();
                this.refreshTimer = null;
			}
		}

		/// <summary>
		/// Change opacity
		/// </summary>
		/// <param name="percent">0=transparent, 1=opaque</param>
		public void UpdateOpacity(float percent)
		{
			Debug.Assert(percent <= 1);
			Debug.Assert(percent >= 0);

			this.m_opacity = (byte)(255 * percent);
			
			this.isInitialized = false;
            this.CreateMesh();
			this.isInitialized = true;
		}

		/// <summary>
		/// Change radius
		/// </summary>
		/// <param name="layerRadius">Sphere radius (meters)</param>
		public void UpdateLayerRadius(float layerRadius)
		{
			this.layerRadius = layerRadius;

			this.isInitialized = false;
            this.CreateMesh();
			this.isInitialized = true;
		}

		public override void BuildContextMenu(System.Windows.Forms.ContextMenu menu)
		{
			base.BuildContextMenu(menu);

			if(this.m_legendImagePath == null || this.m_legendImagePath.Length <= 0)
				return;

			// Add legend menu item
			System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem("&Show Legend", 
				new EventHandler(this.OnLegendClick) );
			menu.MenuItems.Add(0, mi);
		}

		/// <summary>
		/// Called when user chooses to display legendControl
		/// </summary>
		protected virtual void OnLegendClick( object sender, EventArgs e )
		{
			if(this.legendControl == null) this.legendControl = new Colorbar(null);
            this.legendControl.LoadImage(this.m_legendImagePath );
		}

		bool abortedFirstRefresh;

		private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(!this.abortedFirstRefresh)
			{
                this.abortedFirstRefresh = true;
				return;
			}

			if(this._imageUrl == null && this._imagePath != null)
			{
                this.UpdateTexture(this._imagePath);
			}
			else if(this._imageUrl != null && this._imageUrl.ToLower().StartsWith("http://"))
			{
                this._imagePath = getFilePathFromUrl(this._imageUrl);
                this.DownloadImage();
			}

		}
	}
}
