using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Tiled.Json
{
    public class TiledTile
    {

        public TiledFrame[]? animation;
        public int id;
        public string image;
        public int imageheight;
        public int imagewidth;
        public int x;
        public int y;
        public int width;
        public int height;
        public TiledLayer? objectgroup;
        public double? probability;
        public TiledProperty[] properties;
        public string? type;
        // for backwards-compatability with <=1.9
        public string? @class;

        public TiledTile()
        {

        }

    }
}
