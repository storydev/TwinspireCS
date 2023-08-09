using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Tiled.Json
{
    public class TiledObjectTemplate
    {

        public string type;
        public TiledTileset? tileset;
        public TiledObject @object;

        public TiledObjectTemplate()
        {

        }

    }
}
