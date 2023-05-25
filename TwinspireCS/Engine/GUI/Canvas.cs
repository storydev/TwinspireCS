﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.GUI
{
    public class Canvas
    {

        private List<Grid> layouts;
        private List<Element> elements;
        private IDictionary<string, int> elementIdCache;
        private IDictionary<int, TextDim> elementTexts;

        private int activeElement = -1;

        private int currentGridIndex;
        private int currentCellIndex;
        private LayoutFlags currentLayoutFlags;
        private float fixedRowHeight;
        private float fixedColumnWidth;


        private bool preloadedAll;
        private bool requestRebuild;
        private bool firstBuild;

        private string currentFontName;
        private int currentFontSize;
        private int currentFontSpacing;

        private int childInnerPadding;

        private bool firstClick;
        private const float firstClickDelay = 0.25f;
        private float firstClickTime;

        public IEnumerable<Grid> Layouts => layouts;

        public Canvas()
        {
            layouts = new List<Grid>();
            elements = new List<Element>();
            elementIdCache = new Dictionary<string, int>();
            elementTexts = new Dictionary<int, TextDim>();
            activeElement = 0;
            
            currentGridIndex = 0;
            currentCellIndex = 0;

            childInnerPadding = 4;

            firstBuild = true;
            requestRebuild = true;
        }

        public int CreateLayout(Rectangle dimension, int columns, int rows)
        {
            var grid = new Grid();
            grid.Dimension = dimension;
            int totalCells = columns * rows;
            grid.Columns = new float[columns];
            grid.Rows = new float[rows];

            grid.BackgroundColors = new Extras.ColorMethod[totalCells];
            grid.BackgroundImages = new string[totalCells];
            grid.Offsets = new Vector2[totalCells];
            grid.RadiusCorners = new float[totalCells];
            grid.Shadows = new Shadow[totalCells];

            int cornerTotals = totalCells * 4;
            grid.Margin = new float[cornerTotals];
            grid.Padding = new float[cornerTotals];
            grid.BorderColors = new Color[cornerTotals];
            grid.Borders = new bool[cornerTotals];
            grid.BorderThicknesses = new int[cornerTotals];

            layouts.Add(grid);
            return layouts.Count - 1;
        }

        public void Begin()
        {
            if (requestRebuild)
            {
                elements.Clear();
                elementTexts.Clear();
                elementIdCache.Clear();
            }
        }

        public void End()
        {
            if (firstBuild)
            {
                requestRebuild = false;
                firstBuild = false;
            }
        }

        public void DrawTo(int gridIndex, int cellIndex, LayoutFlags flags = LayoutFlags.DynamicRows | LayoutFlags.DynamicColumns)
        {
            currentGridIndex = gridIndex;
            currentCellIndex = cellIndex;
            currentLayoutFlags = flags;
        }

        public void SetFixedRowHeight(float height)
        {
            fixedRowHeight = height;
        }

        public void SetFixedColumnWidth(float width)
        {
            fixedColumnWidth = width;
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

        #region Styling

        public void SetFont(string fontName, int fontSize, int spacing)
        {
            currentFontName = fontName;
            currentFontSize = fontSize;
            currentFontSpacing = spacing;
        }

        #endregion

        #region UI Drawing

        public ElementState Button(string id, string text, TextAlignment alignment = TextAlignment.Center)
        {
            if (elementIdCache.ContainsKey(id) && !requestRebuild)
            {
                var index = elementIdCache[id];
                var element = elements[index];



                TextDim textDim;
                if (elementTexts.ContainsKey(index))
                {
                    textDim = elementTexts[index];
                    DrawText(index, textDim, Color.BLACK, alignment);
                }

                return element.State;
            }
            else if (requestRebuild)
            {
                var element = new Element();
                element.Type = ElementType.Button;
                element.GridIndex = currentGridIndex;
                element.CellIndex = currentCellIndex;
                var elementDim = CalculateNextDimension(elements.Count, text);
                element.Dimension = new Rectangle(elementDim.X, elementDim.Y, elementDim.Z, elementDim.W);

                elements.Add(element);
                elementIdCache.Add(id, elements.Count - 1);
            }

            return ElementState.Idle;
        }


        #region Drawing Utilities

        private void DrawText(int index, TextDim textDim, Color color, TextAlignment alignment)
        {
            var textX = 0.0f;
            var textY = 0.0f;
            if (alignment == TextAlignment.Left)
            {
                textX = elements[index].Dimension.x + childInnerPadding;
                textY = ((elements[index].Dimension.height - textDim.ContentSize.Y) / 2) + elements[index].Dimension.y;
            }
            else if (alignment == TextAlignment.Center)
            {
                textX = ((elements[index].Dimension.width - textDim.ContentSize.X) / 2) + elements[index].Dimension.x;
                textY = ((elements[index].Dimension.height - textDim.ContentSize.Y) / 2) + elements[index].Dimension.y;
            }
            else if (alignment == TextAlignment.Right)
            {
                textX = (elements[index].Dimension.x + elements[index].Dimension.width) - textDim.ContentSize.X - childInnerPadding;
                textY = ((elements[index].Dimension.height - textDim.ContentSize.Y) / 2) + elements[index].Dimension.y;
            }
            else if (alignment == TextAlignment.TopLeft)
            {
                textX = elements[index].Dimension.x + childInnerPadding;
                textY = elements[index].Dimension.y + childInnerPadding;
            }
            else if (alignment == TextAlignment.Top)
            {
                textX = ((elements[index].Dimension.width - textDim.ContentSize.X) / 2) + elements[index].Dimension.x;
                textY = elements[index].Dimension.y + childInnerPadding;
            }
            else if (alignment == TextAlignment.TopRight)
            {
                textX = (elements[index].Dimension.x + elements[index].Dimension.width) - textDim.ContentSize.X - childInnerPadding;
                textY = elements[index].Dimension.y + childInnerPadding;
            }
            else if (alignment == TextAlignment.BottomLeft)
            {
                textX = elements[index].Dimension.x + childInnerPadding;
                textY = elements[index].Dimension.y + elements[index].Dimension.height - textDim.ContentSize.Y - childInnerPadding;
            }
            else if (alignment == TextAlignment.Bottom)
            {
                textX = ((elements[index].Dimension.width - textDim.ContentSize.X) / 2) + elements[index].Dimension.x;
                textY = elements[index].Dimension.y + elements[index].Dimension.height - textDim.ContentSize.Y - childInnerPadding;
            }
            else if (alignment == TextAlignment.BottomRight)
            {
                textX = (elements[index].Dimension.x + elements[index].Dimension.width) - textDim.ContentSize.X - childInnerPadding;
                textY = elements[index].Dimension.y + elements[index].Dimension.height - textDim.ContentSize.Y - childInnerPadding;
            }

            Raylib.BeginScissorMode((int)textX, (int)textY, (int)textDim.ContentSize.X, (int)textDim.ContentSize.Y);
            Utils.RenderMultilineText(Application.Instance.ResourceManager.GetFont(currentFontName), currentFontSize, new Vector2(textX, textY), currentFontSpacing, textDim, Color.BLACK);
            Raylib.EndScissorMode();
        }

        #endregion

        #endregion

        private Vector4 CalculateNextDimension(int elementIndex, string textOrImageName = "")
        {
            var currentRowElements = elements.Where((e) => e.GridIndex == currentGridIndex && e.CellIndex == currentCellIndex);

            Image imageToUse = new Image();
            bool usingImage = false;

            if (textOrImageName.StartsWith("image:"))
            {
                var imageName = textOrImageName.Substring("image:".Length);
                imageToUse = Application.Instance.ResourceManager.GetImage(imageName);
                usingImage = true;
            }

            var gridContentDim = layouts[currentGridIndex].GetContentDimension(currentCellIndex);
            var remainingWidth = gridContentDim.Z;
            var xToBecome = 0.0f;
            var yToBecome = 0.0f;
            var lastRowHeight = 0.0f;
            if (elementIndex > 0)
            {
                for (int i = 0; i < currentRowElements.Count(); i++)
                {
                    var element = currentRowElements.ElementAt(i);
                    xToBecome += element.Dimension.x;
                    remainingWidth -= element.Dimension.width;

                    if (element.Dimension.y > lastRowHeight && currentLayoutFlags.HasFlag(LayoutFlags.DynamicRows))
                    {
                        lastRowHeight = element.Dimension.y;
                    }

                    if (xToBecome > gridContentDim.Z)
                    {
                        if (currentLayoutFlags.HasFlag(LayoutFlags.DynamicRows))
                            yToBecome += lastRowHeight;
                        else if (currentLayoutFlags.HasFlag(LayoutFlags.StaticRows))
                            yToBecome += fixedRowHeight;

                        xToBecome = 0.0f;
                        lastRowHeight = 0;
                        remainingWidth = gridContentDim.Z;
                    }
                }
            }

            xToBecome += gridContentDim.X;
            yToBecome += gridContentDim.Y;

            var widthToBecome = 0.0f;
            var heightToBecome = 0.0f;
            
            var lastPosition = new Vector2(0, 0);
            var lastSize = new Vector2(0, 0);

            if (currentLayoutFlags.HasFlag(LayoutFlags.StaticRows) && currentLayoutFlags.HasFlag(LayoutFlags.FixedComponentHeights))
            {
                heightToBecome = fixedRowHeight;
            }
            else if (currentLayoutFlags.HasFlag(LayoutFlags.SpanColumn))
            {
                heightToBecome = gridContentDim.W;
            }
            else
            {
                heightToBecome += childInnerPadding * 2;
            }

            if (currentLayoutFlags.HasFlag(LayoutFlags.StaticColumns) && currentLayoutFlags.HasFlag(LayoutFlags.FixedComponentWidths))
            {
                widthToBecome = fixedColumnWidth;
            }
            else if (currentLayoutFlags.HasFlag(LayoutFlags.SpanRow))
            {
                widthToBecome = gridContentDim.Z;
            }
            else
            {
                widthToBecome += childInnerPadding * 2;
            }

            if (currentRowElements.Count() > 0)
            {
                var lastElement = currentRowElements.Last();
                lastPosition = new Vector2(lastElement.Dimension.x, lastElement.Dimension.y);
                lastSize = new Vector2(lastElement.Dimension.width, lastElement.Dimension.height);
            }

            TextDim textDim = null;

            if (!string.IsNullOrEmpty(textOrImageName))
            {
                if (currentLayoutFlags.HasFlag(LayoutFlags.DynamicColumns) && !currentLayoutFlags.HasFlag(LayoutFlags.SpanRow))
                {
                    widthToBecome += remainingWidth - (childInnerPadding * 4);
                }

                textDim = Utils.MeasureTextWrapping(Application.Instance.ResourceManager.GetFont(currentFontName), currentFontSize, currentFontSpacing, (int)widthToBecome, textOrImageName);
                if (!currentLayoutFlags.HasFlag(LayoutFlags.SpanColumn))
                    heightToBecome = textDim.ContentSize.Y + (childInnerPadding * 2);

                elementTexts.Add(elementIndex, textDim);
            }

            if (textDim != null && !currentLayoutFlags.HasFlag(LayoutFlags.SpanRow))
            {
                widthToBecome += childInnerPadding * 2;
            }

            if (currentLayoutFlags.HasFlag(LayoutFlags.FillRowsAlways) && !currentLayoutFlags.HasFlag(LayoutFlags.FixedComponentWidths) && !currentLayoutFlags.HasFlag(LayoutFlags.SpanRow))
            {
                if (widthToBecome >= remainingWidth)
                {
                    widthToBecome = remainingWidth;
                }
            }
            else if (currentLayoutFlags.HasFlag(LayoutFlags.DynamicColumns) && !currentLayoutFlags.HasFlag(LayoutFlags.FillRowsAlways))
            {
                if (currentLayoutFlags.HasFlag(LayoutFlags.DynamicRows) && widthToBecome >= remainingWidth)
                {
                    yToBecome += lastRowHeight;
                    xToBecome = gridContentDim.X;
                }
                else if (currentLayoutFlags.HasFlag(LayoutFlags.StaticRows) && widthToBecome >= remainingWidth)
                {
                    yToBecome += fixedRowHeight;
                    xToBecome = gridContentDim.X;
                }
                else if (currentLayoutFlags.HasFlag(LayoutFlags.SpanRow))
                {
                    yToBecome += gridContentDim.W;
                    xToBecome = gridContentDim.X;
                }
            }

            if (usingImage && !currentLayoutFlags.HasFlag(LayoutFlags.SpanRow) && !currentLayoutFlags.HasFlag(LayoutFlags.SpanColumn)
                && !currentLayoutFlags.HasFlag(LayoutFlags.FixedComponentHeights) && !currentLayoutFlags.HasFlag(LayoutFlags.FixedComponentWidths))
            {
                widthToBecome += imageToUse.width;
                heightToBecome += imageToUse.height;
            }

            return new Vector4(xToBecome, yToBecome, widthToBecome, heightToBecome);
        }

        public void SimulateEvents()
        {
            var doubleClick = false;
            var tempClick = false; // to prevent firstClick always being true.
            if (firstClick)
            {
                firstClickTime += Raylib.GetFrameTime();
                if (firstClickTime < firstClickDelay)
                {
                    doubleClick = Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
                    if (doubleClick)
                    {
                        firstClick = false;
                        firstClickTime = 0.0f;
                        tempClick = true;
                    }
                }
                else if (firstClickTime >= firstClickDelay)
                {
                    firstClick = false;
                    firstClickTime = 0.0f;
                    tempClick = true;
                }
            }

            foreach (var element in elements)
            {
                if ((element.State == ElementState.Active || element.State == ElementState.Clicked || element.State == ElementState.DoubleClicked) && Raylib.IsMouseButtonUp(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    element.State = ElementState.Focused;
                }
            }

            var possibleActiveElements = new List<int>();
            var possibleActiveGrids = new List<int>();
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), element.Dimension))
                {
                    possibleActiveElements.Add(i);
                }
                else
                {
                    if (element.State != ElementState.Idle || element.State != ElementState.Inactive)
                    {
                        element.State = ElementState.Idle;
                    }
                }
            }

            for (int i = 0; i < layouts.Count; i++)
            {
                var grid = layouts[i];
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), grid.Dimension))
                {
                    possibleActiveGrids.Add(i);
                }
            }

            if (possibleActiveElements.Count > 0 && elements.Count > 0)
            {
                int last = possibleActiveElements.Count - 1;
                int index = last;
                while (index > -1)
                {
                    var active = elements[possibleActiveElements[index]];
                    var firstClickTimePassed = !firstClick && firstClickTime == 0.0f && tempClick;
                    var mouseReleased = Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);

                    if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && index == last)
                    {
                        active.State = ElementState.Active;
                    }
                    else if ((mouseReleased && index == last && firstClickTimePassed) || (firstClickTimePassed && index == last))
                    {
                        if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_RIGHT) && !doubleClick)
                            active.State = ElementState.ContextMenu;
                        else if (doubleClick)
                            active.State = ElementState.DoubleClicked;
                        else
                            active.State = ElementState.Clicked;
                    }

                    index -= 1;
                }
                
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT) && !tempClick)
            {
                firstClick = true;
            }
        }

        public void Render()
        {
            for (int i = 0; i < layouts.Count; i++)
            {
                RenderLayout(i);
            }
        }

        public unsafe void RenderLayout(int gridIndex)
        {
            var grid = layouts[gridIndex];
            int cells = grid.Columns.Length * grid.Rows.Length;

            for (int i = 0; i < cells; i++)
            {
                var cellDim = grid.GetCellDimension(i);

                if (!Equals(grid.Shadows[i], Shadow.Empty))
                {
                    var name = "Grid_" + gridIndex + "_Cell_" + i + "_Shadow";
                    if (!Application.Instance.ResourceManager.DoesIdentifierExist(name))
                    {
                        var shadowImage = Raylib.GenImageColor((int)cellDim.Z + (grid.Shadows[i].BlurRadius * 2), (int)cellDim.W + (grid.Shadows[i].BlurRadius * 2), Color.WHITE);
                        Raylib.ImageDrawRectangle(ref shadowImage, grid.Shadows[i].BlurRadius, grid.Shadows[i].BlurRadius,
                            (int)(cellDim.Z - grid.Shadows[i].BlurRadius), (int)(cellDim.W - grid.Shadows[i].BlurRadius), grid.Shadows[i].Color);
                        Raylib.ImageBlurGaussian(&shadowImage, grid.Shadows[i].BlurRadius);
                        Application.Instance.ResourceManager.AddResourceImage(name, shadowImage);
                    }
                    else
                    {
                        var shadowTexture = Application.Instance.ResourceManager.GetTexture(name);
                        Raylib.DrawTexture(shadowTexture, (int)grid.Shadows[i].OffsetX + (int)cellDim.X, (int)grid.Shadows[i].OffsetY + (int)cellDim.Y, Color.WHITE);
                    }
                }
                
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

                        if (grid.Borders[cellItemIndex + 1]) // left
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.X, cellDim.Y), new Vector2(cellDim.X, cellDim.Y + cellDim.W), grid.BorderThicknesses[cellItemIndex + 1], grid.BorderColors[cellItemIndex + 1]);
                        }

                        if (grid.Borders[cellItemIndex + 2]) // right
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.X + cellDim.Z, cellDim.Y), new Vector2(cellDim.X + cellDim.Z, cellDim.Y + cellDim.W), grid.BorderThicknesses[cellItemIndex + 2], grid.BorderColors[cellItemIndex + 2]);
                        }

                        if (grid.Borders[cellItemIndex + 3]) // bottom
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.X, cellDim.Y + cellDim.W), new Vector2(cellDim.X + cellDim.Z, cellDim.Y + cellDim.W), grid.BorderThicknesses[cellItemIndex + 3], grid.BorderColors[cellItemIndex + 3]);
                        }
                    }
                }
            }
        }

    }
}
