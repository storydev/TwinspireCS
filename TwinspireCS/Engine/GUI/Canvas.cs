using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class Canvas
    {

        private List<Grid> layouts;
        private List<ElementType> elements;
        private List<object> elementContent;

        public IEnumerable<Grid> Layouts => layouts;
        public IEnumerable<ElementType> Elements => elements;
        public IEnumerable<object> ElementContent => elementContent;


        public Canvas()
        {
            layouts = new List<Grid>();
            elements = new List<ElementType>();
            elementContent = new List<object>();
        }

        public Grid CreateLayout(Vector4 dimension, int columns, int rows)
        {
            var grid = new Grid();
            grid.Dimension = dimension;
            int totalCells = columns * rows;
            grid.Columns = new float[totalCells];
            grid.Rows = new float[totalCells];
            grid.Margin = new float[totalCells];
            grid.Padding = new float[totalCells];

            layouts.Add(grid);
            return layouts[layouts.Count - 1];
        }

    }
}
