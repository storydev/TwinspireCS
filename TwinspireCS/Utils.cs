using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    internal unsafe class Utils
    {


        /// <summary>
        /// Function taken from: https://stackoverflow.com/a/26062198
        /// </summary>
        /// <param name="path"></param>
        /// <param name="IncludeFileName"></param>
        /// <param name="RequireFileName"></param>
        /// <returns></returns>
        public static bool ValidateFilePath(string path, bool IncludeFileName, bool RequireFileName = false, bool relativePath = false)
        {
            if (string.IsNullOrEmpty(path)) { return false; }
            string root = null;
            string directory = null;
            string filename = null;
            try
            {
                // throw ArgumentException - The path parameter contains invalid characters, is empty, or contains only white spaces.
                root = Path.GetPathRoot(path);

                // throw ArgumentException - path contains one or more of the invalid characters defined in GetInvalidPathChars.
                // -or- String.Empty was passed to path.
                directory = Path.GetDirectoryName(path);

                // path contains one or more of the invalid characters defined in GetInvalidPathChars
                if (IncludeFileName) { filename = Path.GetFileName(path); }
            }
            catch (ArgumentException)
            {
                return false;
            }

            // null if path is null, or an empty string if path does not contain root directory information
            if (string.IsNullOrEmpty(root) && !relativePath) { return false; }

            // null if path denotes a root directory or is null. Returns String.Empty if path does not contain directory information
            if (string.IsNullOrEmpty(directory)) { return false; }

            if (RequireFileName)
            {
                // if the last character of path is a directory or volume separator character, this method returns String.Empty
                if (string.IsNullOrEmpty(filename)) { return false; }

                // check for illegal chars in filename
                if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) { return false; }
            }
            return true;
        }

        public static sbyte* GetSByteFromString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            Span<byte> buffer = bytes.AsSpan();
            fixed (byte* ptr = buffer)
            {
                sbyte* sp = (sbyte*)ptr;
                return sp;
            }
        }

        public static byte* GetBytePtrFromArray(byte[] array)
        {
            fixed (byte* ptr = array)
            {
                return ptr;
            }
        }

        public static void Warn(string message, int internalCallCount)
        {
            var callstack = new StackTrace(true);
            var frames = callstack.GetFrames();
            // go x frames out to actual call from the user
            var fileName = frames[internalCallCount].GetFileName();
            var lineNumber = frames[internalCallCount].GetFileLineNumber();

            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("WARNING: ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(string.Format("[{0}, {1}]: ", fileName, lineNumber));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ForegroundColor = temp;
        }

    }
}
