using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TwinspireCS.Engine.Extras;

namespace TwinspireCS.Engine.GUI
{
    public class Grid
    {

        public Vector4 Dimension;
        public float[] Columns;
        public float[] Rows;
        

        public ColorMethod[] BackgroundColors;
        public string[] BackgroundImages;

        public Vector2[] Offsets;

        // multiply by 4 for these ones
        public Color[] BorderColors;
        public int[] BorderThicknesses;
        public bool[] Borders;
        public float[] RadiusCorners;

        public float[] MarginTop;
        public float[] MarginBottom;
        public float[] MarginLeft;
        public float[] MarginRight;

        public float[] PaddingTop;
        public float[] PaddingBottom;
        public float[] PaddingLeft;
        public float[] PaddingRight;

        public Grid()
        {
            Dimension = new Vector4();
            Columns = Array.Empty<float>();
            Rows = Array.Empty<float>();

            BackgroundColors = Array.Empty<ColorMethod>();
            BackgroundImages = Array.Empty<string>();


            Offsets = Array.Empty<Vector2>();

            BorderColors = Array.Empty<Color>();
            BorderThicknesses = Array.Empty<int>();
            Borders = Array.Empty<bool>();
            RadiusCorners = Array.Empty<float>();


        }

        public Grid SetColumnWidth(int column, float percentageWidth)
        {
            Columns[column] = Dimension.Z * percentageWidth;
            return this;
        }

    }
}
