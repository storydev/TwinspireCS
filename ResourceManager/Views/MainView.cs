using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

using Newtonsoft.Json;
using ImGuiNET;
using Raylib_cs;
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

        static string[] resourceFiles;
        static int selectedResource;
        static string resourceNewFile;

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

            resourceNewFile = string.Empty;
            selectedResource = -1;
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

            var topHeight = 90f;
            var sideWidth = 250;
            ImGui.SetNextWindowSize(new Vector2(sideWidth, topHeight), ImGuiCond.Always);
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
            var middleWidth = Raylib.GetRenderWidth() - (sideWidth * 2) - 20f;
            ImGui.SetNextWindowPos(new Vector2(sideWidth + 10f, 5f), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(middleWidth, topHeight), ImGuiCond.Always);
            if (ImGui.Begin("##ResourceFiles", ImGuiWindowFlags.NoDecoration))
            {
                for (int i = 0; i < resourceFiles.Length; i++)
                {
                    if (i > 0)
                    {
                        ImGui.SameLine();
                    }
                    
                    if (ImGui.RadioButton(resourceFiles[i], selectedResource == i))
                    {
                        selectedResource = i;
                    }
                }

                ImGui.Text("New File:"); ImGui.SameLine();
                if (ImGui.InputText("##InputNewFile", ref resourceNewFile, 64, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    var fileName = Path.Combine(currentProject.FolderPath, resourceNewFile);
                    if (!fileName.EndsWith(".dat"))
                        fileName += ".dat";

                    var versionsName = fileName.Substring(0, fileName.Length - 4);
                    versionsName += "-versions.dat";

                    if (!File.Exists(fileName))
                    {
                        File.WriteAllText(fileName, "");
                        File.WriteAllText(versionsName, "");
                        PopulateResourceFiles();
                    }
                }

                ImGui.End();
            }


            //
            // Options (Top-Right)
            //
            var rightX = Raylib.GetRenderWidth() - sideWidth - 5f;
            ImGui.SetNextWindowPos(new Vector2(rightX, 5f), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(sideWidth, topHeight), ImGuiCond.Always);
            if (ImGui.Begin("##Options", ImGuiWindowFlags.NoDecoration))
            {


                ImGui.End();
            }

            //
            // Filters (Middle-Left)
            //
            var middleHeight = Raylib.GetRenderHeight() - topHeight;
            ImGui.SetNextWindowPos(new Vector2(5f, topHeight + 10f), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(sideWidth, middleHeight), ImGuiCond.Always);
            if (ImGui.Begin("##Filters", ImGuiWindowFlags.NoDecoration))
            {

                ImGui.End();
            }

            //
            // File List as Table (Middle)
            //
            ImGui.SetNextWindowPos(new Vector2(sideWidth + 10f, topHeight + 10f), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(middleWidth, middleHeight), ImGuiCond.Always);
            if (ImGui.Begin("##FileList", ImGuiWindowFlags.NoDecoration))
            {

                ImGui.End();
            }


            //
            // File Properties / Error Handling (Middle-Right)
            //
            ImGui.SetNextWindowPos(new Vector2(rightX, topHeight + 10f), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(sideWidth, middleHeight), ImGuiCond.Always);
            if (ImGui.Begin("##Properties", ImGuiWindowFlags.NoDecoration))
            {
                ImGui.BeginTabBar("PropertyItems");

                if (ImGui.BeginTabItem("File Properties"))
                {

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Errors"))
                {

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();

                ImGui.End();
            }
        }

        #region Project Utils

        static bool OpenProject()
        {
            if (string.IsNullOrEmpty(currentProject.Name))
                return false;

            if (currentProject.OpenMethod == OpenMethod.LocalFolder)
            {
                PopulateResourceFiles();
                return Directory.Exists(currentProject.FolderPath);
            }

            return false;
        }

        static void PopulateResourceFiles()
        {
            if (currentProject.OpenMethod == OpenMethod.LocalFolder)
            {
                resourceFiles = Directory.GetFiles(currentProject.FolderPath, "*.dat");
            }
        }

        #endregion

    }
}
