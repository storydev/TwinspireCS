using TwinspireCS;
using Raylib_cs;
using System.IO;

namespace ResourceManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new Application("StoryDev Resource Manager", 1280, 720);
            app.UseImGui = true;

            var corePackage = app.ResourceManager.CreatePackage("core.dat");
            app.ResourceManager.AssetDirectory = "Assets";
            app.ResourceManager.AddResource(corePackage, "Font_Regular", "Assets/Raw/Fonts/TiltNeon-Regular.ttf");
            if (!File.Exists("Assets/core.dat"))
            {
                app.ResourceManager.WriteAll(corePackage);
            }
            else
            {
                app.ResourceManager.ReadHeaders("Assets", "core.dat");
            }

            ResourceGroup core = new ResourceGroup();
            core.RequestedFonts.Add("Font_Regular:40");

            app.ResourceManager.LoadGroup(core);

            app.InitAll();

            while (app.IsOpen())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.GRAY);


                Raylib.EndDrawing();
            }

            app.ResourceManager.UnloadAll();
        }
    }
}