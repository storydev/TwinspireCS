using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.Graphics
{
    public class Spritesheet
    {

        public string ImageName;
        public List<Frame> Frames;
        public Dictionary<string, int[]> Groups;


        public Spritesheet()
        {
            ImageName = string.Empty;
            Frames = new List<Frame>();
            Groups = new Dictionary<string, int[]>();
        }


        private static List<Spritesheet> _spritesheets;
        /// <summary>
        /// Get a list of all spritesheets.
        /// </summary>
        public static IEnumerable<Spritesheet> Spritesheets
        {
            get
            {
                return _spritesheets ??= new List<Spritesheet>();
            }
        }

        /// <summary>
        /// Create an empty spritesheet with an image of the given identifier.
        /// </summary>
        /// <param name="imageName">The identifier of the image to look for.</param>
        /// <returns>The index of the spritesheet in the global Spritesheets enumerator.</returns>
        public static int CreateSpritesheet(string imageName)
        {
            _spritesheets ??= new List<Spritesheet>();

            var spritesheet = new Spritesheet();
            spritesheet.ImageName = imageName;
            _spritesheets.Add(spritesheet);
            return _spritesheets.Count - 1;
        }

        /// <summary>
        /// Create a spritesheet with an image of the given identifier. Frames are
        /// generated based on the given tileSize. The image must be loaded before this
        /// function can operate.<br/><br/>
        /// 
        /// If the given tileSize is not divisible into the total width and height of
        /// the image, an exception is thrown.
        /// </summary>
        /// <param name="imageName">The identifier of the image to look for.</param>
        /// <param name="tileSize">The fixed tile size in the image.</param>
        /// <returns>The index of the spritesheet in the global Spritesheets enumerator.</returns>
        public static unsafe int CreateTiledSpritesheet(string imageName, int tileSize)
        {
            _spritesheets ??= new List<Spritesheet>();

            var hasIdentifier = Application.Instance.ResourceManager.DoesIdentifierExist(imageName);
            if (!hasIdentifier)
            {
                throw new Exception("The image with the given name, '" + imageName + "' could not be found.");
            }

            var image = Application.Instance.ResourceManager.GetImage(imageName);
            if (image.data == null)
            {
                throw new Exception("The image name given does not relate to an image.");
            }

            if (tileSize % image.width != 0 || tileSize % image.height != 0)
            {
                throw new Exception("The given tile size is not divisible by the width or height of the image.");
            }

            var spritesheet = new Spritesheet();
            spritesheet.ImageName = imageName;

            var rows = image.height / tileSize;
            var columns = image.width / tileSize;
            
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    var frame = new Frame();
                    frame.Dimension = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                    spritesheet.Frames.Add(frame);
                }
            }

            _spritesheets.Add(spritesheet);
            return _spritesheets.Count - 1;
        }

    }
}
