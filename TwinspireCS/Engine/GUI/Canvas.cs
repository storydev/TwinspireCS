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

        private bool preloadedAll;
        private List<ElementType> elements;
        private List<object> elementContent;
        private List<Vector4> elementDim;
        private List<Vector2> gridCellPosition;
        private List<string> elementFontName;
        private IDictionary<int, int[]> elementCellGroups;
        private int startingGrid;
        private List<int> gridsToChange;

        public IEnumerable<Grid> Layouts => layouts;
        public IEnumerable<ElementType> Elements => elements;
        public IEnumerable<object> ElementContent => elementContent;

        public bool TextWrapping { get; set; } = true;

        public bool AutoCrop { get; set; } = true;

        public bool UseBackBuffer { get; set; }

        public Canvas()
        {
            layouts = new List<Grid>();
            elements = new List<ElementType>();
            elementContent = new List<object>();
            elementCellGroups = new Dictionary<int, int[]>();
            elementFontName = new List<string>();
        }

        public int CreateLayout(Vector4 dimension, int columns, int rows)
        {
            var grid = new Grid();
            grid.Dimension = dimension;
            int totalCells = columns * rows;
            grid.Columns = new float[columns];
            grid.Rows = new float[rows];
            
            grid.BackgroundColors = new Extras.ColorMethod[totalCells];
            grid.BackgroundImages = new string[totalCells];
            grid.Offsets = new Vector2[totalCells];

            int cornerTotals = totalCells * 4;
            grid.Margin = new float[cornerTotals];
            grid.Padding = new float[cornerTotals];
            grid.BorderColors = new Color[cornerTotals];
            grid.Borders = new bool[cornerTotals];
            grid.BorderThicknesses = new int[cornerTotals];
            grid.RadiusCorners = new float[cornerTotals];

            layouts.Add(grid);
            elementCellGroups.Add(layouts.Count - 1, new int[2] { elements.Count - 1, 0 });
            return layouts.Count - 1;
        }

        public void PreloadAll()
        {
            foreach (var layout in layouts)
            {
                int cells = layout.Columns.Length * layout.Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    if (!string.IsNullOrEmpty(layout.BackgroundImages[i]) && Application.Instance.ResourceManager.DoesNameExist(layout.BackgroundImages[0]))
                    {
                        Application.Instance.ResourceManager.LoadImage(layout.BackgroundImages[i]);
                    }
                }
            }

            preloadedAll = true;
        }

        public void Render()
        {
            
            foreach (var layout in layouts)
            {
                RenderLayout(layout);
            }

        }

        public void RenderLayout(Grid grid)
        {
            int cells = grid.Columns.Length * grid.Rows.Length;

            for (int i = 0; i < cells; i++)
            {
                var cellDim = grid.GetCellDimension(i);
                var contentDim = grid.GetContentDimension(i);
                
                if (!string.IsNullOrEmpty(grid.BackgroundImages[i]))
                {
                    if (!preloadedAll)
                        Application.Instance.ResourceManager.LoadImage(grid.BackgroundImages[i]);

                    var bgImageTexture = Application.Instance.ResourceManager.GetTexture(grid.BackgroundImages[i]);
                    Raylib.DrawTexturePro(bgImageTexture,
                        new Rectangle(0, 0, bgImageTexture.width, bgImageTexture.height),
                        new Rectangle(cellDim.X + grid.Offsets[i].X, cellDim.Y + grid.Offsets[i].Y, cellDim.Z, cellDim.W),
                        new Vector2(0, 0), 0, Color.WHITE);
                }
                else
                {
                    int cellItemIndex = i * 4;
                    var hasRadiusCorners = grid.RadiusCorners[i] > 0.0f;
                    if (hasRadiusCorners)
                    {
                        // cannot use gradient colours with background rectangles using radius corners.
                        // defaults to using first solid colour input
                        var backgroundColor = grid.BackgroundColors[i].Colors[0];
                        Raylib.DrawRectangleRounded(new Rectangle(cellDim.X, cellDim.Y, cellDim.Z, cellDim.W),
                            grid.RadiusCorners[i], (int)(grid.RadiusCorners[i] * Math.PI), backgroundColor);
                    }
                    else
                    {
                        if (grid.BackgroundColors[i].Type == Extras.ColorType.Solid)
                        {
                            Raylib.DrawRectangle((int)cellDim.X, (int)cellDim.Y, (int)cellDim.Z, (int)cellDim.W, grid.BackgroundColors[i].Colors[0]);
                        }
                        else if (grid.BackgroundColors[i].Type == Extras.ColorType.GradientHorizontal)
                        {
                            Raylib.DrawRectangleGradientH((int)cellDim.X, (int)cellDim.Y, (int)cellDim.Z, (int)cellDim.W, grid.BackgroundColors[i].Colors[0], grid.BackgroundColors[i].Colors[1]);
                        }
                        else if (grid.BackgroundColors[i].Type == Extras.ColorType.GradientVertical)
                        {
                            Raylib.DrawRectangleGradientV((int)cellDim.X, (int)cellDim.Y, (int)cellDim.Z, (int)cellDim.W, grid.BackgroundColors[i].Colors[0], grid.BackgroundColors[i].Colors[1]);
                        }

                        // draw borders
                        if (grid.Borders[cellItemIndex]) // top
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.X, cellDim.Y), new Vector2(cellDim.X + cellDim.Z, cellDim.Y), grid.BorderThicknesses[cellItemIndex], grid.BorderColors[cellItemIndex]);
                        }


                    }
                }
            }
        }

    }
}
