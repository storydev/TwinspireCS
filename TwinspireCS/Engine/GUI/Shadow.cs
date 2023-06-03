using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public struct Shadow
    {

        public Color Color;
        public int BlurRadius;
        public float OffsetX;
        public float OffsetY;

        public Shadow()
        {
            Color = new Color(0, 0, 0, 0);
            BlurRadius = 0;
            OffsetX = 0f;
            OffsetY = 0f;
        }

        public Shadow(Color color, float offsetX, float offsetY, int blur = 3)
        {
            Color = color;
            BlurRadius = blur;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public static Shadow Empty => new Shadow();

    }
}
