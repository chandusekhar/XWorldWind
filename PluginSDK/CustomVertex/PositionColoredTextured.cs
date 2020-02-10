using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;

namespace WorldWind.CustomVertex
{

    [StructLayout(LayoutKind.Sequential)]
    public struct PositionColoredTextured
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Int32 Color;
        public Single Tu;
        public Single Tv;
        public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture0;

        public Vector3 Position
        {
            get
            {
                return new Vector3(this.X, this.Y, this.Z);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.Z = value.Z;
            }
        }

        public PositionColoredTextured(Single x, Single y, Single z, Int32 color, Single tu, Single tv)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Color = color;
            this.Tu = tu;
            this.Tv = tv;
        }

        public PositionColoredTextured(Vector3 position, Int32 color, Vector2 textureCoords)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Z = position.Z;
            this.Color = color;
            this.Tu = textureCoords.X;
            this.Tv = textureCoords.Y;
        }
    }
}
