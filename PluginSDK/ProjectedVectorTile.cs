using System;
using System.Collections;
using System.Drawing;
using System.IO;
using Utility;

namespace WorldWind
{
	public class ProjectedVectorTile
	{
		public static string CachePath = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "Cache");
		bool m_Initialized;
		bool m_Initializing;
		bool m_Disposing;

		public int Level;
		public int Row;
		public int Col;

		ProjectedVectorRenderer m_parentProjectedLayer;
		
		ImageLayer m_NwImageLayer;
		ImageLayer m_NeImageLayer;
		ImageLayer m_SwImageLayer;
		ImageLayer m_SeImageLayer;

		public GeographicBoundingBox m_geographicBoundingBox;
		public BoundingBox BoundingBox;
		
		public ProjectedVectorTile(
			GeographicBoundingBox geographicBoundingBox,
			ProjectedVectorRenderer parentLayer
			)
		{
            this.m_geographicBoundingBox = geographicBoundingBox;
            this.m_parentProjectedLayer = parentLayer;

            this.BoundingBox = new BoundingBox( (float)geographicBoundingBox.South, (float)geographicBoundingBox.North, (float)geographicBoundingBox.West, (float)geographicBoundingBox.East, 
				(float)(parentLayer.World.EquatorialRadius + geographicBoundingBox.MinimumAltitude), (float)(parentLayer.World.EquatorialRadius + geographicBoundingBox.MaximumAltitude + 300000f));
		}

		public void Dispose()
		{
            this.m_Initialized = false;
            this.m_Disposing = true;
			if(this.m_NorthWestChild != null)
			{
                this.m_NorthWestChild.Dispose();
			}

			if(this.m_NorthEastChild != null)
			{
                this.m_NorthEastChild.Dispose();
			}

			if(this.m_SouthWestChild != null)
			{
                this.m_SouthWestChild.Dispose();
			}

			if(this.m_SouthEastChild != null)
			{
                this.m_SouthEastChild.Dispose();
			}
			
			if(this.m_NwImageLayer != null)
			{
                this.m_NwImageLayer.Dispose();
                this.m_NwImageLayer = null;
			}

			if(this.m_NeImageLayer != null)
			{
                this.m_NeImageLayer.Dispose();
                this.m_NeImageLayer = null;
			}

			if(this.m_SwImageLayer != null)
			{
                this.m_SwImageLayer.Dispose();
                this.m_SwImageLayer = null;
			}

			if(this.m_SeImageLayer != null)
			{
                this.m_SeImageLayer.Dispose();
                this.m_SeImageLayer = null;
			}

			if(this.m_ImageStream != null)
			{
                this.m_ImageStream.Close();
			}

			if(!this.m_Initializing)
			{
                this.m_Disposing = false;
			}
		}

		public static System.Drawing.Drawing2D.HatchStyle getGDIHatchStyle(ShapeFillStyle shapeFillStyle)
		{
			if(shapeFillStyle == ShapeFillStyle.BackwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.Cross)
			{
				return System.Drawing.Drawing2D.HatchStyle.Cross;
			}
			else if(shapeFillStyle == ShapeFillStyle.DarkDownwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.DarkDownwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.DarkHorizontal)
			{
				return System.Drawing.Drawing2D.HatchStyle.DarkHorizontal;
			}
			else if(shapeFillStyle == ShapeFillStyle.DarkUpwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.DarkUpwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.DarkVertical)
			{
				return System.Drawing.Drawing2D.HatchStyle.DarkVertical;
			}
			else if(shapeFillStyle == ShapeFillStyle.DashedDownwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.DashedDownwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.DashedHorizontal)
			{
				return System.Drawing.Drawing2D.HatchStyle.DashedHorizontal;
			}
			else if(shapeFillStyle == ShapeFillStyle.DashedUpwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.DashedUpwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.DashedVertical)
			{
				return System.Drawing.Drawing2D.HatchStyle.DashedVertical;
			}
			else if(shapeFillStyle == ShapeFillStyle.DiagonalBrick)
			{
				return System.Drawing.Drawing2D.HatchStyle.DiagonalBrick;
			}
			else if(shapeFillStyle == ShapeFillStyle.DiagonalCross)
			{
				return System.Drawing.Drawing2D.HatchStyle.DiagonalCross;
			}
			else if(shapeFillStyle == ShapeFillStyle.Divot)
			{
				return System.Drawing.Drawing2D.HatchStyle.Divot;
			}
			else if(shapeFillStyle == ShapeFillStyle.DottedDiamond)
			{
				return System.Drawing.Drawing2D.HatchStyle.DottedDiamond;
			}
			else if(shapeFillStyle == ShapeFillStyle.DottedGrid)
			{
				return System.Drawing.Drawing2D.HatchStyle.DottedGrid;
			}
			else if(shapeFillStyle == ShapeFillStyle.ForwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.Horizontal)
			{
				return System.Drawing.Drawing2D.HatchStyle.Horizontal;
			}
			else if(shapeFillStyle == ShapeFillStyle.LargeCheckerBoard)
			{
				return System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard;
			}
			else if(shapeFillStyle == ShapeFillStyle.LargeConfetti)
			{
				return System.Drawing.Drawing2D.HatchStyle.LargeConfetti;
			}
			else if(shapeFillStyle == ShapeFillStyle.LargeGrid)
			{
				return System.Drawing.Drawing2D.HatchStyle.LargeGrid;
			}
			else if(shapeFillStyle == ShapeFillStyle.LightDownwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.LightHorizontal)
			{
				return System.Drawing.Drawing2D.HatchStyle.LightHorizontal;
			}
			else if(shapeFillStyle == ShapeFillStyle.LightUpwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.LightVertical)
			{
				return System.Drawing.Drawing2D.HatchStyle.LightVertical;
			}
			else if(shapeFillStyle == ShapeFillStyle.Max)
			{
				return System.Drawing.Drawing2D.HatchStyle.Max;
			}
			else if(shapeFillStyle == ShapeFillStyle.Min)
			{
				return System.Drawing.Drawing2D.HatchStyle.Min;
			}
			else if(shapeFillStyle == ShapeFillStyle.NarrowHorizontal)
			{
				return System.Drawing.Drawing2D.HatchStyle.NarrowHorizontal;
			}
			else if(shapeFillStyle == ShapeFillStyle.NarrowVertical)
			{
				return System.Drawing.Drawing2D.HatchStyle.NarrowVertical;
			}
			else if(shapeFillStyle == ShapeFillStyle.OutlinedDiamond)
			{
				return System.Drawing.Drawing2D.HatchStyle.OutlinedDiamond;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent05)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent05;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent10)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent10;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent20)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent20;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent25)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent25;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent30)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent30;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent40)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent40;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent50)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent50;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent60)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent60;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent70)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent70;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent75)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent75;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent80)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent80;
			}
			else if(shapeFillStyle == ShapeFillStyle.Percent90)
			{
				return System.Drawing.Drawing2D.HatchStyle.Percent90;
			}
			else if(shapeFillStyle == ShapeFillStyle.Plaid)
			{
				return System.Drawing.Drawing2D.HatchStyle.Plaid;
			}
			else if(shapeFillStyle == ShapeFillStyle.Shingle)
			{
				return System.Drawing.Drawing2D.HatchStyle.Shingle;
			}
			else if(shapeFillStyle == ShapeFillStyle.SmallCheckerBoard)
			{
				return System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard;
			}
			else if(shapeFillStyle == ShapeFillStyle.SmallConfetti)
			{
				return System.Drawing.Drawing2D.HatchStyle.SmallConfetti;
			}
			else if(shapeFillStyle == ShapeFillStyle.SmallGrid)
			{
				return System.Drawing.Drawing2D.HatchStyle.SmallGrid;
			}
			else if(shapeFillStyle == ShapeFillStyle.SolidDiamond)
			{
				return System.Drawing.Drawing2D.HatchStyle.SolidDiamond;
			}
			else if(shapeFillStyle == ShapeFillStyle.Sphere)
			{
				return System.Drawing.Drawing2D.HatchStyle.Sphere;
			}
			else if(shapeFillStyle == ShapeFillStyle.Trellis)
			{
				return System.Drawing.Drawing2D.HatchStyle.Trellis;
			}
			else if(shapeFillStyle == ShapeFillStyle.Wave)
			{
				return System.Drawing.Drawing2D.HatchStyle.Wave;
			}
			else if(shapeFillStyle == ShapeFillStyle.Weave)
			{
				return System.Drawing.Drawing2D.HatchStyle.Weave;
			}
			else if(shapeFillStyle == ShapeFillStyle.WideDownwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal;
			}
			else if(shapeFillStyle == ShapeFillStyle.WideUpwardDiagonal)
			{
				return System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal;
			}
			else
			{
				return System.Drawing.Drawing2D.HatchStyle.ZigZag;
			}
			
		}

		/*public byte Opacity
		{
			get
			{
				return m_parentProjectedLayer.Opacity;
			}
			set
			{
				if(m_NwImageLayer != null)
				{
					m_NwImageLayer.Opacity = value;
				}
				if(m_NeImageLayer != null)
				{
					m_NeImageLayer.Opacity = value;
				}
				if(m_SwImageLayer != null)
				{
					m_SwImageLayer.Opacity = value;
				}
				if(m_SeImageLayer != null)
				{
					m_SeImageLayer.Opacity = value;
				}

				if(m_NorthWestChild != null)
				{
					m_NorthWestChild.Opacity = value;
				}
				if(m_NorthEastChild != null)
				{
					m_NorthEastChild.Opacity = value;
				}
				if(m_SouthWestChild != null)
				{
					m_SouthWestChild.Opacity = value;
				}
				if(m_SouthEastChild != null)
				{
					m_SouthEastChild.Opacity = value;
				}
			}
		}*/

		private ImageLayer CreateImageLayer(double north, double south, double west, double east, DrawArgs drawArgs, string imagePath)
		{
			Bitmap b = null;
			Graphics g = null;
			ImageLayer imageLayer = null;
			GeographicBoundingBox geoBB = new GeographicBoundingBox(north, south, west, east);
			int numberPolygonsInTile = 0;

			FileInfo imageFile = new FileInfo(imagePath);

			if(!this.m_parentProjectedLayer.EnableCaching ||
				!imageFile.Exists
				)
			{
				if(this.m_parentProjectedLayer.LineStrings != null)
				{
					for(int i = 0; i < this.m_parentProjectedLayer.LineStrings.Length; i++)
					{
						if(!this.m_parentProjectedLayer.LineStrings[i].Visible)
							continue;
						
						GeographicBoundingBox currentBoundingBox = this.m_parentProjectedLayer.LineStrings[i].GetGeographicBoundingBox();

						if(currentBoundingBox != null && !currentBoundingBox.Intersects(geoBB))
						{
							continue;
						}
						else
						{
							if(b == null)
							{
								b = new Bitmap(this.m_parentProjectedLayer.TileSize.Width, this.m_parentProjectedLayer.TileSize.Height,
									System.Drawing.Imaging.PixelFormat.Format32bppArgb);
							}

							if(g == null)
							{
								g = Graphics.FromImage(b);
							}

                            this.drawLineString(this.m_parentProjectedLayer.LineStrings[i],
								g,
								geoBB,
								b.Size
								);
							
							numberPolygonsInTile++;
						}
					}
				}

				if(this.m_parentProjectedLayer.Polygons != null)
				{
					for(int i = 0; i < this.m_parentProjectedLayer.Polygons.Length; i++)
					{
						if(!this.m_parentProjectedLayer.Polygons[i].Visible)
							continue;

						GeographicBoundingBox currentBoundingBox = this.m_parentProjectedLayer.Polygons[i].GetGeographicBoundingBox();

						if(currentBoundingBox != null && !currentBoundingBox.Intersects(geoBB))
						{
							continue;
						}
						else
						{
							if(b == null)
							{
								b = new Bitmap(this.m_parentProjectedLayer.TileSize.Width, this.m_parentProjectedLayer.TileSize.Height,
									System.Drawing.Imaging.PixelFormat.Format32bppArgb);
							}

							if(g == null)
							{
								g = Graphics.FromImage(b);
							}

                            this.drawPolygon(this.m_parentProjectedLayer.Polygons[i],
								g,
								geoBB,
								b.Size);
	
							numberPolygonsInTile++;
						}
					}
				}
			}

			if(b != null)
			{
				System.Drawing.Imaging.BitmapData srcInfo = b.LockBits(new Rectangle(0, 0, 
					b.Width, b.Height), 
					System.Drawing.Imaging.ImageLockMode.ReadOnly, 
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				bool isBlank = true;
				unsafe
				{
					int* srcPointer = (int*)srcInfo.Scan0;
					for(int i = 0; i < b.Height; i++) 
					{
						for(int j = 0; j < b.Width; j++) 
						{
							int color = *srcPointer++;
						
							if(((color >> 24) & 0xff) > 0)
							{
								isBlank = false;
								break;
							}
						}

						srcPointer += (srcInfo.Stride>>2) - b.Width;
					}
				}

				b.UnlockBits(srcInfo);
				if(isBlank)
					numberPolygonsInTile = 0;
			}
          
		//	if(!m_parentProjectedLayer.EnableCaching)
		//	{
				string id = DateTime.Now.Ticks.ToString();

				if(b != null && numberPolygonsInTile > 0)
				{
					MemoryStream ms = new MemoryStream();
					b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

					//must copy original stream into new stream, if not, error occurs, not sure why
                    this.m_ImageStream = new MemoryStream(ms.GetBuffer());
					
					imageLayer = new ImageLayer(
						id, this.m_parentProjectedLayer.World,
						0, this.m_ImageStream,
						Color.Black.ToArgb(),
						(float)south,
						(float)north,
						(float)west,
						(float)east,
						1.0f//(float)m_parentProjectedLayer.Opacity / 255.0f
						, this.m_parentProjectedLayer.World.TerrainAccessor);
					
					ms.Close();
				}

		/*	}
			else if(imageFile.Exists || numberPolygonsInTile > 0)
			{
				string id = System.DateTime.Now.Ticks.ToString();

				if(b != null)
				{
					MemoryStream ms = new MemoryStream();
					b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
					if(!imageFile.Directory.Exists)
						imageFile.Directory.Create();

					//must copy original stream into new stream, if not, error occurs, not sure why
					m_ImageStream = new MemoryStream(ms.GetBuffer());
					ImageHelper.ConvertToDxt3(m_ImageStream, imageFile.FullName);
					
					ms.Close();
				}

				imageLayer = new WorldWind.Renderable.ImageLayer(
					id,
					m_parentProjectedLayer.World,
					0,
					imageFile.FullName,
					//System.Drawing.Color.Black.ToArgb(),
					(float)south,
					(float)north,
					(float)west,
					(float)east,
					1.0f,//(float)m_parentProjectedLayer.Opacity / 255.0f,
					m_parentProjectedLayer.World.TerrainAccessor);
				
				imageLayer.TransparentColor = System.Drawing.Color.Black.ToArgb();
			}
		*/			
			if(b != null)
			{
				b.Dispose();
			}
			if(g != null)
			{
				g.Dispose();
			}

			b = null;
			g = null;

			//might not be necessary
			//GC.Collect();

			return imageLayer;
		}
		
		Stream m_ImageStream;

		public void Initialize(DrawArgs drawArgs)
		{
			try
			{
                this.m_Initializing = true;


                this.UpdateImageLayers(drawArgs);
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
			finally
			{
                this.m_Initializing = false;
				if(this.m_Disposing)
				{
                    this.Dispose();
                    this.m_Initialized = false;
				}
				else
				{
                    this.m_Initialized = true;
				}
				
			}
		}

		private void UpdateImageLayers(DrawArgs drawArgs)
		{
			try
			{
                this.m_LastUpdate = DateTime.Now;

				if(this.m_NwImageLayer != null) this.m_NwImageLayer.Dispose();
			
				if(this.m_NeImageLayer != null) this.m_NeImageLayer.Dispose();
			
				if(this.m_SwImageLayer != null) this.m_SwImageLayer.Dispose();

				if(this.m_SeImageLayer != null) this.m_SeImageLayer.Dispose();

				double centerLatitude = 0.5 * (this.m_geographicBoundingBox.North + this.m_geographicBoundingBox.South);
				double centerLongitude = 0.5 * (this.m_geographicBoundingBox.West + this.m_geographicBoundingBox.East);

                this.m_NwImageLayer = this.CreateImageLayer(this.m_geographicBoundingBox.North, centerLatitude, this.m_geographicBoundingBox.West, centerLongitude, drawArgs,
					String.Format("{0}\\{1}\\{2}\\{3:D4}\\{3:D4}_{4:D4}.dds",
					null,//ShapeTile.CachePath,
					"R",//ConfigurationLoader.GetRenderablePathString(m_parentProjectedLayer),
                    this.Level + 1,
					2 * this.Row + 1,
					2 * this.Col));

                this.m_NeImageLayer = this.CreateImageLayer(this.m_geographicBoundingBox.North, centerLatitude, centerLongitude, this.m_geographicBoundingBox.East, drawArgs,
					String.Format("{0}\\{1}\\{2}\\{3:D4}\\{3:D4}_{4:D4}.dds",
					null,//ShapeTile.CachePath,
					"R",//ConfigurationLoader.GetRenderablePathString(m_parentProjectedLayer),
                    this.Level + 1,
					2 * this.Row + 1,
					2 * this.Col + 1));

                this.m_SwImageLayer = this.CreateImageLayer(centerLatitude, this.m_geographicBoundingBox.South, this.m_geographicBoundingBox.West, centerLongitude, drawArgs,
					String.Format("{0}\\{1}\\{2}\\{3:D4}\\{3:D4}_{4:D4}.dds",
					null,//ShapeTile.CachePath,
					"R",//ConfigurationLoader.GetRenderablePathString(m_parentProjectedLayer),
                    this.Level + 1,
					2 * this.Row,
					2 * this.Col));

                this.m_SeImageLayer = this.CreateImageLayer(centerLatitude, this.m_geographicBoundingBox.South, centerLongitude, this.m_geographicBoundingBox.East, drawArgs,
					String.Format("{0}\\{1}\\{2}\\{3:D4}\\{3:D4}_{4:D4}.dds",
					null,//ShapeTile.CachePath,
					"R",//ConfigurationLoader.GetRenderablePathString(m_parentProjectedLayer),
                    this.Level + 1,
					2 * this.Row,
					2 * this.Col + 1));

				if(this.m_NwImageLayer != null)
				{
                    this.m_NwImageLayer.Initialize(drawArgs);
				}
				if(this.m_NeImageLayer != null)
				{
                    this.m_NeImageLayer.Initialize(drawArgs);
				}
				if(this.m_SwImageLayer != null)
				{
                    this.m_SwImageLayer.Initialize(drawArgs);
				}
				if(this.m_SeImageLayer != null)
				{
                    this.m_SeImageLayer.Initialize(drawArgs);
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		DateTime m_LastUpdate = DateTime.Now;

		private void drawLineString(LineString lineString, Graphics g, GeographicBoundingBox dstBB, Size imageSize)
		{
			using(Pen p = new Pen(lineString.Color, lineString.LineWidth))
			{
				g.DrawLines(p, this.getScreenPoints(lineString.Coordinates, 0, lineString.Coordinates.Length, dstBB, imageSize));

			}
		}

		private void drawPolygon(Polygon polygon, Graphics g, GeographicBoundingBox dstBB, Size imageSize)
		{
			if(polygon.innerBoundaries != null && polygon.innerBoundaries.Length > 0)
			{
				if(polygon.Fill)
				{
					using(SolidBrush brush = new SolidBrush(polygon.PolgonColor))
					{
						g.FillPolygon(brush, this.getScreenPoints(polygon.outerBoundary.Points, 0, polygon.outerBoundary.Points.Length, dstBB, imageSize));
					}
				}
				
				if(polygon.Outline)
				{
					using(Pen p = new Pen(polygon.OutlineColor, polygon.LineWidth))
					{
						g.DrawPolygon(p, this.getScreenPoints(polygon.outerBoundary.Points, 0, polygon.outerBoundary.Points.Length, dstBB, imageSize));					
					}
				}

				if(polygon.Fill)
				{
					using(SolidBrush brush = new SolidBrush(Color.Black))
					{
						for(int i = 0; i < polygon.innerBoundaries.Length; i++)
						{
							g.FillPolygon(brush, this.getScreenPoints(polygon.innerBoundaries[i].Points, 0, polygon.innerBoundaries[i].Points.Length, dstBB, imageSize)
								);
						}
					}
				}				
			}
			else
			{
				if(polygon.Fill)
				{
					using(SolidBrush brush = new SolidBrush(polygon.PolgonColor))
					{
						g.FillPolygon(brush, this.getScreenPoints(polygon.outerBoundary.Points, 0, polygon.outerBoundary.Points.Length, dstBB, imageSize));
					}
				}

				if(polygon.Outline)
				{
					using(Pen p = new Pen(polygon.OutlineColor, polygon.LineWidth))
					{
						g.DrawPolygon(p, this.getScreenPoints(polygon.outerBoundary.Points, 0, polygon.outerBoundary.Points.Length, dstBB, imageSize));
					}
				}
			}
		}

		private Point[] getScreenPoints(Point3d[] sourcePoints, int offset, int length, GeographicBoundingBox dstBB, Size dstImageSize)
		{
			double degreesPerPixelX = (dstBB.East - dstBB.West) / (double)dstImageSize.Width;
			double degreesPerPixelY = (dstBB.North - dstBB.South) / (double)dstImageSize.Height;

			ArrayList screenPointList = new ArrayList();
			for(int i = offset; i < offset + length; i++)
			{
				double screenX = (sourcePoints[i].X - dstBB.West) / degreesPerPixelX;
				double screenY = (dstBB.North - sourcePoints[i].Y) / degreesPerPixelY;

				if(screenPointList.Count > 0)
				{
					Point v = (Point)screenPointList[screenPointList.Count - 1];
					if(v.X == (int)screenX && v.Y == (int)screenY)
					{
						continue;
					}
				}

				screenPointList.Add(new Point((int)screenX, (int)screenY));
			}

			if(screenPointList.Count <= 1)
				return new Point[] { new Point(0,0), new Point(0,0) };

			return (Point[])screenPointList.ToArray(typeof(Point));
		}
		
		private ProjectedVectorTile ComputeChild( DrawArgs drawArgs, double childSouth, double childNorth, double childWest, double childEast, double tileSize ) 
		{
			ProjectedVectorTile child = new ProjectedVectorTile(
				new GeographicBoundingBox(childNorth, childSouth, childWest, childEast), this.m_parentProjectedLayer);

			return child;
		}

		ProjectedVectorTile m_NorthWestChild;
		ProjectedVectorTile m_NorthEastChild;
		ProjectedVectorTile m_SouthWestChild;
		ProjectedVectorTile m_SouthEastChild;

		float m_verticalExaggeration = World.Settings.VerticalExaggeration;

		protected virtual void CreateMesh(GeographicBoundingBox geographicBoundingBox, int meshPointCount, ref CustomVertex.PositionColoredTextured[] vertices, ref short[] indices)
		{
			int upperBound = meshPointCount - 1;
			float scaleFactor = (float)1/upperBound;
			double latrange = Math.Abs(geographicBoundingBox.North - geographicBoundingBox.South);
			double lonrange;
			if(geographicBoundingBox.West < geographicBoundingBox.East)
				lonrange = geographicBoundingBox.East - geographicBoundingBox.West;
			else
				lonrange = 360.0f + geographicBoundingBox.East - geographicBoundingBox.West;

			double layerRadius = this.m_parentProjectedLayer.World.EquatorialRadius;

			int opacityColor = Color.FromArgb(
				//m_parentProjectedLayer.Opacity,
				0,0,0).ToArgb();
			vertices = new CustomVertex.PositionColoredTextured[meshPointCount * meshPointCount];
			for(int i = 0; i < meshPointCount; i++)
			{
				for(int j = 0; j < meshPointCount; j++)
				{	
					double height = 0;
					if(this.m_parentProjectedLayer.World.TerrainAccessor != null)
						height = this.m_verticalExaggeration * this.m_parentProjectedLayer.World.TerrainAccessor.GetElevationAt(
                                 (double)geographicBoundingBox.North - scaleFactor * latrange * i,
                                 (double)geographicBoundingBox.West + scaleFactor * lonrange * j,
                                 (double)upperBound / latrange);

					Vector3 pos = MathEngine.SphericalToCartesian( 
						geographicBoundingBox.North - scaleFactor*latrange*i,
						geographicBoundingBox.West + scaleFactor*lonrange*j, 
						layerRadius + height);
					
					vertices[i*meshPointCount + j].X = pos.X;
					vertices[i*meshPointCount + j].Y = pos.Y;
					vertices[i*meshPointCount + j].Z = pos.Z;
					
					vertices[i*meshPointCount + j].Tu = j*scaleFactor;
					vertices[i*meshPointCount + j].Tv = i*scaleFactor;
					vertices[i*meshPointCount + j].Color = opacityColor;
				}
			}

			indices = new short[2 * upperBound * upperBound * 3];
			for(int i = 0; i < upperBound; i++)
			{
				for(int j = 0; j < upperBound; j++)
				{
					indices[(2*3*i*upperBound) + 6*j] = (short)(i*meshPointCount + j);
					indices[(2*3*i*upperBound) + 6*j + 1] = (short)((i+1)*meshPointCount + j);
					indices[(2*3*i*upperBound) + 6*j + 2] = (short)(i*meshPointCount + j+1);
	
					indices[(2*3*i*upperBound) + 6*j + 3] = (short)(i*meshPointCount + j+1);
					indices[(2*3*i*upperBound) + 6*j + 4] = (short)((i+1)*meshPointCount + j);
					indices[(2*3*i*upperBound) + 6*j + 5] = (short)((i+1)*meshPointCount + j+1);
				}
			}
		}

		public virtual void ComputeChildren(DrawArgs drawArgs)
		{
			float tileSize = (float)(0.5*(this.m_geographicBoundingBox.North - this.m_geographicBoundingBox.South));
			//TODO: Stop children computation at some lower level
			if(tileSize>0.0001)
			{

				double CenterLat = 0.5f*(this.m_geographicBoundingBox.North + this.m_geographicBoundingBox.South);
				double CenterLon = 0.5f*(this.m_geographicBoundingBox.East + this.m_geographicBoundingBox.West);
			
				if(this.m_NorthWestChild == null && this.m_NwImageLayer != null && this.m_Initialized)
				{
                    this.m_NorthWestChild = this.ComputeChild(drawArgs, CenterLat, this.m_geographicBoundingBox.North, this.m_geographicBoundingBox.West, CenterLon, tileSize );
                    this.m_NorthWestChild.Level = this.Level++;
                    this.m_NorthWestChild.Row = 2 * this.Row + 1;
                    this.m_NorthWestChild.Col = 2 * this.Col;

                    this.m_NorthWestChild.Initialize(drawArgs);
				}

				if(this.m_NorthEastChild == null && this.m_NeImageLayer != null && this.m_Initialized)
				{
                    this.m_NorthEastChild = this.ComputeChild(drawArgs, CenterLat, this.m_geographicBoundingBox.North, CenterLon, this.m_geographicBoundingBox.East, tileSize );
                    this.m_NorthEastChild.Level = this.Level++;
                    this.m_NorthEastChild.Row = 2 * this.Row + 1;
                    this.m_NorthEastChild.Col = 2 * this.Col + 1;

                    this.m_NorthEastChild.Initialize(drawArgs);
				}

				if(this.m_SouthWestChild == null && this.m_SwImageLayer != null && this.m_Initialized)
				{
                    this.m_SouthWestChild = this.ComputeChild(drawArgs, this.m_geographicBoundingBox.South, CenterLat, this.m_geographicBoundingBox.West, CenterLon, tileSize );
                    this.m_SouthWestChild.Level = this.Level++;
                    this.m_SouthWestChild.Row = 2 * this.Row;
                    this.m_SouthWestChild.Col = 2 * this.Col;

                    this.m_SouthWestChild.Initialize(drawArgs);
				}

				if(this.m_SouthEastChild == null && this.m_SeImageLayer != null && this.m_Initialized)
				{
                    this.m_SouthEastChild = this.ComputeChild(drawArgs, this.m_geographicBoundingBox.South, CenterLat, CenterLon, this.m_geographicBoundingBox.East, tileSize );
                    this.m_SouthEastChild.Level = this.Level++;
                    this.m_SouthEastChild.Row = 2 * this.Row;
                    this.m_SouthEastChild.Col = 2 * this.Col + 1;

                    this.m_SouthEastChild.Initialize(drawArgs);
				}
			}
		}

		public void Update(DrawArgs drawArgs)
		{
			try
			{
				double centerLatitude = 0.5 * (this.m_geographicBoundingBox.North + this.m_geographicBoundingBox.South);
				double centerLongitude = 0.5 * (this.m_geographicBoundingBox.West + this.m_geographicBoundingBox.East);
				double tileSize = this.m_geographicBoundingBox.North - this.m_geographicBoundingBox.South;

				if(!this.m_Initialized)
				{
					if(drawArgs.WorldCamera.ViewRange * 0.5f < Angle.FromDegrees(ShapeTileArgs.TileDrawDistance * tileSize) 
						&& MathEngine.SphericalDistance(Angle.FromDegrees(centerLatitude), Angle.FromDegrees(centerLongitude), 
						drawArgs.WorldCamera.Latitude, drawArgs.WorldCamera.Longitude) < Angle.FromDegrees( ShapeTileArgs.TileSpreadFactor * tileSize * 1.25f )
						&& drawArgs.WorldCamera.ViewFrustum.Intersects(this.BoundingBox)
						)
					{
                        this.Initialize(drawArgs);
					}
				}

				if(this.m_Initialized)
				{
					if(this.m_LastUpdate < this.m_parentProjectedLayer.LastUpdate) this.UpdateImageLayers(drawArgs);

					if(this.m_NwImageLayer != null)
					{
                        this.m_NwImageLayer.Update(drawArgs);
					}
					if(this.m_NeImageLayer != null)
					{
                        this.m_NeImageLayer.Update(drawArgs);
					}
					if(this.m_SwImageLayer != null)
					{
                        this.m_SwImageLayer.Update(drawArgs);
					}
					if(this.m_SeImageLayer != null)
					{
                        this.m_SeImageLayer.Update(drawArgs);
					}

					if(
						drawArgs.WorldCamera.ViewRange < Angle.FromDegrees(ShapeTileArgs.TileDrawDistance*tileSize) && 
						MathEngine.SphericalDistance( Angle.FromDegrees(centerLatitude), Angle.FromDegrees(centerLongitude), 
						drawArgs.WorldCamera.Latitude, drawArgs.WorldCamera.Longitude) < Angle.FromDegrees( ShapeTileArgs.TileSpreadFactor*tileSize )
						&& drawArgs.WorldCamera.ViewFrustum.Intersects(this.BoundingBox)
						)
					{
						if(this.m_NorthEastChild == null && this.m_NorthWestChild == null && this.m_SouthEastChild == null && this.m_SouthWestChild == null)
						{
                            this.ComputeChildren(drawArgs);
						}
						else
						{
							if(this.m_NorthEastChild != null)
							{
                                this.m_NorthEastChild.Update(drawArgs);
							}

							if(this.m_NorthWestChild != null)
							{
                                this.m_NorthWestChild.Update(drawArgs);
							}

							if(this.m_SouthEastChild != null)
							{
                                this.m_SouthEastChild.Update(drawArgs);
							}

							if(this.m_SouthWestChild != null)
							{
                                this.m_SouthWestChild.Update(drawArgs);
							}
						}
					}
					else 
					{
						if(this.m_NorthWestChild != null)
						{
                            this.m_NorthWestChild.Dispose();
                            this.m_NorthWestChild = null;
						}

						if(this.m_NorthEastChild != null)
						{
                            this.m_NorthEastChild.Dispose();
                            this.m_NorthEastChild = null;
						}

						if(this.m_SouthEastChild != null)
						{
                            this.m_SouthEastChild.Dispose();
                            this.m_SouthEastChild = null;
						}

						if(this.m_SouthWestChild != null)
						{
                            this.m_SouthWestChild.Dispose();
                            this.m_SouthWestChild = null;
						}
					}
				}

				if(this.m_Initialized)
				{
					if(drawArgs.WorldCamera.ViewRange > Angle.FromDegrees( ShapeTileArgs.TileDrawDistance*tileSize*1.5f )
						|| MathEngine.SphericalDistance(Angle.FromDegrees(centerLatitude), Angle.FromDegrees(centerLongitude), drawArgs.WorldCamera.Latitude, drawArgs.WorldCamera.Longitude) > Angle.FromDegrees( ShapeTileArgs.TileSpreadFactor*tileSize*1.5f ))
					{
						if(this.Level != 0)
						//{
                            this.Dispose();
						//}
					}
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

		public void Render(DrawArgs drawArgs)
		{
			try
			{
				if(this.m_Initialized)
				{
					if(this.m_NorthWestChild != null && this.m_NorthWestChild.m_Initialized)
					{
                        this.m_NorthWestChild.Render(drawArgs);
					}
					else if(this.m_NwImageLayer != null && this.m_NwImageLayer.Initialized)
					{
                        this.m_NwImageLayer.Render(drawArgs);
					}

					if(this.m_NorthEastChild != null && this.m_NorthEastChild.m_Initialized)
					{
                        this.m_NorthEastChild.Render(drawArgs);
					}
					else if(this.m_NeImageLayer != null && this.m_NeImageLayer.Initialized)
					{
                        this.m_NeImageLayer.Render(drawArgs);
					}

					if(this.m_SouthWestChild != null && this.m_SouthWestChild.m_Initialized)
					{
                        this.m_SouthWestChild.Render(drawArgs);
					}
					else if(this.m_SwImageLayer != null && this.m_SwImageLayer.Initialized)
					{
                        this.m_SwImageLayer.Render(drawArgs);
					}

					if(this.m_SouthEastChild != null && this.m_SouthEastChild.m_Initialized)
					{
                        this.m_SouthEastChild.Render(drawArgs);
					}
					else if(this.m_SeImageLayer != null && this.m_SeImageLayer.Initialized)
					{
                        this.m_SeImageLayer.Render(drawArgs);
					}
				}
			}
			catch//(Exception ex)
			{
				//Log.Write(ex);
			}
		}
	}
}
