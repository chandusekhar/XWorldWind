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
    public struct PositionColored
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Int32 Color;
        public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse;

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

        public PositionColored(Single x, Single y, Single z, Int32 color)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Color = color;
        }

        public PositionColored(Vector3 position, Int32 color)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Z = position.Z;
            this.Color = color;
        }
    }
}
