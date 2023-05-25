using Raylib_cs;
using System;
using System.Collections.Generic;
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

        public static void ImageBlurGaussian(ref Image image, int blurSize)
        {
            fixed (Image* p = &image)
            {
                Raylib.ImageBlurGaussian(p, blurSize);
            }
        }

    }
}
