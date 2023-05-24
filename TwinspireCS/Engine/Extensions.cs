using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine
{
    static class Extensions
    {

        public static string GetCharsAsString(this char[] array, int from, int to)
        {
            string result = string.Empty;
            if (to > array.Length)
            {
                to = array.Length;
            }

            if (from < 0)
            {
                from = 0;
            }

            for (int i = from; i < to; i++)
            {
                result += array[i];
            }
            return result;
        }

    }
}
