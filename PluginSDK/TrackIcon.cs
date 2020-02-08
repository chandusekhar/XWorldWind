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
// Copyright (c) 2005 The Johns Hopkins University. 
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using WorldWind.Widgets;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind
{
	/// <summary>
	/// One icon in an icon layer
	/// </summary>
	public class TrackIcon : Icon
    {
        protected bool m_gotoMe;

        /// <summary>
        /// The heading for this object (0-360) in degrees
        /// </summary>
        public double Heading
        {
            get { return this.Rotation.Degrees; }
            set
            {
                this.Rotation = Angle.FromDegrees(value);
            }
        }

        /// <summary>
        /// The speed of this object (knots)
        /// </summary>
        public double Speed
        {
            get { return this.m_speed; }
            set { this.m_speed = value; }
        }

        /// <summary>
        /// The speed of this object (knots)
        /// </summary>
        protected double m_speed;

        /// <summary>
        /// The date time of the last update
        /// </summary>
        public DateTime UpdateTime
        {
            get { return this.m_updateTime; }
            set { this.m_updateTime = value; }
        }
        private DateTime m_updateTime;

        /// <summary>
        /// The date time of the source for this update
        /// </summary>
        public DateTime SourceTime
        {
            get { return this.m_sourceTime; }
            set { this.m_sourceTime = value; }
        }
        private DateTime m_sourceTime;

        /// <summary>
        /// Whether this object is a surface track that should hug the ground.
        /// Default is false.
        /// </summary>
        public bool IsSurfaceTrack
        {
            get { return this.m_isSurfaceTrack; }
            set { this.m_isSurfaceTrack = value; }
        }
        private bool m_isSurfaceTrack;

        #region Hook form

        protected FormWidget m_hookForm;

        protected SimpleTreeNodeWidget m_hookTreeNode;

        protected SimpleTreeNodeWidget m_hookGeneralTreeNode;

        protected SimpleTreeNodeWidget m_hookDetailTreeNode;

        protected SimpleTreeNodeWidget m_hookDescTreeNode;

        protected LabelWidget m_hookGeneralLabel;

        protected LabelWidget m_hookDetailLabel;

        protected LabelWidget m_hookDescLabel;

        #endregion

        #region Secondary textures (for unit size, acknowledgements, etc)

        /// <summary>
        /// Flag that indicates if the secondary icon texture should be shown.
        /// </summary>
        protected bool m_iconTexture2Show;

        public bool IconTexture2Show
        {
            get
            {
                return this.m_iconTexture2Show;
            }
            set
            {
                this.m_iconTexture2Show = value;
            }
        }

        /// <summary>
        /// Secondary texture bitmap path. (Overrides Image)
        /// </summary>
        public string Texture2FileName
        {
            get { return this.m_iconTexture2Name; }
            set
            {
                this.m_iconTexture2Name = value;
                this.m_newTexture = true;
                this.m_isUpdated = false;
            }
        }
        protected string m_iconTexture2Name;

        /// <summary>
        /// The secondary icon texture - typically a unit size
        /// </summary>
        protected IconTexture m_iconTexture2;

        /// <summary>
        /// Texture 2 rotation angle relative to north
        /// </summary>
        public Angle Texture2Rotation
        {
            get { return this.m_texture2Rotation; }
            set { this.m_texture2Rotation = value; }
        }
        private Angle m_texture2Rotation = Angle.Zero;

        /// <summary>
        /// Whether or not to rotate texture 2 differently than main texture
        /// </summary>
        public bool Texture2IsRotatedDifferent
        {
            get { return this.m_texture2IsRotatedDifferent; }
            set { this.m_texture2IsRotatedDifferent = value; }
        }
        private bool m_texture2IsRotatedDifferent;

        /// <summary>
        /// Whether or not to use the main rotation (aka heading) as the rotation angle.
        /// Set this (and Texture2IsRotated to true) if you are using it for a heading indicator
        /// This allows you to not to rotate the main icon while providing heading.
        /// </summary>
        public bool Texture2UseHeading
        {
            get { return this.m_texture2UseHeading; }
            set { this.m_texture2UseHeading = value; }
        }
        private bool m_texture2UseHeading;

        /// <summary>
        /// Flag that indicates if the tertiatary icon texture should be shown.
        /// </summary>
        protected bool m_iconTexture3Show = false;

        /// <summary>
        /// The name of the tertiary icon texture
        /// </summary>
        protected string m_iconTexture3Name;

        /// <summary>
        /// The secondary icon texture - typically a heading indicator or status
        /// </summary>
        protected IconTexture m_iconTexture3;

        /// <summary>
        /// Texture 3 rotation angle relative to north
        /// </summary>
        public Angle Texture3Rotation
        {
            get { return this.m_texture3Rotation; }
            set { this.m_texture3Rotation = value; }
        }
        private Angle m_texture3Rotation = Angle.Zero;

        /// <summary>
        /// Whether or not to rotate texture 3 differently from main texture
        /// </summary>
        public bool Texture3IsRotatedDifferent
        {
            get { return this.m_texture3IsRotatedDifferent; }
            set { this.m_texture3IsRotatedDifferent = value; }
        }
        private bool m_texture3IsRotatedDifferent;

        /// <summary>
        /// Whether or not to use the main rotation (aka heading) as the rotation angle.
        /// Set this (and Texture3IsRotated to true) if you are using it for a heading indicator
        /// This allows you to not to rotate the main icon while providing heading.
        /// </summary>
        public bool Texture3UseHeading
        {
            get { return this.m_texture3UseHeading; }
            set { this.m_texture3UseHeading = value; }
        }
        private bool m_texture3UseHeading;

        #endregion

        #region Track History

        /// <summary>
        /// Maximum number of history points
        /// </summary>
        public int MaxPoints = 600;

        /// <summary>
        /// History points data
        /// </summary>
        public class PosData
        {
            public double lat;
            public double lon;
            public double alt;
            public double spd;
            public double hdg;
            public DateTime updateTime;
            public DateTime sourceTime;
        }

        /// <summary>
        /// Whether or not to render the history trail
        /// </summary>
        public bool RenderTrail
        {
            get { return this.m_renderTrail; }
            set { this.m_renderTrail = value; }
        }
        private bool m_renderTrail = true;

        /// <summary>
        /// The history list
        /// </summary>
        public LinkedList<PosData> History
        {
            get { return this.m_history; }
            set { this.m_history = value; }
        }
        private LinkedList<PosData> m_history = new LinkedList<PosData>();

        /// <summary>
        /// The linefeature that displays the track history
        /// </summary>
        protected LineFeature m_lineFeature;

        /// <summary>
        /// Distance which to start showing the trail
        /// </summary>
        public double TrailShowDistance
        {
            get { return this.m_trailShowDistance; }
            set { this.m_trailShowDistance = value; }
        }
        private double m_trailShowDistance = 300000;

        #endregion

        #region 3-D Model

        /// <summary>
        /// Whether or not to render a 3-D model
        /// </summary>
        public bool RenderModel
        {
            get { return this.m_renderModel; }
            set 
            { 
                if (this.m_renderModel != value)
                {
                    this.m_renderModel = value;
                    this.m_modelFeatureError = false;

                    if (this.m_renderModel) this.updateModel(false);
                }
            }
        }
        private bool m_renderModel = true;

        /// <summary>
        /// Whether this model feature failed to load
        /// </summary>
        protected bool m_modelFeatureError;

        /// <summary>
        /// The ModelFeature representing the detailed view of this icon
        /// </summary>
        protected ModelFeature m_modelFeature;

        /// <summary>
        /// The file path to the model - default is a generic commercial airliner
        /// </summary>         
        public string ModelFilePath
        {
            get { return this.m_modelFilePath; }
            set 
            {
                // if the path has changed, delete old model and create new one
                if (value != this.m_modelFilePath)
                {
                    this.m_modelFilePath = value;
                    this.m_modelFeature = null;
                    this.m_modelFeatureError = false;
                    this.updateModel(false);
                }
            }
        }
        private string m_modelFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Data\Icons\unknown_commercial.x");

        /// <summary>
        /// Distance at which to start showing model
        /// </summary>
        public double ModelShowDistance
        {
            get { return this.m_modelShowDistance; }
            set { this.m_modelShowDistance = value; }
        }
        private double m_modelShowDistance = 200000;


        /// <summary>
        /// X Rotation for model.  Used when model is flipped on its side.
        /// </summary>
        public float ModelRotX
        {
            get { return this.m_modelRotX; }
            set { this.m_modelRotX = value; }
        }
        private float m_modelRotX = 180;

        public float ModelScale
        {
            get { return this.m_modelScale; }
            set { this.m_modelScale = value; }
        }
        private float m_modelScale = 100.0f;

        
        #endregion

        /// <summary>
		/// Initializes a new instance of a <see cref= "T:WorldWind.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		public TrackIcon(string name,
			double latitude, 
			double longitude) : base( name, latitude, longitude )
		{
            this.InitIcon();
		}

		/// <summary>
		/// Initializes a new instance of a <see cref= "T:WorldWind.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		public TrackIcon(string name,
			double latitude, 
			double longitude,
            double heightAboveSurface) : base(name, latitude, longitude, heightAboveSurface)
		{
            this.InitIcon();
		}

        /// <summary>
        /// Initializes a new instance of a <see cref= "T:WorldWind.Icon"/> class 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="heightAboveSurface"></param>
        /// <param name="TextureFileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="actionURL"></param>
        public TrackIcon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			string TextureFileName,
			int width,
			int height,
			string actionURL) : base( name, description, latitude, longitude, heightAboveSurface, TextureFileName, width, height, actionURL )
		{
            this.InitIcon();
        }

        private void InitIcon()
        {
            // Default our Id to Name since most track objects need it
            this.Id = this.Name;

            // we always want to use the hot color
            this.AlwaysHighlight = true;

            // force this to false because we always want icons with altitude to stay in the sky
            this.UseZeroVE = false;

            // update the detailed model view
            this.updateModel(false);

            // use point sprites
            this.UsePointSprite = true;

            // update sub object to initial positions
            this.updateSubObjects();
        }

		#region Overrriden methods

		public override void Initialize(DrawArgs drawArgs)
		{
            this.m_gotoMe = false;

            this.isSelectable = true;

            base.Initialize(drawArgs);

            // note base initialize SHOULD set is initialized flag
		}

        /// <summary>
        /// Updates where we are if the camera has changed position (and thereby might be using higher resolution terrain
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Update(DrawArgs drawArgs)
        {
            if (!this.IsOn)
                return;

            if (!this.m_isUpdated || (drawArgs.WorldCamera.ViewMatrix != this.lastView))
            {
                if (!this.m_isUpdated)
                {
                    this.updateSubObjects();
                }

                // call this to make sure elevation is reset
                if (this.m_modelFeature != null) this.m_modelFeature.Update(drawArgs);

                base.Update(drawArgs);

                // note base update SHOULD set is updated flag
            }
        }

        protected override void BuildIconTexture(DrawArgs drawArgs)
        {
            base.BuildIconTexture(drawArgs);

            try
            {

                // if secondary icon enabled
                if (this.m_iconTexture2Show && this.m_iconTexture2Name.Trim() != String.Empty)
                {
                    if (this.m_iconTexture2 != null)
                    {
                        this.m_iconTexture2.ReferenceCount--;
                    }

                    this.m_iconTexture2 = (IconTexture)DrawArgs.Textures[this.m_iconTexture2Name];
                    if (this.m_iconTexture2 == null)
                    {
                        this.m_iconTexture2 = new IconTexture(drawArgs.device, this.m_iconTexture2Name);
                        DrawArgs.Textures.Add(this.m_iconTexture2Name, this.m_iconTexture2);
                    }

                    if (this.m_iconTexture2 != null)
                    {
                        this.m_iconTexture2.ReferenceCount++;
                    }
                }

                // if teritary icon enabled
                if (this.m_iconTexture3Show && this.m_iconTexture3Name.Trim() != String.Empty)
                {
                    if (this.m_iconTexture3 != null)
                    {
                        this.m_iconTexture3.ReferenceCount--;
                    }

                    this.m_iconTexture3 = (IconTexture)DrawArgs.Textures[this.m_iconTexture3Name];
                    if (this.m_iconTexture3 == null)
                    {
                        this.m_iconTexture3 = new IconTexture(drawArgs.device, this.m_iconTexture3Name);
                        DrawArgs.Textures.Add(this.m_iconTexture3Name, this.m_iconTexture3);
                    }

                    if (this.m_iconTexture3 != null)
                    {
                        this.m_iconTexture3.ReferenceCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            this.m_newTexture = false;

        }

        /// <summary>
        /// Helper function to render icon description.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        protected override void RenderDescription(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color)
        {
            string description = this.GeneralInfo() + this.DetailedInfo() + this.DescriptionInfo();

            if (description != null)
            {
                // Render description field
                DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Bottom;
                int left = 10;
                if (World.Settings.ShowLayerManager)
                    left += World.Settings.LayerManagerWidth;
                Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10);

                // Draw description
                rect.Offset(1, -1);
                drawArgs.defaultDrawingFont.DrawText(
                    sprite, description,
                    rect,
                    format, descriptionColor);
            }
        }

        /// <summary>
        /// Builds the context menu for this icon
        /// </summary>
        /// <param name="menu"></param>
        public override void BuildContextMenu(ContextMenu menu)
        {
            // initialize context menu
            MenuItem gotoMenuItem = new MenuItem("Goto Location", new EventHandler(this.IconGotoMenuItem_Click));

            MenuItem hookMenuItem = new MenuItem("Hook " + this.name, new EventHandler(this.IconHookMenuItem_Click));

            MenuItem urlMenuItem = new MenuItem("Open URL", new EventHandler(this.IconURLMenuItem_Click));

            if ((this.ClickableActionURL == null) || (this.ClickableActionURL.Length <= 0))
            {
                urlMenuItem.Enabled = false;
            }

            menu.MenuItems.Add(gotoMenuItem);
            menu.MenuItems.Add(hookMenuItem);
            menu.MenuItems.Add(urlMenuItem);
        }

        /// <summary>
        /// Called before icon render.  If the user has clicked on one of the GoTos head there now.
        /// Renders 3-D model and history trails.  If you want to only show models or trails on
        /// mouseover set the TrailShowDistance or ModelShowDistance to 0.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public override void PreRender(DrawArgs drawArgs, bool isMouseOver)
        {
            base.PreRender(drawArgs, isMouseOver);

            if (this.RenderTrail && ((this.DistanceToIcon < this.TrailShowDistance) || isMouseOver || this.IsHooked))
            {
                if (this.RenderTrail && this.m_lineFeature != null)
                {
                    if (!this.m_lineFeature.Initialized || this.m_lineFeature.NeedsUpdate) this.m_lineFeature.Update(drawArgs);

                    this.m_lineFeature.Render(drawArgs);
                }
            }

            if (this.RenderModel && ((this.DistanceToIcon < this.ModelShowDistance) || isMouseOver || this.IsHooked))
            {
                if (this.m_modelFeature == null) this.updateModel(isMouseOver);

                if (!this.m_modelFeature.Initialized) this.m_modelFeature.Update(drawArgs);

                this.m_modelFeature.Render(drawArgs);
            }

            if (this.m_gotoMe) this.GoTo(drawArgs);
        }

        /// <summary>
        /// Called after icon render.  Always updates the hook form so the hook form still updates position even when the icon
        /// is out of view.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        public override void PostRender(DrawArgs drawArgs, bool isMouseOver)
        {
            base.PostRender(drawArgs, isMouseOver);

            this.UpdateHookForm();
        }

        /// <summary>
        /// Helper function to render icon texture.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color">the color to render the icon</param>
        /// <param name="isMouseOver">Whether or not the mouse is over the icon</param>
        protected override void RenderTexture(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, bool isMouseOver)
        {
            Matrix scaleTransform;
            Matrix rotationTransform;

            Matrix rotation2Transform;
            Matrix rotation3Transform;

            //Do Altitude depedent scaling for KMLIcons
            if (this.AutoScaleIcon)
            {
                float factor = 1;
                if (this.DistanceToIcon > this.MinIconZoomDistance)
                    factor -= (float)((this.DistanceToIcon - this.MinIconZoomDistance) / this.DistanceToIcon);
                if (factor < this.MinScaleFactor) factor = this.MinScaleFactor;

                this.XScale = factor * ((float) this.Width / this.IconTexture.Width);
                this.YScale = factor * ((float) this.Height / this.IconTexture.Height);
            }

            //scale and rotate image
            scaleTransform = Matrix.Scaling(this.XScale, this.YScale, 0);

            if (this.IsRotated)
                rotationTransform = Matrix.RotationZ((float) this.Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
            else
                rotationTransform = Matrix.Identity;

            sprite.Transform = scaleTransform * rotationTransform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
            sprite.Draw(this.IconTexture.Texture,
                new Vector3(this.IconTexture.Width >> 1, this.IconTexture.Height >> 1, 0),
                Vector3.Zero,
                color);

            if (this.m_iconTexture2Show)
            {
                Matrix tmpMatrix = sprite.Transform; 

                if (this.Texture2IsRotatedDifferent)
                {
                    if (this.Texture2UseHeading)
                        rotation2Transform = Matrix.RotationZ((float) this.Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
                    else
                        rotation2Transform = Matrix.RotationZ((float) this.Texture2Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
 
                   sprite.Transform = scaleTransform * rotation2Transform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
                }

                sprite.Draw(this.m_iconTexture2.Texture,
                    new Vector3(this.m_iconTexture2.Width >> 1, this.m_iconTexture2.Height >> 1, 0),
                    Vector3.Zero,
                    normalColor);

                // restore the main texture transform
                if (this.Texture2IsRotatedDifferent)
                    sprite.Transform = tmpMatrix;
            }

            if (this.m_iconTexture3Show)
            {
                if (this.Texture3IsRotatedDifferent)
                {
                    if (this.Texture3UseHeading)
                        rotation3Transform = Matrix.RotationZ((float) this.Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);
                    else
                        rotation3Transform = Matrix.RotationZ((float) this.Texture3Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);

                    sprite.Transform = scaleTransform * rotation3Transform * Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
                }

                sprite.Draw(this.m_iconTexture3.Texture,
                    new Vector3(this.m_iconTexture3.Width >> 1, this.m_iconTexture3.Height >> 1, 0),
                    Vector3.Zero,
                    normalColor);
            }
            
            // Reset transform to prepare for text rendering later
            sprite.Transform = Matrix.Identity;
        }

        /// <summary>
        /// Forces display of lable if we are hooked
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color"></param>
        /// <param name="labelRectangles"></param>
        /// <param name="isMouseOver"></param>
        protected override void RenderLabel(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, List<Rectangle> labelRectangles, bool isMouseOver)
        {
            base.RenderLabel(drawArgs, sprite, projectedPoint, color, labelRectangles, (isMouseOver || this.IsHooked));
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="spd"></param>
        /// <param name="hdg"></param>
        /// <param name="time"></param>
        public virtual void SetPosition(double lat, double lon, double alt, double spd, double hdg, DateTime time)
        {
            // Set new values
            this.Latitude = lat;
            this.Longitude = lon;
            this.Altitude = alt;
            this.Speed = spd;
            this.Heading = hdg;

            this.SourceTime = time;
            this.UpdateTime = DateTime.Now;

            // Recalculate XYZ coordinates
            this.m_isUpdated = false;
        }

		#endregion

        /// <summary>
        /// Helper class that updates history trail and model position
        /// </summary>
        protected virtual void updateSubObjects()
        {
            if (this.IsSurfaceTrack)
            {
                this.Altitude = 0;
                this.IsAGL = true;  // force this just in case so we aren't under terrain
            }

            // Save current values
            PosData pos = new PosData();

            pos.lat = this.Latitude;
            pos.lon = this.Longitude;
            pos.alt = this.Altitude;
            pos.spd = this.Speed;
            pos.hdg = this.Rotation.Degrees;
            pos.sourceTime = this.m_sourceTime;
            pos.updateTime = this.m_updateTime;

            try
            {
                this.m_history.AddFirst(pos);

                if (this.m_history.Count > this.MaxPoints) this.m_history.RemoveLast();

                // Add to line feature
                if (this.RenderTrail && this.m_lineFeature == null)
                {
                    this.m_lineFeature = new LineFeature(this.name + "_trail", DrawArgs.CurrentWorldStatic, null, null);
                    this.m_lineFeature.LineColor = System.Drawing.Color.White;
                    this.m_lineFeature.MaxPoints = this.MaxPoints;

                    if (this.IsAGL)
                        this.m_lineFeature.AltitudeMode = AltitudeMode.RelativeToGround;
                    else
                        this.m_lineFeature.AltitudeMode = AltitudeMode.Absolute;

                    this.m_lineFeature.Opacity = 128;
                    this.m_lineFeature.LineWidth = 3;

                    if (this.IsSurfaceTrack)
                    {
                        this.m_lineFeature.Extrude = false;
                        this.m_lineFeature.ExtrudeHeight = 0;
                        this.m_lineFeature.LineWidth = 1;
                    }
                    else
                    {
                        this.m_lineFeature.Extrude = true;
                        this.m_lineFeature.ExtrudeHeight = 1;
                        this.m_lineFeature.LineWidth = 1;
                        this.m_lineFeature.ExtrudeToGround = false;
                    }
                }

                if (this.RenderTrail) this.m_lineFeature.AddPoint(this.Longitude, this.Latitude, this.Altitude);

                if (this.RenderModel && this.m_modelFeature != null)
                {
                    this.m_modelFeature.Longitude = (float) this.Longitude;
                    this.m_modelFeature.Latitude = (float) this.Latitude;
                    this.m_modelFeature.Altitude = (float) this.Altitude;

                    float rot = (float)(180 - this.Rotation.Degrees);
                    if (rot < 0)
                        rot += 360;
                    this.m_modelFeature.RotZ = rot;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// updates the ModelFeature to the new mesh, position and color
        /// </summary>
        public virtual void updateModel(bool forceUpdate)
        {
            if (forceUpdate || (this.RenderModel && !this.m_modelFeatureError && (this.DistanceToIcon < this.ModelShowDistance)))
            {
                if (this.m_modelFeature == null)
                {
                    try
                    {
                        // do for current models that happen to point right 90.
                        float rotZ = (float)(180 - this.Rotation.Degrees);
                        if (rotZ < 0)
                            rotZ += 360;

                        // use generic commercial airliner mesh
                        this.m_modelFeature = new ModelFeature(this.name,
                            DrawArgs.CurrentWorldStatic, this.m_modelFilePath,
                            (float) this.Latitude,
                            (float) this.Longitude,
                            (float) this.Altitude, this.ModelScale, this.ModelRotX,
                            0,
                            rotZ);

                        this.m_modelFeature.IsElevationRelativeToGround = this.IsAGL;
                    }
                    catch
                    {
                        this.m_modelFeature = null;
                        this.m_modelFeatureError = true;
                    }
                }

                if (this.m_modelFeature != null) this.m_modelFeature.TintColor = this.PointSpriteColor;
            }
        }

        public void UpdateHookForm()
        {
            // update hook form
            if (this.m_hookForm != null)
            {
                if (this.m_hookForm.Enabled)
                {
                    this.m_hookForm.Text = this.DMS();
                    this.m_hookGeneralLabel.Text = this.GeneralInfo();
                    this.m_hookDetailLabel.Text = this.DetailedInfo();
                    this.m_hookDescLabel.Text = this.DescriptionInfo();
                }
                else
                {
                    this.m_hookForm.Dispose();

                    this.m_hookTreeNode = null;
                    this.m_hookGeneralTreeNode = null;
                    this.m_hookDetailTreeNode = null;
                    this.m_hookDescTreeNode = null;
                    this.m_hookGeneralLabel = null;
                    this.m_hookDetailLabel = null;
                    this.m_hookDescLabel = null;

                    this.m_hookForm = null;
                    this.IsHooked = false;
                }
            }
        }

        public void GoTo(DrawArgs drawArgs)
        {
            drawArgs.WorldCamera.SetPosition(this.Latitude, this.Longitude, this.OnClickZoomHeading, this.OnClickZoomAltitude, this.OnClickZoomTilt);
            this.m_gotoMe = false;
        }

        public void GoTo()
        {
            this.m_gotoMe = true;
        }

        public virtual string GeneralInfo()
        {
            StringBuilder outString = new StringBuilder();

            outString.AppendFormat("{0:-10} {1}\n", "Id:", this.m_id);
            outString.AppendFormat("{0:-10} {1}\n", "Name:", this.Name);
            outString.AppendFormat("{0:-10} {1:00.00000}\n", "Lat:", this.Latitude);
            outString.AppendFormat("{0:-10} {1:000.00000}\n", "Lon:", this.Longitude);
            outString.AppendFormat("{0:-10} {1:F0}\n", "Alt:", this.Altitude);
            outString.AppendFormat("{0:-10} {1:F0}\n", "Spd:", this.Speed);
            outString.AppendFormat("{0:-10} {1:F0}\n", "Hdg:", this.Rotation.Degrees);

            return outString.ToString();
        }

        public virtual string DetailedInfo()
        {
            StringBuilder outString = new StringBuilder();

            outString.AppendFormat("{0:-10} {1}\n", "URL:", this.ClickableActionURL);

            return outString.ToString();
        }

        public virtual string DescriptionInfo()
        {
            StringBuilder outString = new StringBuilder();

            outString.AppendFormat("{0:-10} {1}\n", "Desc:", this.Description);

            return outString.ToString();
        }

        public virtual string Degrees()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.AppendFormat("Lat: {0:00.00000}", this.Latitude); //, (this.Latitude>=0) ? "N":"S" );
            retStr.AppendFormat(" Lon: {0:000.00000}", this.Longitude); //, (this.Longitude>=0)? "E":"W");
            retStr.AppendFormat(" Alt: {0:F0}", this.Altitude);

            return retStr.ToString();
        }

        /// <summary>
        /// Returns a string with this object's position in Degrees Minutes Seconds format
        /// </summary>
        /// <returns>Lat and Lon in DMS and Alt in meters</returns>
        public virtual string DMS()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.AppendFormat("Lat: {0}", WidgetUtilities.Degrees2DMS(this.Latitude, 'N', 'S'));
            retStr.AppendFormat(" Lon: {0}", WidgetUtilities.Degrees2DMS(this.Longitude, 'E', 'W'));
            retStr.AppendFormat(" Alt: {0:F0}", this.Altitude);

            return retStr.ToString();
        }

        void IconGotoMenuItem_Click(object sender, EventArgs s)
        {
            this.m_gotoMe = true;
        }

        void IconHookMenuItem_Click(object sender, EventArgs s)
        {
            if (this.m_hookForm == null)
            {
                this.m_hookForm = new FormWidget(" " + this.Name);

                this.m_hookForm.WidgetSize = new System.Drawing.Size(200, 250);
                this.m_hookForm.Location = new System.Drawing.Point(200, 120);
                this.m_hookForm.DestroyOnClose = true;

                this.m_hookTreeNode = new SimpleTreeNodeWidget("Info");
                this.m_hookTreeNode.IsRadioButton = true;
                this.m_hookTreeNode.Expanded = true;
                this.m_hookTreeNode.EnableCheck = false;

                this.m_hookGeneralLabel = new LabelWidget("");
                this.m_hookGeneralLabel.ClearOnRender = true;
                this.m_hookGeneralLabel.Format = DrawTextFormat.WordBreak;
                this.m_hookGeneralLabel.Location = new System.Drawing.Point(0, 0);
                this.m_hookGeneralLabel.AutoSize = true;
                this.m_hookGeneralLabel.UseParentWidth = false;

                this.m_hookGeneralTreeNode = new SimpleTreeNodeWidget("General");
                this.m_hookGeneralTreeNode.IsRadioButton = true;
                this.m_hookGeneralTreeNode.Expanded = true;
                this.m_hookGeneralTreeNode.EnableCheck = false;

                this.m_hookGeneralTreeNode.Add(this.m_hookGeneralLabel);
                this.m_hookTreeNode.Add(this.m_hookGeneralTreeNode);

                this.m_hookDetailLabel = new LabelWidget("");
                this.m_hookDetailLabel.ClearOnRender = true;
                this.m_hookDetailLabel.Format = DrawTextFormat.WordBreak;
                this.m_hookDetailLabel.Location = new System.Drawing.Point(0, 0);
                this.m_hookDetailLabel.AutoSize = true;
                this.m_hookDetailLabel.UseParentWidth = false;

                this.m_hookDetailTreeNode = new SimpleTreeNodeWidget("Detail");
                this.m_hookDetailTreeNode.IsRadioButton = true;
                this.m_hookDetailTreeNode.Expanded = true;
                this.m_hookDetailTreeNode.EnableCheck = false;

                this.m_hookDetailTreeNode.Add(this.m_hookDetailLabel);
                this.m_hookTreeNode.Add(this.m_hookDetailTreeNode);

                this.m_hookDescTreeNode = new SimpleTreeNodeWidget("Description");
                this.m_hookDescTreeNode.IsRadioButton = true;
                this.m_hookDescTreeNode.Expanded = true;
                this.m_hookDescTreeNode.EnableCheck = false;

                this.m_hookDescLabel = new LabelWidget("");
                this.m_hookDescLabel.ClearOnRender = true;
                this.m_hookDescLabel.Format = DrawTextFormat.WordBreak;
                this.m_hookDescLabel.Location = new System.Drawing.Point(0, 0);
                this.m_hookDescLabel.AutoSize = true;
                this.m_hookDescLabel.UseParentWidth = true;

                this.m_hookDescTreeNode.Add(this.m_hookDescLabel);
                this.m_hookTreeNode.Add(this.m_hookDescTreeNode);

                this.m_hookForm.Add(this.m_hookTreeNode);

                DrawArgs.NewRootWidget.ChildWidgets.Add(this.m_hookForm);
            }

            this.UpdateHookForm();
            this.m_hookForm.Enabled = true;
            this.m_hookForm.Visible = true;
            this.IsHooked = true;
        }

        void IconURLMenuItem_Click(object sender, EventArgs s)
        {
            try
            {
                if (!this.ClickableActionURL.Contains(@"worldwind://"))
                {
                    if (World.Settings.UseInternalBrowser && this.ClickableActionURL.StartsWith("http"))
                    {
                        SplitContainer sc = (SplitContainer)DrawArgs.ParentControl.Parent.Parent;
                        InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                        browser.NavigateTo(this.ClickableActionURL);
                    }
                    else
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = this.ClickableActionURL;
                        psi.Verb = "open";
                        psi.UseShellExecute = true;

                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
	}
}
