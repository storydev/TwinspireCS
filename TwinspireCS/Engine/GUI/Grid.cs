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

        public Vector4 Dimension;
        public float[] Columns;
        public float[] Rows;
        public bool AutoSizeRows;
        public bool AutoSizeColumns;

        public ColorMethod[] BackgroundColors;
        public string[] BackgroundImages;

        public Vector2[] Offsets;
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

            Dimension = new Vector4();
            Columns = Array.Empty<float>();
            Rows = Array.Empty<float>();

            BackgroundColors = Array.Empty<ColorMethod>();
            BackgroundImages = Array.Empty<string>();

            Offsets = Array.Empty<Vector2>();

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
            
            float x = Dimension.X;
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

            float y = Dimension.Y;
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
            Columns[column] = Dimension.Z * percentageWidth;
            return this;
        }
        public Grid SetColumnWidth(int column, int width)
        {
            Columns[column] = width;
            return this;
        }

        public Grid SetRowHeight(int row, float percentageHeight)
        {
            Rows[row] = Dimension.W * percentageHeight;
            return this;
        }

        public Grid SetRowHeight(int row, int height)
        {
            Rows[row] = height;
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
            BackgroundColors[selectedCell] = color;
            return this;
        }

        public Grid SetBackground(string imageName)
        {
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
            Offsets[selectedCell] = offset;
            return this;
        }

        public Grid SetBorderColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell] = color; // left
            BorderColors[cell + 1] = color; // top
            BorderColors[cell + 2] = color; // right
            BorderColors[cell + 3] = color; // bottom
            return this;
        }

        public Grid SetBorderColors(Color color)
        {
            int actualCell = selectedCell * 4;
            BorderColors[actualCell] = color; // left
            BorderColors[actualCell + 1] = color; // top
            BorderColors[actualCell + 2] = color; // right
            BorderColors[actualCell + 3] = color; // bottom
            return this;
        }

        public Grid SetBorderTopColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell + 1] = color; // top
            return this;
        }

        public Grid SetBorderTopColors(Color color)
        {
            int actualCell = selectedCell * 4;
            BorderColors[actualCell + 1] = color; // top
            return this;
        }

        public Grid SetBorderLeftColorsCell(int column, int row, Color color)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderColors[cell] = color; // left
            return this;
        }

        public Grid SetBorderLeftColors(Color color)
        {
            int actualCell = selectedCell * 4;
            BorderColors[actualCell] = color; // left
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
            int actualCell = selectedCell * 4;
            BorderThicknesses[actualCell + 1] = lineThickness;
            return this;
        }

        public Grid SetBorderLeftThicknessCell(int column, int row, int lineThickness)
        {
            int cell = (row * Columns.Length + column) * 4;
            BorderThicknesses[cell] = lineThickness;
            return this;
        }

        public Grid SetBorderleftThickness(int lineThickness)
        {
            int actualCell = selectedCell * 4;
            BorderThicknesses[actualCell] = lineThickness;
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
            int actualCell = selectedCell * 4;
            Padding[actualCell + 3] = padding; // right
            return this;
        }

    }
}
