using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class TileSet
    {

        public string Image;
        public int TileSize;

        public TileSet()
        {
            Image = string.Empty;
        }

        private static List<int> startIDs;
        private static List<TileSet> tilesets;
        /// <summary>
        /// All the tilesets that are available.
        /// </summary>
        public static IEnumerable<TileSet> TileSets
        {
            get => tilesets;
        }

        /// <summary>
        /// Create a TileSet with the given image and size of the tiles.
        /// </summary>
        /// <param name="imageName">The identifier representing the image. Must already be loaded.</param>
        /// <param name="tileSize">The size of each tile.</param>
        /// <returns></returns>
        public static int CreateTileSet(string imageName, int tileSize)
        {
            var firstTileset = tilesets == null;
            if (firstTileset)
            {
                tilesets = new List<TileSet>();
                startIDs = new List<int>
                {
                    0
                };
            }

            var tileset = new TileSet();
            tileset.Image = imageName;
            tileset.TileSize = tileSize;
            tilesets.Add(tileset);
            if (!firstTileset)
            {
                var lastTileset = tilesets[^2];
                var tilesetImage = Application.Instance.ResourceManager.GetImage(lastTileset.Image);
                var columns = Math.Floor((float)(tilesetImage.width / lastTileset.TileSize));
                var rows = Math.Floor((float)(tilesetImage.height / lastTileset.TileSize));
                int cells = (int)(columns * rows);
                var lastStartID = startIDs[^0];
                startIDs.Add(lastStartID + cells);
            }

            return tilesets.Count - 1;
        }

    }
}
