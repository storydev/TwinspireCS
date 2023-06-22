using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public struct Tile
    {

        public Vector2 Offset;
        public float Opacity;
        public bool[] EntranceDirections;
        public bool[] ExitDirections;
        public uint Flags;

        public Tile()
        {
            Offset = new Vector2(0, 0);
            Opacity = 1f;
            EntranceDirections = new bool[4];
            ExitDirections = new bool[4];
            Flags = 0;
        }

    }
}
