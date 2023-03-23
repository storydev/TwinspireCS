using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

using Newtonsoft.Json;
using ImGuiNET;
using TwinspireCS;
using ResourceManager.Data;

namespace ResourceManager.Views
{
    internal class MainView
    {

        static List<Project> recentProjects;
        static string appCachePath;
        static string recentProjectsPath;

        static Application resourceApp;
        static bool projectOpened;
        static Project currentProject;
        static int selectedOpenTab;

        static bool showErrorOpenFolder;
        static string errorOpenFolderText;

        public static void Init()
        {
            currentProject = new Project();

            appCachePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appCachePath = Path.Combine(appCachePath, "StoryDev", "Resource Manager");
            Directory.CreateDirectory(appCachePath);

            recentProjectsPath = Path.Combine(appCachePath, "recents.json");
            if (File.Exists(recentProjectsPath))
            {
                recentProjects = JsonConvert.DeserializeObject<List<Project>>(File.ReadAllText(recentProjectsPath));
            }
            else
            {
                recentProjects = new List<Project>();
            }
        }

        public static void Render()
        {
            //
            #region Project Open
            //

            if (projectOpened)
                goto NEED_MAINVIEW;

            ImGui.SetNextWindowSize(new Vector2(400, 350), ImGuiCond.Appearing);
            if (ImGui.Begin("Open Project", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.Text("Name:"); ImGui.SameLine();
                ImGui.SetNextItemWidth(120f);
                ImGui.InputText("##OpenProjectName", ref currentProject.Name, 256);

                ImGui.SameLine();
                if (ImGui.Button("Save"))
                {
                    if (currentProject.OpenMethod == OpenMethod.LocalFolder)
                    {
                        if (!Directory.Exists(currentProject.FolderPath))
                        {
                            errorOpenFolderText = "The given folder path is not valid.";
                            showErrorOpenFolder = true;
                        }
                        else
                        {
                            if (OpenProject())
                            {
                                projectOpened = true;
                                resourceApp = new Application();
                                resourceApp.ResourceManager.AssetDirectory = currentProject.FolderPath;

                                recentProjects.Add(currentProject);
                                File.WriteAllText(recentProjectsPath, JsonConvert.SerializeObject(recentProjects));
                            }
                        }
                    }
                    else if (currentProject.OpenMethod == OpenMethod.Server)
                    {
                        errorOpenFolderText = "Server functionality has not been fully implemented.";
                        showErrorOpenFolder = true;
                    }
                }

                if (showErrorOpenFolder)
                {
                    ImGui.TextColored(new Vector4(1f, 0, 0, 1f), errorOpenFolderText);
                }

                ImGui.BeginTabBar("OpenProjectMethods");

                if (ImGui.BeginTabItem("Folder"))
                {
                    currentProject.OpenMethod = OpenMethod.LocalFolder;

                    ImGui.Text("Folder Path:"); ImGui.SameLine();
                    ImGui.InputText("##OpenFolderPath", ref currentProject.FolderPath, 512);

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Server"))
                {
                    currentProject.OpenMethod = OpenMethod.Server;

                    ImGui.Text("Host:"); ImGui.SameLine();
                    ImGui.InputText("##OpenHost", ref currentProject.Host, 512);

                    ImGui.Text("Port:"); ImGui.SameLine();
                    int port = currentProject.Port;
                    ImGui.InputInt("##OpenPort", ref port, 1, 10);
                    if (port > ushort.MaxValue)
                        port = ushort.MaxValue;
                    if (port < 0)
                        port = 0;

                    currentProject.Port = (ushort)port;

                    ImGui.Separator();

                    ImGui.Text("FTP Host:"); ImGui.SameLine();
                    ImGui.InputText("##OpenFTPHost", ref currentProject.FTPHost, 512);

                    ImGui.Text("FTP User:"); ImGui.SameLine();
                    ImGui.InputText("##OpenFTPUser", ref currentProject.FTPUser, 128);

                    ImGui.Text("FTP Password:"); ImGui.SameLine();
                    ImGui.InputText("##OpenFTPPassword", ref currentProject.FTPPassword, 128, ImGuiInputTextFlags.Password);

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Recent Projects"))
                {
                    ImGui.BeginChild("RecentProjectList", new Vector2(380, 270), true);

                    foreach (var recent in recentProjects)
                    {
                        ImGui.Text(recent.Name);
                        ImGui.Indent();
                        ImGui.TextColored(new Vector4(.4f, .4f, .4f, 1f), recent.OpenMethod == OpenMethod.LocalFolder 
                            ? "Local Folder" : "Server");
                        ImGui.Unindent();
                        if (ImGui.Button("Open##Open" + recent.Name))
                        {
                            currentProject = recent;
                            if (OpenProject())
                            {
                                projectOpened = true;
                                resourceApp = new Application();
                                resourceApp.ResourceManager.AssetDirectory = currentProject.FolderPath;
                            }
                        }
                    }

                    ImGui.EndChild();

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();

                ImGui.End();
            }

            if (resourceApp == null || !projectOpened)
                return;

            NEED_MAINVIEW:

            #endregion

            //
            // Navigation (Top-Left)
            //

            var navigationWidth = 250;
            ImGui.SetNextWindowSize(new Vector2(navigationWidth, 90), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new Vector2(5, 5), ImGuiCond.Always);
            if (ImGui.Begin("##Views", ImGuiWindowFlags.NoDecoration))
            {
                if (ImGui.Button("Build All"))
                {

                }
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Build and compress all resource files.");

                ImGui.SameLine();
                if (ImGui.Button("Close Project"))
                {
                    projectOpened = false;
                    currentProject = new Project();
                }
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Close the current project.");

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

        #region Project Utils

        static bool OpenProject()
        {
            if (string.IsNullOrEmpty(currentProject.Name))
                return false;

            if (currentProject.OpenMethod == OpenMethod.LocalFolder)
            {
                return Directory.Exists(currentProject.FolderPath);
            }

            return false;
        }

        #endregion

    }
}
