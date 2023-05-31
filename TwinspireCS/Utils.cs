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
