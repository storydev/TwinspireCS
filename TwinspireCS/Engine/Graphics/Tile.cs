using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.Graphics
{
    public struct Tile
    {

        public int Index;
        public Vector2 Offset;
        public float Opacity;
        public float Rotation;
        public Color Tint;
        public bool[] EntranceDirections;
        public bool[] ExitDirections;
        public uint Flags;

        public Tile()
        {
            Index = -1;
            Offset = new Vector2(0, 0);
            Opacity = 1f;
            Rotation = 0f;
            EntranceDirections = new bool[4];
            ExitDirections = new bool[4];
            Flags = 0;
            Tint = Color.WHITE;
        }

    }
}
