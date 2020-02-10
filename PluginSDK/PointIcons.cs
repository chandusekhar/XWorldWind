//========================= (UNCLASSIFIED) ==============================
// Copyright � 2007 The Johns Hopkins University /
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
// Physics Laboratory (�JHU/APL�) that is the author thereof under the 
// �work made for hire� provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the �Software�), to use the 
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
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind
{
	/// <summary>
	/// Holds a collection of icons
	/// </summary>
	public class PointIcons : RenderableObject
	{
        public class PointIcon
        {
            public string Name;
            public string Description;

            public double Lat
            {
                get { return this.m_lat; }
                set 
                {
                    this.m_lat = value;
                    this.Update();
                }
            }
            private double m_lat;

            public double Lon
            {
                get { return this.m_lon; }
                set 
                {
                    this.m_lon = value;
                    this.Update();
                }
            }
            private double m_lon;

            public double Alt
            {
                get { return this.m_alt; }
                set 
                {
                    this.m_alt = value;
                    this.Update();
                }
            }
            private double m_alt;

            public float Size;
            public Color Color;
            public string Url;

            /// <summary>
            /// The cartesian coordinates of this icon.  
            /// Used to be settable but never actually updated the position of the icon.
            /// </summary>
            public Point3d PositionD
            {
                get { return this.m_positionD; }
            }
            private Point3d m_positionD = new Point3d();

            /// <summary>
            /// Object position (XYZ world coordinates)
            /// </summary>
            public virtual Vector3 Position
            {
                get
                {
                    return this.m_position;
                }
                set
                {
                    this.m_position = value;
                }
            }
            private Vector3 m_position = new Vector3();

            /// <summary>
            /// If LMB pressed calls PerformLMBAction, if RMB pressed calls PerformRMBAction
            /// </summary>
            /// <param name="drawArgs"></param>
            /// <returns></returns>
            public bool PerformSelectionAction(DrawArgs drawArgs)
            {
                if (DrawArgs.IsLeftMouseButtonDown)
                    return this.PerformLMBAction(drawArgs);

                if (DrawArgs.IsRightMouseButtonDown)
                    return this.PerformRMBAction(drawArgs);

                return false;
            }

            /// <summary>
            /// Goes to icon if camera positions set.  Also opens URL if it exists
            /// </summary>
            /// <param name="drawArgs"></param>
            /// <returns></returns>
            protected virtual bool PerformLMBAction(DrawArgs drawArgs)
            {
                try
                {
                    // Goto to URL if we have one
                    if (this.Url != null && !this.Url.Contains(@"worldwind://"))
                    {
                        if (World.Settings.UseInternalBrowser && this.Url.StartsWith("http"))
                        {
                            SplitContainer sc = (SplitContainer)drawArgs.parentControl.Parent.Parent;
                            InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                            browser.NavigateTo(this.Url);
                        }
                        else
                        {
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = this.Url;
                            psi.Verb = "open";
                            psi.UseShellExecute = true;

                            psi.CreateNoWindow = true;
                            Process.Start(psi);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                return false;
            }

            /// <summary>
            /// RMB Click Action
            /// </summary>
            /// <param name="drawArgs"></param>
            /// <returns></returns>
            protected virtual bool PerformRMBAction(DrawArgs drawArgs)
            {
                return false;
            }

            private void Update()
            {
                double altitude;

                // Altitude is ASL
                altitude = (World.Settings.VerticalExaggeration * this.Alt) + DrawArgs.Camera.WorldRadius;

                this.Position = MathEngine.SphericalToCartesian(this.Lat, this.Lon, altitude);

                this.m_positionD = MathEngine.SphericalToCartesianD(
                    Angle.FromDegrees(this.Lat),
                    Angle.FromDegrees(this.Lon),
                    altitude);
            }
        }

        /// <summary>
        /// This is the texture for point sprites
        /// </summary>
        protected IconTexture m_pointTexture;

        protected List<PointSpriteVertex> m_pointSprites = new List<PointSpriteVertex>();

        protected Dictionary<string, PointIcon> m_points = new Dictionary<string, PointIcon>(); 

		protected static int nameColor = Color.White.ToArgb();
		protected static int descriptionColor = Color.White.ToArgb();

        /// <summary>
        /// Bounding box centered at (0,0) used to calculate whether mouse is over point
        /// </summary>
        public Rectangle SelectionRectangle;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Icons"/> class 
		/// </summary>
		/// <param name="name">The name of the icons layer</param>
		public PointIcons(string name) : base(name) 
		{
            this.isInitialized = false;
		}

		#region RenderableObject methods

        public override void Update(DrawArgs drawArgs)
        {
        }

		public override void Initialize(DrawArgs drawArgs)
		{
			if(!this.isOn)
				return;

            if (!this.isInitialized)
            {
                string textureFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Data\Icons\sol_wh.gif");

                this.m_pointTexture = new IconTexture(drawArgs.device, textureFilePath);

                // This could be off since the texture is scaled
                this.SelectionRectangle = new Rectangle(0, 0, this.m_pointTexture.Width, this.m_pointTexture.Height);
                
                // Center the box at (0,0)
                this.SelectionRectangle.Offset(-this.SelectionRectangle.Width / 2, -this.SelectionRectangle.Height / 2);
            }

            this.isInitialized = true;
		}


		public override void Dispose()
		{
            try
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            int closestIconDistanceSquared = int.MaxValue;
            PointIcon closestIcon = null;

            foreach (PointIcon point in this.m_points.Values)
            {

                // don't check if we aren't even in view
                if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(point.Position))
                {

                    // check if inside the icons bounding box
                    Vector3 referenceCenter = new Vector3(
                        (float)drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                    Vector3 projectedPoint = drawArgs.WorldCamera.Project(point.Position - referenceCenter);

                    int dx = DrawArgs.LastMousePosition.X - (int)projectedPoint.X;
                    int dy = DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y;

                    if (this.SelectionRectangle.Contains(dx, dy))
                    {
                        // Mouse is over, check whether this icon is closest
                        int distanceSquared = dx * dx + dy * dy;
                        if (distanceSquared < closestIconDistanceSquared)
                        {
                            closestIconDistanceSquared = distanceSquared;
                            closestIcon = point;
                        }
                    }
                }
            }

            // if no other object has handled the selection let the closest icon try
            if (closestIcon != null)
            {
                return closestIcon.PerformSelectionAction(drawArgs);
            }

			return false;
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(!this.isOn)
				return;

			if(!this.isInitialized) this.Initialize(drawArgs);

            this.m_pointSprites.Clear();

            int closestIconDistanceSquared = int.MaxValue;
            PointIcon closestIcon = null;

            // build list of all points in view
            foreach (PointIcon point in this.m_points.Values)
            {
                try
                {
                    // don't bother to do anything else if we aren't even in view
                    if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(point.Position))
                    {
                        Vector3 translationVector = new Vector3(
                        (float)(point.PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                        (float)(point.PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                        (float)(point.PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                        Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

                        // check if inside bounding box of icon
                        int dx = DrawArgs.LastMousePosition.X - (int)projectedPoint.X;
                        int dy = DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y;
                        if (this.SelectionRectangle.Contains(dx, dy))
                        {
                            // Mouse is over, check whether this icon is closest
                            int distanceSquared = dx * dx + dy * dy;
                            if (distanceSquared < closestIconDistanceSquared)
                            {
                                closestIconDistanceSquared = distanceSquared;
                                closestIcon = point;
                            }
                        }

                        PointSpriteVertex pv = new PointSpriteVertex(translationVector.X, translationVector.Y, translationVector.Z, point.Size, point.Color.ToArgb());
                        this.m_pointSprites.Add(pv);
                    }
                }
                catch 
                {
                }
                finally
                {
                }
            }

            // render point sprites if any in the list
            try
            {
                if (this.m_pointSprites.Count > 0)
                {
                    // save device state
                    BaseTexture origTexture = drawArgs.device.GetTexture(0);
                    VertexFormat origVertexFormat = drawArgs.device.VertexFormat;
                    float origPointScaleA = drawArgs.device.GetRenderState<float>(RenderState.PointScaleA);
                    float origPointScaleB = drawArgs.device.GetRenderState<float>(RenderState.PointScaleB);
                    float origPointScaleC = drawArgs.device.GetRenderState<float>(RenderState.PointScaleC);
                    bool origPointSpriteEnable = drawArgs.device.GetRenderState<bool>(RenderState.PointSpriteEnable);
                    bool origPointScaleEnable = drawArgs.device.GetRenderState<bool>(RenderState.PointScaleEnable);
                    Blend origSourceBlend = drawArgs.device.GetRenderState<Blend>(RenderState.SourceBlend);
                    Blend origDestBlend = drawArgs.device.GetRenderState<Blend>(RenderState.DestinationBlend);

                    // set device to do point sprites
                    drawArgs.device.SetTexture(0, this.m_pointTexture.Texture);
                    drawArgs.device.VertexFormat = VertexFormat.Position | VertexFormat.PointSize | VertexFormat.Diffuse;
                    drawArgs.device.SetRenderState(RenderState.PointScaleA , 1f);
                    drawArgs.device.SetRenderState(RenderState.PointScaleB , 0f);
                    drawArgs.device.SetRenderState(RenderState.PointScaleC , 0f);
                    drawArgs.device.SetRenderState(RenderState.PointSpriteEnable , true);
                    drawArgs.device.SetRenderState(RenderState.PointScaleEnable , true);

                    drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, (int)TextureOperation.Modulate);
                    drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, (int)TextureArgument.Texture);
                    drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg2, (int)TextureArgument.Diffuse);

                    // Draw all visible points
                    drawArgs.device.DrawUserPrimitives(PrimitiveType.PointList, this.m_pointSprites.Count, this.m_pointSprites.ToArray());    

                    // Draw label and description of mouseover point
                    if (closestIcon != null)
                    {
                    }

                    // restore device state
                    drawArgs.device.SetTexture(0, origTexture);
                    drawArgs.device.VertexFormat = origVertexFormat;
                    drawArgs.device.SetRenderState(RenderState.PointScaleA , origPointScaleA);
                    drawArgs.device.SetRenderState(RenderState.PointScaleB , origPointScaleB);
                    drawArgs.device.SetRenderState(RenderState.PointScaleC , origPointScaleC);
                    drawArgs.device.SetRenderState(RenderState.PointSpriteEnable , origPointSpriteEnable);
                    drawArgs.device.SetRenderState(RenderState.PointScaleEnable , origPointScaleEnable);
                    drawArgs.device.SetRenderState(RenderState.SourceBlend , origSourceBlend);
                    drawArgs.device.SetRenderState(RenderState.DestinationBlend , origDestBlend);
                }
            }
            catch
            {
            }
		}

		#endregion

        /// <summary>
        /// Add a new fast icon point.  Overwrites any existing fast icon point with the same name. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="url"></param>
        public void AddPoint(string name, string description, double lat, double lon, double alt, float size, Color color, string url)
        {
            if (this.m_points.ContainsKey(name)) this.m_points.Remove(name);

        }

        /// <summary>
        /// Adds the provided point.  Overwrites any existing point with the same key.
        /// </summary>
        /// <param name="point">PointIcon to add.</param>
        public void AddPoint(PointIcon point)
        {
            if (this.m_points.ContainsKey(point.Name)) this.m_points.Remove(point.Name);

            this.m_points.Add(point.Name, point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        public void UpdatePoint(string name, double lat, double lon, double alt)
        {
            if (this.m_points.ContainsKey(name))
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="alt"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="url"></param>
        public void UpdatePoint(string name, string description, double lat, double lon, double alt, float size, Color color, string url)
        {
            if (this.m_points.ContainsKey(name))
            {
            }
        }

        /// <summary>
        /// Changes the point color
        /// </summary>
        /// <param name="name">name of point</param>
        /// <param name="color">new color for this point</param>
        public void UpdatePointColor(string name, Color color)
        {
            if (this.m_points.ContainsKey(name))
            {
                this.m_points[name].Color = color;
            }
        }

        /// <summary>
        /// Changes the rendered scaling size for this point
        /// </summary>
        /// <param name="name">name of point</param>
        /// <param name="size">new scaling size</param>
        public void UpdatePointSize(string name, float size)
        {
            if (this.m_points.ContainsKey(name))
            {
                this.m_points[name].Size = size;
            }
        }

        /// <summary>
        /// Returns the point if it exists
        /// </summary>
        /// <param name="name">Name of point</param>
        /// <returns>Point found or null if not</returns>
        public PointIcon GetPoint(string name)
        {
            return this.m_points[name];
        }

        /// <summary>
        /// Removes point if it exists
        /// </summary>
        /// <param name="name">name of point to remove</param>
        public void RemovePoint(string name)
        {
            if (this.m_points.ContainsKey(name)) this.m_points.Remove(name);
        }
	}
}
