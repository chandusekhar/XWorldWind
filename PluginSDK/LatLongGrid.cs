using System;

namespace WorldWind
{
    /// <summary>
    /// Draws a latitude/longitude grid
    /// </summary>
    public class LatLongGrid : RenderableObject
    {
        /// <summary>
        /// Planet radius (constant)
        /// </summary>
        public double WorldRadius;

        /// <summary>
        /// Grid line radius (varies, >= world radius
        /// </summary>
        protected double radius;

        /// <summary>
        /// Current planet == Earth?
        /// </summary>
        public bool IsEarth;

        /// <summary>
        /// Lowest visible longitude
        /// </summary>
        public int MinVisibleLongitude;

        /// <summary>
        /// Highest visible longitude
        /// </summary>
        public int MaxVisibleLongitude;

        /// <summary>
        /// Lowest visible Latitude
        /// </summary>
        public int MinVisibleLatitude;

        /// <summary>
        /// Highest visible Latitude
        /// </summary>
        public int MaxVisibleLatitude;

        /// <summary>
        /// Interval in degrees between visible latitudes
        /// </summary>
        public int LongitudeInterval;

        /// <summary>
        /// Interval in degrees between visible longitudes
        /// </summary>
        public int LatitudeInterval;

        /// <summary>
        /// The number of visible longitude lines
        /// </summary>
        public int LongitudePointCount;

        /// <summary>
        /// The number of visible latitude lines
        /// </summary>
        public int LatitudePointCount;

        /// <summary>
        /// Temporary buffer used for rendering  lines
        /// </summary>
        protected CustomVertex.PositionColored[] lineVertices;

        /// <summary>
        /// Z Buffer enabled (depending on distance)
        /// </summary>
        protected bool useZBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.LatLongGrid"/> class.
        /// </summary>
        public LatLongGrid(World world)
            : base("Grid lines")
        {
            this.WorldRadius = world.EquatorialRadius;

            this.IsEarth = world.Name == "Earth";

            // Render grid lines on top of imagery
            this.RenderPriority = RenderPriority.LinePaths;
        }

        #region RenderableObject

        /// <summary>
        /// Render the grid lines
        /// </summary>
        public override void Render(DrawArgs drawArgs)
        {
            if (!World.Settings.showLatLonLines)
                return;

            this.ComputeGridValues(drawArgs);

            float offsetDegrees = (float)drawArgs.WorldCamera.TrueViewRange.Degrees / 6;

            drawArgs.device.SetRenderState(RenderState.ZEnable , this.useZBuffer);

            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
            drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
            drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
                    (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                    (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                    (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                    );

            Vector3 referenceCenter = new Vector3(
                    (float)drawArgs.WorldCamera.ReferenceCenter.X,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Z);

            // Turn off light
            if (World.Settings.EnableSunShading) drawArgs.device.SetRenderState(RenderState.Lighting , false;

            // Draw longitudes
            for (float longitude = this.MinVisibleLongitude; longitude < this.MaxVisibleLongitude; longitude += this.LongitudeInterval)
            {
                // Draw longitude lines
                int vertexIndex = 0;
                for (float latitude = this.MinVisibleLatitude; latitude <= this.MaxVisibleLatitude; latitude += this.LatitudeInterval)
                {
                    Vector3 pointXyz = MathEngine.SphericalToCartesian(latitude, longitude, this.radius);
                    this.lineVertices[vertexIndex].X = pointXyz.X;
                    this.lineVertices[vertexIndex].Y = pointXyz.Y;
                    this.lineVertices[vertexIndex].Z = pointXyz.Z;
                    this.lineVertices[vertexIndex].Color = World.Settings.latLonLinesColor;
                    vertexIndex++;
                }
                drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, this.LatitudePointCount - 1, this.lineVertices);

                // Draw longitude label
                float lat = (float)(drawArgs.WorldCamera.Latitude).Degrees;
                if (lat > 70)
                    lat = 70;
                Vector3 v = MathEngine.SphericalToCartesian(lat, (float)longitude, this.radius);
                if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(v))
                {
                    // Make sure longitude is in -180 .. 180 range
                    int longitudeRanged = (int)longitude;
                    if (longitudeRanged <= -180)
                        longitudeRanged += 360;
                    else if (longitudeRanged > 180)
                        longitudeRanged -= 360;

                    string s = Math.Abs(longitudeRanged).ToString();
                    if (longitudeRanged < 0)
                        s += "W";
                    else if (longitudeRanged > 0 && longitudeRanged < 180)
                        s += "E";

                    v = drawArgs.WorldCamera.Project(v - referenceCenter);
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)v.X + 2, (int)v.Y, 10, 10);
                    drawArgs.defaultDrawingFont.DrawText(null, s, rect.Left, rect.Top, World.Settings.latLonLinesColor);
                }
            }

            // Draw latitudes
            for (float latitude = this.MinVisibleLatitude; latitude <= this.MaxVisibleLatitude; latitude += this.LatitudeInterval)
            {
                // Draw latitude label
                float longitude = (float)(drawArgs.WorldCamera.Longitude).Degrees + offsetDegrees;

                Vector3 v = MathEngine.SphericalToCartesian(latitude, longitude, this.radius);
                if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(v))
                {
                    v = drawArgs.WorldCamera.Project(v - referenceCenter);
                    float latLabel = latitude;
                    if (latLabel > 90)
                        latLabel = 180 - latLabel;
                    else if (latLabel < -90)
                        latLabel = -180 - latLabel;
                    string s = ((int)Math.Abs(latLabel)).ToString();
                    if (latLabel > 0)
                        s += "N";
                    else if (latLabel < 0)
                        s += "S";
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)v.X, (int)v.Y, 10, 10);
                    drawArgs.defaultDrawingFont.DrawText(null, s, rect.Left, rect.Top, World.Settings.latLonLinesColor);
                }

                // Draw latitude line
                int vertexIndex = 0;
                for (longitude = this.MinVisibleLongitude; longitude <= this.MaxVisibleLongitude; longitude += this.LongitudeInterval)
                {
                    Vector3 pointXyz = MathEngine.SphericalToCartesian(latitude, longitude, this.radius);
                    this.lineVertices[vertexIndex].X = pointXyz.X;
                    this.lineVertices[vertexIndex].Y = pointXyz.Y;
                    this.lineVertices[vertexIndex].Z = pointXyz.Z;

                    if (latitude == 0)
                        this.lineVertices[vertexIndex].Color = World.Settings.equatorLineColor;
                    else
                        this.lineVertices[vertexIndex].Color = World.Settings.latLonLinesColor;

                    vertexIndex++;
                }
                drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, this.LongitudePointCount - 1, this.lineVertices);
            }

            if (World.Settings.showTropicLines && this.IsEarth) this.RenderTropicLines(drawArgs);

            // Restore state
            drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;
            if (!this.useZBuffer)
                // Reset Z buffer setting
                drawArgs.device.SetRenderState(RenderState.ZEnable , true);
            if (World.Settings.EnableSunShading) drawArgs.device.SetRenderState(RenderState.Lighting , true;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            this.isInitialized = true;
        }

        public override void Dispose()
        {
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

        public override void Update(DrawArgs drawArgs)
        {
        }

        public override bool IsOn
        {
            get
            {
                return World.Settings.showLatLonLines;
            }
            set
            {
                World.Settings.showLatLonLines = value;
            }
        }

        #endregion

        /// <summary>
        /// Draw Tropic of Cancer, Tropic of Capricorn, Arctic and Antarctic lines
        /// </summary>
        void RenderTropicLines(DrawArgs drawArgs)
        {
            this.RenderTropicLine(drawArgs, 23.439444f, "Tropic Of Cancer");
            this.RenderTropicLine(drawArgs, -23.439444f, "Tropic Of Capricorn");
            this.RenderTropicLine(drawArgs, 66.560556f, "Arctic Circle");
            this.RenderTropicLine(drawArgs, -66.560556f, "Antarctic Circle");
        }

        /// <summary>
        /// Draws a tropic line at specified latitude with specified label
        /// </summary>
        /// <param name="latitude">Latitude in degrees</param>
        void RenderTropicLine(DrawArgs drawArgs, float latitude, string label)
        {
            int vertexIndex = 0;
            Vector3 referenceCenter = new Vector3(
                    (float)drawArgs.WorldCamera.ReferenceCenter.X,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Z);

            for (float longitude = this.MinVisibleLongitude; longitude <= this.MaxVisibleLongitude; longitude = longitude + this.LongitudeInterval)
            {
                Vector3 pointXyz = MathEngine.SphericalToCartesian(latitude, longitude, this.radius);

                this.lineVertices[vertexIndex].X = pointXyz.X;
                this.lineVertices[vertexIndex].Y = pointXyz.Y;
                this.lineVertices[vertexIndex].Z = pointXyz.Z;
                this.lineVertices[vertexIndex].Color = World.Settings.tropicLinesColor;
                vertexIndex++;
            }
            drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, this.LongitudePointCount - 1, this.lineVertices);

            Vector3 t1 = MathEngine.SphericalToCartesian(Angle.FromDegrees(latitude),
                    drawArgs.WorldCamera.Longitude - drawArgs.WorldCamera.TrueViewRange * 0.3f * 0.5f, this.radius);
            if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(t1))
            {
                t1 = drawArgs.WorldCamera.Project(t1 - referenceCenter);
                drawArgs.defaultDrawingFont.DrawText(null, label, new System.Drawing.Rectangle((int)t1.X, (int)t1.Y, drawArgs.screenWidth, drawArgs.screenHeight), DrawTextFormat.NoClip, World.Settings.tropicLinesColor);
            }
        }

        /// <summary>
        /// Recalculates the grid bounds + interval values
        /// </summary>
        public void ComputeGridValues(DrawArgs drawArgs)
        {
            double vr = drawArgs.WorldCamera.TrueViewRange.Radians;

            // Compensate for closer grid towards poles
            vr *= 1 + Math.Abs(Math.Sin(drawArgs.WorldCamera.Latitude.Radians));

            if (vr < 0.17)
                this.LatitudeInterval = 1;
            else if (vr < 0.6)
                this.LatitudeInterval = 2;
            else if (vr < 1.0)
                this.LatitudeInterval = 5;
            else
                this.LatitudeInterval = 10;

            this.LongitudeInterval = this.LatitudeInterval;

            if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(MathEngine.SphericalToCartesian(90, 0, this.radius)) ||
                    drawArgs.WorldCamera.ViewFrustum.ContainsPoint(MathEngine.SphericalToCartesian(-90, 0, this.radius)))
            {
                // Pole visible, 10 degree longitude spacing forced
                this.LongitudeInterval = 10;
            }

            this.MinVisibleLongitude = this.LongitudeInterval >= 10 ? -180 : (int)drawArgs.WorldCamera.Longitude.Degrees / this.LongitudeInterval * this.LongitudeInterval - 18 * this.LongitudeInterval;
            this.MaxVisibleLongitude = this.LongitudeInterval >= 10 ? 180 : (int)drawArgs.WorldCamera.Longitude.Degrees / this.LongitudeInterval * this.LongitudeInterval + 18 * this.LongitudeInterval;
            this.MinVisibleLatitude = (int)drawArgs.WorldCamera.Latitude.Degrees / this.LatitudeInterval * this.LatitudeInterval - 9 * this.LatitudeInterval;
            this.MaxVisibleLatitude = (int)drawArgs.WorldCamera.Latitude.Degrees / this.LatitudeInterval * this.LatitudeInterval + 9 * this.LatitudeInterval;

            if (this.MaxVisibleLatitude - this.MinVisibleLatitude >= 180 || this.LongitudeInterval == 10)
            {
                this.MinVisibleLatitude = -90;
                this.MaxVisibleLatitude = 90;
            }

            this.LongitudePointCount = (this.MaxVisibleLongitude - this.MinVisibleLongitude) / this.LongitudeInterval + 1;
            this.LatitudePointCount = (this.MaxVisibleLatitude - this.MinVisibleLatitude) / this.LatitudeInterval + 1;
            int vertexPointCount = Math.Max(this.LatitudePointCount, this.LongitudePointCount);
            if (this.lineVertices == null || vertexPointCount > this.lineVertices.Length) this.lineVertices = new CustomVertex.PositionColored[Math.Max(this.LatitudePointCount, this.LongitudePointCount)];

            this.radius = this.WorldRadius;
            if (drawArgs.WorldCamera.Altitude < 0.10f * this.WorldRadius)
                this.useZBuffer = false;
            else
            {
                this.useZBuffer = true;
                double bRadius = this.WorldRadius * 1.01f;
                double nRadius = this.WorldRadius + 0.015f * drawArgs.WorldCamera.Altitude;

                this.radius = Math.Min(nRadius, bRadius);
            }
        }
    }
}
