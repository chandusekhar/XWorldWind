using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Mathematics.Interop;

namespace WorldWind.Extensions
{
    public static class ColorExtensions
    {
        public static RawColor4 ToRawColor4(this System.Drawing.Color pThis)
        {
            return new RawColor4(pThis.R / 255.0f, pThis.G/255.0f, pThis.B/255.0f, pThis.A/255.0f);
        }
    }
}
