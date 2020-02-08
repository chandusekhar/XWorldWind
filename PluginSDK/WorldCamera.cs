using System;

namespace WorldWind
{
	/// <summary>
	/// The "normal" camera
	/// </summary>
	public class WorldCamera : CameraBase
	{
		protected Angle _targetLatitude;
		protected Angle _targetLongitude;
		protected double _targetAltitude;
		protected double _targetDistance;
		protected Angle _targetHeading;
		protected Angle _targetBank;
		protected Angle _targetTilt;
		protected Angle _targetFov;
		protected Quaternion4d _targetOrientation;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.WorldCamera"/> class.
		/// </summary>
		/// <param name="targetPosition"></param>
		/// <param name="radius"></param>
		public WorldCamera( Vector3 targetPosition,double radius ) : base( targetPosition, radius ) 
		{
			this._targetOrientation = this.m_Orientation;
			this._targetDistance = this._distance;
			this._targetAltitude = this._altitude;
			this._targetTilt = this._tilt;
			this._targetFov = this._fov;
		}

		public override void SetPosition(double lat, double lon, double heading, double _altitude, double tilt, double bank)
		{
			if(double.IsNaN(lat)) lat = this._latitude.Degrees;
			if(double.IsNaN(lon)) lon = this._longitude.Degrees;
			if(double.IsNaN(heading)) heading = this._heading.Degrees;
			if(double.IsNaN(bank)) bank = this._targetBank.Degrees;

			this._targetOrientation = Quaternion4d.EulerToQuaternion(
				MathEngine.DegreesToRadians(lon),
				MathEngine.DegreesToRadians(lat),
				MathEngine.DegreesToRadians(heading));

			Point3d v = Quaternion4d.QuaternionToEuler(this._targetOrientation);


			this._targetLatitude.Radians = v.Y;
			this._targetLongitude.Radians = v.X;
			this._targetHeading.Radians = v.Z;

			if(!double.IsNaN(tilt))
				this.Tilt = Angle.FromDegrees(tilt);
			if(!double.IsNaN(_altitude)) this.Altitude = _altitude;
			this.Bank = Angle.FromDegrees(bank);
		}

		public override Angle Heading
		{
			get { return this._heading; }
			set
			{
                this._heading = value;
                this._targetHeading = value;
			}
		}

		Angle angle;
		protected void SlerpToTargetOrientation(double percent)
		{
			double c = Quaternion4d.Dot(this.m_Orientation, this._targetOrientation);

			if (c > 1.0)
				c = 1.0;
			else if (c < -1.0)
				c = -1.0;

            this.angle = Angle.FromRadians(Math.Acos(c));

            this.m_Orientation = Quaternion4d.Slerp(this.m_Orientation, this._targetOrientation, percent);

            this._tilt += (this._targetTilt - this._tilt)*percent;
            this._bank += (this._targetBank - this._bank)*percent;
            this._distance += (this._targetDistance - this._distance)*percent;
            this.ComputeAltitude(this._distance + this._headZoom, this._tilt + this._headTilt, this._swivel + this._headSwivel);
            this._fov += (this._targetFov - this._fov) * percent;
		}

		#region Public properties

		public override double TargetAltitude
		{
			get
			{
				return this._targetAltitude;
			}
			set
			{
				if(value < this._terrainElevationUnderCamera * World.Settings.VerticalExaggeration + minimumAltitude)
                    value = this._terrainElevationUnderCamera * World.Settings.VerticalExaggeration + minimumAltitude;
				if(value > maximumAltitude)
                    value = maximumAltitude;
				this._targetAltitude = value;
                this.ComputeTargetDistance(this._targetAltitude, this._targetTilt);
			}
		}

		public Angle TargetHeading
		{
			get { return this._targetHeading; }
			set 
			{
				this._targetHeading = value; 
			}
		}
		
		public Angle TargetLatitude
		{
			get { return this._targetLatitude; }
			set { this._targetLatitude = value; }
		}
		
		public Angle TargetLongitude
		{
			get { return this._targetLongitude; }
			set { this._targetLongitude = value; }
		}
		
		public Quaternion4d TargetOrientation
		{
			get { return this._targetOrientation; }
			set { this._targetOrientation = value; }
		}
		
		public override Angle Fov
		{
			get { return this._targetFov; }
			set { 
				if(value > World.Settings.cameraFovMax)
					value = World.Settings.cameraFovMax;
				if(value < World.Settings.cameraFovMin)
					value = World.Settings.cameraFovMin;
				this._targetFov = value; 
			}
		}

		#endregion


		#region ICamera interface

		public override void Update(Device device)
		{
            // Move camera
            this.SlerpToTargetOrientation(World.Settings.cameraSlerpPercentage);
            // Check for terrain collision
            if (this._altitude < this._terrainElevationUnderCamera * World.Settings.VerticalExaggeration + minimumAltitude)
			{
                this._targetAltitude = this._terrainElevationUnderCamera * World.Settings.VerticalExaggeration + minimumAltitude;
                this._altitude = this._targetAltitude;
				//ComputeTargetDistance( _targetAltitude, _targetTilt );
                this.ComputeTargetTilt(this._targetAltitude, this._distance);
                if (Angle.IsNaN(this._targetTilt))
                {
                    this._targetTilt.Degrees = 0;
                    this.ComputeTargetDistance(this._targetAltitude, this._targetTilt);
                    this._distance = this._targetDistance;
                }

                this._tilt = this._targetTilt;
			}
            // Update camera base
			base.Update(device);
		}

		#endregion
	
		public override string ToString()
		{
			string res = base.ToString() + 
				string.Format(
				"\nTarget: ({0}, {1} @ {2:f0}m)\nTarget Altitude: {3:f0}m", this._targetLatitude, this._targetLongitude, this._targetDistance, this._targetAltitude ) + "\nAngle: " + this.angle;
			return res;
		}
	
		public override void RotationYawPitchRoll(Angle yaw, Angle pitch, Angle roll)
		{
            this._targetOrientation = Quaternion4d.EulerToQuaternion(yaw.Radians, pitch.Radians, roll.Radians) * this._targetOrientation;
			
			Point3d v = Quaternion4d.QuaternionToEuler(this._targetOrientation);
			if(!double.IsNaN(v.Y))
				this._targetLatitude.Radians = v.Y;
			if(!double.IsNaN(v.X))
				this._targetLongitude.Radians = v.X;
			if(Math.Abs(roll.Radians)>double.Epsilon)
				this._targetHeading.Radians = v.Z;
		}
	
		public override Angle Bank
		{
			get { return this._targetBank; }
			set
			{
				if(Angle.IsNaN(value))
					return;

                this._targetBank = value;
				if(!World.Settings.cameraSmooth) this._bank = value;
			}
		}

		public override Angle Tilt
		{
			get { return this._targetTilt; }
			set
			{
                if (value > maxTilt)
                    value = maxTilt;
                else
                {
                    if (World.Settings.AllowNegativeTilt)
                    {
                        if (value < minNegTilt) value = minNegTilt;
                    }
                    else
                    {
                        if (value < minTilt) value = minTilt;
                    }
                }

                this._targetTilt = value;
                this.ComputeTargetAltitude(this._targetDistance, this._targetTilt);
				if(!World.Settings.cameraSmooth) this._tilt = value;
			}
		}
	
		public override double TargetDistance
		{
			get
			{
				return this._targetDistance;
			}
			set
			{
				if(value < minimumAltitude)
					value = minimumAltitude;
				if(value > maximumAltitude)
					value = maximumAltitude;
                this._targetDistance = value;
                this.ComputeTargetAltitude(this._targetDistance, this._targetTilt );
				if(!World.Settings.cameraSmooth)
				{
					this._distance = this._targetDistance;
 					this._altitude = this._targetAltitude;
				}
			}
		}
	
		/// <summary>
		/// Zoom camera in/out (distance) 
		/// </summary>
		/// <param name="percent">Positive value = zoom in, negative=out</param>
		public override void Zoom(float percent)
		{
			if(percent>0)
			{
				// In
				double factor = 1.0f + percent;
                this.TargetDistance /= factor;
			}
			else
			{
				// Out
				double factor = 1.0f - percent;
                this.TargetDistance *= factor;
			}
		}

		protected void ComputeTargetDistanceOld( double altitude, Angle tilt )
		{
			double cos = Math.Cos(Math.PI - tilt.Radians);
			double x = this._worldRadius * cos;
			double hyp = this._worldRadius + altitude;
			double y = Math.Sqrt(this._worldRadius* this._worldRadius*cos*cos+hyp*hyp- this._worldRadius* this._worldRadius);
			double res = x-y;
			if(res<0)
				res = x+y;
            this._targetDistance = res;
		}

        protected void ComputeTargetDistance(double altitude, Angle tilt)
        {
            double hyp = this._worldRadius + altitude;
            double a = (this._worldRadius + this.curCameraElevation) * Math.Sin(tilt.Radians);
            double b = Math.Sqrt(hyp * hyp - a * a);
            double c = (this._worldRadius + this.curCameraElevation) * Math.Cos(tilt.Radians);
            this._targetDistance = b - c;
        }

		protected void ComputeTargetAltitude( double distance, Angle tilt )
		{
            double radius = this._worldRadius + this.curCameraElevation;
            double dfromeq = Math.Sqrt(radius * radius + distance * distance -
                2 * radius * distance * Math.Cos(Math.PI - tilt.Radians));
			double alt = dfromeq - this._worldRadius;
            this._targetAltitude = alt;
		}

        protected void ComputeTargetTilt(double alt, double distance)
        {
            double a = this._worldRadius + alt;
            double b = distance;
            double c = this._worldRadius + this.curCameraElevation;
            this._targetTilt.Radians = Math.PI - Math.Acos((c * c + b * b - a * a) / (2 * c * b));
        }

	}

	/// <summary>
	/// Normal camera with MomentumCamera. (perhaps merge with the normal camera)
	/// </summary>
	public class MomentumCamera : WorldCamera
	{
		protected Angle _latitudeMomentum;
		protected Angle _longitudeMomentum;
		protected Angle _headingMomentum;
		//protected Angle _bankMomentum;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.MomentumCamera"/> class.
		/// </summary>
		/// <param name="targetPosition"></param>
		/// <param name="radius"></param>
		public MomentumCamera( Vector3 targetPosition,double radius ) : base( targetPosition, radius ) 
		{
			this._targetOrientation = this.m_Orientation;
			this._targetDistance = this._distance;
			this._targetAltitude = this._altitude;
			this._targetTilt = this._tilt;
		}

		public bool HasMomentum
		{
			get { return World.Settings.cameraHasMomentum; }
			set { 
				World.Settings.cameraHasMomentum = value;
				this._latitudeMomentum.Radians = 0;
				this._longitudeMomentum.Radians = 0;
				this._headingMomentum.Radians = 0;
			}
		}

		public override void RotationYawPitchRoll(Angle yaw, Angle pitch, Angle roll)
		{
			if(World.Settings.cameraHasMomentum)
			{
                this._latitudeMomentum += pitch/100;
                this._longitudeMomentum += yaw/100;
                this._headingMomentum += roll/100;
			}

			this._targetOrientation = Quaternion4d.EulerToQuaternion( yaw.Radians, pitch.Radians, roll.Radians ) * this._targetOrientation;
			Point3d v = Quaternion4d.QuaternionToEuler(this._targetOrientation);
			if(!double.IsNaN(v.Y))
			{
				this._targetLatitude.Radians = v.Y;
				this._targetLongitude.Radians = v.X;
				if(!World.Settings.cameraTwistLock) this._targetHeading.Radians = v.Z;
			}

			base.RotationYawPitchRoll(yaw,pitch,roll);
		}

		/// <summary>
		/// Pan the camera using delta values
		/// </summary>
		/// <param name="lat">Latitude offset</param>
		/// <param name="lon">Longitude offset</param>
		public override void Pan(Angle lat, Angle lon)
		{
			if(World.Settings.cameraHasMomentum)
			{
                this._latitudeMomentum += lat/100;
                this._longitudeMomentum += lon/100;
			}

			if(Angle.IsNaN(lat)) lat = this._targetLatitude;
			if(Angle.IsNaN(lon)) lon = this._targetLongitude;
			lat += this._targetLatitude;
			lon += this._targetLongitude;

			if(Math.Abs(lat.Radians)>Math.PI/2-1e-3)
			{
				lat.Radians = Math.Sign(lat.Radians)*(Math.PI/2 - 1e-3);
			}

			this._targetOrientation = Quaternion4d.EulerToQuaternion(
				lon.Radians,
				lat.Radians, this._targetHeading.Radians);

			Point3d v = Quaternion4d.QuaternionToEuler(this._targetOrientation);
			if(!double.IsNaN(v.Y))
			{
                this._targetLatitude.Radians = v.Y;
                this._targetLongitude.Radians = v.X;
                this._targetHeading.Radians = v.Z;

				if(!World.Settings.cameraSmooth)
				{
                    this._latitude = this._targetLatitude;
                    this._longitude = this._targetLongitude;
                    this._heading = this._targetHeading;
                    this.m_Orientation = this._targetOrientation;
				}
			}
		}

		public override void Update(Device device)
		{
			if(World.Settings.cameraHasMomentum)
			{
				base.RotationYawPitchRoll(this._longitudeMomentum, this._latitudeMomentum, this._headingMomentum );
			}

			base.Update(device);
		}

		public override void SetPosition(double lat, double lon, double heading, double _altitude, double tilt, double bank)
		{
            this._latitudeMomentum.Radians = 0;
            this._longitudeMomentum.Radians = 0;
            this._headingMomentum.Radians = 0;

			base.SetPosition(lat,lon,heading, _altitude, tilt, bank );
		}

		public override string ToString()
		{
			string res = base.ToString() + 
				string.Format(
				"\nMomentum: {0}, {1}, {2}", this._latitudeMomentum, this._longitudeMomentum, this._headingMomentum );
			return res;
		}
	}
}