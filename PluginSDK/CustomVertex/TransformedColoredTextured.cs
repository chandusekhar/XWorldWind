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
    public struct TransformedColoredTextured
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Int32 Color;
        public Single Tu;
        public Single Tv;
        public static readonly VertexFormat Format = VertexFormat.PositionRhw | VertexFormat.Diffuse | VertexFormat.Texture0;

        public TransformedColoredTextured(Vector3 position, Int32 color, Vector2 textureCoords)
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
