using System;
using SharpDX;
using SharpDX.Direct3D9;


namespace WorldWind.Renderable
{

    public class Bar3D : RenderableObject
    {

        double m_distanceAboveSurface;

        double m_latitude;

        double m_longitude;

        double m_height;

        int m_color;

        float m_currentVerticalExaggeration = World.Settings.VerticalExaggeration;

        Point3d m_cartesianPoint;



        private bool m_useScaling;

        private double m_scalarMinimum;

        private double m_scalarMaximum = 1;

        private double m_currentPercent;

        private double m_targetScalar = 1;

        private float m_scaleX = 1;

        private float m_scaleY = 1;



        public double Latitude
        {

            get { return this.m_latitude; }

            set
            {
                this.m_latitude = value;

                this.UpdateCartesianPoint();

            }

        }



        public double Longitude
        {

            get { return this.m_longitude; }

            set
            {
                this.m_longitude = value;

                this.UpdateCartesianPoint();

            }

        }



        public double DistanceAboveSurface
        {

            get { return this.m_distanceAboveSurface; }

            set
            {
                this.m_distanceAboveSurface = value;

                this.UpdateCartesianPoint();

            }

        }



        public double Height
        {

            get { return this.m_height; }

            set { this.m_height = value; }

        }



        public bool UseScaling
        {

            get { return this.m_useScaling; }

            set { this.m_useScaling = value; }

        }



        public double ScalarMinimum
        {

            get { return this.m_scalarMinimum; }

            set { this.m_scalarMinimum = value; }

        }



        public double ScalarMaximum
        {

            get { return this.m_scalarMaximum; }

            set { this.m_scalarMaximum = value; }

        }



        public double ScalarValue
        {

            get { return this.m_targetScalar; }

            set { this.m_targetScalar = value; }

        }



        public float ScaleX
        {

            get { return this.m_scaleX; }

            set { this.m_scaleX = value; }

        }



        public float ScaleY
        {

            get { return this.m_scaleY; }

            set { this.m_scaleY = value; }

        }



        public Bar3D(

            string name,

            World parentWorld,

            double latitude,

            double longitude,

            double distanceAboveSurface,

            double height,

            System.Drawing.Color color)

            : base(name, parentWorld)
        {
            this.m_latitude = latitude;

            this.m_longitude = longitude;

            this.m_distanceAboveSurface = distanceAboveSurface;

            this.m_height = height;

            this.m_color = color.ToArgb();


            this.UpdateCartesianPoint();

        }



        private void UpdateCartesianPoint()
        {
            this.m_cartesianPoint = MathEngine.SphericalToCartesianD(

                Angle.FromDegrees(this.m_latitude), Angle.FromDegrees(this.m_longitude), this.m_world.EquatorialRadius + this.m_distanceAboveSurface * World.Settings.VerticalExaggeration);


            this.m_currentVerticalExaggeration = World.Settings.VerticalExaggeration;

        }



        public override void Initialize(DrawArgs drawArgs)
        {
            this.isInitialized = true;

        }



        public override void Update(DrawArgs drawArgs)
        {

            if (!this.isInitialized) this.Initialize(drawArgs);

        }



        public override void Render(DrawArgs drawArgs)
        {
            this.RenderBar(drawArgs);



        }



        public override void Dispose()
        {



        }



        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {

            return false;

        }



        public double RenderedHeight;



        private void RenderBar(DrawArgs drawArgs)
        {

            bool lighting , drawArgs.device.SetRenderState(RenderState.Lighting);
            drawArgs.device.SetRenderState(RenderState.Lighting , false);

            Matrix translation = Matrix.Translation(

                    (float)(this.m_cartesianPoint.X - drawArgs.WorldCamera.ReferenceCenter.X),

                    (float)(this.m_cartesianPoint.Y - drawArgs.WorldCamera.ReferenceCenter.Y),

                    (float)(this.m_cartesianPoint.Z - drawArgs.WorldCamera.ReferenceCenter.Z)

                        );





            if (this.m_extrudeVertices == null)
            {

                CreateExtrude(drawArgs.device);

            }



            if (this.m_useScaling)
            {

                double targetPercent = (this.m_targetScalar - this.m_scalarMinimum) / (this.m_scalarMaximum - this.m_scalarMinimum);

                if (this.m_currentPercent != targetPercent)
                {

                    double delta = Math.Abs(targetPercent - this.m_currentPercent);

                    delta *= 0.1;

                    if (this.m_currentPercent < targetPercent)
                        this.m_currentPercent += delta;

                    else
                        this.m_currentPercent -= delta;

                }



                if (this.getColor(this.m_currentPercent).ToArgb() != this.m_extrudeVertices[0].Color)
                {
                    this.UpdateColor(this.m_currentPercent);

                }

                this.RenderedHeight = this.m_currentPercent * World.Settings.VerticalExaggeration * this.m_height;

                drawArgs.device.SetTransform(TransformState.World, Matrix.Scaling(this.m_scaleX, this.m_scaleY, (float)-this.m_currentPercent * World.Settings.VerticalExaggeration * (float) this.m_height);

            }

            else
            {

                if (this.m_color != this.m_extrudeVertices[0].Color)
                {
                    this.UpdateColor(this.m_color);

                }

                this.RenderedHeight = this.m_height * World.Settings.VerticalExaggeration;

                drawArgs.device.SetTransform(TransformState.World, Matrix.Scaling(this.m_scaleX, this.m_scaleY, (float)-this.m_height * World.Settings.VerticalExaggeration);

            }



            drawArgs.device.Transform.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(90));

            drawArgs.device.Transform.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(this.m_latitude));

            drawArgs.device.Transform.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.m_longitude));

            drawArgs.device.Transform.World *= translation;



            drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;

            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);

            drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);

            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.Diffuse);

            drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.SelectArg1);

            drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, this.m_extrudeVertices.Length, this.m_extrudeIndices.Length / 3, this.m_extrudeIndices, true, this.m_extrudeVertices);

            drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, this.m_outlineVertices.Length, this.m_outlineIndices.Length / 2, this.m_outlineIndices, true, this.m_outlineVertices);

            drawArgs.device.SetRenderState(RenderState.Lighting , lighting);
        }



        private System.Drawing.Color getColor(double percent)
        {

            double min = 0;

            double max = 1;



            double red = 1.0;

            double green = 1.0;

            double blue = 1.0;





            //TODO: make this a function and abstract to allow multiple gradient mappings

            double dv;



            if (percent < min)

                percent = min;

            if (percent > max)

                percent = max;



            dv = max - min;



            if (percent < (min + 0.25 * dv))
            {

                red = 0;

                green = 4 * (percent - min) / dv;

            }

            else if (percent < (min + 0.5 * dv))
            {

                red = 0;

                blue = 1 + 4 * (min + 0.25 * dv - percent) / dv;

            }

            else if (percent < (min + 0.75 * dv))
            {

                red = 4 * (percent - min - 0.5 * dv) / dv;

                blue = 0;

            }

            else
            {

                green = 1 + 4 * (min + 0.75 * dv - percent) / dv;

                blue = 0;

            }



            return System.Drawing.Color.FromArgb((int)(255 * red), (int)(255 * green), (int)(255 * blue));

        }



        private void UpdateColor(double percent)
        {

            int color = this.getColor(percent).ToArgb();

            this.UpdateColor(color);

        }



        private void UpdateColor(int color)
        {

            for (int i = 0; i < this.m_extrudeVertices.Length; i++)
            {
                this.m_extrudeVertices[i].Color = color;

            }

        }



        short[] m_extrudeIndices;

        CustomVertex.PositionColored[] m_extrudeVertices;



        short[] m_outlineIndices;

        CustomVertex.PositionColored[] m_outlineVertices;



        private void CreateExtrude(Device device)
        {
            this.m_extrudeVertices = new CustomVertex.PositionColored[8];

            this.m_outlineVertices = new CustomVertex.PositionColored[8];

            int c = System.Drawing.Color.Red.ToArgb();

            int outline = System.Drawing.Color.DarkGray.ToArgb();


            this.m_extrudeVertices[0] = new CustomVertex.PositionColored(-1, -1, 1, c);

            this.m_extrudeVertices[1] = new CustomVertex.PositionColored(-1, 1, 1, c);

            this.m_extrudeVertices[2] = new CustomVertex.PositionColored(1, -1, 1, c);

            this.m_extrudeVertices[3] = new CustomVertex.PositionColored(1, 1, 1, c);


            this.m_extrudeVertices[4] = new CustomVertex.PositionColored(-1, -1, 0, c);

            this.m_extrudeVertices[5] = new CustomVertex.PositionColored(-1, 1, 0, c);

            this.m_extrudeVertices[6] = new CustomVertex.PositionColored(1, -1, 0, c);

            this.m_extrudeVertices[7] = new CustomVertex.PositionColored(1, 1, 0, c);


            this.m_outlineVertices[0] = new CustomVertex.PositionColored(-1, -1, 1, outline);

            this.m_outlineVertices[1] = new CustomVertex.PositionColored(-1, 1, 1, outline);

            this.m_outlineVertices[2] = new CustomVertex.PositionColored(1, -1, 1, outline);

            this.m_outlineVertices[3] = new CustomVertex.PositionColored(1, 1, 1, outline);


            this.m_outlineVertices[4] = new CustomVertex.PositionColored(-1, -1, 0, outline);

            this.m_outlineVertices[5] = new CustomVertex.PositionColored(-1, 1, 0, outline);

            this.m_outlineVertices[6] = new CustomVertex.PositionColored(1, -1, 0, outline);

            this.m_outlineVertices[7] = new CustomVertex.PositionColored(1, 1, 0, outline);


            this.m_extrudeIndices = new short[] {
 
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
 
                7, 2, 3
 
            };


            this.m_outlineIndices = new short[] {
 
                0,1, 1,3, 3,2, 2,0,
 
                4,5, 5,7, 7,6, 6,4,
 
                0,4, 2,6, 3,7, 1,5 };



        }

    }

}

