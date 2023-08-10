using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Tiled.Json
{
    public class TiledLayer
    {

        public TiledChunk[]? chunks;
        public string? @class;
        public string compression;
        public object data;
        public string draworder;
        public string encoding;
        public int height;
        public int id;
        public string image;
        public TiledLayer[] layers;
        public bool locked;
        public string name;
        public TiledObject[] objects;
        public double offsetx;
        public double offsety;
        public double opacity;
        public double parallaxx;
        public double parallaxy;
        public TiledProperty[] properties;
        public bool repeatx;
        public bool repeaty;
        public int startx;
        public int starty;
        public string tintcolor;
        public string transparentcolor;
        public string type;
        public bool visible;
        public int width;
        public int x;
        public int y;

        public TiledLayer()
        {

        }

    }
}
