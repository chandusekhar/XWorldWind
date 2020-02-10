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
    public struct TransformedColored
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Int32 Color;
        public static readonly VertexFormat Format = VertexFormat.PositionRhw | VertexFormat.Diffuse;

        public TransformedColored(Vector3 position, Int32 color)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Z = position.Z;
            this.Color = color;
        }
    }
}
