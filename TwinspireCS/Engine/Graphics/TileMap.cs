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
            TilesX = 15;
            TilesY = 15;
        }

        private static List<TileMap> maps;
        public static IEnumerable<TileMap> Maps => maps ??= new List<TileMap>();

        /// <summary>
        /// Add a TileMap to the global map stack.
        /// </summary>
        /// <param name="map">The tile map to add.</param>
        public static void AddMap(TileMap map)
        {
            maps ??= new List<TileMap>();
            maps.Add(map);
        }

    }
}
