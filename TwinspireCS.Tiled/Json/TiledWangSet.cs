using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Tiled.Json
{
    public class TiledWangSet
    {

        public string @class;
        public TiledWangColor[] colors;
        public string name;
        public TiledProperty[] properties;
        public int tile;
        public string type;
        public TiledWangTile[] wangtiles;

        public TiledWangSet()
        {

        }

    }
}
