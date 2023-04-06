using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class Grid
    {

        public Vector4 Dimension;
        public float[] Columns;
        public float[] Rows;
        public float[] Margin;
        public float[] Padding;

        public Grid()
        {
            Dimension = new Vector4();
            Columns = Array.Empty<float>();
            Rows = Array.Empty<float>();
            Margin = Array.Empty<float>();
            Padding = Array.Empty<float>();
        }

    }
}
