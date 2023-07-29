using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace TwinspireCS
{
    public class ResourceManager
    {

        private List<int> currentlyOpen;

        private Dictionary<string, Font> fontCache;
        private Dictionary<string, Image> imageCache;
        private Dictionary<string, Wave> waveCache;
        private Dictionary<string, Music> musicCache;
        private Dictionary<string, Texture2D> textureCache;
        private Dictionary<string, RenderTexture2D> renderTextureCache;
        private Dictionary<string, Blob> blobCache;

        private List<DataPackage> packages;
        private List<bool> packagesEncrypted;
        /// <summary>
        /// Gets the packages that exist in this resource manager.
        /// </summary>
        public IEnumerable<DataPackage> Packages => packages;

        /// <summary>
        /// The directory path in which to find asset files.
        /// </summary>
        public string AssetDirectory { get; set; }

        private List<string> altDirectories;
        private List<DirectoryScope> altDirectoryScopes;
        /// <summary>
        /// A list of alternative directories this Resource Manager has access to.
        /// </summary>
        public IEnumerable<string> AltDirectories { get => altDirectories; }

        public ResourceManager()
        {
            currentlyOpen = new List<int>();
            packages = new List<DataPackage>();
            packagesEncrypted = new List<bool>();
            fontCache = new Dictionary<string, Font>();
            imageCache = new Dictionary<string, Image>();
            waveCache = new Dictionary<string, Wave>();
            musicCache = new Dictionary<string, Music>();
            textureCache = new Dictionary<string, Texture2D>();
            renderTextureCache = new Dictionary<string, RenderTexture2D>();
            blobCache = new Dictionary<string, Blob>();
            AssetDirectory = string.Empty;
            altDirectories = new List<string>();
            altDirectoryScopes = new List<DirectoryScope>();
        }

        /// <summary>
        /// Adds a path to the Documents of the user and returns the full absolute path.
        /// 
        /// If the given path does not exist, a directory will be created.
        /// </summary>
        /// <param name="path">The local path to add.</param>
        /// <param name="success"><c>False</c> if the directory could not be created.</param>
        /// <returns>The full absolute path.</returns>
        public string AddLocalDirectory(string path, out bool success)
        {
            var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path);
            try
            {
                Directory.CreateDirectory(fullPath);
                altDirectories.Add(fullPath);
                altDirectoryScopes.Add(DirectoryScope.Local);
                success = true;
                return fullPath;
            }
            catch
            {
                success = false;
                return string.Empty;
            }
        }

        /// <summary>
        /// Adds a path to a Common directory accessible by all users on the system, and returns the full absolute path.
        /// 
        /// If the given path does not exist, a directory will be created.
        /// </summary>
        /// <param name="path">The common path to add.</param>
        /// <param name="success"><c>False</c> if the directory could not be created.</param>
        /// <returns>The full absolute path.</returns>
        public string AddCommonDirectory(string path, out bool success)
        {
            var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), path);
            try
            {
                Directory.CreateDirectory(fullPath);
                altDirectories.Add(fullPath);
                altDirectoryScopes.Add(DirectoryScope.Common);
                success = true;
                return fullPath;
            }
            catch
            {
                success = false;
                return string.Empty;
            }
        }

        /// <summary>
        /// Adds a path to a directory held by this Application, accessible by the current user, and returns the full absolute path.
        /// 
        /// If the given path does not exist, a directory will be created.
        /// </summary>
        /// <param name="path">The application directory to add.</param>
        /// <param name="success"><c>False</c> if the directory could not be created.</param>
        /// <returns>The full absolute path.</returns>
        public string AddApplicationDirectory(string path, out bool success)
        {
            var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path);
            try
            {
                Directory.CreateDirectory(fullPath);
                altDirectories.Add(fullPath);
                altDirectoryScopes.Add(DirectoryScope.Application);
                success = true;
                return fullPath;
            }
            catch
            {
                success = false;
                return string.Empty;
            }
        }

        /// <summary>
        /// Adds a path onto the default network for this Application, accessible by the current user, and returns the full absolute path.
        /// 
        /// If the given path does not exist, a directory will be created.
        /// </summary>
        /// <param name="path">The application directory to add.</param>
        /// <param name="success"><c>False</c> if the directory could not be created.</param>
        /// <returns>The full absolute path.</returns>
        public string AddNetworkDirectory(string path, out bool success)
        {
            var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path);
            try
            {
                Directory.CreateDirectory(fullPath);
                altDirectories.Add(fullPath);
                altDirectoryScopes.Add(DirectoryScope.Network);
                success = true;
                return fullPath;
            }
            catch
            {
                success = false;
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the path referring to the specific scope.
        /// </summary>
        /// <param name="scope">The scope to find. If Assets is specified, return <c>AssetDirectory</c>.</param>
        /// <returns></returns>
        public string GetPath(DirectoryScope scope)
        {
            if (scope == DirectoryScope.Assets)
                return AssetDirectory;

            var index = altDirectoryScopes.FindIndex((s) => s == scope);
            if (index > -1)
                return altDirectories[index];
            else
                return string.Empty;
        }

        /// <summary>
        /// Create a package from which a binary file is read in the default Asset directory.
        /// </summary>
        /// <param name="sourceFile">The binary source file to read from.</param>
        /// <returns></returns>
        public int CreatePackage(string sourceFile)
        {
            return CreatePackage(sourceFile, DirectoryScope.Assets);
        }

        /// <summary>
        /// Create a package from which a binary file is read using the given scope.
        /// </summary>
        /// <param name="sourceFile">The binary source file to read from.</param>
        /// <param name="scope">The directory path to find from alternative directories.</param>
        public int CreatePackage(string sourceFile, DirectoryScope scope)
        {
            var package = new DataPackage();
            package.SourceFilePath = Path.Combine(GetPath(scope), sourceFile);
            packages.Add(package);
            packagesEncrypted.Add(false);
            return packages.Count - 1;
        }

        /// <summary>
        /// Create a package from which a binary file is read using the given scope.
        /// </summary>
        /// <param name="sourceFile">The binary source file to read from.</param>
        /// <param name="absoluteDir">The full absolute path to a directory.</param>
        /// <returns></returns>
        public int CreatePackage(string sourceFile, string absoluteDir)
        {
            var package = new DataPackage();
            package.SourceFilePath = Path.Combine(absoluteDir, sourceFile);
            packages.Add(package);
            packagesEncrypted.Add(false);
            return packages.Count - 1;
        }

        /// <summary>
        /// Get the package index for the given file name. If there are more than one
        /// of the same file name across different directories, the first one found
        /// will be returned.
        /// </summary>
        /// <param name="fileName">The file name to look for. Does not require the file extension.</param>
        /// <returns>Returns <c>-1</c> if nothing found, otherwise the index of the package.</returns>
        public int GetPackageIndex(string fileName)
        {
            for (int i = 0; i < packages.Count; i++)
            {
                var sourceFile = Path.GetFileNameWithoutExtension(packages[i].SourceFilePath);
                if (sourceFile == Path.GetFileNameWithoutExtension(fileName))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Add the raw bytes of a binary file into a package at the given package
        /// index and identifier. Identifiers should be unique across all packages.
        /// </summary>
        /// <param name="packageIndex">The index of the package to access.</param>
        /// <param name="identifier">The identifier used as the name for the resource.</param>
        /// <param name="buffer">The raw bytes buffer of the file to add to this package.</param>
        public unsafe void AddResource(int packageIndex, string identifier, byte[] buffer)
        {
            if (packageIndex < 0 || packageIndex >= packages.Count)
            {
                return;
            }

            var package = packages[packageIndex];
            int compSize = 0;
            byte* compressed;

            fixed (byte* ptr = buffer)
            {
                compressed = Raylib.CompressData(ptr, buffer.Length, &compSize);
            }

            Raylib.MemFree(compressed);

            package.FileCursor = package.FileBufferCount;
            package.FileBufferCount += compSize;
            package.FileMapping.Add(identifier, new DataSegment()
            {
                Cursor = package.FileCursor,
                Size = buffer.Length,
                CompressedSize = compSize,
                Data = buffer
            });
        }

        /// <summary>
        /// Add the raw bytes of a binary file into a package at the given package
        /// index and identifier. Identifiers should be unique across all packages.
        /// </summary>
        /// <param name="packageIndex">The index of the package to access.</param>
        /// <param name="identifier">The identifier used as the name for the resource.</param>
        /// <param name="sourceFile">The file path to acquire the bytes.</param>
        public unsafe void AddResource(int packageIndex, string identifier, string sourceFile)
        {
            if (packageIndex < 0 || packageIndex >= packages.Count)
            {
                return;
            }

            var buffer = File.ReadAllBytes(sourceFile);
            var package = packages[packageIndex];
            int compSize = 0;
            byte* compressed;

            fixed (byte* ptr = buffer)
            {
                compressed = Raylib.CompressData(ptr, buffer.Length, &compSize);
            }

            Raylib.MemFree(compressed);


            package.FileCursor = package.FileBufferCount;
            package.FileBufferCount += compSize;
            
            package.FileMapping.Add(identifier, new DataSegment()
            {
                OriginalSourceFile = sourceFile,
                Cursor = package.FileCursor,
                Size = buffer.Length,
                CompressedSize = compSize,
                FileExt = Path.GetExtension(sourceFile)
            });
        }

        /// <summary>
        /// Add an image to memory. Will be lost when the application exits.
        /// </summary>
        /// <param name="identifier">The name of the image. Must be unique.</param>
        /// <param name="image">The image resource to add.</param>
        public void AddResourceImage(string identifier, Image image)
        {
            if (imageCache.ContainsKey(identifier))
            {
                throw new Exception("Identifier with the name '" + identifier + "' already exists.");
            }

            imageCache.Add(identifier , image);
        }

        /// <summary>
        /// Add a Texture2D to memory. Will be lost when the application exits.
        /// </summary>
        /// <param name="identifier">The name of the texture. Must be unique.</param>
        /// <param name="texture">The texture resource to add.</param>
        /// <exception cref="Exception"></exception>
        public void AddResourceTexture(string identifier, Texture2D texture)
        {
            if (textureCache.ContainsKey(identifier))
            {
                throw new Exception("Identifier with the name '" + identifier + "' already exists.");
            }

            textureCache.Add(identifier, texture);
        }

        /// <summary>
        /// Add a RenderTexture to memory. Will be lost when the application exits.
        /// </summary>
        /// <param name="identifier">The name of the texture. Must be unique.</param>
        /// <param name="texture">The texture resource to add.</param>
        /// <exception cref="Exception"></exception>
        public void AddResourceRenderTexture(string identifier, RenderTexture2D texture)
        {
            if (renderTextureCache.ContainsKey(identifier))
            {
                throw new Exception("Identifier with the name '" + identifier + "' already exists.");
            }

            renderTextureCache.Add(identifier, texture);
        }

        /// <summary>
        /// Add a Blob to memory. Will be lost when the application exits.
        /// </summary>
        /// <param name="identifier">The name of the blob. Must be unique.</param>
        /// <param name="blob">The blob resource to add.</param>
        public void AddResourceBlob(string identifier, Blob blob)
        {
            if (blobCache.ContainsKey(identifier))
            {
                throw new Exception("Identifier with the name '" + identifier + "' already exists.");
            }

            blobCache.Add(identifier, blob);
        }

        /// <summary>
        /// Write all the data for the given package.
        /// </summary>
        /// <param name="packageIndex">The package to write out to its source file.</param>
        public unsafe void WriteAll(int packageIndex, Action<string[]> error = null)
        {
            if (packageIndex < 0 || packageIndex >= packages.Count)
            {
                return;
            }

            if (packagesEncrypted[packageIndex])
            {
                throw new Exception("The package you are attempting to write to is encrypted.");
            }

            var package = packages[packageIndex];
            var canProceed = true;
            var filePathsNotFound = new List<string>();
            foreach (var kv in package.FileMapping)
            {
                if (!File.Exists(kv.Value.OriginalSourceFile) && kv.Value.Data == null)
                {
                    canProceed = false;
                    filePathsNotFound.Add(kv.Value.OriginalSourceFile);
                }
            }

            if (!canProceed)
            {
                error?.Invoke(filePathsNotFound.ToArray());
                return;
            }

            using (var stream = new FileStream(package.SourceFilePath, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    int headerSize = sizeof(int);
                    headerSize += sizeof(int) * 2;

                    foreach (var kv in package.FileMapping)
                    {
                        headerSize += sizeof(long) * 3;
                        headerSize += kv.Key.Length + 1;
                        headerSize += kv.Value.FileExt.Length + 1;
                        headerSize += kv.Value.OriginalSourceFile.Length + 1;
                    }

                    package.HeaderSize = headerSize;
                    writer.Write(package.HeaderSize);
                    writer.Write(package.Version);
                    writer.Write(package.FileMapping.Count);

                    foreach (var kv in package.FileMapping)
                    {
                        writer.Write(kv.Key);
                        var data = kv.Value;
                        writer.Write(data.FileExt);
                        writer.Write(data.OriginalSourceFile);
                        writer.Write(data.Cursor);
                        writer.Write(data.Size);
                        writer.Write(data.CompressedSize);
                    }

                    // end header data

                    // write the contents
                    foreach (var kv in package.FileMapping)
                    {
                        var data = kv.Value;
                        if (data.Data == null || data.Data.Length == 0)
                        {
                            var bytes = File.ReadAllBytes(data.OriginalSourceFile);
                            fixed (byte* ptr = bytes)
                            {
                                int dataSize = 0;
                                var compressed = Raylib.CompressData(ptr, bytes.Length, &dataSize);
                                var compressedBuffer = new byte[dataSize];
                                for (int i = 0; i < dataSize; i++)
                                    compressedBuffer[i] = compressed[i];

                                writer.Write(compressedBuffer);
                            }
                        }
                        else
                        {
                            fixed (byte* ptr = data.Data)
                            {
                                int dataSize = 0;
                                var compressed = Raylib.CompressData(ptr, data.Data.Length, &dataSize);
                                var compressedBuffer = new byte[dataSize];
                                for (int i = 0; i < dataSize; i++)
                                    compressedBuffer[i] = compressed[i];

                                writer.Write(compressedBuffer);
                            }

                            data.Data = null;
                        }

                        WriteItemsProgress += 1;
                    }
                }
            }
        }

        public int WriteItemsProgress { get; private set; }
        public int WriteItemsMax { get; private set; }

        /// <summary>
        /// Write all the data for the given package asynchronously.
        /// </summary>
        /// <param name="packageIndex">The package to write out to its source file.</param>
        public async void WriteAllAsync(int packageIndex, Action ?complete = null, Action<string[]> ?error = null)
        {
            WriteItemsMax = packages[packageIndex].FileMapping.Count;
            WriteItemsProgress = 0;

            await Task.Run(() => WriteAll(packageIndex, error));
            complete?.Invoke();
        }

        /// <summary>
        /// Encrypt the contents of the selected package. Once in an encrypted state, it cannot be written
        /// to until decrypted. If encryption has already been performed, this method exits immediately.
        /// </summary>
        /// <param name="packageIndex"></param>
        /// <param name="cryptKey"></param>
        /// <param name="authKey"></param>
        /// <remarks>
        /// Encryption can take a while to perform. This method is best not used outside of the Resource Manager
        /// editor.
        /// </remarks>
        public async void EncryptAsync(int packageIndex, byte[] cryptKey, byte[] authKey, Action? complete = null)
        {
            if (packagesEncrypted[packageIndex])
                return;

            await Task.Run(() => Encrypt(packageIndex, cryptKey, authKey));
            packagesEncrypted[packageIndex] = true;
            var encPath = Path.Combine(AssetDirectory, packages[packageIndex].SourceFilePath, ".enc");

            File.WriteAllText(encPath, "1");
            File.SetAttributes(encPath, FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.NotContentIndexed);

            complete?.Invoke();
        }

        private void Encrypt(int packageIndex, byte[] cryptKey, byte[] authKey)
        {
            var package = packages[packageIndex];
            
        }

        private string readFromDirectory;

        /// <summary>
        /// Read the headers of a given package. This is a convenience method over
        /// reading headers from a series of files, assuming the package is being created
        /// at runtime.
        /// </summary>
        /// <param name="packageIndex">The index of the package to read from.</param>
        public void ReadHeaders(int packageIndex)
        {
            if (packageIndex < 0 || packageIndex > packages.Count - 1)
                return;

            readFromDirectory = Path.GetDirectoryName(packages[packageIndex].SourceFilePath);
            var fileName = Path.GetFileName(packages[packageIndex].SourceFilePath);
            ReadHeaders(fileName);
            readFromDirectory = AssetDirectory;
        }


        /// <summary>
        /// Use this method to read the headers of the given files. If any one of the
        /// files headers have already been processed, it will be ignored. This method
        /// must be used for reading before loading in any files.
        /// </summary>
        /// <param name="files">The file paths (without their directories) to check.</param>
        /// <remarks>
        /// If a file is encrypted, it will use any supplied encryption keys. If the keys
        /// are not supplied, or the keys are invalid, the headers will not be read.
        /// </remarks>
        public void ReadHeaders(params string[] files)
        {
            if (string.IsNullOrEmpty(readFromDirectory))
                readFromDirectory = AssetDirectory;

            for (int i = 0; i < files.Length; i++)
            {
                var found = -1;

                for (int j = 0; j < packages.Count; j++)
                {
                    var package = packages[j];
                    var fileName = Path.GetFileName(package.SourceFilePath);
                    var dir = Path.GetDirectoryName(package.SourceFilePath);
                    var absoluteAssetPath = Path.GetFullPath(AssetDirectory);

                    if (fileName == files[i] && dir == absoluteAssetPath)
                    {
                        found = j;
                        break;
                    }
                }

                if (found == -1)
                {
                    var package = new DataPackage();
                    package.SourceFilePath = files[i];
                    ReadHeader_(package);

                    packagesEncrypted.Add(false);
                    packages.Add(package);
                }
                else
                {
                    var package = packages[found];
                    if (package.FileMapping.Count == 0)
                        ReadHeader_(package);
                }
            }
        }

        private void ReadHeader_(DataPackage package)
        {
            using (FileStream stream = new FileStream(package.SourceFilePath, FileMode.Open, FileAccess.Read))
            {
                if (stream.Length == 0) // empty file
                {
                    goto SKIP_READING;
                }

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    package.HeaderSize = reader.ReadInt32();
                    package.Version = reader.ReadInt32();
                    var count = reader.ReadInt32();
                    for (int j = 0; j < count; j++)
                    {
                        var identifier = reader.ReadString();
                        var segment = new DataSegment();
                        segment.FileExt = reader.ReadString();
                        segment.OriginalSourceFile = reader.ReadString();
                        segment.Cursor = reader.ReadInt64();
                        segment.Size = reader.ReadInt64();
                        segment.CompressedSize = reader.ReadInt64();

                        package.FileCursor += segment.CompressedSize;
                        package.FileMapping.Add(identifier, segment);
                    }
                }

            SKIP_READING:
                { }
            }
        }

        /// <summary>
        /// Re-writes the header information of a given package, without affecting
        /// the content of the package.
        /// </summary>
        /// <param name="packageIndex">The index of the package to re-write.</param>
        public void RewriteHeader(int packageIndex)
        {
            currentlyOpen.Add(packageIndex);
            var package = packages[packageIndex];
            var currentHeaderSize = package.HeaderSize;
            int headerSize = sizeof(int);
            headerSize += sizeof(int) * 2;

            foreach (var kv in package.FileMapping)
            {
                headerSize += sizeof(long) * 3;
                headerSize += kv.Key.Length + 1;
                headerSize += kv.Value.FileExt.Length + 1;
                headerSize += kv.Value.OriginalSourceFile.Length + 1;
            }

            var packageStream = File.OpenRead(Path.Combine(AssetDirectory, package.SourceFilePath));
            var packageLength = packageStream.Length - currentHeaderSize;
            var contentBytes = new byte[packageLength];
            packageStream.Position = currentHeaderSize;
            packageStream.Read(contentBytes, 0, (int)packageLength);
            packageStream.Close();

            using (var stream = new FileStream(Path.Combine(AssetDirectory, package.SourceFilePath), FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    package.HeaderSize = headerSize;
                    writer.Write(package.HeaderSize);
                    writer.Write(package.Version);
                    writer.Write(package.FileMapping.Count);

                    foreach (var kv in package.FileMapping)
                    {
                        writer.Write(kv.Key);
                        var data = kv.Value;
                        writer.Write(data.FileExt);
                        writer.Write(data.OriginalSourceFile);
                        writer.Write(data.Cursor);
                        writer.Write(data.Size);
                        writer.Write(data.CompressedSize);
                    }

                    // end header data

                    // write the contents
                    writer.Write(contentBytes);
                }
            }

            currentlyOpen.Remove(packageIndex);
        }

        /// <summary>
        /// Requests a given identifier is deleted from the given package index.
        /// If the identifier doesn't exist, false is returned.
        /// </summary>
        /// <param name="packageIndex"></param>
        /// <param name="identifier"></param>
        public bool DeleteItem(int packageIndex, string identifier)
        {
            var package = packages[packageIndex];

            var result = package.FileMapping.ContainsKey(identifier);
            if (result && package.ToDelete.FindIndex(s => s == identifier) == -1)
                package.ToDelete.Add(identifier);

            return result;
        }

        /// <summary>
        /// Performs physical deletion of items that have previously been marked for
        /// deletion from the given package index.
        /// </summary>
        /// <param name="packageIndex"></param>
        public void DeleteItemsFromPackage(int packageIndex)
        {
            currentlyOpen.Add(packageIndex);
            var package = packages[packageIndex];
            var itemsToRemove = new List<int>();
            foreach (var remove in package.ToDelete)
            {
                for (int i = 0; i < package.FileMapping.Count; i++)
                {
                    if (remove == package.FileMapping.Keys.ElementAt(i))
                    {
                        itemsToRemove.Add(i);
                        break;
                    }
                }
            }

            itemsToRemove.Sort();

            var index = 0;
            var dataToDelete = 0L;

            for (int i = 0; i < package.FileMapping.Count; i++)
            {
                if (itemsToRemove[index] == i)
                {
                    dataToDelete += package.FileMapping.ElementAt(i).Value.CompressedSize;
                    if (index + 1 < itemsToRemove.Count)
                        index += 1;
                    else
                        break;
                }
            }

            index = 0;
            var streamData = File.OpenRead(Path.Combine(AssetDirectory, package.SourceFilePath));
            var newStreamSize = streamData.Length - package.HeaderSize - dataToDelete;
            var contentBytes = new byte[newStreamSize];
            var offset = 0L;
            var currentTotalWritten = 0L;

            for (int i = 0; i < package.FileMapping.Count; i++)
            {
                var map = package.FileMapping.ElementAt(i);
                if (itemsToRemove[index] != i)
                {
                    streamData.Position = map.Value.Cursor + package.HeaderSize;
                    streamData.Read(contentBytes, (int)currentTotalWritten, (int)map.Value.CompressedSize);
                    currentTotalWritten += map.Value.CompressedSize;

                    map.Value.Cursor -= offset;
                }

                if (itemsToRemove[index] == i)
                {
                    offset += map.Value.CompressedSize;
                    index += 1;
                }
            }

            streamData.Close();

            foreach (var item in package.ToDelete)
            {
                package.FileMapping.Remove(item);
            }

            package.ToDelete.Clear();

            int headerSize = sizeof(int);
            headerSize += sizeof(int) * 2;

            foreach (var kv in package.FileMapping)
            {
                headerSize += sizeof(long) * 3;
                headerSize += kv.Key.Length + 1;
                headerSize += kv.Value.FileExt.Length + 1;
                headerSize += kv.Value.OriginalSourceFile.Length + 1;
            }

            using (var stream = new FileStream(Path.Combine(AssetDirectory, package.SourceFilePath), FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    package.HeaderSize = headerSize;
                    writer.Write(package.HeaderSize);
                    writer.Write(package.Version);
                    writer.Write(package.FileMapping.Count);

                    foreach (var kv in package.FileMapping)
                    {
                        writer.Write(kv.Key);
                        var data = kv.Value;
                        writer.Write(data.FileExt);
                        writer.Write(data.OriginalSourceFile);
                        writer.Write(data.Cursor);
                        writer.Write(data.Size);
                        writer.Write(data.CompressedSize);
                    }

                    // end header data

                    // write the contents
                    writer.Write(contentBytes);
                }
            }

            currentlyOpen.Remove(packageIndex);
        }

        /// <summary>
        /// Read from a package the raw bytes of a given identifier.
        /// If the identifier could not be found among all known packages, success returns <c>false</c>.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="ext">A reference to a string to obtain the file extension of the subject resource.</param>
        /// <param name="size">A reference to an integer determining the size of the resource.</param>
        /// <returns></returns>
        public unsafe byte[] GetBytesFromMemory(string identifier, ref string ext, ref int size, out bool success)
        {
            DataSegment foundData = null;
            DataPackage foundPackage = null;
            var found = false;
            for (int i = 0; i < packages.Count; i++)
            {
                foreach (var open in currentlyOpen)
                    if (open == i)
                        Thread.Sleep(100);

                var package = packages[i];
                if (package.FileMapping.ContainsKey(identifier))
                {
                    found = true;
                    foundPackage = package;
                    foundData = package.FileMapping[identifier];
                    break;
                }
            }

            if (!found)
            {
                success = false;
                return null;
            }

            var fullPath = foundPackage?.SourceFilePath;
            byte[] buffer = new byte[foundData.CompressedSize];

            using (FileStream stream = new(fullPath, FileMode.Open, FileAccess.Read))
            {
                using BinaryReader reader = new(stream);

                var headerSize = reader.ReadInt32();
                reader.ReadBytes((headerSize - sizeof(int)) + (int)foundData.Cursor);
                reader.Read(buffer, 0, (int)foundData.CompressedSize);
            }

            byte* decompressed;
            int decompressedSize;
            fixed (byte* ptr = buffer)
            {
                decompressed = Raylib.DecompressData(ptr, buffer.Length, &decompressedSize);
            }

            ext = foundData.FileExt;
            size = decompressedSize;
            byte[] result = new byte[size];
            for (int i = 0; i < result.Length; i++)
                result[i] = decompressed[i];

            Raylib.MemFree(decompressed);

            success = true;
            return result;
        }

        /// <summary>
        /// Loads an image from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// Any errors are discarded. See the secondary overload for more information.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        public void LoadImage(string identifier)
        {
            LoadImage(identifier, out bool _);
        }

        /// <summary>
        /// Loads an image from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but an Image, or the name
        /// could not be found, success is <c>false</c>.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="success">A value determining if this method succeeded.</param>
        public unsafe void LoadImage(string identifier, out bool success)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                success = false;
                return;
            }

            if (imageCache.ContainsKey(identifier))
            {
                success = false;
                return;
            }

            string fileType = null;
            int size = 0;
            var data = GetBytesFromMemory(identifier, ref fileType, ref size, out success);
            if (!success)
            {
                return;
            }

            fixed (byte* ptrData = data.AsSpan())
            {
                var result = Raylib.LoadImageFromMemory(fileType, data);
                if (result.data == (void*)0)
                {
                    success = false;
                    return;
                }

                imageCache.Add(identifier, result);
            }
        }

        /// <summary>
        /// Get an image with an identifier. Must be loaded before retrieved.
        /// </summary>
        /// <param name="identifier">The identifier of the image to get.</param>
        /// <returns>The image, if found. Returns an empty image if not found.</returns>
        public Image GetImage(string identifier)
        {
            if (imageCache.ContainsKey(identifier))
            {
                return imageCache[identifier];
            }

            return new Image();
        }

        /// <summary>
        /// Unloads the specified identifier, assuming its an image.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        /// <returns>Returns true if successful.</returns>
        public bool UnloadImage(string identifier)
        {
            if (imageCache.ContainsKey(identifier))
            {
                Raylib.UnloadImage(GetImage(identifier));
                imageCache.Remove(identifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a texture from an image with the given identifier. The respective image must be loaded before it is retrieved.
        /// </summary>
        /// <param name="identifier">The identifier of the image to get.</param>
        /// <returns>The image as a texture, if found. Returns an empty texture if not found.</returns>
        public Texture2D GetTexture(string identifier)
        {
            if (textureCache.ContainsKey(identifier))
            {
                return textureCache[identifier];
            }

            var image = GetImage(identifier);
            if (!Equals(image, new Image()))
            {
                var texture = Raylib.LoadTextureFromImage(image);
                textureCache.Add(identifier, texture);
                return texture;
            }

            return new Texture2D();
        }

        /// <summary>
        /// Unloads the specified identifier, assuming its a texture.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        /// <returns>Returns true if successful.</returns>
        public bool UnloadTexture(string identifier)
        {
            if (textureCache.ContainsKey(identifier))
            {
                Raylib.UnloadTexture(textureCache[identifier]);
                textureCache.Remove(identifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a RenderTexture with the given identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the image to get.</param>
        /// <returns></returns>
        public RenderTexture2D GetRenderTexture(string identifier)
        {
            if (renderTextureCache.ContainsKey(identifier))
            {
                return renderTextureCache[identifier];
            }

            return new RenderTexture2D();
        }

        /// <summary>
        /// Unloads the specified identifier, assuming its a render texture.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        /// <returns>Returns true if successful.</returns>
        public bool UnloadRenderTexture(string identifier)
        {
            if (renderTextureCache.ContainsKey(identifier))
            {
                Raylib.UnloadRenderTexture(renderTextureCache[identifier]);
                renderTextureCache.Remove(identifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads an audio file from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// Any errors are discarded. See the secondary overload for more information.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        public void LoadMusic(string identifier)
        {
            LoadMusic(identifier, out bool _);
        }

        /// <summary>
        /// Loads an audio file from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but an Music, or the name
        /// could not be found, success is <c>false</c>.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="success">A value determining if this method succeeded.</param>
        public unsafe void LoadMusic(string identifier, out bool success)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                success = false;
                return;
            }

            if (musicCache.ContainsKey(identifier))
            {
                success = false;
                return;
            }

            string fileType = null;
            int size = 0;
            var data = GetBytesFromMemory(identifier, ref fileType, ref size, out success);
            if (!success)
            {
                return;
            }

            var result = Raylib.LoadMusicStreamFromMemory(fileType, data);
            if (result.ctxData == (void*)0)
            {
                success = false;
                return;
            }
            musicCache.Add(identifier, result);
        }

        /// <summary>
        /// Get a music with an identifier. Must be loaded before retrieved.
        /// </summary>
        /// <param name="identifier">The identifier of the music to get.</param>
        /// <returns>The music, if found. Returns an empty music if not found.</returns>
        public Music GetMusic(string identifier)
        {
            if (musicCache.ContainsKey(identifier))
            {
                return musicCache[identifier];
            }

            return new Music();
        }

        /// <summary>
        /// Unloads the specified identifier, assuming its music.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        /// <returns>Returns true if successful.</returns>
        public bool UnloadMusic(string identifier)
        {
            if (musicCache.ContainsKey(identifier))
            {
                Raylib.UnloadMusicStream(GetMusic(identifier));
                musicCache.Remove(identifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads an audio file from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// Any errors are discarded. See the secondary overload for more information.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        public void LoadWave(string identifier)
        {
            LoadWave(identifier, out bool _);
        }

        /// <summary>
        /// Loads an audio file from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but a Wave, or the name
        /// could not be found, success is <c>false</c>.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="success">A value determining if this method succeeded.</param>
        public unsafe void LoadWave(string identifier, out bool success)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                success = false;
                return;
            }

            if (waveCache.ContainsKey(identifier))
            {
                success = false;
                return;
            }

            string fileType = null;
            int size = 0;
            var data = GetBytesFromMemory(identifier, ref fileType, ref size, out success);
            if (!success)
            {
                return;
            }

            var result = Raylib.LoadWaveFromMemory(fileType, data);
            if (result.data == (void*)0)
            {
                success = false;
                return;
            }
            waveCache.Add(identifier, result);
        }

        /// <summary>
        /// Get a wave with an identifier. Must be loaded before retrieved.
        /// </summary>
        /// <param name="identifier">The identifier of the wave to get.</param>
        /// <returns>The wave, if found. Returns an empty wave if not found.</returns>
        public Wave GetWave(string identifier)
        {
            if (waveCache.ContainsKey(identifier))
            {
                return waveCache[identifier];
            }

            return new Wave();
        }

        /// <summary>
        /// Unloads the specified identifier, assuming its a wave.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        /// <returns>Returns true if successful.</returns>
        public bool UnloadWave(string identifier)
        {
            if (waveCache.ContainsKey(identifier))
            {
                Raylib.UnloadWave(GetWave(identifier));
                waveCache.Remove(identifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads a font from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// Any errors are discarded. See the secondary overload for more information.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="fontSize">The size of the font when it loads.</param>
        /// <param name="fontChars">The characters to be included from the font file. Pass null to include the default character set.</param>
        public void LoadFont(string identifier, int fontSize, int[] fontChars = null)
        {
            LoadFont(identifier, fontSize, out bool _, fontChars);
        }

        /// <summary>
        /// Loads a font from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but a Font, or the name
        /// could not be found, success is <c>false</c>.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="fontSize">The size of the font when it loads.</param>
        /// <param name="success">A value determining if this method succeeded.</param>
        /// <param name="fontChars">The characters to be included from the font file. Pass null to include the default character set.</param>
        public unsafe void LoadFont(string identifier, int fontSize, out bool success, int[] fontChars = null)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                success = false;
                return;
            }

            if (fontCache.ContainsKey(identifier))
            {
                success = false;
                return;
            }

            string fileType = null;
            int size = 0;

            var data = GetBytesFromMemory(identifier, ref fileType, ref size, out success);
            if (!success)
            {
                return;
            }

            var result = Raylib.LoadFontFromMemory(fileType, data, fontSize, fontChars, fontChars == null ? 0 : fontChars.Length);
            if (result.texture.id == 0)
            {
                success = false;
                return;
            }
            fontCache.Add(identifier + ":" + fontSize, result);
        }

        /// <summary>
        /// Get a font with an identifier. Must be loaded before retrieved.
        /// </summary>
        /// <param name="identifier">The identifier of the font to get.</param>
        /// <returns>The font, if found. Returns an empty font if not found.</returns>
        public Font GetFont(string identifier)
        {
            if (fontCache.ContainsKey(identifier))
            {
                return fontCache[identifier];
            }

            return new Font();
        }

        /// <summary>
        /// Unloads the specified identifier, assuming its a font.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        /// <returns>Returns true if successful.</returns>
        public bool UnloadFont(string identifier)
        {
            if (fontCache.ContainsKey(identifier))
            {
                Raylib.UnloadFont(GetFont(identifier));
                fontCache.Remove(identifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads a blob from a given identifier.
        /// 
        /// If the name of the identifier gives anything but a Blob, or the name
        /// could not be found, success is <c>false</c>.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="success">A value determining if this method succeeded.</param>
        public unsafe void LoadBlob(string identifier, out bool success)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                success = false;
                return;
            }

            if (fontCache.ContainsKey(identifier))
            {
                success = false;
                return;
            }

            string fileType = null;
            int size = 0;

            var data = GetBytesFromMemory(identifier, ref fileType, ref size, out success);
            if (!success)
            {
                return;
            }

            var result = new Blob();
            result.LoadFromMemory(data);
            blobCache.Add(identifier, result);
        }

        /// <summary>
        /// Get a blob with an identifier. Must be loaded before retrieved.
        /// </summary>
        /// <param name="identifier">The identifier of the blob to get.</param>
        /// <returns>The blob, if found. Returns <c>null</c> if not found.</returns>
        public Blob? GetBlob(string identifier)
        {
            if (blobCache.ContainsKey(identifier))
            {
                return blobCache[identifier];
            }

            return null;
        }

        /// <summary>
        /// Unloads the specified identifier, assuming its a blob.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        /// <returns>Returns true if successful.</returns>
        public bool UnloadBlob(string identifier)
        {
            if (blobCache.ContainsKey(identifier))
            {
                blobCache.Remove(identifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks all packages to determine if the given identifier exists.
        /// </summary>
        /// <param name="identifier">The identifier to check.</param>
        /// <returns></returns>
        public bool DoesNameExist(string identifier)
        {
            var result = false;
            foreach (var pack in packages)
            {
                result = pack.FileMapping.ContainsKey(identifier);
                if (result)
                    break;
            }
            return result;
        }

        /// <summary>
        /// Checks the internal cache for the given identifier, returning <c>true</c> if one exists.
        /// </summary>
        /// <param name="identifier">The name of the identifier.</param>
        /// <returns></returns>
        public bool DoesIdentifierExist(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return false;

            var result = false;
            result = imageCache.ContainsKey(identifier);
            if (result)
                return result;

            result = musicCache.ContainsKey(identifier);
            if (result)
                return result;

            result = waveCache.ContainsKey(identifier);
            if (result)
                return result;

            // special case for font cache, as we may not know
            // the specific size to look for.
            foreach (var key in fontCache.Keys)
            { 
                if (key.StartsWith(identifier))
                {
                    result = true;
                    break;
                }
            }

            if (result)
                return result;

            result = textureCache.ContainsKey(identifier);
            if (result)
                return result;

            result = blobCache.ContainsKey(identifier);

            return result;
        }

        /// <summary>
        /// Gets the package and file mapping index of the given identifier.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <returns></returns>
        public ResourceIndex GetResourceIndex(string identifier)
        {
            for (int i = 0; i < packages.Count; i++)
            {
                var package = packages[i];
                for (int j = 0; j < package.FileMapping.Count; j++)
                {
                    var key = package.FileMapping.ElementAt(j);
                    if (key.Key == identifier)
                    {
                        return new ResourceIndex(i, j);
                    }
                }
            }

            return new ResourceIndex(-1, -1);
        }

        /// <summary>
        /// Loads a group of resources synchronously on the main thread. Use sparingly.
        /// </summary>
        /// <param name="group">The defined group containing the resources to load.</param>
        /// <returns>The loaded resources.</returns>
        public void LoadGroup(ResourceGroup group)
        {
            if (group.RequestedFonts.Count > 0)
            {
                foreach (var font in group.RequestedFonts)
                {
                    var splitted = font.Split(':');
                    if (splitted.Length <= 1)
                    {
                        continue;
                    }

                    LoadFont(splitted[0], int.Parse(splitted[1]), out bool _);
                }
            }

            if (group.RequestedImages.Count > 0)
            {
                foreach (var image in group.RequestedImages)
                {
                    LoadImage(image, out bool _);
                }
            }

            if (group.RequestedMusic.Count > 0)
            {
                foreach (var music in group.RequestedMusic)
                {
                    LoadMusic(music, out bool _);
                }
            }

            if (group.RequestedWaves.Count > 0)
            {
                foreach (var wave in group.RequestedWaves)
                {
                    LoadWave(wave, out bool _);
                }
            }

            if (group.RequestedBlobs.Count > 0)
            {
                foreach (var blob in group.RequestedBlobs)
                {
                    LoadBlob(blob, out bool _);
                }
            }
        }

        /// <summary>
        /// Loads a group of resources asynchronously. Best used for loading screens.
        /// </summary>
        /// <param name="group">The defined group containing the resources to load.</param>
        /// <returns>The loaded resources.</returns>
        public async Task LoadGroupAsync(ResourceGroup group)
        {
            await new Task(CreateLoadGroupAsyncMethod(group), group);
        }

        private Action<object?> CreateLoadGroupAsyncMethod(ResourceGroup group)
        {
            var action = new Action<object?>(LoadGroupAsync_);
            return action;
        }

        private void LoadGroupAsync_(object? group)
        {
            if (group != null)
            {
                LoadGroup((ResourceGroup)group);
            }
        }

        /// <summary>
        /// Requests to unload the specified identifier, irrespective of its actual
        /// format, and removes it from the cache within which its contained. When
        /// unloading a font by an identifier without its size specified, all fonts
        /// relating to the same identifier is unloaded, regardless of size.
        /// </summary>
        /// <param name="identifier">The identifier to scan for.</param>
        public void Unload(string identifier)
        {
            bool success;
            if (identifier.Contains(':'))
            {
                success = UnloadFont(identifier);
            }
            else
            {
                var toRemove = new List<string>();
                foreach (var key in fontCache.Keys)
                {
                    if (key.StartsWith(identifier))
                    {
                        toRemove.Add(key);
                    }
                }

                success = toRemove.Count > 0;
                if (success)
                {
                    foreach (var rem in toRemove)
                    {
                        fontCache.Remove(rem);
                    }
                }
            }

            if (!success)
                success = UnloadImage(identifier);
            if (!success)
                success = UnloadMusic(identifier);
            if (!success)
                success = UnloadRenderTexture(identifier);
            if (!success)
                success = UnloadTexture(identifier);
            if (!success)
                success = UnloadWave(identifier);
            if (!success)
                UnloadBlob(identifier);
        }

        /// <summary>
        /// Unload a group of resources.
        /// </summary>
        /// <param name="resources">The resources to load.</param>
        public void UnloadResources(ResourceGroup resources)
        {
            if (resources.RequestedFonts.Count > 0)
            {
                foreach (var font in resources.RequestedFonts)
                {
                    if (fontCache.ContainsKey(font))
                    {
                        Raylib.UnloadFont(fontCache[font]);
                        fontCache.Remove(font);
                    }
                }
            }
            
            if (resources.RequestedImages.Count > 0)
            {
                foreach (var image in resources.RequestedImages)
                {
                    if (imageCache.ContainsKey(image))
                    {
                        Raylib.UnloadImage(imageCache[image]);
                        imageCache.Remove(image);
                    }
                }
            }

            if (resources.RequestedMusic.Count > 0)
            {
                foreach (var music in resources.RequestedMusic)
                {
                    if (musicCache.ContainsKey(music))
                    {
                        Raylib.UnloadMusicStream(musicCache[music]);
                        musicCache.Remove(music);
                    }
                }
            }

            if (resources.RequestedWaves.Count > 0)
            {
                foreach (var wav in resources.RequestedWaves)
                {
                    if (waveCache.ContainsKey(wav))
                    {
                        Raylib.UnloadWave(waveCache[wav]);
                        waveCache.Remove(wav);
                    }
                }
            }

            if (resources.RequestedBlobs.Count > 0)
            {
                foreach (var blob in resources.RequestedBlobs)
                {
                    if (blobCache.ContainsKey(blob))
                    {
                        blobCache.Remove(blob);
                    }
                }
            }
        }

        /// <summary>
        /// Unload all loaded resources.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var kv in fontCache)
            {
                Raylib.UnloadFont(kv.Value);
            }
            fontCache.Clear();

            foreach (var kv in musicCache)
            {
                Raylib.UnloadMusicStream(kv.Value);
            }
            musicCache.Clear();

            foreach (var kv in waveCache)
            {
                Raylib.UnloadWave(kv.Value);
            }
            waveCache.Clear();

            foreach (var kv in imageCache)
            {
                Raylib.UnloadImage(kv.Value);
            }
            imageCache.Clear();

            foreach (var kv in textureCache)
            {
                Raylib.UnloadTexture(kv.Value);
            }
            textureCache.Clear();

            foreach (var kv in renderTextureCache)
            {
                Raylib.UnloadRenderTexture(kv.Value);
            }
            renderTextureCache.Clear();

            blobCache.Clear();
        }

    }
}
