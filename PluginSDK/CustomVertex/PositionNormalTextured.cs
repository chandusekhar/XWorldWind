using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace WorldWind.CustomVertex
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionNormalTextured
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Single Nx;
        public Single Ny;
        public Single Nz;
        public Single Tu;
        public Single Tv;
        public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture0;

        public PositionNormalTextured(Vector3 position, Vector3 normal, Vector2 textureCoords)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Z = position.Z;
            this.Nx = normal.X;
            this.Ny = normal.Y;
            this.Nz = normal.Z;
            this.Tu = textureCoords.X;
            this.Tv = textureCoords.Y;
        }
    }
}
