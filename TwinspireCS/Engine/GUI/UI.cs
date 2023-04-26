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

        public static Canvas CreateCanvas()
        {
            Canvas canvas = new Canvas();
            canvases.Add(canvas);
            return canvases[canvases.Count - 1];
        }



        public void Render()
        {
            
        }

    }
}
