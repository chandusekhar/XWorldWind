using System;
using System.IO;
using WorldWind.Configuration;
using WorldWind.Terrain;
using Utility;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;

namespace WorldWind
{
	/// <summary>
	///
	/// </summary>
	public class World : RenderableObject
	{
		/// <summary>
		/// Persisted user adjustable settings.
		/// </summary>
		public static WorldSettings Settings = new WorldSettings();

		#region Private Members

		double equatorialRadius;
		// TODO: Add ellipsoid parameters to world.
		const double flattening = 6378.135;
		const double SemiMajorAxis = 6378137.0;
		const double SemiMinorAxis = 6356752.31425;
		TerrainAccessor _terrainAccessor;
		RenderableObjectList _renderableObjects;
		private System.Collections.IList onScreenMessages;
		private DateTime lastElevationUpdate = DateTime.Now;
		WorldSurfaceRenderer m_WorldSurfaceRenderer = null;

		public System.Collections.IList OnScreenMessages
		{
			get 
			{
				return this.onScreenMessages;
			}
			set 
			{
				this.onScreenMessages = value;
			}
		}

        // this is a stub for fixing looping through every RO multiple times.
        private LinkedList<RenderableObject> iconList = new LinkedList<RenderableObject>();

		#endregion

		#region Properties
		public WorldSurfaceRenderer WorldSurfaceRenderer
		{
			get
			{
				return this.m_WorldSurfaceRenderer;
			}
		}
/*		public string DataDirectory
		{
			get
			{
				return this._dataDirectory;
			}
			set
			{
				this._dataDirectory = value;
			}
		} */

		/// <summary>
		/// Whether this world is planet Earth.
		/// </summary>
		public bool IsEarth
		{
			get
			{
				// HACK
				return this.Name=="Earth";
			}
		}
		#endregion

		ProjectedVectorRenderer m_projectedVectorRenderer;
		public ProjectedVectorRenderer ProjectedVectorRenderer
		{
			get{ return this.m_projectedVectorRenderer; }
		}
		
		static World()
		{
			// Don't load settings here - use LoadSettings explicitly
			//LoadSettings();
		}


		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.World"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="position"></param>
		/// <param name="orientation"></param>
		/// <param name="equatorialRadius"></param>
		/// <param name="cacheDirectory"></param>
		/// <param name="terrainAccessor"></param>
		public World(string name, Vector3 position, Quaternion orientation, double equatorialRadius,
			string cacheDirectory,
			TerrainAccessor terrainAccessor)
			: base(name, position, orientation)
		{
			this.equatorialRadius = equatorialRadius;

			this._terrainAccessor = terrainAccessor;
			this._renderableObjects = new RenderableObjectList(this.Name);
			this.MetaData.Add("CacheDirectory", cacheDirectory);

		//	this.m_WorldSurfaceRenderer = new WorldSurfaceRenderer(32, 0, this);
			this.m_projectedVectorRenderer = new ProjectedVectorRenderer(this);

            this.m_outerSphere = new AtmosphericScatteringSphere();
			AtmosphericScatteringSphere.m_fInnerRadius = (float)equatorialRadius;
			AtmosphericScatteringSphere.m_fOuterRadius = (float)equatorialRadius * 1.025f;

            this.m_outerSphere.Init((float)equatorialRadius * 1.025f, 75, 75);
		}

		public AtmosphericScatteringSphere m_outerSphere;
		public void SetLayerOpacity(string category, string name, float opacity)
		{
			this.setLayerOpacity(this._renderableObjects, category, name, opacity);
		}

		private static string getRenderablePathString(RenderableObject renderable)
		{
			if(renderable.ParentList == null)
			{
				return renderable.Name;
			}
			else
			{
				return getRenderablePathString(renderable.ParentList) + Path.DirectorySeparatorChar + renderable.Name;
			}
		}

		private void setLayerOpacity(RenderableObject ro, string category, string name, float opacity)
		{
			foreach(string key in ro.MetaData.Keys)
			{
				if (String.Compare(key, category, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
				{
					if(ro.MetaData[key].GetType() == typeof(String))
					{
						string curValue = ro.MetaData[key] as string;
						if (String.Compare(curValue, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
						{
							ro.Opacity = (byte)(255 * opacity);
						}
					}
					break;
				}
			}

			RenderableObjectList rol = ro as RenderableObjectList;
			if (rol != null)
			{
				foreach (RenderableObject childRo in rol.ChildObjects) this.setLayerOpacity(childRo, category, name, opacity);
			}
		}

		/// <summary>
		/// Deserializes settings from default location
		/// </summary>
		public static void LoadSettings()
		{
			try
			{
				Settings = (WorldSettings) SettingsBase.Load(Settings);
			}
			catch(Exception caught)
			{
				Log.Write(caught);
			}
		}

		/// <summary>
		/// Deserializes settings from specified location
		/// </summary>
		public static void LoadSettings(string directory)
		{
			try
			{
				Settings = (WorldSettings) SettingsBase.LoadFromPath(Settings, directory);
			}
			catch(Exception caught)
			{
				Log.Write(caught);
			}
		}

		public TerrainAccessor TerrainAccessor
		{
			get
			{
				return this._terrainAccessor;
			}
			set
			{
				this._terrainAccessor = value;
			}
		}

		public double EquatorialRadius
		{
			get
			{
				return this.equatorialRadius;
			}
		}

		public RenderableObjectList RenderableObjects
		{
			get
			{
				return this._renderableObjects;
			}
			set
			{
				this._renderableObjects = value;
			}
		}

		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				if (this.isInitialized)
					return;

				this.RenderableObjects.Initialize(drawArgs);
			}
			catch (Exception caught)
			{
				Log.DebugWrite(caught);
			}
			finally
			{
				this.isInitialized = true;
			}
		}

		private void DrawAxis(DrawArgs drawArgs)
		{
			CustomVertex.PositionColored[] axis = new CustomVertex.PositionColored[2];
			Vector3 topV = MathEngine.SphericalToCartesian(90, 0, this.EquatorialRadius + 0.15f * this.EquatorialRadius);
			axis[0].X = topV.X;
			axis[0].Y = topV.Y;
			axis[0].Z = topV.Z;

			axis[0].Color = System.Drawing.Color.Pink.ToArgb();

			Vector3 botV = MathEngine.SphericalToCartesian(-90, 0, this.EquatorialRadius + 0.15f * this.EquatorialRadius);
			axis[1].X = botV.X;
			axis[1].Y = botV.Y;
			axis[1].Z = botV.Z;
			axis[1].Color = System.Drawing.Color.Pink.ToArgb();

			drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
			drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
			drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
				(float)-drawArgs.WorldCamera.ReferenceCenter.X,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Y,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Z
				);

			drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axis);
			drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;

		}

		public override void Update(DrawArgs drawArgs)
		{
			if (!this.isInitialized)
			{
				this.Initialize(drawArgs);
			}

			if (this.RenderableObjects != null)
			{
				this.RenderableObjects.Update(drawArgs);
			}
			if (this.m_WorldSurfaceRenderer != null)
			{
				this.m_WorldSurfaceRenderer.Update(drawArgs);
			}

			if (this.m_projectedVectorRenderer != null)
			{
				this.m_projectedVectorRenderer.Update(drawArgs);
			}
            // Update camera terrain elevation
            // drawArgs.WorldCamera.UpdateTerrainElevation(this.TerrainAccessor); // Moved to WorldWindow.Render()

			if (Settings.EnableAtmosphericScattering && this.m_outerSphere != null) this.m_outerSphere.Update(drawArgs);
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return this._renderableObjects.PerformSelectionAction(drawArgs);
		}

		private void RenderSun(DrawArgs drawArgs)
		{
			Point3d sunPosition = -SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);

			Point3d sunSpherical = MathEngine.CartesianToSphericalD(sunPosition.X, sunPosition.Y, sunPosition.Z);
			sunPosition = MathEngine.SphericalToCartesianD(
				Angle.FromRadians(sunSpherical.Y),
				Angle.FromRadians(sunSpherical.Z),
				150000000000);

			Vector3 sunVector = new Vector3((float)sunPosition.X, (float)sunPosition.Y, (float)sunPosition.Z);

			Frustum viewFrustum = new Frustum();

			float aspectRatio = (float)drawArgs.WorldCamera.Viewport.Width / drawArgs.WorldCamera.Viewport.Height;
			Matrix projectionMatrix = Matrix.PerspectiveFovRH((float)drawArgs.WorldCamera.Fov.Radians, aspectRatio, 1000.0f, 300000000000);

			viewFrustum.Update(
				Matrix.Multiply(drawArgs.WorldCamera.AbsoluteWorldMatrix,
				Matrix.Multiply(drawArgs.WorldCamera.AbsoluteViewMatrix,
					projectionMatrix)));

			if (!viewFrustum.ContainsPoint(sunVector))
				return;

			Vector3 translationVector = new Vector3(
				(float)(sunPosition.X - drawArgs.WorldCamera.ReferenceCenter.X),
				(float)(sunPosition.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
				(float)(sunPosition.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

			Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

			if (this.m_sunTexture == null)
			{
                this.m_sunTexture = ImageHelper.LoadTexture(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Data\\sun.dds");
                this.m_sunSurfaceDescription = this.m_sunTexture.GetLevelDescription(0);
			}

			if (this.m_sprite == null)
			{
                this.m_sprite = new Sprite(drawArgs.device);
			}

            this.m_sprite.Begin(SpriteFlags.AlphaBlend);

			// Render icon
			float xscale = (float) this.m_sunWidth / this.m_sunSurfaceDescription.Width;
			float yscale = (float) this.m_sunHeight / this.m_sunSurfaceDescription.Height;
            this.m_sprite.Transform = Matrix.Scaling(xscale, yscale, 0);

            this.m_sprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
            this.m_sprite.Draw(this.m_sunTexture,
				new Vector3(this.m_sunSurfaceDescription.Width >> 1, this.m_sunSurfaceDescription.Height >> 1, 0),
				Vector3.Zero,
				System.Drawing.Color.FromArgb(253, 253, 200).ToArgb());

			// Reset transform to prepare for text rendering later
            this.m_sprite.Transform = Matrix.Identity;
            this.m_sprite.End();
		}

		int m_sunWidth = 72;
		int m_sunHeight = 72;

		Sprite m_sprite = null;
		Texture m_sunTexture = null;
		SurfaceDescription m_sunSurfaceDescription;

		public override void Render(DrawArgs drawArgs)
		{
            using(new DirectXProfilerEvent("World::Render"))
            {
			try
			{
				if (this.m_WorldSurfaceRenderer != null && Settings.UseWorldSurfaceRenderer)
				{
                    this.m_WorldSurfaceRenderer.RenderSurfaceImages(drawArgs);
				}

				//  Old method -- problems with RenderPriority sorting
				//	RenderableObjects.Render(drawArgs);

                // allow fog and other renderstate setup changers to get in front of everything else
                this.Render(this.RenderableObjects, (RenderPriority)(-1), drawArgs);

                this.RenderStars(drawArgs, this.RenderableObjects);

                if (drawArgs.CurrentWorld.IsEarth && Settings.EnableAtmosphericScattering)
                {
                    // Render atmospheric scattering
                    bool origFog = drawArgs.device.GetRenderState(RenderState.FogEnable);
                    drawArgs.device.SetRenderState(RenderState.FogEnable, false);
                    float aspectRatio = (float)drawArgs.WorldCamera.Viewport.Width / drawArgs.WorldCamera.Viewport.Height;
                    float zNear = (float)drawArgs.WorldCamera.Altitude * 0.1f;
                    double distToCenterOfPlanet = (drawArgs.WorldCamera.Altitude + this.equatorialRadius);
                    double tangentalDistance = Math.Sqrt(distToCenterOfPlanet * distToCenterOfPlanet - this.equatorialRadius * this.equatorialRadius);
                    double amosphereThickness = Math.Sqrt(this.m_outerSphere.m_radius * this.m_outerSphere.m_radius + this.equatorialRadius * this.equatorialRadius);
                    Matrix proj = drawArgs.device.SetTransform(TransformState.Projection;
                    drawArgs.device.SetTransform(TransformState.Projection = Matrix.PerspectiveFovRH((float)drawArgs.WorldCamera.Fov.Radians, aspectRatio, zNear, (float)(tangentalDistance + amosphereThickness));
                    drawArgs.device.SetRenderState(RenderState.ZEnable,false);
                    drawArgs.device.SetRenderState(RenderState.CullMode,Cull.Counterclockwise);
                    this.m_outerSphere.Render(drawArgs);
                    drawArgs.device.SetRenderState(RenderState.CullMode,Cull.Clockwise);
                    drawArgs.device.SetRenderState(RenderState.ZEnable,true);
                    drawArgs.device.SetTransform(TransformState.Projection = proj;
                    drawArgs.device.SetRenderState(RenderState.FogEnable,origFog);
                }

				if (Settings.EnableSunShading) this.RenderSun(drawArgs);

                DirectXProfiler.BeginEvent("render surface images");
                //render SurfaceImages
                this.Render(this.RenderableObjects, RenderPriority.TerrainMappedImages, drawArgs);
                DirectXProfiler.EndEvent();

				if (this.m_projectedVectorRenderer != null) this.m_projectedVectorRenderer.Render(drawArgs);

                DirectXProfiler.BeginEvent("render atmospheric images");
                //render AtmosphericImages
                this.Render(this.RenderableObjects, RenderPriority.AtmosphericImages, drawArgs);
                DirectXProfiler.EndEvent();

                DirectXProfiler.BeginEvent("render line paths");
                //render LinePaths
                this.Render(this.RenderableObjects, RenderPriority.LinePaths, drawArgs);
                DirectXProfiler.EndEvent();

                DirectXProfiler.BeginEvent("render placenames");
                //render Placenames
                if (Settings.ShowPlacenames) this.Render(this.RenderableObjects, RenderPriority.Placenames, drawArgs);
                DirectXProfiler.EndEvent();

                DirectXProfiler.BeginEvent("render icons");
				//render Icons
                this.Render(this.RenderableObjects, RenderPriority.Icons, drawArgs);
                DirectXProfiler.EndEvent();

                DirectXProfiler.BeginEvent("render custom");
                //render Custom
                this.Render(this.RenderableObjects, RenderPriority.Custom, drawArgs);
                DirectXProfiler.EndEvent();

				if (Settings.showPlanetAxis)
					this.DrawAxis(drawArgs);
			}
			catch (Exception ex)
			{
				Log.Write(ex);
			}
            }
		}

		private void RenderStars(DrawArgs drawArgs, RenderableObject renderable)
		{
			if (renderable is RenderableObjectList)
			{
				RenderableObjectList rol = (RenderableObjectList)renderable;
				for (int i = 0; i < rol.ChildObjects.Count; i++)
				{
                    this.RenderStars(drawArgs, (RenderableObject)rol.ChildObjects[i]);
				}
			}
			else if (renderable.Name != null && renderable.Name.Equals("Starfield"))
			{
				try
				{
					renderable.Render(drawArgs);
				}
				catch (Exception ex)
				{
					Log.Write(ex);
				}
			}
		}

		private void Render(RenderableObject renderable, RenderPriority priority, DrawArgs drawArgs)
		{
			if (!renderable.IsOn || (renderable.Name != null && renderable.Name.Equals("Starfield")))
				return;

			try
			{
                // If we're a ROL then render children if they are the right priority
                if (renderable is RenderableObjectList)
                {
                    ((RenderableObjectList)renderable).RenderChildren(drawArgs, priority);
                }
                // hack at the moment
                else if (priority == RenderPriority.TerrainMappedImages)
                {
                    if (renderable.RenderPriority == RenderPriority.SurfaceImages || renderable.RenderPriority == RenderPriority.TerrainMappedImages)
                    {
                        renderable.Render(drawArgs);
                    }
                }
                else if (renderable.RenderPriority == priority)
                {
                    renderable.Render(drawArgs);
                }
			}
			catch (Exception ex)
			{
				Log.Write(ex);
			}
		}

		private void saveRenderableState(RenderableObject ro)
		{
			string path = getRenderablePathString(ro);
			bool found = false;
			for (int i = 0; i < Settings.loadedLayers.Count; i++)
			{
				string s = (string)Settings.loadedLayers[i];
				if (s.Equals(path))
				{
					if (!ro.IsOn)
					{
						Settings.loadedLayers.RemoveAt(i);
						break;

					}
					else
					{
						found = true;
					}
				}
			}

			if (!found && ro.IsOn)
			{
				Settings.loadedLayers.Add(path);
			}
		}

		private void saveRenderableStates(RenderableObjectList rol)
		{
            this.saveRenderableState(rol);

			foreach (RenderableObject ro in rol.ChildObjects)
			{
				if (ro is RenderableObjectList)
				{
					RenderableObjectList childRol = (RenderableObjectList)ro;
                    this.saveRenderableStates(childRol);
				}
				else
				{
                    this.saveRenderableState(ro);
				}
			}
		}

		public override void Dispose()
		{
            this.saveRenderableStates(this.RenderableObjects);

			if (this.RenderableObjects != null)
			{
				this.RenderableObjects.Dispose();
				this.RenderableObjects = null;
			}

			if (this.m_WorldSurfaceRenderer != null)
			{
                this.m_WorldSurfaceRenderer.Dispose();
			}

			if (this.m_outerSphere != null)
			{
                this.m_outerSphere.Dispose();
			}
		}

		/// <summary>
		/// Computes the great circle distance between two pairs of lat/longs.
		/// TODO: Compute distance using ellipsoid.
		/// </summary>
		public static Angle ApproxAngularDistance(Angle latA, Angle lonA, Angle latB, Angle lonB)
		{
			Angle dlon = lonB - lonA;
			Angle dlat = latB - latA;
			double k = Math.Sin(dlat.Radians * 0.5);
			double l = Math.Sin(dlon.Radians * 0.5);
			double a = k * k + Math.Cos(latA.Radians) * Math.Cos(latB.Radians) * l * l;
			double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
			return Angle.FromRadians(c);
		}

		/// <summary>
		/// Computes the distance between two pairs of lat/longs in meters.
		/// </summary>
		public double ApproxDistance(Angle latA, Angle lonA, Angle latB, Angle lonB)
		{
			double distance = this.equatorialRadius * ApproxAngularDistance(latA, lonA, latB, lonB).Radians;
			return distance;
		}

		/// <summary>
		/// Intermediate points on a great circle
		/// In previous sections we have found intermediate points on a great circle given either
		/// the crossing latitude or longitude. Here we find points (lat,lon) a given fraction of the
		/// distance (d) between them. Suppose the starting point is (lat1,lon1) and the final point
		/// (lat2,lon2) and we want the point a fraction f along the great circle route. f=0 is
		/// point 1. f=1 is point 2. The two points cannot be antipodal ( i.e. lat1+lat2=0 and
		/// abs(lon1-lon2)=pi) because then the route is undefined.
		/// </summary>
		/// <param name="f">Fraction of the distance for intermediate point (0..1)</param>
		public static void IntermediateGCPoint(float f, Angle lat1, Angle lon1, Angle lat2, Angle lon2, Angle d,
			out Angle lat, out Angle lon)
		{
			double sind = Math.Sin(d.Radians);
			double cosLat1 = Math.Cos(lat1.Radians);
			double cosLat2 = Math.Cos(lat2.Radians);
			double A = Math.Sin((1 - f) * d.Radians) / sind;
			double B = Math.Sin(f * d.Radians) / sind;
			double x = A * cosLat1 * Math.Cos(lon1.Radians) + B * cosLat2 * Math.Cos(lon2.Radians);
			double y = A * cosLat1 * Math.Sin(lon1.Radians) + B * cosLat2 * Math.Sin(lon2.Radians);
			double z = A * Math.Sin(lat1.Radians) + B * Math.Sin(lat2.Radians);
			lat = Angle.FromRadians(Math.Atan2(z, Math.Sqrt(x * x + y * y)));
			lon = Angle.FromRadians(Math.Atan2(y, x));
		}

		/// <summary>
		/// Intermediate points on a great circle
		/// In previous sections we have found intermediate points on a great circle given either
		/// the crossing latitude or longitude. Here we find points (lat,lon) a given fraction of the
		/// distance (d) between them. Suppose the starting point is (lat1,lon1) and the final point
		/// (lat2,lon2) and we want the point a fraction f along the great circle route. f=0 is
		/// point 1. f=1 is point 2. The two points cannot be antipodal ( i.e. lat1+lat2=0 and
		/// abs(lon1-lon2)=pi) because then the route is undefined.
		/// </summary>
		/// <param name="f">Fraction of the distance for intermediate point (0..1)</param>
		public Vector3 IntermediateGCPoint(float f, Angle lat1, Angle lon1, Angle lat2, Angle lon2, Angle d)
		{
			double sind = Math.Sin(d.Radians);
			double cosLat1 = Math.Cos(lat1.Radians);
			double cosLat2 = Math.Cos(lat2.Radians);
			double A = Math.Sin((1 - f) * d.Radians) / sind;
			double B = Math.Sin(f * d.Radians) / sind;
			double x = A * cosLat1 * Math.Cos(lon1.Radians) + B * cosLat2 * Math.Cos(lon2.Radians);
			double y = A * cosLat1 * Math.Sin(lon1.Radians) + B * cosLat2 * Math.Sin(lon2.Radians);
			double z = A * Math.Sin(lat1.Radians) + B * Math.Sin(lat2.Radians);
			Angle lat = Angle.FromRadians(Math.Atan2(z, Math.Sqrt(x * x + y * y)));
			Angle lon = Angle.FromRadians(Math.Atan2(y, x));

			Vector3 v = MathEngine.SphericalToCartesian(lat, lon, this.equatorialRadius);
			return v;
		}
	}

	public class AtmosphericScatteringSphere
	{
		public float m_radius;
		protected int m_numberSlices;
		protected int m_numberSections;

		public static float m_fInnerRadius;
		public static float m_fOuterRadius;
		public static int TilesHigh = 4;
		public static int TilesWide = 8;

		public void Init(float radius, int slices, int sections)
		{
			try
			{
                this.m_radius = radius;
                this.m_numberSlices = slices;
                this.m_numberSections = sections;

				Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
				Vector3 sunVector = new Vector3(
					(float)-sunPosition.X,
					(float)-sunPosition.Y,
					(float)-sunPosition.Z);

                this.m_vLight = sunVector * 100000000f;
                this.m_vLightDirection = new Vector3(this.m_vLight.X / this.m_vLight.Length(), this.m_vLight.Y / this.m_vLight.Length(), this.m_vLight.Z / this.m_vLight.Length()
					);

                this.m_fScale = 1 / (m_fOuterRadius - m_fInnerRadius);

                this.m_meshList.Clear();

				double latRange = 180.0 / (double)TilesHigh;
				double lonRange = 360.0 / (double)TilesWide;

				int meshDensity = this.m_numberSlices / TilesHigh;

				for (int y = 0; y < TilesHigh; y++)
				{
					for (int x = 0; x < TilesWide; x++)
					{
						MeshSubset mesh = new MeshSubset();
						double north = y * latRange + latRange - 90;
						double south = y * latRange - 90;

						double west = x * lonRange - 180;
						double east = x * lonRange + lonRange - 180;

						mesh.Vertices = this.CreateMesh(south, north, west, east, meshDensity);
						mesh.HigherResolutionVertices = this.CreateMesh(south, north, west, east, 2 * meshDensity);
						mesh.BoundingBox = new BoundingBox((float)south, (float)north, (float)west, (float)east, (float)radius, (float)radius);
                        this.m_meshList.Add(mesh);
					}
				}

                this.m_indices = this.computeIndices(meshDensity);
                this.m_indicesHighResolution = this.computeIndices(2 * meshDensity);

                this.m_nSamples = 4;		// Number of sample rays to use in integral equation
                this.m_Kr = 0.0025f;		// Rayleigh scattering constant
                this.m_Kr4PI = this.m_Kr * 4.0f * (float)Math.PI;
                this.m_Km = 0.0015f;		// Mie scattering constant
                this.m_Km4PI = this.m_Km * 4.0f * (float)Math.PI;
                this.m_ESun = 15.0f;		// Sun brightness constant
                this.m_g = -0.85f;		// The Mie phase asymmetry factor

                this.m_fWavelength[0] = 0.650f;		// 650 nm for red
                this.m_fWavelength[1] = 0.570f;		// 570 nm for green
                this.m_fWavelength[2] = 0.475f;		// 475 nm for blue
                this.m_fWavelength4[0] = (float)Math.Pow(this.m_fWavelength[0], 4.0f);
                this.m_fWavelength4[1] = (float)Math.Pow(this.m_fWavelength[1], 4.0f);
                this.m_fWavelength4[2] = (float)Math.Pow(this.m_fWavelength[2], 4.0f);

                this.m_fRayleighScaleDepth = 0.25f;
                this.m_fMieScaleDepth = 0.1f;

			}
			catch (Exception ex)
			{
				Log.Write(ex);
			}
		}

		public void Dispose()
		{
            this.active = false;
		}

		class MeshSubset
		{
			public CustomVertex.PositionColored[] Vertices;
			public CustomVertex.PositionColored[] HigherResolutionVertices;
			public BoundingBox BoundingBox;
		}
		List<MeshSubset> m_meshList = new List<MeshSubset>();

		Vector3 m_lastSunPosition = Vector3.Zero;

		System.Threading.Thread m_backgroundThread;
		bool active;
		DateTime m_lastOpticalUpdate = DateTime.MinValue;
		bool m_canDoShaders;

		private void Updater()
		{
			try
			{
				while (this.active)
				{
					if (World.Settings.EnableAtmosphericScattering && this.m_meshList.Count > 0)
					{
						DateTime currentTime = TimeKeeper.CurrentTimeUtc;
						// Update Sun
                        this.UpdateLightVector();
						if (World.Settings.ForceCpuAtmosphere)
						{
                            this.m_nSamples = 4;		// Number of sample rays to use in integral equation
                            this.m_Kr = 0.0025f;		// Rayleigh scattering constant
                            this.m_Kr4PI = this.m_Kr * 4.0f * (float)Math.PI;
                            this.m_Km = 0.0015f;		// Mie scattering constant
                            this.m_Km4PI = this.m_Km * 4.0f * (float)Math.PI;
                            this.m_ESun = 15.0f;		// Sun brightness constant
                            this.m_g = -0.85f;		// The Mie phase asymmetry factor

                            this.m_fWavelength[0] = 0.650f;		// 650 nm for red
                            this.m_fWavelength[1] = 0.570f;		// 570 nm for green
                            this.m_fWavelength[2] = 0.475f;		// 475 nm for blue
                            this.m_fWavelength4[0] = (float)Math.Pow(this.m_fWavelength[0], 4.0f);
                            this.m_fWavelength4[1] = (float)Math.Pow(this.m_fWavelength[1], 4.0f);
                            this.m_fWavelength4[2] = (float)Math.Pow(this.m_fWavelength[2], 4.0f);

                            this.m_fRayleighScaleDepth = 0.25f;
                            this.m_fMieScaleDepth = 0.1f;

							if (currentTime.Subtract(this.m_lastOpticalUpdate) > TimeSpan.FromSeconds(100))
							{
                                this.MakeOpticalDepthBuffer(m_fInnerRadius, m_fOuterRadius, this.m_fRayleighScaleDepth, this.m_fMieScaleDepth);
                                this.m_lastOpticalUpdate = currentTime;
							}
						}
						else
						{
                            this.m_nSamples = 2;		// Number of sample rays to use in integral equation
                            this.m_Kr = 0.0025f;		// Rayleigh scattering constant
                            this.m_Kr4PI = this.m_Kr * 4.0f * (float)Math.PI;
                            this.m_Km = 0.0015f;		// Mie scattering constant
                            this.m_Km4PI = this.m_Km * 4.0f * (float)Math.PI;
                            this.m_ESun = 15.0f;		// Sun brightness constant
                            this.m_g = -0.95f;		// The Mie phase asymmetry factor
                            this.m_fScale = 1 / (m_fOuterRadius - m_fInnerRadius);

                            this.m_fWavelength[0] = 0.650f;		// 650 nm for red
                            this.m_fWavelength[1] = 0.570f;		// 570 nm for green
                            this.m_fWavelength[2] = 0.475f;		// 475 nm for blue
                            this.m_fWavelength4[0] = (float)Math.Pow(this.m_fWavelength[0], 4.0f);
                            this.m_fWavelength4[1] = (float)Math.Pow(this.m_fWavelength[1], 4.0f);
                            this.m_fWavelength4[2] = (float)Math.Pow(this.m_fWavelength[2], 4.0f);

                            this.m_fRayleighScaleDepth = 0.25f;
                            this.m_fMieScaleDepth = 0.1f;
						}
					}
					System.Threading.Thread.Sleep(500);
				}
			}
			catch
			{ }
		}
		public void Update(DrawArgs drawArgs)
		{


			if (this.m_backgroundThread == null)
			{
				if (drawArgs.device.Capabilities.PixelShaderVersion.Major >= 2)
				{
                    this.m_canDoShaders = true;
				}
				else
				{
                    this.m_canDoShaders = false;
				}

                this.active = true;
                this.m_backgroundThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.Updater));
                this.m_backgroundThread.Name = "AtmosphericScatteringSphere Updater";
                this.m_backgroundThread.Priority = System.Threading.ThreadPriority.Lowest;
                this.m_backgroundThread.IsBackground = true;
                this.m_backgroundThread.Start();
			}
		}

		CustomVertex.PositionColored[] CreateMesh(double minLat, double maxLat, double minLon, double maxLon, int meshPointCount)
		{
			int upperBound = meshPointCount - 1;
			float scaleFactor = (float)1 / upperBound;
			double latrange = Math.Abs(maxLat - minLat);
			double lonrange;
			if (minLon < maxLon)
				lonrange = maxLon - minLon;
			else
				lonrange = 360.0f + maxLon - minLon;

			CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[meshPointCount * meshPointCount];

			for (int i = 0; i < meshPointCount; i++)
			{
				for (int j = 0; j < meshPointCount; j++)
				{
					Vector3 pos = MathEngine.SphericalToCartesian(
						maxLat - scaleFactor * latrange * i,
						minLon + scaleFactor * lonrange * j, this.m_radius);

					vertices[i * meshPointCount + j].X = pos.X;
					vertices[i * meshPointCount + j].Y = pos.Y;
					vertices[i * meshPointCount + j].Z = pos.Z;
				}
			}

			return vertices;
		}

		short[] computeIndices(int meshPointCount)
		{
			int upperBound = meshPointCount - 1;
			short[] indices = new short[2 * upperBound * upperBound * 3];
			for (int i = 0; i < upperBound; i++)
			{
				for (int j = 0; j < upperBound; j++)
				{
					indices[(2 * 3 * i * upperBound) + 6 * j] = (short)(i * meshPointCount + j);
					indices[(2 * 3 * i * upperBound) + 6 * j + 1] = (short)((i + 1) * meshPointCount + j);
					indices[(2 * 3 * i * upperBound) + 6 * j + 2] = (short)(i * meshPointCount + j + 1);

					indices[(2 * 3 * i * upperBound) + 6 * j + 3] = (short)(i * meshPointCount + j + 1);
					indices[(2 * 3 * i * upperBound) + 6 * j + 4] = (short)((i + 1) * meshPointCount + j);
					indices[(2 * 3 * i * upperBound) + 6 * j + 5] = (short)((i + 1) * meshPointCount + j + 1);
				}
			}

			return indices;
		}

		short[] m_indices;
		short[] m_indicesHighResolution;
		float[] fCameraDepth = new float[4] { 0, 0, 0, 0 };
		float[] fLightDepth = new float[4];
		float[] fSampleDepth = new float[4];
		float[] fRayleighSum = new float[] { 0, 0, 0 };
		float[] fMieSum = new float[] { 0, 0, 0 };
		Vector3 vPos = new Vector3();
		float[] fAttenuation = new float[3];
		Vector3 vCamera = new Vector3();

		public void SetColor(ref CustomVertex.PositionColored pVertex, DrawArgs drawArgs)
		{
            this.vPos.X = pVertex.X;
            this.vPos.Y = pVertex.Y;
            this.vPos.Z = pVertex.Z;

			// Get the ray from the camera to the vertex, and its length (which is the far point of the ray passing through the atmosphere)
            this.vCamera.X = drawArgs.WorldCamera.Position.X;
            this.vCamera.Y = drawArgs.WorldCamera.Position.Y;
            this.vCamera.Z = drawArgs.WorldCamera.Position.Z;

			Vector3 vRay = this.vPos - this.vCamera;
			float fFar = vRay.Length();

			vRay.Normalize();

			// Calculate the closest intersection of the ray with the outer atmosphere (which is the near point of the ray passing through the atmosphere)
			float B = 2.0f * Vector3.Dot(this.vCamera, vRay);
			float C = Vector3.Dot(this.vCamera, this.vCamera) - m_fOuterRadius * m_fOuterRadius;
			float fDet = (float)Math.Max(0.0f, B * B - 4.0f * C);
			float fNear = 0.5f * (-B - (float)Math.Sqrt(fDet));

			bool bCameraAbove = true;

			for (int i = 0; i < this.fCameraDepth.Length; i++) this.fCameraDepth[i] = 0;

			for (int i = 0; i < this.fLightDepth.Length; i++) this.fLightDepth[i] = 0;

			for (int i = 0; i < this.fSampleDepth.Length; i++) this.fSampleDepth[i] = 0;

			if (fNear <= 0)
			{
				// If the near point is behind the camera, it means the camera is inside the atmosphere
				fNear = 0;
				float fCameraHeight = this.vCamera.Length();
				float fCameraAltitude = (fCameraHeight - m_fInnerRadius) * this.m_fScale;
				bCameraAbove = fCameraHeight >= this.vPos.Length();
				float fCameraAngle = Vector3.Dot((bCameraAbove ? -vRay : vRay), this.vCamera) / fCameraHeight;
                this.Interpolate(ref this.fCameraDepth, fCameraAltitude, 0.5f - fCameraAngle * 0.5f);
			}
			else
			{
				// Otherwise, move the camera up to the near intersection point
                this.vCamera += vRay * fNear;
				fFar -= fNear;
				fNear = 0;
			}

			// If the distance between the points on the ray is negligible, don't bother to calculate anything
			if (fFar <= this.DELTA)
			{
				pVertex.Color = System.Drawing.Color.FromArgb(255, 0, 0, 0).ToArgb();
				return;
			}

			// Initialize a few variables to use inside the loop
			for (int i = 0; i < this.fRayleighSum.Length; i++) this.fRayleighSum[i] = 0;
			for (int i = 0; i < this.fMieSum.Length; i++) this.fMieSum[i] = 0;

			float fSampleLength = fFar / this.m_nSamples;
			float fScaledLength = fSampleLength * this.m_fScale;
			Vector3 vSampleRay = vRay * fSampleLength;

			// Start at the center of the first sample ray, and loop through each of the others
            this.vPos = this.vCamera + vSampleRay * 0.5f;
			for (int i = 0; i < this.m_nSamples; i++)
			{
				float fHeight = this.vPos.Length();

				// Start by looking up the optical depth coming from the light source to this point
				float fLightAngle = Vector3.Dot(this.m_vLightDirection, this.vPos) / fHeight;
				float fAltitude = (fHeight - m_fInnerRadius) * this.m_fScale;
                this.Interpolate(ref this.fLightDepth, fAltitude, 0.5f - fLightAngle * 0.5f);

				// If no light light reaches this part of the atmosphere, no light is scattered in at this point
				if (this.fLightDepth[0] < this.DELTA)
					continue;

				// Get the density at this point, along with the optical depth from the light source to this point
				float fRayleighDensity = fScaledLength * this.fLightDepth[0];
				float fRayleighDepth = this.fLightDepth[1];
				float fMieDensity = fScaledLength * this.fLightDepth[2];
				float fMieDepth = this.fLightDepth[3];

				// If the camera is above the point we're shading, we calculate the optical depth from the sample point to the camera
				// Otherwise, we calculate the optical depth from the camera to the sample point
				if (bCameraAbove)
				{
					float fSampleAngle = Vector3.Dot(-vRay, this.vPos) / fHeight;
                    this.Interpolate(ref this.fSampleDepth, fAltitude, 0.5f - fSampleAngle * 0.5f);
					fRayleighDepth += this.fSampleDepth[1] - this.fCameraDepth[1];
					fMieDepth += this.fSampleDepth[3] - this.fCameraDepth[3];
				}
				else
				{
					float fSampleAngle = Vector3.Dot(vRay, this.vPos) / fHeight;
                    this.Interpolate(ref this.fSampleDepth, fAltitude, 0.5f - fSampleAngle * 0.5f);
					fRayleighDepth += this.fCameraDepth[1] - this.fSampleDepth[1];
					fMieDepth += this.fCameraDepth[3] - this.fSampleDepth[3];
				}

				// Now multiply the optical depth by the attenuation factor for the sample ray
				fRayleighDepth *= this.m_Kr4PI;
				fMieDepth *= this.m_Km4PI;

				// Calculate the attenuation factor for the sample ray
                this.fAttenuation[0] = (float)Math.Exp(-fRayleighDepth / this.m_fWavelength4[0] - fMieDepth);
                this.fAttenuation[1] = (float)Math.Exp(-fRayleighDepth / this.m_fWavelength4[1] - fMieDepth);
                this.fAttenuation[2] = (float)Math.Exp(-fRayleighDepth / this.m_fWavelength4[2] - fMieDepth);

                this.fRayleighSum[0] += fRayleighDensity * this.fAttenuation[0];
                this.fRayleighSum[1] += fRayleighDensity * this.fAttenuation[1];
                this.fRayleighSum[2] += fRayleighDensity * this.fAttenuation[2];

                this.fMieSum[0] += fMieDensity * this.fAttenuation[0];
                this.fMieSum[1] += fMieDensity * this.fAttenuation[1];
                this.fMieSum[2] += fMieDensity * this.fAttenuation[2];

				// Move the position to the center of the next sample ray
                this.vPos += vSampleRay;
			}

			// Calculate the angle and phase values (this block of code could be handled by a small 1D lookup table, or a 1D texture lookup in a pixel shader)
			float fAngle = (float)Vector3.Dot(-vRay, this.m_vLightDirection);
			float[] fPhase = new float[2];
			float fAngle2 = fAngle * fAngle;
			float g2 = this.m_g * this.m_g;
			fPhase[0] = 0.75f * (1.0f + fAngle2);
			fPhase[1] = 1.5f * ((1 - g2) / (2 + g2)) * (1.0f + fAngle2) / (float)Math.Pow(1 + g2 - 2 * this.m_g * fAngle, 1.5f);
			fPhase[0] *= this.m_Kr * this.m_ESun;
			fPhase[1] *= this.m_Km * this.m_ESun;
			// Calculate the in-scattering color and clamp it to the max color value
			float[] fColor = new float[3] { 0, 0, 0 };
			fColor[0] = this.fRayleighSum[0] * fPhase[0] / this.m_fWavelength4[0] + this.fMieSum[0] * fPhase[1];
			fColor[1] = this.fRayleighSum[1] * fPhase[0] / this.m_fWavelength4[1] + this.fMieSum[1] * fPhase[1];
			fColor[2] = this.fRayleighSum[2] * fPhase[0] / this.m_fWavelength4[2] + this.fMieSum[2] * fPhase[1];
			fColor[0] = (float)Math.Min(fColor[0], 1.0f);
			fColor[1] = (float)Math.Min(fColor[1], 1.0f);
			fColor[2] = (float)Math.Min(fColor[2], 1.0f);

			// Compute alpha transparency (PM 2006-11-19)
			float alpha = (fColor[0] + fColor[1] + fColor[2]) / 3;  // Average luminosity
			alpha = (float)Math.Min(alpha + 0.50, 1f);			  // increase opacity

			// Last but not least, set the color
			pVertex.Color = System.Drawing.Color.FromArgb((byte)(alpha * 255), (byte)(fColor[0] * 255), (byte)(fColor[1] * 255), (byte)(fColor[2] * 255)).ToArgb();

		}

		void Interpolate(ref float[] p, float x, float y)
		{
			float fX = x * (m_nWidth - 1);
			float fY = y * (m_nHeight - 1);
			int nX = Math.Min(m_nWidth - 2, Math.Max(0, (int)fX));
			int nY = Math.Min(m_nHeight - 2, Math.Max(0, (int)fY));
			float fRatioX = fX - nX;
			float fRatioY = fY - nY;

			//float *pValue = (float *)((unsigned long)m_pBuffer + m_nElementSize * (m_nWidth * nY + nX));
			//float pValue = m_opticalDepthBuffer[m_nWidth * nY + nX];
			int pValueOffset = (m_nWidth * nY + nX) * 4;

			for (int i = 0; i < m_nChannels; i++)
			{
				if (this.m_currentOpticalBuffer == 1)
				{
					p[i] = this.m_opticalDepthBuffer1[pValueOffset] * (1 - fRatioX) * (1 - fRatioY) + this.m_opticalDepthBuffer1[pValueOffset + m_nChannels * 1] * (fRatioX) * (1 - fRatioY) + this.m_opticalDepthBuffer1[pValueOffset + m_nChannels * m_nWidth] * (1 - fRatioX) * (fRatioY) + this.m_opticalDepthBuffer1[pValueOffset + m_nChannels * (m_nWidth + 1)] * (fRatioX) * (fRatioY);
				}
				else
				{
					p[i] = this.m_opticalDepthBuffer2[pValueOffset] * (1 - fRatioX) * (1 - fRatioY) + this.m_opticalDepthBuffer2[pValueOffset + m_nChannels * 1] * (fRatioX) * (1 - fRatioY) + this.m_opticalDepthBuffer2[pValueOffset + m_nChannels * m_nWidth] * (1 - fRatioX) * (fRatioY) + this.m_opticalDepthBuffer2[pValueOffset + m_nChannels * (m_nWidth + 1)] * (fRatioX) * (fRatioY);
				}
				pValueOffset++;
			}
		}

		float DELTA = 1e-6f;
		//static float[] m_opticalDepthBuffer = null;
		static int m_nChannels = 4;
		static int m_nWidth;				// The width of the buffer (x axis)
		static int m_nHeight;				// The height of the buffer (y axis)
		//static int m_nDepth = 0;				// The depth of the buffer (z axis)
		//static int m_nDataType = 0;			// The data type stored in the buffer (i.e. GL_UNSIGNED_BYTE, GL_FLOAT)
		static int m_nElementSize;			// The size of one element in the buffer
		float m_fScale;
		float[] m_fWavelength = new float[3];
		float[] m_fWavelength4 = new float[3];
		float m_fRayleighScaleDepth;
		float m_fMieScaleDepth;

		int m_nSamples;
		float m_Kr, m_Kr4PI;
		float m_Km, m_Km4PI;
		float m_ESun;
		float m_g;

		Vector3 m_vLight;
		Vector3 m_vLightDirection;

		private void UpdateColor(DrawArgs drawArgs, MeshSubset meshSubset, bool doHighResolution)
		{
			int blank = System.Drawing.Color.FromArgb(255, 0, 0, 0).ToArgb();

			if (doHighResolution)
			{
				for (int i = 0; i < meshSubset.HigherResolutionVertices.Length; i++)
				{
					if (Vector3.Dot(drawArgs.WorldCamera.Position, new Vector3(meshSubset.HigherResolutionVertices[i].X, meshSubset.HigherResolutionVertices[i].Y, meshSubset.HigherResolutionVertices[i].Z)) > 0)
                        this.SetColor(ref meshSubset.HigherResolutionVertices[i], drawArgs);
					else
						meshSubset.HigherResolutionVertices[i].Color = blank;
				}
			}
			else
			{
				for (int i = 0; i < meshSubset.Vertices.Length; i++)
				{
					if (Vector3.Dot(drawArgs.WorldCamera.Position, new Vector3(meshSubset.Vertices[i].X, meshSubset.Vertices[i].Y, meshSubset.Vertices[i].Z)) > 0)
                        this.SetColor(ref meshSubset.Vertices[i], drawArgs);
					else
						meshSubset.Vertices[i].Color = blank;
				}
			}
		}

		// -- Addition from Sky Gradient geometry (PM 2006-11-27)
		// Rebuild sky mesh with updated colors
		double thickness;
		Mesh skyMesh;
		private void UpdateSkyMesh(DrawArgs drawArgs, double horizonSpan)
		{
			CameraBase camera = drawArgs.WorldCamera;
			Device device = drawArgs.device;

			// Use world atmospheric scattering sphere radius for thickness
            this.thickness = this.m_radius - camera.WorldRadius;

			double distToCenterOfPlanet = (camera.Altitude + camera.WorldRadius);
			// Compute distance to horizon and dome radius
			double tangentalDistance = Math.Sqrt(distToCenterOfPlanet * distToCenterOfPlanet - camera.WorldRadius * camera.WorldRadius);
			double domeRadius = tangentalDistance;

			// horizon latitude
			double horizonLat = (-Math.PI / 2 + Math.Acos(tangentalDistance / distToCenterOfPlanet)) * 180 / Math.PI;

			// zenith latitude
			double zenithLat = 90;
			if (camera.Altitude >= this.thickness)
			{
				double tangentalDistanceZenith = Math.Sqrt(distToCenterOfPlanet * distToCenterOfPlanet - (camera.WorldRadius + this.thickness) * (camera.WorldRadius + this.thickness));
				zenithLat = (-Math.PI / 2 + Math.Acos(tangentalDistanceZenith / distToCenterOfPlanet)) * 180 / Math.PI;
			}
			if (camera.Altitude < this.thickness && camera.Altitude > this.thickness * 0.8)
			{
				zenithLat = (this.thickness - camera.Altitude) / (this.thickness - this.thickness * 0.8) * 90;
			}
			// new mesh
			if (this.skyMesh != null) this.skyMesh.Dispose();
			int res = horizonSpan > 180 ? 64 : 128;
            this.skyMesh = this.ColoredSpherePartial(drawArgs, (float)domeRadius, horizonLat, zenithLat, res, res / 2, horizonSpan, camera.Heading.Degrees);

		}

		/// <summary>
		/// Creates a partial PositionColored sphere pre-transformed to the camera position
		/// modified to provide a sky/atmosphere gradient dome
		/// </summary>
        /// <param name="drawArgs">Where to get the current direct3D drawing device.</param>
		/// <param name="radius">The sphere's radius</param>
		/// <param name="slices">Number of slices (Horizontal resolution).</param>
		/// <param name="stacks">Number of stacks. (Vertical resolution)</param>
		/// <returns></returns>
		/// <remarks>
		/// Number of vertices in the sphere will be (slices+1)*(stacks+1)<br/>
		/// Number of faces :slices*stacks*2
		/// Number of Indexes   : Number of faces * 3;
		/// </remarks>
		private Mesh ColoredSpherePartial(DrawArgs drawArgs, float radius, double startLat, double endLat, int slices, int stacks, double lonSpan, double heading)
		{
			slices = (int)((double)slices * lonSpan / 360);

			int numVertices = (slices + 1) * (stacks + 1);
			int numFaces = slices * stacks * 2;
			int indexCount = numFaces * 3;
			Device device = drawArgs.device;

			// SkyGradient dome transform
			CameraBase camera = drawArgs.WorldCamera;
			Vector3 cameraPos = camera.Position;
			double distToCenterOfPlanet = (camera.Altitude + camera.WorldRadius);
			Vector3 cameraCoord = MathEngine.CartesianToSpherical(cameraPos.X, cameraPos.Y, cameraPos.Z);
			float camLat = cameraCoord.Y;
			float camLon = cameraCoord.Z;
			Matrix SkyGradientTrans = Matrix.Identity;
			SkyGradientTrans *= Matrix.Translation(0, 0, (float)distToCenterOfPlanet);
			SkyGradientTrans *= Matrix.RotationY(-camLat + (float)Math.PI / 2);
			SkyGradientTrans *= Matrix.RotationZ(camLon);

			// Find camera heading from the camera lat/lon to the target
			double d = MathEngine.SphericalDistance(Angle.FromRadians(camLat), Angle.FromRadians(camLon), camera.Latitude, camera.Longitude).Radians;
			double camHead = Math.Acos((Math.Sin(camera.Latitude.Radians) - Math.Sin(camLat) * Math.Cos(d)) / (Math.Sin(d) * Math.Cos(camLat)));
			if (Math.Sign(camera.Longitude.Radians - camLon) < 0) camHead = Math.PI * 2 - camHead;
			if (double.IsNaN(camHead)) camHead = 0;
			camHead = MathEngine.RadiansToDegrees(camHead);
			double startLon = -camHead - 180 + (lonSpan / 2);

			Mesh mesh = new Mesh(numFaces, numVertices, MeshFlags.Managed, CustomVertex.PositionColored.Format, device);

			// Get the original sphere's vertex buffer.
			int[] ranks = new int[1];
			ranks[0] = mesh.NumberVertices;
			Array arr = mesh.VertexBuffer.Lock(0, typeof(CustomVertex.PositionColored), LockFlags.None, ranks);

			// Set the vertex buffer
			int vertIndex = 0;
			CustomVertex.PositionColored pnt;
			Vector3 v;

			// bottom fade
			double latitude = startLat - ((endLat - startLat) / 10);
			if (latitude < startLat - 1) latitude = startLat - 1;
			for (int slice = 0; slice <= slices; slice++)
			{
				pnt = new CustomVertex.PositionColored();
				double longitude = startLon - ((float)slice / slices * lonSpan);
				v = MathEngine.SphericalToCartesian(latitude, longitude, radius);
				v.TransformCoordinate(SkyGradientTrans);
				pnt.X = v.X;
				pnt.Y = v.Y;
				pnt.Z = v.Z;
				pnt.Color = System.Drawing.Color.FromArgb(0, 0, 0, 0).ToArgb();
				arr.SetValue(pnt, vertIndex++);
			}
			// stacks and slices
			for (int stack = 1; stack < stacks; stack++)
			{
				//latitude = startLat + ((float)(stack-1)/(stacks-1f)*(float)(endLat - startLat));
				double linear = (float)(stack - 1) / (stacks - 1f);
				double k = 1 - Math.Cos((float)(stack - 1) / (stacks - 1f) * Math.PI / 2);
				latitude = startLat + (k * k * k * (float)(endLat - startLat));
				//double colorFactorZ = (float)(stack-1)/(stacks-1f); 	// coef zenith color
				double colorFactorZ = linear; 				// coef zenith color
				double colorFactorH = 1 - colorFactorZ;			// coef horizon color
				double alphaFactor = 1 - (linear * linear * linear);	// coef alpha transparency
				if (alphaFactor > .8) alphaFactor = .8f;
				for (int slice = 0; slice <= slices; slice++)
				{
					pnt = new CustomVertex.PositionColored();
					double longitude = startLon - ((float)slice / slices * lonSpan);
					v = MathEngine.SphericalToCartesian(latitude, longitude, radius);
					v.TransformCoordinate(SkyGradientTrans);
					pnt.X = v.X;
					pnt.Y = v.Y;
					pnt.Z = v.Z;
					pnt.Color = this.getAtmosphereColor(drawArgs, pnt);
					arr.SetValue(pnt, vertIndex++);
				}
			}
			// top fade
			latitude = endLat + ((endLat - startLat) / 10);
			for (int slice = 0; slice <= slices; slice++)
			{
				pnt = new CustomVertex.PositionColored();
				double longitude = startLon - ((float)slice / slices * lonSpan);
				v = MathEngine.SphericalToCartesian(latitude, longitude, radius);
				v.TransformCoordinate(SkyGradientTrans);
				pnt.X = v.X;
				pnt.Y = v.Y;
				pnt.Z = v.Z;
				pnt.Color = System.Drawing.Color.FromArgb(0, 0, 0, 0).ToArgb();
				arr.SetValue(pnt, vertIndex++);
			}

			mesh.VertexBuffer.Unlock();
			ranks[0] = indexCount;
			arr = mesh.LockIndexBuffer(typeof(short), LockFlags.None, ranks);
			int i = 0;
			short bottomVertex = 0;
			short topVertex = 0;

			// stacks and slices
			for (short x = 0; x < stacks; x++)
			{
				bottomVertex = (short)((slices + 1) * x);
				topVertex = (short)(bottomVertex + slices + 1);
				for (int y = 0; y < slices; y++)
				{
					arr.SetValue(bottomVertex, i++);
					arr.SetValue((short)(topVertex + 1), i++);
					arr.SetValue(topVertex, i++);
					arr.SetValue(bottomVertex, i++);
					arr.SetValue((short)(bottomVertex + 1), i++);
					arr.SetValue((short)(topVertex + 1), i++);
					bottomVertex++;
					topVertex++;
				}
			}

            mesh.UnlockIndexBuffer();
            mesh.IndexBuffer.SetData(arr, 0, LockFlags.None);

			return mesh;
		}

		CustomVertex.PositionColored p2 = new CustomVertex.PositionColored();
		/// <summary>
		/// Compute sky vertex color using atmospheric scattering
		/// </summary>
		int getAtmosphereColor(DrawArgs drawArgs, CustomVertex.PositionColored pVertex)
		{
			// Find out intersection point on world scattering sphere
			//Vector3 vPos = new Vector3();
            this.vPos.X = pVertex.X;
            this.vPos.Y = pVertex.Y;
            this.vPos.Z = pVertex.Z;

			// Get the ray from the camera to the vertex
			//Vector3 vCamera = new Vector3();
            this.vCamera.X = drawArgs.WorldCamera.Position.X;
            this.vCamera.Y = drawArgs.WorldCamera.Position.Y;
            this.vCamera.Z = drawArgs.WorldCamera.Position.Z;

			Vector3 vRay = this.vPos - this.vCamera;
			vRay.Normalize();

			// Calculate the closest intersection of the ray with the outer atmosphere 
			float B = 2.0f * Vector3.Dot(this.vCamera, vRay);
			float C = Vector3.Dot(this.vCamera, this.vCamera) - this.m_radius * this.m_radius;
			float fDet = B * B - 4.0f * C;

            this.p2.Color = System.Drawing.Color.FromArgb(0, 0, 0, 0).ToArgb();
			if (fDet >= 0)
			{
				// Camera ray intersect atmosphere
				float fNear1 = 0.5f * (-B - (float)Math.Sqrt(fDet));
				float fNear2 = 0.5f * (-B + (float)Math.Sqrt(fDet));
				if (fNear1 >= 0 || fNear2 >= 0)
				{
					// biggest distance - not sure why...
					float fNear = (float)Math.Max(fNear1, fNear2);
                    this.vPos = this.vCamera + vRay * fNear;
                    this.p2.X = this.vPos.X;
                    this.p2.Y = this.vPos.Y;
                    this.p2.Z = this.vPos.Z;
                    this.SetColor(ref this.p2, drawArgs);
				}
			}
			return this.p2.Color;
		}
		// -- End SkyGradiant geometry addition

		static Effect skyFromSpaceEffect = null;
		static Effect skyFromAtmosphere = null;

		public void Render(DrawArgs drawArgs)
		{
			try
			{
				if (this.m_meshList.Count > 0 && ((!World.Settings.ForceCpuAtmosphere && this.m_canDoShaders) || this.m_opticalDepthBuffer1 != null))
				{
					double horizonSpan = this.HorizonSpan(drawArgs);
					if (horizonSpan == 0) return;   // Check if horizon visible (PM 2006-11-28)

					if (skyFromSpaceEffect == null)
					{
						drawArgs.device.DeviceReset += new EventHandler(this.device_DeviceReset);
                        this.device_DeviceReset(drawArgs.device, null);
					}

                    this.vCamera.X = drawArgs.WorldCamera.Position.X;
                    this.vCamera.Y = drawArgs.WorldCamera.Position.Y;
                    this.vCamera.Z = drawArgs.WorldCamera.Position.Z;

					drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
					drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
					if (drawArgs.device.SetRenderState(RenderState.Lighting)
						drawArgs.device.SetRenderState(RenderState.Lighting , false);

					drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
						(float)-drawArgs.WorldCamera.ReferenceCenter.X,
						(float)-drawArgs.WorldCamera.ReferenceCenter.Y,
						(float)-drawArgs.WorldCamera.ReferenceCenter.Z
						);

					bool doHighResolution = (drawArgs.WorldCamera.Altitude < 300000);

					Frustum frustum = new Frustum();

					frustum.Update(
						Matrix.Multiply(drawArgs.device.SetTransform(TransformState.World,
						Matrix.Multiply(drawArgs.device.SetTransform(TransformState.View, drawArgs.device.SetTransform(TransformState.Projection)));

					if (!World.Settings.ForceCpuAtmosphere && this.m_canDoShaders)
					{
						Effect shader = null;
						// Update Sun
                        this.UpdateLightVector();
						if (this.vCamera.Length() >= m_fOuterRadius)
							shader = skyFromSpaceEffect;
						else
							shader = skyFromAtmosphere;

						shader.Technique = "Sky";
						shader.SetValue("v3CameraPos", new Vector4(this.vCamera.X, this.vCamera.Y, this.vCamera.Z, 0));
						shader.SetValue("v3LightPos", Vector4.Normalize(new Vector4(this.m_vLightDirection.X, this.m_vLightDirection.Y, this.m_vLightDirection.Z, 0)));
						shader.SetValue("WorldViewProj", Matrix.Multiply(drawArgs.device.SetTransform(TransformState.World, Matrix.Multiply(drawArgs.device.SetTransform(TransformState.View, drawArgs.device.SetTransform(TransformState.Projection)));
						shader.SetValue("v3InvWavelength", new Vector4(1.0f / this.m_fWavelength4[0], 1.0f / this.m_fWavelength4[1], 1.0f / this.m_fWavelength4[2], 0));
						shader.SetValue("fCameraHeight", this.vCamera.Length());
						shader.SetValue("fCameraHeight2", this.vCamera.LengthSq());
						shader.SetValue("fInnerRadius", m_fInnerRadius);
						shader.SetValue("fInnerRadius2", m_fInnerRadius * m_fInnerRadius);
						shader.SetValue("fOuterRadius", m_fOuterRadius);
						shader.SetValue("fOuterRadius2", m_fOuterRadius * m_fOuterRadius);
						shader.SetValue("fKrESun", this.m_Kr * this.m_ESun);
						shader.SetValue("fKmESun", this.m_Km * this.m_ESun);
						shader.SetValue("fKr4PI", this.m_Kr4PI);
						shader.SetValue("fKm4PI", this.m_Km4PI);
						shader.SetValue("fScale", 1.0f / (m_fOuterRadius - m_fInnerRadius));
						shader.SetValue("fScaleDepth", this.m_fRayleighScaleDepth);
						shader.SetValue("fScaleOverScaleDepth", (1.0f / (m_fOuterRadius - m_fInnerRadius)) / this.m_fRayleighScaleDepth);
						shader.SetValue("g", this.m_g);
						shader.SetValue("g2", this.m_g * this.m_g);
						shader.SetValue("nSamples", this.m_nSamples);
						shader.SetValue("fSamples", this.m_nSamples);

						for (int i = 0; i < this.m_meshList.Count; i++)
						{
							if (!frustum.Intersects(this.m_meshList[i].BoundingBox))
								continue;

							int numPasses = shader.Begin(0);
							for (int j = 0; j < numPasses; j++)
							{
								shader.BeginPass(j);
								if (doHighResolution)
									drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, this.m_meshList[i].HigherResolutionVertices.Length, this.m_indicesHighResolution.Length / 3, this.m_indicesHighResolution, true, this.m_meshList[i].HigherResolutionVertices);
								else
									drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, this.m_meshList[i].Vertices.Length, this.m_indices.Length / 3, this.m_indices, true, this.m_meshList[i].Vertices);
								shader.EndPass();
							}
							shader.End();
						}
					}
					else
					{
						/*for (int i = 0; i < m_meshList.Count; i++)
						{
							if (!frustum.Intersects(m_meshList[i].BoundingBox))
								continue;

							UpdateColor(drawArgs, m_meshList[i], doHighResolution);
							if (doHighResolution)
								drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, m_meshList[i].HigherResolutionVertices.Length, m_indicesHighResolution.Length / 3, m_indicesHighResolution, true, m_meshList[i].HigherResolutionVertices);
							else
								drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, m_meshList[i].Vertices.Length, m_indices.Length / 3, m_indices, true, m_meshList[i].Vertices);
							
						} */
						// Update Sun
                        this.UpdateLightVector();
						// Use SkyGradient geometry
                        this.UpdateSkyMesh(drawArgs, horizonSpan);
						drawArgs.device.SetRenderState(RenderState.CullMode , Cull.Clockwise);
                        this.skyMesh.DrawSubset(0);
					}
					drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex);
			}
		}

		// Check if horizon is visible in camera viewport (PM 2006-11-28)
		// Returns the horizon span angle or 0 if not visible
		double HorizonSpan(DrawArgs drawArgs)
		{
			// Camera & Viewport shortcuts
			CameraBase camera = drawArgs.WorldCamera;
			Viewport viewport = camera.Viewport;

			// Compute camera altitude
			double cameraAltitude = camera.Position.Length() - camera.WorldRadius;

			// Compute camera absolute field of view (to the horizon)
			double fovH = Math.Abs(Math.Asin(camera.WorldRadius / (camera.WorldRadius + camera.Altitude))) * 2;

			// Compute viewport diagonal field of view
			int h = viewport.Height;
			int w = viewport.Width;
			double fovV = camera.Fov.Radians;
			double fovD = Math.Abs(Math.Atan(Math.Sqrt(h * h + w * w) * Math.Tan(fovV / 2) / h)) * 2;

			// Compute camera tilt from vertical at the camera position
			double tilt = camera.Tilt.Radians * 2;
			if (camera.Altitude > 10000)
			{
				double a = camera.WorldRadius;					  // World radius
				double b = camera.WorldRadius + camera.Altitude;	 // Camera to center of planet
				double c = camera.Distance;						 // Distance to target
				tilt = Math.Abs(Math.Acos((a * a - c * c - b * b) / (-2 * c * b))) * 2;
				if (double.IsNaN(tilt)) tilt = 0;
			}

			// Check if cones intersect
			double span = 0;
			if (fovD + tilt > fovH)
			{
				span = fovD < fovH ? Math.Abs(Math.Asin(Math.Sin(fovD / 2) / Math.Sin(fovH / 2))) * 2 : Math.PI * 2;
				span *= 180 / Math.PI;
			}

			return span;
		}

		void device_DeviceReset(object sender, EventArgs e)
		{
			Device device = (Device)sender;

			try
			{
				string outerrors = "";

				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				Stream skyFromSpaceStream = assembly.GetManifestResourceStream("WorldWind.Shaders.SkyFromSpace.fx");

				skyFromSpaceEffect =
					Effect.FromStream(
					device,
					skyFromSpaceStream,
					null,
					null,
					ShaderFlags.None,
					null,
					out outerrors);

				if (outerrors != null && outerrors.Length > 0)
					Log.Write(Log.Levels.Error, outerrors);

				Stream skyFromAtmosphereStream = assembly.GetManifestResourceStream("WorldWind.Shaders.SkyFromAtmosphere.fx");

				skyFromAtmosphere =
					Effect.FromStream(
					device,
					skyFromAtmosphereStream,
					null,
					null,
					ShaderFlags.None,
					null,
					out outerrors);

				if (outerrors != null && outerrors.Length > 0)
					Log.Write(Log.Levels.Error, outerrors);

			}
			catch (Exception ex)
			{
				Log.Write(ex);
			}
		}

		// Updates vLight and vLightDirection according to Sun position
		void UpdateLightVector()
		{
			DateTime currentTime = TimeKeeper.CurrentTimeUtc;
			Point3d sunPosition = SunCalculator.GetGeocentricPosition(currentTime);
			Vector3 sunVector = new Vector3(
				(float)-sunPosition.X,
				(float)-sunPosition.Y,
				(float)-sunPosition.Z);

            this.m_vLight = sunVector * 100000000f;
            this.m_vLightDirection = new Vector3(this.m_vLight.X / this.m_vLight.Length(), this.m_vLight.Y / this.m_vLight.Length(), this.m_vLight.Z / this.m_vLight.Length()
				);
		}

		void MakeOpticalDepthBuffer(float fInnerRadius, float fOuterRadius, float fRayleighScaleHeight, float fMieScaleHeight)
		{
			int nSize = 128;
			int nSamples = 50;
			float fScale = 1.0f / (fOuterRadius - fInnerRadius);

			if (this.m_opticalDepthBuffer1 == null) this.m_opticalDepthBuffer1 = new float[nSize * nSize * 4];

			if (this.m_opticalDepthBuffer2 == null) this.m_opticalDepthBuffer2 = new float[nSize * nSize * 4];

			if (this.m_currentOpticalBuffer == 1)
			{
				for (int i = 0; i < this.m_opticalDepthBuffer2.Length; i++)
				{
                    this.m_opticalDepthBuffer2[i] = 0;
				}
			}
			else
			{
				for (int i = 0; i < this.m_opticalDepthBuffer1.Length; i++)
				{
                    this.m_opticalDepthBuffer1[i] = 0;
				}
			}

			m_nWidth = nSize;
			m_nHeight = nSize;
			//m_nDepth = 1;
			//m_nDataType = 4;
			m_nChannels = 4;
			m_nElementSize = m_nChannels * 4;
			int nIndex = 0;
			for (int nAngle = 0; nAngle < nSize; nAngle++)
			{
				// As the y tex coord goes from 0 to 1, the angle goes from 0 to 180 degrees
				float fCos = 1.0f - (nAngle + nAngle) / (float)nSize;
				float fAngle = (float)Math.Acos(fCos);

				Vector3 vRay = new Vector3((float)Math.Sin(fAngle), (float)Math.Cos(fAngle), 0);	// Ray pointing to the viewpoint
				for (int nHeight = 0; nHeight < nSize; nHeight++)
				{
					// As the x tex coord goes from 0 to 1, the height goes from the bottom of the atmosphere to the top
					float fHeight = this.DELTA + fInnerRadius + ((fOuterRadius - fInnerRadius) * nHeight) / nSize;
					Vector3 vPos = new Vector3(0, fHeight, 0);				// The position of the camera

					// If the ray from vPos heading in the vRay direction intersects the inner radius (i.e. the planet), then this spot is not visible from the viewpoint
					float B = 2.0f * Vector3.Dot(vPos, vRay);
					float Bsq = B * B;
					float Cpart = Vector3.Dot(vPos, vPos);
					float C = Cpart - fInnerRadius * fInnerRadius;
					float fDet = Bsq - 4.0f * C;
					bool bVisible = (fDet < 0 || (0.5f * (-B - (float)Math.Sqrt(fDet)) <= 0) && (0.5f * (-B + (float)Math.Sqrt(fDet)) <= 0));
					float fRayleighDensityRatio;
					float fMieDensityRatio;
					if (bVisible)
					{
						fRayleighDensityRatio = (float)Math.Exp(-(fHeight - fInnerRadius) * fScale / fRayleighScaleHeight);
						fMieDensityRatio = (float)Math.Exp(-(fHeight - fInnerRadius) * fScale / fMieScaleHeight);
					}
					else
					{
						if (this.m_currentOpticalBuffer == 1)
						{
							// Smooth the transition from light to shadow (it is a soft shadow after all)
							fRayleighDensityRatio = this.m_opticalDepthBuffer2[nIndex - nSize * m_nChannels] * 0.75f;
							fMieDensityRatio = this.m_opticalDepthBuffer2[nIndex + 2 - nSize * m_nChannels] * 0.75f;
						}
						else
						{
							// Smooth the transition from light to shadow (it is a soft shadow after all)
							fRayleighDensityRatio = this.m_opticalDepthBuffer1[nIndex - nSize * m_nChannels] * 0.75f;
							fMieDensityRatio = this.m_opticalDepthBuffer1[nIndex + 2 - nSize * m_nChannels] * 0.75f;
						}

					}

					// Determine where the ray intersects the outer radius (the top of the atmosphere)
					// This is the end of our ray for determining the optical depth (vPos is the start)
					C = Cpart - fOuterRadius * fOuterRadius;
					fDet = Bsq - 4.0f * C;
					float fFar = 0.5f * (-B + (float)Math.Sqrt(fDet));

					// Next determine the length of each sample, scale the sample ray, and make sure position checks are at the center of a sample ray
					float fSampleLength = fFar / nSamples;
					float fScaledLength = fSampleLength * fScale;
					Vector3 vSampleRay = vRay * fSampleLength;
					vPos += vSampleRay * 0.5f;

					// Iterate through the samples to sum up the optical depth for the distance the ray travels through the atmosphere
					float fRayleighDepth = 0;
					float fMieDepth = 0;
					for (int i = 0; i < nSamples; i++)
					{
						fHeight = vPos.Length();
						float fAltitude = (fHeight - fInnerRadius) * fScale;
						fAltitude = (float)Math.Max(fAltitude, 0.0f);
						fRayleighDepth += (float)Math.Exp(-fAltitude / fRayleighScaleHeight);
						fMieDepth += (float)Math.Exp(-fAltitude / fMieScaleHeight);
						vPos += vSampleRay;
					}

					// Multiply the sums by the length the ray traveled
					fRayleighDepth *= fScaledLength;
					fMieDepth *= fScaledLength;

					// Store the results for Rayleigh to the light source, Rayleigh to the camera, Mie to the light source, and Mie to the camera
					if (this.m_currentOpticalBuffer == 1)
					{
                        this.m_opticalDepthBuffer2[nIndex++] = fRayleighDensityRatio;
                        this.m_opticalDepthBuffer2[nIndex++] = fRayleighDepth;
                        this.m_opticalDepthBuffer2[nIndex++] = fMieDensityRatio;
                        this.m_opticalDepthBuffer2[nIndex++] = fMieDepth;
					}
					else
					{
                        this.m_opticalDepthBuffer1[nIndex++] = fRayleighDensityRatio;
                        this.m_opticalDepthBuffer1[nIndex++] = fRayleighDepth;
                        this.m_opticalDepthBuffer1[nIndex++] = fMieDensityRatio;
                        this.m_opticalDepthBuffer1[nIndex++] = fMieDepth;
					}
				}
			}

			if (this.m_currentOpticalBuffer == 1)
                this.m_currentOpticalBuffer = 2;
			else
                this.m_currentOpticalBuffer = 1;

		}
		int m_currentOpticalBuffer = 2;
		float[] m_opticalDepthBuffer1;
		float[] m_opticalDepthBuffer2;
	}
}
