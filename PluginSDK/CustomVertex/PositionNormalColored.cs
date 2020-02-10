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

        public PositionNormalColored(Single x, Single y, Single z, Single nx, Single ny, Single nz, Int32 color)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Color = color;
            this.Nx = nx;
            this.Ny = ny;
            this.Nz = nz;
        }

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
