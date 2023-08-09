using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Tiled.Json
{
    public class TiledObject
    {

        public bool ellipse;
        public int gid;
        public double height;
        public int id;
        public string name;
        public bool point;
        public TiledPoint[] polygon;
        public TiledPoint[] polyline;
        public TiledProperty[] properties;
        public double rotation;
        public string template;
        public TiledText text;
        public string? type;
        public string? @class;
        public bool visible;
        public double width;
        public double x;
        public double y;

        public TiledObject()
        {

        }

    }
}
