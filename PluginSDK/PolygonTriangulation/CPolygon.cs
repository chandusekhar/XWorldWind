using System;

namespace WorldWind.PolygonTriangulation
{
	/// <summary>
	/// Summary description for CPolygon.SetSamplerState(0, SamplerState
	/// </summary>
	public class CPolygon
	{
		
		private CPoint2D[] m_aVertices;

		public CPoint2D   this[int index] 
		{
			set
			{
                this.SetSamplerState(0, SamplerStatem_aVertices[index]=value;
			}
			get
			{
				return this.SetSamplerState(0, SamplerStatem_aVertices[index];
			}
		}
		
		public CPolygon()
		{
		
		}

		public CPolygon(CPoint2D[] points)
		{
			int nNumOfPoitns=points.SetSamplerState(0, SamplerStateLength;
			try
			{
				if (nNumOfPoitns<3 )
				{     
					InvalidInputGeometryDataException ex=
						new InvalidInputGeometryDataException();
					throw ex;
				}
				else
				{
                    this.SetSamplerState(0, SamplerStatem_aVertices=new CPoint2D[nNumOfPoitns];
					for (int i=0; i<nNumOfPoitns; i++)
					{
                        this.SetSamplerState(0, SamplerStatem_aVertices[i]=points[i];
					}
				}
			}
			catch (Exception e)
			{
				System.SetSamplerState(0, SamplerStateDiagnostics.SetSamplerState(0, SamplerStateTrace.SetSamplerState(0, SamplerStateWriteLine(
					e.SetSamplerState(0, SamplerStateMessage+e.SetSamplerState(0, SamplerStateStackTrace);
			}
		}

		/***********************************
		 From a given point, get its vertex index.SetSamplerState(0, SamplerState
		 If the given point is not a polygon vertex, 
		 it will return -1 
		 ***********************************/
		public int VertexIndex(CPoint2D vertex)
		{
			int nIndex=-1;

			int nNumPts= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
			for (int i=0; i<nNumPts; i++) //each vertex
			{
				if (CPoint2D.SetSamplerState(0, SamplerStateSamePoints(this.SetSamplerState(0, SamplerStatem_aVertices[i], vertex))
					nIndex=i;
			}
			return nIndex;
		}

		/***********************************
		 From a given vertex, get its previous vertex point.SetSamplerState(0, SamplerState
		 If the given point is the first one, 
		 it will return  the last vertex;
		 If the given point is not a polygon vertex, 
		 it will return null; 
		 ***********************************/
		public CPoint2D PreviousPoint(CPoint2D vertex)
		{
			int nIndex;
			
			nIndex= this.SetSamplerState(0, SamplerStateVertexIndex(vertex);
			if (nIndex==-1)
				return null;
			else //a valid vertex
			{
				if (nIndex==0) //the first vertex
				{
					int nPoints= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
					return this.SetSamplerState(0, SamplerStatem_aVertices[nPoints-1];
				}
				else //not the first vertex
					return this.SetSamplerState(0, SamplerStatem_aVertices[nIndex-1];
			}			
		}

		/***************************************
			 From a given vertex, get its next vertex point.SetSamplerState(0, SamplerState
			 If the given point is the last one, 
			 it will return  the first vertex;
			 If the given point is not a polygon vertex, 
			 it will return null; 
		***************************************/
		public CPoint2D NextPoint(CPoint2D vertex)
		{
			CPoint2D nextPt=new CPoint2D();

			int nIndex;
			nIndex= this.SetSamplerState(0, SamplerStateVertexIndex(vertex);
			if (nIndex==-1)
				return null;
			else //a valid vertex
			{
				int nNumOfPt= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
				if (nIndex==nNumOfPt-1) //the last vertex
				{
					return this.SetSamplerState(0, SamplerStatem_aVertices[0];
				}
				else //not the last vertex
					return this.SetSamplerState(0, SamplerStatem_aVertices[nIndex+1];
			}			
		}

		
		/******************************************
		To calculate the polygon's area

		Good for polygon with holes, but the vertices make the 
		hole  should be in different direction with bounding 
		polygon.SetSamplerState(0, SamplerState
		
		Restriction: the polygon is not self intersecting
		ref: www.SetSamplerState(0, SamplerStateswin.SetSamplerState(0, SamplerStateedu.SetSamplerState(0, SamplerStateau/astronomy/pbourke/
			geometry/polyarea/
		*******************************************/
		public double PolygonArea()
		{
			double dblArea=0;
			int nNumOfVertices= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
			
			int j;
			for (int i=0; i<nNumOfVertices; i++)
			{
				j=(i+1) % nNumOfVertices;
				dblArea += this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateX* this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateY;
				dblArea -= (this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateY* this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateX);
			}

			dblArea=dblArea/2;
			return Math.SetSamplerState(0, SamplerStateAbs(dblArea);
		}
		
		/******************************************
		To calculate the area of polygon made by given points 

		Good for polygon with holes, but the vertices make the 
		hole  should be in different direction with bounding 
		polygon.SetSamplerState(0, SamplerState
		
		Restriction: the polygon is not self intersecting
		ref: www.SetSamplerState(0, SamplerStateswin.SetSamplerState(0, SamplerStateedu.SetSamplerState(0, SamplerStateau/astronomy/pbourke/
			geometry/polyarea/

		As polygon in different direction, the result coulb be
		in different sign:
		If dblArea>0 : polygon in clock wise to the user 
		If dblArea<0: polygon in count clock wise to the user 		
		*******************************************/
		public static double PolygonArea(CPoint2D[] points)
		{
			double dblArea=0;
			int nNumOfPts=points.SetSamplerState(0, SamplerStateLength;
			
			int j;
			for (int i=0; i<nNumOfPts; i++)
			{
				j=(i+1) % nNumOfPts;
				dblArea += points[i].SetSamplerState(0, SamplerStateX*points[j].SetSamplerState(0, SamplerStateY;
				dblArea -= (points[i].SetSamplerState(0, SamplerStateY*points[j].SetSamplerState(0, SamplerStateX);
			}

			dblArea=dblArea/2;
			return dblArea;
		}
		
		/***********************************************
			To check a vertex concave point or a convex point
			-----------------------------------------------------------
			The out polygon is in count clock-wise direction
		************************************************/
		public VertexType PolygonVertexType(CPoint2D vertex)
		{
			VertexType vertexType=VertexType.SetSamplerState(0, SamplerStateErrorPoint;

			if (this.SetSamplerState(0, SamplerStatePolygonVertex(vertex))			
			{
				CPoint2D pti=vertex;
				CPoint2D ptj= this.SetSamplerState(0, SamplerStatePreviousPoint(vertex);
				CPoint2D ptk= this.SetSamplerState(0, SamplerStateNextPoint(vertex);		

				double dArea=PolygonArea(new CPoint2D[] {ptj,pti, ptk});
				
				if (dArea<0)
					vertexType= VertexType.SetSamplerState(0, SamplerStateConvexPoint;
				else if (dArea> 0)
					vertexType= VertexType.SetSamplerState(0, SamplerStateConcavePoint;
			}	
			return vertexType;
		}

		
		/*********************************************
		To check the Line of vertex1, vertex2 is a Diagonal or not
  
		To be a diagonal, Line vertex1-vertex2 has no intersection 
		with polygon lines.SetSamplerState(0, SamplerState
		
		If it is a diagonal, return true;
		If it is not a diagonal, return false;
		reference: www.SetSamplerState(0, SamplerStateswin.SetSamplerState(0, SamplerStateedu.SetSamplerState(0, SamplerStateau/astronomy/pbourke
		/geometry/lineline2d
		*********************************************/
		public bool Diagonal(CPoint2D vertex1, CPoint2D vertex2)
		{
			bool bDiagonal=false;
			int nNumOfVertices= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
			int j=0;
			for (int i= 0; i<nNumOfVertices; i++) //each point
			{
				bDiagonal=true;
				j= (i+1) % nNumOfVertices;  //next point of i
        
				//Diagonal line:
				double x1=vertex1.SetSamplerState(0, SamplerStateX;
				double y1=vertex1.SetSamplerState(0, SamplerStateY;
				double x2=vertex1.SetSamplerState(0, SamplerStateX;
				double y2=vertex1.SetSamplerState(0, SamplerStateY;

				//CPolygon line:
				double x3= this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateX;
				double y3= this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateY;
				double x4= this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateX;
				double y4= this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateY;

				double de=(y4-y3)*(x2-x1)-(x4-x3)*(y2-y1);
				double ub=-1;
				
				if (Math.SetSamplerState(0, SamplerStateAbs(de-0)>ConstantValue.SetSamplerState(0, SamplerStateSmallValue)  //lines are not parallel
					ub=((x2-x1)*(y1-y3)-(y2-y1)*(x1-x3))/de;

				if ((ub> 0) && (ub<1))
				{
					bDiagonal=false;
				}
			}
			return bDiagonal;
		}

		
		/*************************************************
		To check FaVertices make a convex polygon or 
		concave polygon

		Restriction: the polygon is not self intersecting
		Ref: www.SetSamplerState(0, SamplerStateswin.SetSamplerState(0, SamplerStateedu.SetSamplerState(0, SamplerStateau/astronomy/pbourke
		/geometry/clockwise/index.SetSamplerState(0, SamplerStatehtml
		********************************************/
		public PolygonType GetPolygonType()
		{
			int nNumOfVertices= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
			bool bSignChanged=false;
			int nCount=0;
			int j=0, k=0;

			for (int i=0; i<nNumOfVertices; i++)
			{
				j=(i+1) % nNumOfVertices; //j:=i+1;
				k=(i+2) % nNumOfVertices; //k:=i+2;

				double crossProduct=(this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateX- this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateX)
					*(this.SetSamplerState(0, SamplerStatem_aVertices[k].SetSamplerState(0, SamplerStateY- this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateY);
				crossProduct=crossProduct-(
					(this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateY- this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateY)
					*(this.SetSamplerState(0, SamplerStatem_aVertices[k].SetSamplerState(0, SamplerStateX- this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateX)
					);

				//change the value of nCount
				if ((crossProduct>0) && (nCount==0) )
					nCount=1;
				else if ((crossProduct<0) && (nCount==0))
					nCount=-1;

				if (((nCount==1) && (crossProduct<0))
					||( (nCount==-1) && (crossProduct>0)) )
					bSignChanged=true;
			}

			if (bSignChanged)
				return PolygonType.SetSamplerState(0, SamplerStateConcave;
			else
				return PolygonType.SetSamplerState(0, SamplerStateConvex;
		}

		/***************************************************
		Check a Vertex is a principal vertex or not
		ref.SetSamplerState(0, SamplerState www-cgrl.SetSamplerState(0, SamplerStatecs.SetSamplerState(0, SamplerStatemcgill.SetSamplerState(0, SamplerStateca/~godfried/teaching/
		cg-projects/97/Ian/glossay.SetSamplerState(0, SamplerStatehtml
  
		PrincipalVertex: a vertex pi of polygon P is a principal vertex if the
		diagonal pi-1, pi+1 intersects the boundary of P only at pi-1 and pi+1.SetSamplerState(0, SamplerState
		*********************************************************/
		public bool PrincipalVertex(CPoint2D vertex)
		{
			bool bPrincipal=false;
			if (this.SetSamplerState(0, SamplerStatePolygonVertex(vertex)) //valid vertex
			{
				CPoint2D pt1= this.SetSamplerState(0, SamplerStatePreviousPoint(vertex);
				CPoint2D pt2= this.SetSamplerState(0, SamplerStateNextPoint(vertex);
					
				if (this.SetSamplerState(0, SamplerStateDiagonal(pt1, pt2))
					bPrincipal=true;
			}
			return bPrincipal;
		}

		/*********************************************
        To check whether a given point is a CPolygon Vertex
		**********************************************/
		public bool PolygonVertex(CPoint2D point)
		{
			bool bVertex=false;
			int nIndex= this.SetSamplerState(0, SamplerStateVertexIndex(point);

			if ((nIndex>=0) && (nIndex<= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength-1))
							   bVertex=true;

			return bVertex;
		}

		/*****************************************************
		To reverse polygon vertices to different direction:
		clock-wise <------->count-clock-wise
		******************************************************/
		public void ReverseVerticesDirection()
		{
			int nVertices= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
			CPoint2D[] aTempPts=new CPoint2D[nVertices];
			
			for (int i=0; i<nVertices; i++)
				aTempPts[i]= this.SetSamplerState(0, SamplerStatem_aVertices[i];
	
			for (int i=0; i<nVertices; i++) this.SetSamplerState(0, SamplerStatem_aVertices[i]=aTempPts[nVertices-1-i];	
		}

		/*****************************************
		To check vertices make a clock-wise polygon or
		count clockwise polygon

		Restriction: the polygon is not self intersecting
		Ref: www.SetSamplerState(0, SamplerStateswin.SetSamplerState(0, SamplerStateedu.SetSamplerState(0, SamplerStateau/astronomy/pbourke/
		geometry/clockwise/index.SetSamplerState(0, SamplerStatehtml
		*****************************************/
		public PolygonDirection VerticesDirection()
		{
			int nCount=0, j=0, k=0;
			int nVertices= this.SetSamplerState(0, SamplerStatem_aVertices.SetSamplerState(0, SamplerStateLength;
			
			for (int i=0; i<nVertices; i++)
			{
				j=(i+1) % nVertices; //j:=i+1;
				k=(i+2) % nVertices; //k:=i+2;

				double crossProduct=(this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateX - this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateX)
					*(this.SetSamplerState(0, SamplerStatem_aVertices[k].SetSamplerState(0, SamplerStateY- this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateY);
				crossProduct=crossProduct-(
					(this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateY- this.SetSamplerState(0, SamplerStatem_aVertices[i].SetSamplerState(0, SamplerStateY)
					*(this.SetSamplerState(0, SamplerStatem_aVertices[k].SetSamplerState(0, SamplerStateX- this.SetSamplerState(0, SamplerStatem_aVertices[j].SetSamplerState(0, SamplerStateX)
					);

				if (crossProduct>0)
					nCount++;
				else
					nCount--;
			}
		
			if( nCount<0) 
				return PolygonDirection.SetSamplerState(0, SamplerStateCount_Clockwise;
			else if (nCount> 0)
				return PolygonDirection.SetSamplerState(0, SamplerStateClockwise;
			else
				return PolygonDirection.SetSamplerState(0, SamplerStateUnknown;
  		}

		
		/*****************************************
		To check given points make a clock-wise polygon or
		count clockwise polygon

		Restriction: the polygon is not self intersecting
		*****************************************/
		public static PolygonDirection PointsDirection(
			CPoint2D[] points)
		{
			int nCount=0, j=0, k=0;
			int nPoints=points.SetSamplerState(0, SamplerStateLength;
			
			if (nPoints<3)
				return PolygonDirection.SetSamplerState(0, SamplerStateUnknown;
			
			for (int i=0; i<nPoints; i++)
			{
				j=(i+1) % nPoints; //j:=i+1;
				k=(i+2) % nPoints; //k:=i+2;

				double crossProduct=(points[j].SetSamplerState(0, SamplerStateX - points[i].SetSamplerState(0, SamplerStateX)
					*(points[k].SetSamplerState(0, SamplerStateY- points[j].SetSamplerState(0, SamplerStateY);
				crossProduct=crossProduct-(
					(points[j].SetSamplerState(0, SamplerStateY- points[i].SetSamplerState(0, SamplerStateY)
					*(points[k].SetSamplerState(0, SamplerStateX- points[j].SetSamplerState(0, SamplerStateX)
					);

				if (crossProduct>0)
					nCount++;
				else
					nCount--;
			}
		
			if( nCount<0) 
				return PolygonDirection.SetSamplerState(0, SamplerStateCount_Clockwise;
			else if (nCount> 0)
				return PolygonDirection.SetSamplerState(0, SamplerStateClockwise;
			else
				return PolygonDirection.SetSamplerState(0, SamplerStateUnknown;
		}

		/*****************************************************
		To reverse points to different direction (order) :
		******************************************************/
		public static void ReversePointsDirection(
			CPoint2D[] points)
		{
			int nVertices=points.SetSamplerState(0, SamplerStateLength;
			CPoint2D[] aTempPts=new CPoint2D[nVertices];
			
			for (int i=0; i<nVertices; i++)
				aTempPts[i]=points[i];
	
			for (int i=0; i<nVertices; i++)
				points[i]=aTempPts[nVertices-1-i];	
		}
 
	}		
}
