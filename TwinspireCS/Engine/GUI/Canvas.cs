using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class Canvas
    {

        private List<Grid> layouts;
        private List<int> elements;
        private List<object> elementContent;

        public Canvas()
        {
            layouts = new List<Grid>();
            elements = new List<int>();
            elementContent = new List<object>();
        }


    }
}
