using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.GUI
{
    internal static class RaylibExtensions
    {

        public static string ToString(this Rectangle a, bool formatted)
        {
            if (formatted)
            {
                return string.Format("{0},{1},{2},{3}", a.X, a.Y, a.Width, a.Height);
            }

            return string.Empty;
        }

    }
}
