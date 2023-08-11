using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Tiled.Json
{
    public class TiledMap
    {

        public string backgroundcolor;
        public string @class;
        public int compressionlevel;
        public int height;
        public int hexsidelength;
        public bool infinite;
        public TiledLayer[] layers;
        public int nextlayerid;
        public int nextobjectid;
        public string orientation;
        public double parallaxoriginx;
        public double parallaxoriginy;
        public TiledProperty[] properties;
        public string renderorder;
        public string staggeraxis;
        public string staggerindex;
        public string tiledversion;
        public int tileheight;
        public TiledTileset[] tilesets;
        public int tilewidth;
        public string type;
        public string version;
        public int width;

        public TiledMap()
        {

        }

    }
}
