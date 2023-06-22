using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class TileMap
    {

        public string Name;
        public List<TileLayer> Layers;
        public int TileSize;
        public int TilesX;
        public int TilesY;

        public TileMap()
        {
            Name = string.Empty;
            Layers = new List<TileLayer>();
            TileSize = 48;
            TilesX = 25;
            TilesY = 25;
        }

    }
}
