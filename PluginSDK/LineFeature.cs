using System;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;
using WorldWind.Extensions;
using Color = System.Drawing.Color;

namespace WorldWind
{
    /// <summary>
    /// Summary description for LineFeature.
    /// </summary>
    public class LineFeature : RenderableObject
    {
        #region Static Members
        #endregion

        #region Private Members
        double m_distanceAboveSurface;
        protected LinkedList<Point3d> m_points = new LinkedList<Point3d>();
        CustomVertex.PositionNormalTextured[] m_wallVertices;

        CustomVertex.PositionColored[] m_colorWallVertices;

        CustomVertex.PositionColored[] m_topVertices;
        CustomVertex.PositionColored[] m_bottomVertices;
        CustomVertex.PositionColored[] m_sideVertices;

        Color m_lineColor = Color.Black;
        Color finalLineColor = Color.Black;
        float m_verticalExaggeration = World.Settings.VerticalExaggeration;
        double m_minimumDisplayAltitude;
        double m_maximumDisplayAltitude = double.MaxValue;
        string m_imageUri;
        Texture m_texture = null;
        Color m_polygonColor = Color.Black;
        Color finalPolygonColor = Color.Black;
        bool m_outline = true;
        float m_lineWidth = 1.0f;
        bool m_extrude;
        AltitudeMode m_altitudeMode = AltitudeMode.ClampedToGround;
        //protected long m_numPoints = 0;
        Vector3 m_localOrigin;
        bool m_extrudeUpwards;
        double m_extrudeHeight = 1;
        bool m_extrudeToGround;
	    bool m_enableLighting;

        long m_maxPoints = -1;

        #endregion

        #region Accessors
        /// <summary>
        /// Boolean indicating whether or not the line needs rebuilding.
        /// </summary>
        public bool NeedsUpdate = true;

        /// <summary>
        /// Whether line should be extruded
        /// </summary>
        public bool Extrude
        {
            get { return this.m_extrude; }
            set
            {
                this.m_extrude = value;
                if (this.m_topVertices != null) this.NeedsUpdate = true;
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
                if (this.m_topVertices != null) this.NeedsUpdate = true;
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
                if (this.m_topVertices != null) this.NeedsUpdate = true;
            }
        }

        /// <summary>
        /// Whether line should be extruded to the ground 
        /// (completely overrides other extrusion options)
        /// </summary>
        public bool ExtrudeToGround
        {
            get { return this.m_extrudeToGround; }
            set
            {
                this.m_extrude = value;
                this.m_extrudeToGround = value;
                if (this.m_topVertices != null) this.NeedsUpdate = true;
            }
        }

        public AltitudeMode AltitudeMode
        {
            get { return this.m_altitudeMode; }
            set { this.m_altitudeMode = value; }
        }

        public bool EnableLighting
        {
            get { return this.m_enableLighting; }
            set { this.m_enableLighting = value; }
        }

        public Color LineColor
        {
            get { return this.m_lineColor; }
            set
            {
                this.m_lineColor = value;
                this.NeedsUpdate = true;
            }
        }

        public float LineWidth
        {
            get { return this.m_lineWidth; }
            set
            {
                this.m_lineWidth = value;
                this.NeedsUpdate = true;
            }
        }

        public double DistanceAboveSurface
        {
            get { return this.m_distanceAboveSurface; }
            set
            {
                this.m_distanceAboveSurface = value;
                if (this.m_topVertices != null)
                {
                    this.NeedsUpdate = true;
                }
            }
        }

        public Color PolygonColor
        {
            get { return this.m_polygonColor; }
            set
            {
                this.m_polygonColor = value;
                if (this.m_topVertices != null)
                {
                    this.NeedsUpdate = true;
                }
            }
        }

        public bool Outline
        {
            get { return this.m_outline; }
            set
            {
                this.m_outline = value;
                if (this.m_topVertices != null)
                {
                    this.NeedsUpdate = true;
                }
            }
        }

        public LinkedList<Point3d> PointsList
        {
            get { return this.m_points; }
            set 
            {
                this.m_points = value;
                this.NeedsUpdate = true;
            }
        }

        public Point3d[] Points
        {
            get 
            {
                Point3d[] points = new Point3d[this.m_points.Count];
                this.m_points.CopyTo(points, 0);
                return points;
            }
            set
            {
                this.m_points.Clear();
                if (value != null)
                {
                    foreach (Point3d point in value) this.m_points.AddLast(point);
                }

                this.NeedsUpdate = true;
            }
        }

        /// <summary>
        /// Sets the maximum number of points to grow this line.  
        /// If less than or equal to 0 then unlimited.
        /// </summary>
        public long MaxPoints
        {
            get { return this.m_maxPoints; }
            set 
            {
                this.m_maxPoints = value;
                this.NeedsUpdate = true;
            }
        }

        public long NumPoints
        {
            get { return this.m_points.Count; }
        }

        public double MinimumDisplayAltitude
        {
            get { return this.m_minimumDisplayAltitude; }
            set { this.m_minimumDisplayAltitude = value; }
        }

        public double MaximumDisplayAltitude
        {
            get { return this.m_maximumDisplayAltitude; }
            set { this.m_maximumDisplayAltitude = value; }
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
                if (this.m_topVertices != null)
                {
                    this.NeedsUpdate = true;
                }
            }
        }

        /// <summary>
        /// Uri of image to paint on 'wall', changing/updating requires re-initializing
        /// </summary>
        public string ImageUri
        {
            get
            {
                return this.m_imageUri;
            }
            set
            {
                this.m_imageUri = value;
            }
        }
        #endregion

        public LineFeature(string name, World parentWorld, Point3d[] points, Color lineColor)
            : base(name, parentWorld)
        {
            foreach (Point3d point in points)
            {
                this.m_points.AddLast(point);
            }

            this.m_lineColor = lineColor;
            this.m_polygonColor = lineColor;

            this.RenderPriority = RenderPriority.LinePaths;
        }

        public LineFeature(string name, World parentWorld, Point3d[] points, string imageUri)
            : base(name, parentWorld)
        {
            this.Points = points;
            this.m_imageUri = imageUri;

            this.RenderPriority = RenderPriority.LinePaths;
        }

        public override void Dispose()
        {
            if (this.m_texture != null && !this.m_texture.IsDisposed)
            {
                this.m_texture.Dispose();
                this.m_texture = null;
            }

            if (this.m_lineString != null)
            {
                this.m_lineString.Remove = true;
                this.m_lineString = null;
            }

            this.NeedsUpdate = true;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (this.m_points == null)
            {
                this.isInitialized = true;
                return;
            }

            if (this.m_imageUri != null)
            {
                //load image
                if (this.m_imageUri.ToLower().StartsWith("http://"))
                {
                    string savePath = string.Format("{0}\\image", ConfigurationLoader.GetRenderablePathString(this));
                    System.IO.FileInfo file = new System.IO.FileInfo(savePath);

                    if (!file.Exists)
                    {
                        //Offline check
                        if (!World.Settings.WorkOffline)
                        {
                            Net.WebDownload download = new Net.WebDownload(this.m_imageUri);

                            if (!file.Directory.Exists)
                                file.Directory.Create();

                            download.DownloadFile(file.FullName, Net.DownloadType.Unspecified);
                        }
                    }

                    //file might not have downloaded.  Especially if we are offline
                    if (!file.Exists)
                    {
                        this.m_texture = ImageHelper.LoadTexture(file.FullName);
                    }
                    else
                    {
                        this.m_texture = null;
                    }                    
                }
                else
                {
                    this.m_texture = ImageHelper.LoadTexture(this.m_imageUri);
                }
            }

            this.UpdateVertices();

            this.isInitialized = true;
        }

        public void Clear()
        {
            this.m_points.Clear();
            this.NeedsUpdate = true;
        }

        /// <summary>
        /// Adds a point to the end of the line.
        /// </summary>
        /// <param name="x">Lon</param>
        /// <param name="y">Lat</param>
        /// <param name="z">Alt (meters)</param>
        public void AddPoint(double x, double y, double z)
        {
            Point3d point = new Point3d(x, y, z);
            //TODO:Divide into subsegments if too far
            if (this.m_points.Count > 0)
            {
                Angle startlon = Angle.FromDegrees(this.m_points.Last.Value.X);
                Angle startlat = Angle.FromDegrees(this.m_points.Last.Value.Y);
                double startalt = this.m_points.Last.Value.Z;
                Angle endlon = Angle.FromDegrees(x);
                Angle endlat = Angle.FromDegrees(y);
                double endalt = z;

                Angle dist = World.ApproxAngularDistance(startlat, startlon, endlat, endlon);
                if (dist.Degrees > 0.25)
                {

                    double stepSize = 0.25;
                    int samples = (int)(dist.Degrees / stepSize);

                    for (int i = 0; i < samples; i++)
                    {
                        Angle lat, lon = Angle.Zero;
                        float frac = (float)i / samples;
                        World.IntermediateGCPoint(frac, startlat, startlon, endlat, endlon,
                        dist, out lat, out lon);
                        double alt = startalt + frac * (endalt - startalt);
                        Point3d pointint = new Point3d(lon.Degrees, lat.Degrees, alt);
                        this.AddPoint(pointint);
                    }

                    this.AddPoint(point);
                }
                else
                {
                    this.AddPoint(point);
                }
            }
            else
            {
                this.AddPoint(point);
            }

            this.NeedsUpdate = true;
        }

        /// <summary>
        /// Adds a point to the line at the end of the line.
        /// </summary>
        /// <param name="point">The Point3d object to add.</param>
        public void AddPoint(Point3d point)
        {
            this.m_points.AddLast(point);

            if ((this.m_maxPoints > 0) && (this.m_points.Count > this.m_maxPoints)) this.m_points.RemoveFirst();

            this.NeedsUpdate = true;
        }

        public void RemovePoint(Point3d point)
        {
            LinkedList<Point3d> tempList = new LinkedList<Point3d>();
            foreach (Point3d p in this.m_points)
            {
                if (!p.Equals(point))
                    tempList.AddLast(p);
            }

            this.m_points = tempList;

            this.NeedsUpdate = true;
        }

        /// <summary>
        /// Updates a point if it exists within a LineFeature with a newpoint
        /// </summary>
        /// <param name="oldpoint"></param>
        /// <param name="newpoint"></param>
        public void UpdatePoint(Point3d oldpoint, Point3d newpoint)
        {
            foreach (Point3d p in this.PointsList)
            {
                if (p.Equals(oldpoint))
                {
                    p.Y = newpoint.Y;
                    p.X = newpoint.X;
                    p.Z = newpoint.Z;

                    this.NeedsUpdate = true;

                    break;
                }
            }
        }

        private void UpdateVertices()
        {
            int lineAlpha = (int)(((double) this.m_lineColor.A / 255 * (double)base.Opacity / 255) * 255);
            this.finalLineColor = Color.FromArgb(lineAlpha, this.m_lineColor);
            int polyAlpha = (int)(((double) this.m_polygonColor.A/255*(double)base.Opacity/255)*255);
            this.finalPolygonColor = Color.FromArgb(polyAlpha, this.m_polygonColor);

            try
            {
                this.m_verticalExaggeration = World.Settings.VerticalExaggeration;

                if (this.m_points.Count > 0)
                {
                    this.UpdateTexturedVertices();
                }

                if (this.m_lineString != null && this.m_outline && this.m_wallVertices != null && this.m_wallVertices.Length > this.m_topVertices.Length)
                {
                    this.UpdateOutlineVertices();
                }

                this.NeedsUpdate = false;
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
        }

        private void UpdateOutlineVertices()
        {
            this.m_bottomVertices = new CustomVertex.PositionColored[this.m_points.Count];
            this.m_sideVertices = new CustomVertex.PositionColored[this.m_points.Count * 2];

            for (int i = 0; i < this.m_points.Count; i++)
            {
                this.m_sideVertices[2 * i] = this.m_topVertices[i];

                Vector3 xyzVertex = new Vector3(this.m_wallVertices[2 * i + 1].X, this.m_wallVertices[2 * i + 1].Y, this.m_wallVertices[2 * i + 1].Z);

                this.m_bottomVertices[i].X = xyzVertex.X;
                this.m_bottomVertices[i].Y = xyzVertex.Y;
                this.m_bottomVertices[i].Z = xyzVertex.Z;
                this.m_bottomVertices[i].Color = this.finalLineColor.ToArgb();

                this.m_sideVertices[2 * i + 1] = this.m_bottomVertices[i];
            }
        }

        LineString m_lineString;
        private void UpdateTexturedVertices()
        {
            if (this.m_altitudeMode == AltitudeMode.ClampedToGround)
            {
                if (this.m_lineString != null)
                {
                    this.m_lineString.Remove = true;
                    this.m_lineString = null;
                }

                this.m_lineString = new LineString();
                this.m_lineString.Coordinates = this.Points;
                this.m_lineString.Color = this.finalLineColor;
                this.m_lineString.LineWidth = this.LineWidth;
                this.m_lineString.ParentRenderable = this;
                this.World.ProjectedVectorRenderer.Add(this.m_lineString);

                if (this.m_wallVertices != null) this.m_wallVertices = null;

                if (this.m_colorWallVertices != null) this.m_colorWallVertices = null;

                return;
            }

            // wall replicates vertices to account for different normal vectors.
            // TODO: would be nice to automatically figure out when NOT to separate
            // triangles (smooth shading) vs. having separate vertices (and thus normals)
            // ('hard' edges). GE doesn't do that, by the way.
            // -stepman
            if (this.m_extrude || this.m_altitudeMode != AltitudeMode.ClampedToGround)
            {
                this.m_wallVertices = new CustomVertex.PositionNormalTextured[this.m_points.Count * 4 - 2];

                this.m_colorWallVertices = new CustomVertex.PositionColored[this.m_points.Count * 4 - 2];
            }

            float textureCoordIncrement = 1.0f / (float)(this.m_points.Count - 1);
            this.m_verticalExaggeration = World.Settings.VerticalExaggeration;
            int vertexColor = this.finalPolygonColor.ToArgb();

            this.m_topVertices = new CustomVertex.PositionColored[this.m_points.Count];

            // precalculate feature center (at zero elev)
            Point3d center = new Point3d(0, 0, 0);


            foreach (Point3d point in this.m_points)
            {
                center += MathEngine.SphericalToCartesianD(
                    Angle.FromDegrees(point.Y),
                    Angle.FromDegrees(point.X), this.World.EquatorialRadius
                    );
            }
            center = center * (1.0 / this.m_points.Count);
            // round off to nearest 10^5.
            center.X = ((int)(center.X / 10000.0)) * 10000.0;
            center.Y = ((int)(center.Y / 10000.0)) * 10000.0;
            center.Z = ((int)(center.Z / 10000.0)) * 10000.0;

            this.m_localOrigin = center.Vector3;

            long i = 0;

            try
            {
                foreach (Point3d point in this.m_points)
                {
                    double terrainHeight = 0;
                    if (this.m_altitudeMode == AltitudeMode.RelativeToGround)
                    {
                        if (this.World.TerrainAccessor != null)
                        {
                            terrainHeight = this.World.TerrainAccessor.GetElevationAt(
                                point.Y,
                                point.X,
                                (100.0 / DrawArgs.Camera.ViewRange.Degrees)
                                );
                        }
                    }

                    // polygon point
                    Point3d xyzPoint = MathEngine.SphericalToCartesianD(
                        Angle.FromDegrees(point.Y),
                        Angle.FromDegrees(point.X), this.m_verticalExaggeration * (this.m_distanceAboveSurface + terrainHeight + point.Z) + this.World.EquatorialRadius
                        );

                    Vector3 xyzVertex = (xyzPoint - center).Vector3;

                    this.m_topVertices[i].X = xyzVertex.X;
                    this.m_topVertices[i].Y = xyzVertex.Y;
                    this.m_topVertices[i].Z = xyzVertex.Z;
                    this.m_topVertices[i].Color = this.finalLineColor.ToArgb();

                    if (this.m_extrude || this.m_altitudeMode != AltitudeMode.ClampedToGround)
                    {
                        this.m_wallVertices[4 * i].X = xyzVertex.X;
                        this.m_wallVertices[4 * i].Y = xyzVertex.Y;
                        this.m_wallVertices[4 * i].Z = xyzVertex.Z;
                        this.m_wallVertices[4 * i].Tu = i * textureCoordIncrement;
                        this.m_wallVertices[4 * i].Tv = 1.0f;

                        this.m_wallVertices[4 * i].Normal = new Vector3(0, 0, 0);

                        this.m_colorWallVertices[4 * i].X = xyzVertex.X;
                        this.m_colorWallVertices[4 * i].Y = xyzVertex.Y;
                        this.m_colorWallVertices[4 * i].Z = xyzVertex.Z;
                        this.m_colorWallVertices[4 * i].Color = vertexColor;

                        // extruded point
                        if (this.m_extrudeToGround)
                        {
                            xyzPoint = MathEngine.SphericalToCartesianD(
                                Angle.FromDegrees(point.Y),
                                Angle.FromDegrees(point.X), this.m_verticalExaggeration * (this.m_distanceAboveSurface + terrainHeight) + this.World.EquatorialRadius
                                );
                        }
                        else
                        {
                            double extrudeDist = this.m_extrudeHeight;
                            if (!this.m_extrudeUpwards)
                                extrudeDist *= -1;

                            xyzPoint = MathEngine.SphericalToCartesianD(
                                Angle.FromDegrees(point.Y),
                                Angle.FromDegrees(point.X), this.m_verticalExaggeration * (this.m_distanceAboveSurface + terrainHeight + extrudeDist + point.Z) + this.World.EquatorialRadius
                                );
                        }

                        xyzVertex = (xyzPoint - center).Vector3;


                        this.m_wallVertices[4 * i + 1].X = xyzVertex.X;
                        this.m_wallVertices[4 * i + 1].Y = xyzVertex.Y;
                        this.m_wallVertices[4 * i + 1].Z = xyzVertex.Z;
                        //m_wallVertices[4 * i + 1].Color = vertexColor;
                        this.m_wallVertices[4 * i + 1].Tu = i * textureCoordIncrement;
                        this.m_wallVertices[4 * i + 1].Tv = 0.0f;

                        this.m_wallVertices[4 * i + 1].Normal = new Vector3(0, 0, 0);

                        this.m_colorWallVertices[4 * i + 1].X = xyzVertex.X;
                        this.m_colorWallVertices[4 * i + 1].Y = xyzVertex.Y;
                        this.m_colorWallVertices[4 * i + 1].Z = xyzVertex.Z;
                        this.m_colorWallVertices[4 * i + 1].Color = vertexColor;

                        // replicate to previous vertex as well
                        if (i > 0)
                        {
                            this.m_wallVertices[4 * i - 2] = this.m_wallVertices[4 * i];
                            this.m_wallVertices[4 * i - 1] = this.m_wallVertices[4 * i + 1];

                            this.m_colorWallVertices[4 * i - 2] = this.m_colorWallVertices[4 * i];
                            this.m_colorWallVertices[4 * i - 1] = this.m_colorWallVertices[4 * i + 1];
                        }
                    }
                    i++;
                }
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
            }

            // calculate normals
            // -----------------
            // first pass -- accumulate normals
            // we start with the second index because we're looking at (i-1) to calculate normals
            // we also use the same normal for top and bottom vertices...
            for (i = 1; i < this.m_points.Count && (4*i-1) < this.m_wallVertices.Length; i++)
            {
                Vector3 dir = this.m_wallVertices[4 * i - 2].Position - this.m_wallVertices[4 * i - 3].Position;
                Vector3 up = this.m_wallVertices[4 * i - 1].Position - this.m_wallVertices[4 * i - 2].Position;
                Vector3 norm = Vector3.Normalize(Vector3.Cross(dir, up));
                this.m_wallVertices[4 * i - 4].Normal += norm;
                this.m_wallVertices[4 * i - 3].Normal += norm;
                this.m_wallVertices[4 * i - 2].Normal += norm;
                this.m_wallVertices[4 * i - 1].Normal += norm;
            }
            // normalize normal vectors.
            if (this.m_points.Count * 4 <= this.m_wallVertices.Length)
            {
                for (i = 0; i < this.m_points.Count; i++)
                {
                    this.m_wallVertices[4 * i + 0].Normal.Normalize();
                    this.m_wallVertices[4 * i + 1].Normal.Normalize();
                    this.m_wallVertices[4 * i + 2].Normal.Normalize();
                    this.m_wallVertices[4 * i + 3].Normal.Normalize();
                }
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (drawArgs.WorldCamera.Altitude >= this.m_minimumDisplayAltitude && drawArgs.WorldCamera.Altitude <= this.m_maximumDisplayAltitude)
            {
                if (!this.isInitialized) this.Initialize(drawArgs);

                if (this.NeedsUpdate || (this.m_verticalExaggeration != World.Settings.VerticalExaggeration)) this.UpdateVertices();
            }

        }

        public override void Render(DrawArgs drawArgs)
        {
            using (new DirectXProfilerEvent("LineFeature::Render"))
            {
                if (!this.isInitialized || drawArgs.WorldCamera.Altitude < this.m_minimumDisplayAltitude || drawArgs.WorldCamera.Altitude > this.m_maximumDisplayAltitude)
                {
                    return;
                }

                try
                {
                    if (this.m_lineString != null)
                        return;

                    Cull currentCull = drawArgs.device.GetRenderState<Cull>(RenderState.CullMode);
                    drawArgs.device.SetRenderState(RenderState.CullMode , Cull.None);

                    bool currentAlpha = drawArgs.device.GetRenderState<bool>(RenderState.AlphaBlendEnable);
                    drawArgs.device.SetRenderState(RenderState.AlphaBlendEnable , true);

                    drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
                        (float)-drawArgs.WorldCamera.ReferenceCenter.X + this.m_localOrigin.X,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Y + this.m_localOrigin.Y,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Z + this.m_localOrigin.Z
                        ));

                    //Fix for sunshading screwing with everything
                    bool lighting = drawArgs.device.GetRenderState<bool>(RenderState.Lighting);
                    drawArgs.device.SetRenderState(RenderState.Lighting , this.m_enableLighting);

                    if (this.m_wallVertices != null)
                    {
                        drawArgs.device.SetRenderState(RenderState.ZEnable , true);

                        if (this.m_texture != null && !this.m_texture.IsDisposed)
                        {
                            drawArgs.device.SetTexture(0, this.m_texture);
                            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.Modulate);
                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Add);
                            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.Texture);
                        }
                        else
                        {
                            // drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);

                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.SelectArg1);

                            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.Diffuse);
                            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.SelectArg1);
                        }

                        Material mat = new Material();
                        mat.Diffuse = mat.Ambient = this.finalPolygonColor.ToRawColor4();; // this.m_polygonColor;

                        drawArgs.device.Material = mat;

                        if (this.m_texture != null && !this.m_texture.IsDisposed)
                        {
                            drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, this.m_wallVertices.Length - 2, this.m_wallVertices);
                        }
                        else
                        {
                            drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
                            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, this.m_colorWallVertices.Length - 2, this.m_colorWallVertices);
                        }

                        if (this.m_outline)
                        {

                            // drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);

                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.SelectArg1);

                            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.Diffuse);
                            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.SelectArg1);

                            drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
                            drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, this.m_topVertices.Length - 1, this.m_topVertices);

                            if (this.m_bottomVertices != null)
                                drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, this.m_bottomVertices.Length - 1, this.m_bottomVertices);

                            if (this.m_sideVertices != null)
                                drawArgs.device.DrawUserPrimitives(PrimitiveType.LineList, this.m_sideVertices.Length / 2, this.m_sideVertices);

                        }
                    }
                    else
                    {
                        // drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.SelectArg1);

                        drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.Diffuse);
                        drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.SelectArg1);

                        drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
                        drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, this.m_topVertices.Length - 1, this.m_topVertices);
                    }

                    drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix);
                    drawArgs.device.SetRenderState(RenderState.CullMode , currentCull);
                    drawArgs.device.SetRenderState(RenderState.AlphaBlendEnable , currentAlpha);


                    //put lighting back like it was (see above fix)
                    drawArgs.device.SetRenderState(RenderState.Lighting , lighting);
                }
                catch//(Exception ex)
                {
                    //Utility.Log.Write(ex);
                }

            }
        }

        public override XmlNode ToXml(XmlDocument worldDoc)
        {
            XmlNode lfNode = worldDoc.CreateElement("LineFeature");

            ConfigurationSaver.getRenderableObjectProperties(this, lfNode);


            XmlNode altitudeModeNode = worldDoc.CreateElement("AltitudeMode");
            altitudeModeNode.AppendChild(worldDoc.CreateTextNode(this.AltitudeMode.ToString()));
            lfNode.AppendChild(altitudeModeNode);

            XmlNode minDANode = worldDoc.CreateElement("MinimumDisplayAltitude");
            minDANode.AppendChild(worldDoc.CreateTextNode(this.MinimumDisplayAltitude.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(minDANode);

            XmlNode maxDANode = worldDoc.CreateElement("MaximumDisplayAltitude");
            maxDANode.AppendChild(worldDoc.CreateTextNode(this.MaximumDisplayAltitude.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(maxDANode);

            XmlNode dASNode = worldDoc.CreateElement("DistanceAboveSurface");
            dASNode.AppendChild(worldDoc.CreateTextNode(this.DistanceAboveSurface.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(dASNode);

            XmlNode extrudeHeightNode = worldDoc.CreateElement("ExtrudeHeight");
            extrudeHeightNode.AppendChild(worldDoc.CreateTextNode(this.ExtrudeHeight.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(extrudeHeightNode);

            XmlNode extrudeNode = worldDoc.CreateElement("Extrude");
            extrudeNode.AppendChild(worldDoc.CreateTextNode(this.Extrude.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(extrudeNode);

            XmlNode extrudeUpwardsNode = worldDoc.CreateElement("ExtrudeUpwards");
            extrudeUpwardsNode.AppendChild(worldDoc.CreateTextNode(this.ExtrudeUpwards.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(extrudeUpwardsNode);

            XmlNode extrudeToGroundNode = worldDoc.CreateElement("ExtrudeToGround");
            extrudeToGroundNode.AppendChild(worldDoc.CreateTextNode(this.ExtrudeToGround.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(extrudeToGroundNode);

            XmlNode imageUriNode = worldDoc.CreateElement("ImageUri");
            imageUriNode.AppendChild(worldDoc.CreateTextNode(this.ImageUri));
            lfNode.AppendChild(imageUriNode);

            XmlNode outlineNode = worldDoc.CreateElement("Outline");
            outlineNode.AppendChild(worldDoc.CreateTextNode(this.Outline.ToString(CultureInfo.InvariantCulture)));
            lfNode.AppendChild(outlineNode);

            // TODO: are these right?
            // FeatureColor in xml = LineColor, OutlineColor in xml = PolygonColor ?
            XmlNode featureColorNode = worldDoc.CreateElement("FeatureColor");
            ConfigurationSaver.createColorNode(featureColorNode, this.LineColor);
            lfNode.AppendChild(featureColorNode);

            XmlNode outlineColornode = worldDoc.CreateElement("OutlineColor");
            ConfigurationSaver.createColorNode(outlineColornode, this.PolygonColor);
            lfNode.AppendChild(outlineColornode);


            string posList = ConfigurationSaver.createPointList(this.Points);
            XmlNode lineStringNode = worldDoc.CreateElement("LineString");
            XmlNode posListNode = worldDoc.CreateElement("posList");
            posListNode.AppendChild(worldDoc.CreateTextNode(posList));
            lineStringNode.AppendChild(posListNode);
            lfNode.AppendChild(lineStringNode);

            return lfNode;
        }
    }
}
