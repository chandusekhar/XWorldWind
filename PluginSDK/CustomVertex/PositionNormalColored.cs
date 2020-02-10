using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace WorldWind.CustomVertex
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionNormalColored
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Single Nx;
        public Single Ny;
        public Single Nz;
        public Int32 Color;
        public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse;

        public PositionNormalColored(Vector3 position, Vector3 normal, Int32 color)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Z = position.Z;
            this.Nx = normal.X;
            this.Ny = normal.Y;
            this.Nz = normal.Z;
            this.Color = color;
        }
    }
}
