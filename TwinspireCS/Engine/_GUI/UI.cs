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

        public static int CurrentCanvas => currentCanvasIndex;

        public static InterfaceBuilder InterfaceBuilder { get; private set; }

        public static void Init()
        {
            InterfaceBuilder = new InterfaceBuilder();
        }

        public static int CreateCanvas(string name)
        {
            if (canvases == null)
                canvases = new List<Canvas>();

            Canvas canvas = new Canvas();
            canvas.Name = name;
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

        public static Rectangle CentreRectangleHorizontally(Rectangle a, Rectangle b)
        {
            return new Rectangle(((a.width - b.width) / 2) + a.x, b.y, b.width, b.height);
        }

        public static Rectangle CentreRectangleVertically(Rectangle a, Rectangle b)
        {
            return new Rectangle(b.x, ((a.height - b.height) / 2) + b.y, b.width, b.height);
        }

        public static Rectangle CentreRectangle(Rectangle a, Rectangle b)
        {
            var result = CentreRectangleHorizontally(a, b);
            result = CentreRectangleVertically(a, result);
            return result;
        }

    }
}
