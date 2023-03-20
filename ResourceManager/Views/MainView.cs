using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

using ImGuiNET;
using TwinspireCS;

namespace ResourceManager.Views
{
    internal class MainView
    {

        static Application resourceApp;
        static string projectPath;
        static bool openingProject;

        public static void Init()
        {
            projectPath = string.Empty;
        }

        public static void Render()
        {
            //
            // Project Open
            //

            ImGui.SetNextWindowSize(new Vector2(400, 350), ImGuiCond.Appearing);
            if (ImGui.Begin("Open Project", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.BeginTabBar("OpenProjectMethods");

                if (ImGui.BeginTabItem("Folder"))
                {


                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabBar("Server"))
                {


                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();

                ImGui.End();
            }

            //
            // Navigation (Top-Left)
            //

            var navigationWidth = 250;
            ImGui.SetNextWindowSize(new Vector2(navigationWidth, 90), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new Vector2(5, 5), ImGuiCond.Always);
            if (ImGui.Begin("##Views", ImGuiWindowFlags.NoDecoration))
            {
                

                ImGui.End();
            }

            //
            // Resource Files (Top-Middle)
            //

            

            //
            // Options (Top-Right)
            //



            //
            // Quick Search (Middle-Left)
            //

            
            
            //
            // File List as Table (Middle)
            //



            //
            // File Properties / Error Handling (Middle-Right)
            //

        }

    }
}
