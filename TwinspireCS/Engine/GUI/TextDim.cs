using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class TextDim
    {

        public Vector2 ContentSize;
        public char[] Characters;
        public int[] Breaks;

        public TextDim()
        {
            ContentSize = new Vector2();
            Characters = Array.Empty<char>();
            Breaks = Array.Empty<int>();
        }

    }
}
