using TwinspireCS;

namespace ResourceManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new Application("StoryDev Resource Manager", 1280, 720);
            app.InitAll();

            while (app.IsOpen())
            {


            }
        }
    }
}