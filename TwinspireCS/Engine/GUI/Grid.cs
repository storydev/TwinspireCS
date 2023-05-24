using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TwinspireCS.Engine.Extras;

namespace TwinspireCS.Engine.GUI
{
    public class Grid
    {

        private int selectedCell;
        private int selectedRow;
        private int selectedColumn;
        private int[] selectedCells;
        private bool selectedEverything;

        public Rectangle Dimension;
        public float[] Columns;
        public float[] Rows;
        public bool AutoSizeRows;
        public bool AutoSizeColumns;

        public ColorMethod[] BackgroundColors;
        public string[] BackgroundImages;

        public Vector2[] Offsets;
        public Vector2[] Scrolling;
        public float[] RadiusCorners;

        // multiply by 4 for these ones
        public Color[] BorderColors;
        public int[] BorderThicknesses;
        public bool[] Borders;
        public float[] Margin;
        public float[] Padding;

        public Grid()
        {
            selectedCell = 0;
            selectedColumn = 0;
            selectedRow = 0;

            Dimension = new Rectangle();
            Columns = Array.Empty<float>();
            Rows = Array.Empty<float>();

            BackgroundColors = Array.Empty<ColorMethod>();
            BackgroundImages = Array.Empty<string>();

            Offsets = Array.Empty<Vector2>();
            Scrolling = Array.Empty<Vector2>();

            BorderColors = Array.Empty<Color>();
            BorderThicknesses = Array.Empty<int>();
            Borders = Array.Empty<bool>();
            RadiusCorners = Array.Empty<float>();
            Margin = Array.Empty<float>();
            Padding = Array.Empty<float>();

            selectedCells = Array.Empty<int>();
        }

        public Grid Next()
        {
            selectedCell += 1;
            selectedColumn += 1;
            if (selectedColumn >= Columns.Length)
            {
                selectedColumn = 0;
                selectedRow += 1;
            }    
            return this;
        }

        public Grid GotoCell(int column, int row)
        {
            selectedCell = row * Columns.Length + column;
            selectedColumn = column;
            selectedRow = row;
            return this;
        }

        public Grid SelectRange(params Vector2[] cells)
        {


            return this;
        }

        public Grid SelectAll()
        {
            selectedEverything = true;
            return this;
        }

        public Grid Deselect()
        {
            selectedEverything = false;
            selectedCells = Array.Empty<int>();
            return this;
        }

        public int GetCellIndex(int column, int row)
        {
            return row * Columns.Length + column;
        }

        public int GetRow(int index)
        {
            return (int)Math.Floor((float)(index / Columns.Length));
        }

        public int GetColumn(int index)
        {
            return (int)Math.Floor((float)(index % Columns.Length));
        }

        public Vector4 GetCellDimension(int cell)
        {
            int cellItemIndex = cell * 4;
            int column = GetColumn(cell);
            int row = GetRow(cell);
            
            float x = Dimension.x;
            if (column > 0)
            {
                float xOffset = 0.0f;
                for (int c = 0; c < column; c++)
                {
                    xOffset += Columns[c];
                }
                x += xOffset;
            }
            x += Margin[cellItemIndex + 1];

            float y = Dimension.y;
            if (row > 0)
            {
                float yOffset = 0.0f;
                for (int r = 0; r < row; r++)
                {
                    yOffset += Rows[r];
                }
                y += yOffset;
            }
            y += Margin[cellItemIndex];

            float cellWidth = Columns[column];
            float cellHeight = Rows[row];

            if (cellWidth > 0)
            {
                cellWidth -= Margin[cellItemIndex + 1];
                cellWidth -= Margin[cellItemIndex + 2];
            }

            if (cellHeight > 0)
            {
                cellHeight -= Margin[cellItemIndex];
                cellHeight -= Margin[cellItemIndex + 3];
            }

            return new Vector4(x, y, cellWidth, cellHeight);
        }

        public Vector4 GetContentDimension(int cell)
        {
            int cellItemIndex = cell * 4;
            int column = GetColumn(cell);
            int row = GetRow(cell);

            var cellDim = GetCellDimension(cell);
            var x = cellDim.X;
            var y = cellDim.Y;
            var width = cellDim.Z;
            var height = cellDim.W;

            if (Borders[cellItemIndex])
            {
                y += BorderThicknesses[cellItemIndex];
                height -= BorderThicknesses[cellItemIndex];
            }
            
            if (Borders[cellItemIndex + 1])
            {
                x += BorderThicknesses[cellItemIndex + 1];
                width -= BorderThicknesses[cellItemIndex + 1];
            }

            if (Borders[cellItemIndex + 2])
            {
                width -= BorderThicknesses[cellItemIndex + 2];
            }

            if (Borders[cellItemIndex + 3])
            {
                height -= BorderThicknesses[cellItemIndex + 3];
            }

            return new Vector4(x + Padding[cellItemIndex + 1], y + Padding[cellItemIndex], 
                width - Padding[cellItemIndex + 2] - Padding[cellItemIndex + 1], height - Padding[cellItemIndex + 3] - Padding[cellItemIndex]);
        }

        public int GetCurrentRow()
        {
            return selectedRow;
        }

        public int GetCurrentColumn()
        {
            return selectedColumn;
        }

        public int GetSelectedCell()
        {
            return selectedCell;
        }

        public Grid SetColumnWidth(int column, float percentageWidth)
        {
            Columns[column] = Dimension.width * percentageWidth;
            return this;
        }
        public Grid SetColumnWidth(int column, int width)
        {
            Columns[column] = width;
            return this;
        }

        public Grid SetRowHeight(int row, float percentageHeight)
        {
            Rows[row] = Dimension.height * percentageHeight;
            return this;
        }

        public Grid SetRowHeight(int row, int height)
        {
            Rows[row] = height;
            return this;
        }

        public Grid SetRowHeightsAuto()
        {
            float rowHeight = (float)Math.Floor((double)(Dimension.height / Rows.Length));
            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = rowHeight;
            }

            return this;
        }

        public Grid SetColumnWidthsAuto()
        {
            float columnWidth = (float)Math.Floor((double)(Dimension.width / Columns.Length));
            for (int i = 0; i < Columns.Length; i++)
            {
                Columns[i] = columnWidth;
            }

            return this;
        }

        public Grid SetAutoSize(bool autoSizeRows, bool autoSizeColumns)
        {
            AutoSizeRows = autoSizeRows;
            AutoSizeColumns = autoSizeColumns;
            return this;
        }

        public Grid SetBackgroundRow(int row, ColorMethod color)
        {
            var columns = Columns.Length;
            int startCell = row * columns;
            int endCell = startCell + columns;
            for (int i = startCell; i < endCell; i++)
            {
                BackgroundColors[i] = color;
            }
            return this;
        }

        public Grid SetBackgroundRow(int row, string imageName)
        {
            var columns = Columns.Length;
            int startCell = row * columns;
            int endCell = startCell + columns;
            for (int i = startCell; i < endCell; i++)
            {
                BackgroundImages[i] = imageName;
            }
            return this;
        }

        public Grid SetBackgroundColumn(int column, ColorMethod color)
        {
            for (int i = 0; i < Columns.Length; i++)
            {
                int cell = i * Columns.Length + column;
                BackgroundColors[cell] = color;
            }

            return this;
        }

        public Grid SetBackgroundColumn(int column, string imageName)
        {
            for (int i = 0; i < Columns.Length; i++)
            {
                int cell = i * Columns.Length + column;
                BackgroundImages[cell] = imageName;
            }

            return this;
        }

        public Grid SetBackgroundCell(int column, int row, ColorMethod color)
        {
            int cell = row * Columns.Length + column;
            BackgroundColors[cell] = color;
            return this;
        }

        public Grid SetBackgroundCell(int column, int row, string imageName)
        {
            int cell = row * Columns.Length + column;
            BackgroundImages[cell] = imageName;
            return this;
        }

        public Grid SetBackground(ColorMethod color)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    BackgroundColors[i] = color;
                }
                return this;
            }

            BackgroundColors[selectedCell] = color;
            return this;
        }

        public Grid SetBackground(string imageName)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    BackgroundImages[i] = imageName;
                }
                return this;
            }

            BackgroundImages[selectedCell] = imageName;
            return this;
        }

        public Grid SetOffsetCell(int column, int row, Vector2 offset)
        {
            int cell = row * Columns.Length + column;
            Offsets[cell] = offset;
            return this;
        }

        public Grid SetOffset(Vector2 offset)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    Offsets[i] = offset;
                }
                return this;
            }

            Offsets[selectedCell] = offset;
            return this;
        }

        public Grid SetBorderColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell] = color; // top
            BorderColors[cell + 1] = color; // left
            BorderColors[cell + 2] = color; // right
            BorderColors[cell + 3] = color; // bottom
            return this;
        }

        public Grid SetBorderColors(Color color)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderColors[actualCellIndex] = color; // top
                    BorderColors[actualCellIndex + 1] = color; // left
                    BorderColors[actualCellIndex + 2] = color; // right
                    BorderColors[actualCellIndex + 3] = color; // bottom
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderColors[actualCell] = color; // top
            BorderColors[actualCell + 1] = color; // left
            BorderColors[actualCell + 2] = color; // right
            BorderColors[actualCell + 3] = color; // bottom
            return this;
        }

        public Grid SetBorderTopColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell] = color; // top
            return this;
        }

        public Grid SetBorderTopColors(Color color)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderColors[actualCellIndex] = color; // top
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderColors[actualCell] = color; // top
            return this;
        }

        public Grid SetBorderLeftColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell + 1] = color; // left
            return this;
        }

        public Grid SetBorderLeftColors(Color color)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderColors[actualCellIndex + 1] = color; // left
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderColors[actualCell + 1] = color; // left
            return this;
        }

        public Grid SetBorderRightColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell + 2] = color; // right
            return this;
        }

        public Grid SetBorderRightColors(Color color)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderColors[actualCellIndex + 2] = color; // right
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderColors[actualCell + 2] = color; // right
            return this;
        }

        public Grid SetBorderBottomColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell + 3] = color; // bottom
            return this;
        }

        public Grid SetBorderBottomColors(Color color)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderColors[actualCellIndex + 3] = color; // bottom
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderColors[actualCell + 3] = color; // bottom
            return this;
        }

        public Grid SetBorderThicknessCell(int column, int row, int lineThickness)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderThicknesses[cell] = lineThickness;
            BorderThicknesses[cell + 1] = lineThickness;
            BorderThicknesses[cell + 2] = lineThickness;
            BorderThicknesses[cell + 3] = lineThickness;
            return this;
        }

        public Grid SetBorderThickness(int lineThickness)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderThicknesses[actualCellIndex] = lineThickness;
                    BorderThicknesses[actualCellIndex + 1] = lineThickness;
                    BorderThicknesses[actualCellIndex + 2] = lineThickness;
                    BorderThicknesses[actualCellIndex + 3] = lineThickness;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderThicknesses[actualCell] = lineThickness;
            BorderThicknesses[actualCell + 1] = lineThickness;
            BorderThicknesses[actualCell + 2] = lineThickness;
            BorderThicknesses[actualCell + 3] = lineThickness;
            return this;
        }

        public Grid SetBorderTopThicknessCell(int column, int row, int lineThickness)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderThicknesses[cell + 1] = lineThickness;
            return this;
        }

        public Grid SetBorderTopThickness(int lineThickness)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderThicknesses[actualCellIndex] = lineThickness;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderThicknesses[actualCell] = lineThickness;
            return this;
        }

        public Grid SetBorderLeftThicknessCell(int column, int row, int lineThickness)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderThicknesses[cell] = lineThickness;
            return this;
        }

        public Grid SetBorderLeftThickness(int lineThickness)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderThicknesses[actualCellIndex + 1] = lineThickness;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderThicknesses[actualCell + 1] = lineThickness;
            return this;
        }

        public Grid SetBorderRightThicknessCell(int column, int row, int lineThickness)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderThicknesses[cell + 2] = lineThickness;
            return this;
        }

        public Grid SetBorderRightThickness(int lineThickness)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderThicknesses[actualCellIndex + 2] = lineThickness;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderThicknesses[actualCell + 2] = lineThickness;
            return this;
        }

        public Grid SetBorderBottomThicknessCell(int column, int row, int lineThickness)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderThicknesses[cell + 3] = lineThickness;
            return this;
        }

        public Grid SetBorderBottomThickness(int lineThickness)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    BorderThicknesses[actualCellIndex + 3] = lineThickness;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            BorderThicknesses[actualCell + 3] = lineThickness;
            return this;
        }

        public Grid ApplyBordersCell(int column, int row, bool yes)
        {
            int cell = (row * Columns.Length + column) * 4;
            Borders[cell] = yes;
            Borders[cell + 1] = yes;
            Borders[cell + 2] = yes;
            Borders[cell + 3] = yes;
            return this;
        }

        public Grid ApplyBorders(bool yes)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Borders[actualCellIndex] = yes;
                    Borders[actualCellIndex + 1] = yes;
                    Borders[actualCellIndex + 2] = yes;
                    Borders[actualCellIndex + 3] = yes;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Borders[actualCell] = yes;
            Borders[actualCell + 1] = yes;
            Borders[actualCell + 2] = yes;
            Borders[actualCell + 3] = yes;
            return this;
        }

        public Grid ApplyBordersTopCell(int column, int row, bool yes)
        {
            int cell = (row * Columns.Length + column) * 4;
            Borders[cell] = yes;
            return this;
        }

        public Grid ApplyBordersTop(bool yes)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Borders[actualCellIndex] = yes;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Borders[actualCell] = yes;
            return this;
        }

        public Grid ApplyBordersLeftCell(int column, int row, bool yes)
        {
            int cell = (row * Columns.Length + column) * 4;
            Borders[cell + 1] = yes;
            return this;
        }

        public Grid ApplyBordersLeft(bool yes)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Borders[actualCellIndex + 1] = yes;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Borders[actualCell + 1] = yes;
            return this;
        }

        public Grid ApplyBordersRightCell(int column, int row, bool yes)
        {
            int cell = (row * Columns.Length + column) * 4;
            Borders[cell + 2] = yes;
            return this;
        }

        public Grid ApplyBordersRight(bool yes)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Borders[actualCellIndex + 2] = yes;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Borders[actualCell + 2] = yes;
            return this;
        }

        public Grid ApplyBordersBottomCell(int column, int row, bool yes)
        {
            int cell = (row * Columns.Length + column) * 4;
            Borders[cell + 3] = yes;
            return this;
        }

        public Grid ApplyBordersBottom(bool yes)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Borders[actualCellIndex + 3] = yes;
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Borders[actualCell + 3] = yes;
            return this;
        }

        public Grid SetRadiusCornersCell(int column, int row, float radius)
        {
            int cell = row * Columns.Length + column;
            RadiusCorners[cell] = radius;
            return this;
        }

        public Grid SetRadiusCorners(float radius)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    RadiusCorners[i] = radius;
                }
                return this;
            }

            RadiusCorners[selectedCell] = radius;
            return this;
        }

        public Grid SetMarginCell(int column, int row, float margin)
        {
            int cell = (row * Columns.Length + column) * 4;
            Margin[cell] = margin; // top
            Margin[cell + 1] = margin; // left
            Margin[cell + 2] = margin; // right
            Margin[cell + 3] = margin; // bottom
            return this;
        }

        public Grid SetMargin(float margin)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Margin[actualCellIndex] = margin; // top
                    Margin[actualCellIndex + 1] = margin; // left
                    Margin[actualCellIndex + 2] = margin; // right
                    Margin[actualCellIndex + 3] = margin; // bottom
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Margin[actualCell] = margin; // top
            Margin[actualCell + 1] = margin; // left
            Margin[actualCell + 2] = margin; // right
            Margin[actualCell + 3] = margin; // bottom
            return this;
        }

        public Grid SetMarginTopCell(int column, int row, float margin)
        {
            int cell = (row * Columns.Length + column) * 4;
            Margin[cell] = margin; // top
            return this;
        }

        public Grid SetMarginTop(float margin)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Margin[actualCellIndex] = margin; // top
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Margin[actualCell] = margin; // top
            return this;
        }

        public Grid SetMarginLeftCell(int column, int row, float margin)
        {
            int cell = (row * Columns.Length + column) * 4;
            Margin[cell + 1] = margin; // left
            return this;
        }

        public Grid SetMarginLeft(float margin)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Margin[actualCellIndex + 1] = margin; // left
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Margin[actualCell + 1] = margin; // left
            return this;
        }

        public Grid SetMarginRightCell(int column, int row, float margin)
        {
            int cell = (row * Columns.Length + column) * 4;
            Margin[cell + 2] = margin; // right
            return this;
        }

        public Grid SetMarginRight(float margin)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Margin[actualCellIndex + 2] = margin; // right
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Margin[actualCell + 2] = margin; // right
            return this;
        }

        public Grid SetMarginBottomCell(int column, int row, float margin)
        {
            int cell = (row * Columns.Length + column) * 4;
            Margin[cell + 3] = margin; // bottom
            return this;
        }

        public Grid SetMarginBottom(float margin)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Margin[actualCellIndex + 3] = margin; // bottom
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Margin[actualCell + 3] = margin; // bottom
            return this;
        }

        public Grid SetPaddingCell(int column, int row, float padding)
        {
            int cell = (row * Columns.Length + column) * 4;
            Padding[cell] = padding; // top
            Padding[cell + 1] = padding; // left
            Padding[cell + 2] = padding; // right
            Padding[cell + 3] = padding; // bottom
            return this;
        }

        public Grid SetPadding(float padding)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Padding[actualCellIndex] = padding; // top
                    Padding[actualCellIndex + 1] = padding; // left
                    Padding[actualCellIndex + 2] = padding; // right
                    Padding[actualCellIndex + 3] = padding; // bottom
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Padding[actualCell] = padding; // top
            Padding[actualCell + 1] = padding; // left
            Padding[actualCell + 2] = padding; // right
            Padding[actualCell + 3] = padding; // bottom
            return this;
        }

        public Grid SetPaddingTopCell(int column, int row, float padding)
        {
            int cell = (row * Columns.Length + column) * 4;
            Padding[cell] = padding; // top
            return this;
        }

        public Grid SetPaddingTop(float padding)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Padding[actualCellIndex] = padding; // top
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Padding[actualCell] = padding; // top
            return this;
        }

        public Grid SetPaddingLeftCell(int column, int row, float padding)
        {
            int cell = (row * Columns.Length + column) * 4;
            Padding[cell + 1] = padding; // left
            return this;
        }

        public Grid SetLeftPadding(float padding)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Padding[actualCellIndex + 1] = padding; // left
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Padding[actualCell + 1] = padding; // left
            return this;
        }

        public Grid SetPaddingRightCell(int column, int row, float padding)
        {
            int cell = (row * Columns.Length + column) * 4;
            Padding[cell + 2] = padding; // right
            return this;
        }

        public Grid SetPaddingRight(float padding)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Padding[actualCellIndex + 2] = padding; // right
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Padding[actualCell + 2] = padding; // right
            return this;
        }

        public Grid SetPaddingBottomCell(int column, int row, float padding)
        {
            int cell = (row * Columns.Length + column) * 4;
            Padding[cell + 3] = padding; // right
            return this;
        }

        public Grid SetPaddingBottom(float padding)
        {
            if (selectedEverything)
            {
                int cells = Columns.Length * Rows.Length;
                for (int i = 0; i < cells; i++)
                {
                    int actualCellIndex = i * 4;
                    Padding[actualCellIndex + 3] = padding; // bottom
                }
                return this;
            }

            int actualCell = selectedCell * 4;
            Padding[actualCell + 3] = padding; // right
            return this;
        }

    }
}
