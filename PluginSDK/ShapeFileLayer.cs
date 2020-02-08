using System;
using System.Collections;
using System.Drawing;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind
{
	/// <summary>
	/// Summary description for ShapeFileLayer.
	/// </summary>
	/// 

	//TODO: Upper and lower random color limits, line "caps" styles
	public class ShapeFileLayer : RenderableObject
	{
		int m_NumberRootTilesHigh = 5;
		ShapeTileArgs m_ShapeTileArgs;
		ShapeTile[] m_RootTiles;
		string m_ShapeFilePath;
		
		double m_BoundingBoxXMin;
		double m_BoundingBoxYMin;
		double m_BoundingBoxXMax;
		double m_BoundingBoxYMax;
		double m_BoundingBoxZMin;
		double m_BoundingBoxZMax;
		double m_BoundingBoxMMin;
		double m_BoundingBoxMMax;

		double m_ScalarFilterMin = double.NaN;
		double m_ScalarFilterMax = double.NaN;
		double m_MinimumViewingAltitude;
		double m_MaximumViewingAltitude = double.MaxValue;
		double m_lztsd = 36.0;


		int m_IconWidth;
		int m_IconHeight;
		string m_IconFilePath;
		Texture m_IconTexture;
		SurfaceDescription m_IconTextureDescription;

		byte m_IconOpacity = 255;

        public double North
        {
            get
            {
                return this.m_BoundingBoxYMax;
            }
        }

        public double South
        {
            get
            {
                return this.m_BoundingBoxYMin;
            }
        }

        public double East
        {
            get
            {
                return this.m_BoundingBoxXMax;
            }
        }

        public double West
        {
            get
            {
                return this.m_BoundingBoxXMin;
            }
        }

        public double MinAltitude
        {
            get
            {
                return this.m_MinimumViewingAltitude;
            }
        }

        public double MaxAltitude
        {
            get
            {
                return this.m_MaximumViewingAltitude;
            }
        }

		public ShapeFileLayer(
			string id,
			World parentWorld,
			string shapeFilePath,
			double minimumViewingAltitude,
			double maximumViewingAltitude,
			float lztsd,
			GeographicBoundingBox bounds,
			string dataKey,
			bool scaleColorsToData,
			double scalarFilterMin,
			double scalarFilterMax,
			double scaleMin,
			double scaleMax,
			string[] noDataValues,
			string[] activeDataValues,
			bool polygonFill,
			bool outlinePolygons,
			Color polygonFillColor,
			ShapeFillStyle shapeFillHatchStyle,
			Color lineColor,
			float lineWidth,
			bool showLabels,
			Color labelColor,
			string iconFilePath,
			int iconWidth,
			int iconHeight,
			byte iconOpacity) : base(id, parentWorld.Position, parentWorld.Orientation)
		{

			this.RenderPriority = RenderPriority.LinePaths;

            this.m_MinimumViewingAltitude = minimumViewingAltitude;
            this.m_MaximumViewingAltitude = maximumViewingAltitude;
            this.m_lztsd = lztsd;

            this.m_ShapeTileArgs = new ShapeTileArgs(
				parentWorld,
				new Size(256, 256),
				parentWorld.EquatorialRadius,
				this,
				dataKey,
				scaleColorsToData,
				scaleMin,
				scaleMax,
				noDataValues,
				activeDataValues,
				polygonFill,
				outlinePolygons,
				polygonFillColor,
				shapeFillHatchStyle,
				lineColor,
				labelColor,
				lineWidth,
				showLabels
				);

            this.m_ScalarFilterMin = scalarFilterMin;
            this.m_ScalarFilterMax = scalarFilterMax;

            this.m_ShapeFilePath = shapeFilePath;

            this.m_IconFilePath = iconFilePath;
            this.m_IconWidth = iconWidth;
            this.m_IconHeight = iconHeight;
            this.m_IconOpacity = iconOpacity;
			/*Produces tile tree for whole earth*/
			/*Need to implement clipping*/
            this.m_NumberRootTilesHigh = (int)(180.0f/ this.m_lztsd);
			double tileSize = 180.0f/ this.m_NumberRootTilesHigh;
            this.m_RootTiles = new ShapeTile[this.m_NumberRootTilesHigh * (this.m_NumberRootTilesHigh * 2)];

			Console.WriteLine("North:{0} South:{1} East:{2} West:{3}",
				bounds.North,bounds.South,bounds.East,bounds.West);
			int istart = 0;
			int iend = this.m_NumberRootTilesHigh;
			int jstart = 0;
			int jend = this.m_NumberRootTilesHigh * 2;

			int createdtiles = 0;
			for(int i = istart; i < iend; i++)
			{
				for(int j = jstart; j < jend; j++)
				{
					double north = (i + 1) * tileSize - 90.0f;
					double south = i  * tileSize - 90.0f;
					double west = j * tileSize - 180.0f;
					double east = (j + 1) * tileSize - 180.0f;
                    this.m_RootTiles[i * this.m_NumberRootTilesHigh * 2 + j] = new ShapeTile(
							new GeographicBoundingBox(
							north,
							south,
							west,
							east), this.m_ShapeTileArgs);
					createdtiles++;
				}
			}
			Console.WriteLine("Created Tiles "+createdtiles);
		}

		public override void Dispose()
		{
            this.isInitialized = false;
			if(this.m_IconTexture != null && !this.m_IconTexture.Disposed)
			{
                this.m_IconTexture.Dispose();
			}
			if(this.m_Sprite != null && !this.m_Sprite.Disposed)
			{
                this.m_Sprite.Dispose();
			}
			foreach(ShapeTile shapeTile in this.m_RootTiles)
			{
				shapeTile.Dispose();
			}
		}

		public override byte Opacity 
		{
			get
			{
				return base.Opacity;
			}
			set
			{
				if(this.m_RootTiles != null)
				{
					foreach(ShapeTile tile in this.m_RootTiles)
					{
						if(tile != null)
						{
							tile.Opacity = value;
						}
					}
				}
				base.Opacity = value;	
			}
		}

		Sprite m_Sprite;
		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
                this.m_Sprite = new Sprite(drawArgs.device);
				if(this.m_IconFilePath != null && File.Exists(this.m_IconFilePath))
				{
                    this.m_IconTexture = ImageHelper.LoadIconTexture(this.m_IconFilePath);
                    this.m_IconTextureDescription = this.m_IconTexture.GetLevelDescription(0);
				}
				if(this.m_ShapeFilePath.ToLower().EndsWith(".zip"))
				{
                    this.loadZippedShapeFile(this.m_ShapeFilePath);
				}
				else
				{
                    this.loadShapeFile(this.m_ShapeFilePath);
				}

				if((this.m_ShapeTileArgs.ShowLabels && this.m_ShapeTileArgs.DataKey != null) || this.m_IconTexture != null)
				{
					foreach(ShapeRecord record in this.m_ShapeTileArgs.ShapeRecords)
					{
						if(record.Value != null)
						{
							if(record.Point != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = record.Point.X;
								p.Y = record.Point.Y;
								p.Tag = record.Value;
                                this.m_LabelList.Add(p);
							}
							else if(record.MultiPoint != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = 0.5 * (record.MultiPoint.BoundingBox.West + record.MultiPoint.BoundingBox.East);
								p.Y = 0.5 * (record.MultiPoint.BoundingBox.North + record.MultiPoint.BoundingBox.South);
								p.Tag = record.Value;
                                this.m_LabelList.Add(p);
							}
							else if(record.PolyLine != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = 0.5 * (record.PolyLine.BoundingBox.West + record.PolyLine.BoundingBox.East);
								p.Y = 0.5 * (record.PolyLine.BoundingBox.North + record.PolyLine.BoundingBox.South);
								p.Tag = record.Value;
                                this.m_LabelList.Add(p);
							}
							else if(record.Polygon != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = 0.5 * (record.Polygon.BoundingBox.West + record.Polygon.BoundingBox.East);
								p.Y = 0.5 * (record.Polygon.BoundingBox.North + record.Polygon.BoundingBox.South);
								p.Tag = record.Value;
                                this.m_LabelList.Add(p);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
			finally
			{
                this.isInitialized = true;
			}
		}

		ArrayList m_LabelList = new ArrayList();
		ArrayList m_PointList = new ArrayList();

		public override void Update(DrawArgs drawArgs)
		{
			if(drawArgs.WorldCamera.AltitudeAboveTerrain >= this.m_MinimumViewingAltitude &&
				drawArgs.WorldCamera.AltitudeAboveTerrain <= this.m_MaximumViewingAltitude)
			{
				if(!this.isInitialized)
				{
                    this.Initialize(drawArgs);
				}

				foreach(ShapeTile shapeTile in this.m_RootTiles)
				{
					if(shapeTile!=null&&(shapeTile.m_GeoBB.North-shapeTile.m_GeoBB.South)<= this.m_lztsd)
						shapeTile.Update(drawArgs);
				}
			}
			else
			{
				if(this.isInitialized)
				{
                    this.Dispose();
				}
			}

		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(!this.isInitialized || 
				drawArgs.WorldCamera.AltitudeAboveTerrain < this.m_MinimumViewingAltitude ||
				drawArgs.WorldCamera.AltitudeAboveTerrain > this.m_MaximumViewingAltitude
				)
			{
				return;
			}

			try
			{
				foreach(ShapeTile shapeTile in this.m_RootTiles)
				{
					if(shapeTile!=null&&(shapeTile.m_GeoBB.North-shapeTile.m_GeoBB.South)<= this.m_lztsd)
						shapeTile.Render(drawArgs);
				}

				Vector3 referenceCenter = new Vector3(
					(float)drawArgs.WorldCamera.ReferenceCenter.X,
					(float)drawArgs.WorldCamera.ReferenceCenter.Y,
					(float)drawArgs.WorldCamera.ReferenceCenter.Z);

				if(this.m_PointList.Count > 0)
				{
					drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
						
					float curPointSize = drawArgs.device.SetRenderState(RenderState.PointSize;
						
					drawArgs.device.SetRenderState(RenderState.PointSize = 5.0f;
					drawArgs.device.SetRenderState(RenderState.ZBufferEnable = false;
					CustomVertex.PositionColored[] verts = new SharpDX.Direct3D9.CustomVertex.PositionColored[1];
					Vector3 camPoint = MathEngine.SphericalToCartesian(drawArgs.WorldCamera.Latitude.Degrees, drawArgs.WorldCamera.Longitude.Degrees, this.m_ShapeTileArgs.LayerRadius);
					
					drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(-referenceCenter);
					foreach(Vector3 v in this.m_PointList)
					{
						if(Vector3.Subtract(v, camPoint).Length() < this.m_ShapeTileArgs.LayerRadius)
						{
							verts[0].Color = this.m_ShapeTileArgs.LabelColor.ToArgb();
							verts[0].X = v.X;
							verts[0].Y = v.Y;
							verts[0].Z = v.Z;

							drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
							drawArgs.device.DrawUserPrimitives(PrimitiveType.PointList, 1, verts);
						}
					}

					drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix);
					drawArgs.device.SetRenderState(RenderState.PointSize = curPointSize;
					drawArgs.device.SetRenderState(RenderState.ZBufferEnable = true;
				}

				if(this.m_LabelList.Count > 0)
				{
					Color iconColor = Color.FromArgb(this.m_IconOpacity, 255, 255, 255);
					foreach(Shapefile_Point p in this.m_LabelList)
					{
						Vector3 cartesianPoint = MathEngine.SphericalToCartesian(p.Y, p.X, drawArgs.WorldCamera.WorldRadius + drawArgs.WorldCamera.TerrainElevation);
					
						if(!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(cartesianPoint) ||
							MathEngine.SphericalDistanceDegrees(p.Y, p.X, drawArgs.WorldCamera.Latitude.Degrees, drawArgs.WorldCamera.Longitude.Degrees) > 90.0)
							continue;

						Vector3 projectedPoint = drawArgs.WorldCamera.Project(cartesianPoint - referenceCenter);

						/*if(isMouseOver)
						{
							// Mouse is over
							isMouseOver = true;

							if(icon.isSelectable)
								DrawArgs.MouseCursor = CursorType.Hand;

							string description = icon.Description;
							if(description==null)
								description = icon.ClickableActionURL;
							if(description!=null)
							{
								// Render description field
								DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Bottom;
								int left = 10;
								if(World.Settings.showLayerManager)
									left += World.Settings.layerManagerWidth;
								Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10 );

								// Draw outline
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );
					
								rect.Offset(2,0);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );

								rect.Offset(0,2);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );

								rect.Offset(-2,0);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );

								// Draw description
								rect.Offset(1,-1);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect, 
									format, descriptionColor );
							}
						}*/
                        this.m_Sprite.Begin(SpriteFlags.AlphaBlend);

						if(this.m_IconTexture != null)
						{
							float xscale = (float) this.m_IconWidth / this.m_IconTextureDescription.Width;
							float yscale = (float) this.m_IconHeight / this.m_IconTextureDescription.Height;
                            this.m_Sprite.Transform = Matrix.Scaling(xscale,yscale,0);
                            this.m_Sprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
                            this.m_Sprite.Draw(this.m_IconTexture,
								new Vector3(this.m_IconWidth>>1, this.m_IconHeight>>1,0),
								Vector3.Zero,
								iconColor.ToArgb() );
				
							// Reset transform to prepare for text rendering later
                            this.m_Sprite.Transform = Matrix.Identity;
						}

						if(this.m_ShapeTileArgs.ShowLabels && this.m_ShapeTileArgs.DataKey != null)
						{
						
							// Render label
							if(p.Tag != null)
							{
								// Render name field
								const int labelWidth = 1000; // Dummy value needed for centering the text
								if(this.m_IconTexture==null)
								{
									// Center over target as we have no bitmap
									Rectangle rect = new Rectangle(
										(int)projectedPoint.X - (labelWidth>>1), 
										(int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
										labelWidth, 
										drawArgs.screenHeight );

									drawArgs.defaultDrawingFont.DrawText(this.m_Sprite, p.Tag.ToString(), rect, DrawTextFormat.Center, this.m_ShapeTileArgs.LabelColor);
								}
								else
								{
									// Adjust text to make room for icon
									int spacing = (int)(this.m_IconWidth * 0.3f);
									if(spacing>10)
										spacing = 10;
									int offsetForIcon = (this.m_IconWidth>>1) + spacing;

									Rectangle rect = new Rectangle(
										(int)projectedPoint.X + offsetForIcon, 
										(int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
										labelWidth, 
										drawArgs.screenHeight );

									drawArgs.defaultDrawingFont.DrawText(this.m_Sprite, p.Tag.ToString(), rect, DrawTextFormat.WordBreak, this.m_ShapeTileArgs.LabelColor);
								}
							}
						}

                        this.m_Sprite.End();
					}
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

				
		//Loads a Zipped Shapefile without extracting
		//return==true means it was successfull
		private void loadZippedShapeFile(string shapeFilePath)
		{							
			//ZipFileIndexes
			int shpIndex = -1;
			int	dbfIndex = -1;

			try
			{		
				//Navigate the Zip to find the files and update their index
				ZipFile zFile = new ZipFile(shapeFilePath);
				foreach (ZipEntry ze in zFile) 
				{
					if(ze.Name.ToLower().EndsWith(".shp"))
						shpIndex=ze.ZipFileIndex;					
					else if(ze.Name.ToLower().EndsWith(".dbf"))
						dbfIndex=ze.ZipFileIndex;					
				}							
			}
			catch { /* Ignore */ }			


			if((dbfIndex == -1)||(shpIndex == -1))
				return ;
			
			Random random = new Random(Path.GetFileName(shapeFilePath).GetHashCode());

			ArrayList metaValues = new ArrayList();
			
			if(this.m_ShapeTileArgs.DataKey != null)
			{
				ExtendedZipInputStream dbfReader =
					new ExtendedZipInputStream(File.OpenRead(shapeFilePath));

				ZipEntry dbfEntry= null;			
				dbfEntry = dbfReader.GetNextEntry();
				for(int p=0;p<dbfIndex;p++)
				{
					dbfEntry = dbfReader.GetNextEntry();
				}

				if(!dbfEntry.IsFile)
					return;
				
				byte dbfVersion = dbfReader.ReadByte();

				byte updateYear = dbfReader.ReadByte();
				byte updateMonth = dbfReader.ReadByte();
				byte updateDay = dbfReader.ReadByte();

				int numberRecords = dbfReader.ReadInt32();
				short headerLength = dbfReader.ReadInt16();

				short recordLength = dbfReader.ReadInt16();
				byte[] reserved = dbfReader.ReadBytes(20);

				int numberFields = (headerLength - 33) / 32;

				// Read Field Descriptor Array
				DBF_Field_Header[] fieldHeaders = new DBF_Field_Header[numberFields];

				for (int i = 0; i < numberFields; i++)
				{
					char[] fieldNameChars = dbfReader.ReadChars(10);
					char fieldNameTerminator = dbfReader.ReadChar();
					string fn = new string(fieldNameChars);
					fieldHeaders[i].FieldName = fn.Trim().Replace(" ","");

					fieldHeaders[i].FieldType = dbfReader.ReadChar();
					byte[] reserved1 = dbfReader.ReadBytes(4);

					if(String.Compare(fieldHeaders[i].FieldName.Trim(), this.m_ShapeTileArgs.DataKey, true) == 0)
					{
                        this.m_ShapeTileArgs.DataKey = fieldHeaders[i].FieldName;
							
						if(fieldHeaders[i].FieldType == 'N')
						{
                            this.m_ShapeTileArgs.UseScalar = true;
						}
						else
						{
                            this.m_ShapeTileArgs.UseScalar = false;
						}

					}

					fieldHeaders[i].FieldLength = dbfReader.ReadByte();

					byte[] reserved2 = dbfReader.ReadBytes(15);
					
				}
				byte headerTerminator = dbfReader.ReadByte();

				double scalarMin = double.MaxValue;
				double scalarMax = double.MinValue;
				for (int i = 0; i < numberRecords; i++)
				{
					//	Shapefile_Polygon curPoly = (Shapefile_Polygon)this.m_ShapeTileArgs.ShapeRecords[i];
								
					byte isValid = dbfReader.ReadByte();
					for (int j = 0; j < fieldHeaders.Length; j++)
					{
						char[] fieldDataChars = dbfReader.ReadChars(fieldHeaders[j].FieldLength);
						string fieldData = new string(fieldDataChars);

						if(fieldHeaders[j].FieldName == this.m_ShapeTileArgs.DataKey)
						{
							metaValues.Add(fieldData);

							if(fieldHeaders[j].FieldType == 'N')
							{
								try
								{
									if(this.m_ShapeTileArgs.ScaleMin == double.NaN || this.m_ShapeTileArgs.ScaleMax == double.NaN)
									{
										double data = double.Parse(fieldData);
										if(this.m_ShapeTileArgs.ScaleMin == double.NaN && data < scalarMin)
										{
											scalarMin = data;
										}

										if(this.m_ShapeTileArgs.ScaleMax == double.NaN && data > scalarMax)
										{
											scalarMax = data;
										}
									}
								}
								catch(Exception ex)
								{
									Log.Write(ex);
								}
							}
							else
							{
								if(!this.m_ShapeTileArgs.ColorAssignments.Contains(fieldData))
								{
									Color newColor = 
										Color.FromArgb(
										1 + random.Next(254),
										1 + random.Next(254),
										1 + random.Next(254));

                                    this.m_ShapeTileArgs.ColorAssignments.Add(fieldData, newColor);		
								}
							}
						}
					}

					if(this.m_ShapeTileArgs.UseScalar && this.m_ShapeTileArgs.ScaleMin == double.NaN)
					{
                        this.m_ShapeTileArgs.ScaleMin = scalarMin;
					}
					if(this.m_ShapeTileArgs.UseScalar && this.m_ShapeTileArgs.ScaleMax == double.NaN)
					{
                        this.m_ShapeTileArgs.ScaleMax = scalarMax;
					}
				}
				dbfReader.Close();
										
			}


			ExtendedZipInputStream shpReader= new ExtendedZipInputStream(File.OpenRead(shapeFilePath));
			
			ZipEntry shpEntry= null;			
			shpEntry = shpReader.GetNextEntry();
			for(int p=0;p<shpIndex;p++)
			{
				shpEntry = shpReader.GetNextEntry();
			}

			if(!shpEntry.IsFile)
				return ;

	
			//get file header info
			// Big-Endian Integer File Type
			byte[] fileTypeBytes = shpReader.ReadBytes(4);
			int fileType = 16 * 16 * 16 * 16 * 16 * 16 * fileTypeBytes[0] + 16 * 16 * 16 * 16 * fileTypeBytes[1] + 16 * 16 * fileTypeBytes[2] + fileTypeBytes[3];

			byte[] unused1 = shpReader.ReadBytes(5 * 4);

			byte[] fileLengthBytes = shpReader.ReadBytes(4);
			int fileLength = 16 * 16 * 16 * 16 * 16 * 16 * fileLengthBytes[0] + 16 * 16 * 16 * 16 * fileLengthBytes[1] + 16 * 16 * fileLengthBytes[2] + fileLengthBytes[3];

			int version = shpReader.ReadInt32();
			int shapeType = shpReader.ReadInt32();

            this.m_BoundingBoxXMin = shpReader.ReadDouble();
            this.m_BoundingBoxYMin = shpReader.ReadDouble();
            this.m_BoundingBoxXMax = shpReader.ReadDouble();
            this.m_BoundingBoxYMax = shpReader.ReadDouble();
            this.m_BoundingBoxZMin = shpReader.ReadDouble();
            this.m_BoundingBoxZMax = shpReader.ReadDouble();
            this.m_BoundingBoxMMin = shpReader.ReadDouble();
            this.m_BoundingBoxMMax = shpReader.ReadDouble();
			
			//start reading records...
			int bytesRead = 100;
			int counter = 0;

			while (bytesRead < shpEntry.Size)
			{
				ArrayList pendingPoints = new ArrayList();
				
				//read record header
				byte[] recordNumberBytes = shpReader.ReadBytes(4);
				byte[] contentLengthBytes = shpReader.ReadBytes(4);

				int recordNumber = 16 * 16 * 16 * 16 * 16 * 16 * recordNumberBytes[0] + 16 * 16 * 16 * 16 * recordNumberBytes[1] + 16 * 16 * recordNumberBytes[2] + recordNumberBytes[3];
				int contentLength = 16 * 16 * 16 * 16 * 16 * 16 * contentLengthBytes[0] + 16 * 16 * 16 * 16 * contentLengthBytes[1] + 16 * 16 * contentLengthBytes[2] + contentLengthBytes[3];

				//read shape type to determine record structure and content
				int recordShapeType = shpReader.ReadInt32();
				
				ShapeRecord newRecord = new ShapeRecord();

				if(recordShapeType == 0) //Null shape type -- generally used as a placeholder
				{
					newRecord.Null = new Shapefile_Null();
				}
				else if(recordShapeType == 1) //Point shape type
				{
					newRecord.Point = new Shapefile_Point();
					newRecord.Point.X = shpReader.ReadDouble();
					newRecord.Point.Y = shpReader.ReadDouble();					

					pendingPoints.Add(
						MathEngine.SphericalToCartesian(newRecord.Point.Y, newRecord.Point.X, this.m_ShapeTileArgs.LayerRadius));
				}
				else if(recordShapeType == 8) //Multi-point shape type
				{
					newRecord.MultiPoint = new Shapefile_MultiPoint();
					newRecord.MultiPoint.BoundingBox.West = shpReader.ReadDouble();
					newRecord.MultiPoint.BoundingBox.South = shpReader.ReadDouble();
					newRecord.MultiPoint.BoundingBox.East = shpReader.ReadDouble();
					newRecord.MultiPoint.BoundingBox.North = shpReader.ReadDouble();

					newRecord.MultiPoint.NumPoints = shpReader.ReadInt32();
					
					newRecord.MultiPoint.Points = new Shapefile_Point[newRecord.MultiPoint.NumPoints];
					for(int i = 0; i < newRecord.MultiPoint.NumPoints; i++)
					{
						newRecord.MultiPoint.Points[i] = new Shapefile_Point();
						newRecord.MultiPoint.Points[i].X = shpReader.ReadDouble();
						newRecord.MultiPoint.Points[i].Y = shpReader.ReadDouble();
						
						pendingPoints.Add(
							MathEngine.SphericalToCartesian(newRecord.MultiPoint.Points[i].Y, newRecord.MultiPoint.Points[i].X, this.m_ShapeTileArgs.LayerRadius));
					}
				}
				else if(recordShapeType == 3)
				{
					newRecord.PolyLine = new Shapefile_PolyLine();
				
					newRecord.PolyLine.BoundingBox.West = shpReader.ReadDouble();
					newRecord.PolyLine.BoundingBox.South = shpReader.ReadDouble();
					newRecord.PolyLine.BoundingBox.East = shpReader.ReadDouble();
					newRecord.PolyLine.BoundingBox.North = shpReader.ReadDouble();
				
					newRecord.PolyLine.NumParts = shpReader.ReadInt32();
					newRecord.PolyLine.NumPoints = shpReader.ReadInt32();
					
					newRecord.PolyLine.Parts = new int[newRecord.PolyLine.NumParts];

					for (int i = 0; i < newRecord.PolyLine.Parts.Length; i++)
					{
						newRecord.PolyLine.Parts[i] = shpReader.ReadInt32();					
					}

					newRecord.PolyLine.Points = new Shapefile_Point[newRecord.PolyLine.NumPoints];
					for (int i = 0; i < newRecord.PolyLine.Points.Length; i++)
					{
						newRecord.PolyLine.Points[i] = new Shapefile_Point();
						newRecord.PolyLine.Points[i].X = shpReader.ReadDouble();
						newRecord.PolyLine.Points[i].Y = shpReader.ReadDouble();						
					}
				}
				else if(recordShapeType == 5)
				{
					newRecord.Polygon = new Shapefile_Polygon();
				
					newRecord.Polygon.BoundingBox.West = shpReader.ReadDouble();
					newRecord.Polygon.BoundingBox.South = shpReader.ReadDouble();
					newRecord.Polygon.BoundingBox.East = shpReader.ReadDouble();
					newRecord.Polygon.BoundingBox.North = shpReader.ReadDouble();
				
					newRecord.Polygon.NumParts = shpReader.ReadInt32();
					newRecord.Polygon.NumPoints = shpReader.ReadInt32();
					
					newRecord.Polygon.Parts = new int[newRecord.Polygon.NumParts];

					for (int i = 0; i < newRecord.Polygon.Parts.Length; i++)
					{
						newRecord.Polygon.Parts[i] = shpReader.ReadInt32();
						
					}
				

					newRecord.Polygon.Points = new Shapefile_Point[newRecord.Polygon.NumPoints];

					for (int i = 0; i < newRecord.Polygon.Points.Length; i++)
					{
						newRecord.Polygon.Points[i] = new Shapefile_Point();
						

						byte[] temp=new byte[16];
						for(int t=0;t<16;t++)
						{
							temp[t]=shpReader.ReadByte();
						}		
						newRecord.Polygon.Points[i].X=BitConverter.ToDouble(temp,0);
						newRecord.Polygon.Points[i].Y=BitConverter.ToDouble(temp,8);
					}
					
				}

				bool ignoreRecord = false;
					
				if(metaValues != null && metaValues.Count > 0)
				{
					newRecord.Value = metaValues[counter];
				
					if(this.m_ShapeTileArgs.ActiveDataValues != null)
					{
						ignoreRecord = true;
						if(this.m_ShapeTileArgs.UseScalar)
						{
							double currentValue = double.Parse(newRecord.Value.ToString());
							foreach(string activeValueString in this.m_ShapeTileArgs.ActiveDataValues)
							{
								double activeValue = double.Parse(activeValueString);
								if(activeValue == currentValue)
								{
									ignoreRecord = false;
									break;
								}
							}
						}
						else
						{
							string currentValue = (string)newRecord.Value;
							foreach(string activeValue in this.m_ShapeTileArgs.ActiveDataValues)
							{
								if(String.Compare(activeValue.Trim(), currentValue.Trim(), true) == 0)
								{
									ignoreRecord = false;
									break;
								}
							}
						}
					}
					else
					{
						if(this.m_ShapeTileArgs.UseScalar)
						{
							double currentValue = double.Parse(newRecord.Value.ToString());
							if(this.m_ScalarFilterMin != double.NaN)
							{
								if(currentValue < this.m_ScalarFilterMin)
								{
									ignoreRecord = true;
								}
							}
				
							if(this.m_ScalarFilterMax != double.NaN)
							{
								if(currentValue > this.m_ScalarFilterMax)
								{
									ignoreRecord = true;
								}
							}

							if(this.m_ShapeTileArgs.NoDataValues != null)
							{
								foreach(string noDataValueString in this.m_ShapeTileArgs.NoDataValues)
								{
									double noDataValue = double.Parse(noDataValueString);
									//TODO: might consider using epsilon if floating point errors occur
									if(noDataValue == currentValue)
									{
										ignoreRecord = true;
										break;
									}
								}
							}
						}
						else
						{
							string currentValue = (string)newRecord.Value;
							if(this.m_ShapeTileArgs.NoDataValues != null)
							{
								foreach(string noDataValue in this.m_ShapeTileArgs.NoDataValues)
								{
									if(String.Compare(currentValue.Trim(), noDataValue.Trim(), true) == 0)
									{
										ignoreRecord = true;
										break;
									}
								}
							}
						}
					}
				}
				
				if(!ignoreRecord)
				{
                    this.m_ShapeTileArgs.ShapeRecords.Add(newRecord);

					if(pendingPoints.Count > 0)
					{
						foreach(Vector3 v in pendingPoints) this.m_PointList.Add(v);
					}
				}

				bytesRead += 8 + contentLength * 2;
				counter++;
			}
		}



		private void loadShapeFile(string shapeFilePath)
		{								
			FileInfo shapeFile = new FileInfo(shapeFilePath);
			FileInfo dbaseFile = new FileInfo(shapeFile.FullName.Replace(".shp", ".dbf"));
			
			Random random = new Random(shapeFile.Name.GetHashCode());

			ArrayList metaValues = new ArrayList();
			
			if(this.m_ShapeTileArgs.DataKey != null && dbaseFile.Exists)
			{
				using (BinaryReader dbfReader = new BinaryReader(new BufferedStream(dbaseFile.OpenRead()), System.Text.Encoding.ASCII))
				{
					// First Read 32-byte file header
					int bytesRead = 0;
					byte dbfVersion = dbfReader.ReadByte();

					byte updateYear = dbfReader.ReadByte();
					byte updateMonth = dbfReader.ReadByte();
					byte updateDay = dbfReader.ReadByte();

					int numberRecords = dbfReader.ReadInt32();
					short headerLength = dbfReader.ReadInt16();

					short recordLength = dbfReader.ReadInt16();
					byte[] reserved = dbfReader.ReadBytes(20);

					bytesRead += 32;
					int numberFields = (headerLength - 33) / 32;

					// Read Field Descriptor Array
					DBF_Field_Header[] fieldHeaders = new DBF_Field_Header[numberFields];

					for (int i = 0; i < numberFields; i++)
					{
						char[] fieldNameChars = dbfReader.ReadChars(10);
						char fieldNameTerminator = dbfReader.ReadChar();
						string fn = new string(fieldNameChars);
						fieldHeaders[i].FieldName = fn.Trim().Replace(" ","");

						fieldHeaders[i].FieldType = dbfReader.ReadChar();
						byte[] reserved1 = dbfReader.ReadBytes(4);

						if(String.Compare(fieldHeaders[i].FieldName.Trim(), this.m_ShapeTileArgs.DataKey, true) == 0)
						{
                            this.m_ShapeTileArgs.DataKey = fieldHeaders[i].FieldName;
							
							if(fieldHeaders[i].FieldType == 'N')
							{
                                this.m_ShapeTileArgs.UseScalar = true;
							}
							else
							{
                                this.m_ShapeTileArgs.UseScalar = false;
							}

						}

						fieldHeaders[i].FieldLength = dbfReader.ReadByte();

						byte[] reserved2 = dbfReader.ReadBytes(15);
						bytesRead += 32;

					}
					byte headerTerminator = dbfReader.ReadByte();

					double scalarMin = double.MaxValue;
					double scalarMax = double.MinValue;
					for (int i = 0; i < numberRecords; i++)
					{
					//	Shapefile_Polygon curPoly = (Shapefile_Polygon)this.m_ShapeTileArgs.ShapeRecords[i];
								
						byte isValid = dbfReader.ReadByte();
						for (int j = 0; j < fieldHeaders.Length; j++)
						{
							char[] fieldDataChars = dbfReader.ReadChars(fieldHeaders[j].FieldLength);
							string fieldData = new string(fieldDataChars);

							if(fieldHeaders[j].FieldName == this.m_ShapeTileArgs.DataKey)
							{
								metaValues.Add(fieldData);

								if(fieldHeaders[j].FieldType == 'N')
								{
									try
									{
										if(this.m_ShapeTileArgs.ScaleMin == double.NaN || this.m_ShapeTileArgs.ScaleMax == double.NaN)
										{
											double data = double.Parse(fieldData);
											if(this.m_ShapeTileArgs.ScaleMin == double.NaN && data < scalarMin)
											{
												scalarMin = data;
											}

											if(this.m_ShapeTileArgs.ScaleMax == double.NaN && data > scalarMax)
											{
												scalarMax = data;
											}
										}
									}
									catch(Exception ex)
									{
										Log.Write(ex);
									}
								}
								else
								{
									if(!this.m_ShapeTileArgs.ColorAssignments.Contains(fieldData))
									{
										Color newColor = 
											Color.FromArgb(
											1 + random.Next(254),
											1 + random.Next(254),
											1 + random.Next(254));

                                        this.m_ShapeTileArgs.ColorAssignments.Add(fieldData, newColor);		
									}
								}
							}
						}

						if(this.m_ShapeTileArgs.UseScalar && this.m_ShapeTileArgs.ScaleMin == double.NaN)
						{
                            this.m_ShapeTileArgs.ScaleMin = scalarMin;
						}
						if(this.m_ShapeTileArgs.UseScalar && this.m_ShapeTileArgs.ScaleMax == double.NaN)
						{
                            this.m_ShapeTileArgs.ScaleMax = scalarMax;
						}
					}
				}
			}

			FileInfo shapeFileInfo = new FileInfo(shapeFilePath);

			using( FileStream fs = File.OpenRead(shapeFileInfo.FullName) )
			{
				using (BinaryReader reader = new BinaryReader(new BufferedStream(fs)))
				{
					//get file header info
					// Big-Endian Integer File Type
					byte[] fileTypeBytes = reader.ReadBytes(4);
					int fileType = 16 * 16 * 16 * 16 * 16 * 16 * fileTypeBytes[0] + 16 * 16 * 16 * 16 * fileTypeBytes[1] + 16 * 16 * fileTypeBytes[2] + fileTypeBytes[3];

					byte[] unused1 = reader.ReadBytes(5 * 4);

					byte[] fileLengthBytes = reader.ReadBytes(4);
					int fileLength = 16 * 16 * 16 * 16 * 16 * 16 * fileLengthBytes[0] + 16 * 16 * 16 * 16 * fileLengthBytes[1] + 16 * 16 * fileLengthBytes[2] + fileLengthBytes[3];

					int version = reader.ReadInt32();
					int shapeType = reader.ReadInt32();

                    this.m_BoundingBoxXMin = reader.ReadDouble();
                    this.m_BoundingBoxYMin = reader.ReadDouble();
                    this.m_BoundingBoxXMax = reader.ReadDouble();
                    this.m_BoundingBoxYMax = reader.ReadDouble();
                    this.m_BoundingBoxZMin = reader.ReadDouble();
                    this.m_BoundingBoxZMax = reader.ReadDouble();
                    this.m_BoundingBoxMMin = reader.ReadDouble();
                    this.m_BoundingBoxMMax = reader.ReadDouble();

					//start reading records...
					int bytesRead = 100;
					int counter = 0;

					while (bytesRead < shapeFileInfo.Length)
					{
						ArrayList pendingPoints = new ArrayList();
					
						//read record header
						byte[] recordNumberBytes = reader.ReadBytes(4);
						byte[] contentLengthBytes = reader.ReadBytes(4);

						int recordNumber = 16 * 16 * 16 * 16 * 16 * 16 * recordNumberBytes[0] + 16 * 16 * 16 * 16 * recordNumberBytes[1] + 16 * 16 * recordNumberBytes[2] + recordNumberBytes[3];
						int contentLength = 16 * 16 * 16 * 16 * 16 * 16 * contentLengthBytes[0] + 16 * 16 * 16 * 16 * contentLengthBytes[1] + 16 * 16 * contentLengthBytes[2] + contentLengthBytes[3];

						//read shape type to determine record structure and content
						int recordShapeType = reader.ReadInt32();
						
						ShapeRecord newRecord = new ShapeRecord();

						if(recordShapeType == 0) //Null shape type -- generally used as a placeholder
						{
							newRecord.Null = new Shapefile_Null();
						}
						else if(recordShapeType == 1) //Point shape type
						{
							newRecord.Point = new Shapefile_Point();
							newRecord.Point.X = reader.ReadDouble();
							newRecord.Point.Y = reader.ReadDouble();

							pendingPoints.Add(
								MathEngine.SphericalToCartesian(newRecord.Point.Y, newRecord.Point.X, this.m_ShapeTileArgs.LayerRadius));
						}
						else if(recordShapeType == 8) //Multi-point shape type
						{
							newRecord.MultiPoint = new Shapefile_MultiPoint();
							newRecord.MultiPoint.BoundingBox.West = reader.ReadDouble();
							newRecord.MultiPoint.BoundingBox.South = reader.ReadDouble();
							newRecord.MultiPoint.BoundingBox.East = reader.ReadDouble();
							newRecord.MultiPoint.BoundingBox.North = reader.ReadDouble();

							newRecord.MultiPoint.NumPoints = reader.ReadInt32();
							newRecord.MultiPoint.Points = new Shapefile_Point[newRecord.MultiPoint.NumPoints];
							for(int i = 0; i < newRecord.MultiPoint.NumPoints; i++)
							{
								newRecord.MultiPoint.Points[i] = new Shapefile_Point();
								newRecord.MultiPoint.Points[i].X = reader.ReadDouble();
								newRecord.MultiPoint.Points[i].Y = reader.ReadDouble();

								pendingPoints.Add(
									MathEngine.SphericalToCartesian(newRecord.MultiPoint.Points[i].Y, newRecord.MultiPoint.Points[i].X, this.m_ShapeTileArgs.LayerRadius));
							}
						}
						else if(recordShapeType == 3)
						{
							newRecord.PolyLine = new Shapefile_PolyLine();
						
							newRecord.PolyLine.BoundingBox.West = reader.ReadDouble();
							newRecord.PolyLine.BoundingBox.South = reader.ReadDouble();
							newRecord.PolyLine.BoundingBox.East = reader.ReadDouble();
							newRecord.PolyLine.BoundingBox.North = reader.ReadDouble();
						
							newRecord.PolyLine.NumParts = reader.ReadInt32();
							newRecord.PolyLine.NumPoints = reader.ReadInt32();
							newRecord.PolyLine.Parts = new int[newRecord.PolyLine.NumParts];

							for (int i = 0; i < newRecord.PolyLine.Parts.Length; i++)
							{
								newRecord.PolyLine.Parts[i] = reader.ReadInt32();
							}

							newRecord.PolyLine.Points = new Shapefile_Point[newRecord.PolyLine.NumPoints];
							for (int i = 0; i < newRecord.PolyLine.Points.Length; i++)
							{
								newRecord.PolyLine.Points[i] = new Shapefile_Point();
								newRecord.PolyLine.Points[i].X = reader.ReadDouble();
								newRecord.PolyLine.Points[i].Y = reader.ReadDouble();
							}
						}
						else if(recordShapeType == 5)
						{
							newRecord.Polygon = new Shapefile_Polygon();
						
							newRecord.Polygon.BoundingBox.West = reader.ReadDouble();
							newRecord.Polygon.BoundingBox.South = reader.ReadDouble();
							newRecord.Polygon.BoundingBox.East = reader.ReadDouble();
							newRecord.Polygon.BoundingBox.North = reader.ReadDouble();
						
							newRecord.Polygon.NumParts = reader.ReadInt32();
							newRecord.Polygon.NumPoints = reader.ReadInt32();
							newRecord.Polygon.Parts = new int[newRecord.Polygon.NumParts];

							for (int i = 0; i < newRecord.Polygon.Parts.Length; i++)
							{
								newRecord.Polygon.Parts[i] = reader.ReadInt32();
							}

							newRecord.Polygon.Points = new Shapefile_Point[newRecord.Polygon.NumPoints];
							for (int i = 0; i < newRecord.Polygon.Points.Length; i++)
							{
								newRecord.Polygon.Points[i] = new Shapefile_Point();
								newRecord.Polygon.Points[i].X = reader.ReadDouble();
								newRecord.Polygon.Points[i].Y = reader.ReadDouble();
							}
						}

						bool ignoreRecord = false;
							
						if(metaValues != null && metaValues.Count > 0)
						{
							newRecord.Value = metaValues[counter];
						
							if(this.m_ShapeTileArgs.ActiveDataValues != null)
							{
								ignoreRecord = true;
								if(this.m_ShapeTileArgs.UseScalar)
								{
									double currentValue = double.Parse(newRecord.Value.ToString());
									foreach(string activeValueString in this.m_ShapeTileArgs.ActiveDataValues)
									{
										double activeValue = double.Parse(activeValueString);
										if(activeValue == currentValue)
										{
											ignoreRecord = false;
											break;
										}
									}
								}
								else
								{
									string currentValue = (string)newRecord.Value;
									foreach(string activeValue in this.m_ShapeTileArgs.ActiveDataValues)
									{
										if(String.Compare(activeValue.Trim(), currentValue.Trim(), true) == 0)
										{
											ignoreRecord = false;
											break;
										}
									}
								}
							}
							else
							{
								if(this.m_ShapeTileArgs.UseScalar)
								{
									double currentValue = double.Parse(newRecord.Value.ToString());
									if(this.m_ScalarFilterMin != double.NaN)
									{
										if(currentValue < this.m_ScalarFilterMin)
										{
											ignoreRecord = true;
										}
									}
						
									if(this.m_ScalarFilterMax != double.NaN)
									{
										if(currentValue > this.m_ScalarFilterMax)
										{
											ignoreRecord = true;
										}
									}

									if(this.m_ShapeTileArgs.NoDataValues != null)
									{
										foreach(string noDataValueString in this.m_ShapeTileArgs.NoDataValues)
										{
											double noDataValue = double.Parse(noDataValueString);
											//TODO: might consider using epsilon if floating point errors occur
											if(noDataValue == currentValue)
											{
												ignoreRecord = true;
												break;
											}
										}
									}
								}
								else
								{
									string currentValue = (string)newRecord.Value;
									if(this.m_ShapeTileArgs.NoDataValues != null)
									{
										foreach(string noDataValue in this.m_ShapeTileArgs.NoDataValues)
										{
											if(String.Compare(currentValue.Trim(), noDataValue.Trim(), true) == 0)
											{
												ignoreRecord = true;
												break;
											}
										}
									}
								}
							}
						}
						
						if(!ignoreRecord)
						{
                            this.m_ShapeTileArgs.ShapeRecords.Add(newRecord);

							if(pendingPoints.Count > 0)
							{
								foreach(Vector3 v in pendingPoints) this.m_PointList.Add(v);
							}
						}

						bytesRead += 8 + contentLength * 2;
						counter++;
					}
				}
			}
		}
	}


	public class ExtendedZipInputStream 
	{
		private ZipInputStream zis;

		public ExtendedZipInputStream ( Stream baseInputStream )
		{
            this.zis=new ZipInputStream(baseInputStream);
		}
		
		public ZipEntry GetNextEntry()
		{
			return this.zis.GetNextEntry();
		}

		public byte ReadByte()
		{
			return (byte) this.zis.ReadByte();
		}

		public int ReadByteAsInt()
		{
			return this.zis.ReadByte();
		}

		public Int32 ReadInt32()
		{
			byte[]temp=new byte[4];
			for(int i=0;i<4;i++)
				temp[i]=(byte) this.zis.ReadByte();			
			return BitConverter.ToInt32(temp,0);
		}
		
		public Int16 ReadInt16()
		{
			byte[]temp=new byte[2];
			for(int i=0;i<2;i++)
				temp[i]=(byte) this.zis.ReadByte();			
			return BitConverter.ToInt16(temp,0);
		}

		public double ReadDouble()
		{
			byte[]temp=new byte[8];
			for(int i=0;i<8;i++)
				temp[i]=(byte) this.zis.ReadByte();
			return BitConverter.ToDouble(temp,0);
		}

		public byte[] ReadBytes(int count)
		{
			byte[]temp=new byte[count];
			for(int i=0;i<count;i++)
				temp[i]=(byte) this.zis.ReadByte();
			return temp;
		}
		public char ReadChar()
		{
			return (char) this.zis.ReadByte();
		}
		public char[] ReadChars(int count)
		{
			char[]temp=new char[count];			
			for(int i=0;i<count;i++)
				temp[i]=(char) this.zis.ReadByte();

			return temp;
		}

		public void Close()
		{
            this.zis.Close();
		}
	}


	public class ShapeTileArgs
	{
		public static float TileDrawDistance = 2.5f;
		public static float TileSpreadFactor = 2.0f;
		public Hashtable ColorAssignments = new Hashtable();
		public ShapeFileLayer ParentShapeFileLayer;
		public double LayerRadius;

		bool m_ScaleColors;
		bool m_ShowLabels;
		string m_DataKey;
		bool m_OutlinePolygons;
		bool m_PolygonFill;
		ShapeFillStyle m_ShapeFillStyle = ShapeFillStyle.Solid;
		Color m_PolygonColor;
		Color m_LineColor;
		Color m_LabelColor;
		bool m_UseScalar;
		float m_LineWidth = 1.0f;
		double m_ScaleMin = double.MaxValue;
		double m_ScaleMax = double.MinValue;
		bool m_IsPolyLine;
		string[] m_NoDataValues;
		string[] m_ActiveDataValues;
		ArrayList m_ShapeRecords = new ArrayList();
		World m_ParentWorld;
		Size m_TilePixelSize;

		public string[] ActiveDataValues
		{
			get
			{
				return this.m_ActiveDataValues;
			}
		}

		public bool ScaleColors
		{
			get
			{
				return this.m_ScaleColors;
			}
		}	

		public ShapeFillStyle ShapeFillStyle
		{
			get
			{
				return this.m_ShapeFillStyle;
			}
		}
		public bool IsPolyLine
		{
			get
			{
				return this.m_IsPolyLine;
			}
			set
			{
                this.m_IsPolyLine = value;
			}
		}
		public float LineWidth
		{
			get
			{
				return this.m_LineWidth;
			}
		}
		public bool OutlinePolygons
		{
			get
			{
				return this.m_OutlinePolygons;
			}
		}
		public Color PolygonColor
		{
			get
			{
				return this.m_PolygonColor;
			}
		}
		public Color LineColor
		{
			get
			{
				return this.m_LineColor;
			}
		}
		public Color LabelColor
		{
			get
			{
				return this.m_LabelColor;
			}
		}
		public string[] NoDataValues
		{
			get
			{
				return this.m_NoDataValues;
			}
		}
		public bool PolygonFill
		{
			get
			{
				return this.m_PolygonFill;
			}
		}
		public bool UseScalar
		{
			get
			{
				return this.m_UseScalar;
			}
			set
			{
                this.m_UseScalar = value;
			}
		}
		public double ScaleMin
		{
			get
			{
				return this.m_ScaleMin;
			}
			set
			{
                this.m_ScaleMin = value;
			}
		}
		public double ScaleMax
		{
			get
			{
				return this.m_ScaleMax;
			}
			set
			{
                this.m_ScaleMax = value;
			}
		}
		public string DataKey
		{
			get
			{
				return this.m_DataKey;
			}
			set
			{
                this.m_DataKey = value;
			}
		}
		public ArrayList ShapeRecords
		{
			get
			{
				return this.m_ShapeRecords;
			}
			set
			{
                this.m_ShapeRecords = value;
			}
		}
		public World ParentWorld
		{
			get
			{
				return this.m_ParentWorld;
			}
		}
		public Size TilePixelSize
		{
			get
			{
				return this.m_TilePixelSize;
			}
		}
		public bool ShowLabels
		{
			get
			{
				return this.m_ShowLabels;
			}
		}

		public ShapeTileArgs(
			World parentWorld,
			Size tilePixelSize,
			double layerRadius,
			ShapeFileLayer parentShapeLayer,
			string dataKey,
			bool scaleColors,
			double scaleMin,
			double scaleMax,
			string[] noDataValues,
			string[] activeDataValues,
			bool polygonFill,
			bool outlinePolygons,
			Color polygonColor,
			ShapeFillStyle shapeFillStyle,
			Color lineColor,
			Color labelColor,
			float lineWidth,
			bool showLabels
			)
		{
            this.ParentShapeFileLayer = parentShapeLayer;
            this.LayerRadius = layerRadius;

            this.m_ParentWorld = parentWorld;
            this.m_TilePixelSize = tilePixelSize;
            this.m_DataKey = dataKey;
            this.m_ScaleColors = scaleColors;
            this.m_PolygonFill = polygonFill;
            this.m_ScaleMin = scaleMin;
            this.m_ScaleMax = scaleMax;
            this.m_NoDataValues = noDataValues;
            this.m_ActiveDataValues = activeDataValues;
            this.m_PolygonFill = polygonFill;
            this.m_OutlinePolygons = outlinePolygons;
            this.m_PolygonColor = polygonColor;
            this.m_ShapeFillStyle = shapeFillStyle;
            this.m_LineColor = lineColor;
            this.m_LabelColor = labelColor;
            this.m_LineWidth = lineWidth;
            this.m_ShowLabels = showLabels;
		}
	}

	/// <summary>
	/// Polygon shape types can be filled with any of these styles, which are the same as the HatchStyles in the GDI .NET framework.
	/// The exception is the "Solid" style, signifies a "solid" fill style (no hatching).
	/// </summary>
	public enum ShapeFillStyle
	{
		Solid,
		BackwardDiagonal,
		Cross,
		DarkDownwardDiagonal,
		DarkHorizontal,
		DarkUpwardDiagonal,
		DarkVertical,
		DashedDownwardDiagonal,
		DashedHorizontal,
		DashedUpwardDiagonal,
		DashedVertical,
		DiagonalBrick,
		DiagonalCross,
		Divot,
		DottedDiamond,
		DottedGrid,
		ForwardDiagonal,
		Horizontal,
		LargeCheckerBoard,
		LargeConfetti,
		LargeGrid,
		LightDownwardDiagonal,
		LightHorizontal,
		LightUpwardDiagonal,
		LightVertical,
		Max,
		Min,
		NarrowHorizontal,
		NarrowVertical,
		OutlinedDiamond,
		Percent05,
		Percent10,
		Percent20,
		Percent25,
		Percent30,
		Percent40,
		Percent50,
		Percent60,
		Percent70,
		Percent75,
		Percent80,
		Percent90,
		Plaid,
		Shingle,
		SmallCheckerBoard,
		SmallConfetti,
		SmallGrid,
		SolidDiamond,
		Sphere,
		Trellis,
		Wave,
		Weave,
		WideDownwardDiagonal,
		WideUpwardDiagonal,
		ZigZag
	}

	public class ShapeTile
	{
		bool m_Initialized;
		bool m_Initializing;
		ShapeTileArgs m_ShapeTileArgs;
		bool m_Disposing;
	//	SurfaceImage m_NwSurfaceImage;
	//	SurfaceImage m_NeSurfaceImage;
	//	SurfaceImage m_SwSurfaceImage;
	//	SurfaceImage m_SeSurfaceImage;
		ImageLayer m_NwImageLayer;
		ImageLayer m_NeImageLayer;
		ImageLayer m_SwImageLayer;
		ImageLayer m_SeImageLayer;

		public GeographicBoundingBox m_GeoBB;

		public BoundingBox BoundingBox;
		

		public ShapeTile(
			GeographicBoundingBox geoBB,
			ShapeTileArgs shapeTileArgs
			)
		{
            this.m_GeoBB = geoBB;
            this.m_ShapeTileArgs = shapeTileArgs;

            this.BoundingBox = new BoundingBox( (float)geoBB.South, (float)geoBB.North, (float)geoBB.West, (float)geoBB.East, 
				(float) this.m_ShapeTileArgs.LayerRadius, (float) this.m_ShapeTileArgs.LayerRadius + 300000f);
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

		private bool isShapeRecordInBounds(GeographicBoundingBox geoBB, ShapeRecord record)
		{
			if(record.Point != null)
			{
				if(record.Point.X < geoBB.West || record.Point.X > geoBB.East ||
					record.Point.Y > geoBB.North || record.Point.Y < geoBB.South)
				{
					return false;
				}
				else
				{	
					return true;
				}
			}
			else if(record.MultiPoint != null)
			{
				if(record.MultiPoint.BoundingBox.North <= geoBB.South ||
					record.MultiPoint.BoundingBox.South >= geoBB.North ||
					record.MultiPoint.BoundingBox.West >= geoBB.East ||
					record.MultiPoint.BoundingBox.East <= geoBB.West)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else if(record.PolyLine != null)
			{
				if(record.PolyLine.BoundingBox.North <= geoBB.South ||
					record.PolyLine.BoundingBox.South >= geoBB.North ||
					record.PolyLine.BoundingBox.West >= geoBB.East ||
					record.PolyLine.BoundingBox.East <= geoBB.West)
				{
					return false;
				}
				else
				{
					return true;
				}	
			}
			else if(record.Polygon != null)
			{
				if(record.Polygon.BoundingBox.North <= geoBB.South ||
					record.Polygon.BoundingBox.South >= geoBB.North ||
					record.Polygon.BoundingBox.West >= geoBB.East ||
					record.Polygon.BoundingBox.East <= geoBB.West)
				{
					return false;
				}
				else
				{
					return true;
				}	
			}

			return false;
		}


		public byte Opacity
		{
			get
			{
				return this.m_ShapeTileArgs.ParentShapeFileLayer.Opacity;
			}
			set
			{
				if(this.m_NwImageLayer != null)
				{
                    this.m_NwImageLayer.Opacity = value;
				}
				if(this.m_NeImageLayer != null)
				{
                    this.m_NeImageLayer.Opacity = value;
				}
				if(this.m_SwImageLayer != null)
				{
                    this.m_SwImageLayer.Opacity = value;
				}
				if(this.m_SeImageLayer != null)
				{
                    this.m_SeImageLayer.Opacity = value;
				}

				if(this.m_NorthWestChild != null)
				{
                    this.m_NorthWestChild.Opacity = value;
				}
				if(this.m_NorthEastChild != null)
				{
                    this.m_NorthEastChild.Opacity = value;
				}
				if(this.m_SouthWestChild != null)
				{
                    this.m_SouthWestChild.Opacity = value;
				}
				if(this.m_SouthEastChild != null)
				{
                    this.m_SouthEastChild.Opacity = value;
				}
			}
		}

		private ImageLayer CreateImageLayer(double north, double south, double west, double east, DrawArgs drawArgs)
		{
			Bitmap b = null;
			Graphics g = null;
			ImageLayer imageLayer = null;
			GeographicBoundingBox geoBB = new GeographicBoundingBox(north, south, west, east);
	
			int numberPolygonsInTile = 0;
			for(int i = 0; i < this.m_ShapeTileArgs.ShapeRecords.Count; i++)
			{
				ShapeRecord currentRecord = (ShapeRecord) this.m_ShapeTileArgs.ShapeRecords[i];
				
				if(currentRecord.Null != null || 
					currentRecord.Point != null || 
					currentRecord.MultiPoint != null ||
					!this.isShapeRecordInBounds(geoBB, currentRecord))
				{
					continue;
				}
				else
				{
					if(b == null)
					{
						b = new Bitmap(this.m_ShapeTileArgs.TilePixelSize.Width, this.m_ShapeTileArgs.TilePixelSize.Height,
							System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					}

					if(g == null)
					{
						g = Graphics.FromImage(b);
					}

					Color color = this.m_ShapeTileArgs.PolygonColor;

					//Fix Black Tiles
					g.DrawLine(new Pen(color),0,0,1,1);
					
					
					if(this.m_ShapeTileArgs.UseScalar && this.m_ShapeTileArgs.ScaleColors)
					{
						double red = 1.0;
						double green = 1.0;
						double blue = 1.0;
							
						try
						{
							//TODO: make this a function and abstract to allow multiple gradient mappings
							double dv;

							double curScalar = double.Parse(currentRecord.Value.ToString());

							if (curScalar < this.m_ShapeTileArgs.ScaleMin)
								curScalar = this.m_ShapeTileArgs.ScaleMin;
							if (curScalar > this.m_ShapeTileArgs.ScaleMax)
								curScalar = this.m_ShapeTileArgs.ScaleMax;
							
							dv = this.m_ShapeTileArgs.ScaleMax - this.m_ShapeTileArgs.ScaleMin;

							if (curScalar < (this.m_ShapeTileArgs.ScaleMin + 0.25 * dv)) 
							{
								red = 0;
								green = 4 * (curScalar - this.m_ShapeTileArgs.ScaleMin) / dv;
							} 
							else if (curScalar < (this.m_ShapeTileArgs.ScaleMin + 0.5 * dv)) 
							{
								red = 0;
								blue = 1 + 4 * (this.m_ShapeTileArgs.ScaleMin + 0.25 * dv - curScalar) / dv;
							} 
							else if (curScalar < (this.m_ShapeTileArgs.ScaleMin + 0.75 * dv)) 
							{
								red = 4 * (curScalar - this.m_ShapeTileArgs.ScaleMin - 0.5 * dv) / dv;
								blue = 0;
							} 
							else 
							{
								green = 1 + 4 * (this.m_ShapeTileArgs.ScaleMin + 0.75 * dv - curScalar) / dv;
								blue = 0;
							}

							color = Color.FromArgb((int)(255*red), (int)(255*green), (int)(255*blue));
							
						}
						catch(Exception)
						{
						//	Log.Write((string)currentPoly.ScalarHash[m_ShapeTileArgs.ColorKey]);
						//	Log.Write(String.Format("Min: {0}, Max: {1}", m_ShapeTileArgs.ScaleMin, m_ShapeTileArgs.ScaleMax));
						//	Log.Write(String.Format("{0},{1},{2}", red, green, blue));
						//	Log.Write(ex);
						}
					}
					else
					{
						if(this.m_ShapeTileArgs.ColorAssignments.Count > 0 && this.m_ShapeTileArgs.ScaleColors)
						{
							try
							{
								string colorAssignmentKey = (string)currentRecord.Value;
								foreach(string cak in this.m_ShapeTileArgs.ColorAssignments.Keys)
								{
									if(String.Compare(cak, colorAssignmentKey, true) == 0)
									{
										color = (Color) this.m_ShapeTileArgs.ColorAssignments[cak];
										break;
									}
								}
							}
							catch(Exception)
							{
							}
						}
					}
					
					if(currentRecord.Polygon != null)
					{
                        this.drawPolygon(currentRecord.Polygon,
							g,
							color,
							geoBB,
							b.Size);
					}
					
					if(this.m_ShapeTileArgs.ColorAssignments.Count == 0 ||  
                       !this.m_ShapeTileArgs.ScaleColors)
					{
						color = this.m_ShapeTileArgs.LineColor;
					}

					if(currentRecord.PolyLine != null)
					{
                        this.drawPolyLine(currentRecord.PolyLine,
							g,
							color,
							geoBB,
							b.Size);
					}
					numberPolygonsInTile++;
				}
				
			}
            
			if(numberPolygonsInTile > 0)
			{
				string id = DateTime.Now.Ticks.ToString();

				MemoryStream ms = new MemoryStream();
				b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				//b.Save("Shapefiles\\imagecache\\"+north+""+south+""+east+""+west+ ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
				
				//must copy original stream into new stream, if not, error occurs, not sure why
                this.m_ImageStream = new MemoryStream(ms.GetBuffer());

			//	Texture texture = TextureLoader.FromStream(
			//		drawArgs.device, mss, 0, 0, 1,
			//		Usage.None, World.Settings.TextureFormat, Pool.Managed, Filter.Box, Filter.Box, System.Drawing.Color.Black.ToArgb());

			
				imageLayer = new ImageLayer(
					id, this.m_ShapeTileArgs.ParentWorld,
					0,// should be distance above surface
                    this.m_ImageStream,
					Color.Black.ToArgb(),
					(float)south,
					(float)north,
					(float)west,
					(float)east,
					(float) this.m_ShapeTileArgs.ParentShapeFileLayer.Opacity / 255.0f, this.m_ShapeTileArgs.ParentWorld.TerrainAccessor);
					
				
				//mss.Close();
				ms.Close();
			}
					
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

				double centerLatitude = 0.5 * (this.m_GeoBB.North + this.m_GeoBB.South);
				double centerLongitude = 0.5 * (this.m_GeoBB.West + this.m_GeoBB.East);

                this.m_NwImageLayer = this.CreateImageLayer(this.m_GeoBB.North, centerLatitude, this.m_GeoBB.West, centerLongitude, drawArgs);
                this.m_NeImageLayer = this.CreateImageLayer(this.m_GeoBB.North, centerLatitude, centerLongitude, this.m_GeoBB.East, drawArgs);
                this.m_SwImageLayer = this.CreateImageLayer(centerLatitude, this.m_GeoBB.South, this.m_GeoBB.West, centerLongitude, drawArgs);
                this.m_SeImageLayer = this.CreateImageLayer(centerLatitude, this.m_GeoBB.South, centerLongitude, this.m_GeoBB.East, drawArgs);

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

		private void drawPolyLine(Shapefile_PolyLine polyLine, Graphics g, Color c, GeographicBoundingBox dstBB, Size imageSize)
		{
			using(Pen p = new Pen(c, this.m_ShapeTileArgs.LineWidth))
			{
				p.Color = c;
				if(polyLine.Parts.Length > 1)
				{
					for(int partsItr = 0; partsItr < polyLine.Parts.Length - 1; partsItr++)
					{
						g.DrawLines(p, this.getScreenPoints(polyLine.Points, polyLine.Parts[partsItr], polyLine.Parts[partsItr + 1] - polyLine.Parts[partsItr], dstBB, imageSize));

					}

					g.DrawLines(p, this.getScreenPoints(polyLine.Points, polyLine.Parts[polyLine.Parts.Length - 1],
						polyLine.Points.Length - polyLine.Parts[polyLine.Parts.Length - 1], dstBB, imageSize));
				}
				else
				{
					g.DrawLines(p, this.getScreenPoints(polyLine.Points, 0, polyLine.Points.Length, dstBB, imageSize));
				}
			}
		}

		private void drawPolygon(Shapefile_Polygon polygon, Graphics g, Color c, GeographicBoundingBox dstBB, Size imageSize)
		{
			if(polygon.Parts.Length > 1)
			{
				if(this.m_ShapeTileArgs.PolygonFill)
				{
					if(this.m_ShapeTileArgs.ShapeFillStyle == ShapeFillStyle.Solid)
					{
						using(SolidBrush brush = new SolidBrush(c))
						{
							g.FillPolygon(brush, this.getScreenPoints(polygon.Points, 0, polygon.Parts[1], dstBB, imageSize));
						}
					}
					else
					{
						using(System.Drawing.Drawing2D.HatchBrush brush = new System.Drawing.Drawing2D.HatchBrush(
								  getGDIHatchStyle(this.m_ShapeTileArgs.ShapeFillStyle),
								  c,
								  Color.Black))
						{
							g.FillPolygon(brush, this.getScreenPoints(polygon.Points, 0, polygon.Parts[1], dstBB, imageSize));
						}
					}
				}
				
				if(this.m_ShapeTileArgs.OutlinePolygons)
				{
					using(Pen p = new Pen(this.m_ShapeTileArgs.LineColor, this.m_ShapeTileArgs.LineWidth))
					{
						for(int partsItr = 0; partsItr < polygon.Parts.Length - 1; partsItr++)
						{
							g.DrawPolygon(p, this.getScreenPoints(polygon.Points, polygon.Parts[partsItr], polygon.Parts[partsItr+1] - polygon.Parts[partsItr], dstBB, imageSize));
						}

						g.DrawPolygon(p, this.getScreenPoints(polygon.Points, polygon.Parts[polygon.Parts.Length - 1],
							polygon.Points.Length - polygon.Parts[polygon.Parts.Length - 1], dstBB, imageSize)
							);
					}
				}

				if(this.m_ShapeTileArgs.PolygonFill)
				{
					if(this.m_ShapeTileArgs.ShapeFillStyle == ShapeFillStyle.Solid)
					{
						using(SolidBrush brush = new SolidBrush(Color.Black))
						{
							for(int partsItr = 1; partsItr < polygon.Parts.Length - 1; partsItr++)
							{
								g.FillPolygon(brush, this.getScreenPoints(polygon.Points, polygon.Parts[partsItr], polygon.Parts[partsItr+1] - polygon.Parts[partsItr], dstBB, imageSize)
									);
							}

							g.FillPolygon(brush, this.getScreenPoints(polygon.Points, polygon.Parts[polygon.Parts.Length - 1],
								polygon.Points.Length - polygon.Parts[polygon.Parts.Length - 1], dstBB, imageSize)
								);
						}
					}
					else
					{
						using(System.Drawing.Drawing2D.HatchBrush brush = new System.Drawing.Drawing2D.HatchBrush(
								  getGDIHatchStyle(this.m_ShapeTileArgs.ShapeFillStyle),
								  c,
								  Color.Black))
						{
							for(int partsItr = 1; partsItr < polygon.Parts.Length - 1; partsItr++)
							{
								g.FillPolygon(brush, this.getScreenPoints(polygon.Points, polygon.Parts[partsItr], polygon.Parts[partsItr+1] - polygon.Parts[partsItr], dstBB, imageSize)
									);
							}

							g.FillPolygon(brush, this.getScreenPoints(polygon.Points, polygon.Parts[polygon.Parts.Length - 1],
								polygon.Points.Length - polygon.Parts[polygon.Parts.Length - 1], dstBB, imageSize)
								);
						}
					}
				}				
			}
			else
			{
				if(this.m_ShapeTileArgs.PolygonFill)
				{
					if(this.m_ShapeTileArgs.ShapeFillStyle == ShapeFillStyle.Solid)
					{
						using(SolidBrush brush = new SolidBrush(c))
						{
							g.FillPolygon(brush, this.getScreenPoints(polygon.Points, 0, polygon.Points.Length, dstBB, imageSize));
						}
					}
					else
					{
						using(System.Drawing.Drawing2D.HatchBrush brush = new System.Drawing.Drawing2D.HatchBrush(
								  getGDIHatchStyle(this.m_ShapeTileArgs.ShapeFillStyle),
								  c,
								  Color.Black))
						{
							g.FillPolygon(brush, this.getScreenPoints(polygon.Points, 0, polygon.Points.Length, dstBB, imageSize));
						}
					}
				}

				
				if(this.m_ShapeTileArgs.OutlinePolygons)
				{
					using(Pen p = new Pen(this.m_ShapeTileArgs.LineColor, this.m_ShapeTileArgs.LineWidth))
					{
						g.DrawPolygon(p, this.getScreenPoints(polygon.Points, 0, polygon.Points.Length, dstBB, imageSize));
					}
				}
			}
		}

		private Point[] getScreenPoints(Shapefile_Point[] sourcePoints, int offset, int length, GeographicBoundingBox dstBB, Size dstImageSize)
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
		
		private ShapeTile ComputeChild( DrawArgs drawArgs, double childSouth, double childNorth, double childWest, double childEast, double tileSize ) 
		{
			ShapeTile child = new ShapeTile(
				new GeographicBoundingBox(childNorth, childSouth, childWest, childEast), this.m_ShapeTileArgs);

			return child;
		}

		ShapeTile m_NorthWestChild;
		ShapeTile m_NorthEastChild;
		ShapeTile m_SouthWestChild;
		ShapeTile m_SouthEastChild;

		public virtual void ComputeChildren(DrawArgs drawArgs)
		{
			
			float tileSize = (float)(0.5*(this.m_GeoBB.North - this.m_GeoBB.South));
			//TODO: Stop children computation at some lower level
			if(tileSize>0.0001)
			{

				double CenterLat = 0.5f*(this.m_GeoBB.North + this.m_GeoBB.South);
				double CenterLon = 0.5f*(this.m_GeoBB.East + this.m_GeoBB.West);
			
				if(this.m_NorthWestChild == null && this.m_NwImageLayer != null && this.m_Initialized)
				{
                    this.m_NorthWestChild = this.ComputeChild(drawArgs, CenterLat, this.m_GeoBB.North, this.m_GeoBB.West, CenterLon, tileSize );
                    this.m_NorthWestChild.Initialize(drawArgs);
				}

				if(this.m_NorthEastChild == null && this.m_NeImageLayer != null && this.m_Initialized)
				{
                    this.m_NorthEastChild = this.ComputeChild(drawArgs, CenterLat, this.m_GeoBB.North, CenterLon, this.m_GeoBB.East, tileSize );
                    this.m_NorthEastChild.Initialize(drawArgs);
				}

				if(this.m_SouthWestChild == null && this.m_SwImageLayer != null && this.m_Initialized)
				{
                    this.m_SouthWestChild = this.ComputeChild(drawArgs, this.m_GeoBB.South, CenterLat, this.m_GeoBB.West, CenterLon, tileSize );
                    this.m_SouthWestChild.Initialize(drawArgs);
				}

				if(this.m_SouthEastChild == null && this.m_SeImageLayer != null && this.m_Initialized)
				{
                    this.m_SouthEastChild = this.ComputeChild(drawArgs, this.m_GeoBB.South, CenterLat, CenterLon, this.m_GeoBB.East, tileSize );
                    this.m_SouthEastChild.Initialize(drawArgs);
				}
			}
		}

		public void Update(DrawArgs drawArgs)
		{
			try
			{
				double centerLatitude = 0.5 * (this.m_GeoBB.North + this.m_GeoBB.South);
				double centerLongitude = 0.5 * (this.m_GeoBB.West + this.m_GeoBB.East);
				double tileSize = this.m_GeoBB.North - this.m_GeoBB.South;

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
						//if(this.level != 0)
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
	}

	public class Shapefile_Polygon
	{
		public GeographicBoundingBox BoundingBox = new GeographicBoundingBox();
		public int NumParts;
		public int NumPoints;
		public int[] Parts;
		public Shapefile_Point[] Points;
	}

	public class Shapefile_PolyLine
	{
		public GeographicBoundingBox BoundingBox = new GeographicBoundingBox();
		public int NumParts;
		public int NumPoints;
		public int[] Parts;
		public Shapefile_Point[] Points;
	}

	public class Shapefile_Null
	{
		
	}

	public class Shapefile_Point
	{
		public double X;
		public double Y;
		public object Tag;
	}

	public class Shapefile_MultiPoint
	{
		public GeographicBoundingBox BoundingBox = new GeographicBoundingBox();
		public int NumPoints;
		public Shapefile_Point[] Points;
	}

	struct DBF_Field_Header
	{
		public string FieldName;
		public char FieldType;
		public byte FieldLength;
	}

	class ShapeRecord
	{
		#region Private Members
		string m_Id;
		Shapefile_Null m_Null;
		Shapefile_Point m_Point;
		Shapefile_MultiPoint m_MultiPoint;
		Shapefile_PolyLine m_PolyLine;
		Shapefile_Polygon m_Polygon;
		object m_Value;
		#endregion

		#region Properties
		public string ID
		{
			get
			{
				return this.m_Id;
			}
			set
			{
                this.m_Id = value;
			}
		}
		public Shapefile_Null Null
		{
			get
			{
				return this.m_Null;
			}
			set
			{
                this.m_Null = value;
			}
		}
		public Shapefile_Point Point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
                this.m_Point = value;
			}
		}
		public Shapefile_MultiPoint MultiPoint
		{
			get
			{
				return this.m_MultiPoint;
			}
			set
			{
                this.m_MultiPoint = value;
			}
		}
		public Shapefile_PolyLine PolyLine
		{
			get
			{
				return this.m_PolyLine;
			}
			set
			{
                this.m_PolyLine = value;
			}
		}

		public Shapefile_Polygon Polygon
		{
			get
			{
				return this.m_Polygon;
			}
			set
			{
                this.m_Polygon = value;
			}
		}

		public object Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
                this.m_Value = value;
			}
		}
		
		#endregion

	}
}
