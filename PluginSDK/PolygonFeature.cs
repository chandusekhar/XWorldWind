using System;
using System.Collections;
using System.Drawing;
using Tao.OpenGl;
using Utility;

namespace WorldWind
{
	/// <summary>
	/// Creates 2D or 3D polygons.  ClampedToGround polygons are drawn as tiled images using ProjectedVectorRenderer.
	/// </summary>
	public class PolygonFeature : RenderableObject
	{        
        CustomVertex.PositionNormalColored[] m_vertices;
		double m_distanceAboveSurface;
		float m_verticalExaggeration = World.Settings.VerticalExaggeration;
		double m_minimumDisplayAltitude;
		double m_maximumDisplayAltitude = double.MaxValue;
		Color m_outlineColor = Color.Black;
		bool m_outline;
        float m_outlineWidth = 1.0f;
		LineFeature[] m_lineFeature;
		AltitudeMode m_altitudeMode = AltitudeMode.ClampedToGround;
		public BoundingBox BoundingBox;
		bool m_extrude;
		bool m_fill = true;
        Vector3 m_localOrigin;
        bool m_extrudeUpwards;
        double m_extrudeHeight=1;
        bool m_extrudeToGround;

        protected Color m_polygonColor = Color.Yellow;
        protected LinearRing m_outerRing;
        protected LinearRing[] m_innerRings;

        /// <summary>
        /// Enables or disables depth buffering - disable if you are having terrain collision rendering issues.
        /// </summary>
        public bool ZBufferEnable
        {
            get { return this.m_ZBufferEnable; }
            set { this.m_ZBufferEnable = value; }
        }
        bool m_ZBufferEnable = true;

        /// <summary>
        /// Whether polygon should be extruded
        /// </summary>
        public bool Extrude
        {
            get { return this.m_extrude; }
            set
            {
                this.m_extrude = value;
                if(this.m_vertices != null) this.UpdateVertices();
            }
        }

        /// <summary>
        /// Whether extrusion should be upwards
        /// </summary>
        public bool ExtrudeUpwards
        {
            get { return this.m_extrudeUpwards; }
            set
            {
                this.m_extrudeUpwards = value;
                if (this.m_vertices != null) this.UpdateVertices();
            }
        }

        /// <summary>
        /// Distance to extrude
        /// </summary>
        public double ExtrudeHeight
        {
            get { return this.m_extrudeHeight; }
            set
            {
                this.m_extrudeHeight = value;
                if (this.m_vertices != null) this.UpdateVertices();
             }
        }

        /// <summary>
        /// Whether polygon should be extruded to the ground (completely overrides other extrusion options)
        /// </summary>
        public bool ExtrudeToGround
        {
            get { return this.m_extrudeToGround; }
            set {
                this.m_extrude = value;
                this.m_extrudeToGround = value;
                if(this.m_vertices != null) this.UpdateVertices();
            }
        }


        public float OutlineWidth
        {
            get { return this.m_outlineWidth; }
            set { this.m_outlineWidth = value; }
        }

		public bool Fill
		{
			get{ return this.m_fill; }
			set
			{
                this.m_fill = value;
				if(this.m_vertices != null) this.UpdateVertices();
			}
		}

		public AltitudeMode AltitudeMode
		{
			get{ return this.m_altitudeMode; }
			set{ this.m_altitudeMode = value; }
		}

		public Color OutlineColor
		{
			get{ return this.m_outlineColor; }
			set
			{
                this.m_outlineColor = value;
				if(this.m_vertices != null)
				{
                    this.UpdateVertices();
				}
			}
		}

		public bool Outline
		{
			get{ return this.m_outline; }
			set
			{
                this.m_outline = value;
				if(this.m_vertices != null)
				{
                    this.UpdateVertices();
				}
			}
		}

		public double DistanceAboveSurface
		{
			get{ return this.m_distanceAboveSurface; }
			set
			{
                this.m_distanceAboveSurface = value; 
				if(this.m_vertices != null) this.UpdateVertices();
			}
		}

		public double MinimumDisplayAltitude
		{
			get{ return this.m_minimumDisplayAltitude; }
			set{ this.m_minimumDisplayAltitude = value; }
		}

		public double MaximumDisplayAltitude
		{
			get{ return this.m_maximumDisplayAltitude; }
			set{ this.m_maximumDisplayAltitude = value; }
		}

		public override byte Opacity
		{
			get
			{
				return base.Opacity;
			}
			set
			{
				base.Opacity = value;
				if(this.m_vertices != null) this.UpdateVertices();
			}
		}

        /// <summary>
        /// Allow runtime updates of outerring
        /// </summary>
        public LinearRing OuterRing
        {
            get { return this.m_outerRing; }
            set{
                //update out ring
                this.m_outerRing = value;
                //update bounding box
                this.CalcBoundingBox();
                //update tessalated polygon
                this.UpdateVertices();
            }
            
        }
        // Public accessor to the geographic coordinates of the bounding box
        public GeographicBoundingBox GeographicBoundingBox
        {
            get { return this.m_geographicBoundingBox; }
        }

        /// <summary>
        /// Polygon Feature Constructor
        /// </summary>
        /// <param name="name">Name of the layer</param>
        /// <param name="parentWorld">Base world</param>
        /// <param name="outerRing">Polygon's outer boundary</param>
        /// <param name="innerRings">Inner Hole's</param>
        /// <param name="polygonColor">Colour of the rendered object</param>
		public PolygonFeature(
			string name, 
			World parentWorld, 
			LinearRing outerRing,
			LinearRing[] innerRings,
			Color polygonColor) : base(name, parentWorld)
		{
            this.RenderPriority = RenderPriority.LinePaths;
            this.m_outerRing = outerRing;
            this.m_innerRings = innerRings;
            this.m_polygonColor = polygonColor;
            this.m_world = parentWorld;

            this.CalcBoundingBox();
		}


        /// <summary>
        /// Ring Polygon Feature Constructor
        /// </summary>
        /// <param name="name">Name of the layer</param>
        /// <param name="parentWorld">Base world</param>
        /// <param name="lat">Lat Center of Ring</param>
        /// <param name="lon">Lon Center of Ring</param>
        /// <param name="alt">Altitude in meters</param>
        /// <param name="radius">Radius of Ring in meters</param>
        /// <param name="numPoints">Number of points desired</param>
        /// <param name="polygonColor">Colour of the rendered object</param>
        public PolygonFeature(
            string name,
            World parentWorld,
            Angle lat,
            Angle lon,
            double alt,
            double radius,
            int numPoints,
            Color polygonColor)
            : base(name, parentWorld)
        {
            this.RenderPriority = RenderPriority.LinePaths;
            this.m_polygonColor = polygonColor;
            this.m_world = parentWorld;

            this.UpdateCircle(lat, lon, alt, radius, numPoints);
        }

        /// <summary>
        /// Used to create or update this polygon as a circle.  Typically called if a new radius is desired.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="radius"></param>
        /// <param name="numPoints"></param>
        /// <param name="polygonColor"></param>
        public virtual void UpdateCircle(
            Angle lat,
            Angle lon,
            double alt,
            double radius,
            int numPoints)
        {
            // Original circle code by ink_polaroid from this thread:
            // http://bbs.keyhole.com/ubb/showflat.php/Cat/0/Number/23634/page/vc/fpart/all/vc/1
            // http://dev.bt23.org/keyhole/circlegen/output.phps

            // build the outer ring
            LinearRing outerRing = new LinearRing();
            Point3d[] points = new Point3d[numPoints];

            double dist_rad = radius / this.m_world.EquatorialRadius;
            Angle lat_rad = Angle.FromDegrees(0);
            Angle lon_rad = Angle.FromDegrees(0);
            double dlon_rad = 0;
            double d_alt = alt;
            double numDegrees = 0;
            numDegrees = 360 / numPoints;
            Angle curr = Angle.FromDegrees(0);
            for (int i = 0; i < numPoints; i++)
            {
                curr.Degrees = (double)i * numDegrees;

                lat_rad.Radians = Math.Asin(Math.Sin(lat.Radians) * Math.Cos(dist_rad) + Math.Cos(lat.Radians) * Math.Sin(dist_rad) * Math.Cos(curr.Radians));
                dlon_rad = Math.Atan2(Math.Sin(curr.Radians) * Math.Sin(dist_rad) * Math.Cos(lat.Radians), Math.Cos(dist_rad) - Math.Sin(lat.Radians) * Math.Sin(lat_rad.Radians));
                lon_rad.Radians = ((lon.Radians + dlon_rad + Math.PI) % (2 * Math.PI)) - Math.PI;

                //This algorithm is limited to distances such that dlon <pi/2
                points[i] = new Point3d(lon_rad.Degrees, lat_rad.Degrees, d_alt);
            }

            outerRing.Points = points;

            this.m_outerRing = outerRing;
            this.m_innerRings = null;

            this.CalcBoundingBox();

            this.isInitialized = false;
        }

		Polygon m_polygon;
		GeographicBoundingBox m_geographicBoundingBox;

        /// <summary>
        /// Method to update polygon bounding box
        /// </summary>
        protected void CalcBoundingBox()
        {
            double minY = double.MaxValue;
			double maxY = double.MinValue;
			double minX = double.MaxValue;
			double maxX = double.MinValue;
			double minZ = double.MaxValue;
			double maxZ = double.MinValue;

            if (this.m_outerRing != null && this.m_outerRing.Points.Length > 0)
            {
                for (int i = 0; i < this.m_outerRing.Points.Length; i++)
                {
                    if (this.m_outerRing.Points[i].X < minX)
                        minX = this.m_outerRing.Points[i].X;
                    if (this.m_outerRing.Points[i].X > maxX)
                        maxX = this.m_outerRing.Points[i].X;

                    if (this.m_outerRing.Points[i].Y < minY)
                        minY = this.m_outerRing.Points[i].Y;
                    if (this.m_outerRing.Points[i].Y > maxY)
                        maxY = this.m_outerRing.Points[i].Y;

                    if (this.m_outerRing.Points[i].Z < minZ)
                        minZ = this.m_outerRing.Points[i].Z;
                    if (this.m_outerRing.Points[i].Z > maxZ)
                        maxZ = this.m_outerRing.Points[i].Z;
                }

                // set a uniform Z for all the points
                for (int i = 0; i < this.m_outerRing.Points.Length; i++)
                {
                    if (this.m_outerRing.Points[i].Z != maxZ) this.m_outerRing.Points[i].Z = maxZ;
                }

                if (this.m_innerRings != null && this.m_innerRings.Length > 0)
                {
                    for (int n = 0; n < this.m_innerRings.Length; n++)
                    {
                        for (int i = 0; i < this.m_innerRings[n].Points.Length; i++)
                        {
                            if (this.m_innerRings[n].Points[i].Z != maxZ) this.m_innerRings[n].Points[i].Z = maxZ;
                        }
                    }
                }
            }

            this.m_geographicBoundingBox = new GeographicBoundingBox(maxY, minY, minX, maxX, minZ, maxZ);

			minZ += this.m_world.EquatorialRadius;
			maxZ += this.m_world.EquatorialRadius;

            this.BoundingBox = new BoundingBox(
				(float)minY, (float)maxY, (float)minX, (float)maxX, (float)minZ, (float)maxZ);
        }

        /// <summary>
        /// Intialize Polygon with Tessalation
        /// </summary>
        /// <param name="drawArgs"></param>
		public override void Initialize(DrawArgs drawArgs)
		{
            this.UpdateVertices();
            this.isInitialized = true;
		}

		ArrayList primList = new ArrayList();
		ArrayList tessList = new ArrayList();
		PrimitiveType m_primitiveType = PrimitiveType.PointList;

		private void f(IntPtr vertexData) 
		{
			try
			{
				double[] v = new double[3];
				System.Runtime.InteropServices.Marshal.Copy(vertexData, v, 0, 3);

				Point3d p = new Point3d(v[0], v[1], 0);
                this.tessList.Add(p);
			} 
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		} 

		private void e() 
		{
            this.finishTesselation((Point3d[]) this.tessList.ToArray(typeof(Point3d)));
		}

		private void b(int which) 
		{
            this.tessList.Clear();
			switch(which)
			{
				case 4:
                    this.m_primitiveType = PrimitiveType.TriangleList;
					break;
				case 5:
                    this.m_primitiveType = PrimitiveType.TriangleStrip;
					break;
				case 6:
                    this.m_primitiveType = PrimitiveType.TriangleFan;
					break;
			}

            this.primTypes.Add(this.m_primitiveType);
		}

		private void r(int which) 
		{
			Log.Write(Log.Levels.Error, "error: " + which.ToString());
		}

		private void getTessellation()
		{
			try
			{
                this.primList.Clear();
                this.primTypes.Clear();

				ArrayList pointList = new ArrayList();
				for(int i = 0; i < this.m_outerRing.Points.Length; i++)
				{
					double[] p = new double[3];
					p[0] = this.m_outerRing.Points[i].X;
					p[1] = this.m_outerRing.Points[i].Y;
					p[2] = this.m_outerRing.Points[i].Z;
					
					pointList.Add(p);
				}

				Glu.GLUtesselator tess = Glu.gluNewTess();
				Glu.gluTessCallback(tess, Glu.GLU_TESS_BEGIN, new Glu.TessBeginCallback(this.b));
				Glu.gluTessCallback(tess, Glu.GLU_TESS_END, new Glu.TessEndCallback(this.e));

				Glu.gluTessCallback(tess, Glu.GLU_TESS_ERROR, new Glu.TessErrorCallback(this.r));
				Glu.gluTessCallback(tess, Glu.GLU_TESS_VERTEX, new Glu.TessVertexCallback(this.f));

				Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
				Glu.gluTessBeginContour(tess);
		
				for(int i = 0; i < pointList.Count - 1; i++)
				{
					double[] p = (double[])pointList[i];
					Glu.gluTessVertex(tess, p, p);
				}
				Glu.gluTessEndContour(tess);
				
				if(this.m_innerRings != null && this.m_innerRings.Length > 0)
				{
					for(int i = 0; i < this.m_innerRings.Length; i++)
					{
						Glu.gluTessBeginContour(tess);
						for(int j = this.m_innerRings[i].Points.Length - 1; j >= 0; j--)
						{
							double[] p = new double[3];
							p[0] = this.m_innerRings[i].Points[j].X;
							p[1] = this.m_innerRings[i].Points[j].Y;
							p[2] = this.m_innerRings[i].Points[j].Z;
							Glu.gluTessVertex(tess, p, p);
						}
						Glu.gluTessEndContour(tess);
					}
				}
				
				Glu.gluTessEndPolygon(tess);
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		ArrayList primTypes = new ArrayList();

		private void finishTesselation(Point3d[] tesselatorList)
		{
            int alpha = (int)(((double) this.m_polygonColor.A/255 * (double)this.Opacity/255)*255);
			int polygonColor = Color.FromArgb(alpha, this.m_polygonColor.R, this.m_polygonColor.G, this.m_polygonColor.B).ToArgb();
			CustomVertex.PositionNormalColored[] vertices = new CustomVertex.PositionNormalColored[tesselatorList.Length];

            // precalculate feature center (at zero elev)
            Point3d center = new Point3d(0, 0, 0);
            for (int i = 0; i < this.m_outerRing.Points.Length; i++)
            {
                center += MathEngine.SphericalToCartesianD(
                    Angle.FromDegrees(this.m_outerRing.Points[i].Y),
                    Angle.FromDegrees(this.m_outerRing.Points[i].X), this.World.EquatorialRadius 
                    );
            }
            center = center * (1.0 / this.m_outerRing.Points.Length);
            // round off to nearest 10^5.
            
            center.X = ((int)(center.X / 10000.0)) * 10000.0;
            center.Y = ((int)(center.Y / 10000.0)) * 10000.0;
            center.Z = ((int)(center.Z / 10000.0)) * 10000.0;

            this.m_localOrigin = center.Vector3;

			for(int i = 0; i < vertices.Length; i++)
			{
                Point3d sphericalPoint = tesselatorList[i];
                //System.Console.WriteLine("Point" + sphericalPoint.X+" "+sphericalPoint.Y+" "+sphericalPoint.Z);
					
				double terrainHeight = 0;
				if(this.m_altitudeMode == AltitudeMode.RelativeToGround)
				{
					if(this.World.TerrainAccessor != null)
					{
						terrainHeight = this.World.TerrainAccessor.GetElevationAt(
							sphericalPoint.Y,
							sphericalPoint.X,
							(100.0 / DrawArgs.Camera.ViewRange.Degrees)
							);
					}
				}

                Point3d xyzPoint = MathEngine.SphericalToCartesianD(
                    Angle.FromDegrees(sphericalPoint.Y),
                    Angle.FromDegrees(sphericalPoint.X), this.World.EquatorialRadius 
                                                         + this.m_verticalExaggeration * 
                                                         (this.m_geographicBoundingBox.MaximumAltitude 
                                                          + this.m_distanceAboveSurface + terrainHeight));

                //Vector3 xyzVector = (xyzPoint + center).Vector3;
                
				vertices[i].Color = polygonColor;
				vertices[i].X = (float)(xyzPoint.X - center.X);
                vertices[i].Y = (float)(xyzPoint.Y - center.Y);
                vertices[i].Z = (float)(xyzPoint.Z - center.Z);
                vertices[i].Nx = 0;
                vertices[i].Ny = 0;
                vertices[i].Nz = 0;
			}

            this.primList.Add(vertices);

            this.ComputeNormals((PrimitiveType) this.primTypes[this.primList.Count - 1], vertices);
		}

        private void ComputeNormals(PrimitiveType primType, CustomVertex.PositionNormalColored[] vertices)
        {
            // normal vector computation: 
            // triangle list: (0 1 2) (3 4 5) (6 7 8) ... (0+3n 1+3n 2+3n)
            // triangle strip: (0 1 2) (1 3 2) (2 3 4) (3 5 4) ... (0+n 1+n 2+n)
            // triangle fan: (0 1 2) (0 2 3) (0 3 4) ... (0 1+n 2+n)
            // to handle it all at once, we use two factors for n: f0 for the first and f12 for the other two
            // notice how triangle strips reverse CW/CCW with each triangle!

            int triCount, f0, f12;
            bool flipflop = false;
            switch(primType)
            {
                case PrimitiveType.TriangleFan:
                    triCount = vertices.Length - 2;
                    f0 = 0;
                    f12 = 1;
                    break;
                case PrimitiveType.TriangleList:
                    triCount = vertices.Length / 3;
                    f0 = f12 = 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    triCount = vertices.Length - 2;
                    f0 = f12 = 1;
                    flipflop = true;
                    break;
                default:
                    // cannot calculate normals for non-polygon features
                    return;
            }

            for (int i = 0; i < triCount; i++)
            {
                // local triangle
                Vector3 a = vertices[0 + i * f0].Position;
                Vector3 b = vertices[1 + i * f12].Position;
                Vector3 c = vertices[2 + i * f12].Position;

                // normal vector
                Vector3 n = Vector3.Normalize(Vector3.Cross(b - a, c - a));
                if (flipflop && ((i & 1) == 1))
                    n = -n;
                vertices[0 + i * f0 ].Normal += n;
                vertices[1 + i * f12].Normal += n;
                vertices[2 + i * f12].Normal += n;
            }

            // all done, re-normalize (required for strips and fans only, but we do it in any case)
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
        }

        protected void UpdateVertices()
		{
            this.m_verticalExaggeration = World.Settings.VerticalExaggeration;

			if(this.m_altitudeMode == AltitudeMode.ClampedToGround)
			{
				if(this.m_polygon != null)
				{
                    this.m_polygon.Remove = true;
                    this.m_polygon = null;
				}

                this.m_polygon = new Polygon();
                this.m_polygon.outerBoundary = this.m_outerRing;
                this.m_polygon.innerBoundaries = this.m_innerRings;
                this.m_polygon.PolgonColor = this.m_polygonColor;
                this.m_polygon.Fill = this.m_fill;
                this.m_polygon.ParentRenderable = this;
                this.m_polygon.LineWidth = this.m_outlineWidth;
                this.m_polygon.Outline = this.m_outline;
                this.m_polygon.OutlineColor = this.m_outlineColor;
				this.World.ProjectedVectorRenderer.Add(this.m_polygon);

				if(this.m_vertices != null) this.m_vertices = null;
				
				if(this.m_lineFeature != null)
				{
                    for (int i = 0; i < this.m_lineFeature.Length; i++) this.m_lineFeature[i].Dispose();

                    this.m_lineFeature = null;
				}
				
				return;
			}

            this.getTessellation();
			
			if(this.m_extrude || this.m_outline)
			{
                if (this.m_lineFeature != null)
                {
                    for (int i = 0; i < this.m_lineFeature.Length; i++)
                    {
                        this.m_lineFeature[i].Dispose();
                    }

                    this.m_lineFeature = null;
                }

                this.m_lineFeature = new LineFeature[1 + (this.m_innerRings != null && this.m_innerRings.Length > 0 ? this.m_innerRings.Length : 0)];

				Point3d[] linePoints = new Point3d[this.m_outerRing.Points.Length + 1];
				for(int i = 0; i < this.m_outerRing.Points.Length; i++)
				{
					linePoints[i] = this.m_outerRing.Points[i];
				}

				linePoints[linePoints.Length - 1] = this.m_outerRing.Points[0];

                this.m_lineFeature[0] = new LineFeature(this.Name, this.World, linePoints, this.m_polygonColor);
                this.m_lineFeature[0].DistanceAboveSurface = this.m_distanceAboveSurface;
                this.m_lineFeature[0].MinimumDisplayAltitude = this.m_minimumDisplayAltitude;
                this.m_lineFeature[0].MaximumDisplayAltitude = this.m_maximumDisplayAltitude;
                this.m_lineFeature[0].AltitudeMode = this.AltitudeMode;
                this.m_lineFeature[0].Opacity = this.Opacity;
                this.m_lineFeature[0].Outline = this.m_outline;
                this.m_lineFeature[0].LineColor = this.m_outlineColor;
                this.m_lineFeature[0].Opacity = this.Opacity;
                this.m_lineFeature[0].Extrude = this.m_extrude;
                if (this.m_extrude || this.m_extrudeToGround)
                {
                    this.m_lineFeature[0].ExtrudeUpwards = this.m_extrudeUpwards;
                    this.m_lineFeature[0].ExtrudeHeight = this.m_extrudeHeight;
                    this.m_lineFeature[0].ExtrudeToGround = this.m_extrudeToGround;
                    this.m_lineFeature[0].PolygonColor = this.m_outlineColor;
                }

				if(this.m_innerRings != null && this.m_innerRings.Length > 0)
				{
					for(int i = 0; i < this.m_innerRings.Length; i++)
					{
						Point3d[] innerPoints = new Point3d[this.m_innerRings[i].Points.Length + 1];
						for(int j = 0; j < this.m_innerRings[i].Points.Length; j++)
						{
							innerPoints[j] = this.m_innerRings[i].Points[j];
						}

						innerPoints[innerPoints.Length - 1] = this.m_innerRings[i].Points[0];

                        this.m_lineFeature[1 + i] = new LineFeature(this.Name, this.World, innerPoints, this.m_polygonColor);
                        this.m_lineFeature[1 + i].DistanceAboveSurface = this.m_distanceAboveSurface;
                        this.m_lineFeature[1 + i].MinimumDisplayAltitude = this.m_minimumDisplayAltitude;
                        this.m_lineFeature[1 + i].MaximumDisplayAltitude = this.m_maximumDisplayAltitude;
                        this.m_lineFeature[1 + i].AltitudeMode = this.AltitudeMode;
                        this.m_lineFeature[1 + i].Opacity = this.Opacity;
                        this.m_lineFeature[1 + i].Outline = this.m_outline;
                        this.m_lineFeature[1 + i].LineColor = this.m_outlineColor;
                        this.m_lineFeature[1 + i].Opacity = this.Opacity;
                        this.m_lineFeature[1 + i].Extrude = this.m_extrude;
                        if (this.m_extrude || this.m_extrudeToGround)
                        {
                            this.m_lineFeature[1+i].ExtrudeUpwards = this.m_extrudeUpwards;
                            this.m_lineFeature[1 + i].ExtrudeHeight = this.m_extrudeHeight;
                            this.m_lineFeature[1 + i].ExtrudeToGround = this.m_extrudeToGround;
                            this.m_lineFeature[1 + i].PolygonColor = this.m_outlineColor;
                        }
					}
				}
			}
			else
			{
				if(this.m_lineFeature != null && this.m_lineFeature.Length > 0)
				{
					for(int i = 0; i < this.m_lineFeature.Length; i++)
					{
						if(this.m_lineFeature[i] != null)
						{
                            this.m_lineFeature[i].Dispose();
                            this.m_lineFeature[i] = null;
						}	
					}

                    this.m_lineFeature = null;
				}
			}
		}

		public override void Dispose()
		{
			if(this.m_polygon != null)
			{
                this.m_polygon.Remove = true;
                this.m_polygon = null;
			}

			if(this.m_lineFeature != null)
			{
				for(int i = 0; i < this.m_lineFeature.Length; i++)
				{
					if(this.m_lineFeature[i] != null) this.m_lineFeature[i].Dispose();
				}

                this.m_lineFeature = null;
			}
		}

        //store reference centre
        private Point3d referenceCentre = new Point3d();

		public override void Update(DrawArgs drawArgs)
		{
			try
			{
                this.referenceCentre = drawArgs.WorldCamera.ReferenceCenter;
                if ((this.m_extrude || this.m_outline) && this.m_lineFeature == null) this.UpdateVertices();

                if (drawArgs.WorldCamera.Altitude >= this.m_minimumDisplayAltitude && drawArgs.WorldCamera.Altitude <= this.m_maximumDisplayAltitude)
				{
					if(!this.isInitialized) this.Initialize(drawArgs);

					if(this.m_verticalExaggeration != World.Settings.VerticalExaggeration || (this.m_lineFeature == null && this.m_extrude))
					{
                        this.UpdateVertices();
					}

					if(this.m_lineFeature != null)
					{
						for(int i = 0; i < this.m_lineFeature.Length; i++)
						{
							if(this.m_lineFeature[i] != null) this.m_lineFeature[i].Update(drawArgs);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		public override void Render(DrawArgs drawArgs)
		{
            using (new DirectXProfilerEvent("PolygonFeature::Render"))
            {
                if (!this.isInitialized /*|| m_vertices == null*/ || drawArgs.WorldCamera.Altitude < this.m_minimumDisplayAltitude || drawArgs.WorldCamera.Altitude > this.m_maximumDisplayAltitude)
                {
                    return;
                }

                if (!drawArgs.WorldCamera.ViewFrustum.Intersects(this.BoundingBox))
                    return;

                // save state
                Cull currentCull = drawArgs.device.SetRenderState(RenderState.CullMode;
                bool currentZBufferEnable = drawArgs.device.SetRenderState(RenderState.ZBufferEnable;

                try
                {
                    drawArgs.device.SetRenderState(RenderState.CullMode = Cull.None;
                    drawArgs.device.SetRenderState(RenderState.ZBufferEnable = this.m_ZBufferEnable;

                    drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
                        (float)-drawArgs.WorldCamera.ReferenceCenter.X + this.m_localOrigin.X,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Y + this.m_localOrigin.Y,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Z + this.m_localOrigin.Z
                        );

                    if (World.Settings.EnableSunShading)
                    {
                        Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                        Vector3 sunVector = new Vector3(
                            (float)sunPosition.X,
                            (float)sunPosition.Y,
                            (float)sunPosition.Z);

                        drawArgs.device.SetRenderState(RenderState.Lighting = true;
                        Material material = new Material();
                        material.Diffuse = Color.White;
                        material.Ambient = Color.White;

                        drawArgs.device.Material = material;
                        drawArgs.device.SetRenderState(RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                        drawArgs.device.SetRenderState(RenderState.NormalizeNormals = true;
                        drawArgs.device.SetRenderState(RenderState.AlphaBlendEnable = true;

                        drawArgs.device.Lights[0].Enabled = true;
                        drawArgs.device.Lights[0].Type = LightType.Directional;
                        drawArgs.device.Lights[0].Diffuse = Color.White;
                        drawArgs.device.Lights[0].Direction = sunVector;

                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.Modulate;
                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse;
                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg2,TextureArgument.TextureColor;
                    }
                    else
                    {
                        drawArgs.device.SetRenderState(RenderState.Lighting = false;
                        drawArgs.device.SetRenderState(RenderState.Ambient = World.Settings.StandardAmbientColor;
                    }

                    //if(m_vertices != null)
                    if (this.Fill)
                    {
                        if (this.primList.Count > 0)
                        {
                            drawArgs.device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.Disable;
                            for (int i = 0; i < this.primList.Count; i++)
                            {
                                int vertexCount = 0;
                                PrimitiveType primType = (PrimitiveType) this.primTypes[i];
                                CustomVertex.PositionNormalColored[] vertices = (CustomVertex.PositionNormalColored[]) this.primList[i];

                                if (primType == PrimitiveType.TriangleList)
                                    vertexCount = vertices.Length / 3;
                                else
                                    vertexCount = vertices.Length - 2;

                                drawArgs.device.DrawUserPrimitives(
                                    primType,//PrimitiveType.TriangleList, 
                                    vertexCount,
                                    vertices);
                            }
                        }
                    }

                    if (this.m_lineFeature != null)
                    {
                        for (int i = 0; i < this.m_lineFeature.Length; i++)
                        {
                            if (this.m_lineFeature[i] != null) this.m_lineFeature[i].Render(drawArgs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
                finally
                {
                    // restore device state
                    drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;
                    drawArgs.device.SetRenderState(RenderState.CullMode = currentCull;
                    drawArgs.device.SetRenderState(RenderState.ZBufferEnable = currentZBufferEnable;
                }
            }
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
            Point currentPosition = DrawArgs.LastMousePosition;
            Angle Latitude, Longitude;
            drawArgs.WorldCamera.PickingRayIntersection(
                currentPosition.X,
                currentPosition.Y,
                out Latitude,
                out Longitude);
            Point3d queryPoint = new Point3d(Longitude.Degrees, Latitude.Degrees, 0);

            if (!this.GeographicBoundingBox.Contains(queryPoint))
                return false;
            //TODO: Test holes/inner rings(if any) if point is in hole return false
            if (this.m_innerRings != null)
            {
                foreach (LinearRing ring in this.m_innerRings)
                {
                    bool isIn = this.pointInRing(ring.Points, queryPoint);
                    if (isIn)
                        return false;
                }
            }

            //TODO: Test outer ring
            return this.pointInRing(this.OuterRing.Points, queryPoint);
		}

        /// <summary>
        /// Utility method to check if point is in a simple polygon
        /// Uses Winding number/Jordan Curve Theorem
        /// http://en.wikipedia.org/wiki/Jordan_curve_theorem
        /// </summary>
        /// <param name="points">Polygon ring points</param>
        /// <param name="queryPoint">Test point</param>
        /// <returns>Location status</returns>
        private bool pointInRing(Point3d[] points, Point3d queryPoint)
        {
            bool isIn = false;
            int i, j = 0;
            for (i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                if ((((points[i].Y <= queryPoint.Y) && (queryPoint.Y < points[j].Y)) || ((points[j].Y <= queryPoint.Y)
                    && (queryPoint.Y < points[i].Y))) && (queryPoint.X < (points[j].X - points[i].X)
                    * (queryPoint.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    isIn = !isIn;
                }
            }

            return isIn;
        }
	}
}
