using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TwinspireCS.Tiled.Json;

namespace TwinspireCS.Tiled
{
    public class Tiled
    {

        /// <summary>
        /// Get the full paths of the resource files from the given Tiled-formatted JSON file.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        /// <param name="type">The type of file represented by Tiled of the given path.</param>
        /// <returns></returns>
        public static string[] GetResourceFiles(string path, TiledFileType type)
        {
            if (!File.Exists(path))
                return null;

            var fullPath = string.Empty;
            if (!Path.IsPathRooted(path))
                fullPath = Path.GetFullPath(path);
            else
                fullPath = path;

            var temp = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Path.GetDirectoryName(fullPath);

            var content = File.ReadAllText(fullPath);
            if (type == TiledFileType.Map)
            {
                var decoded = JsonSerializer.Deserialize<TiledMap>(content);
                var results = new string[decoded.tilesets.Length];
                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = Path.GetFullPath(decoded.tilesets[i].source);
                }
                Environment.CurrentDirectory = temp;
                return results;
            }
            else if (type == TiledFileType.Tileset)
            {
                var decoded = JsonSerializer.Deserialize<TiledTileset>(content);
                Environment.CurrentDirectory = temp;
                return new string[1] { Path.GetFullPath(decoded.source) };
            }

            Environment.CurrentDirectory = temp;
            return null;
        }

        /// <summary>
        /// Import a Tiled-formatted JSON file, extracting all relevant
        /// resource files and writing the results into the given package.
        /// </summary>
        /// <remarks>
        /// The path to the Tiled-formatted JSON file must represent a map.
        /// Tilesets do not require "importing" and can be added to a package directly if required.<br/><br/>
        /// Due to the way files are imported, it is recommended that any data needing to be overwritten
        /// is re-imported using this same method to avoid accidental deletion of existing data.
        /// </remarks>
        /// <param name="packageIndex">The package to write the json file into.</param>
        /// <param name="identifier">The identifier to give to the JSON file in the package.</param>
        /// <param name="path">The full path to the JSON file.</param>
        /// <param name="options">An option determining if the resources supplied by the Tiled-formatted JSON file should also be added.</param>
        public static void Import(int packageIndex, string identifier, string path, ImportOptions options = ImportOptions.None)
        {
            if (!File.Exists(path))
                return;

            var content = File.ReadAllText(path);
            var blob = new Blob();
            blob.LoadFromMemory(content, Encoding.UTF8);
            Application.Instance.ResourceManager.AddResource(packageIndex, identifier, blob.Data);
            
            if (options == ImportOptions.IncludeResources)
            {
                var resources = GetResourceFiles(path, TiledFileType.Map);
                for (int i = 0; i < resources.Length; i++)
                {
                    var resource = resources[i];
                    var resIdentifier = Path.GetFileNameWithoutExtension(resource);
                    if (Application.Instance.ResourceManager.DoesNameExist(resIdentifier))
                    {
                        continue;
                    }

                    var resBlob = new Blob();
                    var resContent = File.ReadAllText(resource);
                    resBlob.LoadFromMemory(resContent, Encoding.UTF8);
                    Application.Instance.ResourceManager.AddResource(packageIndex, resIdentifier, resBlob.Data);
                }
            }

            Application.Instance.ResourceManager.WriteAll(packageIndex);
        }

    }

    public enum TiledFileType
    {
        Map,
        Tileset
    }

    public enum ImportOptions
    {
        None,
        IncludeResources
    }
}
