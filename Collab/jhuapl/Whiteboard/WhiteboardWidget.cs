//========================= (UNCLASSIFIED) ==============================
// Copyright © 2005-2007 The Johns Hopkins University /
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

using WorldWind;
using WorldWind.Renderable;
using Collab.jhuapl.Util;
using WorldWind.Widgets;

namespace Collab.jhuapl.Whiteboard
{
	/// <summary>
	/// This form widget contains all the tools needed for whiteboard 
	/// operations.
	/// </summary>
	public class WhiteboardWidget : FormWidget
	{
        /// <summary>
        /// The basepath for all icons/images
        /// </summary>
        public string BasePath
        {
            get { return this.m_basePath; }
            set { this.m_basePath = value; }
        }
        protected string m_basePath;

        /// <summary>
        /// The whiteboard drawing layer
        /// </summary>
		public DrawLayer WhiteboardLayer
		{
			get { return this.m_whiteboardLayer; }
			set { this.m_whiteboardLayer = value; }
		}
        protected DrawLayer m_whiteboardLayer;

		/// <summary>
		/// Top level treenode for the whiteboard widget
		/// </summary>
		public TreeNodeWidget RootWidget
		{
			get { return this.m_rootWidget; }
			set { this.m_rootWidget = value; }
		}
		protected TreeNodeWidget m_rootWidget;

		/// <summary>
		/// The treenode that holds all the drawing tools
		/// </summary>
		public TreeNodeWidget DrawWidget
		{
			get { return this.m_drawWidget; }
			set { this.m_drawWidget = value; }
		}
		protected TreeNodeWidget m_drawWidget;

		/// <summary>
		/// This widget enables draw lock.  Once draw lock is enabled every
		/// mouse event triggers whatever drawing mode was last used.
		/// </summary>
		public TreeNodeWidget LockWidget
		{
			get { return this.m_lockWidget; }
			set { this.m_lockWidget = value; }
		}
		protected TreeNodeWidget m_lockWidget;

		public TreeNodeWidget ShapeWidget
		{
			get { return this.m_shapeWidget; }
			set { this.m_shapeWidget = value; }
		}
		protected TreeNodeWidget m_shapeWidget;

		public PanelWidget ShapePalette
		{
			get { return this.m_shapePalette; }
			set { this.m_shapePalette = value; }
		}
		protected PanelWidget m_shapePalette;

		public TreeNodeWidget ColorWidget
		{
			get { return this.m_colorWidget; }
			set { this.m_colorWidget = value; }
		}
		protected TreeNodeWidget m_colorWidget;

		public PanelWidget ColorPalette
		{
			get { return this.m_colorPalette; }
			set { this.m_colorPalette = value; }
		}
		protected PanelWidget m_colorPalette;

		public TreeNodeWidget IconWidget
		{
			get { return this.m_iconWidget; }
			set { this.m_iconWidget = value; }
		}
		protected TreeNodeWidget m_iconWidget;

		public PanelWidget IconPalette
		{
			get { return this.m_iconPalette; }
			set { this.m_iconPalette = value; }
		}
		protected PanelWidget m_iconPalette;

		protected ButtonWidget m_lockButton;
		protected ButtonWidget m_paletteButton;

		protected ButtonWidget m_hotspotButton;
		protected ButtonWidget m_postitButton;

		protected ButtonWidget m_polygonButton;
		protected ButtonWidget m_polylineButton;

		protected ButtonWidget m_freehandButton;

		public WhiteboardWidget(string name, string basePath) : base(name)
		{
            this.m_basePath = basePath;

			// Probably should put this in a try catch block or move out of constructor
            this.BuildForm();
		}

		/// <summary>
		/// Adds all out subwidgets
		/// </summary>
		protected void BuildForm()
		{
            this.m_rootWidget = new SimpleTreeNodeWidget();
            this.m_rootWidget.Name = "CollabSpace";
            this.m_rootWidget.IsRadioButton = true;
            this.m_rootWidget.Enabled = true;
            this.m_rootWidget.EnableCheck = false;
            this.m_rootWidget.IsChecked = false;
            this.m_rootWidget.Expanded = true;

			// Settings for sharing whiteboard info
//			m_sharingWidget = new SimpleTreeNodeWidget();
//			m_sharingWidget.Name = "Settings";
//			m_sharingWidget.IsRadioButton = true;
//			m_sharingWidget.Enabled = true;
//			m_sharingWidget.EnableCheck = false;
//			m_sharingWidget.IsChecked = true;
//			Add(m_sharingWidget);
//
//			m_cameraWidget = new SimpleTreeNodeWidget();
//			m_cameraWidget.Name = "Allow Camera Sharing";
//			m_cameraWidget.Enabled = true;
//			m_cameraWidget.EnableCheck = false;
//			m_cameraWidget.IsChecked = false;
//			m_sharingWidget.Add(m_cameraWidget);
//
//			m_whiteboardWidget = new SimpleTreeNodeWidget();
//			m_whiteboardWidget.Name = "Allow Whiteboard Sharing";
//			m_whiteboardWidget.Enabled = true;
//			m_whiteboardWidget.EnableCheck = false;
//			m_whiteboardWidget.IsChecked = false;
//			m_sharingWidget.Add(m_whiteboardWidget);

			// Drawing tool widgets
            this.m_drawWidget = new SimpleTreeNodeWidget();
            this.m_drawWidget.Name = "Drawing Tools";
            this.m_drawWidget.IsRadioButton = true;
            this.m_drawWidget.Enabled = true;
            this.m_drawWidget.EnableCheck = false;
            this.m_drawWidget.IsChecked = false;
            this.m_drawWidget.Expanded = true;
            this.m_rootWidget.Add(this.m_drawWidget);

            this.m_shapeWidget = new SimpleTreeNodeWidget();
            this.m_shapeWidget.Name = "Shape Palette";
            this.m_shapeWidget.IsRadioButton = true;
            this.m_shapeWidget.Enabled = true;
            this.m_shapeWidget.EnableCheck = false;
            this.m_shapeWidget.IsChecked = false;
            this.m_shapeWidget.Expanded = true;
            this.m_drawWidget.Add(this.m_shapeWidget);

            this.m_shapePalette = new PanelWidget("Shape Box");
            this.m_shapePalette.Location = new System.Drawing.Point(0,0);
            this.m_shapePalette.WidgetSize = new System.Drawing.Size(104, 64);
            this.m_shapePalette.HeaderEnabled = false;
            this.m_shapeWidget.Add(this.m_shapePalette);

            this.m_lockButton = new ButtonWidget();
            this.m_lockButton.Location = new System.Drawing.Point(4,4);
            this.m_lockButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\lock_open.png";
            this.m_lockButton.WidgetSize = new System.Drawing.Size(16,16);
            this.m_lockButton.CountHeight = true;
            this.m_lockButton.CountWidth = true;
            this.m_lockButton.LeftClickAction = new MouseClickAction(this.PerformLock);

            this.m_shapePalette.Add(this.m_lockButton);

            this.m_paletteButton = new ButtonWidget();
            this.m_paletteButton.Location = new System.Drawing.Point(24,4);
            this.m_paletteButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\palette.png";
            this.m_paletteButton.WidgetSize = new System.Drawing.Size(16,16);
            this.m_paletteButton.CountHeight = true;
            this.m_paletteButton.CountWidth = true;
			// m_paletteButton.LeftClickAction = new MouseClickAction(this.PerformColorPalette);

			// m_shapePalette.Add(m_paletteButton);

            this.m_hotspotButton = new ButtonWidget();
            this.m_hotspotButton.Location = new System.Drawing.Point(4,44);
            this.m_hotspotButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\flag_red.png";
            this.m_hotspotButton.WidgetSize = new System.Drawing.Size(16,16);
            this.m_hotspotButton.CountHeight = true;
            this.m_hotspotButton.CountWidth = true;
            this.m_hotspotButton.LeftClickAction = new MouseClickAction(this.PerformHotspot);

            this.m_shapePalette.Add(this.m_hotspotButton);

            this.m_postitButton = new ButtonWidget();
            this.m_postitButton.Location = new System.Drawing.Point(24,44);
            this.m_postitButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\comment.png";
            this.m_postitButton.WidgetSize = new System.Drawing.Size(16,16);
            this.m_postitButton.CountHeight = true;
            this.m_postitButton.CountWidth = true;
			// m_postitButton.LeftClickAction = new MouseClickAction(this.PerformColorPalette);

			// m_shapePalette.Add(m_postitButton);

            this.m_polygonButton = new ButtonWidget();
            this.m_polygonButton.Location = new System.Drawing.Point(44,44);
            this.m_polygonButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\shape_handles.png";
            this.m_polygonButton.WidgetSize = new System.Drawing.Size(16,16);
            this.m_polygonButton.CountHeight = true;
            this.m_polygonButton.CountWidth = true;
            this.m_polygonButton.LeftClickAction = new MouseClickAction(this.PerformPolygon);

            this.m_shapePalette.Add(this.m_polygonButton);

            this.m_polylineButton = new ButtonWidget();
            this.m_polylineButton.Location = new System.Drawing.Point(64,44);
            this.m_polylineButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\chart_line.png";
            this.m_polylineButton.WidgetSize = new System.Drawing.Size(16,16);
            this.m_polylineButton.CountHeight = true;
            this.m_polylineButton.CountWidth = true;
            this.m_polylineButton.LeftClickAction = new MouseClickAction(this.PerformPolyline);

            this.m_shapePalette.Add(this.m_polylineButton);

            this.m_freehandButton = new ButtonWidget();
            this.m_freehandButton.Location = new System.Drawing.Point(84,44);
            this.m_freehandButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\pencil.png";
            this.m_freehandButton.WidgetSize = new System.Drawing.Size(16,16);
            this.m_freehandButton.CountHeight = true;
            this.m_freehandButton.CountWidth = true;
            this.m_freehandButton.LeftClickAction = new MouseClickAction(this.PerformFreehand);

            this.m_shapePalette.Add(this.m_freehandButton);


            this.m_colorWidget = new SimpleTreeNodeWidget();
            this.m_colorWidget.Name = "Color Palette";
            this.m_colorWidget.IsRadioButton = true;
            this.m_colorWidget.Enabled = false;
            this.m_colorWidget.EnableCheck = false;
            this.m_colorWidget.IsChecked = false;
            this.m_drawWidget.Add(this.m_colorWidget);

            this.m_colorPalette = new PanelWidget("Color Box");
            this.m_colorPalette.Location = new System.Drawing.Point(0,0);
            this.m_colorPalette.WidgetSize = new System.Drawing.Size(125, 80);
            this.m_colorPalette.HeaderEnabled = false;
            this.m_colorWidget.Add(this.m_colorPalette);

            this.m_iconWidget = new SimpleTreeNodeWidget();
            this.m_iconWidget.Name = "Icon Palette";
            this.m_iconWidget.IsRadioButton = true;
            this.m_iconWidget.Enabled = true;
            this.m_iconWidget.EnableCheck = false;
            this.m_iconWidget.IsChecked = false;
            this.m_drawWidget.Add(this.m_iconWidget);

			//m_iconWidget.Add(m_iconPalette);

            this.Add(this.m_rootWidget);
		}

		#region button actions

		public void PerformLock(System.Windows.Forms.MouseEventArgs e)
		{
			if (this.m_whiteboardLayer != null)
			{
                this.m_whiteboardLayer.DrawLock = !this.m_whiteboardLayer.DrawLock;
				if (this.m_whiteboardLayer.DrawLock)
                    this.m_lockButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\lock_edit.png";
				else
                    this.m_lockButton.ImageName = this.m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\lock_open.png";
			}

			Logger.Write(1, "DRAW", "", "Lock Button Pressed");
		}

		public void PerformHotspot(System.Windows.Forms.MouseEventArgs e)
		{
			if (this.m_whiteboardLayer != null)
			{
                this.ResetDrawingIcons();
                this.m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Hotspot;
			}

			Logger.Write(1, "DRAW", "", "Hotspot Button Pressed");
		}

		public void PerformPolygon(System.Windows.Forms.MouseEventArgs e)
		{
			if (this.m_whiteboardLayer != null)
			{
                this.ResetDrawingIcons();
                this.m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Polygon;
			}

			Logger.Write(1, "DRAW", "", "Polygon Button Pressed");
		}

		public void PerformPolyline(System.Windows.Forms.MouseEventArgs e)
		{
			if (this.m_whiteboardLayer != null)
			{
                this.ResetDrawingIcons();
                this.m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Polyline;
			}

			Logger.Write(1, "DRAW", "", "Polyline Button Pressed");
		}

		public void PerformFreehand(System.Windows.Forms.MouseEventArgs e)
		{
			if (this.m_whiteboardLayer != null)
			{
                this.ResetDrawingIcons();
                this.m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Freehand;
			}

			Logger.Write(1, "DRAW", "", "Freehand Button Pressed");
		}

		public void ResetDrawingIcons()
		{
		}

		#endregion
	}
}
