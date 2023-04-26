using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.Extras
{
    public struct ColorMethod
    {

        public ColorType Type;
        public Color[] Colors;
        
        public ColorMethod(ColorType type, Color[] colors)
        {
            Type = type;
            Colors = colors;
        }

    }
}
