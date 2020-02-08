//========================= (UNCLASSIFIED) ==============================
// Copyright © 2007 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2007 The Johns Hopkins University. 
//
// This software was developed at The Johns Hopkins University/Applied 
// Physics Laboratory (“JHU/APL”) that is the author thereof under the 
// “work made for hire” provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the “Software”), to use the 
// Software without restriction, including without limitation the rights 
// to copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit others to do so, subject to the 
// following conditions: 
//
// 1.  This LICENSE AND DISCLAIMER, including the copyright notice, shall 
//     be included in all copies of the Software, including copies of 
//     substantial portions of the Software; 
//
// 2.  JHU/APL assumes no obligation to provide support of any kind with 
//     regard to the Software.  This includes no obligation to provide 
//     assistance in using the Software nor to provide updated versions of 
//     the Software; and 
//
// 3.  THE SOFTWARE AND ITS DOCUMENTATION ARE PROVIDED AS IS AND WITHOUT 
//     ANY EXPRESS OR IMPLIED WARRANTIES WHATSOEVER.  ALL WARRANTIES 
//     INCLUDING, BUT NOT LIMITED TO, PERFORMANCE, MERCHANTABILITY, FITNESS
//     FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT ARE HEREBY DISCLAIMED.  
//     USERS ASSUME THE ENTIRE RISK AND LIABILITY OF USING THE SOFTWARE.  
//     USERS ARE ADVISED TO TEST THE SOFTWARE THOROUGHLY BEFORE RELYING ON 
//     IT.  IN NO EVENT SHALL THE JOHNS HOPKINS UNIVERSITY BE LIABLE FOR 
//     ANY DAMAGES WHATSOEVER, INCLUDING, WITHOUT LIMITATION, ANY LOST 
//     PROFITS, LOST SAVINGS OR OTHER INCIDENTAL OR CONSEQUENTIAL DAMAGES, 
//     ARISING OUT OF THE USE OR INABILITY TO USE THE SOFTWARE. 
//
using System;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Xml;
using System.ComponentModel;
using System.Text;

using Microsoft.DirectX;
using SharpDX.Direct3D;

using WorldWind;
using WorldWind.Menu;
using WorldWind.Renderable;

using Collab.jhuapl.Util;
using Icon = WorldWind.Icon;

namespace Collab.jhuapl.Whiteboard
{
	/// <summary>
	/// CS DrawLayer implements whiteboarding capability.  Users can draw basic shapes and
	/// overlay them as terrain mapped paths.
	/// </summary>
	public class DrawLayer : RenderableObjectList, IMenu
	{
		/// <summary>
		/// The current drawmode.
		/// None = Draw nothing.
		/// Hotspot = Adds an icon where you left mouse click.
		/// PostIt = Adds a text block to where you left mouse click.
		/// Polyline = Add a point per left mouse click. Right mouse to stop.		
		/// Polygon = Add a point per left mouse click. Right mouse to stop.  Last segment to close polygon automatically added.
		/// Freehand = Add points starting on mouse down until mouse up.'
		/// Disabled = This draw layer is read-only.
		/// </summary>
		public enum DrawMode
		{
			None,
			Hotspot,
			PostIt,
			Polyline,
			Polygon,
			Freehand,
			Incident,
			Disabled,
			Delete
		}
		
		// Where the mouse down even occured.  If this is different than mouse up then dragging occured.
		Point m_mouseDownPoint;

		// The draw color
		Color m_color = Color.Red;

		// A unique to this draw layer path id.  Simply increments.
		int m_lastPathId;

		// The altitude to draw the shapes and objects = terrain height + default height
		float m_drawAlt;

		// The standard height to draw.
		float m_height = (float) 0.0;

		/// <summary>
		/// The path being built
		/// </summary>
		TerrainPath m_currPathLine;

		/// <summary>
		/// The hotspot built;
		/// </summary>
		Hotspot m_hotspot;

		Icons m_hotspotLayer;

		HotspotForm m_hotspotDialog;

		/// <summary>
		/// A guideline to show where the line segment would be drawn if you left click
		/// </summary>
		CustomVertex.PositionColored[] m_guideLine = new CustomVertex.PositionColored[17];

		/// <summary>
		/// The current drawing mode.  Set to the desired shape when starting and reset 
		/// to none when complete.
		/// </summary>
		private DrawMode m_currDrawMode = DrawMode.None;

		/// <summary>
		/// Latitude of first position
		/// </summary>
		Angle m_firstLat;

		/// <summary>
		/// Longitude of first position
		/// </summary>
		Angle m_firstLon;

		/// <summary>
		/// Altitude of first position
		/// </summary>
		double m_firstAlt;

		/// <summary>
		/// Latitude of last position
		/// </summary>
		Angle m_lastLat;

		/// <summary>
		/// Longitude of last position
		/// </summary>
		Angle m_lastLon;

		Point m_lastMousePoint;

		bool m_drawModeChange;

		#region Properties

		/// <summary>
		/// Determines if the draw layer is writeable.  True = writeable, False = Read only.
		/// </summary>
		public bool Writeable
		{
			get
			{
				return this.m_writeable;
			}
			set
			{
				if (this.m_writeable != value)
				{
                    this.m_writeable = value;
					if (this.m_writeable)
					{
                        this.DrawingMode = DrawMode.None;
                        this.DrawLock = false;
					}
					else
					{
                        this.DrawingMode = DrawMode.Disabled;
                        this.DrawLock = false;
					};
				}
			}
		}
        bool m_writeable = true;


		/// <summary>
		/// Determines if the draw layer stays in a draw mode when the current shape is complete.
		/// True = keep drawing, False = go back to the none state when current drawing complete.
		/// </summary>
		public bool DrawLock
		{
			get
			{
				return this.m_drawLock;
			}
			set
			{
                this.m_drawLock = value;
			}
		}
        bool m_drawLock;

        /// <summary>
        /// The specified drawing mode.  Used when locks is enabled to reset the current
        /// draw mode for the next shape.
        /// </summary>
		public DrawMode DrawingMode
		{
			get
			{
				return this.m_drawMode;
			}
			set
			{
				// If we're changing the drawing mode
				if (this.m_drawMode != value)
				{
					switch (value)
					{
						case DrawMode.None:
                            this.StopDrawing();
							break;
						case DrawMode.Hotspot:
                            this.DrawHotspot();
							break;
						case DrawMode.Incident:
                            this.DrawIncident();
							break;
						case DrawMode.PostIt:
                            this.DrawPostIt();
							break;
						case DrawMode.Polyline:
                            this.DrawPolyline();
							break;
						case DrawMode.Polygon:
                            this.DrawPolygon();
							break;
						case DrawMode.Freehand:
                            this.DrawFreehand();
							break;
						case DrawMode.Disabled:
                            this.StopDrawing();
                            this.Writeable = false;
							break;
						default:
							break;
					}
				}
			}
		}
        private DrawMode m_drawMode = DrawMode.None;

        /// <summary>
        /// Whther or not the mouse is currently in drag mode for drawlayers
        /// </summary>
        public bool MouseDragMode
        {
            get { return m_mouseDragMode; }
            set { m_mouseDragMode = value; }
        }
        protected static bool m_mouseDragMode;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of this drawing layer</param>
		public DrawLayer(string name) : base (name)
		{
			// initialize colors
			for(int i = 0; i < this.m_guideLine.Length; i++) this.m_guideLine[i].Color = World.Settings.MeasureLineLinearColorXml;

            this.m_hotspotDialog = new HotspotForm();
            this.m_hotspotDialog.Hide();

            this.m_world = DrawArgs.CurrentWorldStatic;
		}
	
		public override void Dispose()
		{
			this.isInitialized = false;
		}
	
		#region Drawing Methods

		/// <summary>
		/// Draw a line from the last point to current position
		/// </summary>
		void BuildGuideLine( Angle lat, Angle lon )
		{
			Angle angularDistance = World.ApproxAngularDistance(this.m_lastLat, this.m_lastLon, lat, lon );

			for(int i=0; i< this.m_guideLine.Length; i++)
			{
				float t = (float)i / (this.m_guideLine.Length-1);
				Vector3 cart = DrawArgs.CurrentWorldStatic.IntermediateGCPoint(t, this.m_lastLat, this.m_lastLon, lat, lon,
					angularDistance );

                this.m_guideLine[i].X = cart.X;
                this.m_guideLine[i].Y = cart.Y;
                this.m_guideLine[i].Z = cart.Z;
			}
		}

		/// <summary>
		/// Stops any drawing in progress and reset the drawmode to none.
		/// </summary>
		public void StopDrawing()
		{
			switch (this.m_currDrawMode)
			{
				// if we are in hotspot mode
				case DrawMode.Hotspot:
				{
                    this.stopHotspot();
					break;
				}
					
				// if we are in incident mode
				case DrawMode.Incident:
				{
                    this.stopIncident();
					break;
				}
				// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
                    this.stopPoly();
					break;
				}

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
                    this.stopFreehand();
					break;
				}
			}

            this.m_drawMode = DrawMode.None;
            this.m_currDrawMode = DrawMode.None;
		}

		/// <summary>
		/// Stops any current drawing in progress and sets the current draw mode to none.  This
		/// allows the next mouse down event to start a new drawing using the desired draw mode.
		/// </summary>
		public void PauseDrawing()
		{
			switch (this.m_currDrawMode)
			{
				// if we are in hotspot mode
				case DrawMode.Hotspot:
				{
                    this.stopHotspot();
					break;
				}				
					
				// if we are in incident mode
				case DrawMode.Incident:
				{
                    this.stopIncident();
					break;
				}

				// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
                    this.stopPoly();
					break;
				}

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
                    this.stopFreehand();
					break;
				}
			}

            this.m_currDrawMode = DrawMode.None;
		}

		/// <summary>
		/// Sets up drawing of a hotspot (icon) on the layer.  Pops up a dialog asking for 
		/// hotspot bitmap, name, description and URL.
		/// </summary>
		public void DrawHotspot()
		{
            this.StopDrawing();
            this.m_drawMode = DrawMode.Hotspot;
		}

		/// <summary>
		/// Sets up drawing of a incident (icon) on the layer.  Pops up a dialog asking for 
		/// incident bitmap, name, description and URL.
		/// </summary>
		public void DrawIncident()
		{
            this.StopDrawing();
            this.m_drawMode = DrawMode.Incident;
		}

		/// <summary>
		/// Sets up drawing of a text box on the layer.  Pops up a dialog asking for text to 
		/// insert.
		/// </summary>
		public void DrawPostIt()
		{
            this.StopDrawing();
            this.m_drawMode = DrawMode.PostIt;
		}

		/// <summary>
		/// Sets up drawing of a polyline.
		/// </summary>
		public void DrawPolyline()
		{
            this.StopDrawing();
            this.m_drawMode = DrawMode.Polyline;
		}

		/// <summary>
		/// Sets up drawing of a polygon
		/// </summary>
		public void DrawPolygon()
		{
            this.StopDrawing();
            this.m_drawMode = DrawMode.Polygon;
		}

		/// <summary>
		/// Sets up drawing of a freehand shape.
		/// </summary>
		public void DrawFreehand()
		{
            this.StopDrawing();
            this.m_drawMode = DrawMode.Freehand;
		}

		/// <summary>
		/// Stops the drawing of a polyline or polygon by completing the WW path object.
		/// </summary>
		/// <returns>Success or failure of building the final path object.</returns>
		private bool stopPoly()
		{					
			Logger.Write(2, "DRAW", this.m_currPathLine.Name, "Stop Poly" + this.m_currPathLine.Name);

			// if we are a polygon add the starting point as the last point
			if (this.m_currDrawMode == DrawMode.Polygon)
			{
				// add intermediate points if distance is great
				Angle dist = World.ApproxAngularDistance(this.m_lastLat, this.m_lastLon, this.m_firstLat, this.m_firstLon);
				if (dist.Degrees > 10)
				{
					for(int i=0; i < 10; i++)
					{
						float t = (float)i / 9;
						Angle iLat, iLon;

						World.IntermediateGCPoint(t, this.m_lastLat, this.m_lastLon, this.m_firstLat, this.m_firstLon,
							dist, out iLat, out iLon );

                        this.m_currPathLine.Add((float)iLat.Degrees, (float) iLon.Degrees, this.m_drawAlt);
					}
				}

                this.m_currPathLine.Add((float) this.m_firstLat.Degrees, (float) this.m_firstLon.Degrees, this.m_drawAlt);
				Logger.Write(3, "DRAW", this.m_firstLat.Degrees, this.m_firstLon.Degrees, this.m_drawAlt, this.m_currPathLine.Name, "Adding Point to path " + this.m_currPathLine.Name);
			}

			// copy to a LineFeature
            Point3d[] points = new Point3d[this.m_currPathLine.SphericalCoordinatesList.Count];

			for (int i = 0; i < this.m_currPathLine.SphericalCoordinatesList.Count; i++)
			{
                Vector3 outCoord = this.m_currPathLine.SphericalCoordinatesList[i];

				points[i] = new Point3d(outCoord.Y, outCoord.X, outCoord.Z);
			}

			// TODO fix display altitude
			LineFeature path = new LineFeature(this.m_currPathLine.Name, DrawArgs.CurrentWorldStatic, points, this.m_color);
			path.LineWidth = (float) 3.0;

			// delete current path
            this.Remove(this.m_currPathLine.Name);

			// add the new path
            this.Add(path);

			Debug.WriteLine(path.Name + ": " + path.NumPoints);

            this.SendPath(this.m_currDrawMode, path);

			// clear out positions
            this.m_lastMousePoint = Point.Empty;
            this.m_mouseDownPoint = Point.Empty;

			// set current drawmode off
            this.m_currDrawMode = DrawMode.None;
            this.MouseDragMode = true;

			return true;
		}

		/// <summary>
		/// Stops the drawing of a freehand shape.
		/// </summary>
		/// <returns>Success or failure of building the final path object.</returns>
		private bool stopFreehand()
		{
			// copy to a LineFeature
            Point3d[] points = new Point3d[this.m_currPathLine.SphericalCoordinatesList.Count];

            for (int i = 0; i < this.m_currPathLine.SphericalCoordinatesList.Count; i++)
			{
				Vector3 outCoord = this.m_currPathLine.SphericalCoordinatesList[i];

				points[i] = new Point3d(outCoord.Y, outCoord.X, outCoord.Z);
			}

			// TODO fix display altitude
			LineFeature path = new LineFeature(this.m_currPathLine.Name, DrawArgs.CurrentWorldStatic, points, this.m_color);
			path.LineWidth = (float) 3.0;
			path.MaximumDisplayAltitude = double.MaxValue;
			path.MinimumDisplayAltitude = 0;

			// delete current path
            this.Remove(this.m_currPathLine.Name);

			// add the new path
            this.Add(path);

            Debug.WriteLine(this.m_currPathLine.Name + ": " + this.m_currPathLine.SphericalCoordinatesList.Count);

            this.SendPath(this.m_currDrawMode, path);

			// clear out positions
            this.m_lastMousePoint = Point.Empty;
            this.m_mouseDownPoint = Point.Empty;

			// set current drawmode off
            this.m_currDrawMode = DrawMode.None;
            this.MouseDragMode = true;

			Logger.Write(2, "DRAW", this.m_currPathLine.Name, "Stop Freehand" + this.m_currPathLine.Name);

			return true;
		}

		/// <summary>
		/// Stops the drawing of Hotspots.
		/// </summary>
		/// <returns>True</returns>
		private bool stopHotspot()
		{
			// clear out positions
            this.m_lastMousePoint = Point.Empty;
            this.m_mouseDownPoint = Point.Empty;

			// set current drawmode off
            this.m_currDrawMode = DrawMode.None;
            this.MouseDragMode = true;

			return true;
		}

		/// <summary>
		/// Stops the drawing of Incidents.
		/// </summary>
		/// <returns>True</returns>
		private bool stopIncident()
		{
			// clear out positions
            this.m_lastMousePoint = Point.Empty;
            this.m_mouseDownPoint = Point.Empty;

			// set current drawmode off
            this.m_currDrawMode = DrawMode.None;
            this.MouseDragMode = true;

			return true;
		}

		/// <summary>
		/// Gets the next available "unique" path id.
		/// </summary>
		/// <returns>Next path id</returns>
		private int getAvailablePathId()
		{
            this.m_lastPathId++;
			return this.m_lastPathId;
		}

		#endregion

		#region UI Event Methods

		/// <summary>
		/// Called when a key is pressed down.
		/// </summary>
		/// <param name="keyEvent"></param>
		public void OnKeyDown(KeyEventArgs keyEvent)
		{
			// TODO:  Add JHU_DrawLayer.OnKeyDown implementation
		}
	
		/// <summary>
		/// Called when a key is released.
		/// </summary>
		/// <param name="keyEvent"></param>
		public void OnKeyUp(KeyEventArgs keyEvent)
		{
			// TODO:  Add JHU_DrawLayer.OnKeyUp implementation

		}
	
		/// <summary>
		/// Called when a mouse button is pressed.  Depending on the draw mode this will either
		/// start the drawing of a shape, add a point to the shape or draw the new object.
		/// </summary>
		/// <param name="e">Mouse event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseDown(MouseEventArgs e)
		{
			// If we aren't active just ignore
			if ( (!this.isOn) && (this.m_drawMode != DrawMode.None) )
				return false;

			Angle lat;
			Angle lon;

			// get the current mouse position
            if (DrawArgs.Camera.Altitude > 500000)
            {
                DrawArgs.Camera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out lat,
                    out lon);
            }
            else
            {
                DrawArgs.Camera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out lat,
                    out lon,
                    DrawArgs.CurrentWorldStatic);
            }


			// if we're off the globe ignore
			if (Angle.IsNaN(lat))
			{
				return false;
			}

			// save start position
            this.m_mouseDownPoint.X = e.X;
            this.m_mouseDownPoint.Y = e.Y;

            this.m_lastMousePoint = this.m_mouseDownPoint;

			// if we aren't currently drawing then start
			if (this.m_currDrawMode == DrawMode.None)
			{
                this.m_currDrawMode = this.m_drawMode;
                this.m_drawModeChange = true;
			}

			switch (this.m_currDrawMode)
			{
				// if we are in click mode (polys and points)
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					break;
				}
				case DrawMode.Hotspot:
				{
					// if the right mouse button was hit we're done with the poly
					if (e.Button == MouseButtons.Right)
					{
                        this.stopHotspot();
						return true;
					}

					// ignore if we've been dragged
					int dx = e.X - this.m_lastMousePoint.X;
					int dy = e.Y - this.m_lastMousePoint.Y;
					if(dx*dx+dy*dy > 3*3)
					{
                        this.m_mouseDownPoint = Point.Empty;

						return false;
					}

					// if this is the first click then
					if (this.m_drawModeChange)
					{
						int pathId = this.getAvailablePathId();
                        this.m_drawAlt = this.m_height;

						// Prompt for hotspot name
                        this.m_hotspotDialog.HotspotName = this.m_currDrawMode + "-" + pathId;
                        this.m_hotspotDialog.ShowDialog();

						if (this.m_hotspotDialog.DialogResult == DialogResult.OK)
						{
                            this.m_hotspot = new Hotspot (this.m_hotspotDialog.HotspotName, 
								lat.Degrees,
								lon.Degrees, this.m_drawAlt, this.m_color, this.m_hotspotDialog.URL, this.m_hotspotDialog.Description);

                            if (this.m_hotspotDialog.SaveCameraAngle)
                            {
                                this.m_hotspot.OnClickZoomAltitude = DrawArgs.Camera.Altitude;
                                this.m_hotspot.OnClickZoomHeading = DrawArgs.Camera.Heading.Degrees;
                                this.m_hotspot.OnClickZoomTilt = DrawArgs.Camera.Tilt.Degrees;
                            }

							if (this.m_hotspotLayer == null)
							{
                                this.m_hotspotLayer = new Icons("Hotspot Layer");
                                this.m_hotspotLayer.IsOn = true;
                                this.Add(this.m_hotspotLayer);
							}

                            this.m_hotspotLayer.Add(this.m_hotspot);

							Logger.Write(2, "DRAW", lat.Degrees, lon.Degrees, this.m_drawAlt, this.m_hotspotDialog.HotspotName, "Added Hotspot " + this.m_hotspotDialog.HotspotName);

                            this.SendHotspot(this.m_hotspot);
						}

						if (!this.m_drawLock) this.m_drawMode = DrawMode.None;

                        this.stopHotspot();
						return true;
					}
					break;
				}				
				case DrawMode.Incident:
				{
					// if the right mouse button was hit we're done with the poly
					if (e.Button == MouseButtons.Right)
					{
                        this.stopIncident();
						return true;
					}
//
//					// ignore if we've been dragged
//					int dx = e.X - m_lastMousePoint.X;
//					int dy = e.Y - m_lastMousePoint.Y;
//					if(dx*dx+dy*dy > 3*3)
//					{
//						m_mouseDownPoint = Point.Empty;
//
//						return false;
//					}
//
//					// if this is the first click then
//					if (m_drawModeChange)
//					{
//						int pathId = this.getAvailablePathId();	
//						m_drawAlt = m_height;
//
//						// Prompt for hotspot name & type
//						m_iconDialog.IconName = "New" + m_iconDialog.IconTypeName + "-" + pathId;
//						m_iconDialog.IconIdNum = pathId;
//						m_iconDialog.IconType = this.CurrIconType;
//						m_iconDialog.ShowDialog();
//
//						if (m_iconDialog.DialogResult == DialogResult.OK)
//						{
//							// See if already in track table
//							m_incident = m_activeTracks[m_iconDialog.IconName] as CS_TrackData;
//
//							if (m_incident == null)
//							{
//								m_incident = new CS_TrackData (
//									"Operator",
//									m_iconDialog.IconName,
//									m_iconDialog.IconName,
//									m_iconDialog.IconType,
//									JHU_Enums.Affiliations.HOSTILE,
//									JHU_Enums.BattleDimensions.GROUND,
//									m_iconDialog.Description,
//									m_iconDialog.Description,
//									lat.Degrees,
//									lon.Degrees,
//									m_drawAlt,
//									0,
//									0,
//									"",
//									DateTime.UtcNow,
//									DateTime.UtcNow,
//									"");
//
//								this.CurrIconType = m_iconDialog.IconTypeName;
//
//								Logger.Write(2, "DRAW", lat.Degrees, lon.Degrees, m_drawAlt, m_iconDialog.IconName, "Added Incident " + m_iconDialog.IconName);
//
//								// add to master table
//								m_activeTracks.Add(m_incident);
//							}
//							else
//							{
//								// Update potentially changing values
//								m_incident.Type = m_iconDialog.IconType;
//								m_incident.SetPosition(lat.Degrees, lon.Degrees, m_drawAlt);
//
//								m_incident.UpdateTime = DateTime.UtcNow;
//								m_incident.Description = m_iconDialog.Description;
//							}
//
//							SendTrack(m_incident);
//						}
//
//						if (!m_drawLock)
//							m_drawMode = DrawMode.None;											
//
//						stopIncident();
//						return true;
//					}
					break;
				}
				case DrawMode.PostIt:
				{
					// Actually add the object on mouse up, not mouse down.
					// If mouse is dragged too much between down and up then ignore this click.
					break;
				}

				// if we are in freehand draw then start drawing.
				case DrawMode.Freehand:
				{
                    this.MouseDragMode = false;

                    this.m_drawAlt = DrawArgs.Camera.TerrainElevation + this.m_height;

					int pathId = this.getAvailablePathId();
                    this.m_currPathLine = new TerrainPath("Shape" + "-" + pathId + ".wwb", 
						DrawArgs.CurrentWorldStatic, 
						0.0,
                        (DrawArgs.Camera.Altitude * 1.1) + 100000, 
						"Shape" + "-" + pathId + ".wwb", this.m_drawAlt, this.m_color,
						DrawArgs.CurrentWorldStatic.TerrainAccessor);

                    this.Add(this.m_currPathLine);

					// add point to path line
                    this.m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, this.m_drawAlt);

					// save the first point
                    this.m_firstLat = lat;
                    this.m_firstLon = lon;
                    this.m_firstAlt = this.m_drawAlt;

                    this.m_currPathLine.IsOn = true;

					Logger.Write(2, "DRAW", this.m_firstLat.Degrees, this.m_firstLon.Degrees, this.m_drawAlt, this.m_currPathLine.Name, "Starting Freehand path " + this.m_currPathLine.Name);

					if (!this.m_drawLock) this.m_drawMode = DrawMode.None;

					break;
				}
					// We aren't drawing.  Just ignore this event.
				default:
					return false;
			}

			return true;
		}
	
		/// <summary>
		/// Called when the mose moves.
		/// </summary>
		/// <param name="e">Mouse event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseMove(MouseEventArgs e)
		{
			// If we aren't active just ignore
			if ( (!this.isOn) && (this.m_drawMode != DrawMode.None) )
				return false;

			Angle lat;
			Angle lon;

			// get the current mouse position
            if (DrawArgs.Camera.Altitude > 500000)
            {
                DrawArgs.Camera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out lat,
                    out lon);
            }
            else
            {
                DrawArgs.Camera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out lat,
                    out lon,
                    DrawArgs.CurrentWorldStatic);
            }

			// if we're off the globe ignore
			if (Angle.IsNaN(lat))
			{
				return false;
			}

			switch (this.m_currDrawMode)
			{

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
					// if we've moved more than a jiggle
					int dx = e.X - this.m_lastMousePoint.X;
					int dy = e.Y - this.m_lastMousePoint.Y;
					if(dx*dx+dy*dy > 2*2)
					{
                        this.m_drawAlt = DrawArgs.Camera.TerrainElevation + this.m_height;

						// add point to the path line
                        this.m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, this.m_drawAlt);

						// save last position
                        this.m_lastLat = lat;
                        this.m_lastLon = lon;

                        this.m_lastMousePoint.X = e.X;
                        this.m_lastMousePoint.Y = e.Y;
					}
					break;
				}

					// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					// if this isn't the first point draw a guide line
					if (!this.m_drawModeChange) this.BuildGuideLine(lat, lon);

					break;
				}

				case DrawMode.Hotspot:
				case DrawMode.Incident:
				case DrawMode.PostIt:
				{
					// No need to do anything.  A little mouse jiggle wont cancel this
					// drawing.  Determine if it is too much on mouse up.  Keep this
					// so we return true at the end and not let someone else handle 
					// this mouse event (or the camera might move or angle).
					break;
				}

				// We aren't drawing.  Just ignore this event.
				default:
					return false;
			}

			return true;
		}

		/// <summary>
		/// Called when a mouse button is released.
		/// </summary>
		/// <param name="e">Mouse event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseUp(MouseEventArgs e)
		{
			// If we aren't active just ignore
			if ( (!this.isOn) && (this.m_drawMode != DrawMode.None) )
				return false;

			// if we were off the globe on mouse down ignore
			if (this.m_mouseDownPoint == Point.Empty)
			{
				return false;
			}

			Angle lat;
			Angle lon;

            // get the current mouse position
            if (DrawArgs.Camera.Altitude > 500000)
            {
                DrawArgs.Camera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out lat,
                    out lon);
            }
            else
            {
                DrawArgs.Camera.PickingRayIntersectionWithTerrain(
                    e.X,
                    e.Y,
                    out lat,
                    out lon,
                    DrawArgs.CurrentWorldStatic);
            }

			// if we're off the globe clear modes and ignore
			if (Angle.IsNaN(lat))
			{
				// if they let go in a free hand draw off the map cancel path?
				if (this.m_currDrawMode == DrawMode.Freehand)
				{
                    this.stopFreehand();
				}

                this.m_mouseDownPoint = Point.Empty;
                this.m_lastMousePoint = Point.Empty;

				return false;
			}

            this.m_drawAlt = DrawArgs.Camera.TerrainElevation + this.m_height;

			switch (this.m_currDrawMode)
			{
					// if we're not drawing ignore
				case DrawMode.None:
					return false;

				case DrawMode.Hotspot:
				{
					break;
				}

				case DrawMode.Incident:
				{
					break;
				}
					// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{
					// if the right mouse button was hit we're done with the poly
					if (e.Button == MouseButtons.Right)
					{
                        this.stopPoly();

						return true;
					}

					// ignore if we've been dragged
					int dx = e.X - this.m_lastMousePoint.X;
					int dy = e.Y - this.m_lastMousePoint.Y;
					if(dx*dx+dy*dy > 3*3)
					{
                        this.m_mouseDownPoint = Point.Empty;

						return false;
					}

					// if this is the first click then
					if (this.m_drawModeChange)
					{
                        this.MouseDragMode = false;

                        this.m_drawAlt = DrawArgs.Camera.TerrainElevation + this.m_height;

						// create a new path list
						int pathId = this.getAvailablePathId();
                        this.m_currPathLine = new TerrainPath(this.m_currDrawMode + "-" + pathId + ".wwb", 
							DrawArgs.CurrentWorldStatic, 
							0.0, 
							DrawArgs.Camera.Altitude + 100000, this.m_currDrawMode + "-" + pathId + ".wwb", this.m_drawAlt, this.m_color,
							DrawArgs.CurrentWorldStatic.TerrainAccessor);

                        this.Add(this.m_currPathLine);

						// add point to the path line
                        this.m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, this.m_drawAlt);

						Logger.Write(2, "DRAW", lat.Degrees, lon.Degrees, this.m_drawAlt, this.m_currPathLine.Name, "Starting Poly path " + this.m_currPathLine.Name);

                        this.m_currPathLine.IsOn = true;
						this.IsOn = true;

						// save the first point
                        this.m_firstLat = lat;
                        this.m_firstLon = lon;
                        this.m_firstAlt = this.m_drawAlt;

						// save last position
                        this.m_lastLat = lat;
                        this.m_lastLon = lon;

                        this.m_drawModeChange = false;

						if (!this.m_drawLock) this.m_drawMode = DrawMode.None;
					}
					else
					{
						// add intermediate points if distance is great
						Angle dist = World.ApproxAngularDistance(this.m_lastLat, this.m_lastLon, lat, lon);
						if (dist.Degrees > 10)
						{
							for(int i=0; i < 10; i++)
							{
								float t = (float)i / 9;
								Angle iLat, iLon;

								World.IntermediateGCPoint(t, this.m_lastLat, this.m_lastLon, lat, lon,
									dist, out iLat, out iLon );

                                this.m_currPathLine.Add((float) iLat.Degrees, (float) iLon.Degrees, this.m_drawAlt);
							}
						}

						// add point to the path line
                        this.m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, this.m_drawAlt);

						Logger.Write(3, "DRAW", lat.Degrees, lon.Degrees, this.m_drawAlt, this.m_currPathLine.Name, "Added point to poly path " + this.m_currPathLine.Name);

						// save last position
                        this.m_lastLat = lat;
                        this.m_lastLon = lon;
					}

					break;
				}

					// if we are in freehand draw
				case DrawMode.Freehand:
				{
					// save the last point
                    this.m_currPathLine.Add((float) lat.Degrees, (float) lon.Degrees, this.m_drawAlt);

                    this.stopFreehand();

					break;
				}
			}

			return true;
		}
	
		/// <summary>
		/// Called when the mouse wheel is used.  Not implemented.
		/// </summary>
		/// <param name="e">Mouse Event.</param>
		/// <returns>Whether we've handled this event completely.  False allows another event
		/// handler the opportunity to handle this mouse event.</returns>
		public bool OnMouseWheel(MouseEventArgs e)
		{
			// TODO:  Add JHU_DrawLayer.OnMouseWheel implementation
			return false;
		}
	
		#endregion

		#region UI Thread Methods

		public override void Render(DrawArgs drawArgs)
		{
			if(!this.isOn)
				return;

			if (!this.Initialized)
				return;

			if ((this.m_hotspotLayer != null) && (!this.m_hotspotLayer.Initialized)) this.m_hotspotLayer.Initialize(drawArgs);

			// render all the child objects (paths)
			base.Render(drawArgs);

			switch (this.m_currDrawMode)
			{
				// if we are in poly mode
				case DrawMode.Polyline:
				case DrawMode.Polygon:
				{

					Device device = drawArgs.device;
					device.SetRenderState(RenderState.ZBufferEnable = false;
					device.SetTextureStageState(, TextureStage.ColorOperation = TextureOperation.Disable;
					device.VertexFormat = CustomVertex.PositionColored.Format;

					// Draw the measure line + ends
					Vector3 referenceCenter = new Vector3(
						(float)drawArgs.WorldCamera.ReferenceCenter.X,
						(float)drawArgs.WorldCamera.ReferenceCenter.Y,
						(float)drawArgs.WorldCamera.ReferenceCenter.Z);

					drawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
						-referenceCenter
						);

					device.DrawUserPrimitives(PrimitiveType.LineStrip, this.m_guideLine.Length-1, this.m_guideLine);

					drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;

					device.SetRenderState(RenderState.ZBufferEnable = true;
					break;
				}
			}
		}
	
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
            return base.PerformSelectionAction(drawArgs);
		}
	
		public override void Update(DrawArgs drawArgs)
		{
			// if we aren't initialized go ahead and init
			if(!this.isInitialized) this.Initialize(drawArgs);

			base.Update(drawArgs);
		}
	
		public override void Initialize(DrawArgs drawArgs)
		{
			base.Initialize(drawArgs);

            this.isInitialized = true;
		}

		#endregion

		#region Communication Methods
		public void SendPath(DrawMode mode, LineFeature path)
		{
			// SavePathAsXML(mode, path);
			// Send somewhere
		}

		public void SendHotspot(Hotspot hotspot)
		{
			// SaveHotspotAsXML(hotspot)
			// Send somewhere
		}

		#endregion

		#region XML Support Methods

		public string SavePathAsXML(DrawMode type, LineFeature path)
		{
			string result; 
			try
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter xw = new XmlTextWriter(sw);
				xw.Formatting = Formatting.Indented;

				CollabObj ds = new CollabObj();

				CollabObj.InfoRow ir = ds.Info.NewInfoRow();
				ir.Name = path.Name;
				ir.Type = type.ToString();
				ir.Color = path.LineColor.Name;
				ir.LineWidth = path.LineWidth;
				ir.MinAlt = path.MinimumDisplayAltitude;
				ir.MaxAlt = path.MaximumDisplayAltitude;
				ir.NumPoints = path.Points.Length;
				ds.Info.AddInfoRow(ir);

				CollabObj.PointsRow pr;

				foreach (Point3d item in path.Points)
				{
					pr = ds.Points.NewPointsRow();

					pr.Lat = item.X;
					pr.Lon = item.Y;
					pr.Alt = item.Z;
					ds.Points.AddPointsRow(pr);
				}

				ds.AcceptChanges();
				ds.WriteXml(xw);

				result = sw.ToString();

				sw.Close();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());			
				result = string.Empty;
			}

			return result;
		}

		public string SaveHotspotAsXML(Hotspot hotspot)
		{
			string result; 
			try
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter xw = new XmlTextWriter(sw);
				xw.Formatting = Formatting.Indented;

				CollabObj ds = new CollabObj();

				CollabObj.InfoRow ir = ds.Info.NewInfoRow();
				ir.Name = hotspot.Name;
				ir.Type = DrawMode.Hotspot.ToString();
				ir.Icon = "Hotspot";
				ir.Desc = hotspot.Description;
				ir.Affiliation = ""; // hotspot.Affiliation.ToString();
				ir.Dimension = ""; // hotspot.BattleDimension.ToString();
				ir.Color = this.m_color.Name;
				ir.NumPoints = 1;
                ir.URL = hotspot.ClickableActionURL;
				ds.Info.AddInfoRow(ir);

				CollabObj.PointsRow pr = ds.Points.NewPointsRow();
				pr.Lat = hotspot.Latitude;
				pr.Lon = hotspot.Longitude;
				pr.Alt = hotspot.Altitude;
				ds.Points.AddPointsRow(pr);

                CollabObj.CameraRow cr = ds.Camera.NewCameraRow();
                cr.Altitude = this.m_hotspot.OnClickZoomAltitude;
                cr.Heading = this.m_hotspot.OnClickZoomHeading;
                cr.Tilt = this.m_hotspot.OnClickZoomTilt;

				ds.AcceptChanges();
				ds.WriteXml(xw);
				result = sw.ToString();

				sw.Close();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());			
				result = string.Empty;
			}

			return result;
		}

		public string SaveTrackAsXML(Icon track)
		{
			string result; 
			try
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter xw = new XmlTextWriter(sw);
				xw.Formatting = Formatting.Indented;


				CollabObj ds = new CollabObj();

				CollabObj.InfoRow ir = ds.Info.NewInfoRow();
				ir.Name = track.Name;
				ir.Type = DrawMode.Incident.ToString();
				ir.Icon = "";  // generated from bitmap
				ir.Desc = track.Description;
				ir.Affiliation = ""; // track.Affiliation.ToString();
				ir.Dimension = ""; // track.BattleDimension.ToString();
				ir.Color = this.m_color.Name;
				ir.NumPoints = 1;
				ds.Info.AddInfoRow(ir);

				CollabObj.PointsRow pr = ds.Points.NewPointsRow();
				pr.Lat = track.Latitude;
				pr.Lon = track.Longitude;
				pr.Alt = track.Altitude;
				ds.Points.AddPointsRow(pr);

				ds.AcceptChanges();
				ds.WriteXml(xw);
				result = sw.ToString();

				sw.Close();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());			
				result = string.Empty;
			}

			return result;
		}


		public LineFeature LoadLineFromXML (CollabObj ds)
		{
			string name; 
			string type;
			string iconName; 
			string description; 
			string affiliation; 
			string dimension; 
			string color; 
			double lineWidth;
			double minAlt;
			double maxAlt;
			int numPoints; 
			
			CollabObj.InfoRow ir = (CollabObj.InfoRow) ds.Info.Rows[0];

			name = ir.Name;
			type = ir.Type;

			iconName = ir.Icon;

			description = ir.Desc;
			affiliation = ir.Affiliation;
			dimension = ir.Dimension;
			color = ir.Color;
			lineWidth = ir.LineWidth;
			numPoints = ir.NumPoints;

			minAlt = ir.MinAlt;
			maxAlt = ir.MaxAlt;

			if (maxAlt <= 0)
				maxAlt = double.MaxValue;

			CollabObj.PointsRow pr; 

			Point3d[] points = new Point3d[numPoints];

			for (int i=0; i < numPoints; i++)
			{
				pr = (CollabObj.PointsRow) ds.Points.Rows[i];

				points[i] = new Point3d(pr.Lon, pr.Lat, pr.Alt);
			}

			int pathId = this.getAvailablePathId();

			LineFeature path = new LineFeature(type + "-" + pathId + ".wwb", DrawArgs.CurrentWorldStatic, points, Color.FromName(color));
			path.LineWidth = (float) lineWidth;

			return path;
		}


		public Hotspot LoadHotspotFromXML (CollabObj ds)
		{
			string name; 
			string type;
			string iconName; 
			string description; 
			string affiliation; 
			string dimension; 
			string color; 
			int numPoints; 
			float lat; 
			float lon; 
			float alt; 

			CollabObj.InfoRow ir = (CollabObj.InfoRow) ds.Info.Rows[0];

			name = ir.Name;
			type = ir.Type;

			iconName = ir.Icon;

			description = ir.Desc;
			affiliation = ir.Affiliation;
			dimension = ir.Dimension;
			color = ir.Color;
			numPoints = ir.NumPoints;

			CollabObj.PointsRow pr = (CollabObj.PointsRow) ds.Points.Rows[0];

			lat = (float) pr.Lat;
			lon = (float) pr.Lon;
			alt = (float) pr.Alt;

			int id = this.getAvailablePathId();

			Hotspot hotspot = new Hotspot("Hotspot-" + id.ToString(),
				lat,
				lon,
				alt,
				Color.FromName(color),
				ir.URL,
				description);

            CollabObj.CameraRow cr = (CollabObj.CameraRow) ds.Camera.Rows[0];

            this.m_hotspot.OnClickZoomAltitude = cr.Altitude;
            this.m_hotspot.OnClickZoomHeading = cr.Heading;
            this.m_hotspot.OnClickZoomTilt = cr.Tilt;

			return hotspot;
		}
//
//
//		public CS_TrackData LoadTrackFromXML (CollabObj ds)
//		{
//			string name; 
//			string type;
//			string iconName; 
//			string description; 
//			string affiliation; 
//			string dimension; 
//			string color; 
//			int numPoints; 
//			float lat; 
//			float lon; 
//			float alt; 
//
//			CollabObj.InfoRow ir = (CollabObj.InfoRow) ds.Info.Rows[0];
//
//			name = ir.Name;
//			type = ir.Type;
//
//			iconName = ir.Icon;
//
//			description = ir.Desc;
//			affiliation = ir.Affiliation;
//			dimension = ir.Dimension;
//			color = ir.Color;
//			numPoints = ir.NumPoints;
//
//			CollabObj.PointsRow pr = (CollabObj.PointsRow) ds.Points.Rows[0];
//
//			lat = (float) pr.Lat;
//			lon = (float) pr.Lon;
//			alt = (float) pr.Alt;
//
//			CS_TrackData track = new CS_TrackData("CollabSpace",
//				name,
//				name,
//				iconName,
//				(JHU_Enums.Affiliations) Enum.Parse(typeof(JHU_Enums.Affiliations), affiliation, true),
//				(JHU_Enums.BattleDimensions) Enum.Parse(typeof(JHU_Enums.BattleDimensions), dimension, true),
//				description,
//				description,
//				lat,
//				lon,
//				alt,
//				0.0,
//				0.0,
//				"",
//				DateTime.UtcNow,
//				DateTime.UtcNow,
//				"");
//
//			return track;
//		}

		#endregion

		#region Context Menu Methods

		public override void BuildContextMenu(ContextMenu menu)
		{
			// initialize context menu
			MenuItem topMenuItem = new MenuItem(this.Name);
			switch (this.DrawingMode)
			{
				case DrawMode.None:
					topMenuItem.Text = this.Name + ": None";
					break;
				case DrawMode.Hotspot:
					topMenuItem.Text = this.Name + ": Hotspot";
					break;
				case DrawMode.Incident:
					topMenuItem.Text = this.Name + ": Incident";
					break;
				case DrawMode.PostIt:
					topMenuItem.Text = this.Name + ": PostIt";
					break;
				case DrawMode.Polyline:
					topMenuItem.Text = this.Name + ": Polyline";
					break;
				case DrawMode.Polygon:
					topMenuItem.Text = this.Name + ": Polygon";
					break;
				case DrawMode.Freehand:
					topMenuItem.Text = this.Name + ": Freehand";
					break;
				case DrawMode.Disabled:
					topMenuItem.Text = this.Name + ": Disabled (Read Only)";
					break;
				default:
					break;
			}

			if (this.m_drawLock)
			{
				topMenuItem.Text += " (Locked)";
			}

			MenuItem stopMenuItem = new MenuItem("Stop Drawing", new EventHandler(this.whiteboardStopMenuItem_Click));
			if (this.DrawingMode == DrawMode.None)
			{
				stopMenuItem.Enabled = false;
			}

			MenuItem lockMenuItem = new MenuItem("Enable Draw Lock", new EventHandler(this.whiteboardLockMenuItem_Click));
			if (this.DrawLock)
			{
				lockMenuItem.Text = "Disable Draw Lock";
			}

			MenuItem drawMenuItem = new MenuItem("Draw modes...");

			MenuItem hotspotMenuItem = new MenuItem("Hotspot", new EventHandler(this.whiteboardHotspotMenuItem_Click));
			drawMenuItem.MenuItems.Add(hotspotMenuItem);

			MenuItem incidentMenuItem = new MenuItem("Incident", new EventHandler(this.whiteboardIncidentMenuItem_Click));
			drawMenuItem.MenuItems.Add(incidentMenuItem);

			MenuItem postItMenuItem = new MenuItem("PostIt", new EventHandler(this.whiteboardPostItMenuItem_Click));
			drawMenuItem.MenuItems.Add(postItMenuItem);
			postItMenuItem.Enabled = false;

			MenuItem polygonMenuItem = new MenuItem("Polygon", new EventHandler(this.whiteboardPolygonMenuItem_Click));
			drawMenuItem.MenuItems.Add(polygonMenuItem);

			MenuItem polylineMenuItem = new MenuItem("Polyline", new EventHandler(this.whiteboardPolyLineMenuItem_Click));
			drawMenuItem.MenuItems.Add(polylineMenuItem);
	
			MenuItem freehandMenuItem = new MenuItem("Freehand", new EventHandler(this.whiteboardFreehandMenuItem_Click));
			drawMenuItem.MenuItems.Add(freehandMenuItem);

			MenuItem colorMenuItem = new MenuItem("Draw colors... "+ this.m_color.ToString());

			MenuItem redMenuItem = new MenuItem("Red", new EventHandler(this.whiteboardRedMenuItem_Click));
			colorMenuItem.MenuItems.Add(redMenuItem);

			MenuItem blueMenuItem = new MenuItem("Blue", new EventHandler(this.whiteboardBlueMenuItem_Click));
			colorMenuItem.MenuItems.Add(blueMenuItem);

			MenuItem greenMenuItem = new MenuItem("Green", new EventHandler(this.whiteboardGreenMenuItem_Click));
			colorMenuItem.MenuItems.Add(greenMenuItem);

			MenuItem yellowMenuItem = new MenuItem("Yellow", new EventHandler(this.whiteboardYellowMenuItem_Click));
			colorMenuItem.MenuItems.Add(yellowMenuItem);

			MenuItem orangeMenuItem = new MenuItem("Orange", new EventHandler(this.whiteboardOrangeMenuItem_Click));
			colorMenuItem.MenuItems.Add(orangeMenuItem);

			MenuItem whiteMenuItem = new MenuItem("White", new EventHandler(this.whiteboardWhiteMenuItem_Click));
			colorMenuItem.MenuItems.Add(whiteMenuItem);

			MenuItem turquoiseMenuItem = new MenuItem("Turquoise", new EventHandler(this.whiteboardTurquoiseMenuItem_Click));
			colorMenuItem.MenuItems.Add(turquoiseMenuItem);

            MenuItem clearMenuItem = new MenuItem("Clear WhiteBoard", new EventHandler(this.whiteboardClearMenuItem_Click));

            MenuItem shareMenuItem = new MenuItem("Share WhiteBoard", new EventHandler(this.whiteboardShareMenuItem_Click));

            MenuItem getSharedMenuItem = new MenuItem("Get Shared WhiteBoard", new EventHandler(this.whiteboardGetSharedMenuItem_Click));

			if (!this.Writeable)
			{
				topMenuItem.Enabled = false;
				lockMenuItem.Enabled = false;
				stopMenuItem.Enabled = false;
				drawMenuItem.Enabled = false;
				hotspotMenuItem.Enabled = false;
				incidentMenuItem.Enabled = false;
				polygonMenuItem.Enabled = false;
				polylineMenuItem.Enabled = false;
				freehandMenuItem.Enabled = false;
				colorMenuItem.Enabled = false;
			}

			menu.MenuItems.Add(topMenuItem);
			menu.MenuItems.Add(stopMenuItem);
			menu.MenuItems.Add(lockMenuItem);
			menu.MenuItems.Add(drawMenuItem);
			menu.MenuItems.Add(colorMenuItem);
            menu.MenuItems.Add(clearMenuItem);
            menu.MenuItems.Add(shareMenuItem);
            menu.MenuItems.Add(getSharedMenuItem);
		}

        void whiteboardClearMenuItem_Click(object sender, EventArgs s)
        {
            this.StopDrawing();
            this.RemoveAll();
            Logger.Write(1, "DRAW", "", "Whiteboard Clear Menu Item Clicked");
        }

        void whiteboardShareMenuItem_Click(object sender, EventArgs s)
        {
//            m_whiteboardDialog.ShowDialog();
//
//            if (m_whiteboardDialog.DialogResult == DialogResult.OK)
//            {
//                ShareLatestShapes(m_whiteboardDialog.OverlayName);
//            }

            Logger.Write(1, "DRAW", "", "Whiteboard Share Menu Item Clicked");
        }

        void whiteboardGetSharedMenuItem_Click(object sender, EventArgs s)
        {
//            m_getWhiteboardDialog.ShowDialog();
//
//            if (m_getWhiteboardDialog.DialogResult == DialogResult.OK)
//            {
//                GetSharedOverlay(m_getWhiteboardDialog.OverlayName);
//            }

            Logger.Write(1, "DRAW", "", "Whiteboard Get Shared Menu Item Clicked");
        }

		void whiteboardStopMenuItem_Click(object sender, EventArgs s)
		{
            this.DrawingMode = DrawMode.None;
			Logger.Write(1, "DRAW", "", "Whiteboard Stop Menu Item Clicked");
		}

		void whiteboardLockMenuItem_Click(object sender, EventArgs s)
		{
			// Toggle the draw lock
            this.DrawLock = !this.DrawLock;
			Logger.Write(1, "DRAW", "", "Whiteboard Drawlock Menu Item Clicked");
		}

		void whiteboardPolyLineMenuItem_Click(object sender, EventArgs s)
		{
            this.DrawingMode = DrawMode.Polyline;
			Logger.Write(1, "DRAW", "", "Whiteboard Polyline Menu Item Clicked");
		}

		void whiteboardHotspotMenuItem_Click(object sender, EventArgs s)
		{
            this.DrawingMode = DrawMode.Hotspot;
			Logger.Write(1, "DRAW", "", "Whiteboard Hotspot Menu Item Clicked");
		}
		
		void whiteboardIncidentMenuItem_Click(object sender, EventArgs s)
		{
            this.DrawingMode = DrawMode.Incident;
			Logger.Write(1, "DRAW", "", "Whiteboard Incident Menu Item Clicked");
		}

		void whiteboardPostItMenuItem_Click(object sender, EventArgs s)
		{
            this.DrawingMode = DrawMode.PostIt;
			Logger.Write(1, "DRAW", "", "Whiteboard PostIt Menu Item Clicked");

		}

		void whiteboardPolygonMenuItem_Click(object sender, EventArgs s)
		{
            this.DrawingMode = DrawMode.Polygon;
			Logger.Write(1, "DRAW", "", "Whiteboard Polygon Menu Item Clicked");
		}

		void whiteboardFreehandMenuItem_Click(object sender, EventArgs s)
		{
            this.DrawingMode = DrawMode.Freehand;
			Logger.Write(1, "DRAW", "", "Whiteboard Freehand Menu Item Clicked");
		}

		void whiteboardRedMenuItem_Click(object sender, EventArgs s)
		{
            this.m_color = Color.Red;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Red Item Clicked");
		}

		void whiteboardBlueMenuItem_Click(object sender, EventArgs s)
		{
            this.m_color = Color.Blue;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Blue Item Clicked");
		}

		void whiteboardGreenMenuItem_Click(object sender, EventArgs s)
		{
            this.m_color = Color.LightGreen;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Green Item Clicked");
		}

		void whiteboardYellowMenuItem_Click(object sender, EventArgs s)
		{
            this.m_color = Color.Yellow;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Yellow Item Clicked");
		}
	
		void whiteboardOrangeMenuItem_Click(object sender, EventArgs s)
		{
            this.m_color = Color.Orange;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Orange Item Clicked");
		}

		void whiteboardWhiteMenuItem_Click(object sender, EventArgs s)
		{
            this.m_color = Color.White;
			Logger.Write(1, "DRAW", "", "Whiteboard Color White Item Clicked");
		}

		void whiteboardTurquoiseMenuItem_Click(object sender, EventArgs s)
		{
            this.m_color = Color.Turquoise;
			Logger.Write(1, "DRAW", "", "Whiteboard Color Turquose Item Clicked");
		}

		#endregion

	}
}
