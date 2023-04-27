using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

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

        public bool UseBackBuffer { get; set; }

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
            grid.Columns = new float[columns];
            grid.Rows = new float[rows];
            grid.Margin = new float[totalCells];
            grid.Padding = new float[totalCells];
            grid.BackgroundColors = new Extras.ColorMethod[totalCells];
            grid.BackgroundImages = new string[totalCells];
            grid.Offsets = new Vector2[totalCells];

            int cornerTotals = totalCells * 4;
            grid.BorderColors = new Color[cornerTotals];
            grid.Borders = new bool[cornerTotals];
            grid.BorderThicknesses = new int[cornerTotals];
            grid.RadiusCorners = new float[cornerTotals];

            layouts.Add(grid);
            return layouts[layouts.Count - 1];
        }

    }
}
