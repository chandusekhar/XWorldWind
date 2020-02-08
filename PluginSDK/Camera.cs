using System;
using System.Globalization;
using SharpDX;
using SharpDX.Direct3D9;

namespace WorldWind
{
	/// <summary>
	/// Camera base class (simple camera)
	/// </summary>
	public class CameraBase
	{
        protected short _terrainElevation;              // at the camera target position (not under the camera)
        protected short _terrainElevationUnderCamera;   // right under the camera
        protected double _worldRadius;

		protected Angle _latitude;
		protected Angle _longitude;
		protected Angle _heading;
		protected Angle _tilt;
        protected Angle _swivel = Angle.FromDegrees(0.0);
		protected Angle _bank;
		protected double _distance; // Distance from eye to target
		protected double _altitude; // Altitude above sea level
		//protected Quaternion _orientation;
		protected Quaternion4d m_Orientation;

		protected Frustum _viewFrustum = new Frustum();
		protected Angle _fov = World.Settings.cameraFov;

		protected Vector3 _position;

        protected static readonly Angle minTilt = Angle.FromDegrees(0.0);
        protected static readonly Angle minNegTilt = Angle.FromDegrees(-85.0);
        protected static readonly Angle maxTilt = Angle.FromDegrees(85.0);
        protected static readonly Angle minSwivel = Angle.FromDegrees(-85.0);
        protected static readonly Angle maxSwivel = Angle.FromDegrees(85.0);

        protected static readonly double minimumAltitude = 100;
		protected static double maximumAltitude = double.MaxValue;

		protected Matrix m_ProjectionMatrix; // Projection matrix used in last render.
		protected Matrix m_ViewMatrix; // View matrix used in last render.
		protected Matrix m_WorldMatrix = Matrix.Identity;

		protected Angle viewRange;
		protected Angle trueViewRange;
		protected Viewport viewPort;

		protected int lastStepZoomTickCount;
		static Point3d cameraUpVector = new Point3d(0,0,1);
        public Point3d ReferenceCenter = new Point3d(0,0,0);

		// Camera Reset variables
        static bool twotaps;
		static int lastResetTime; // Used by Reset() to keep track type of reset.
		const int DoubleTapDelay = 1000; // Double tap max time (ms)

        // Head tracker values to add to camera tilt and swivel values

        /// <summary>
        /// Whether or not we should allow headtracking 
        /// </summary>
        protected bool _headTracking;

        /// <summary>
        /// Modifies camera tilt based on head position
        /// </summary>
        protected Angle _headTilt = Angle.FromDegrees(0.0);

        /// <summary>
        /// Modifies camera swivel based on head position
        /// </summary>
        protected Angle _headSwivel = Angle.FromDegrees(0.0);

        /// <summary>
        /// Modifies distance to target based on head position
        /// </summary>
        protected double _headZoom;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.CameraBase"/> class.
		/// </summary>
		/// <param name="targetPosition"></param>
		/// <param name="radius">Planet's radius in meters</param>
		public CameraBase( Vector3 targetPosition, double radius ) 
		{
			this._worldRadius = radius;
			this._distance = 2* this._worldRadius;
			this._altitude = this._distance;
			maximumAltitude = 20 * this._worldRadius;
		//	this._orientation = MathEngine.EulerToQuaternion(0,0,0);
			this.m_Orientation = Quaternion4d.EulerToQuaternion(0,0,0);
		}

		public Viewport Viewport
		{
			get
			{
				return this.viewPort;
			}
		}

		public Matrix ViewMatrix
		{
			get
			{
				return this.m_ViewMatrix;
			}
		}

		public Matrix ProjectionMatrix
		{
			get
			{
				return this.m_ProjectionMatrix;
			}
		}
		public Matrix WorldMatrix
		{
			get
			{
				return this.m_WorldMatrix;
			}
		}

		public bool IsPointGoto
		{
			get { return World.Settings.cameraIsPointGoto; }
			set { World.Settings.cameraIsPointGoto = value; }
		}

		public virtual Angle Latitude
		{
			get{ return this._latitude; }
		}

		public virtual Angle Longitude
		{
			get { return this._longitude; }
		}

		public virtual Angle Tilt
		{
			get { return this._tilt; }
			set
			{
                if (value > maxTilt)
                {
                    value = maxTilt;

                    if (this.HeadTilt + value > maxTilt)
                    {
                        this.HeadTilt = maxTilt - value;
                    }
                }
                else
                {
                    if (World.Settings.AllowNegativeTilt)
                    {
                        if (value < minNegTilt) value = minNegTilt;

                        if (this.HeadTilt + value < minNegTilt)
                        {
                            this.HeadTilt = minNegTilt - value;
                        }
                    }
                    else
                    {
                        if (value < minTilt) value = minTilt;

                        if (this.HeadTilt + value < minTilt)
                        {
                            this.HeadTilt = minTilt - value;
                        }
                    }
                }

                this._tilt = value;
                this.ComputeAltitude(this._distance + this._headZoom, this._tilt + this._headTilt, this._swivel + this._headSwivel);
            }
		}

        public virtual Angle Swivel
        {
            get { return this._swivel; }
            set
            {
                if (value > maxSwivel)
                    value = maxSwivel;
                else if (value < minSwivel)
                    value = minSwivel;

                this._swivel = value;
                this.ComputeAltitude(this._distance + this._headZoom, this._tilt + this._headTilt, this._swivel + this._headSwivel);
            }
        }

        public virtual Angle HeadTilt
        {
            get { return this._headTilt; }
            set 
            {
                if (this._headTracking)
                {
                    this._headTilt = value;

                    if (this._headTilt + this.Tilt > maxTilt)
                    {
                        this._headTilt = maxTilt - this.Tilt;
                    }
                    else if (this.HeadTilt + this.Tilt < minNegTilt)
                    {
                        this._headTilt = minNegTilt - this.Tilt;
                    }

                    this.ComputeAltitude(this._distance + this._headZoom, this._tilt + this._headTilt, this._swivel + this._headSwivel);
                }
            }
        }

        public virtual Angle HeadSwivel
        {
            get { return this._headSwivel; }
            set 
            {
                if (this._headTracking)
                {
                    this._headSwivel = value;
                    this.ComputeAltitude(this._distance + this._headZoom, this._tilt + this._headTilt, this._swivel + this._headSwivel);
                }
            }
        }

        public virtual double HeadZoom
        {
            get { return this._headZoom; }
            set
            {
                if (this._headTracking)
                {
                    this._headZoom = value;
                    this.ComputeAltitude(this._distance + this._headZoom, this._tilt + this._headTilt, this._swivel + this._headSwivel);
                }
            }
        }

		public virtual Angle Bank
		{
			get { return this._bank; }
			set
			{
				if(Angle.IsNaN(value))
					return;

                this._bank = value;
			}
		}

		public virtual Angle Heading
		{
			get { return this._heading; }
			set { this._heading = value; }
		}

		public virtual Quaternion4d CurrentOrientation
		{
		//	get { return this._orientation; }	
		//	set { this._orientation = value; }
			get { return this.m_Orientation; }	
			set { this.m_Orientation = value; }
		}

		/// <summary>
		/// Altitude above sea level (meters)
		/// </summary>
		public virtual double Altitude
		{
			get { return this._altitude; }
			set
			{
                this.TargetAltitude = value;
			}
		}

		/// <summary>
		/// Altitude above terrain (meters)
		/// </summary>
		public virtual double AltitudeAboveTerrain
		{
			get { return this._altitude - this._terrainElevation; }
		}

		/// <summary>
		/// Target altitude above sea level (meters) (after travel)
		/// </summary>
		public virtual double TargetAltitude
		{
			get { return this._altitude; }
			set
			{
                if (value < this._terrainElevationUnderCamera * World.Settings.VerticalExaggeration + minimumAltitude)
                    value = this._terrainElevationUnderCamera * World.Settings.VerticalExaggeration + minimumAltitude;
				if(value > maximumAltitude)
					value = maximumAltitude;
				this._altitude = value;
                this.ComputeDistance(this._altitude, this._tilt);
			}
		}

        public virtual short TerrainElevation
        {
            get { return this._terrainElevation; }
            set { this._terrainElevation = value; }
        }

        public virtual short TerrainElevationUnderCamera
        {
            get { return this._terrainElevationUnderCamera; }
            set { this._terrainElevationUnderCamera = value; }
        }

        private DateTime lastElevationUpdate = DateTime.Now;

        public void UpdateTerrainElevation(TerrainAccessor terrainAccessor)
        {
            // Update camera terrain elevation
            if (terrainAccessor != null)
            {
                if (this.Altitude < 300000)
                {
                    if (DateTime.Now - this.lastElevationUpdate > TimeSpan.FromMilliseconds(500))
                    {
                        float elevation;
                        // Under camera target
                        elevation = terrainAccessor.GetCachedElevationAt(this.Latitude.Degrees, this.Longitude.Degrees);
                        this.TerrainElevation = float.IsNaN(elevation) ? (short)0 : (short)elevation;
                        // Under the camera itself
                        Vector3 cameraPos = this.Position;
                        Vector3 cameraCoord = MathEngine.CartesianToSpherical(cameraPos.X, cameraPos.Y, cameraPos.Z);
                        double camLat = MathEngine.RadiansToDegrees(cameraCoord.Y);
                        double camLon = MathEngine.RadiansToDegrees(cameraCoord.Z);
                        elevation = terrainAccessor.GetCachedElevationAt(camLat, camLon);
                        this.TerrainElevationUnderCamera = float.IsNaN(elevation) ? (short)0 : (short)elevation;
                        if (this.TerrainElevationUnderCamera < 0 && !World.Settings.AllowNegativeAltitude) this.TerrainElevationUnderCamera = 0;
                        // reset timer
                        this.lastElevationUpdate = DateTime.Now;
                    }
                }
                else
                {
                    this.TerrainElevation = 0;
                    this.TerrainElevationUnderCamera = 0;
                }
            }
            else
            {
                this.TerrainElevation = 0;
                this.TerrainElevationUnderCamera = 0;
            }
        }

		public virtual Angle ViewRange
		{
			get 
			{
				return this.viewRange;
			}
		}

		/// <summary>
		/// Angle from horizon - center earth - horizon in opposite directon
		/// </summary>
		public virtual Angle TrueViewRange
		{
			get 
			{
				return this.trueViewRange;
			}
		}

		/// <summary>
		/// Camera position (World XYZ coordinates)
		/// </summary>
		public virtual Vector3 Position
		{
			get{ return this._position; }
		}

		/// <summary>
		/// The planet's radius in meters
		/// </summary>
		public virtual double WorldRadius
		{
			get { return this._worldRadius;	}
			set { this._worldRadius = value; }
		}

		public Vector3 EyeDiff = new Vector3();

		public float curCameraElevation;
		float targetCameraElevation;

		public static Point3d LookFrom = new Point3d();
		public static Point3d relCameraPos = new Point3d();

		public virtual void ComputeAbsoluteMatrices()
		{
            this.m_absoluteWorldMatrix = Matrix.Identity;
			
			float aspectRatio =  (float) this.viewPort.Width / this.viewPort.Height;
			float zNear = (float)Math.Max(this._altitude - this.TerrainElevationUnderCamera, minimumAltitude) * 0.1f;
			double distToCenterOfPlanet = (this._altitude + this.WorldRadius);
			double tangentalDistance  = Math.Sqrt( distToCenterOfPlanet*distToCenterOfPlanet - this._worldRadius* this._worldRadius);
            if (tangentalDistance < 1000000 || double.IsNaN(tangentalDistance))
                tangentalDistance = 1000000;

            this.m_absoluteProjectionMatrix = Matrix.PerspectiveFovRH( (float) this._fov.Radians, aspectRatio, zNear, (float)tangentalDistance );

            this.m_absoluteViewMatrix = Matrix.LookAtRH(
				MathEngine.SphericalToCartesian(this._latitude.Degrees, this._longitude.Degrees, this.WorldRadius + this.curCameraElevation),
				Vector3.Zero,
				new Vector3(0,0,1));

            this.m_absoluteViewMatrix *= Matrix.RotationYawPitchRoll(
                (float)-(this._swivel.Radians + this._headSwivel.Radians), 
                (float)-(this._tilt.Radians + this._headTilt.Radians),
				(float)this._heading.Radians);

            //m_absoluteViewMatrix *= Matrix.Translation(0, 0, (float)(-this._distance + curCameraElevation));
            this.m_absoluteViewMatrix *= Matrix.Translation(0, 0, (float)(-(this._distance + this._headZoom)));
            this.m_absoluteViewMatrix *= Matrix.RotationZ((float)this._bank.Radians);
		}

		public virtual void ComputeViewMatrix()
		{
            // Compute camera elevation
			if(World.Settings.ElevateCameraLookatPoint)
			{
                int minStep = 10;
                this.targetCameraElevation = this.TerrainElevation * World.Settings.VerticalExaggeration;
                float stepToTarget = this.targetCameraElevation - this.curCameraElevation;
                if (Math.Abs(stepToTarget) > minStep)
                {
                    float step = 0.05f * stepToTarget;
                    if (Math.Abs(step) < minStep) step = step > 0 ? minStep : -minStep;
                    this.curCameraElevation = this.curCameraElevation + step;
                }
                else
                    this.curCameraElevation = this.targetCameraElevation;                
			}
			else
			{
                this.curCameraElevation = 0;
			}

            // Absolute matrices
            this.ComputeAbsoluteMatrices();
			
            // needs to be double precsion
            double radius = this.WorldRadius + this.curCameraElevation;
            double radCosLat = radius*Math.Cos(this._latitude.Radians);
            LookFrom = new Point3d(radCosLat * Math.Cos(this._longitude.Radians),
                                radCosLat * Math.Sin(this._longitude.Radians),
                                radius * Math.Sin(this._latitude.Radians));

			// this constitutes a local tri-frame hovering above the sphere		
            Point3d zAxis = LookFrom.normalize(); // on sphere the normal vector and position vector are the same
	        Point3d xAxis = Point3d.cross(cameraUpVector,zAxis).normalize();
	        Point3d yAxis = Point3d.cross(zAxis, xAxis);

            this.ReferenceCenter = MathEngine.SphericalToCartesianD(
				Angle.FromRadians(Convert.ToSingle(this._latitude.Radians)),
				Angle.FromRadians(Convert.ToSingle(this._longitude.Radians)), this.WorldRadius);
			
			// Important step !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // In order to use single precsion rendering, we need to define a local frame (i.e. center of center tile, etc.)
            // Vector3d LocalCenter should be defined & initialized in the CameraBase class
            // Each time the camera moves, a new local center could be defined
            // The local center also has to be subtracted from all the terrain vertices!!!!
	        relCameraPos = LookFrom - this.ReferenceCenter;

	        // Important step: construct the single precision m_ViewMatrix by hand

            // We can build the m_ViewMatrix by hand
            this.m_ViewMatrix.M11 = (float)xAxis.X;
            this.m_ViewMatrix.M21 = (float)xAxis.Y;
            this.m_ViewMatrix.M31 = (float)xAxis.Z;

            this.m_ViewMatrix.M12 = (float)yAxis.X;
            this.m_ViewMatrix.M22 = (float)yAxis.Y;
            this.m_ViewMatrix.M32 = (float)yAxis.Z;

            this.m_ViewMatrix.M13 = (float)zAxis.X;
            this.m_ViewMatrix.M23 = (float)zAxis.Y;
            this.m_ViewMatrix.M33 = (float)zAxis.Z;

            this.m_ViewMatrix.M41 = -(float)(xAxis.X*relCameraPos.X + xAxis.Y*relCameraPos.Y + xAxis.Z*relCameraPos.Z);
            this.m_ViewMatrix.M42 = -(float)(yAxis.X*relCameraPos.X + yAxis.Y*relCameraPos.Y + yAxis.Z*relCameraPos.Z);
            this.m_ViewMatrix.M43 = -(float)(zAxis.X*relCameraPos.X + zAxis.Y*relCameraPos.Y + zAxis.Z*relCameraPos.Z);

            this.m_ViewMatrix.M14 = (float)0.0;
            this.m_ViewMatrix.M24 = (float)0.0;
            this.m_ViewMatrix.M34 = (float)0.0;
            this.m_ViewMatrix.M44 = (float)1.0;

            double cameraDisplacement = this._distance + this._headZoom;
			//if(cameraDisplacement < targetCameraElevation + minimumAltitude)
			//	cameraDisplacement = targetCameraElevation + minimumAltitude;

            this.m_ViewMatrix *= Matrix.RotationYawPitchRoll(
                (float)-(this._swivel.Radians + this._headSwivel.Radians), 
                (float)-(this._tilt.Radians + this._headTilt.Radians),
				(float) this._heading.Radians);
            //m_ViewMatrix *= Matrix.Translation(0, 0, (float)(-cameraDisplacement + curCameraElevation));
            this.m_ViewMatrix *= Matrix.Translation(0, 0, (float)(-cameraDisplacement));
            this.m_ViewMatrix *= Matrix.RotationZ((float) this._bank.Radians);
			
			// Extract camera position
			Matrix cam = Matrix.Invert(this.m_absoluteViewMatrix);

            this._position = new Vector3(cam.M41, cam.M42, cam.M43);
		}

		/// <summary>
		/// Field of view (degrees)
		/// </summary>
		public virtual Angle Fov
		{
			get{ return this._fov; }
			set
			{
				if(value > World.Settings.cameraFovMax)
					value = World.Settings.cameraFovMax;
				if(value < World.Settings.cameraFovMin)
					value = World.Settings.cameraFovMin;
				this._fov = value;
			}
		}

		/// <summary>
		/// Distance to target position on ground.
		/// </summary>
		public virtual double Distance
		{
			get
			{
				// TODO: Accurate altitude / distance calculations
                //return _distance - _terrainElevation;
                return this._distance;
            }
			set
			{
                this.TargetDistance = value;
			}
		}

		/// <summary>
		/// Distance to target position on ground (after traveling to target)
		/// </summary>
		public virtual double TargetDistance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if(value < minimumAltitude)
					value = minimumAltitude;
				if(value > maximumAltitude)
					value = maximumAltitude;
                this._distance = value;
                this.ComputeAltitude(this._distance + this._headZoom, this._tilt + this._headTilt, this._swivel + this._headSwivel);
            }
		}

		public virtual Frustum ViewFrustum
		{
			get { return this._viewFrustum; }
		}

		public virtual void Update(Device device)
		{
            this.viewPort = device.Viewport;

			Point3d p = Quaternion4d.QuaternionToEuler(this.m_Orientation);

			if(!double.IsNaN(p.Y))
				this._latitude.Radians = p.Y;
			if(!double.IsNaN(p.X))
				this._longitude.Radians = p.X;
			if(!double.IsNaN(p.Z))
				this._heading.Radians = p.Z;

            // Compute matrices
            this.ComputeProjectionMatrix(this.viewPort);
            this.ComputeViewMatrix();
			device.Transform.Projection = this.m_ProjectionMatrix;
			device.Transform.View = this.m_ViewMatrix;
			device.SetTransform(TransformState.World, this.m_WorldMatrix;

            this.ViewFrustum.Update(
				Matrix.Multiply(this.m_absoluteWorldMatrix,
				Matrix.Multiply(this.m_absoluteViewMatrix, this.m_absoluteProjectionMatrix)));

			// Old view range (used in quadtile logic)
			double factor = (this._altitude) / this._worldRadius;
			if(factor > 1)
                this.viewRange = Angle.FromRadians(Math.PI);
			else
                this.viewRange = Angle.FromRadians(Math.Abs(Math.Asin((this._altitude) / this._worldRadius))*2);

			// True view range 
			if(factor < 1)
                this.trueViewRange = Angle.FromRadians(Math.Abs(Math.Asin((this._distance + this._headZoom) / this._worldRadius))*2);
			else
                this.trueViewRange = Angle.FromRadians(Math.PI);	
	
			World.Settings.cameraAltitudeMeters = this.Altitude;
			World.Settings.cameraLatitude = this._latitude;
			World.Settings.cameraLongitude = this._longitude;
			World.Settings.cameraHeading = this._heading;
			World.Settings.cameraTilt = this._tilt;
            World.Settings.cameraSwivel = this._swivel;

            if (World.Settings.cameraHeadTracking != this._headTracking)
            {
                this._headTracking = World.Settings.cameraHeadTracking;

                if (!this._headTracking)
                {
                    this._headTilt = Angle.Zero;
                    this._headSwivel = Angle.Zero;
                    this._headZoom = 0.0;
                    World.Settings.cameraHeadTilt = this._headTilt;
                    World.Settings.cameraHeadSwivel = this._headSwivel;
                    World.Settings.cameraHeadZoom = this._headZoom;
                }
            }

            if (this._headTracking)
            {
                World.Settings.cameraHeadTilt = this._headTilt;
                World.Settings.cameraHeadSwivel = this._headSwivel;
                World.Settings.cameraHeadZoom = this._headZoom;
            }
		}

        /// <summary>
        /// Resets the camera settings as dictated in WW-788
        /// Two consecutive resets closer than DoubleTapDelay ms apart performs a full reset.
        /// </summary>
        public virtual void Space()
        {
            this.Fov = World.Settings.cameraFov;

            int curTime = Environment.TickCount;
            if (curTime - lastResetTime < DoubleTapDelay)
            {
                if (twotaps)
                {
                    // three taps, move out to sattelite
                    twotaps = false;
                    this.SetPosition(
                        World.Settings.cameraLatitude.Degrees,
                        World.Settings.cameraLongitude.Degrees,
                        0,
                        2 * this._worldRadius,
                        World.Settings.CameraTilt.Degrees,
                        this._bank.Degrees);
                }
                else
                {
                    // two taps, orient north
                    twotaps = true;
                    this.SetPosition(
                        World.Settings.cameraLatitude.Degrees,
                        World.Settings.cameraLongitude.Degrees,
                        0.0,
                        World.Settings.CameraAltitude,
                        0.0,
                        0.0);
                }
            }
            else
            {
                twotaps = false;
                // one tap, pause current view
                this.SetPosition(
                    World.Settings.cameraLatitude.Degrees,
                    World.Settings.cameraLongitude.Degrees,
                    World.Settings.CameraHeading.Degrees,
                    World.Settings.CameraAltitude,
                    World.Settings.CameraTilt.Degrees,
                    this._bank.Degrees);
            }
            lastResetTime = curTime;
        }

		/// <summary>
		/// Resets the camera settings
		/// Two consecutive resets closer than DoubleTapDelay ms apart performs a full reset.
		/// </summary>
		public virtual void Reset()
		{
            this.Fov = World.Settings.cameraFov;

			// Reset direction, tilt & bank
			this.SetPosition(double.NaN,double.NaN,0,double.NaN,0,0);
		}

		/// <summary>
		/// Sets camera position.
		/// </summary>
		/// <param name="lat">Latitude in decimal degrees</param>
		/// <param name="lon">Longitude in decimal degrees</param>
		public virtual void PointGoto(double lat, double lon )
		{
			if(!World.Settings.cameraIsPointGoto)
				return;

            this.SetPosition( lat,lon,double.NaN,double.NaN,double.NaN,double.NaN );
		}

		/// <summary>
		/// Sets camera position.
		/// </summary>
		/// <param name="lat">Latitude in decimal degrees</param>
		/// <param name="lon">Longitude in decimal degrees</param>
		public virtual void PointGoto(Angle lat, Angle lon )
		{
			if(!World.Settings.cameraIsPointGoto)
				return;

            this.SetPosition( lat.Degrees,lon.Degrees,double.NaN,double.NaN,double.NaN,double.NaN );
		}

		/// <summary>
		/// Sets camera position.
		/// </summary>
		/// <param name="lat">Latitude in decimal degrees</param>
		/// <param name="lon">Longitude in decimal degrees</param>
		public virtual void SetPosition(double lat, double lon )
		{
            this.SetPosition( lat,lon,0,double.NaN,0,0 );
		}

		/// <summary>
		/// Sets camera position.
		/// </summary>
		/// <param name="lat">Latitude in decimal degrees</param>
		/// <param name="lon">Longitude in decimal degrees</param>
		/// <param name="heading">Heading in decimal degrees</param>
		/// <param name="_altitude">Altitude above ground level in meters</param>
		/// <param name="tilt">Tilt in decimal degrees</param>
		public virtual void SetPosition(double lat, double lon, double heading, double _altitude, double tilt )
		{
            this.SetPosition( lat,lon,heading,_altitude,tilt,0);
		}

		/// <summary>
		/// Sets camera position.
		/// </summary>
		/// <param name="lat">Latitude in decimal degrees</param>
		/// <param name="lon">Longitude in decimal degrees</param>
		/// <param name="heading">Heading in decimal degrees</param>
		/// <param name="_altitude">Altitude above ground level in meters</param>
		/// <param name="tilt">Tilt in decimal degrees</param>
		/// <param name="bank">Camera bank (roll) in decimal degrees</param>
		public virtual void SetPosition(double lat, double lon, double heading, double _altitude, double tilt, double bank)
		{
			if(double.IsNaN(lat)) lat = this._latitude.Degrees;
			if(double.IsNaN(lon)) lon = this._longitude.Degrees;
			if(double.IsNaN(heading)) heading = this._heading.Degrees;
			if(double.IsNaN(bank))  bank = this._bank.Degrees;

            this.m_Orientation = Quaternion4d.EulerToQuaternion(
				MathEngine.DegreesToRadians(lon),
				MathEngine.DegreesToRadians(lat),
				MathEngine.DegreesToRadians(heading));

			Point3d p = Quaternion4d.QuaternionToEuler(this.m_Orientation);

            this._latitude.Radians = p.Y;
            this._longitude.Radians = p.X;
            this._heading.Radians = p.Z;

			if(!double.IsNaN(tilt)) this.Tilt = Angle.FromDegrees(tilt);
			if(!double.IsNaN(_altitude)) 
				this.Altitude = _altitude;
			this.Bank = Angle.FromDegrees(bank);
		}
			
		Matrix m_absoluteViewMatrix = Matrix.Identity;
		Matrix m_absoluteWorldMatrix = Matrix.Identity;
		Matrix m_absoluteProjectionMatrix = Matrix.Identity;
		
		public Matrix AbsoluteViewMatrix
		{
			get{ return this.m_absoluteViewMatrix; }
		}
		
		public Matrix AbsoluteWorldMatrix
		{
			get{ return this.m_absoluteWorldMatrix; }
		}

		public Matrix AbsoluteProjectionMatrix
		{
			get{ return this.m_absoluteProjectionMatrix; }
		}

		/// <summary>
		/// Calculates latitude/longitude for given screen coordinate.
		/// </summary>
		public virtual void PickingRayIntersection(
			int screenX, 
			int screenY,
			out Angle latitude,
			out Angle longitude)
		{
			Vector3 v1 = new Vector3();
			v1.X = screenX;
			v1.Y = screenY;
			v1.Z = this.viewPort.MinZ;
			v1.Unproject(this.viewPort, this.m_absoluteProjectionMatrix, this.m_absoluteViewMatrix, this.m_absoluteWorldMatrix);

			Vector3 v2 = new Vector3();
			v2.X = screenX;
			v2.Y = screenY;
			v2.Z = this.viewPort.MaxZ;
			v2.Unproject(this.viewPort, this.m_absoluteProjectionMatrix, this.m_absoluteViewMatrix, this.m_absoluteWorldMatrix);

			Point3d p1 = new Point3d(v1.X, v1.Y, v1.Z);
			Point3d p2 = new Point3d(v2.X, v2.Y, v2.Z);

			double a = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y) + (p2.Z - p1.Z) * (p2.Z - p1.Z);
			double b = 2.0*((p2.X - p1.X)*(p1.X) + (p2.Y - p1.Y)*(p1.Y) + (p2.Z - p1.Z)*(p1.Z));
			double c = p1.X*p1.X + p1.Y*p1.Y + p1.Z*p1.Z - this._worldRadius * this._worldRadius;

			double discriminant = b*b - 4 * a * c;
			if(discriminant <= 0)
			{
				latitude = Angle.NaN;
				longitude = Angle.NaN;
				return;
			}

			//	float t0 = ((-1.0f) * b + (float)Math.Sqrt(b*b - 4 * a * c)) / (2*a);
			double t1 = ((-1.0) * b - Math.Sqrt(b*b - 4 * a * c)) / (2*a);

			//	Vector3 i0 = new Vector3(p1.X + t0*(p2.X - p1.X), p1.Y + t0*(p2.Y - p1.Y), p1.Z + t0 *(p2.Z - p1.Z));
			Point3d i1 = new Point3d(p1.X + t1*(p2.X - p1.X), p1.Y + t1*(p2.Y - p1.Y), p1.Z + t1 *(p2.Z - p1.Z));

			//	Vector3 i0t = MathEngine.CartesianToSpherical(i0.X, i0.Y, i0.Z);
			Point3d i1t = MathEngine.CartesianToSphericalD(i1.X, i1.Y, i1.Z);
			Point3d mousePointer = i1t;

			latitude = Angle.FromRadians(mousePointer.Y);
			longitude = Angle.FromRadians(mousePointer.Z);
		}

        /// <summary>
        /// Calculates latitude/longitude for given screen coordinate.
        /// Cast a ray to the terrain geometry (Patrick Murris - march 2007)
        /// </summary>
        public virtual void PickingRayIntersectionWithTerrain(
            int screenX,
            int screenY,
            out Angle latitude,
            out Angle longitude,
            World world)
        {
            // Get near and far points on the ray
            Vector3 v1 = new Vector3(screenX, screenY, this.viewPort.MinZ);
            v1.Unproject(this.viewPort, this.m_absoluteProjectionMatrix, this.m_absoluteViewMatrix, this.m_absoluteWorldMatrix);
            Vector3 v2 = new Vector3(screenX, screenY, this.viewPort.MaxZ);
            v2.Unproject(this.viewPort, this.m_absoluteProjectionMatrix, this.m_absoluteViewMatrix, this.m_absoluteWorldMatrix);
            Point3d p1 = new Point3d(v1.X, v1.Y, v1.Z);
            Point3d p2 = new Point3d(v2.X, v2.Y, v2.Z);
            // Find intersection
            RayCasting.RayIntersectionWithTerrain(p1, p2, 100, 1, out latitude, out longitude, world);
        }

		/// <summary>
		///  Calculates the projection transformation matrix, which transforms 3-D camera or 
		///  view space coordinates into 2-D screen coordinates.
		/// </summary>
		protected virtual void ComputeProjectionMatrix(Viewport viewport)
		{
			float aspectRatio =  (float)viewport.Width / viewport.Height;
            float zNear = (float)Math.Max(this._altitude - this.TerrainElevationUnderCamera, minimumAltitude) * 0.1f;
            double distToCenterOfPlanet = (this._altitude + this.WorldRadius);
			double tangentalDistance  = Math.Sqrt( distToCenterOfPlanet*distToCenterOfPlanet - this._worldRadius* this._worldRadius);
            if (tangentalDistance < 1000000 || double.IsNaN(tangentalDistance))
                tangentalDistance = 1000000;
            this.m_ProjectionMatrix = Matrix.PerspectiveFovRH( (float) this._fov.Radians, aspectRatio, zNear, (float)tangentalDistance );
		}

		public virtual void RotationYawPitchRoll(Angle yaw, Angle pitch, Angle roll)
		{
		//	this._orientation *= MathEngine.EulerToQuaternion(yaw.Radians, pitch.Radians, roll.Radians);
		//	Vector3 v = MathEngine.QuaternionToEuler(this._orientation);
			
		//	if(!double.IsNaN(v.Y))
		//		this._latitude.Radians = v.Y;
		//	if(!double.IsNaN(v.X))
		//		this._longitude.Radians = v.X;
		//	if(Math.Abs(roll.Radians)>Single.Epsilon)
		//		this._heading.Radians = v.Z;


        this.m_Orientation = Quaternion4d.EulerToQuaternion(yaw.Radians, pitch.Radians, roll.Radians) * this.m_Orientation;

			Point3d p = Quaternion4d.QuaternionToEuler(this.m_Orientation);
			if(!double.IsNaN(p.Y)) this._latitude.Radians = p.Y;
			if(!double.IsNaN(p.X)) this._longitude.Radians = p.X;
			if(Math.Abs(roll.Radians) > double.Epsilon) this._heading.Radians = p.Z;
		}

		/// <summary>
		/// Digital zoom (keyboard/mouse wheel style)
		/// </summary>
		/// <param name="ticks">Positive value for zoom in, negative for zoom out.</param>
		public virtual void ZoomStepped( float ticks )
		{
			int currentTickCount = Environment.TickCount;

			double factor = World.Settings.cameraZoomStepFactor;
			if(factor<0)
				factor = 0;
			if (factor > 1)
				factor = 1;

			double minTime = 50;  // <= 50ms: fastest
			double maxTime = 250; // >=250ms: slowest
			double time = currentTickCount - this.lastStepZoomTickCount;
			if (time<minTime)
				time = minTime;
			double multiplier = 1-Math.Abs( (time-minTime)/maxTime ); // Range: 1 .. 2
			if(multiplier<0)
				multiplier=0;

			multiplier= multiplier * World.Settings.cameraZoomAcceleration;
			double mulfac = Math.Pow(1 - factor, multiplier+1 );
			mulfac = Math.Pow(mulfac, Math.Abs(ticks));

			if (ticks > 0)
                this.TargetDistance *= mulfac;
			else
                this.TargetDistance /= mulfac;

            this.lastStepZoomTickCount = currentTickCount;
		}

		/// <summary>
		/// Zoom camera in/out (distance) 
		/// </summary>
		/// <param name="percent">Positive value = zoom in, negative=out</param>
		public virtual void Zoom(float percent)
		{
			if(percent>0)
                this.TargetDistance /= 1.0f + percent;
			else
                this.TargetDistance *= 1.0f - percent;
		}

		/// <summary>
		/// Pan the camera using delta values
		/// </summary>
		/// <param name="lat">Latitude offset</param>
		/// <param name="lon">Longitude offset</param>
		public virtual void Pan(Angle lat, Angle lon)
		{
			if(Angle.IsNaN(lat)) lat = this._latitude; // should be zero (PM 2007-05)
			if(Angle.IsNaN(lon)) lon = this._longitude;
			lat += this._latitude;
			lon += this._longitude;

		//	this._orientation = MathEngine.EulerToQuaternion(
		//		lon.Radians,
		//		lat.Radians,
		//		_heading.Radians);

        this.m_Orientation = Quaternion4d.EulerToQuaternion(
				lon.Radians, lat.Radians, this._heading.Radians);

			Point3d p = Quaternion4d.QuaternionToEuler(this.m_Orientation);

		//	Vector3 v = MathEngine.QuaternionToEuler(this._orientation);
		//	if(!double.IsNaN(v.Y))
		//	{
		//		this._latitude.Radians = v.Y;
		//		this._longitude.Radians = v.X;
		//	}

			if(!double.IsNaN(p.Y))
			{
                this._latitude.Radians = p.Y;
                this._longitude.Radians = p.X;
			}
		}

        protected void ComputeDistanceOld(double altitude, Angle tilt)
        {
            double cos = Math.Cos(Math.PI - tilt.Radians);
            double x = this._worldRadius * cos;
            double hyp = this._worldRadius + altitude;
            double y = Math.Sqrt(this._worldRadius * this._worldRadius * cos * cos + hyp * hyp - this._worldRadius * this._worldRadius);
            double res = x - y;
            if (res < 0)
                res = x + y;
            this._distance = res;
        }

        protected void ComputeDistance(double altitude, Angle tilt)
        {
            double hyp = this._worldRadius + altitude;
            double a = (this._worldRadius + this.curCameraElevation) * Math.Sin(tilt.Radians);
            double b = Math.Sqrt(hyp * hyp - a * a);
            double c = (this._worldRadius + this.curCameraElevation) * Math.Cos(tilt.Radians);
            this._distance = b - c;
        }

        protected void ComputeAltitude(double distance, Angle tilt)
        {
            double radius = this._worldRadius + this.curCameraElevation;
            double dfromeq = Math.Sqrt(radius * radius + distance * distance -
                2 * radius * distance * Math.Cos(Math.PI - tilt.Radians));
            double alt = dfromeq - this._worldRadius;
            this._altitude = alt;
        }

        protected void ComputeAltitude(double distance, Angle tilt, Angle swivel)
        {
            Angle angle = tilt;
            if (Math.Abs(swivel.Degrees) > Math.Abs(tilt.Degrees)) angle = swivel;

            double radius = this._worldRadius + this.curCameraElevation;
            double dfromeq = Math.Sqrt(radius * radius + distance * distance -
                2 * radius * distance * Math.Cos(Math.PI - angle.Radians));
            double alt = dfromeq - this._worldRadius;
            this._altitude = alt;
        }

		protected void ComputeTilt(double alt, double distance )
		{
			double a = this._worldRadius + alt;
			double b = distance;
			double c = this._worldRadius + this.curCameraElevation;
            //_tilt.Radians = Math.Acos((a * a + b * b - c * c) / (2 * a * b)); // Wrong angle (PM 2007-05)
            this._tilt.Radians = Math.PI - Math.Acos((c * c + b * b - a * a) / (2 * c * b));
        }

		/// <summary>
		/// Projects a point from world to screen coordinates.
		/// </summary>
		/// <param name="point">Point in world space</param>
		/// <returns>Point in screen space</returns>
		public Vector3 Project( Vector3 point )
		{
			point.Project(this.viewPort, this.m_ProjectionMatrix, this.m_ViewMatrix, this.m_WorldMatrix);
			return point;
		}

		public override string ToString()
		{
			string res = string.Format(CultureInfo.InvariantCulture,
				"Altitude: {6:f0}m\nView Range: {0}\nHeading: {1}\nTilt: {2}\nFOV: {7}\nPosition: ({3}, {4} @ {5:f0}m)", this.ViewRange, this._heading, this._tilt, this._latitude, this._longitude, this._distance, this._altitude, this._fov);
			return res;
		}
		
		/// <summary>
		/// Gets the visible bounding box for the application in degrees.
		/// </summary>
		/// <returns>An array of Angles in minx.miny,maxx, maxy order</returns>
		public static Angle[] getViewBoundingBox()
		{
			// TODO: Correct the ViewRange for non-square windows. Is is accurate horizontally
			// but not vertically.
			Angle[] bbox = new Angle[4];
			
			// HACK: need to deal with startup of World (nothing is instantiated yet)
			if (DrawArgs.Camera !=null)
			{
				Angle lat = DrawArgs.Camera.Latitude;
				Angle lon = DrawArgs.Camera.Longitude;
				Angle vr  = DrawArgs.Camera.ViewRange;
				
				Angle North = lat + (0.5 * vr);
				Angle South = lat - (0.5 * vr);
				Angle East  = lon + (0.5 * vr);
				Angle West  = lon - (0.5 * vr);
				
				//minX(West), minY(South), maxX(East), MaxY(North)
				bbox[0] = West; bbox[1] = South; 
				bbox[2] = East; bbox[3] = North;
			}
			else
			{
				bbox[0] = Angle.FromDegrees(-180.0); bbox[1]= Angle.FromDegrees(-90.0);
				bbox[2] = Angle.FromDegrees(180.0);  bbox[3]= Angle.FromDegrees(90.0);
			}
			return bbox; 
		}
	}
}
