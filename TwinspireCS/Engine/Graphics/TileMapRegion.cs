using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public struct TileMapRegion
    {

        public int RenderTilesX;
        public int RenderTilesY;
        public bool AllowMapMoveOffBorders;

        public TileMapRegion(int renderTilesX, int renderTilesY)
        {
            RenderTilesX = renderTilesX;
            RenderTilesY = renderTilesY;
            AllowMapMoveOffBorders = false;
        }

    }
}
