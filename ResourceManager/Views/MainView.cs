using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

using ImGuiNET;
using TwinspireCS;
using ResourceManager.Data;

namespace ResourceManager.Views
{
    internal class MainView
    {

        static Application resourceApp;
        static bool projectOpened;
        static Project currentProject;
        static int selectedOpenTab;

        public static void Init()
        {
            currentProject = new Project();
        }

        public static void Render()
        {
            //
            // Project Open
            //

            ImGui.SetNextWindowSize(new Vector2(400, 350), ImGuiCond.Appearing);
            if (ImGui.Begin("Open Project", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.Text("Name:"); ImGui.SameLine();
                ImGui.SetNextItemWidth(120f);
                ImGui.InputText("##OpenProjectName", ref currentProject.Name, 256);

                ImGui.SameLine();
                if (ImGui.Button("Save"))
                {
                    
                }

                ImGui.BeginTabBar("OpenProjectMethods");

                if (ImGui.BeginTabItem("Folder"))
                {
                    selectedOpenTab = 0;

                    ImGui.Text("Folder Path:"); ImGui.SameLine();
                    ImGui.InputText("##OpenFolderPath", ref currentProject.FolderPath, 512);



                    ImGui.EndTabItem();
                }



                if (ImGui.BeginTabItem("Server"))
                {
                    selectedOpenTab = 1;

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
