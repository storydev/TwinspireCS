using TwinspireCS;
using System.IO;

namespace ResourceManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new Application("StoryDev Resource Manager", 1280, 720);
            app.InitAll();

            var corePackage = app.ResourceManager.CreatePackage("core.dat");
            app.ResourceManager.AddResource(corePackage, "Font_Regular", "Assets/Raw/Fonts/TiltNeon-Regular.ttf");
            if (!File.Exists("Assets/core.dat"))
            {
                app.ResourceManager.WriteAll(corePackage, "Assets");
            }
            else
            {
                app.ResourceManager.ReadHeaders("Assets", "core.dat");
            }

            ResourceGroup core = new ResourceGroup();
            core.RequestedFonts.Add("Font_Regular");

            ResourceResults coreResults = app.ResourceManager.LoadGroup(core);

            while (app.IsOpen())
            {


            }

            app.ResourceManager.UnloadResources(coreResults);
        }
    }
}