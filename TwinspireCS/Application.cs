using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using TwinspireCS.Engine.GUI;
using TwinspireCS.Engine.Extras;
using TwinspireCS.Engine.Input;
using TwinspireCS.Engine;

namespace TwinspireCS
{
    public class Application
    {

        private ImGuiController imguiController;

        private ResourceManager resourceManager;
        /// <summary>
        /// Gets the Resource Manager for this application.
        /// </summary>
        public ResourceManager ResourceManager
        {
            get => resourceManager;
        }

        private int startingWidth;
        private int startingHeight;

        /// <summary>
        /// Get or set the width of the window. Getting or setting this value will
        /// retrieve the client width, not including borders.
        /// </summary>
        public int Width
        {
            get => Raylib.GetRenderWidth();
            set => Raylib.SetWindowSize(value, Height);
        }

        /// <summary>
        /// Get or set the height of the window. Getting or setting this value will
        /// retrieve the client height, not including borders.
        /// </summary>
        public int Height
        {
            get => Raylib.GetRenderHeight();
            set => Raylib.SetWindowSize(Width, value);
        }


        private string windowTitle;
        /// <summary>
        /// Get or set the title of the window.
        /// </summary>
        public string Title
        {
            get => windowTitle;
            set
            {
                Raylib.SetWindowTitle(value);
                windowTitle = value;
            }
        }

        private bool useImgui;
        /// <summary>
        /// Determines if the ImGui editor should be created
        /// and rendered.
        /// </summary>
        public bool UseImGui
        {
            get => useImgui;
            set => useImgui = value;
        }

        /// <summary>
        /// Set the maximum frames per second that this application should render at.
        /// </summary>
        public int TargetFPS { get; set; }

        /// <summary>
        /// Set the window's icon. Will not change after InitAll has been called.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Gets or sets the state of the game, as a 
        /// </summary>
        public IGameState GameState { get; set; }

        /// <summary>
        /// Initialise a new application.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        public Application(string title, int width, int height)
        {
            windowTitle = title;
            startingWidth = width;
            startingHeight = height;
            resourceManager = new ResourceManager();
        }

        /// <summary>
        /// Initialise a new application with only resource management.
        /// </summary>
        public Application()
        {
            resourceManager = new ResourceManager();
        }

        /// <summary>
        /// Initialise ImGui context.
        /// </summary>
        public void InitImGui()
        {
            if (useImgui)
            {
                var context = ImGui.CreateContext();
                ImGui.SetCurrentContext(context);

                imguiController = new ImGuiController();
            }
        }

        /// <summary>
        /// Get the instance of the imgui controller.
        /// </summary>
        /// <returns></returns>
        public ImGuiController GetImGuiController()
        {
            return imguiController;
        }

        /// <summary>
        /// Initialise all options for Raylib.
        /// </summary>
        public void InitAll()
        {
            InitDefaultUI();
            HotKeys.Init();
            Animate.Init();

            Raylib.InitWindow(startingWidth, startingHeight, windowTitle);
            if (!string.IsNullOrEmpty(IconPath))
            {
                Raylib.SetWindowIcon(Raylib.LoadImage(IconPath));
            }

            Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_MSAA_4X_HINT);
            Raylib.SetTargetFPS(TargetFPS);
        }

        /// <summary>
        /// Initialise the Raylib audio device.
        /// </summary>
        public void InitAudio()
        {
            Raylib.InitAudioDevice();
        }

        private void InitDefaultUI()
        {
            UI.Init();

            UI.InterfaceBuilder.AddComponent("Button", new Component(2)
            {
                Elements = new ComponentElement[]
                {
                    new ComponentElement()
                    {
                        Type = ElementType.Interactive
                    },
                    new ComponentElement()
                    {
                        HorizontalMeasureType = MeasureType.Pixels,
                        VerticalMeasureType = MeasureType.Pixels                        
                    }
                },
                ElementInterpetedTypes = new InterpretedType[]
                {
                    InterpretedType.Shape,
                    InterpretedType.Text
                }
            });

            UI.InterfaceBuilder.AddComponent("Label", new Component(1)
            {
                Elements = new ComponentElement[]
                {
                    new ComponentElement()
                    {
                        HorizontalMeasureType = MeasureType.Pixels,
                        VerticalMeasureType = MeasureType.Pixels
                    }
                },
                ElementInterpetedTypes = new InterpretedType[]
                {
                    InterpretedType.Text
                }
            });

            Theme.Default.Styles.Add(Theme.BUTTON, new Style()
                .SetBackground(new ColorMethod(ColorType.GradientVertical, new Color[] { new Color(220, 220, 220, 255), new Color(185, 185, 185, 255) }))
                .SetBorderBottomColors(new Color(155, 155, 155, 255))
                .SetBorderBottomThickness(1)
                .ApplyBordersBottom(true));

            Theme.Default.Styles.Add(Theme.BUTTON_HOVER, new Style()
                .SetBackground(new ColorMethod(ColorType.GradientVertical, new Color[] { new Color(240, 240, 240, 255), new Color(205, 205, 205, 255) }))
                .SetBorderBottomColors(new Color(190, 190, 190, 255))
                .SetBorderBottomThickness(1)
                .ApplyBordersBottom(true));

            Theme.Default.Styles.Add(Theme.BUTTON_DOWN, new Style()
                .SetBackground(new ColorMethod(ColorType.GradientVertical, new Color[] { new Color(155, 155, 155, 255), new Color(170, 170, 170, 255) }))
                .SetBorderBottomColors(new Color(140, 140, 140, 255))
                .SetBorderBottomThickness(1)
                .ApplyBordersBottom(true));

            Theme.Default.Styles.Add(Theme.DISABLED, new Style()
                .SetBackground(new ColorMethod(ColorType.Solid, new Color[] { new Color(230, 230, 230, 127) }))
                .SetBorderColors(new Color(127, 127, 127, 127))
                .SetBorderThickness(2)
                .ApplyBorders(true));
                
        }

        /// <summary>
        /// Add a font to the imgui context from a path.
        /// </summary>
        /// <param name="fontPath">The path to the font file.</param>
        /// <param name="fontSize">The size of the font.</param>
        public void ImGuiAddFont(string fontPath, int fontSize)
        {
            imguiController.AddFontFile(fontPath, fontSize);
        }

        /// <summary>
        /// Add a font to the imgui context from a buffer.
        /// </summary>
        /// <param name="data">The data of the font.</param>
        /// <param name="fontSize">The size the font should be rendered at.</param>
        public void ImGuiAddFont(byte[] data, int fontSize)
        {
            imguiController.AddFontBytes(data, fontSize);
        }

        /// <summary>
        /// Gets a value to determine if the window remains open.
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return !Raylib.WindowShouldClose();
        }

        /// <summary>
        /// Close the application and free all resources.
        /// </summary>
        public void Close()
        {
            resourceManager.UnloadAll();
            if (Raylib.IsAudioDeviceReady())
            {
                Raylib.CloseAudioDevice();
            }

            Raylib.CloseWindow();
        }

        private static Application? instance;
        public static Application Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Application();
                }

                return instance;
            }
        }

        public static Application CreateApp(string title, int width, int height)
        {
            instance = new Application(title, width, height);
            return instance;
        }

    }
}
