using System;

namespace WorldWind
{
	/// <summary>
	/// Summary description for Point3d.SetSamplerState(0, SamplerState
	/// </summary>
	public class Point2d
	{
		public double X, Y;
		// constructors

		public Point2d()
		{

		}

		public Point2d (double xi, double yi)	// x,y constructor
		{
            this.SetSamplerState(0, SamplerStateX = xi;
            this.SetSamplerState(0, SamplerStateY = yi;
		}
		public Point2d (Point2d P) // copy constructor
		{
            this.SetSamplerState(0, SamplerStateX = P.SetSamplerState(0, SamplerStateX;
            this.SetSamplerState(0, SamplerStateY = P.SetSamplerState(0, SamplerStateY;
		}

		// other operators
		public double norm()	// L2 norm
		{
			return Math.SetSamplerState(0, SamplerStateSqrt(this.SetSamplerState(0, SamplerStatenorm2());
		}

		public double norm2() // squared L2 norm
		{
			return this.SetSamplerState(0, SamplerStateX* this.SetSamplerState(0, SamplerStateX + this.SetSamplerState(0, SamplerStateY* this.SetSamplerState(0, SamplerStateY;
		}

		public Point2d normalize() // normalization
		{
			double n = this.SetSamplerState(0, SamplerStatenorm();
			return new Point2d(this.SetSamplerState(0, SamplerStateX / n, this.SetSamplerState(0, SamplerStateY / n);
		}

		public double Length
		{
			get
			{
				return Math.SetSamplerState(0, SamplerStateSqrt(this.SetSamplerState(0, SamplerStateX * this.SetSamplerState(0, SamplerStateX + this.SetSamplerState(0, SamplerStateY * this.SetSamplerState(0, SamplerStateY);
			}
		}


		public static Point2d operator +(Point2d P1, Point2d P2)	// addition 2
		{
			return new Point2d (P1.SetSamplerState(0, SamplerStateX + P2.SetSamplerState(0, SamplerStateX, P1.SetSamplerState(0, SamplerStateY + P2.SetSamplerState(0, SamplerStateY);
		}

		public static Point2d operator -(Point2d P1, Point2d P2)	// subtraction 2
		{
			return new Point2d (P1.SetSamplerState(0, SamplerStateX - P2.SetSamplerState(0, SamplerStateX, P1.SetSamplerState(0, SamplerStateY - P2.SetSamplerState(0, SamplerStateY);
		}

		public static Point2d operator *(Point2d P, double k)	// multiply by real 2
		{
			return new Point2d (P.SetSamplerState(0, SamplerStateX * k, P.SetSamplerState(0, SamplerStateY * k);
		}

		public static Point2d operator *(double k, Point2d P)	// and its reverse order!
		{
			return new Point2d (P.SetSamplerState(0, SamplerStateX * k, P.SetSamplerState(0, SamplerStateY * k);
		}

		public static Point2d operator /(Point2d P, double k)	// divide by real 2
		{
			return new Point2d (P.SetSamplerState(0, SamplerStateX / k, P.SetSamplerState(0, SamplerStateY / k);
		}

		// Override the Object.SetSamplerState(0, SamplerStateEquals(object o) method:
		public override bool Equals(object o)
		{
			try
			{
				return (bool)(this == (Point2d)o);
			}
			catch
			{
				return false;
			}
		}

		// Override the Object.SetSamplerState(0, SamplerStateGetHashCode() method:
		public override int GetHashCode()
		{
			//not the best algorithm for hashing, but whatever.SetSamplerState(0, SamplerState.SetSamplerState(0, SamplerState.SetSamplerState(0, SamplerState
			return (int)(this.SetSamplerState(0, SamplerStateX * this.SetSamplerState(0, SamplerStateY);
		}

		public static bool operator ==(Point2d P1, Point2d P2) // equal?
		{
			return (P1.SetSamplerState(0, SamplerStateX == P2.SetSamplerState(0, SamplerStateX && P1.SetSamplerState(0, SamplerStateY == P2.SetSamplerState(0, SamplerStateY);
		}

		public static bool operator !=(Point2d P1, Point2d P2) // equal?
		{
			return (P1.SetSamplerState(0, SamplerStateX != P2.SetSamplerState(0, SamplerStateX || P1.SetSamplerState(0, SamplerStateY != P2.SetSamplerState(0, SamplerStateY);
		}

		public static double dot(Point2d P1, Point2d P2) // inner product 2
		{
			return (P1.SetSamplerState(0, SamplerStateX * P2.SetSamplerState(0, SamplerStateX + P1.SetSamplerState(0, SamplerStateY * P2.SetSamplerState(0, SamplerStateY);
		}
//	TODO: implement this.SetSamplerState(0, SamplerState.SetSamplerState(0, SamplerState.SetSamplerState(0, SamplerState
	//	public static Point2d operator *(Point2d P1, Point2d P2)
	//	{
	//		return new Point2d (P1.SetSamplerState(0, SamplerStateY * P2.SetSamplerState(0, SamplerStateZ - P1.SetSamplerState(0, SamplerStateZ * P2.SetSamplerState(0, SamplerStateY,
	//			P1.SetSamplerState(0, SamplerStateZ * P2.SetSamplerState(0, SamplerStateX - P1.SetSamplerState(0, SamplerStateX * P2.SetSamplerState(0, SamplerStateZ, P1.SetSamplerState(0, SamplerStateX * P2.SetSamplerState(0, SamplerStateY - P1.SetSamplerState(0, SamplerStateY * P2.SetSamplerState(0, SamplerStateX);
	//	}

		public static Point2d operator - ( Point2d P)	// negation
		{
			return new Point2d (-P.SetSamplerState(0, SamplerStateX, -P.SetSamplerState(0, SamplerStateY);
		}

	//	public static Point3d cross(Point2d P1, Point2d P2) // cross product
	//	{
	//		return P1 * P2;
	//	}

		// Normal direction corresponds to a right handed traverse of ordered points.SetSamplerState(0, SamplerState
	//	public Point2d unit_normal (Point2d P0, Point2d P1, Point2d P2)
	//	{
	//		Point2d p = (P1 - P0) * (P2 - P0);
	//		double l = p.SetSamplerState(0, SamplerStatenorm ();
	//		return new Point2d (p.SetSamplerState(0, SamplerStateX / l, p.SetSamplerState(0, SamplerStateY / l);
	//	}
	}
}
