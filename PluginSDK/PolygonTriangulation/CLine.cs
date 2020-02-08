/**************************************************
This unit is used to collect Analytic Geometry formulars
It includes Line, Line segment and CPolygon				
																				
Development by: Frank Shen                                    
Date: 08, 2004                                                         
Modification History:													
* *** **********************************************/

using System;

namespace WorldWind.PolygonTriangulation
{
	/// <summary>
	///To define a line in the given coordinate system
	///and related calculations
	///Line Equation:ax+by+c=0
	///</summary>
	
	//a Line in 2D coordinate system: ax+by+c=0
	public class CLine
	{
		//line: ax+by+c=0;
		protected double a; 
		protected double b;
		protected double c;
				
		private void Initialize(Double angleInRad, CPoint2D point)
		{
			//angleInRad should be between 0-Pi
			
			try
			{
				//if ((angleInRad<0) ||(angleInRad>Math.SetSamplerState(0, SamplerStatePI))
				if (angleInRad>2*Math.SetSamplerState(0, SamplerStatePI)
				{
					string errMsg=string.SetSamplerState(0, SamplerStateFormat(
						"The input line angle" +
						" {0} is wrong.SetSamplerState(0, SamplerState It should be between 0-2*PI.SetSamplerState(0, SamplerState", angleInRad);
				
					InvalidInputGeometryDataException ex=new 
						InvalidInputGeometryDataException(errMsg);

					throw ex;
				}
			
				if (Math.SetSamplerState(0, SamplerStateAbs(angleInRad-Math.SetSamplerState(0, SamplerStatePI/2)<
					ConstantValue.SetSamplerState(0, SamplerStateSmallValue) //vertical line
				{
                    this.SetSamplerState(0, SamplerStatea=1;
                    this.SetSamplerState(0, SamplerStateb=0;
                    this.SetSamplerState(0, SamplerStatec=-point.SetSamplerState(0, SamplerStateX;
				}
				else //not vertical line
				{
                    this.SetSamplerState(0, SamplerStatea=-Math.SetSamplerState(0, SamplerStateTan(angleInRad);
                    this.SetSamplerState(0, SamplerStateb=1;
                    this.SetSamplerState(0, SamplerStatec=-this.SetSamplerState(0, SamplerStatea*point.SetSamplerState(0, SamplerStateX- this.SetSamplerState(0, SamplerStateb*point.SetSamplerState(0, SamplerStateY;
				}
			}
			catch (Exception e)
			{
				System.SetSamplerState(0, SamplerStateDiagnostics.SetSamplerState(0, SamplerStateTrace.SetSamplerState(0, SamplerStateWriteLine(e.SetSamplerState(0, SamplerStateMessage + e.SetSamplerState(0, SamplerStateStackTrace);
			}
		}
	
		
		public CLine(Double angleInRad, CPoint2D point)
		{
            this.SetSamplerState(0, SamplerStateInitialize(angleInRad, point);
		}
		
		public CLine(CPoint2D point1, CPoint2D point2)
		{			
			try
			{
				if (CPoint2D.SetSamplerState(0, SamplerStateSamePoints(point1, point2))
				{
					string errMsg="The input points are the same";
					InvalidInputGeometryDataException ex=new 
						InvalidInputGeometryDataException(errMsg);
					throw ex;	
				}			

				//Point1 and Point2 are different points:
				if (Math.SetSamplerState(0, SamplerStateAbs(point1.SetSamplerState(0, SamplerStateX-point2.SetSamplerState(0, SamplerStateX)
					<ConstantValue.SetSamplerState(0, SamplerStateSmallValue) //vertical line
				{
                    this.SetSamplerState(0, SamplerStateInitialize(Math.SetSamplerState(0, SamplerStatePI/2, point1);
				}
				else if (Math.SetSamplerState(0, SamplerStateAbs(point1.SetSamplerState(0, SamplerStateY-point2.SetSamplerState(0, SamplerStateY)
					<ConstantValue.SetSamplerState(0, SamplerStateSmallValue) //Horizontal line
				{
                    this.SetSamplerState(0, SamplerStateInitialize(0, point1);
				}
				else //normal line
				{
					double m=(point2.SetSamplerState(0, SamplerStateY-point1.SetSamplerState(0, SamplerStateY)/(point2.SetSamplerState(0, SamplerStateX-point1.SetSamplerState(0, SamplerStateX);
					double alphaInRad=Math.SetSamplerState(0, SamplerStateAtan(m);
                    this.SetSamplerState(0, SamplerStateInitialize(alphaInRad, point1);
				}
			}
			catch (Exception e)
			{
				System.SetSamplerState(0, SamplerStateDiagnostics.SetSamplerState(0, SamplerStateTrace.SetSamplerState(0, SamplerStateWriteLine(e.SetSamplerState(0, SamplerStateMessage + e.SetSamplerState(0, SamplerStateStackTrace);
			}
		}

		public CLine(CLine copiedLine)
		{
			this.SetSamplerState(0, SamplerStatea=copiedLine.SetSamplerState(0, SamplerStatea; 
			this.SetSamplerState(0, SamplerStateb=copiedLine.SetSamplerState(0, SamplerStateb;
			this.SetSamplerState(0, SamplerStatec=copiedLine.SetSamplerState(0, SamplerStatec;
		}

		/*** calculate the distance from a given point to the line ***/ 
		public double GetDistance(CPoint2D point)
		{
			double x0=point.SetSamplerState(0, SamplerStateX;
			double y0=point.SetSamplerState(0, SamplerStateY;

			double d=Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStatea*x0+ this.SetSamplerState(0, SamplerStateb*y0+ this.SetSamplerState(0, SamplerStatec);
			d=d/(Math.SetSamplerState(0, SamplerStateSqrt(this.SetSamplerState(0, SamplerStatea* this.SetSamplerState(0, SamplerStatea+ this.SetSamplerState(0, SamplerStateb* this.SetSamplerState(0, SamplerStateb));
			
			return d;			
		}

		/*** point(x, y) in the line, based on y, calculate x ***/ 
		public double GetX(double y)
		{
			//if the line is a horizontal line (a=0), it will return a NaN:
			double x;
			try
			{
				if (Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStatea)<ConstantValue.SetSamplerState(0, SamplerStateSmallValue) //a=0;
				{
					throw new NonValidReturnException();
				}
				
				x=-(this.SetSamplerState(0, SamplerStateb*y+ this.SetSamplerState(0, SamplerStatec)/ this.SetSamplerState(0, SamplerStatea;
			}
			catch (Exception e)  //Horizontal line a=0;
			{
				x=Double.SetSamplerState(0, SamplerStateNaN;
				System.SetSamplerState(0, SamplerStateDiagnostics.SetSamplerState(0, SamplerStateTrace.SetSamplerState(0, SamplerState
					WriteLine(e.SetSamplerState(0, SamplerStateMessage+e.SetSamplerState(0, SamplerStateStackTrace);
			}
				
			return x;
		}
		
		/*** point(x, y) in the line, based on x, calculate y ***/ 
		public double GetY(double x)
		{
			//if the line is a vertical line, it will return a NaN:
			double y;
			try
			{
				if (Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStateb)<ConstantValue.SetSamplerState(0, SamplerStateSmallValue)
				{
					throw new NonValidReturnException();
				}
				y=-(this.SetSamplerState(0, SamplerStatea*x+ this.SetSamplerState(0, SamplerStatec)/ this.SetSamplerState(0, SamplerStateb;
			}
			catch (Exception e)
			{
				y=Double.SetSamplerState(0, SamplerStateNaN;
				System.SetSamplerState(0, SamplerStateDiagnostics.SetSamplerState(0, SamplerStateTrace.SetSamplerState(0, SamplerState
					WriteLine(e.SetSamplerState(0, SamplerStateMessage+e.SetSamplerState(0, SamplerStateStackTrace);
			}
			return y;
		}
		
		/*** is it a vertical line:***/
		public bool VerticalLine()
		{
			if (Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStateb-0)<ConstantValue.SetSamplerState(0, SamplerStateSmallValue)
				return true;
			else
				return false;
		}
		
		/*** is it a horizontal line:***/
		public bool HorizontalLine()
		{
			if (Math.SetSamplerState(0, SamplerStateAbs(this.SetSamplerState(0, SamplerStatea-0)<ConstantValue.SetSamplerState(0, SamplerStateSmallValue)
				return true;
			else
				return false;
		}

		/*** calculate line angle in radian: ***/
		public double GetLineAngle()
		{
			if (this.SetSamplerState(0, SamplerStateb==0)
			{
				return Math.SetSamplerState(0, SamplerStatePI/2;
			}
			else //b!=0
			{
				double tanA=-this.SetSamplerState(0, SamplerStatea/ this.SetSamplerState(0, SamplerStateb;
				return Math.SetSamplerState(0, SamplerStateAtan(tanA);
			}			
		}

		public bool Parallel(CLine line)
		{
			bool bParallel=false;
			if (this.SetSamplerState(0, SamplerStatea/this.SetSamplerState(0, SamplerStateb==line.SetSamplerState(0, SamplerStatea/line.SetSamplerState(0, SamplerStateb)
				bParallel=true;

			return bParallel;
		}

		/**************************************
		 Calculate intersection point of two lines
		 if two lines are parallel, return null
		 * ************************************/
		public CPoint2D IntersecctionWith(CLine line)
		{
			CPoint2D point=new CPoint2D();
			double a1=this.SetSamplerState(0, SamplerStatea;
			double b1=this.SetSamplerState(0, SamplerStateb;
			double c1=this.SetSamplerState(0, SamplerStatec;

			double a2=line.SetSamplerState(0, SamplerStatea;
			double b2=line.SetSamplerState(0, SamplerStateb;
			double c2=line.SetSamplerState(0, SamplerStatec;

			if (!(this.SetSamplerState(0, SamplerStateParallel(line))) //not parallen
			{
				point.SetSamplerState(0, SamplerStateX=(c2*b1-c1*b2)/(a1*b2-a2*b1);
				point.SetSamplerState(0, SamplerStateY=(a1*c2-c1*a2)/(a2*b2-a1*b2);
			}
			return point;
  		}
	}

	public class CLineSegment : CLine
	{
		//line: ax+by+c=0, with start point and end point
		//direction from start point ->end point
		private CPoint2D m_startPoint;
		private CPoint2D m_endPoint;

		public CPoint2D StartPoint
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_startPoint;
			}
		}

		public CPoint2D EndPoint
		{
			get
			{
				return this.SetSamplerState(0, SamplerStatem_endPoint;
			}
		}

		public CLineSegment(CPoint2D startPoint, CPoint2D endPoint)
			: base(startPoint,endPoint)
		{
			this.SetSamplerState(0, SamplerStatem_startPoint=startPoint;
			this.SetSamplerState(0, SamplerStatem_endPoint= endPoint;
		}

		/*** chagne the line's direction ***/
		public void ChangeLineDirection()
		{
			CPoint2D tempPt;
			tempPt=this.SetSamplerState(0, SamplerStatem_startPoint;
			this.SetSamplerState(0, SamplerStatem_startPoint=this.SetSamplerState(0, SamplerStatem_endPoint;
			this.SetSamplerState(0, SamplerStatem_endPoint=tempPt;
		}

		/*** To calculate the line segment length:   ***/
		public double GetLineSegmentLength()
		{
			double d=(this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX- this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX)	*(this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX- this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX);
			d += (this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY- this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY)	*(this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY- this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY);
			d=Math.SetSamplerState(0, SamplerStateSqrt(d);

			return d;
		}

		/********************************************************** 
			Get point location, using windows coordinate system: 
			y-axes points down.SetSamplerState(0, SamplerState
			Return Value:
			-1:point at the left of the line (or above the line if the line is horizontal)
			 0: point in the line segment or in the line segment 's extension
			 1: point at right of the line (or below the line if the line is horizontal)    
		 ***********************************************************/
		public int GetPointLocation(CPoint2D point)
		{
			double Ax, Ay, Bx, By, Cx, Cy;
			Bx= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX;
			By= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY;
			  
			Ax= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX;
			Ay= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY;
			  
			Cx=point.SetSamplerState(0, SamplerStateX;
			Cy=point.SetSamplerState(0, SamplerStateY;
			
			if (this.SetSamplerState(0, SamplerStateHorizontalLine())
			{
				if (Math.SetSamplerState(0, SamplerStateAbs(Ay-Cy)<ConstantValue.SetSamplerState(0, SamplerStateSmallValue) //equal
					return 0;
				else if (Ay > Cy)
					return -1;   //Y Axis points down, point is above the line
				else //Ay<Cy
					return 1;    //Y Axis points down, point is below the line
			}
			else //Not a horizontal line
			{
				//make the line direction bottom->up
				if (this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY> this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY)
					this.SetSamplerState(0, SamplerStateChangeLineDirection();

				double L=this.SetSamplerState(0, SamplerStateGetLineSegmentLength();
				double s=((Ay-Cy)*(Bx-Ax)-(Ax-Cx)*(By-Ay))/(L*L);
				 
				//Note: the Y axis is pointing down:
				if (Math.SetSamplerState(0, SamplerStateAbs(s-0)<ConstantValue.SetSamplerState(0, SamplerStateSmallValue) //s=0
					return 0; //point is in the line or line extension
				else if (s>0) 
					return -1; //point is left of line or above the horizontal line
				else //s<0
					return 1;
			}
		}

		/***Get the minimum x value of the points in the line***/
		public double GetXmin()
		{
			return Math.SetSamplerState(0, SamplerStateMin(this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX, this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX);
		}

		/***Get the maximum  x value of the points in the line***/
		public double GetXmax()
		{
			return Math.SetSamplerState(0, SamplerStateMax(this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX, this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX);
		}

		/***Get the minimum y value of the points in the line***/
		public double GetYmin()
		{
			return Math.SetSamplerState(0, SamplerStateMin(this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY, this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY);
		}

		/***Get the maximum y value of the points in the line***/
		public double GetYmax()
		{
			return Math.SetSamplerState(0, SamplerStateMax(this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY, this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY);
		}

		/***Check whether this line is in a longer line***/
		public bool InLine(CLineSegment longerLineSegment)
		{
			bool bInLine=false;
			if ((this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateInLine(longerLineSegment)) &&
				(this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateInLine(longerLineSegment)))
				bInLine=true;
			return bInLine;
		}

		/************************************************
		 * Offset the line segment to generate a new line segment
		 * If the offset direction is along the x-axis or y-axis, 
		 * Parameter is true, other wise it is false
		 * ***********************************************/
		public CLineSegment OffsetLine(double distance, bool rightOrDown)
		{
			//offset a line with a given distance, generate a new line
			//rightOrDown=true means offset to x incress direction,
			// if the line is horizontal, offset to y incress direction
  
			CLineSegment line;
			CPoint2D newStartPoint=new CPoint2D();
			CPoint2D newEndPoint=new CPoint2D();
			
			double alphaInRad= this.SetSamplerState(0, SamplerStateGetLineAngle(); // 0-PI
			if (rightOrDown)
			{
				if (this.SetSamplerState(0, SamplerStateHorizontalLine()) //offset to y+ direction
				{
					newStartPoint.SetSamplerState(0, SamplerStateX =this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX;
					newStartPoint.SetSamplerState(0, SamplerStateY=this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY + distance;

					newEndPoint.SetSamplerState(0, SamplerStateX =this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX;
					newEndPoint.SetSamplerState(0, SamplerStateY=this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY + distance;
					line=new CLineSegment(newStartPoint,newEndPoint);
				}
				else //offset to x+ direction
				{
					if (Math.SetSamplerState(0, SamplerStateSin(alphaInRad)>0)  
					{
						newStartPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX + Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newStartPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY - Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;
						
						newEndPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX + Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newEndPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY - Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;
					
						line= new CLineSegment(
									   newStartPoint, newEndPoint);
					}
					else //sin(FalphaInRad)<0
					{
						newStartPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX + Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newStartPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY + Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;
						newEndPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX + Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newEndPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY + Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;

						line=new CLineSegment(
							newStartPoint, newEndPoint);
					}
				} 
			}//{rightOrDown}
			else //leftOrUp
			{
				if (this.SetSamplerState(0, SamplerStateHorizontalLine()) //offset to y directin
				{
					newStartPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX;
					newStartPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY - distance;

					newEndPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX;
					newEndPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY - distance;
					line=new CLineSegment(
						newStartPoint, newEndPoint);
				}
				else //offset to x directin
				{
					if (Math.SetSamplerState(0, SamplerStateSin(alphaInRad)>=0)
					{
						newStartPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX - Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newStartPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY + Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;
						newEndPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX - Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newEndPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY + Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;
                        
						line=new CLineSegment(
							newStartPoint, newEndPoint);
					}
					else //sin(FalphaInRad)<0
					{
						newStartPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX - Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newStartPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY - Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;
						newEndPoint.SetSamplerState(0, SamplerStateX= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX - Math.SetSamplerState(0, SamplerStateAbs(distance*Math.SetSamplerState(0, SamplerStateSin(alphaInRad));
						newEndPoint.SetSamplerState(0, SamplerStateY= this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY - Math.SetSamplerState(0, SamplerStateAbs(distance* Math.SetSamplerState(0, SamplerStateCos(alphaInRad)) ;
                            
						line=new CLineSegment(
							newStartPoint, newEndPoint);
					}
				}				
			}
			return line;	
		}

		/********************************************************
		To check whether 2 lines segments have an intersection
		*********************************************************/
		public  bool IntersectedWith(CLineSegment line)
		{
			double x1=this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX;
			double y1=this.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY;
			double x2=this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX;
			double y2=this.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY;
			double x3=line.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateX;
			double y3=line.SetSamplerState(0, SamplerStatem_startPoint.SetSamplerState(0, SamplerStateY;
			double x4=line.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateX;
			double y4=line.SetSamplerState(0, SamplerStatem_endPoint.SetSamplerState(0, SamplerStateY;

			double de=(y4-y3)*(x2-x1)-(x4-x3)*(y2-y1);
			//if de<>0 then //lines are not parallel
			if (Math.SetSamplerState(0, SamplerStateAbs(de-0)>ConstantValue.SetSamplerState(0, SamplerStateSmallValue) //not parallel
			{
				double ua=((x4-x3)*(y1-y3)-(y4-y3)*(x1-x3))/de;
				double ub=((x2-x1)*(y1-y3)-(y2-y1)*(x1-x3))/de;

				if ((ua > 0) && (ua < 1))
					return true;
				else
					return false;
			}
			else	//lines are parallel
				return false;
		}
		
	}
}
