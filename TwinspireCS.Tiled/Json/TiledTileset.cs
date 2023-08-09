using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Tiled.Json
{
    public class TiledTileset
    {

        public string? backgroundcolor;
        public string? @class;
        public int columns;
        public string fillmode;
        public int firstgid;
        public TiledGrid? grid;
        public string image;
        public int imageheight;
        public int imagewidth;
        public int margin;
        public string name;
        public string objectalignment;
        public TiledProperty[] properties;
        public string source;
        public int spacing;
        public TiledTerrain[] terrains;
        public int tilecount;
        public string tiledversion;
        public int tileheight;
        public TiledTileOffset? tileoffset;
        public string tilerendersize;
        public TiledTile[] tiles;
        public int tilewidth;
        public TiledTransformation? transformations;
        public string? transparentcolor;
        public string type;
        public string version;
        public TiledWangSet[] wangsets;

        public TiledTileset()
        {

        }

    }
}
