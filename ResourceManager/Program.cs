using TwinspireCS;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

using System.IO;

namespace ResourceManager
{
    internal class Program
    {
        static unsafe void Main(string[] args)
        {
            var app = new Application("StoryDev Resource Manager", 1280, 720);
            app.UseImGui = true;
            app.TargetFPS = 120;

            var corePackage = app.ResourceManager.CreatePackage("core.dat");
            app.ResourceManager.AssetDirectory = "Assets";
            app.ResourceManager.AddResource(corePackage, "Font_Regular", "Assets/Raw/Fonts/TiltNeon-Regular.ttf");
            if (!File.Exists("Assets/core.dat"))
            {
                app.ResourceManager.WriteAll(corePackage);
            }
            else
            {
                app.ResourceManager.ReadHeaders("core.dat");
            }

            app.InitImGui();

            sbyte* regularFontExt = null;
            int regularFontSize = 0;
            var regularFontData = app.ResourceManager.GetBytesFromMemory("Font_Regular", ref regularFontExt, ref regularFontSize);
            app.ImGuiAddFont(regularFontData, 24);

            app.InitAll();

            ResourceGroup core = new ResourceGroup();
            core.RequestedFonts.Add("Font_Regular:40");
            app.ResourceManager.LoadGroup(core);

            var imgui = app.GetImGuiController();
            imgui.Init();


            while (app.IsOpen())
            {
                imgui.NewFrame();
                imgui.ProcessEvent();
                ImGui.NewFrame();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.GRAY);

                

                ImGui.Render();
                imgui.Render(ImGui.GetDrawData());
                Raylib.EndDrawing();
            }

            imgui.Shutdown();
            app.ResourceManager.UnloadAll();
        }
    }
}