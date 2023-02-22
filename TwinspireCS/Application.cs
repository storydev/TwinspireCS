using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public class Application
    {

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
        }

        /// <summary>
        /// Initialise all options for Raylib.
        /// </summary>
        public void InitAll()
        {
            Raylib.InitWindow(startingWidth, startingHeight, windowTitle);
            Raylib.SetTargetFPS(TargetFPS);
            Raylib.InitAudioDevice();


        }



    }
}
