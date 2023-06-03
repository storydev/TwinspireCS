using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Raylib_cs;
using System.Xml.Linq;

namespace TwinspireCS.Engine.GUI
{
    public class Canvas
    {

        private List<Grid> layouts;
        private List<Element> elements;
        private IDictionary<string, int[]> elementIdCache;
        private IDictionary<int, TextDim> elementTexts;

        private List<int> animationIndices;
        private List<Tween> tweens;
        private List<bool> tweensRunning;
        private IDictionary<string, int[]> elementTweens;
        private List<Tween> tweenStack;

        private List<string> backgroundImages;

        private int activeElement = -1;

        private int actualMouseX;
        private int actualMouseY;
        private int backBufferWidth;
        private int backBufferHeight;


        private int currentGridIndex;
        private int currentCellIndex;
        private LayoutFlags currentLayoutFlags;
        private float fixedRowHeight;
        private float fixedColumnWidth;

        // styles
        private Style basicStyle;
        private Style hoveredStyle;
        private Style downStyle;
        private List<Style> styleStack;
        private bool usingCustomStyle;

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

        public string Name { get; set; }

        public Canvas()
        {
            layouts = new List<Grid>();
            elements = new List<Element>();
            elementIdCache = new Dictionary<string, int[]>();
            elementTexts = new Dictionary<int, TextDim>();
            animationIndices = new List<int>();
            styleStack = new List<Style>();
            tweens = new List<Tween>();
            tweenStack = new List<Tween>();
            elementTweens = new Dictionary<string, int[]>();
            tweensRunning = new List<bool>();
            activeElement = 0;

            backgroundImages = new List<string>();
            
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

        public int CreateLayoutFromImage(string imageName, Vector2 position)
        {
            var image = Application.Instance.ResourceManager.GetImage(imageName);

            var grid = new Grid();
            grid.Dimension = new Rectangle(position.X, position.Y, image.width, image.height);
            grid.Columns = new float[1];
            grid.Columns[0] = image.width;
            grid.Rows = new float[1];
            grid.Rows[0] = image.height;

            grid.BackgroundColors = new Extras.ColorMethod[1];
            grid.BackgroundImages = new string[1];
            grid.BackgroundImages[0] = imageName;

            grid.Offsets = new Vector2[1];
            grid.RadiusCorners = new float[1];
            grid.Shadows = new Shadow[1];

            int cornerTotals = 4;
            grid.Margin = new float[cornerTotals];
            grid.Padding = new float[cornerTotals];
            grid.BorderColors = new Color[cornerTotals];
            grid.Borders = new bool[cornerTotals];
            grid.BorderThicknesses = new int[cornerTotals];

            layouts.Add(grid);
            return layouts.Count - 1;
        }

        public int CreateLayoutFromImage(string imageName, ContentAlignment alignment, Rectangle alignTo, bool outside = false)
        {
            var image = Application.Instance.ResourceManager.GetImage(imageName);

            var grid = new Grid();
            grid.Dimension = CalculateDimension(new Vector2(0, 0), new Vector2(image.width, image.height), alignment, alignTo, outside);
            grid.Columns = new float[1];
            grid.Columns[0] = image.width;
            grid.Rows = new float[1];
            grid.Rows[0] = image.height;

            grid.BackgroundColors = new Extras.ColorMethod[1];
            grid.BackgroundImages = new string[1];
            grid.BackgroundImages[0] = imageName;

            grid.Offsets = new Vector2[1];
            grid.RadiusCorners = new float[1];
            grid.Shadows = new Shadow[1];

            int cornerTotals = 4;
            grid.Margin = new float[cornerTotals];
            grid.Padding = new float[cornerTotals];
            grid.BorderColors = new Color[cornerTotals];
            grid.Borders = new bool[cornerTotals];
            grid.BorderThicknesses = new int[cornerTotals];

            layouts.Add(grid);
            return layouts.Count - 1;
        }

        public void AddBackgroundImage(string imageName)
        {
            backgroundImages.Add(imageName);
        }

        public void Begin()
        {
            if (requestRebuild)
            {
                elements.Clear();
                elementTexts.Clear();
                elementIdCache.Clear();
                elementTweens.Clear();
            }
        }

        public void TransformMousePosition(int width, int height)
        {
            backBufferWidth = width;
            backBufferHeight = height;
            actualMouseX = Raylib.GetMouseX() * (width / Raylib.GetRenderWidth());
            actualMouseY = Raylib.GetMouseY() * (height / Raylib.GetRenderHeight());
        }

        public Vector2 GetMousePosition()
        {
            TransformMousePosition(backBufferWidth, backBufferHeight);
            return new Vector2(actualMouseX, actualMouseY);
        }

        public void End()
        {
            if (firstBuild)
            {
                requestRebuild = false;
                firstBuild = false;
            }

            if (requestRebuild)
            {
                foreach (var id in animationIndices)
                {
                    Animate.Reset(id);
                }
            }

            Animate.ResetTicks();
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

        public void SetNextStyle(Style style)
        {
            basicStyle = style;
            hoveredStyle = style;
            downStyle = style;
            usingCustomStyle = true;
        }

        public void SetNextStyle(Style basicState, Style hoverState, Style downState)
        {
            basicStyle = basicState;
            hoveredStyle = hoverState;
            downStyle = downState;
            usingCustomStyle = true;
        }

        public void PushStyle(Style nextStyle)
        {
            styleStack.Add(nextStyle);
        }

        public void PopStyle()
        {
            try
            {
                styleStack.RemoveAt(styleStack.Count - 1);
            }
            catch (Exception)
            {
#if DEBUG
                TwinspireCS.Utils.Warn("Style stack has already been emptied.", 1);
#endif
            }
        }

        private Style? GetLocalStyle(ElementState state)
        {
            if (usingCustomStyle)
            {
                usingCustomStyle = false;
                if (state == ElementState.Hovered || state == ElementState.Clicked || state == ElementState.DoubleClicked)
                {
                    return hoveredStyle;
                }
                else if (state == ElementState.Active)
                {
                    return downStyle;
                }
                else
                {
                    return basicStyle;
                }
            }

            if (styleStack.Count > 0)
            {
                var style = styleStack[styleStack.Count-1];
                return style;
            }

            return null;
        }

        public void PushTween(Tween tween)
        {
            tweenStack.Add(tween);
        }

        public void PopTween()
        {
            try
            {
                tweenStack.RemoveAt(tweenStack.Count - 1);
            }
            catch (Exception)
            {
#if DEBUG
                TwinspireCS.Utils.Warn("Tween stack has already been emptied.", 1);
#endif
            }
        }

        public int AddTween(string id, Tween tween)
        {
            tweens.Add(tween);
            tweensRunning.Add(false);
            var index = tweens.Count - 1;
            var animateIndex = Animate.Create();
            elementTweens.Add(id, new int[] { index, animateIndex });
            return index;
        }

        public void StartTween(string id)
        {
            if (elementTweens.ContainsKey(id))
            {
                var tweenIndices = elementTweens[id];
                tweensRunning[tweenIndices[0]] = true;
            }
        }

        public void ResetTween(string id)
        {
            if (elementTweens.ContainsKey(id))
            {
                var tweenIndices = elementTweens[id];
                Animate.Reset(tweenIndices[1]);
            }
        }

        public void ReverseTween(string id, bool reverse = true)
        {
            if (elementTweens.ContainsKey(id))
            {
                var tweenIndices = elementTweens[id];
                Animate.ReverseIndex(tweenIndices[1], reverse);
            }
        }

        public float RunTween(string id, float duration, float delay = 0.0f)
        {
            float result;
            if (elementTweens.ContainsKey(id))
            {
                var tweenIndices = elementTweens[id];
                var isRunning = tweensRunning[tweenIndices[0]];
                if (isRunning)
                {
                    var animationIndex = tweenIndices[1];
                    if (Animate.Tick(animationIndex, duration, delay))
                    {
                        result = Animate.GetReverse(animationIndex) ? 0.0f : 1.0f;
                    }
                    else
                    {
                        result = Animate.GetRatio(animationIndex);
                    }
                    return result;
                }
                else
                {
                    return float.NaN;
                }
            }

            return float.NaN;
        }

        private Style? GetTweenState(string id, float ratio)
        {
            if (elementTweens.ContainsKey(id))
            {
                var tweenIndices = elementTweens[id];
                var actualTween = tweens[tweenIndices[0]];
                var style = new Style();

                // border check
                {
                    // change from false to true,
                    // when this happens, default to animate in via opacity
                    if (actualTween.From.Borders[0] != actualTween.To.Borders[0] && actualTween.To.Borders[0])
                    {
                        style.Opacity = ratio;
                        style.BorderColors[0] = Utils.ChangeColour(actualTween.From.BorderColors[0], actualTween.To.BorderColors[0], ratio);
                        style.BorderThicknesses[0] = actualTween.To.BorderThicknesses[0];
                    }
                    else if (actualTween.From.Borders[0]) // if already true, expand via thickness
                    {
                        style.BorderThicknesses[0] = (int)(((actualTween.To.BorderThicknesses[0] - actualTween.From.BorderThicknesses[0]) * ratio) + actualTween.From.BorderThicknesses[0]);
                        style.BorderColors[0] = Utils.ChangeColour(actualTween.From.BorderColors[0], actualTween.To.BorderColors[0], ratio);
                    }

                    style.Borders[0] = actualTween.To.Borders[0];

                    // change from false to true,
                    // when this happens, default to animate in via opacity
                    if (actualTween.From.Borders[1] != actualTween.To.Borders[1] && actualTween.To.Borders[1])
                    {
                        style.Opacity = ratio;
                        style.BorderColors[1] = Utils.ChangeColour(actualTween.From.BorderColors[1], actualTween.To.BorderColors[1], ratio);
                        style.BorderThicknesses[1] = actualTween.To.BorderThicknesses[1];
                    }
                    else if (actualTween.From.Borders[1]) // if already true, expand via thickness
                    {
                        style.BorderThicknesses[1] = (int)(((actualTween.To.BorderThicknesses[1] - actualTween.From.BorderThicknesses[1]) * ratio) + actualTween.From.BorderThicknesses[1]);
                        style.BorderColors[1] = Utils.ChangeColour(actualTween.From.BorderColors[1], actualTween.To.BorderColors[1], ratio);
                    }

                    style.Borders[1] = actualTween.To.Borders[1];

                    // change from false to true,
                    // when this happens, default to animate in via opacity
                    if (actualTween.From.Borders[2] != actualTween.To.Borders[2] && actualTween.To.Borders[2])
                    {
                        style.Opacity = ratio;
                        style.BorderColors[2] = Utils.ChangeColour(actualTween.From.BorderColors[2], actualTween.To.BorderColors[2], ratio);
                        style.BorderThicknesses[2] = actualTween.To.BorderThicknesses[2];
                    }
                    else if (actualTween.From.Borders[2]) // if already true, expand via thickness
                    {
                        style.BorderThicknesses[2] = (int)(((actualTween.To.BorderThicknesses[2] - actualTween.From.BorderThicknesses[2]) * ratio) + actualTween.From.BorderThicknesses[2]);
                        style.BorderColors[2] = Utils.ChangeColour(actualTween.From.BorderColors[2], actualTween.To.BorderColors[2], ratio);
                    }

                    style.Borders[2] = actualTween.To.Borders[2];

                    // change from false to true,
                    // when this happens, default to animate in via opacity
                    if (actualTween.From.Borders[3] != actualTween.To.Borders[3] && actualTween.To.Borders[3])
                    {
                        style.Opacity = ratio;
                        style.BorderColors[3] = Utils.ChangeColour(actualTween.From.BorderColors[3], actualTween.To.BorderColors[3], ratio);
                        style.BorderThicknesses[3] = actualTween.To.BorderThicknesses[3];
                    }
                    else if (actualTween.From.Borders[3]) // if already true, expand via thickness
                    {
                        style.BorderThicknesses[3] = (int)(((actualTween.To.BorderThicknesses[3] - actualTween.From.BorderThicknesses[3]) * ratio) + actualTween.From.BorderThicknesses[3]);
                        style.BorderColors[3] = Utils.ChangeColour(actualTween.From.BorderColors[3], actualTween.To.BorderColors[3], ratio);
                    }

                    style.Borders[3] = actualTween.To.Borders[3];
                }

                // background color

                style.BackgroundColor.Type = actualTween.To.BackgroundColor.Type;

                if (actualTween.From.BackgroundColor.Type == actualTween.To.BackgroundColor.Type) // do literal conversion regardless of type here
                {
                    if (actualTween.From.BackgroundColor.Colors.Length != actualTween.To.BackgroundColor.Colors.Length)
                    {
                        // if the length of the two types are different, it was probably setup incorrectly!
                        TwinspireCS.Utils.Warn("The length of the tween background colours for this call does not match. Exiting animation.", 2);
                        return null;
                    }

                    style.BackgroundColor.Colors = new Color[actualTween.From.BackgroundColor.Colors.Length];
                    for (int i = 0; i < actualTween.From.BackgroundColor.Colors.Length; i++)
                    {
                        style.BackgroundColor.Colors[i] = Utils.ChangeColour(actualTween.From.BackgroundColor.Colors[i], actualTween.To.BackgroundColor.Colors[i], ratio);
                    }
                }
                else if (actualTween.From.BackgroundColor.Type != actualTween.To.BackgroundColor.Type)
                {
                    // if types are not the same, check if changing from solid -> gradient or gradient -> solid
                    if (actualTween.From.BackgroundColor.Type == Extras.ColorType.Solid) // changing to gradient
                    {
                        // use solid colour for first colour in gradient
                        style.BackgroundColor.Colors[0] = Utils.ChangeColour(actualTween.From.BackgroundColor.Colors[0], actualTween.To.BackgroundColor.Colors[0], ratio);
                        style.BackgroundColor.Colors[1] = Utils.ChangeColour(actualTween.From.BackgroundColor.Colors[0], actualTween.To.BackgroundColor.Colors[1], ratio);
                    }
                    else if (actualTween.To.BackgroundColor.Type == Extras.ColorType.Solid) // changing from gradient
                    {
                        style.BackgroundColor.Colors[0] = Utils.ChangeColour(actualTween.From.BackgroundColor.Colors[0], actualTween.To.BackgroundColor.Colors[0], ratio);
                        style.BackgroundColor.Colors[0] = Utils.ChangeColour(actualTween.From.BackgroundColor.Colors[1], actualTween.To.BackgroundColor.Colors[0], ratio);
                    }
                }

                // radius corners
                style.RadiusCorners = ((actualTween.To.RadiusCorners - actualTween.From.RadiusCorners) * ratio) + actualTween.From.RadiusCorners;

                return style;
            }

            return null;
        }

        #endregion

        #region UI Drawing

        public ElementState Button(string id, string text, ContentAlignment alignment = ContentAlignment.Center)
        {
            if (elementIdCache.ContainsKey(id) && !requestRebuild)
            {
                var index = elementIdCache[id];
                var element = elements[index[0]];

                var hoverTween = id + ":hover";

                if (elementTweens.ContainsKey(hoverTween))
                {
                    var tween = tweens[elementTweens[hoverTween][0]];
                    if (element.State == ElementState.Hovered || element.State == ElementState.Idle)
                    {
                        ReverseTween(hoverTween, element.State == ElementState.Idle);
                        var tweenRatio = RunTween(hoverTween, tween.Duration, tween.Delay);
                        var tweenStyle = GetTweenState(hoverTween, tweenRatio != float.NaN ? tweenRatio : 0);
                        if (tweenStyle != null)
                            DrawRectStyle(element.Dimension, tweenStyle);
                    }
                    else
                    {
                        var style = GetLocalStyle(element.State);
                        if (style != null)
                        {
                            DrawRectStyle(element.Dimension, style);
                        }
                        else if (element.State == ElementState.Active)
                        {
                            DrawRectStyle(element.Dimension, Theme.Default.Styles[Theme.BUTTON_DOWN]);
                        }
                        else if (element.State == ElementState.Hovered || element.State == ElementState.Clicked || element.State == ElementState.DoubleClicked)
                        {
                            DrawRectStyle(element.Dimension, Theme.Default.Styles[Theme.BUTTON_HOVER]);
                        }
                        else
                        {
                            DrawRectStyle(element.Dimension, Theme.Default.Styles[Theme.BUTTON]);
                        }
                    }
                }
                else
                {
                    var style = GetLocalStyle(element.State);
                    if (style != null)
                    {
                        DrawRectStyle(element.Dimension, style);
                    }
                    else if (element.State == ElementState.Active)
                    {
                        DrawRectStyle(element.Dimension, Theme.Default.Styles[Theme.BUTTON_DOWN]);
                    }
                    else if (element.State == ElementState.Hovered || element.State == ElementState.Clicked || element.State == ElementState.DoubleClicked)
                    {
                        DrawRectStyle(element.Dimension, Theme.Default.Styles[Theme.BUTTON_HOVER]);
                    }
                    else
                    {
                        DrawRectStyle(element.Dimension, Theme.Default.Styles[Theme.BUTTON]);
                    }
                }

                TextDim textDim;
                if (elementTexts.ContainsKey(index[0]))
                {
                    textDim = elementTexts[index[0]];
                    DrawText(index[0], textDim, Color.BLACK, alignment);
                }

                return element.State;
            }
            else if (requestRebuild)
            {
                var elementDim = CalculateNextDimension(elements.Count, text);
                var build = BuildElementsFromComponent("Button", elementDim);
                int elementsLength = build.Length;
                for (int i = 0; i < build.Length; i++)
                {
                    build[i].IsBaseElement = i == 0;
                    build[i].CellIndex = currentCellIndex;
                    build[i].GridIndex = currentGridIndex;

                    elements.Add(build[i]);
                    if (i == 0)
                        elementIdCache.Add(id, new int[] { elements.Count - 1, elementsLength });
                }

                var tween = new Tween();
                tween.Duration = 0.25f;
                tween.From = Theme.Default.Styles[Theme.BUTTON];
                tween.To = Theme.Default.Styles[Theme.BUTTON_HOVER];
                AddTween(id + ":hover", tween);
                StartTween(id + ":hover");
            }

            return ElementState.Idle;
        }


        #region Drawing Utilities

        /// <summary>
        /// Elements are built by using the given dimension as the maximum size, as determined by the 
        /// flow of the container in which the subject elements will be rendered.
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="dimension"></param>
        /// <returns></returns>
        private Element[]? BuildElementsFromComponent(string componentName, Rectangle dimension)
        {
            if (!UI.InterfaceBuilder.Components.ContainsKey(componentName))
            {
                TwinspireCS.Utils.Warn("[INTERNAL-DEBUG]: The component name, '" + componentName + "' does not exist.", 1);
                return null;
            }

            var component = UI.InterfaceBuilder.Components[componentName];
            var elements = new Element[component.Elements.Length];
            for (int i = 0; i < component.Elements.Length; i++)
            {
                elements[i] = new Element();
                var compElement = component.Elements[i];
                elements[i].Type = compElement.Type;
                elements[i].State = ElementState.Idle;
                elements[i].Shape = compElement.Shape;

                Vector2 offset = new Vector2();
                offset.X = compElement.HorizontalMeasureType == MeasureType.Percentage ? compElement.Offset.X * dimension.width : compElement.Offset.X;
                offset.Y = compElement.VerticalMeasureType == MeasureType.Percentage ? compElement.Offset.Y * dimension.height : compElement.Offset.Y;

                Vector2 measure = new Vector2();
                measure.X = compElement.HorizontalMeasureType == MeasureType.Percentage ? compElement.Measure.X * dimension.width : compElement.Measure.X;
                measure.Y = compElement.HorizontalMeasureType == MeasureType.Percentage ? compElement.Measure.Y * dimension.height : compElement.Measure.Y;

                if (compElement.AlignAgainstIndex == -1)
                {
                    elements[i].Dimension = CalculateDimension(dimension, offset, measure, compElement.Alignment);
                }
                else
                {
                    var againstElementDim = elements[compElement.AlignAgainstIndex].Dimension;
                    elements[i].Dimension = CalculateDimension(offset, measure, compElement.Alignment, againstElementDim, compElement.AlignmentAgainstOnOutside);
                }
            }

            return elements;
        }

        private Rectangle CalculateDimension(Rectangle constraints, Vector2 offset, Vector2 measure, ContentAlignment alignment)
        {
            Rectangle result = new Rectangle();
            var fullWidth = measure.X > constraints.width;
            var fullHeight = measure.Y > constraints.height;

            if (fullWidth) // ignore any horizontal alignment + offset
            {
                result.width = constraints.width;
                result.x = 0;
            }

            if (fullHeight) // ignore any vertical alignment + offset
            {
                result.height = constraints.height;
                result.y = 0;
            }

            if (!fullWidth)
            {
                if (alignment == ContentAlignment.Left || alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.TopLeft)
                {
                    result.x = offset.X + constraints.x;
                }
                else if (alignment == ContentAlignment.Center || alignment == ContentAlignment.Bottom || alignment == ContentAlignment.Top)
                {
                    result.x = ((constraints.width - measure.X) / 2) + constraints.x;
                }
                else if (alignment == ContentAlignment.Right || alignment == ContentAlignment.BottomRight || alignment == ContentAlignment.TopRight)
                {
                    result.x = constraints.width - measure.X - offset.X + constraints.x;
                }

                result.width = measure.X;
            }

            if (!fullHeight)
            {
                if (alignment == ContentAlignment.Top || alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.TopRight)
                {
                    result.y = offset.Y;
                }
                else if (alignment == ContentAlignment.Center || alignment == ContentAlignment.Left || alignment == ContentAlignment.Right)
                {
                    result.y = ((constraints.height - measure.Y) / 2) + constraints.y;
                }
                else if (alignment == ContentAlignment.Bottom || alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.BottomRight)
                {
                    result.y = constraints.height - measure.Y - offset.Y + constraints.y;
                }

                result.height = measure.Y;
            }

            return result;
        }

        private Rectangle CalculateDimension(Vector2 offset, Vector2 measure, ContentAlignment alignment, Rectangle against, bool outside = false)
        {
            Rectangle result = new Rectangle();
            result.width = measure.X;
            result.height = measure.Y;

            if (!outside)
            {
                result = CalculateDimension(against, offset, measure, alignment);
            }
            else
            {
                if (alignment == ContentAlignment.Bottom)
                {
                    result.y = against.height + against.y + offset.Y;
                    result.x = ((against.width - measure.X) / 2) + against.x;
                }
                else if (alignment == ContentAlignment.BottomLeft)
                {
                    result.y = against.height + against.y + offset.Y;
                    result.x = against.x;
                }
                else if (alignment == ContentAlignment.BottomRight)
                {
                    result.y = against.height + against.y + offset.Y;
                    result.x = (against.width - measure.X) + against.x;
                }
                else if (alignment == ContentAlignment.Center)
                {
                    result.x = ((against.width - measure.X) / 2) + offset.X;
                    result.y = ((against.height - measure.Y) / 2) + offset.Y;
                }
                else if (alignment == ContentAlignment.Left)
                {
                    result.x = against.x - measure.X - offset.X;
                    result.y = ((against.height - measure.Y) / 2) + offset.Y;
                }
                else if (alignment == ContentAlignment.Right)
                {
                    result.x = against.x + against.width + offset.X;
                    result.y = ((against.height - measure.Y) / 2) + offset.Y;
                }
                else if (alignment == ContentAlignment.Top)
                {
                    result.x = ((against.width - measure.X) / 2) + offset.X;
                    result.y = against.y - measure.Y - offset.Y;
                }
                else if (alignment == ContentAlignment.TopLeft)
                {
                    result.x = against.x;
                    result.y = against.y - measure.Y - offset.Y;
                }
                else if (alignment == ContentAlignment.TopRight)
                {
                    result.x = (against.x + against.width) - measure.X - offset.X;
                    result.y = against.y - measure.Y - offset.Y;
                }
            }
            return result;
        }

        private void DrawRectStyle(Rectangle rect, Style style)
        {
            if (!string.IsNullOrEmpty(style.BackgroundImage))
            {
                Application.Instance.ResourceManager.LoadImage(style.BackgroundImage);

                var bgImageTexture = Application.Instance.ResourceManager.GetTexture(style.BackgroundImage);
                var color = new Color(255, 255, 255, (int)(style.Opacity * 255));

                Raylib.DrawTexturePro(bgImageTexture,
                    new Rectangle(0, 0, bgImageTexture.width, bgImageTexture.height),
                    rect,
                    new Vector2(0, 0), 0, color);
            }
            else
            {
                var hasRadiusCorners = style.RadiusCorners > 0.0f;
                if (hasRadiusCorners)
                {
                    // cannot use gradient colours with background rectangles using radius corners.
                    // defaults to using first solid colour input
                    var backgroundColor = style.BackgroundColor.Colors[0];
                    backgroundColor.a = (byte)(style.Opacity * 255);
                    Raylib.DrawRectangleRounded(rect,
                        style.RadiusCorners, (int)(style.RadiusCorners * Math.PI), backgroundColor);
                }
                else
                {
                    if (style.BackgroundColor.Colors == null)
                        goto SKIP_TO_BORDERS;

                    if (style.BackgroundColor.Type == Extras.ColorType.Solid)
                    {
                        var color = style.BackgroundColor.Colors[0];
                        color.a = (byte)(style.Opacity * 255);
                        Raylib.DrawRectangle((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, color);
                    }
                    else if (style.BackgroundColor.Type == Extras.ColorType.GradientHorizontal)
                    {
                        var color1 = style.BackgroundColor.Colors[0];
                        var color2 = style.BackgroundColor.Colors[1];
                        color1.a = (byte)(style.Opacity * 255);
                        color2.a = (byte)(style.Opacity * 255);

                        Raylib.DrawRectangleGradientH((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, color1, color2);
                    }
                    else if (style.BackgroundColor.Type == Extras.ColorType.GradientVertical)
                    {
                        var color1 = style.BackgroundColor.Colors[0];
                        var color2 = style.BackgroundColor.Colors[1];
                        color1.a = (byte)(style.Opacity * 255);
                        color2.a = (byte)(style.Opacity * 255);

                        Raylib.DrawRectangleGradientV((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, color1, color2);
                    }

                    SKIP_TO_BORDERS:

                    // draw borders
                    if (style.Borders[0]) // top
                    {
                        var color = style.BorderColors[0];
                        color.a = (byte)(style.Opacity * 255);
                        Raylib.DrawLineEx(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y), style.BorderThicknesses[0], color);
                    }

                    if (style.Borders[1]) // left
                    {
                        var color = style.BorderColors[1];
                        color.a = (byte)(style.Opacity * 255);
                        Raylib.DrawLineEx(new Vector2(rect.x, rect.y), new Vector2(rect.x, rect.y + rect.height), style.BorderThicknesses[1], color);
                    }

                    if (style.Borders[2]) // right
                    {
                        var color = style.BorderColors[2];
                        color.a = (byte)(style.Opacity * 255);
                        Raylib.DrawLineEx(new Vector2(rect.x + rect.width, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), style.BorderThicknesses[2], color);
                    }

                    if (style.Borders[3]) // bottom
                    {
                        var color = style.BorderColors[3];
                        color.a = (byte)(style.Opacity * 255);
                        Raylib.DrawLineEx(new Vector2(rect.x, rect.y + rect.height), new Vector2(rect.x + rect.width, rect.y + rect.height), style.BorderThicknesses[3], color);
                    }
                }
            }
        }

        private void DrawText(int index, TextDim textDim, Color color, ContentAlignment alignment)
        {
            var textX = 0.0f;
            var textY = 0.0f;
            if (alignment == ContentAlignment.Left)
            {
                textX = elements[index].Dimension.x + childInnerPadding;
                textY = ((elements[index].Dimension.height - textDim.ContentSize.Y) / 2) + elements[index].Dimension.y;
            }
            else if (alignment == ContentAlignment.Center)
            {
                textX = ((elements[index].Dimension.width - textDim.ContentSize.X) / 2) + elements[index].Dimension.x;
                textY = ((elements[index].Dimension.height - textDim.ContentSize.Y) / 2) + elements[index].Dimension.y;
            }
            else if (alignment == ContentAlignment.Right)
            {
                textX = (elements[index].Dimension.x + elements[index].Dimension.width) - textDim.ContentSize.X - childInnerPadding;
                textY = ((elements[index].Dimension.height - textDim.ContentSize.Y) / 2) + elements[index].Dimension.y;
            }
            else if (alignment == ContentAlignment.TopLeft)
            {
                textX = elements[index].Dimension.x + childInnerPadding;
                textY = elements[index].Dimension.y + childInnerPadding;
            }
            else if (alignment == ContentAlignment.Top)
            {
                textX = ((elements[index].Dimension.width - textDim.ContentSize.X) / 2) + elements[index].Dimension.x;
                textY = elements[index].Dimension.y + childInnerPadding;
            }
            else if (alignment == ContentAlignment.TopRight)
            {
                textX = (elements[index].Dimension.x + elements[index].Dimension.width) - textDim.ContentSize.X - childInnerPadding;
                textY = elements[index].Dimension.y + childInnerPadding;
            }
            else if (alignment == ContentAlignment.BottomLeft)
            {
                textX = elements[index].Dimension.x + childInnerPadding;
                textY = elements[index].Dimension.y + elements[index].Dimension.height - textDim.ContentSize.Y - childInnerPadding;
            }
            else if (alignment == ContentAlignment.Bottom)
            {
                textX = ((elements[index].Dimension.width - textDim.ContentSize.X) / 2) + elements[index].Dimension.x;
                textY = elements[index].Dimension.y + elements[index].Dimension.height - textDim.ContentSize.Y - childInnerPadding;
            }
            else if (alignment == ContentAlignment.BottomRight)
            {
                textX = (elements[index].Dimension.x + elements[index].Dimension.width) - textDim.ContentSize.X - childInnerPadding;
                textY = elements[index].Dimension.y + elements[index].Dimension.height - textDim.ContentSize.Y - childInnerPadding;
            }

            Raylib.BeginScissorMode((int)textX, (int)textY, (int)textDim.ContentSize.X, (int)textDim.ContentSize.Y);
            Utils.RenderMultilineText(Application.Instance.ResourceManager.GetFont(currentFontName), currentFontSize, new Vector2(textX, textY), currentFontSpacing, textDim, color);
            Raylib.EndScissorMode();
        }

        #endregion

        #endregion

        private Rectangle CalculateNextDimension(int elementIndex, string textOrImageName = "")
        {
            var currentRowElements = elements.Where((e) => e.GridIndex == currentGridIndex && e.CellIndex == currentCellIndex && e.IsBaseElement);

            Image imageToUse = new Image();
            bool usingImage = false;

            if (textOrImageName.StartsWith("image:"))
            {
                var imageName = textOrImageName["image:".Length..];
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
                var count = currentRowElements.Count();
                for (int i = 0; i < count; i++)
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

            if (currentRowElements.Any())
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

                if (currentLayoutFlags.HasFlag(LayoutFlags.DynamicColumns))
                    widthToBecome = textDim.ContentSize.X;

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

            return new Rectangle(xToBecome, yToBecome, widthToBecome, heightToBecome);
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
                if (Raylib.CheckCollisionPointRec(GetMousePosition(), element.Dimension))
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
                if (Raylib.CheckCollisionPointRec(GetMousePosition(), grid.Dimension))
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
                    if (active.Type != ElementType.Interactive && active.Type != ElementType.Input)
                    {
                        index -= 1;
                        continue;
                    }

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
                    else
                    {
                        active.State = ElementState.Hovered;
                    }

                    index -= 1;
                }
                
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT) && !tempClick)
            {
                firstClick = true;
            }
        }

        /// <summary>
        /// Called by UI when UI.Render is called. Do not use directly.
        /// </summary>
        public void Render()
        {
            foreach (var image in backgroundImages)
            {
                if (!preloadedAll)
                    Application.Instance.ResourceManager.LoadImage(image);

                var textureBG = Application.Instance.ResourceManager.GetTexture(image);
                Raylib.DrawTexturePro(textureBG,
                    new Rectangle(0, 0, textureBG.width, textureBG.height),
                    new Rectangle(0, 0, backBufferWidth, backBufferHeight),
                    new Vector2(0, 0), 0, Color.WHITE);
            }

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
                    var name = Name + "_Grid_" + gridIndex + "_Cell_" + i + "_Shadow";
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
                        if (grid.BackgroundColors[i].Colors == null)
                            goto DRAW_BORDERS;

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

                        DRAW_BORDERS:

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
