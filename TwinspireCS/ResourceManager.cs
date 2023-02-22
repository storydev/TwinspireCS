using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public class ResourceManager
    {

        private List<DataPackage> packages;
        /// <summary>
        /// Gets the packages that exist in this resource manager.
        /// </summary>
        public IEnumerable<DataPackage> Packages => packages;

        public ResourceManager()
        {
            packages = new List<DataPackage>();
        }

        /// <summary>
        /// Create a package from which a binary file is read. This method opens
        /// a file stream for editing for the given source file. Packages must be
        /// closed before exiting.
        /// </summary>
        /// <param name="sourceFile">The binary source file to read from.</param>
        public int CreatePackage(string sourceFile)
        {
            var package = new DataPackage();
            package.SourceFilePath = sourceFile;
            package.RawStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Write);
            packages.Add(package);
            return packages.Count - 1;
        }

        /// <summary>
        /// Close a package stream at a given index.
        /// </summary>
        /// <param name="packageIndex">The index of the package to close its' file stream.</param>
        public void ClosePackage(int packageIndex)
        {
            if (packageIndex < 0 || packageIndex >= packages.Count)
            {
                return;
            }

            var package = packages[packageIndex];
            package.RawStream?.Close();
        }

        /// <summary>
        /// Add the raw bytes of a binary file into a package at the given package
        /// index and identifier. Identifiers should be unique across all packages.
        /// </summary>
        /// <param name="packageIndex">The index of the package to access.</param>
        /// <param name="identifier">The identifier used as the name for the resource.</param>
        /// <param name="buffer">The raw bytes buffer of the file to add to this package.</param>
        public void AddResource(int packageIndex, string identifier, byte[] buffer)
        {
            if (packageIndex < 0 || packageIndex >= packages.Count)
            {
                return;
            }

            var package = packages[packageIndex];
            package.FileCursor = package.FileBufferCount;
            package.FileBufferCount += buffer.LongLength;
            package.FileMapping.Add(identifier, new long[] { package.FileCursor, buffer.LongLength });

            if (package.RawStream != null)
            {
                package.RawStream.Write(buffer, (int)package.FileCursor, (int)package.FileBufferCount);
            }
        }

        /// <summary>
        /// Loads an image from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but an Image, or the name
        /// could not be found, an exception is thrown.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <returns>A Raylib associated Image.</returns>
        public unsafe Image LoadImage(string identifier)
        {
            long[] foundBytes = null;
            DataPackage foundPackage = null;
            var found = false;
            foreach (var package in packages)
            {
                if (package.FileMapping.ContainsKey(identifier))
                {
                    found = true;
                    foundPackage = package;
                    foundBytes = package.FileMapping[identifier];
                    break;
                }
            }

            if (!found)
                throw new Exception("Identifier could not be found. ID: " + identifier);

            var fileExt = Path.GetExtension(foundPackage?.SourceFilePath);
            var fileData = File.OpenRead(foundPackage?.SourceFilePath);
            byte[] buffer = new byte[foundBytes[1]];
            fileData.Read(buffer, (int)foundBytes[0], (int)foundBytes[1]);
            fileData.Close();

            var fileType = Utils.GetSByteFromString(fileExt);
            var ptrData = Utils.GetBytePtrFromArray(buffer);

            try
            {
                var result = Raylib.LoadImageFromMemory(fileType, ptrData, (int)foundBytes[1]);
                return result;
            }
            catch
            {
                throw new Exception("Unable to get image from identifier. Format from binary file is incorrect.");
            }
        }

        /// <summary>
        /// Loads an audio file from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but an Music, or the name
        /// could not be found, an exception is thrown.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <returns>A Raylib associated Music.</returns>
        public unsafe Music LoadMusic(string identifier)
        {
            long[] foundBytes = null;
            DataPackage foundPackage = null;
            var found = false;
            foreach (var package in packages)
            {
                if (package.FileMapping.ContainsKey(identifier))
                {
                    found = true;
                    foundPackage = package;
                    foundBytes = package.FileMapping[identifier];
                    break;
                }
            }

            if (!found)
                throw new Exception("Identifier could not be found. ID: " + identifier);

            var fileExt = Path.GetExtension(foundPackage?.SourceFilePath);
            var fileData = File.OpenRead(foundPackage?.SourceFilePath);
            byte[] buffer = new byte[foundBytes[1]];
            fileData.Read(buffer, (int)foundBytes[0], (int)foundBytes[1]);
            fileData.Close();

            var fileType = Utils.GetSByteFromString(fileExt);
            var ptrData = Utils.GetBytePtrFromArray(buffer);

            try
            {
                var result = Raylib.LoadMusicStreamFromMemory(fileType, ptrData, (int)foundBytes[1]);
                return result;
            }
            catch
            {
                throw new Exception("Unable to get music data from identifier. Format from binary file is incorrect.");
            }
        }

        /// <summary>
        /// Loads an audio file from a given identifier. This method will scan all known
        /// packages for the given identifier until a name has been found.
        /// 
        /// If the name of the identifier gives anything but a Wave, or the name
        /// could not be found, an exception is thrown.
        /// </summary>
        /// <param name="identifier">The name of the resource to find.</param>
        /// <returns>A Raylib associated Wave.</returns>
        public unsafe Wave LoadWave(string identifier)
        {
            long[] foundBytes = null;
            DataPackage foundPackage = null;
            var found = false;
            foreach (var package in packages)
            {
                if (package.FileMapping.ContainsKey(identifier))
                {
                    found = true;
                    foundPackage = package;
                    foundBytes = package.FileMapping[identifier];
                    break;
                }
            }

            if (!found)
                throw new Exception("Identifier could not be found. ID: " + identifier);

            var fileExt = Path.GetExtension(foundPackage?.SourceFilePath);
            var fileData = File.OpenRead(foundPackage?.SourceFilePath);
            byte[] buffer = new byte[foundBytes[1]];
            fileData.Read(buffer, (int)foundBytes[0], (int)foundBytes[1]);
            fileData.Close();

            var fileType = Utils.GetSByteFromString(fileExt);
            var ptrData = Utils.GetBytePtrFromArray(buffer);

            try
            {
                var result = Raylib.LoadWaveFromMemory(fileType, ptrData, (int)foundBytes[1]);
                return result;
            }
            catch
            {
                throw new Exception("Unable to get wave data from identifier. Format from binary file is incorrect.");
            }
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
        /// <returns>A Raylib associated Font.</returns>
        public unsafe Font LoadFont(string identifier, int fontSize, int[] fontChars = null)
        {
            long[] foundBytes = null;
            DataPackage foundPackage = null;
            var found = false;
            foreach (var package in packages)
            {
                if (package.FileMapping.ContainsKey(identifier))
                {
                    found = true;
                    foundPackage = package;
                    foundBytes = package.FileMapping[identifier];
                    break;
                }
            }

            if (!found)
                throw new Exception("Identifier could not be found. ID: " + identifier);

            var fileExt = Path.GetExtension(foundPackage?.SourceFilePath);
            var fileData = File.OpenRead(foundPackage?.SourceFilePath);
            byte[] buffer = new byte[foundBytes[1]];
            fileData.Read(buffer, (int)foundBytes[0], (int)foundBytes[1]);
            fileData.Close();

            var fileType = Utils.GetSByteFromString(fileExt);
            var ptrData = Utils.GetBytePtrFromArray(buffer);

            int* fontCharPtr = null;
            int glyphs = 0;
            if (fontChars != null)
            {
                fixed (int* tempPtr = fontChars)
                {
                    fontCharPtr = tempPtr;
                }
                glyphs = fontChars.Length;
            }

            try
            {
                var result = Raylib.LoadFontFromMemory(fileType, ptrData, (int)foundBytes[1], fontSize, fontCharPtr, glyphs);
                return result;
            }
            catch
            {
                throw new Exception("Unable to get font from identifier. Format from binary file is incorrect.");
            }
        }

        

    }
}
