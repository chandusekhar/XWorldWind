//
// Copyright � 2005 NASA. Available under the NOSA License
//
// Portions copied from JHU_Icons - Copyright � 2005-2006 The Johns Hopkins University 
// Applied Physics Laboratory.  Available under the JHU/APL Open Source Agreement.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind
{
    /// <summary>
    /// Holds a collection of icons
    /// </summary>
    public class Icons : RenderableObjectList
    {
        /// <summary>
        /// This is the texture for point sprites
        /// </summary>
        protected IconTexture m_pointTexture;

        protected List<PointSpriteVertex> pointSprites = new List<PointSpriteVertex>();

        protected bool m_canUsePointSprites = true;

        // The list of icons
        protected List<Icon> m_icons = new List<Icon>();
        protected ReaderWriterLock m_iconsRWLock = new ReaderWriterLock();

        /// <summary>
        /// Sprite for all icons in this layer
        /// </summary>
        protected Sprite m_sprite;

        protected bool m_mouseOver;

        protected static int hotColor = Color.White.ToArgb();
        protected static int normalColor = Color.FromArgb(150, 255, 255, 255).ToArgb();
        protected static int nameColor = Color.White.ToArgb();
        protected static int descriptionColor = Color.White.ToArgb();

        protected const int minIconZoomAltitude = 5000000;

        /// <summary>
        /// Earliest time an iconset is visible
        /// </summary>
        protected DateTime m_earlisettime = DateTime.MinValue;
        /// <summary>
        /// Latest time an iconset is visible
        /// </summary>
        protected DateTime m_latesttime = DateTime.MaxValue;

        System.Timers.Timer refreshTimer;

        /// <summary>
        /// This list holds all the rendered labels rectangles for declutter purposes
        /// </summary>
        protected List<Rectangle> m_labelRectangles = new List<Rectangle>();

        /// <summary>
        /// The closest icon the mouse is currently over
        /// </summary>
        protected Icon mouseOverIcon;

        /// <summary>
        /// Accessor Method for eartliest time
        /// </summary>
        public DateTime EarliestTime
        {
            get { return this.m_earlisettime; }
            set { this.m_earlisettime = value; }
        }

        /// <summary>
        /// Accessor method for latest time
        /// </summary>
        public DateTime LatestTime
        {
            get { return this.m_latesttime; }
            set { this.m_latesttime = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Icons"/> class 
        /// </summary>
        /// <param name="name">The name of the icons layer</param>
        public Icons(string name)
            : base(name)
        {
            this.m_mouseOver = true;
            this.isInitialized = false;
            this.isSelectable = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Icons"/> class 
        /// Sets up refresh of this layer from a data source.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataSource"></param>
        /// <param name="refreshInterval"></param>
        /// <param name="parentWorld"></param>
        /// <param name="cache"></param>
        public Icons(string name,
            string dataSource,
            TimeSpan refreshInterval,
            World parentWorld,
            Cache cache)
            : base(name, dataSource, refreshInterval, parentWorld, cache)
        {
            this.m_mouseOver = true;
            this.isInitialized = false;
            this.isSelectable = true;
        }

        /// <summary>
        /// Adds an icon to this layer.
        /// </summary>
        /// <param name="icon">The icon to add to this Icons layer.  Deprecateed.  Use Add(ro).</param>
        [Obsolete]
        public virtual void AddIcon(Icon icon)
        {
            this.Add(icon);
        }

        #region RenderableObject methods


        /// <summary>
        /// Add a child object to this layer.
        /// </summary>
        /// <param name="ro">The renderable object to add to this layer</param>
        public override void Add(RenderableObject ro)
        {
            ro.ParentList = this;
            base.Add(ro);
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (!this.isOn)
                return;

            TimeSpan smallestRefreshInterval = TimeSpan.MaxValue;

            if (!this.isInitialized)
            {
                if (this.m_sprite != null)
                {
                    this.m_sprite.Dispose();
                    this.m_sprite = null;
                }

                // init Icons layer
                this.m_sprite = new Sprite(drawArgs.device);

                string textureFilePath = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), @"Data\Icons\sol_wh.gif");

                this.m_pointTexture = new IconTexture(drawArgs.device, textureFilePath);

                            // if the device can't do point sprites don't use them
                if (drawArgs.device.Capabilities.MaxPointSize == 1)
                {
                    this.m_canUsePointSprites = false;
                }
                else
                {
                    this.m_canUsePointSprites = true;
                }
            }

            // figure out refresh period
            this.m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    Icon icon = ro as Icon;
                    if (icon != null)
                    {
                        if (icon.RefreshInterval.TotalMilliseconds != 0 && icon.RefreshInterval != TimeSpan.MaxValue && icon.RefreshInterval < smallestRefreshInterval)
                            smallestRefreshInterval = icon.RefreshInterval;
                    }
                }
            }
            finally
            {
                this.m_childrenRWLock.ReleaseReaderLock();
            }


            if (this.refreshTimer == null && smallestRefreshInterval != TimeSpan.MaxValue)
            {
                this.refreshTimer = new System.Timers.Timer(smallestRefreshInterval.TotalMilliseconds);
                this.refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.refreshTimer_Elapsed);
                this.refreshTimer.Start();
            }

            this.isInitialized = true;
        }

        public override void Dispose()
        {

            try
            {
                // call rol dispose to get all the children disposed first
                base.Dispose();

                // remove any textures no longer being used.
                if (DrawArgs.Textures != null)
                {
                    List<object> removeList = new List<object>();
                    foreach (object key in DrawArgs.Textures.Keys)
                    {
                        IconTexture iconTexture = (IconTexture)DrawArgs.Textures[key];
                        if (iconTexture.ReferenceCount <= 0)
                        {
                            removeList.Add(key);
                        }
                    }

                    foreach (object key in removeList)
                    {
                        IconTexture iconTexture = (IconTexture)DrawArgs.Textures[key];
                        DrawArgs.Textures.Remove(key);
                        iconTexture.Dispose();
                    }
                }

                if (this.m_sprite != null)
                {
                    this.m_sprite.Dispose();
                    this.m_sprite = null;
                }

                if (this.refreshTimer != null)
                {
                    this.refreshTimer.Stop();
                    this.refreshTimer.Dispose();
                    this.refreshTimer = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            int closestIconDistanceSquared = int.MaxValue;
            Icon closestIcon = null;

            //Respect icon set temporal extents
            if (TimeKeeper.CurrentTimeUtc < this.EarliestTime || TimeKeeper.CurrentTimeUtc > this.LatestTime)
            {
                return false;
            }

            this.m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    if (!ro.IsOn)
                        continue;
                    if (!ro.isSelectable)
                        continue;

                    Icon icon = ro as Icon;

                    // if it's not an icon just do the normal selection action
                    if (icon == null)
                    {
                        // Child is not an icon
                        if (ro.PerformSelectionAction(drawArgs))
                            return true;
                    }
                    else
                    {
                        // don't check if we aren't even in view
                        if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(icon.Position))
                        {

                            // check if inside the icons bounding box
                            Vector3 referenceCenter = new Vector3(
                                (float)drawArgs.WorldCamera.ReferenceCenter.X,
                                (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                                (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                            Vector3 projectedPoint = drawArgs.WorldCamera.Project(Vector3.Subtract(icon.Position, referenceCenter));

                            int dx = DrawArgs.LastMousePosition.X - (int)projectedPoint.X;
                            int dy = DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y;

                            if (icon.SelectionRectangle.Contains(dx, dy))
                            {
                                // Mouse is over, check whether this icon is closest
                                int distanceSquared = dx * dx + dy * dy;
                                if (distanceSquared < closestIconDistanceSquared)
                                {
                                    closestIconDistanceSquared = distanceSquared;
                                    closestIcon = icon;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                this.m_childrenRWLock.ReleaseReaderLock();
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
        }

        /// <summary>
        /// This used to be done in the World class but moved here so folks could override a ROL's behavior.
        /// 
        /// NOTE:  Everything under an Icons is rendered using RenderPriority.Icons.  If you put any other kind of 
        /// ROL in here it (and it's children) probably wont render.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="priority"></param>
        public override void RenderChildren(DrawArgs drawArgs, RenderPriority priority)
        {
            //Respect icon set temporal extents
            if (TimeKeeper.CurrentTimeUtc < this.EarliestTime || TimeKeeper.CurrentTimeUtc > this.LatestTime)
            {
                return;
            }

            if (!this.isOn)
                return;

            if (!this.isInitialized)
                return;

            if (priority != RenderPriority.Icons)
                return;

            // render ourselves
            this.Render(drawArgs);

            if (this.m_canUsePointSprites) this.pointSprites.Clear();

            // First render everything except icons - we need to do this twice since the other loop is INSIDE the
            // sprite.begin which can mess up the rendering of other ROs.  This is why prerender is in this loop.
            this.m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    if (!ro.IsOn)
                        continue;

                    if (ro is Icon) // && (priority == RenderPriority.Icons))
                    {
                        Icon icon = ro as Icon;
                        if (icon == null)
                            continue;

                        // do this once for both passes
                        icon.DistanceToIcon = Vector3.Length(icon.Position - drawArgs.WorldCamera.Position);

                        // do PreRender regardless of everything else
                        // note that mouseover actions happen one render cycle after mouse is over icon

                        icon.PreRender(drawArgs, (this.mouseOverIcon != null) && (icon == this.mouseOverIcon));
                    }
                    else if (ro is RenderableObjectList)
                    {
                        (ro as RenderableObjectList).RenderChildren(drawArgs, priority);
                    }
                    //// hack to render both surface images and terrain mapped images.
                    //else if (priority == RenderPriority.TerrainMappedImages)
                    //{
                    //    if (ro.RenderPriority == RenderPriority.SurfaceImages || ro.RenderPriority == RenderPriority.TerrainMappedImages)
                    //    {
                    //        ro.Render(drawArgs);
                    //    }
                    //}
                    else  // if (ro.RenderPriority == priority)
                    {
                        ro.Render(drawArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            finally
            {
                this.m_childrenRWLock.ReleaseReaderLock();
            }

            this.m_labelRectangles.Clear();

            int closestIconDistanceSquared = int.MaxValue;
            Icon closestIcon = null;

            if (priority == RenderPriority.Icons)
            {
                try
                {
                    this.m_sprite.Begin(SpriteFlags.AlphaBlend);

                    // Now render just the icons
                    this.m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
                    try
                    {
                        foreach (RenderableObject ro in this.m_children)
                        {
                            if (!ro.IsOn)
                                continue;

                            Icon icon = ro as Icon;
                            if (icon == null)
                                continue;

                            // don't try to render if we aren't in view or too far away
                            if ((drawArgs.WorldCamera.ViewFrustum.ContainsPoint(icon.Position)) &&
                                 (icon.DistanceToIcon <= icon.MaximumDisplayDistance) &&
                                 (icon.DistanceToIcon >= icon.MinimumDisplayDistance))
                            {
                                Vector3 translationVector = new Vector3(
                                (float)(icon.PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                                (float)(icon.PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                                (float)(icon.PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                                Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

                                // check if inside bounding box of icon
                                int dx = DrawArgs.LastMousePosition.X - (int)projectedPoint.X;
                                int dy = DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y;
                                if (icon.SelectionRectangle.Contains(dx, dy))
                                {
                                    // Mouse is over, check whether this icon is closest
                                    int distanceSquared = dx * dx + dy * dy;
                                    if (distanceSquared < closestIconDistanceSquared)
                                    {
                                        closestIconDistanceSquared = distanceSquared;
                                        closestIcon = icon;
                                    }
                                }

                                // mouseover is always one render cycle behind...we mark that an icon is the mouseover
                                // icon and render it normally.  On the NEXT pass it renders as a mouseover icon

                                if (icon != this.mouseOverIcon)
                                {
                                    // Note: Always render hooked icons as a real icon rather than a sprite.
                                    if (icon.UsePointSprite && (icon.DistanceToIcon > icon.PointSpriteDistance) && this.m_canUsePointSprites && !icon.IsHooked)
                                    {
                                        PointSpriteVertex pv = new PointSpriteVertex(translationVector.X, translationVector.Y, translationVector.Z, icon.PointSpriteSize, icon.PointSpriteColor.ToArgb());
                                        this.pointSprites.Add(pv);
                                    }
                                    else
                                    {
                                        // add pointsprite anyway
                                        if (icon.UsePointSprite && icon.AlwaysRenderPointSprite)
                                        {
                                            PointSpriteVertex pv = new PointSpriteVertex(translationVector.X, translationVector.Y, translationVector.Z, icon.PointSpriteSize, icon.PointSpriteColor.ToArgb());
                                            this.pointSprites.Add(pv);
                                        }

                                        icon.FastRender(drawArgs, this.m_sprite, projectedPoint, false, this.m_labelRectangles);
                                    }
                                }
                            }
                            else
                            {
                                // do whatever we're supposed to do if we're not in view or too far away
                                icon.NoRender(drawArgs);
                            }

                            // do post rendering even if we don't render
                            // note that mouseover actions happen one render cycle after mouse is over icon
                            icon.PostRender(drawArgs, icon == this.mouseOverIcon);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                    finally
                    {
                        this.m_childrenRWLock.ReleaseReaderLock();
                    }

                    // Clear the rectangles so that mouseover label always appears
                    this.m_labelRectangles.Clear();

                    // Render the mouse over icon last (on top)
                    if (this.mouseOverIcon != null)
                    {
                        Vector3 translationVector = new Vector3(
                            (float)(this.mouseOverIcon.PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                            (float)(this.mouseOverIcon.PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                            (float)(this.mouseOverIcon.PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                        Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

                        if (this.mouseOverIcon.UsePointSprite && this.mouseOverIcon.AlwaysRenderPointSprite && this.m_canUsePointSprites)
                        {
                            // add pointsprite anyway
                            PointSpriteVertex pv = new PointSpriteVertex(translationVector.X, translationVector.Y, translationVector.Z, this.mouseOverIcon.PointSpriteSize, this.mouseOverIcon.PointSpriteColor.ToArgb());
                            this.pointSprites.Add(pv);
                        }

                        this.mouseOverIcon.FastRender(drawArgs, this.m_sprite, projectedPoint, true, this.m_labelRectangles);
                    }

                    // set new mouseover icon
                    this.mouseOverIcon = closestIcon;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                finally
                {
                    this.m_sprite.End();
                }

                // render point sprites if any in the list
                try
                {
                    if (this.pointSprites.Count > 0)
                    {
                        // save device state
                        Texture origTexture = drawArgs.device.GetTexture(0);
                        VertexFormat origVertexFormat = drawArgs.device.VertexFormat;
                        float origPointScaleA , drawArgs.device.SetRenderState(RenderState.PointScaleA);
                        float origPointScaleB , drawArgs.device.SetRenderState(RenderState.PointScaleB);
                        float origPointScaleC , drawArgs.device.SetRenderState(RenderState.PointScaleC);
                        bool origPointSpriteEnable , drawArgs.device.SetRenderState(RenderState.PointSpriteEnable);
                        bool origPointScaleEnable , drawArgs.device.SetRenderState(RenderState.PointScaleEnable);
                        Blend origSourceBlend , drawArgs.device.SetRenderState(RenderState.SourceBlend);
                        Blend origDestBlend , drawArgs.device.SetRenderState(RenderState.DestinationBlend);

                        // set device to do point sprites
                        drawArgs.device.SetTexture(0, this.m_pointTexture.Texture);
                        drawArgs.device.VertexFormat = VertexFormat.Position | VertexFormat.PointSize | VertexFormat.Diffuse;
                        drawArgs.device.SetRenderState(RenderState.PointScaleA, 1f);
                        drawArgs.device.SetRenderState(RenderState.PointScaleB, 0f);
                        drawArgs.device.SetRenderState(RenderState.PointScaleC, 0f);
                        //drawArgs.device.SetRenderState(RenderState.PointScaleA , 0f);
                        //drawArgs.device.SetRenderState(RenderState.PointScaleB , 0f);
                        //drawArgs.device.SetRenderState(RenderState.PointScaleC , .0000000000001f);
                        drawArgs.device.SetRenderState(RenderState.PointSpriteEnable, true);
                        drawArgs.device.SetRenderState(RenderState.PointScaleEnable, true);
                        drawArgs.device.SetRenderState(RenderState.SourceBlend, Blend.One);
                        drawArgs.device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, (int)TextureOperation.Modulate);
                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, (int)TextureArgument.Texture);
                        drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg2, (int)TextureArgument.Diffuse);

                        drawArgs.device.DrawUserPrimitives(PrimitiveType.PointList, this.pointSprites.Count, this.pointSprites.ToArray());

                        // restore device state
                        drawArgs.device.SetTexture(0, origTexture);
                        drawArgs.device.VertexFormat = origVertexFormat;
                        drawArgs.device.SetRenderState(RenderState.PointScaleA, , origPointScaleA);
                        drawArgs.device.SetRenderState(RenderState.PointScaleB, , origPointScaleB);
                        drawArgs.device.SetRenderState(RenderState.PointScaleC, , origPointScaleC);
                        drawArgs.device.SetRenderState(RenderState.PointSpriteEnable, , origPointSpriteEnable);
                        drawArgs.device.SetRenderState(RenderState.PointScaleEnable, , origPointScaleEnable);
                        drawArgs.device.SetRenderState(RenderState.SourceBlend, , origSourceBlend);
                        drawArgs.device.SetRenderState(RenderState.DestinationBlend, , origDestBlend);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        #endregion

        /// <summary>
        /// Draw the icon
        /// </summary>
        //[Obsolete]
        //protected virtual void Render(DrawArgs drawArgs, Icon icon, Vector3 projectedPoint)
        //{
        //    if (!icon.isInitialized)
        //        icon.Initialize(drawArgs);

        //    if (!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(icon.Position))
        //        return;

        //    // Check icons for within "visual" range
        //    double distanceToIcon = Vector3.Length(icon.Position - drawArgs.WorldCamera.Position);
        //    if (distanceToIcon > icon.MaximumDisplayDistance)
        //        return;
        //    if (distanceToIcon < icon.MinimumDisplayDistance)
        //        return;

        //    //Respect icon set temporal extents
        //    if (TimeKeeper.CurrentTimeUtc < EarliestTime || TimeKeeper.CurrentTimeUtc > LatestTime)
        //        return;

        //    IconTexture iconTexture = GetTexture(icon);
        //    bool isMouseOver = icon == mouseOverIcon;

        //    //Show description for normal Icons at the bottom
        //    //of the page
        //    if (isMouseOver) // && !icon.IsKMLIcon)
        //    {
        //        // Mouse is over
        //        isMouseOver = true;

        //        if (icon.isSelectable)
        //            DrawArgs.MouseCursor = CursorType.Hand;

        //        string description = icon.Description;
        //        if (description == null)
        //            description = icon.ClickableActionURL;
        //        if (description != null)
        //        {
        //            // Render description field
        //            DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Bottom;
        //            int left = 10;
        //            if (World.Settings.showLayerManager)
        //                left += World.Settings.layerManagerWidth;
        //            Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10);

        //            // Draw outline
        //            drawArgs.defaultDrawingFont.DrawText(
        //                m_sprite, description,
        //                rect,
        //                format, 0xb0 << 24);

        //            rect.Offset(2, 0);
        //            drawArgs.defaultDrawingFont.DrawText(
        //                m_sprite, description,
        //                rect,
        //                format, 0xb0 << 24);

        //            rect.Offset(0, 2);
        //            drawArgs.defaultDrawingFont.DrawText(
        //                m_sprite, description,
        //                rect,
        //                format, 0xb0 << 24);

        //            rect.Offset(-2, 0);
        //            drawArgs.defaultDrawingFont.DrawText(
        //                m_sprite, description,
        //                rect,
        //                format, 0xb0 << 24);

        //            // Draw description
        //            rect.Offset(1, -1);
        //            drawArgs.defaultDrawingFont.DrawText(
        //                m_sprite, description,
        //                rect,
        //                format, descriptionColor);
        //        }
        //    }

        //    int color = isMouseOver ? hotColor : normalColor;
        //    if (iconTexture == null || isMouseOver || icon.NameAlwaysVisible)
        //    {
        //        // Render label
        //        if (icon.Name != null)
        //        {
        //            // Render name field
        //            const int labelWidth = 1000; // Dummy value needed for centering the text
        //            if (iconTexture == null)
        //            {
        //                // Center over target as we have no bitmap
        //                Rectangle rect = new Rectangle(
        //                    (int)projectedPoint.X - (labelWidth >> 1),
        //                    (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
        //                    labelWidth,
        //                    drawArgs.screenHeight);

        //                drawArgs.defaultDrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.Center, color);
        //            }
        //            else
        //            {
        //                // Adjust text to make room for icon
        //                int spacing = (int)(icon.Width * 0.3f);
        //                if (spacing > 10)
        //                    spacing = 10;
        //                int offsetForIcon = (icon.Width >> 1) + spacing;

        //                Rectangle rect = new Rectangle(
        //                    (int)projectedPoint.X + offsetForIcon,
        //                    (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
        //                    labelWidth,
        //                    drawArgs.screenHeight);

        //                drawArgs.defaultDrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.WordBreak, color);
        //            }
        //        }
        //    }

        //    if (iconTexture != null)
        //    {
        //        // Render icon
        //        float factor = 1;
        //        //Do Altitude depedent scaling for KMLIcons
        //        //if(icon.IsKMLIcon)
        //        //    if (drawArgs.WorldCamera.Altitude > MinIconZoomDistance)
        //        //        factor -= (float)((drawArgs.WorldCamera.Altitude - MinIconZoomDistance) / drawArgs.WorldCamera.Altitude);

        //        float xscale = factor * ((float)icon.Width / iconTexture.Width);
        //        float yscale = factor * ((float)icon.Height / iconTexture.Height);
        //        m_sprite.Transform = Matrix.Scaling(xscale, yscale, 0);

        //        if (icon.IsRotated)
        //            m_sprite.Transform *= Matrix.RotationZ((float)icon.Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);

        //        m_sprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
        //        m_sprite.Draw(iconTexture.Texture,
        //            new Vector3(iconTexture.Width >> 1, iconTexture.Height >> 1, 0),
        //            Vector3.Zero,
        //            color);

        //        // Reset transform to prepare for text rendering later
        //        m_sprite.Transform = Matrix.Identity;
        //    }
        //}

        /// <summary>
        /// Retrieve an icon's texture
        /// </summary>
        protected IconTexture GetTexture(Icon icon)
        {
            object key = null;

            if (icon.Image == null)
            {
                key = icon.TextureFileName;
            }
            else
            {
                key = icon.Image;
            }
            if (key == null)
                return null;

            IconTexture res = (IconTexture)DrawArgs.Textures[key];
            return res;
        }

        bool isUpdating;
        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.isUpdating)
                return;
            this.isUpdating = true;
            try
            {
                for (int i = 0; i < this.ChildObjects.Count; i++)
                {
                    RenderableObject ro = (RenderableObject)this.ChildObjects[i];
                    if (ro != null && ro.IsOn && ro is Icon)
                    {
                        Icon icon = (Icon)ro;

                        if (icon.RefreshInterval == TimeSpan.MaxValue || icon.LastRefresh > DateTime.Now - icon.RefreshInterval)
                            continue;

                        object key = null;
                        IconTexture iconTexture = null;

                        if (icon.TextureFileName != null && icon.TextureFileName.Length > 0)
                        {
                            iconTexture = (IconTexture)DrawArgs.Textures[icon.TextureFileName];
                            if (iconTexture != null)
                            {
                                iconTexture.UpdateTexture(DrawArgs.Device, icon.TextureFileName);
                            }
                            else
                            {
                                key = icon.TextureFileName;
                                iconTexture = new IconTexture(DrawArgs.Device, icon.TextureFileName);

                                iconTexture.ReferenceCount++;

                                // New texture, cache it
                                DrawArgs.Textures.Add(key, iconTexture);

                                // Use default dimensions if not set
                                if (icon.Width == 0)
                                    icon.Width = iconTexture.Width;
                                if (icon.Height == 0)
                                    icon.Height = iconTexture.Height;
                            }

                        }
                        else
                        {
                            // Icon image from bitmap
                            if (icon.Image != null)
                            {
                                iconTexture = (IconTexture)DrawArgs.Textures[icon.Image];
                                if (iconTexture != null)
                                {
                                    IconTexture tempTexture = iconTexture;
                                    DrawArgs.Textures[icon.SaveFilePath] = new IconTexture(DrawArgs.Device, icon.Image);
                                    tempTexture.Dispose();
                                }
                                else
                                {
                                    key = icon.SaveFilePath;
                                    iconTexture = new IconTexture(DrawArgs.Device, icon.Image);

                                    // New texture, cache it
                                    DrawArgs.Textures.Add(key, iconTexture);

                                    // Use default dimensions if not set
                                    if (icon.Width == 0)
                                        icon.Width = iconTexture.Width;
                                    if (icon.Height == 0)
                                        icon.Height = iconTexture.Height;
                                }
                            }
                        }

                        icon.LastRefresh = DateTime.Now;
                    }
                }
            }
            catch { }
            finally
            {
                this.isUpdating = false;
            }
        }
    }
}
