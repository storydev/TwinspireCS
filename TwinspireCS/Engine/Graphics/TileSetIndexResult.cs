using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public struct TileSetIndexResult
    {

        public int TileSetIndex;
        public int TileIndex;

        public TileSetIndexResult(int tileSetIndex, int tileIndex)
        {
            TileSetIndex = tileSetIndex;
            TileIndex = tileIndex;
        }

    }
}
