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
using TwinspireCS.Engine.Render;
using TwinspireCS.Engine.Graphics;
using TwinspireCS.Engine.Input;

namespace TwinspireCS.Engine.GUI
{
    public class Canvas
    {

        private GameContext gameContext;
        private List<Grid> layouts;
        private List<Element> elements;
        private List<string> elementsToAdd;
        private IDictionary<string, int[]> elementIdCache;
        private IDictionary<int, TextDim> elementTexts;
        private int currentElementIndex;
        private bool elementsChanged;
        private int forceChangeElementIndex;
        private ElementState forceChangeElementStateTo;

        private List<int> animationIndices;
        private List<Tween> tweens;
        private List<bool> tweensRunning;
        private IDictionary<string, int[]> elementTweens;
        private List<Tween> tweenStack;

        private List<MenuWrapper> menuWrappers;
        private int currentMenuWrapper;

        private List<string> backgroundImages;

        private int activeElement = -1;

        private int actualMouseX;
        private int actualMouseY;
        private int backBufferWidth;
        private int backBufferHeight;
        private RenderTexture2D backBuffer;

        private int currentGridIndex;
        private int currentCellIndex;
        private FlowDirection currentFlowDirection;
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
        private Color currentFontColor;

        private int childInnerPadding;

        private List<Rectangle> dragDropRegions;
        private List<string> dragDropAcceptedFileTypes;
        private List<string> dragDropFilesDropped;
        private int dragDropCurrentRegion;

        private bool firstClick;
        private const float firstClickDelay = 0.25f;
        private float firstClickTime;

        private int fadeOutEffectAnimateIndex = -1;
        private float fadeOutEffectDuration;
        private Color fadeOutEffectToColor;
        private bool fadingOut;

        private int fadeInEffectAnimateIndex = -1;
        private float fadeInEffectDuration;
        private Color fadeInEffectFromColor;
        private bool fadingIn;

        private int previousAfterTransitionAnimateIndex;
        private int afterTransitionAnimateIndex;
        private float afterTransitionDuration;
        private Action afterTransitionCallback;
        private bool afterTransitionComplete;

        private bool disableNextItem;

        public IEnumerable<Grid> Layouts => layouts;

        public string Name { get; set; }

        public Canvas()
        {
            layouts = new List<Grid>();
            elements = new List<Element>();
            elementIdCache = new Dictionary<string, int[]>();
            elementsToAdd = new List<string>();
            elementTexts = new Dictionary<int, TextDim>();
            animationIndices = new List<int>();
            styleStack = new List<Style>();
            tweens = new List<Tween>();
            tweenStack = new List<Tween>();
            elementTweens = new Dictionary<string, int[]>();
            tweensRunning = new List<bool>();
            activeElement = 0;
            elementsChanged = true;

            previousAfterTransitionAnimateIndex = -1;
            afterTransitionAnimateIndex = -1;
            afterTransitionDuration = 0.0f;

            menuWrappers = new List<MenuWrapper>();
            currentMenuWrapper = -1;

            backgroundImages = new List<string>();

            dragDropRegions = new List<Rectangle>();
            dragDropAcceptedFileTypes = new List<string>();
            dragDropFilesDropped = new List<string>();
            
            currentGridIndex = 0;
            currentCellIndex = 0;
            currentFlowDirection = FlowDirection.LeftToRight;
            currentFontColor = Color.BLACK;

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

        public void SetGameContext(GameContext context)
        {
            gameContext = context;
        }

        public void SetBackBuffer(RenderTexture2D backBuffer)
        {
            this.backBuffer = backBuffer;
        }

        public void AddBackgroundImage(string imageName)
        {
            backgroundImages.Add(imageName);
        }

        public void Begin()
        {
            if (!requestRebuild)
            {
                foreach (var element in elements)
                {
                    element.Rendered = false;
                }
            }

            if (requestRebuild || firstBuild)
            {
                elements.Clear();
                elementTexts.Clear();
                elementIdCache.Clear();
                elementTweens.Clear();

                dragDropRegions.Clear();
                dragDropAcceptedFileTypes.Clear();

                menuWrappers.Clear();
            }
            else
            {
                foreach (var element in elements)
                {
                    element.LastState = element.State;
                    element.State = element.NextState;
                }
            }

            currentElementIndex = -1;

            HotKeys.DisableHotkeys(afterTransitionAnimateIndex > -1);
        }

        public int GetLastElementIndex()
        {
            return currentElementIndex;
        }
        
        /// <summary>
        /// Checks if the next element after this call will become the given state. If the state
        /// of the element is already this state, or there is no next element, the result is <c>false</c>.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns></returns>
        public bool IsNextElementState(ElementState state)
        {
            var elementToSelect = currentElementIndex + 1;
            var temp = currentElementIndex;
            if (elements.Count == 0)
                return false;

            while (!elements[elementToSelect].IsBaseElement &&
                        elements[elementToSelect].Type != ElementType.Interactive)
            {
                elementToSelect += 1;
                if (elementToSelect >= elements.Count - 1)
                {
                    elementToSelect = temp;
                    break;
                }
            }

            return elements[elementToSelect].NextState == state &&
                elements[elementToSelect].State != state &&
                elementToSelect != temp;
        }
        
        /// <summary>
        /// Checks if the element at the given index is currently the given state. If the next state for the
        /// given element is the same as the current state of the element, the result is <c>false</c>.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsElementState(int index, ElementState state)
        {
            if (index > -1 && index < elements.Count)
                return elements[index].State == state && 
                    elements[index].NextState != state;

            return false;
        }

        public void SetNextItemDisabled()
        {
            disableNextItem = true;
        }

        public void TransformMousePosition(int width, int height)
        {
            backBufferWidth = width;
            backBufferHeight = height;
            actualMouseX = (int)(Raylib.GetMouseX() * (float)((float)width / (float)Raylib.GetRenderWidth()));
            actualMouseY = (int)(Raylib.GetMouseY() * (float)((float)height / (float)Raylib.GetRenderHeight()));
        }

        public Vector2 GetMousePosition()
        {
            TransformMousePosition(backBufferWidth, backBufferHeight);
            return new Vector2(actualMouseX, actualMouseY);
        }

        public void End()
        {
            if (fadingIn)
            {
                if (Animate.Tick(fadeInEffectAnimateIndex, fadeInEffectDuration))
                {
                    fadingIn = false;
                }

                var color = new Color(fadeInEffectFromColor.r, fadeInEffectFromColor.g, fadeInEffectFromColor.b, (byte)(255 - (Animate.GetRatio(fadeInEffectAnimateIndex) * 255)));
                Raylib.DrawRectangle(0, 0, backBufferWidth, backBufferHeight, color);
            }

            if (fadingOut && !fadingIn)
            {
                if (Animate.Tick(fadeOutEffectAnimateIndex, fadeOutEffectDuration))
                {
                    fadingOut = false;
                }

                var color = new Color(fadeOutEffectToColor.r, fadeOutEffectToColor.g, fadeOutEffectToColor.b, (byte)(Animate.GetRatio(fadeOutEffectAnimateIndex) * 255));
                Raylib.DrawRectangle(0, 0, backBufferWidth, backBufferHeight, color);
            }

            if (afterTransitionComplete)
                return;

            if (afterTransitionAnimateIndex > -1)
            {
                if (Animate.Tick(afterTransitionAnimateIndex, afterTransitionDuration))
                {
                    afterTransitionCallback();
                    afterTransitionComplete = true;
                    return;
                }
            }

            foreach (var id in elementIdCache)
            {
                var found = true;
                foreach (var key in elementsToAdd)
                {
                    if (id.Key != key)
                    {
                        found = false;
                        break;
                    }
                }

                if (!found)
                {
                    requestRebuild = true;
                    elementsChanged = false;
                    break;
                }
                else
                {
                    requestRebuild = false;
                }
            }

            elementsToAdd.Clear();   

            if (requestRebuild)
            {
                foreach (var id in animationIndices)
                {
                    Animate.Reset(id);
                }
            }

            if (firstBuild)
            {
                requestRebuild = false;
                firstBuild = false;
            }

            Animate.ResetTicks();
        }

        /// <summary>
        /// Performs a call to the given callback after the given delay. Once the callback is called, this
        /// canvas blocks further render instructions until <c>ResetAfter</c> is called.
        /// 
        /// This also disables event simulation and hotkeys on this canvas.
        /// </summary>
        /// <param name="delay">The delay, in seconds, before the callback method is called.</param>
        /// <param name="callback">The method to call after the delay timer has passed.</param>
        public void After(float delay, Action callback)
        {
            if (afterTransitionAnimateIndex == -1)
            {
                if (previousAfterTransitionAnimateIndex > -1)
                {
                    afterTransitionAnimateIndex = previousAfterTransitionAnimateIndex;
                }
                else
                {
                    afterTransitionAnimateIndex = Animate.Create();
                }
            }

            afterTransitionDuration = delay;
            afterTransitionCallback = callback;
        }

        /// <summary>
        /// Resets the After transition to allow rendering this canvas again.
        /// </summary>
        public void ResetAfter()
        {
            afterTransitionComplete = false;
            previousAfterTransitionAnimateIndex = afterTransitionAnimateIndex;
            afterTransitionAnimateIndex = -1;
        }

        public void ForceChangeElementState(int elementIndex, ElementState state)
        {
            forceChangeElementIndex = elementIndex;
            forceChangeElementStateTo = state;
        }

        public void DrawTo(int gridIndex, int cellIndex, LayoutFlags flags = LayoutFlags.DynamicRows | LayoutFlags.DynamicColumns)
        {
            currentGridIndex = gridIndex;
            currentCellIndex = cellIndex;
            currentLayoutFlags = flags;
        }

        public void SetFlowDirection(FlowDirection direction)
        {
            currentFlowDirection = direction;
        }

        public void SetFixedRowHeight(float height)
        {
            fixedRowHeight = height;
        }

        public void SetFixedColumnWidth(float width)
        {
            fixedColumnWidth = width;
        }

        public void BeginDragDrop(string filters)
        {
            var gridCellDim = layouts.ElementAt(currentGridIndex).GetCellDimension(currentCellIndex);
            dragDropRegions.Add(gridCellDim);
            dragDropAcceptedFileTypes.Add(filters);
            dragDropCurrentRegion = dragDropRegions.Count - 1;
        }

        public string[]? GetDroppedFiles()
        {
            if (dragDropCurrentRegion > -1)
            {
                return dragDropFilesDropped.ToArray();
            }

            return null;
        }

        public void EndDragDrop()
        {
            dragDropCurrentRegion = -1;
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

        public void BeginMenuWrapper(string id, string confirmHotkeyName)
        {
            if (currentMenuWrapper > -1)
            {
                throw new Exception("Cannot wrap a menu inside another menu.");
            }

            var menus = menuWrappers.Where((wrap) => wrap.ID == id);
            if (menus.Any())
            {
                var menu = menus.First();
                if (HotKeys.IsHotkeyPressed(HotKeys.UP))
                {
                    if (menu.SelectedElement - 1 < 0)
                    {
                        menu.SelectedElement = 0;
                    }
                    else
                    {
                        var elementToSelect = menu.StartElement + menu.SelectedElement - 1;
                        while (!elements[elementToSelect].IsBaseElement && 
                            elements[elementToSelect].Type != ElementType.Interactive)
                        {
                            elementToSelect -= 1;
                        }
                        menu.SelectedElement = elementToSelect;
                        ForceChangeElementState(elementToSelect, ElementState.Hovered);
                    }
                }
                else if (HotKeys.IsHotkeyPressed(HotKeys.DOWN))
                {
                    var count = menu.EndElement - menu.StartElement;
                    var temp = menu.SelectedElement;
                    var elementToSelect = menu.SelectedElement + menu.StartElement + 1;
                    if (elementToSelect >= elements.Count - 1)
                    {
                        menu.SelectedElement = temp;
                    }
                    else
                    {
                        while (!elements[elementToSelect].IsBaseElement &&
                        elements[elementToSelect].Type != ElementType.Interactive)
                        {
                            elementToSelect += 1;
                            if (elementToSelect > count || elementToSelect >= elements.Count - 1)
                            {
                                elementToSelect = temp;
                                break;
                            }
                        }

                        menu.SelectedElement = elementToSelect;
                        ForceChangeElementState(elementToSelect, ElementState.Hovered);
                    }
                }
            }
            else
            {
                var menuWrapper = new MenuWrapper();
                menuWrapper.ID = id;
                menuWrapper.StartElement = elements.Count;
                menuWrapper.ConfirmKeyName = confirmHotkeyName;
                menuWrappers.Add(menuWrapper);
            }

            var index = menuWrappers.FindIndex((wrap) => wrap.ID == id);
            if (index > -1)
            {
                currentMenuWrapper = index;
            }
            else
            {
                currentMenuWrapper = menuWrappers.Count - 1;
            }
        }

        public void EndMenuWrapper()
        {
            if (currentMenuWrapper == -1)
            {
                TwinspireCS.Utils.Warn("Trying to end a menu wrapper where one is not open.", 1);
            }

            var menu = menuWrappers[currentMenuWrapper];
            if (requestRebuild)
            {
                menu.EndElement = elements.Count;
            }
            else
            {
                if (HotKeys.IsHotkeyReleased(menu.ConfirmKeyName))
                {
                    ForceChangeElementState(menu.SelectedElement, ElementState.Clicked);                    
                }
            }

            currentMenuWrapper = -1;
        }

        #region Canvas Effects

        public void FadeOutCanvas(float duration, Color to)
        {
            if (fadeOutEffectAnimateIndex == -1)
            {
                fadeOutEffectAnimateIndex = Animate.Create();
                fadeOutEffectDuration = duration;
                fadeOutEffectToColor = to;
                fadingOut = true;
            }
        }

        public void FadeInCanvas(float duration, Color from)
        {
            if (fadeInEffectAnimateIndex == -1)
            {
                fadeInEffectAnimateIndex = Animate.Create();
                fadeInEffectDuration = duration;
                fadeInEffectFromColor = from;
                fadingIn = true;
            }
        }

        #endregion

        #region Styling

        public void SetFont(string fontName, int fontSize, int spacing)
        {
            currentFontName = fontName;
            currentFontSize = fontSize;
            currentFontSpacing = spacing;
        }

        public void SetFontColor(Color color)
        {
            currentFontColor = color;
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

        protected Style GetLocalStyle(ElementState state, string defaultState)
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

            return Theme.Default.Styles[defaultState];
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

        protected Style? GetTweenState(string id, float ratio)
        {
            if (tweenStack.Count > 0)
            {
                var tween = tweenStack.Last();
                if (tween.From.Spritesheet > -1 || tween.To.Spritesheet > -1)
                    return tween.From;

                return CreateStyleFromRatio(ratio, tween);
            }
            else if (elementTweens.ContainsKey(id))
            {
                var tweenIndices = elementTweens[id];
                var actualTween = tweens[tweenIndices[0]];
                if (actualTween.From.Spritesheet > -1 || actualTween.To.Spritesheet > -1)
                    return actualTween.From;

                return CreateStyleFromRatio(ratio, actualTween);
            }

            return null;
        }

        private Style CreateStyleFromRatio(float ratio, Tween actualTween)
        {
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

        #endregion

        #region UI Drawing

        public ElementState Button(string id, string text, ContentAlignment alignment = ContentAlignment.Center)
        {
            if (afterTransitionComplete && afterTransitionAnimateIndex > -1)
                return ElementState.Idle;

            if (!requestRebuild && elementIdCache.ContainsKey(id))
            {
                var index = elementIdCache[id];
                var element = elements[index[0]];
                for (int i = index[0]; i < (index[1] + index[0]); i++)
                {
                    elements[i].Rendered = true;
                }

                currentElementIndex = index[0];

                var hoverTween = id + ":hover";
                string stateToChangeTo = "";
                if (currentMenuWrapper > -1)
                {
                    var menu = menuWrappers[currentMenuWrapper];
                    if (menu.SelectedElement == index[0]
                        && (HotKeys.IsHotkeyDown(menu.ConfirmKeyName) 
                        || elements[index[0]].State == ElementState.Active))
                    {
                        stateToChangeTo = Theme.BUTTON_DOWN;
                    }
                    else if (menu.SelectedElement == index[0])
                    {
                        stateToChangeTo = Theme.BUTTON_HOVER;
                    }
                    else
                    {
                        stateToChangeTo = Theme.BUTTON;
                    }
                }
                else if (disableNextItem)
                {
                    element.State = ElementState.Inactive;
                    stateToChangeTo = Theme.DISABLED;
                    disableNextItem = false;
                }
                else if (element.State == ElementState.Idle)
                {
                    stateToChangeTo = Theme.BUTTON;
                }
                else if (element.State == ElementState.Hovered || element.State == ElementState.Clicked || element.State == ElementState.Focused)
                {
                    stateToChangeTo = Theme.BUTTON_HOVER;
                }
                else if (element.State == ElementState.Active)
                {
                    stateToChangeTo = Theme.BUTTON_DOWN;
                }

                var style = GetLocalStyle(element.State, stateToChangeTo);
                if (elementTweens.ContainsKey(hoverTween) && style.Spritesheet == -1)
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
                        DrawRectStyle(element.Dimension, style);
                    }
                }
                else
                {
                    DrawRectStyle(element.Dimension, style);
                }

                TextDim textDim;
                if (elementTexts.ContainsKey(index[0]) && elements[index[0] + 1].Visible)
                {
                    textDim = elementTexts[index[0]];
                    DrawText(index[0], textDim, currentFontColor, alignment);
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
                    build[i].Visible = true;
                    build[i].ID = id;

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
            else
            {
                elementsChanged = true;
                elementsToAdd.Add(id);
            }

            return ElementState.Idle;
        }

        public ElementState Label(string id, string text, ContentAlignment alignment = ContentAlignment.Center)
        {
            if (afterTransitionComplete && afterTransitionAnimateIndex > -1)
                return ElementState.Idle;

            if (!requestRebuild && elementIdCache.ContainsKey(id))
            {
                var index = elementIdCache[id];
                var element = elements[index[0]];
                for (int i = index[0]; i < (index[1] + index[0]); i++)
                {
                    elements[i].Rendered = true;
                }

                currentElementIndex = index[0];

                TextDim textDim;
                if (elementTexts.ContainsKey(index[0]))
                {
                    textDim = elementTexts[index[0]];
                    DrawText(index[0], textDim, currentFontColor, alignment);
                }

                return element.State;
            }
            else if (requestRebuild)
            {
                var elementDim = CalculateNextDimension(elements.Count, text);
                var build = BuildElementsFromComponent("Label", elementDim);
                int elementsLength = build.Length;
                for (int i = 0; i < build.Length; i++)
                {
                    build[i].IsBaseElement = i == 0;
                    build[i].CellIndex = currentCellIndex;
                    build[i].GridIndex = currentGridIndex;
                    build[i].Visible = true;
                    build[i].ID = id;

                    elements.Add(build[i]);
                    if (i == 0)
                        elementIdCache.Add(id, new int[] { elements.Count - 1, elementsLength });
                }
            }
            else
            {
                elementsChanged = true;
                elementsToAdd.Add(id);
            }

            return ElementState.Idle;
        }

        public ElementState ButtonImage(string id, string text, string imageName, Vector2 imageSize, ImageAlignment imageAlignment, ButtonImageFormat imageFormat, ContentAlignment alignment = ContentAlignment.Center)
        {
            if (afterTransitionComplete && afterTransitionAnimateIndex > -1)
                return ElementState.Idle;

            if (!requestRebuild && elementIdCache.ContainsKey(id))
            {
                var index = elementIdCache[id];
                var element = elements[index[0]];
                for (int i = index[0]; i < (index[1] + index[0]); i++)
                {
                    elements[i].Rendered = true;
                }

                currentElementIndex = index[0];

                var hoverTween = id + ":hover";
                string stateToChangeTo = "";
                if (currentMenuWrapper > -1)
                {
                    var menu = menuWrappers[currentMenuWrapper];
                    if (menu.SelectedElement == index[0]
                        && (HotKeys.IsHotkeyDown(menu.ConfirmKeyName)
                        || elements[index[0]].State == ElementState.Active))
                    {
                        stateToChangeTo = Theme.BUTTON_DOWN;
                    }
                    else if (menu.SelectedElement == index[0])
                    {
                        stateToChangeTo = Theme.BUTTON_HOVER;
                    }
                    else
                    {
                        stateToChangeTo = Theme.BUTTON;
                    }
                }
                else if (disableNextItem)
                {
                    element.State = ElementState.Inactive;
                    stateToChangeTo = Theme.DISABLED;
                    disableNextItem = false;
                }
                else if (element.State == ElementState.Idle)
                {
                    stateToChangeTo = Theme.BUTTON;
                }
                else if (element.State == ElementState.Hovered || element.State == ElementState.Clicked || element.State == ElementState.Focused)
                {
                    stateToChangeTo = Theme.BUTTON_HOVER;
                }
                else if (element.State == ElementState.Active)
                {
                    stateToChangeTo = Theme.BUTTON_DOWN;
                }

                var style = GetLocalStyle(element.State, stateToChangeTo);
                if (elementTweens.ContainsKey(hoverTween) && style.Spritesheet == -1)
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
                        DrawRectStyle(element.Dimension, style);
                    }
                }
                else
                {
                    DrawRectStyle(element.Dimension, style);
                }

                var imageElement = elements[index[0] + 1];

                TextDim textDim = null;
                if (elementTexts.ContainsKey(index[0]))
                {
                    textDim = elementTexts[index[0]];
                }

                var imageTexture = Application.Instance.ResourceManager.GetTexture(imageName);
                Raylib.DrawTexturePro(imageTexture, new Rectangle(0, 0, imageTexture.width, imageTexture.height), 
                    new Rectangle(imageElement.Dimension.x, imageElement.Dimension.y, imageSize.X, imageSize.Y), new Vector2(0, 0), 0, Color.WHITE);

                if (imageFormat == ButtonImageFormat.ImageAndText)
                {
                    if (textDim != null)
                    {
                        DrawText(index[0] + 2, textDim, style.ForegroundColor, alignment);
                    }
                }

                return element.State;
            }
            else if (requestRebuild)
            {
                Rectangle elementDim = CalculateNextDimension(elements.Count, text + "<image:" + imageName + ">", true);
                if (imageFormat == ButtonImageFormat.Image || imageFormat == ButtonImageFormat.ImageTextAsTooltip)
                {
                    elementDim.width = imageSize.X + childInnerPadding * 2;
                    elementDim.height = imageSize.Y + childInnerPadding * 2;
                }

                var build = BuildElementsFromComponent("ButtonImage", elementDim);
                int elementsLength = build.Length;
                TextDim textInfo = null;
                if (elementTexts.ContainsKey(elements.Count))
                {
                    textInfo = elementTexts[elements.Count];
                }

                for (int i = 0; i < build.Length; i++)
                {
                    build[i].IsBaseElement = i == 0;
                    build[i].CellIndex = currentCellIndex;
                    build[i].GridIndex = currentGridIndex;
                    build[i].Visible = true;
                    build[i].ID = id;


                    var totalElementsToBuild = build.Length;
                    if (imageFormat == ButtonImageFormat.ImageTextAsTooltip)
                    {
                        totalElementsToBuild = totalElementsToBuild + 1;
                    }

                    if (i == 1) // for image component
                    {
                        if (imageFormat == ButtonImageFormat.ImageAndText)
                        {
                            if (imageAlignment == ImageAlignment.BeforeText)
                            { 
                                ContentAlignment againstAlignment = ContentAlignment.Right;
                                if (alignment == ContentAlignment.Bottom || alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.BottomRight)
                                {
                                    // if alignment at bottom, set label alignment first, then image
                                    build[i + 1].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), new Vector2(build[i].Dimension.width, build[i].Dimension.height), alignment);
                                    if (alignment == ContentAlignment.Bottom)
                                    {
                                        build[i].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), textInfo.ContentSize, ContentAlignment.Top);
                                    }
                                    else if (alignment == ContentAlignment.BottomLeft)
                                    {
                                        build[i].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), textInfo.ContentSize, ContentAlignment.TopLeft);
                                    }
                                    else if (alignment == ContentAlignment.BottomRight)
                                    {
                                        build[i].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), textInfo.ContentSize, ContentAlignment.TopRight);
                                    }
                                }
                                else
                                {
                                    // if alignment elsewhere, set image alignment first, then text
                                    build[i].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), imageSize, alignment);
                                    if (alignment == ContentAlignment.Center)
                                    {
                                        againstAlignment = ContentAlignment.Bottom;
                                    }
                                    else if (alignment == ContentAlignment.Right)
                                    {
                                        againstAlignment = ContentAlignment.BottomRight;
                                    }
                                    else if (alignment == ContentAlignment.Top)
                                    {
                                        againstAlignment = ContentAlignment.Bottom;
                                    }
                                    else if (alignment == ContentAlignment.TopRight)
                                    {
                                        againstAlignment = ContentAlignment.BottomRight;
                                    }
                                    build[i + 1].Dimension = CalculateDimension(new Vector2(0, 0), textInfo.ContentSize, againstAlignment, build[i].Dimension, true);
                                }
                            }
                            else if (imageAlignment == ImageAlignment.AfterText)
                            {
                                ContentAlignment againstAlignment = ContentAlignment.Right;
                                if (alignment == ContentAlignment.Bottom || alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.BottomRight)
                                {
                                    // if alignment at bottom, set image alignment first, then label
                                    build[i].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), new Vector2(build[i].Dimension.width, build[i].Dimension.height), alignment);
                                    if (alignment == ContentAlignment.Bottom)
                                    {
                                        build[i + 1].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), textInfo.ContentSize, ContentAlignment.Top);
                                    }
                                    else if (alignment == ContentAlignment.BottomLeft)
                                    {
                                        build[i + 1].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), textInfo.ContentSize, ContentAlignment.TopLeft);
                                    }
                                    else if (alignment == ContentAlignment.BottomRight)
                                    {
                                        build[i + 1].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), textInfo.ContentSize, ContentAlignment.TopRight);
                                    }
                                }
                                else
                                {
                                    // if alignment elsewhere, set label alignment first, then image
                                    build[i + 1].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), imageSize, alignment);
                                    if (alignment == ContentAlignment.Center)
                                    {
                                        againstAlignment = ContentAlignment.Bottom;
                                    }
                                    else if (alignment == ContentAlignment.Right)
                                    {
                                        againstAlignment = ContentAlignment.BottomRight;
                                    }
                                    else if (alignment == ContentAlignment.Top)
                                    {
                                        againstAlignment = ContentAlignment.Bottom;
                                    }
                                    else if (alignment == ContentAlignment.TopRight)
                                    {
                                        againstAlignment = ContentAlignment.BottomRight;
                                    }
                                    build[i].Dimension = CalculateDimension(new Vector2(childInnerPadding, childInnerPadding), textInfo.ContentSize, againstAlignment, build[0].Dimension, true);
                                }
                            }
                        }
                        else
                        {
                            build[i].Dimension = CalculateDimension(build[0].Dimension, new Vector2(childInnerPadding, childInnerPadding), imageSize, alignment);
                            build[i + 1].Visible = false;
                        }
                    }

                    elements.Add(build[i]);
                    if (i == 0)
                        elementIdCache.Add(id, new int[] { elements.Count - 1, totalElementsToBuild });

                    if (imageFormat == ButtonImageFormat.ImageTextAsTooltip)
                    {
                        var toolTipElement = new Element();
                        toolTipElement.CellIndex = -1;
                        toolTipElement.GridIndex = -1;
                        toolTipElement.IsBaseElement = true;
                        toolTipElement.State = ElementState.Idle;
                        toolTipElement.Type = ElementType.NonInteractive;
                        toolTipElement.Visible = false;
                        toolTipElement.ID = id + "_tooltip";
                        // set position at zero, will be determined on-demand
                        toolTipElement.Dimension = new Rectangle(0, 0, textInfo.ContentSize.X + (childInnerPadding * 2), textInfo.ContentSize.Y + (childInnerPadding * 2));
                        elements.Add(toolTipElement);
                    }
                }
            }
            else
            {
                elementsChanged = true;
                elementsToAdd.Add(id);
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
        protected Element[]? BuildElementsFromComponent(string componentName, Rectangle dimension)
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
                var compInterpreted = component.ElementInterpetedTypes[i];
                elements[i].Type = compElement.Type;
                elements[i].State = ElementState.Idle;
                elements[i].Shape = compElement.Shape;

                Vector2 offset = new Vector2();
                offset.X = compElement.HorizontalMeasureType == MeasureType.Percentage ? compElement.Offset.X * dimension.width : compElement.Offset.X;
                offset.Y = compElement.VerticalMeasureType == MeasureType.Percentage ? compElement.Offset.Y * dimension.height : compElement.Offset.Y;

                Vector2 measure = new Vector2();
                var pixelWidth = compElement.Measure.X;
                var pixelHeight = compElement.Measure.Y;

                // if text is the first element, use the maximum dimension for both width and height
                // to perform the correct content alignment
                if (compInterpreted == InterpretedType.Text && i == 0)
                {
                    pixelWidth = dimension.width;
                    pixelHeight = dimension.height;
                }

                measure.X = compElement.HorizontalMeasureType == MeasureType.Percentage ? compElement.Measure.X * dimension.width : pixelWidth;
                measure.Y = compElement.HorizontalMeasureType == MeasureType.Percentage ? compElement.Measure.Y * dimension.height : pixelHeight;

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

        protected Rectangle CalculateDimension(Rectangle constraints, Vector2 offset, Vector2 measure, ContentAlignment alignment)
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
                    result.y = constraints.y + offset.Y;
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

        protected Rectangle CalculateDimension(Vector2 offset, Vector2 measure, ContentAlignment alignment, Rectangle against, bool outside = false)
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
                    result.x = against.x + ((against.width - measure.X) / 2) + offset.X;
                    result.y = against.y + ((against.height - measure.Y) / 2) + offset.Y;
                }
                else if (alignment == ContentAlignment.Left)
                {
                    result.x = against.x - measure.X - offset.X;
                    result.y = against.y + (((against.height - measure.Y) / 2) + offset.Y);
                }
                else if (alignment == ContentAlignment.Right)
                {
                    result.x = against.x + against.width + offset.X;
                    result.y = against.y + (((against.height - measure.Y) / 2) + offset.Y);
                }
                else if (alignment == ContentAlignment.Top)
                {
                    result.x = against.x + (((against.width - measure.X) / 2) + offset.X);
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

        protected void DrawRectStyle(Rectangle rect, Style style)
        {
            if (style.Spritesheet > -1 && style.Spritesheet < Spritesheet.Spritesheets.Count())
            {
                var spritesheet = Spritesheet.Spritesheets.ElementAt(style.Spritesheet);
                if (style.FrameIndex < spritesheet.Frames.Count)
                {
                    var frame = spritesheet.Frames[style.FrameIndex];
                    var texture = Application.Instance.ResourceManager.GetTexture(spritesheet.ImageName);
                    if (frame.UsingPatch)
                    {
                        Raylib.DrawTextureNPatch(texture, frame.Patch, rect, new Vector2(0, 0), 0, Color.WHITE);
                    }
                    else
                    {
                        Raylib.DrawTexturePro(texture, frame.Dimension, rect, new Vector2(0, 0), 0, Color.WHITE);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(style.BackgroundImage))
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

        protected void DrawText(int index, TextDim textDim, Color color, ContentAlignment alignment)
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

        #region Game Rendering

        private Vector2 lookingAtTile;
        private bool mapBufferInited = false;

        public void BuildMapTexture(TileMap map, bool completeRebuild = false)
        {
            RenderTexture2D mapBuffer;
            if (!mapBufferInited || completeRebuild)
            {
                mapBuffer = Raylib.LoadRenderTexture(map.TilesX * map.TileSize, map.TilesY * map.TileSize);
                Application.Instance.ResourceManager.AddResourceRenderTexture("TileMap_" + map.Name, mapBuffer);


                mapBufferInited = true;
            }

            mapBuffer = Application.Instance.ResourceManager.GetRenderTexture("TileMap_" + map.Name);
            Raylib.BeginTextureMode(mapBuffer);
            Raylib.ClearBackground(Color.BLACK);

            foreach (var layer in map.Layers)
            {
                for (int i = 0; i < layer.Tiles.Length; i++)
                {
                    var tile = layer.Tiles[i];
                    var x = map.TileSize * Math.Floor((float)(i % map.TilesX));
                    var y = map.TileSize * Math.Floor((float)(i / map.TilesY));

                    if (tile.Index > -1 && tile.Opacity > 0.0f)
                    {
                        var color = Raylib.ColorAlpha(tile.Tint, tile.Opacity);
                        var actualTileResult = TileSet.GetTileSetFromTileIndex(tile.Index);
                        var tileset = TileSet.TileSets.ElementAt(actualTileResult.TileSetIndex);
                        var texture = Application.Instance.ResourceManager.GetTexture(tileset.Image);
                        var tilesetX = tileset.TileSize * Math.Floor((float)(actualTileResult.TileIndex % (texture.width / tileset.TileSize)));
                        var tilesetY = tileset.TileSize * Math.Floor((float)(actualTileResult.TileIndex / (texture.width / tileset.TileSize)));

                        Raylib.DrawTexturePro(texture,
                            new Rectangle((float)tilesetX, (float)tilesetY, tileset.TileSize, tileset.TileSize),
                            new Rectangle((float)x + tile.Offset.X, (float)y + tile.Offset.Y, map.TileSize, map.TileSize),
                            new Vector2(0, 0),
                            tile.Rotation, color);
                    }
                }
            }

            Raylib.EndTextureMode();
        }


        public void TileMap(TileMap map)
        {
            RenderTexture2D mapBuffer;

            if (!mapBufferInited)
            {
                mapBuffer = Raylib.LoadRenderTexture(map.TilesX * map.TileSize, map.TilesY * map.TileSize);
                Application.Instance.ResourceManager.AddResourceRenderTexture("TileMap_" + map.Name, mapBuffer);
                mapBufferInited = true;

                BuildMapTexture(map);
            }

            //var startTileX = lookingAtTile.X - (renderRegion.RenderTilesX / 2);
            //var startTileY = lookingAtTile.Y - (renderRegion.RenderTilesY / 2);
            //if (startTileX < 0)
            //    startTileX = 0;

            //if (startTileY < 0)
            //    startTileY = 0;

            //var endTileX = lookingAtTile.X + (renderRegion.RenderTilesX / 2);
            //var endTileY = lookingAtTile.Y + (renderRegion.RenderTilesY / 2);
            //var lookaheadX = endTileX + 1;
            //var lookaheadY = endTileY + 1;

            //if (endTileX > map.TilesX)
            //{
            //    endTileX = map.TilesX;
            //}
            //else if (lookaheadX <= map.TilesX)
            //{
            //    endTileX = lookaheadX;
            //}

            //if (endTileY > map.TilesY)
            //{
            //    endTileY = map.TilesY;
            //}
            //else if (lookaheadY <= map.TilesY)
            //{
            //    endTileY = lookaheadY;
            //}

            mapBuffer = Application.Instance.ResourceManager.GetRenderTexture("TileMap_" + map.Name);
            Raylib.DrawTexturePro(mapBuffer.texture,
                new Rectangle(0, 0, mapBuffer.texture.width, -mapBuffer.texture.height),
                new Rectangle(0, 0, mapBuffer.texture.width, mapBuffer.texture.height), 
                new Vector2(0, 0), 0, Color.WHITE);
        }


        #endregion

        protected Rectangle CalculateNextDimension(int elementIndex, string textOrImageName = "", bool includeImage = false)
        {
            var currentRowElements = elements.Where((e) => e.GridIndex == currentGridIndex && e.CellIndex == currentCellIndex && e.IsBaseElement);

            Image imageToUse = new Image();
            bool usingImage = false;

            if (textOrImageName.StartsWith("image:") && !includeImage)
            {
                var imageName = textOrImageName["image:".Length..];
                imageToUse = Application.Instance.ResourceManager.GetImage(imageName);
                usingImage = true;
            }
            else if (includeImage)
            {
                var startImageIndex = textOrImageName.IndexOf("<image:");
                var totalLength = "<image:>".Length;
                if (textOrImageName.Length < startImageIndex + totalLength)
                {
                    var imageName = textOrImageName.Substring(startImageIndex + totalLength - 1, textOrImageName.Length - startImageIndex - 1);
                    imageToUse = Application.Instance.ResourceManager.GetImage(imageName);
                    usingImage = true;
                }

                textOrImageName = textOrImageName[..startImageIndex];
            }

            var gridContentDim = layouts[currentGridIndex].GetContentDimension(currentCellIndex);
            var remainingWidth = gridContentDim.width;
            var xToBecome = 0.0f;
            var yToBecome = 0.0f;
            if (currentFlowDirection == FlowDirection.RightToLeft)
            {
                xToBecome = gridContentDim.width;
            }

            var lastRowHeight = 0.0f;
            if (elementIndex > 0)
            {
                var count = currentRowElements.Count();
                for (int i = 0; i < count; i++)
                {
                    var element = currentRowElements.ElementAt(i);
                    if (currentFlowDirection == FlowDirection.RightToLeft)
                    {
                        xToBecome -= element.Dimension.width;
                    }
                    else
                    {
                        xToBecome += element.Dimension.x;
                    }
                    remainingWidth -= element.Dimension.width;

                    if (element.Dimension.y > lastRowHeight && currentLayoutFlags.HasFlag(LayoutFlags.DynamicRows))
                    {
                        lastRowHeight = element.Dimension.y;
                    }

                    if ((xToBecome > gridContentDim.width && currentFlowDirection == FlowDirection.LeftToRight) || (xToBecome < 0 && currentFlowDirection == FlowDirection.RightToLeft))
                    {
                        if (currentLayoutFlags.HasFlag(LayoutFlags.DynamicRows))
                            yToBecome += lastRowHeight;
                        else if (currentLayoutFlags.HasFlag(LayoutFlags.StaticRows))
                            yToBecome += fixedRowHeight;

                        xToBecome = gridContentDim.width;
                        lastRowHeight = 0;
                        remainingWidth = gridContentDim.width;
                    }
                }
            }

            xToBecome += gridContentDim.x;
            yToBecome += gridContentDim.y;

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
                heightToBecome = gridContentDim.height;
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
                widthToBecome = gridContentDim.width;
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
                    if (currentFlowDirection == FlowDirection.LeftToRight)
                        xToBecome = gridContentDim.x;
                    else
                        xToBecome = gridContentDim.x + gridContentDim.width;
                }
                else if (currentLayoutFlags.HasFlag(LayoutFlags.StaticRows) && widthToBecome >= remainingWidth)
                {
                    yToBecome += fixedRowHeight;
                    if (currentFlowDirection == FlowDirection.LeftToRight)
                        xToBecome = gridContentDim.x;
                    else
                        xToBecome = gridContentDim.x + gridContentDim.width;
                }
                else if (currentLayoutFlags.HasFlag(LayoutFlags.SpanRow))
                {
                    yToBecome += gridContentDim.height;
                    if (currentFlowDirection == FlowDirection.LeftToRight)
                        xToBecome = gridContentDim.x;
                    else
                        xToBecome = gridContentDim.x + gridContentDim.width;
                }
            }

            if (usingImage && !currentLayoutFlags.HasFlag(LayoutFlags.SpanRow) && !currentLayoutFlags.HasFlag(LayoutFlags.SpanColumn)
                && !currentLayoutFlags.HasFlag(LayoutFlags.FixedComponentHeights) && !currentLayoutFlags.HasFlag(LayoutFlags.FixedComponentWidths))
            {
                widthToBecome += imageToUse.width;
                if (!includeImage)
                    heightToBecome += imageToUse.height;

                if (widthToBecome > remainingWidth && currentLayoutFlags.HasFlag(LayoutFlags.DynamicRows))
                {
                    yToBecome += lastRowHeight;
                    if (currentFlowDirection == FlowDirection.LeftToRight)
                        xToBecome = gridContentDim.x;
                    else
                        xToBecome = gridContentDim.x + gridContentDim.width;
                }
            }

            if (currentFlowDirection == FlowDirection.RightToLeft)
                xToBecome -= widthToBecome;

            return new Rectangle(xToBecome, yToBecome, widthToBecome, heightToBecome);
        }

        public void SimulateEvents()
        {
            if (afterTransitionAnimateIndex > -1)
                return;

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
                    element.NextState = ElementState.Focused;
                }
            }

            var possibleActiveElements = new List<int>();
            var possibleActiveGrids = new List<int>();
            var possiblyDropped = new List<int>();
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
                        element.NextState = ElementState.Idle;
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

            for (int i = 0; i < dragDropRegions.Count; i++)
            {
                var region = dragDropRegions[i];
                if (Raylib.CheckCollisionPointRec(GetMousePosition(), region) && Raylib.IsFileDropped())
                {
                    possiblyDropped.Add(i);
                }
            }

            var allowingDroppedFiles = false;
            if (possiblyDropped.Count > 0)
            {
                var lastPossibleDropped = possiblyDropped[possiblyDropped.Count - 1];
                var files = Raylib.GetDroppedFiles();
                var filters = dragDropAcceptedFileTypes[lastPossibleDropped];
                if (!string.IsNullOrWhiteSpace(filters))
                {
                    var fileTypes = filters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < files.Length; i++)
                    {
                        var allowed = false;
                        for (int j = 0; j < fileTypes.Length; j++)
                        {
                            if (Path.GetExtension(files[i]) == fileTypes[j])
                            {
                                allowed = true;
                                break;
                            }
                        }

                        if (allowed)
                        {
                            dragDropFilesDropped.Add(files[i]);
                            allowingDroppedFiles = true;
                        }
                    }
                }
            }

            if (possibleActiveElements.Count > 0 && elements.Count > 0 && !ImGuiController.IsImGuiInteracted())
            {
                int last = possibleActiveElements.Count - 1;
                int index = last;
                while (index > -1 && !allowingDroppedFiles)
                {
                    var activeIndex = possibleActiveElements[index];
                    var active = elements[activeIndex];
                    var firstClickTimePassed = !firstClick && firstClickTime == 0.0f && tempClick;
                    var mouseReleased = Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
                    if (active.Type != ElementType.Interactive && active.Type != ElementType.Input)
                    {
                        index -= 1;
                        continue;
                    }

                    var isElementInMenu = menuWrappers.Where((menu) => activeIndex >= menu.StartElement && activeIndex < menu.EndElement);
                    var changeStateTo = ElementState.Idle;
                    
                    if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && index == last)
                    {
                        changeStateTo = ElementState.Active;
                    }
                    else if ((mouseReleased && index == last && firstClickTimePassed) || (firstClickTimePassed && index == last))
                    {
                        if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_RIGHT) && !doubleClick)
                            changeStateTo = ElementState.ContextMenu;
                        else if (doubleClick)
                            changeStateTo = ElementState.DoubleClicked;
                        else
                            changeStateTo = ElementState.Clicked;
                    }
                    else
                    {
                        if (isElementInMenu.Any())
                        {
                            var menu = isElementInMenu.First();
                            var elementToSelect = activeIndex - menu.StartElement;
                            while (!elements[elementToSelect].IsBaseElement &&
                            elements[elementToSelect].Type != ElementType.Interactive)
                            {
                                elementToSelect -= 1;
                            }

                            if (elementToSelect > -1)
                                menu.SelectedElement = elementToSelect;
                            else
                                menu.SelectedElement = 0;

                            changeStateTo = ElementState.Hovered;
                        }
                        else
                        {
                            changeStateTo = ElementState.Hovered;
                        }
                    }

                    index -= 1;
                    active.NextState = changeStateTo;
                }
            }

            if (forceChangeElementIndex > -1 && forceChangeElementIndex < elements.Count - 1)
            {
                elements[forceChangeElementIndex].NextState = forceChangeElementStateTo;
                forceChangeElementIndex = -1;
                forceChangeElementStateTo = ElementState.Idle;
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
            if (afterTransitionComplete && afterTransitionAnimateIndex > -1)
                return;

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

            if (gameContext != null)
            {
                gameContext.Render();
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
                        var shadowImage = Raylib.GenImageColor((int)cellDim.width + (grid.Shadows[i].BlurRadius * 2), (int)cellDim.height + (grid.Shadows[i].BlurRadius * 2), Color.WHITE);
                        Raylib.ImageDrawRectangle(ref shadowImage, grid.Shadows[i].BlurRadius, grid.Shadows[i].BlurRadius,
                            (int)(cellDim.width - grid.Shadows[i].BlurRadius), (int)(cellDim.height - grid.Shadows[i].BlurRadius), grid.Shadows[i].Color);
                        Raylib.ImageBlurGaussian(&shadowImage, grid.Shadows[i].BlurRadius);
                        Application.Instance.ResourceManager.AddResourceImage(name, shadowImage);
                    }
                    else
                    {
                        var shadowTexture = Application.Instance.ResourceManager.GetTexture(name);
                        Raylib.DrawTexture(shadowTexture, (int)grid.Shadows[i].OffsetX + (int)cellDim.x, (int)grid.Shadows[i].OffsetY + (int)cellDim.y, Color.WHITE);
                    }
                }
                
                if (!string.IsNullOrEmpty(grid.BackgroundImages[i]))
                {
                    if (!preloadedAll)
                        Application.Instance.ResourceManager.LoadImage(grid.BackgroundImages[i]);

                    var bgImageTexture = Application.Instance.ResourceManager.GetTexture(grid.BackgroundImages[i]);
                    Raylib.DrawTexturePro(bgImageTexture,
                        new Rectangle(0, 0, bgImageTexture.width, bgImageTexture.height),
                        new Rectangle(cellDim.x + grid.Offsets[i].X, cellDim.y + grid.Offsets[i].Y, cellDim.width, cellDim.height),
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
                        Raylib.DrawRectangleRounded(new Rectangle(cellDim.x, cellDim.y, cellDim.width, cellDim.height),
                            grid.RadiusCorners[i], (int)(grid.RadiusCorners[i] * Math.PI), backgroundColor);
                    }
                    else
                    {
                        if (grid.BackgroundColors[i].Colors == null)
                            goto DRAW_BORDERS;

                        if (grid.BackgroundColors[i].Type == Extras.ColorType.Solid)
                        {
                            Raylib.DrawRectangle((int)cellDim.x, (int)cellDim.y, (int)cellDim.width, (int)cellDim.height, grid.BackgroundColors[i].Colors[0]);
                        }
                        else if (grid.BackgroundColors[i].Type == Extras.ColorType.GradientHorizontal)
                        {
                            Raylib.DrawRectangleGradientH((int)cellDim.x, (int)cellDim.y, (int)cellDim.width, (int)cellDim.height, grid.BackgroundColors[i].Colors[0], grid.BackgroundColors[i].Colors[1]);
                        }
                        else if (grid.BackgroundColors[i].Type == Extras.ColorType.GradientVertical)
                        {
                            Raylib.DrawRectangleGradientV((int)cellDim.x, (int)cellDim.y, (int)cellDim.width, (int)cellDim.height, grid.BackgroundColors[i].Colors[0], grid.BackgroundColors[i].Colors[1]);
                        }

                        DRAW_BORDERS:

                        // draw borders
                        if (grid.Borders[cellItemIndex]) // top
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.x, cellDim.y), new Vector2(cellDim.x + cellDim.width, cellDim.y), grid.BorderThicknesses[cellItemIndex], grid.BorderColors[cellItemIndex]);
                        }

                        if (grid.Borders[cellItemIndex + 1]) // left
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.x, cellDim.y), new Vector2(cellDim.x, cellDim.y + cellDim.height), grid.BorderThicknesses[cellItemIndex + 1], grid.BorderColors[cellItemIndex + 1]);
                        }

                        if (grid.Borders[cellItemIndex + 2]) // right
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.x + cellDim.width, cellDim.y), new Vector2(cellDim.x + cellDim.width, cellDim.y + cellDim.height), grid.BorderThicknesses[cellItemIndex + 2], grid.BorderColors[cellItemIndex + 2]);
                        }

                        if (grid.Borders[cellItemIndex + 3]) // bottom
                        {
                            Raylib.DrawLineEx(new Vector2(cellDim.x, cellDim.y + cellDim.height), new Vector2(cellDim.x + cellDim.width, cellDim.y + cellDim.height), grid.BorderThicknesses[cellItemIndex + 3], grid.BorderColors[cellItemIndex + 3]);
                        }
                    }
                }
            }
        }

    }
}
