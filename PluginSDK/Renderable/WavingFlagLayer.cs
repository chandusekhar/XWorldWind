using System;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;

namespace WorldWind.Renderable
{
    public class WavingFlagLayer : RenderableObject
    {
        public event EventHandler OnMouseEnterEvent;
        public event EventHandler OnMouseLeaveEvent;
        public event System.Windows.Forms.MouseEventHandler OnMouseUpEvent;

        double m_latitude, m_longitude;
        string m_imageUri;
        PolygonFeature m_polygonFeature;

        Texture m_texture;
        float m_angle;
        string highlightTexturePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Data\\ring.dds";

        // share these across all flag objects
        static Texture HighlightTexture;
        static CustomVertex.PositionColoredTextured[] m_highlightVertices;
        static CustomVertex.PositionNormalTextured[] m_vertices;
        static VertexDeclaration m_vertexDeclaration;
        static CustomVertex.PositionColored[] m_flagPoleVertices;
        static short[] m_flagPoleIndices;
        static short[] m_outlineFlagPoleIndices;
        static CustomVertex.PositionColored[] m_outlineFlagPoleVertices;
        static VertexBuffer m_vertexBuffer;
        static IndexBuffer m_indexBuffer;
        static short[] m_indices;
        static Effect m_effect;

        public string SavedImagePath = null;
        public bool ShowHighlight = false;
        public float Attentuation = 0.3f;
        public Bar3D Bar3D = null;

        public float ScaleX = 1;
        public float ScaleY = 1;
        public float ScaleZ = 1;


        public WavingFlagLayer(
            string name,
            World parentWorld,
            double latitude,
            double longitude,
            string imageUri)
            : base(name, parentWorld)
        {
            this.m_latitude = latitude;
            this.m_longitude = longitude;
            this.m_imageUri = imageUri;
            this.isSelectable = true;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (this.m_polygonFeature == null)
            {
                double offset = 0.02;
                Point3d[] points = new Point3d[4];

                points[0] = new Point3d(this.m_longitude - offset, this.m_latitude - offset,
                    200000);

                points[1] = new Point3d(this.m_longitude - offset, this.m_latitude + offset,
                    200000);

                points[2] = new Point3d(this.m_longitude + offset, this.m_latitude + offset,
                    200000);

                points[3] = new Point3d(this.m_longitude + offset, this.m_latitude - offset,
                    200000);

                LinearRing outerRing = new LinearRing();
                outerRing.Points = points;

                this.m_polygonFeature = new PolygonFeature(this.name, this.World,
                    outerRing,
                    null,
                    System.Drawing.Color.Chocolate);

                this.m_polygonFeature.AltitudeMode = AltitudeMode.Absolute;
                this.m_polygonFeature.Extrude = true;
                this.m_polygonFeature.Outline = true;
                this.m_polygonFeature.OutlineColor = System.Drawing.Color.Chocolate;
            }

            FileInfo savedFlagFile = new FileInfo(this.SavedImagePath);
            FileInfo placeHolderFile = new FileInfo(this.SavedImagePath + ".blk");

            if (savedFlagFile.Exists)
            {
                try
                {
                    this.m_texture = ImageHelper.LoadTexture(
                        savedFlagFile.FullName,
                        System.Drawing.Color.Black.ToArgb());
                }
                catch
                {
                    savedFlagFile.Delete();
                    savedFlagFile.Refresh();
                }
            }

            if (!savedFlagFile.Exists && !placeHolderFile.Exists)
            {
                if (!savedFlagFile.Directory.Exists)
                    savedFlagFile.Directory.Create();
                try
                {
                    // Offline check
                    if (World.Settings.WorkOffline)
                        throw new Exception("Offline mode active.");
                    
                    Net.WebDownload download = new Net.WebDownload(this.m_imageUri);
                    download.DownloadFile(savedFlagFile.FullName);
                    download.Dispose();
                    savedFlagFile.Refresh();
                }
                catch
                {
                    FileStream fs = placeHolderFile.Create();
                    fs.Close();
                    fs = null;
                    placeHolderFile.Refresh();
                }

                if (savedFlagFile.Exists)
                {
                    this.m_texture = ImageHelper.LoadTexture(
                        savedFlagFile.FullName,
                        System.Drawing.Color.Black.ToArgb());
                }
            }

            if (m_vertexDeclaration == null)
            {
                VertexElement[] velements = new VertexElement[]
                {
                    new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                    new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                    VertexElement.VertexDeclarationEnd
                };
                m_vertexDeclaration = new VertexDeclaration(drawArgs.device, velements);
            }

            this.UpdateVertices();
            this.isInitialized = true;
        }

        public override void Dispose()
        {
            if (this.m_texture != null && !this.m_texture.Disposed)
            {
                this.m_texture.Dispose();
                this.m_texture = null;
            }
        }

        private void UpdateVertices()
        {
            if (m_vertices != null)
                return;

            int vertexWidth = 32;
            int vertexHeight = 32;

            m_vertices = new CustomVertex.PositionNormalTextured[(vertexWidth + 1) * (vertexHeight + 1)];

            for (int y = 0; y <= vertexHeight; y++)
            {
                for (int x = 0; x <= vertexWidth; x++)
                {
                    m_vertices[y * (vertexWidth + 1) + x].X = (float)x / (float)vertexWidth;
                    m_vertices[y * (vertexWidth + 1) + x].Y = (float)y / (float)vertexHeight;
                    m_vertices[y * (vertexWidth + 1) + x].Z = 0;
                    m_vertices[y * (vertexWidth + 1) + x].Nx = 0;
                    m_vertices[y * (vertexWidth + 1) + x].Ny = 0;
                    m_vertices[y * (vertexWidth + 1) + x].Nz = 1;
                    m_vertices[y * (vertexWidth + 1) + x].Z *= m_vertices[y * (vertexWidth + 1) + x].Y * 0.09f;
                    m_vertices[y * (vertexWidth + 1) + x].Tu = m_vertices[y * (vertexWidth + 1) + x].Y;
                    m_vertices[y * (vertexWidth + 1) + x].Tv = 1.0f - m_vertices[y * (vertexWidth + 1) + x].X;
                }
            }
            m_indices = new short[2 * vertexWidth * vertexHeight * 3];
            int baseIndex = 0;
            for (int i = 0; i < vertexHeight; i++)
            {
                baseIndex = (2 * 3 * i * vertexWidth);

                for (int j = 0; j < vertexWidth; j++)
                {
                    m_indices[baseIndex] = (short)(i * (vertexWidth + 1) + j);
                    m_indices[baseIndex + 1] = (short)((i + 1) * (vertexWidth + 1) + j);
                    m_indices[baseIndex + 2] = (short)(i * (vertexWidth + 1) + j + 1);
                    m_indices[baseIndex + 3] = (short)(i * (vertexWidth + 1) + j + 1);
                    m_indices[baseIndex + 4] = (short)((i + 1) * (vertexWidth + 1) + j);
                    m_indices[baseIndex + 5] = (short)((i + 1) * (vertexWidth + 1) + j + 1);

                    baseIndex += 6;
                }
            }
        }

        public override void Update(DrawArgs drawArgs)
        {
            try
            {
                if (!this.isInitialized) this.Initialize(drawArgs);

                if (this.m_polygonFeature != null) this.m_polygonFeature.Update(drawArgs);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void renderHighlight(DrawArgs drawArgs)
        {
            bool lighting , drawArgs.device.SetRenderState(RenderState.Lighting);
            drawArgs.device.SetRenderState(RenderState.Lighting , false);

            if (m_highlightVertices == null)
            {
                m_highlightVertices = new CustomVertex.PositionColoredTextured[4];

                m_highlightVertices[0].X = -0.5f;
                m_highlightVertices[0].Y = 0.5f;
                m_highlightVertices[0].Z = 0.0f;
                m_highlightVertices[0].Tu = 0.0f;
                m_highlightVertices[0].Tv = 0.0f;

                m_highlightVertices[1].X = -0.5f;
                m_highlightVertices[1].Y = -0.5f;
                m_highlightVertices[1].Z = 0.0f;
                m_highlightVertices[1].Tu = 0.0f;
                m_highlightVertices[1].Tv = 1.0f;

                m_highlightVertices[2].X = 0.5f;
                m_highlightVertices[2].Y = 0.5f;
                m_highlightVertices[2].Z = 0.0f;
                m_highlightVertices[2].Tu = 1.0f;
                m_highlightVertices[2].Tv = 0.0f;

                m_highlightVertices[3].X = 0.5f;
                m_highlightVertices[3].Y = -0.5f;
                m_highlightVertices[3].Z = 0.0f;
                m_highlightVertices[3].Tu = 1.0f;
                m_highlightVertices[3].Tv = 1.0f;
            }

            if (HighlightTexture == null)
                HighlightTexture = ImageHelper.LoadTexture(this.highlightTexturePath);

            drawArgs.device.SetTransform(TransformState.World, Matrix.RotationY((float)MathEngine.DegreesToRadians(90));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.Scaling(World.Settings.VerticalExaggeration * this.ScaleX,
                World.Settings.VerticalExaggeration * this.ScaleY, World.Settings.VerticalExaggeration * this.ScaleZ);
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(this.m_latitude));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.m_longitude));

            Vector3 surfacePos = MathEngine.SphericalToCartesian(this.m_latitude, this.m_longitude, this.World.EquatorialRadius);

            Vector3 rc = new Vector3(
                (float)drawArgs.WorldCamera.ReferenceCenter.X,
                (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)drawArgs.WorldCamera.ReferenceCenter.Z
                );

            drawArgs.device.SetTransform(TransformState.World *= Matrix.Translation(surfacePos - rc);
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.SelectArg1);
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.TextureColor);
            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.TextureColor);
            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.SelectArg1);
            drawArgs.device.SetRenderState(RenderState.ZEnable , false);
            drawArgs.device.SetTexture(0, HighlightTexture);
            drawArgs.device.VertexFormat = CustomVertex.PositionColoredTextured.Format;

            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, m_highlightVertices);
            drawArgs.device.SetRenderState(RenderState.ZEnable , true);
            drawArgs.device.SetRenderState(RenderState.Lighting , lighting);
        }

        bool m_isMouseInside;

        public override void Render(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
                return;

            if (this.m_polygonFeature == null || !drawArgs.WorldCamera.ViewFrustum.Intersects(this.m_polygonFeature.BoundingBox))
                return;
            try
            {
                double offset = 0;
                if (this.Bar3D != null && this.Bar3D.IsOn)
                {
                    this.Bar3D.Render(drawArgs);
                    offset = this.Bar3D.RenderedHeight;
                }
                Cull cull , drawArgs.device.SetRenderState(RenderState.CullMode);
                drawArgs.device.SetRenderState(RenderState.CullMode , Cull.None);
                drawArgs.device.SetRenderState(RenderState.ZEnable , true);
                drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.SelectArg1);
                drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.TextureColor);

                Vector3 surfacePos = MathEngine.SphericalToCartesian(this.m_latitude, this.m_longitude, this.World.EquatorialRadius);

                Vector3 rc = new Vector3(
                    (float)drawArgs.WorldCamera.ReferenceCenter.X,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Z
                    );

                Vector3 projectedPoint = drawArgs.WorldCamera.Project(surfacePos - rc);
                int mouseBuffer = 15;
                if (projectedPoint.X > DrawArgs.LastMousePosition.X - mouseBuffer &&
                        projectedPoint.X < DrawArgs.LastMousePosition.X + mouseBuffer &&
                        projectedPoint.Y > DrawArgs.LastMousePosition.Y - mouseBuffer &&
                        projectedPoint.Y < DrawArgs.LastMousePosition.Y + mouseBuffer)
                {
                    if (!this.m_isMouseInside)
                    {
                        this.m_isMouseInside = true;
                        if (this.OnMouseEnterEvent != null) this.OnMouseEnterEvent(this, null);
                    }
                }
                else
                {
                    if (this.m_isMouseInside)
                    {
                        this.m_isMouseInside = false;
                        if (this.OnMouseLeaveEvent != null) this.OnMouseLeaveEvent(this, null);
                    }
                }
                drawArgs.device.SetRenderState(RenderState.CullMode , Cull.None);

                if (this.ShowHighlight) this.renderHighlight(drawArgs);
                this.RenderFlag(drawArgs, offset);
                drawArgs.device.SetRenderState(RenderState.CullMode , cull);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void RenderFlag(DrawArgs drawArgs, double offset)
        {
            if (m_effect == null)
            {
                string outerrors = "";

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Stream effectStream = assembly.GetManifestResourceStream("WorldWind.Shaders.flag.fx");

                m_effect =
                    Effect.FromStream(
                    drawArgs.device,
                    effectStream,
                    null,
                    null,
                    ShaderFlags.None,
                    null,
                    out outerrors);

                if (outerrors != null && outerrors.Length > 0)
                    Log.Write(Log.Levels.Error, outerrors);
            }

            if (m_vertexBuffer == null)
            {
                drawArgs.device.DeviceReset += new EventHandler(this.device_DeviceReset);
                this.device_DeviceReset(drawArgs.device, null);
            }

            if (m_flagPoleVertices == null)
            {
                CreateFlagPole(drawArgs.device);
            }

            Vector3 pos = MathEngine.SphericalToCartesian(this.m_latitude, this.m_longitude, this.World.EquatorialRadius + World.Settings.VerticalExaggeration * this.ScaleZ + offset);
            Vector3 surfacePos = MathEngine.SphericalToCartesian(this.m_latitude, this.m_longitude, this.World.EquatorialRadius + offset);

            Vector3 rc = new Vector3(
                (float)drawArgs.WorldCamera.ReferenceCenter.X,
                (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)drawArgs.WorldCamera.ReferenceCenter.Z
                );

            drawArgs.device.SetTransform(TransformState.World, Matrix.Scaling(World.Settings.VerticalExaggeration * this.ScaleX * 0.01f, 
                World.Settings.VerticalExaggeration * this.ScaleY * 0.01f, -World.Settings.VerticalExaggeration * 2 * this.ScaleZ);
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(90));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(this.m_latitude));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.m_longitude));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.Translation(surfacePos - rc);

            drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.SelectArg1);
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.Diffuse);
            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.SelectArg1);
            drawArgs.device.VertexShader = null;
            drawArgs.device.PixelShader = null;
            drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 
                0, m_flagPoleVertices.Length, m_flagPoleIndices.Length / 3, m_flagPoleIndices, 
                true, m_flagPoleVertices);
            drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, 
                m_outlineFlagPoleVertices.Length, m_outlineFlagPoleIndices.Length / 2, 
                m_outlineFlagPoleIndices, true, m_outlineFlagPoleVertices);
            drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            drawArgs.device.VertexDeclaration = m_vertexDeclaration;
            this.m_angle += .04f;

            if (this.m_angle > 360) this.m_angle = 0;

            drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

            drawArgs.device.SetTransform(TransformState.World, Matrix.Scaling(World.Settings.VerticalExaggeration * this.ScaleX, 
                World.Settings.VerticalExaggeration * this.ScaleY, World.Settings.VerticalExaggeration * this.ScaleZ);
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(this.m_latitude));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.m_longitude));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.Translation(pos - rc);

            Matrix worldViewProj = drawArgs.device.SetTransform(TransformState.World * 
                drawArgs.device.SetTransform(TransformState.View * drawArgs.device.SetTransform(TransformState.Projection;

            DateTime currentTime = TimeKeeper.CurrentTimeUtc;
            Point3d sunPosition = SunCalculator.GetGeocentricPosition(currentTime);

            Vector3 sunVector = new Vector3(
                (float)-sunPosition.X,
                (float)-sunPosition.Y,
                (float)-sunPosition.Z);

            m_effect.Technique = "VertexAndPixelShader";
            m_effect.SetValue("angle", (float) this.m_angle);
            m_effect.SetValue("attentuation", this.Attentuation);
            m_effect.SetValue("World", drawArgs.device.GetTransform(TransformState.World);
            m_effect.SetValue("View", drawArgs.device.GetTransform(TransformState.View);
            m_effect.SetValue("Projection", drawArgs.device.GetTransform(TransformState.Projection);
            m_effect.SetValue("Tex0", this.m_texture);
            m_effect.SetValue("lightDir", new Vector4(sunVector.X, sunVector.Y, sunVector.Z, 0));

            drawArgs.device.Indices = m_indexBuffer;
            drawArgs.device.SetStreamSource(0, m_vertexBuffer, 0);
            int numPasses = m_effect.Begin(0);

            for (int i = 0; i < numPasses; i++)
            {
                m_effect.BeginPass(i);
                drawArgs.device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0, m_vertices.Length, 0,
                    m_indices.Length / 3);

                m_effect.EndPass();
            }
            m_effect.End();
            drawArgs.device.Indices = null;
            drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix;
            drawArgs.device.SetTransform(TransformState.View = drawArgs.WorldCamera.ViewMatrix;
        }

        private void CreateFlagPole(Device device)
        {
            m_flagPoleVertices = new CustomVertex.PositionColored[8];
            m_outlineFlagPoleVertices = new CustomVertex.PositionColored[8];
            int c = System.Drawing.Color.Gray.ToArgb();
            int outline = System.Drawing.Color.DarkGray.ToArgb();

            m_flagPoleVertices[0] = new CustomVertex.PositionColored(-1, -1, 1, c);
            m_flagPoleVertices[1] = new CustomVertex.PositionColored(-1, 1, 1, c);
            m_flagPoleVertices[2] = new CustomVertex.PositionColored(1, -1, 1, c);
            m_flagPoleVertices[3] = new CustomVertex.PositionColored(1, 1, 1, c);
            m_flagPoleVertices[4] = new CustomVertex.PositionColored(-1, -1, 0, c);
            m_flagPoleVertices[5] = new CustomVertex.PositionColored(-1, 1, 0, c);
            m_flagPoleVertices[6] = new CustomVertex.PositionColored(1, -1, 0, c);
            m_flagPoleVertices[7] = new CustomVertex.PositionColored(1, 1, 0, c);

            m_outlineFlagPoleVertices[0] = new CustomVertex.PositionColored(-1, -1, 1, outline);
            m_outlineFlagPoleVertices[1] = new CustomVertex.PositionColored(-1, 1, 1, outline);
            m_outlineFlagPoleVertices[2] = new CustomVertex.PositionColored(1, -1, 1, outline);
            m_outlineFlagPoleVertices[3] = new CustomVertex.PositionColored(1, 1, 1, outline);
            m_outlineFlagPoleVertices[4] = new CustomVertex.PositionColored(-1, -1, 0, outline);
            m_outlineFlagPoleVertices[5] = new CustomVertex.PositionColored(-1, 1, 0, outline);
            m_outlineFlagPoleVertices[6] = new CustomVertex.PositionColored(1, -1, 0, outline);
            m_outlineFlagPoleVertices[7] = new CustomVertex.PositionColored(1, 1, 0, outline);

            m_flagPoleIndices = new short[] {
                // top face
                0, 1, 2,
                2, 1, 3,
 
                // bottom face
                4, 5, 6,
                6, 5, 7,
 
                // front face
                4, 0, 6,
                6, 0, 2,
 
                // back face
                7, 3, 5,
                5, 3, 1,

                // left face
                5, 1, 4,
                4, 1, 0,
 
                // right face
                6, 2, 7,
                7, 2, 3};

            m_outlineFlagPoleIndices = new short[] {
                0,1, 1,3, 3,2, 2,0,
                4,5, 5,7, 7,6, 6,4,
                0,4, 2,6, 3,7, 1,5 };
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            Vector3 surfacePos = MathEngine.SphericalToCartesian(this.m_latitude, this.m_longitude, this.World.EquatorialRadius);
            Vector3 rc = new Vector3(
                (float)drawArgs.WorldCamera.ReferenceCenter.X,
                (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)drawArgs.WorldCamera.ReferenceCenter.Z
                );
            Vector3 projectedPoint = drawArgs.WorldCamera.Project(surfacePos - rc);
            int mouseBuffer = 15;

            if (projectedPoint.X > DrawArgs.LastMousePosition.X - mouseBuffer &&
                    projectedPoint.X < DrawArgs.LastMousePosition.X + mouseBuffer &&
                    projectedPoint.Y > DrawArgs.LastMousePosition.Y - mouseBuffer &&
                    projectedPoint.Y < DrawArgs.LastMousePosition.Y + mouseBuffer)
            {
                if (this.OnMouseUpEvent != null)
                {
                    this.OnMouseUpEvent(this, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left,
                        1, DrawArgs.LastMousePosition.X, DrawArgs.LastMousePosition.Y, 0));
                }
                return true;
            }
            return false;
        }

        private void device_DeviceReset(object sender, EventArgs e)
        {
            Device device = (Device)sender;
            m_vertexBuffer = new VertexBuffer(
                typeof(CustomVertex.PositionNormalTextured),
                m_vertices.Length,
                device,
                Usage.Dynamic | Usage.WriteOnly,
                CustomVertex.PositionNormalTextured.Format,
                Pool.Default);

            m_vertexBuffer.SetData(m_vertices, 0, LockFlags.NoOverwrite | LockFlags.Discard);
            m_indexBuffer = new IndexBuffer(
                typeof(short),
                m_indices.Length,
                device,
                Usage.Dynamic | Usage.WriteOnly,
                Pool.Default);

            m_indexBuffer.SetData(m_indices, 0, LockFlags.NoOverwrite | LockFlags.Discard);
        }
    }
}
