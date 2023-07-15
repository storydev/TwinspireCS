using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            AssetDirectory = string.Empty;
        }

        /// <summary>
        /// Create a package from which a binary file is read.
        /// </summary>
        /// <param name="sourceFile">The binary source file to read from.</param>
        public int CreatePackage(string sourceFile)
        {
            var package = new DataPackage();
            package.SourceFilePath = sourceFile;
            packages.Add(package);
            packagesEncrypted.Add(false);
            return packages.Count - 1;
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

            var outdir = AssetDirectory;

            if (!Directory.Exists(outdir))
                Directory.CreateDirectory(outdir);

            var package = packages[packageIndex];
            var canProceed = true;
            var filePathsNotFound = new List<string>();
            foreach (var kv in package.FileMapping)
            {
                if (!File.Exists(kv.Value.OriginalSourceFile))
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

            using (var stream = new FileStream(Path.Combine(outdir, package.SourceFilePath), FileMode.Create))
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
                            writer.Write(data.Data);
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

        /// <summary>
        /// Use this method to read the headers of the given files. If any one of the
        /// files headers have already been processed, it will be ignored. This method
        /// must be used for reading before loading in any files.
        /// </summary>
        /// <param name="files">The file paths (without their directories) to check.</param>
        /// <remarks>
        /// If a file is encrypted, it will use any supplied encryption keys. If the keys
        /// are not supplied, or the the keys are invalid, the headers will not be read.
        /// </remarks>
        public void ReadHeaders(params string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                var path = Path.Combine(AssetDirectory, files[i]);
                var found = false;

                foreach (var package in packages)
                {
                    if (package.SourceFilePath == files[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    var package = new DataPackage();
                    package.SourceFilePath = files[i];
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
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

                    packagesEncrypted.Add(false);
                    packages.Add(package);
                }
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
        /// If the identifier could not be found among all known packages, this function
        /// throws an exception.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="ext">A reference to a pointer of <c>sbyte</c> to obtain the file extension of the subject resource.</param>
        /// <param name="size">A reference to an integer determining the size of the resource.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws if an identifier could not be found.</exception>
        public unsafe byte[] GetBytesFromMemory(string identifier, ref string ext, ref int size)
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
                throw new Exception("Identifier could not be found. ID: " + identifier);

            var fullPath = Path.Combine(AssetDirectory, foundPackage?.SourceFilePath);
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

            return result;
        }

        /// <summary>
        /// Loads an image from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but an Image, or the name
        /// could not be found, an exception is thrown.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        public unsafe void LoadImage(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return;

            if (imageCache.ContainsKey(identifier))
            {
                return;
            }

            string fileType = null;
            int size = 0;
            var data = GetBytesFromMemory(identifier, ref fileType, ref size);
            fixed (byte* ptrData = data.AsSpan())
            {
                try
                {
                    var result = Raylib.LoadImageFromMemory(fileType, data);
                    imageCache.Add(identifier, result);
                }
                catch
                {
                    throw new Exception("Unable to get image from identifier. Format from binary file is incorrect.");
                }
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
        /// If the name of the identifier gives anything but an Music, or the name
        /// could not be found, an exception is thrown.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        public unsafe void LoadMusic(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return;

            if (musicCache.ContainsKey(identifier))
            {
                return;
            }

            string fileType = null;
            int size = 0;
            var data = GetBytesFromMemory(identifier, ref fileType, ref size);
            try
            {
                var result = Raylib.LoadMusicStreamFromMemory(fileType, data);
                musicCache.Add(identifier, result);
            }
            catch
            {
                throw new Exception("Unable to get music data from identifier. Format from binary file is incorrect.");
            }
            
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
        /// If the name of the identifier gives anything but a Wave, or the name
        /// could not be found, an exception is thrown.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        public unsafe void LoadWave(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return;

            if (waveCache.ContainsKey(identifier))
            {
                return;
            }

            string fileType = null;
            int size = 0;
            var data = GetBytesFromMemory(identifier, ref fileType, ref size);
            try
            {
                var result = Raylib.LoadWaveFromMemory(fileType, data);
                waveCache.Add(identifier, result);
            }
            catch
            {
                throw new Exception("Unable to get wave data from identifier. Format from binary file is incorrect.");
            }
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
        /// If the name of the identifier gives anything but a Font, or the name
        /// could not be found, an exception is thrown.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <param name="fontSize">The size of the font when it loads.</param>
        /// <param name="fontChars">The characters to be included from the font file. Pass null to include the default character set.</param>
        public unsafe void LoadFont(string identifier, int fontSize, int[] fontChars = null)
        {
            if (string.IsNullOrEmpty(identifier))
                return;

            if (fontCache.ContainsKey(identifier))
            {
                return;
            }

            string fileType = null;
            int size = 0;

            var data = GetBytesFromMemory(identifier, ref fileType, ref size);

            try
            {
                var result = Raylib.LoadFontFromMemory(fileType, data, fontSize, fontChars, fontChars == null ? 0 : fontChars.Length);
                fontCache.Add(identifier + ":" + fontSize, result);
            }
            catch
            {
                throw new Exception("Unable to get font from identifier. Format from binary file is incorrect.");
            }
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

                    LoadFont(splitted[0], int.Parse(splitted[1]));
                }
            }

            if (group.RequestedImages.Count > 0)
            {
                foreach (var image in group.RequestedImages)
                {
                    LoadImage(image);
                }
            }

            if (group.RequestedMusic.Count > 0)
            {
                foreach (var music in group.RequestedMusic)
                {
                    LoadMusic(music);
                }
            }

            if (group.RequestedWaves.Count > 0)
            {
                foreach (var wave in group.RequestedWaves)
                {
                    LoadWave(wave);
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
            await new Task(() => LoadGroup(group));
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
                UnloadWave(identifier);
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

            foreach (var kv in musicCache)
            {
                Raylib.UnloadMusicStream(kv.Value);
            }

            foreach (var kv in waveCache)
            {
                Raylib.UnloadWave(kv.Value);
            }

            foreach (var kv in imageCache)
            {
                Raylib.UnloadImage(kv.Value);
            }

            foreach (var kv in textureCache)
            {
                Raylib.UnloadTexture(kv.Value);
            }

            foreach (var kv in renderTextureCache)
            {
                Raylib.UnloadRenderTexture(kv.Value);
            }
        }

    }
}
