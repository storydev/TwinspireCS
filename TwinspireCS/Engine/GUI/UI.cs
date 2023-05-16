using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.GUI
{
    public class UI
    {

        private static List<Canvas> canvases;
        private static int currentCanvasIndex;
        private static int nextCanvasIndex;
        private static bool allowInput;

        public static Canvas CreateCanvas()
        {
            Canvas canvas = new Canvas();
            canvases.Add(canvas);
            return canvases[canvases.Count - 1];
        }



        public static void Render()
        {
            if (currentCanvasIndex == -1)
                return;

            
        }

    }
}
