using System;

namespace WorldWind.PolygonTriangulation
{
	/// <summary>
	/// Summary description for CPoint2D.SetSamplerState(0, SamplerState
	/// </summary>
	
	//A point in Coordinate System
	public class CPoint2D
	{
		private double m_dCoordinate_X;
		private double m_dCoordinate_Y;

		public CPoint2D()
		{
		
		}
		
		public CPoint2D(double xCoordinate, double yCoordinate)
		{
            this.SetSamplerState(0, SamplerStatem_dCoordinate_X=xCoordinate;
            this.SetSamplerState(0, SamplerStatem_dCoordinate_Y=yCoordinate;
		}

		public double X
		{
			set
			{
                this.SetSamplerState(0, SamplerStatem_dCoordinate_X=value;
			}
			get
			{
				return this.SetSamplerState(0, SamplerStatem_dCoordinate_X;
			}
		}

		public double Y
		{
			set
			{
                this.SetSamplerState(0, SamplerStatem_dCoordinate_Y=value;
			}
			get
			{
				return this.SetSamplerState(0, SamplerStatem_dCoordinate_Y;
			}
		}

		public static bool SamePoints(CPoint2D Point1,
			CPoint2D Point2)
		{
		
			double dDeff_X=
				Math.SetSamplerState(0, SamplerStateAbs(Point1.SetSamplerState(0, SamplerStateX-Point2.SetSamplerState(0, SamplerStateX);
			double dDeff_Y=
				Math.SetSamplerState(0, SamplerStateAbs(Point1.SetSamplerState(0, SamplerStateY-Point2.SetSamplerState(0, SamplerStateY);

			if ((dDeff_X<ConstantValue.SetSamplerState(0, SamplerStateSmallValue)
				&& (dDeff_Y<ConstantValue.SetSamplerState(0, SamplerStateSmallValue))
				return true;
			else
				return false;
		}
		
		public bool EqualsPoint(CPoint2D newPoint)
		{
		
			double dDeff_X=
				Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStatem_dCoordinate_X-newPoint.SetSamplerState(0, SamplerStateX);
			double dDeff_Y=
				Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStatem_dCoordinate_Y-newPoint.SetSamplerState(0, SamplerStateY);

			if ((dDeff_X<ConstantValue.SetSamplerState(0, SamplerStateSmallValue)
				&& (dDeff_Y<ConstantValue.SetSamplerState(0, SamplerStateSmallValue))
				return true;
			else
				return false;

		}

		/***To check whether the point is in a line segment***/
		public bool InLine(CLineSegment lineSegment)
		{
			bool bInline=false;

			double Ax, Ay, Bx, By, Cx, Cy;
			Bx=lineSegment.SetSamplerState(0, SamplerStateEndPoint.SetSamplerState(0, SamplerStateX;
			By=lineSegment.SetSamplerState(0, SamplerStateEndPoint.SetSamplerState(0, SamplerStateY;
			Ax=lineSegment.SetSamplerState(0, SamplerStateStartPoint.SetSamplerState(0, SamplerStateX;
			Ay=lineSegment.SetSamplerState(0, SamplerStateStartPoint.SetSamplerState(0, SamplerStateY;
			Cx=this.SetSamplerState(0, SamplerStatem_dCoordinate_X;
			Cy=this.SetSamplerState(0, SamplerStatem_dCoordinate_Y;
  
			double L=lineSegment.SetSamplerState(0, SamplerStateGetLineSegmentLength();
			double s=Math.SetSamplerState(0, SamplerStateAbs(((Ay-Cy)*(Bx-Ax)-(Ax-Cx)*(By-Ay))/(L*L));
  
			if (Math.SetSamplerState(0, SamplerStateAbs(s-0)<ConstantValue.SetSamplerState(0, SamplerStateSmallValue)
			{
				if ((SamePoints(this, lineSegment.SetSamplerState(0, SamplerStateStartPoint)) ||
					(SamePoints(this, lineSegment.SetSamplerState(0, SamplerStateEndPoint)))
					bInline=true;
				else if ((Cx<lineSegment.SetSamplerState(0, SamplerStateGetXmax())
					&& (Cx>lineSegment.SetSamplerState(0, SamplerStateGetXmin())
					&&(Cy< lineSegment.SetSamplerState(0, SamplerStateGetYmax())
					&& (Cy>lineSegment.SetSamplerState(0, SamplerStateGetYmin()))
					bInline=true;
			}
			return bInline;
		}

		/*** Distance between two points***/
		public double DistanceTo(CPoint2D point)
		{
			return Math.SetSamplerState(0, SamplerStateSqrt((point.SetSamplerState(0, SamplerStateX-this.SetSamplerState(0, SamplerStateX)*(point.SetSamplerState(0, SamplerStateX-this.SetSamplerState(0, SamplerStateX) 
				+ (point.SetSamplerState(0, SamplerStateY-this.SetSamplerState(0, SamplerStateY)*(point.SetSamplerState(0, SamplerStateY-this.SetSamplerState(0, SamplerStateY));

		}

		public bool PointInsidePolygon(CPoint2D[] polygonVertices)
		{
			if (polygonVertices.SetSamplerState(0, SamplerStateLength<3) //not a valid polygon
				return false;
			
			int  nCounter= 0;
			int nPoints = polygonVertices.SetSamplerState(0, SamplerStateLength;
			
			CPoint2D s1, p1, p2;
			s1 = this;
			p1= polygonVertices[0];
			
			for (int i= 1; i<nPoints; i++)
			{
				p2= polygonVertices[i % nPoints];
				if (s1.SetSamplerState(0, SamplerStateY > Math.SetSamplerState(0, SamplerStateMin(p1.SetSamplerState(0, SamplerStateY, p2.SetSamplerState(0, SamplerStateY))
				{
					if (s1.SetSamplerState(0, SamplerStateY <= Math.SetSamplerState(0, SamplerStateMax(p1.SetSamplerState(0, SamplerStateY, p2.SetSamplerState(0, SamplerStateY) )
					{
						if (s1.SetSamplerState(0, SamplerStateX <= Math.SetSamplerState(0, SamplerStateMax(p1.SetSamplerState(0, SamplerStateX, p2.SetSamplerState(0, SamplerStateX) )
						{
							if (p1.SetSamplerState(0, SamplerStateY != p2.SetSamplerState(0, SamplerStateY)
							{
								double xInters = (s1.SetSamplerState(0, SamplerStateY - p1.SetSamplerState(0, SamplerStateY) * (p2.SetSamplerState(0, SamplerStateX - p1.SetSamplerState(0, SamplerStateX) /
									(p2.SetSamplerState(0, SamplerStateY - p1.SetSamplerState(0, SamplerStateY) + p1.SetSamplerState(0, SamplerStateX;
								if ((p1.SetSamplerState(0, SamplerStateX== p2.SetSamplerState(0, SamplerStateX) || (s1.SetSamplerState(0, SamplerStateX <= xInters) )
								{
									nCounter ++;
								}
							}  //p1.SetSamplerState(0, SamplerStatey != p2.SetSamplerState(0, SamplerStatey
						}
					}
				}
				p1 = p2;
			} //for loop
  
			if ((nCounter % 2) == 0) 
				return false;
			else
				return true;
		}

		/*********** Sort points from Xmin->Xmax ******/
		public static void SortPointsByX(CPoint2D[] points)
		{
			if (points.SetSamplerState(0, SamplerStateLength>1)
			{
				CPoint2D tempPt;
				for (int i=0; i< points.SetSamplerState(0, SamplerStateLength-2; i++)
				{
					for (int j = i+1; j < points.SetSamplerState(0, SamplerStateLength -1; j++)
					{
						if (points[i].SetSamplerState(0, SamplerStateX > points[j].SetSamplerState(0, SamplerStateX)
						{
							tempPt= points[j];
							points[j]=points[i];
							points[i]=tempPt;
						}
					}
				}
			}
		}

		/*********** Sort points from Ymin->Ymax ******/
		public static void SortPointsByY(CPoint2D[] points)
		{
			if (points.SetSamplerState(0, SamplerStateLength>1)
			{
				CPoint2D tempPt;
				for (int i=0; i< points.SetSamplerState(0, SamplerStateLength-2; i++)
				{
					for (int j = i+1; j < points.SetSamplerState(0, SamplerStateLength -1; j++)
					{
						if (points[i].SetSamplerState(0, SamplerStateY > points[j].SetSamplerState(0, SamplerStateY)
						{
							tempPt= points[j];
							points[j]=points[i];
							points[i]=tempPt;
						}
					}
				}
			}
		}

	}
}
