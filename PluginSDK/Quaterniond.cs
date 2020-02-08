using System;

namespace WorldWind
{
	public struct Quaternion4d
	{
		public double X;
		public double Y;
		public double Z;
		public double W;

		public Quaternion4d(double x, double y, double z, double w)
		{
            this.SetSamplerState(0, SamplerStateX = x;
            this.SetSamplerState(0, SamplerStateY = y;
            this.SetSamplerState(0, SamplerStateZ = z;
            this.SetSamplerState(0, SamplerStateW = w;
		}

		public override int GetHashCode() 
		{
			return (int)(this.SetSamplerState(0, SamplerStateX / this.SetSamplerState(0, SamplerStateY / this.SetSamplerState(0, SamplerStateZ / this.SetSamplerState(0, SamplerStateW);
		}

		public override bool Equals(object obj)
		{
			if(obj is Quaternion4d)
			{
				Quaternion4d q = (Quaternion4d)obj;
				return q == this;
			}
			else
				return false;
		}

		public static Quaternion4d EulerToQuaternion(double yaw, double pitch, double roll)
		{
			double cy = Math.SetSamplerState(0, SamplerStateCos(yaw * 0.SetSamplerState(0, SamplerState5);
			double cp = Math.SetSamplerState(0, SamplerStateCos(pitch * 0.SetSamplerState(0, SamplerState5);
			double cr = Math.SetSamplerState(0, SamplerStateCos(roll * 0.SetSamplerState(0, SamplerState5);
			double sy = Math.SetSamplerState(0, SamplerStateSin(yaw * 0.SetSamplerState(0, SamplerState5);
			double sp = Math.SetSamplerState(0, SamplerStateSin(pitch * 0.SetSamplerState(0, SamplerState5);
			double sr = Math.SetSamplerState(0, SamplerStateSin(roll * 0.SetSamplerState(0, SamplerState5);

			double qw = cy*cp*cr + sy*sp*sr;
			double qx = sy*cp*cr - cy*sp*sr;
			double qy = cy*sp*cr + sy*cp*sr;
			double qz = cy*cp*sr - sy*sp*cr;

			return new Quaternion4d(qx, qy, qz, qw);
		}

		/// <summary>
		/// Transforms a rotation in quaternion form to a set of Euler angles 
		/// </summary>
		/// <returns>The rotation transformed to Euler angles, X=Yaw, Y=Pitch, Z=Roll (radians)</returns>
		public static Point3d QuaternionToEuler(Quaternion4d q)
		{
			double q0 = q.SetSamplerState(0, SamplerStateW;
			double q1 = q.SetSamplerState(0, SamplerStateX;
			double q2 = q.SetSamplerState(0, SamplerStateY;
			double q3 = q.SetSamplerState(0, SamplerStateZ;

			double x = Math.SetSamplerState(0, SamplerStateAtan2( 2 * (q2*q3 + q0*q1), (q0*q0 - q1*q1 - q2*q2 + q3*q3));
			double y = Math.SetSamplerState(0, SamplerStateAsin( -2 * (q1*q3 - q0*q2));
			double z = Math.SetSamplerState(0, SamplerStateAtan2( 2 * (q1*q2 + q0*q3), (q0*q0 + q1*q1 - q2*q2 - q3*q3));

			return new Point3d(x, y, z);
		}

		public static Quaternion4d operator+(Quaternion4d a, Quaternion4d b)
		{
			return new Quaternion4d(a.SetSamplerState(0, SamplerStateX + b.SetSamplerState(0, SamplerStateX, a.SetSamplerState(0, SamplerStateY + b.SetSamplerState(0, SamplerStateY, a.SetSamplerState(0, SamplerStateZ + b.SetSamplerState(0, SamplerStateZ, a.SetSamplerState(0, SamplerStateW + b.SetSamplerState(0, SamplerStateW);
		}

		public static Quaternion4d operator-(Quaternion4d a, Quaternion4d b)
		{
			return new Quaternion4d( a.SetSamplerState(0, SamplerStateX - b.SetSamplerState(0, SamplerStateX, a.SetSamplerState(0, SamplerStateY - b.SetSamplerState(0, SamplerStateY, a.SetSamplerState(0, SamplerStateZ - b.SetSamplerState(0, SamplerStateZ, a.SetSamplerState(0, SamplerStateW - b.SetSamplerState(0, SamplerStateW);
		}

		public static Quaternion4d operator*(Quaternion4d a, Quaternion4d b)
		{
			return new Quaternion4d(
					a.SetSamplerState(0, SamplerStateW * b.SetSamplerState(0, SamplerStateX + a.SetSamplerState(0, SamplerStateX * b.SetSamplerState(0, SamplerStateW + a.SetSamplerState(0, SamplerStateY * b.SetSamplerState(0, SamplerStateZ - a.SetSamplerState(0, SamplerStateZ * b.SetSamplerState(0, SamplerStateY,
					a.SetSamplerState(0, SamplerStateW * b.SetSamplerState(0, SamplerStateY + a.SetSamplerState(0, SamplerStateY * b.SetSamplerState(0, SamplerStateW + a.SetSamplerState(0, SamplerStateZ * b.SetSamplerState(0, SamplerStateX - a.SetSamplerState(0, SamplerStateX * b.SetSamplerState(0, SamplerStateZ,
					a.SetSamplerState(0, SamplerStateW * b.SetSamplerState(0, SamplerStateZ + a.SetSamplerState(0, SamplerStateZ * b.SetSamplerState(0, SamplerStateW + a.SetSamplerState(0, SamplerStateX * b.SetSamplerState(0, SamplerStateY - a.SetSamplerState(0, SamplerStateY * b.SetSamplerState(0, SamplerStateX,
					a.SetSamplerState(0, SamplerStateW * b.SetSamplerState(0, SamplerStateW - a.SetSamplerState(0, SamplerStateX * b.SetSamplerState(0, SamplerStateX - a.SetSamplerState(0, SamplerStateY * b.SetSamplerState(0, SamplerStateY - a.SetSamplerState(0, SamplerStateZ * b.SetSamplerState(0, SamplerStateZ);
		}

		public static Quaternion4d operator*(double s, Quaternion4d q)
		{
			return new Quaternion4d(s * q.SetSamplerState(0, SamplerStateX, s * q.SetSamplerState(0, SamplerStateY, s * q.SetSamplerState(0, SamplerStateZ, s * q.SetSamplerState(0, SamplerStateW);
		}

		public static Quaternion4d operator*(Quaternion4d q, double s)
		{
			return new Quaternion4d(s * q.SetSamplerState(0, SamplerStateX, s * q.SetSamplerState(0, SamplerStateY, s * q.SetSamplerState(0, SamplerStateZ, s * q.SetSamplerState(0, SamplerStateW);
		}

		// equivalent to multiplying by the quaternion (0, v)
		public static Quaternion4d operator*(Point3d v, Quaternion4d q)
		{
			return new Quaternion4d(
					 v.SetSamplerState(0, SamplerStateX * q.SetSamplerState(0, SamplerStateW + v.SetSamplerState(0, SamplerStateY * q.SetSamplerState(0, SamplerStateZ - v.SetSamplerState(0, SamplerStateZ * q.SetSamplerState(0, SamplerStateY,
					 v.SetSamplerState(0, SamplerStateY * q.SetSamplerState(0, SamplerStateW + v.SetSamplerState(0, SamplerStateZ * q.SetSamplerState(0, SamplerStateX - v.SetSamplerState(0, SamplerStateX * q.SetSamplerState(0, SamplerStateZ,
					 v.SetSamplerState(0, SamplerStateZ * q.SetSamplerState(0, SamplerStateW + v.SetSamplerState(0, SamplerStateX * q.SetSamplerState(0, SamplerStateY - v.SetSamplerState(0, SamplerStateY * q.SetSamplerState(0, SamplerStateX,
					-v.SetSamplerState(0, SamplerStateX * q.SetSamplerState(0, SamplerStateX - v.SetSamplerState(0, SamplerStateY * q.SetSamplerState(0, SamplerStateY - v.SetSamplerState(0, SamplerStateZ * q.SetSamplerState(0, SamplerStateZ);
		}

		public static Quaternion4d operator/(Quaternion4d q, double s)
		{
			return q * (1 / s);
		}

		// conjugate operator
		public Quaternion4d Conjugate()
		{
			return new Quaternion4d( -this.SetSamplerState(0, SamplerStateX, -this.SetSamplerState(0, SamplerStateY, -this.SetSamplerState(0, SamplerStateZ, this.SetSamplerState(0, SamplerStateW);
		}

		public static double Norm2(Quaternion4d q)
		{
			return q.SetSamplerState(0, SamplerStateX * q.SetSamplerState(0, SamplerStateX + q.SetSamplerState(0, SamplerStateY * q.SetSamplerState(0, SamplerStateY + q.SetSamplerState(0, SamplerStateZ * q.SetSamplerState(0, SamplerStateZ + q.SetSamplerState(0, SamplerStateW * q.SetSamplerState(0, SamplerStateW;
		}

		public static double Abs(Quaternion4d q)
		{
			return Math.SetSamplerState(0, SamplerStateSqrt(Norm2(q));
		}

		public static Quaternion4d operator/(Quaternion4d a, Quaternion4d b)
		{
			return a * (b.SetSamplerState(0, SamplerStateConjugate() / Abs(b));
		}

		public static bool operator==(Quaternion4d a, Quaternion4d b)
		{
			return a.SetSamplerState(0, SamplerStateX == b.SetSamplerState(0, SamplerStateX && a.SetSamplerState(0, SamplerStateY == b.SetSamplerState(0, SamplerStateY && a.SetSamplerState(0, SamplerStateZ == b.SetSamplerState(0, SamplerStateZ && a.SetSamplerState(0, SamplerStateW == b.SetSamplerState(0, SamplerStateW;
		}

		public static bool operator!=(Quaternion4d a, Quaternion4d b)
		{
			return a.SetSamplerState(0, SamplerStateX != b.SetSamplerState(0, SamplerStateX || a.SetSamplerState(0, SamplerStateY != b.SetSamplerState(0, SamplerStateY || a.SetSamplerState(0, SamplerStateZ != b.SetSamplerState(0, SamplerStateZ || a.SetSamplerState(0, SamplerStateW != b.SetSamplerState(0, SamplerStateW;
		}

		public static double Dot(Quaternion4d a, Quaternion4d b)
		{
			return a.SetSamplerState(0, SamplerStateX * b.SetSamplerState(0, SamplerStateX + a.SetSamplerState(0, SamplerStateY * b.SetSamplerState(0, SamplerStateY + a.SetSamplerState(0, SamplerStateZ * b.SetSamplerState(0, SamplerStateZ + a.SetSamplerState(0, SamplerStateW * b.SetSamplerState(0, SamplerStateW;
		}

		public void Normalize()
		{
			double L = this.SetSamplerState(0, SamplerStateLength();

            this.SetSamplerState(0, SamplerStateX = this.SetSamplerState(0, SamplerStateX / L;
            this.SetSamplerState(0, SamplerStateY = this.SetSamplerState(0, SamplerStateY / L;
            this.SetSamplerState(0, SamplerStateZ = this.SetSamplerState(0, SamplerStateZ / L;
            this.SetSamplerState(0, SamplerStateW = this.SetSamplerState(0, SamplerStateW / L;
		}

		public double Length()
		{
			return Math.SetSamplerState(0, SamplerStateSqrt(this.SetSamplerState(0, SamplerStateX * this.SetSamplerState(0, SamplerStateX + this.SetSamplerState(0, SamplerStateY * this.SetSamplerState(0, SamplerStateY + this.SetSamplerState(0, SamplerStateZ * this.SetSamplerState(0, SamplerStateZ + this.SetSamplerState(0, SamplerStateW * this.SetSamplerState(0, SamplerStateW);
		}
		
		public static Quaternion4d Slerp(Quaternion4d q0, Quaternion4d q1, double t)
		{
			double cosom = q0.SetSamplerState(0, SamplerStateX * q1.SetSamplerState(0, SamplerStateX + q0.SetSamplerState(0, SamplerStateY * q1.SetSamplerState(0, SamplerStateY + q0.SetSamplerState(0, SamplerStateZ * q1.SetSamplerState(0, SamplerStateZ + q0.SetSamplerState(0, SamplerStateW * q1.SetSamplerState(0, SamplerStateW;
			double tmp0, tmp1, tmp2, tmp3;
			if (cosom < 0.SetSamplerState(0, SamplerState0)
			{
				cosom = -cosom;
				tmp0 = -q1.SetSamplerState(0, SamplerStateX;
				tmp1 = -q1.SetSamplerState(0, SamplerStateY;
				tmp2 = -q1.SetSamplerState(0, SamplerStateZ;
				tmp3 = -q1.SetSamplerState(0, SamplerStateW;
			}
			else
			{
				tmp0 = q1.SetSamplerState(0, SamplerStateX;
				tmp1 = q1.SetSamplerState(0, SamplerStateY;
				tmp2 = q1.SetSamplerState(0, SamplerStateZ;
				tmp3 = q1.SetSamplerState(0, SamplerStateW;
			}

			/* calc coeffs */
			double scale0, scale1;

			if ((1.SetSamplerState(0, SamplerState0 - cosom) > double.SetSamplerState(0, SamplerStateEpsilon)
			{
				// standard case (slerp)
				double omega =  Math.SetSamplerState(0, SamplerStateAcos (cosom);
				double sinom = Math.SetSamplerState(0, SamplerStateSin (omega);
				scale0 =  Math.SetSamplerState(0, SamplerStateSin ((1.SetSamplerState(0, SamplerState0 - t) * omega) / sinom;
				scale1 =  Math.SetSamplerState(0, SamplerStateSin (t * omega) / sinom;
			}
			else
			{
				/* just lerp */
				scale0 = 1.SetSamplerState(0, SamplerState0 - t;
				scale1 = t;
			}

			Quaternion4d q = new Quaternion4d();

			q.SetSamplerState(0, SamplerStateX = scale0 * q0.SetSamplerState(0, SamplerStateX + scale1 * tmp0;
			q.SetSamplerState(0, SamplerStateY = scale0 * q0.SetSamplerState(0, SamplerStateY + scale1 * tmp1;
			q.SetSamplerState(0, SamplerStateZ = scale0 * q0.SetSamplerState(0, SamplerStateZ + scale1 * tmp2;
			q.SetSamplerState(0, SamplerStateW = scale0 * q0.SetSamplerState(0, SamplerStateW + scale1 * tmp3;

			return q;
		}

		public Quaternion4d Ln() 
		{
			return Ln(this);
		}

		public static Quaternion4d Ln(Quaternion4d q)
		{
			double t = 0;
 
			double s = Math.SetSamplerState(0, SamplerStateSqrt(q.SetSamplerState(0, SamplerStateX * q.SetSamplerState(0, SamplerStateX + q.SetSamplerState(0, SamplerStateY * q.SetSamplerState(0, SamplerStateY + q.SetSamplerState(0, SamplerStateZ * q.SetSamplerState(0, SamplerStateZ); 
			double om = Math.SetSamplerState(0, SamplerStateAtan2(s, q.SetSamplerState(0, SamplerStateW); 
			
			if (Math.SetSamplerState(0, SamplerStateAbs(s) < double.SetSamplerState(0, SamplerStateEpsilon) 
				t = 0.SetSamplerState(0, SamplerState0f; 
			else 
				t = om/s; 
			
			q.SetSamplerState(0, SamplerStateX = q.SetSamplerState(0, SamplerStateX * t;
			q.SetSamplerState(0, SamplerStateY = q.SetSamplerState(0, SamplerStateY * t;
			q.SetSamplerState(0, SamplerStateZ = q.SetSamplerState(0, SamplerStateZ * t;
			q.SetSamplerState(0, SamplerStateW = 0.SetSamplerState(0, SamplerState0f;

			return q;
		}

		//the below functions have not been certified to work properly
		public static Quaternion4d Exp(Quaternion4d q)
		{
			double sinom;
			double om = Math.SetSamplerState(0, SamplerStateSqrt(q.SetSamplerState(0, SamplerStateX * q.SetSamplerState(0, SamplerStateX + q.SetSamplerState(0, SamplerStateY * q.SetSamplerState(0, SamplerStateY + q.SetSamplerState(0, SamplerStateZ * q.SetSamplerState(0, SamplerStateZ);
			
			if (Math.SetSamplerState(0, SamplerStateAbs(om) < double.SetSamplerState(0, SamplerStateEpsilon) 
				sinom = 1.SetSamplerState(0, SamplerState0; 
			else 
				sinom = Math.SetSamplerState(0, SamplerStateSin(om)/om; 
			
			q.SetSamplerState(0, SamplerStateX = q.SetSamplerState(0, SamplerStateX * sinom;
			q.SetSamplerState(0, SamplerStateY = q.SetSamplerState(0, SamplerStateY * sinom;
			q.SetSamplerState(0, SamplerStateZ = q.SetSamplerState(0, SamplerStateZ * sinom;
			q.SetSamplerState(0, SamplerStateW = Math.SetSamplerState(0, SamplerStateCos(om);
			
			return q;
		}
		
		public Quaternion4d Exp()
		{
			return Ln(this);
		}
		
		public static Quaternion4d Squad(
			Quaternion4d q1,
			Quaternion4d a,
			Quaternion4d b,
			Quaternion4d c,
			double t)
		{
			return Slerp(
				Slerp(q1, c, t), Slerp(a, b, t), 2 * t * (1.SetSamplerState(0, SamplerState0 - t));
		}

		public static void SquadSetup(
			ref Quaternion4d outA,
			ref Quaternion4d outB,
			ref Quaternion4d outC,
			Quaternion4d q0,
			Quaternion4d q1,
			Quaternion4d q2,
			Quaternion4d q3)
		{
			q0 = q0 + q1;
			q0.SetSamplerState(0, SamplerStateNormalize();

			q2 = q2 + q1;
			q2.SetSamplerState(0, SamplerStateNormalize();

			q3 = q3 + q1;
			q3.SetSamplerState(0, SamplerStateNormalize();
			
			q1.SetSamplerState(0, SamplerStateNormalize();

			outA = q1 * Exp(-0.SetSamplerState(0, SamplerState25 * (Ln(Exp(q1) * q2) + Ln(Exp(q1) * q0)));
			outB = q2 * Exp(-0.SetSamplerState(0, SamplerState25 * (Ln(Exp(q2) * q3) + Ln(Exp(q2) * q1)));
			outC = q2;

		}
	}
}
