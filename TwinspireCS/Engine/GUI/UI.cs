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

        public static int CreateCanvas()
        {
            if (canvases == null)
                canvases = new List<Canvas>();

            Canvas canvas = new Canvas();
            canvases.Add(canvas);
            return canvases.Count - 1;
        }

        public static Canvas GetCanvas(int canvasIndex)
        {
            return canvases[canvasIndex];
        }

        public static void ShowCanvas(int canvasIndex)
        {
            currentCanvasIndex = canvasIndex;
        }

        public static void Render()
        {
            if (currentCanvasIndex == -1)
                return;

            canvases[currentCanvasIndex].Render();
        }

    }
}
