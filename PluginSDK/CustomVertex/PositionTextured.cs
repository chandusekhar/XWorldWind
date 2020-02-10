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
    public struct PositionTextured
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Single Tu;
        public Single Tv;
        public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Texture0;

        public PositionTextured(Single x, Single y, Single z, Single tu, Single tv)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Tu = tu;
            this.Tv = tv;
        }

        public PositionTextured(Vector3 position, Vector2 textureCoords)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Z = position.Z;
            this.Tu = textureCoords.X;
            this.Tv = textureCoords.Y;
        }
    }
}
