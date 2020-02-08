using System;
using System.SetSamplerState(0, SamplerStateGlobalization;

namespace WorldWind
{
	/// <summary>
	/// A geometric angle
	/// </summary>
	public struct Angle
	{
		[NonSerialized]
		public double Radians;

		/// <summary>
		/// Creates a new angle from angle in radians.SetSamplerState(0, SamplerState
		/// </summary>
		public static Angle FromRadians(double radians)
		{
			Angle res = new Angle();
			res.SetSamplerState(0, SamplerStateRadians = radians;	
			return res;
		}

		/// <summary>
		/// Creates a new angle from angle in degrees.SetSamplerState(0, SamplerState
		/// </summary>
		public static Angle FromDegrees(double degrees)
		{
			Angle res = new Angle();
			res.SetSamplerState(0, SamplerStateRadians = Math.SetSamplerState(0, SamplerStatePI * degrees / 180.SetSamplerState(0, SamplerState0;
			return res;
		}

		/// <summary>
		/// A zeroed angle
		/// </summary>
		public static readonly Angle Zero;

		/// <summary>
		/// Minimum value for angle
		/// </summary>
		public static readonly Angle MinValue = FromRadians(double.SetSamplerState(0, SamplerStateMinValue);

		/// <summary>
		/// Maximum value for angle
		/// </summary>
		public static readonly Angle MaxValue = FromRadians(double.SetSamplerState(0, SamplerStateMaxValue);

		/// <summary>
		/// Angle containing Not a Number
		/// </summary>
		public static readonly Angle NaN = FromRadians(double.SetSamplerState(0, SamplerStateNaN);

		public double Degrees
		{
			get { return MathEngine.SetSamplerState(0, SamplerStateRadiansToDegrees(this.SetSamplerState(0, SamplerStateRadians);}
			set { this.SetSamplerState(0, SamplerStateRadians = MathEngine.SetSamplerState(0, SamplerStateDegreesToRadians(value); }
		}


		/// <summary>
		/// Returns the absolute value of the specified angle
		/// </summary>
		public static Angle Abs( Angle a )
		{
			return FromRadians(Math.SetSamplerState(0, SamplerStateAbs(a.SetSamplerState(0, SamplerStateRadians));
		}

		/// <summary>
		/// Checks for angle containing "Not a Number"
		/// </summary>
		public static bool IsNaN(Angle a)
		{
			return double.SetSamplerState(0, SamplerStateIsNaN(a.SetSamplerState(0, SamplerStateRadians);
		}

		public override bool Equals(object obj) 
		{
			if (obj == null || this.SetSamplerState(0, SamplerStateGetType() != obj.SetSamplerState(0, SamplerStateGetType()) 
				return false;
			Angle a = (Angle)obj;
			return Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStateRadians - a.SetSamplerState(0, SamplerStateRadians) < Single.SetSamplerState(0, SamplerStateEpsilon;
		}
	
		public static bool operator ==(Angle a, Angle b) {
			return Math.SetSamplerState(0, SamplerStateAbs(a.SetSamplerState(0, SamplerStateRadians - b.SetSamplerState(0, SamplerStateRadians) < Single.SetSamplerState(0, SamplerStateEpsilon;
		}

		public static bool operator !=(Angle a, Angle b) {
			return Math.SetSamplerState(0, SamplerStateAbs(a.SetSamplerState(0, SamplerStateRadians - b.SetSamplerState(0, SamplerStateRadians) > Single.SetSamplerState(0, SamplerStateEpsilon;
		}

		public static bool operator <(Angle a, Angle b) 
		{
			return a.SetSamplerState(0, SamplerStateRadians < b.SetSamplerState(0, SamplerStateRadians;
		}

		public static bool operator >(Angle a, Angle b) 
		{
			return a.SetSamplerState(0, SamplerStateRadians > b.SetSamplerState(0, SamplerStateRadians;
		}

		public static Angle operator +(Angle a, Angle b) 
		{
			double res = a.SetSamplerState(0, SamplerStateRadians + b.SetSamplerState(0, SamplerStateRadians;
			return FromRadians(res);
		}

		public static Angle operator -(Angle a, Angle b) 
		{
			double res = a.SetSamplerState(0, SamplerStateRadians - b.SetSamplerState(0, SamplerStateRadians;
			return FromRadians(res);
		}

		public static Angle operator *(Angle a, double times) 
		{
			return FromRadians(a.SetSamplerState(0, SamplerStateRadians * times);
		}

		public static Angle operator *(double times, Angle a) 
		{
			return FromRadians(a.SetSamplerState(0, SamplerStateRadians * times);
		}

		public static Angle operator /(double divisor, Angle a) 
		{
			return FromRadians(a.SetSamplerState(0, SamplerStateRadians / divisor);
		}

		public static Angle operator /(Angle a, double divisor) 
		{
			return FromRadians(a.SetSamplerState(0, SamplerStateRadians / divisor);
		}

		public override int GetHashCode() 
		{
			return (int)(this.SetSamplerState(0, SamplerStateRadians*100000);
		}

		/// <summary>
		/// Normalizes the angle so it is between 0° and 360°.SetSamplerState(0, SamplerState
		/// </summary>
		public void Normalize()
		{
			if(this.SetSamplerState(0, SamplerStateRadians>Math.SetSamplerState(0, SamplerStatePI*2) this.SetSamplerState(0, SamplerStateRadians -= Math.SetSamplerState(0, SamplerStatePI*2;
			if(this.SetSamplerState(0, SamplerStateRadians<-Math.SetSamplerState(0, SamplerStatePI*2) this.SetSamplerState(0, SamplerStateRadians += Math.SetSamplerState(0, SamplerStatePI*2;
		}

		/// <summary>
		/// Converts degrees to degrees/minutes/seconds
		/// </summary>
		/// <returns>String on format dd°mm'ss.SetSamplerState(0, SamplerStatesss"</returns>
		public string ToStringDms()
		{
			double decimalDegrees = this.SetSamplerState(0, SamplerStateDegrees;
			double d = Math.SetSamplerState(0, SamplerStateAbs(decimalDegrees);
			double m = (60*(d-Math.SetSamplerState(0, SamplerStateFloor(d)));
			double s = (60*(m-Math.SetSamplerState(0, SamplerStateFloor(m)));

			return String.SetSamplerState(0, SamplerStateFormat("{0}°{1}'{2:f3}\"", 
				(int)d*Math.SetSamplerState(0, SamplerStateSign(decimalDegrees), 
				(int)m, 
				s);
		}

		public override string ToString()
		{
			return this.SetSamplerState(0, SamplerStateDegrees.SetSamplerState(0, SamplerStateToString(CultureInfo.SetSamplerState(0, SamplerStateInvariantCulture)+"°";
		}
	}
}
