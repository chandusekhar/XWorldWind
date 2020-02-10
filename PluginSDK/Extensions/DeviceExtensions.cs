using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;

namespace WorldWind.Extensions
{
    public static class DeviceExtensions
    {
        public static Matrix GetWorldViewProjMatrix(this Device pThis)
        {
            return Matrix.Multiply(pThis.GetTransform(TransformState.World), Matrix.Multiply(pThis.GetTransform(TransformState.View), pThis.GetTransform(TransformState.Projection)));
        }
    }
}
