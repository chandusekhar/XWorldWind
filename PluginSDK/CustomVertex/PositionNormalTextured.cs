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

        public Vector3 Normal
        {
            get
            {
                return new Vector3(this.Nx, this.Ny, this.Nz);
            }
            set
            {
                this.Nx = value.X;
                this.Ny = value.Y;
                this.Nz = value.Z;
            }
        }

        public PositionNormalTextured(Single x, Single y, Single z, Single nx, Single ny, Single nz, Single tu, Single tv)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Nx = nx;
            this.Ny = ny;
            this.Nz = nz;
            this.Tu = tu;
            this.Tv = tv;
        }

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
