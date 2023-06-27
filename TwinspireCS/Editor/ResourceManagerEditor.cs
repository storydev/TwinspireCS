﻿using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace TwinspireCS.Editor
{
    public class ResourceManagerEditor
    {

        static bool isOpen;
        static int maxItems;
        static int page;
        static List<ResourceFile> resources;
        static string[] tableCells;
        static string[] packageNames;
        static int selectedPackage;
        static int lastSelectedPackage;
        static bool allowBack;
        static bool allowForward;


        static string resourcePackageName;

        public static void Init()
        {
            maxItems = 100;
            page = 0;
            resources = new List<ResourceFile>();
            tableCells = Array.Empty<string>();
            resourcePackageName = string.Empty;

            var rm = Application.Instance.ResourceManager;
            for (int i = 0; i < rm.Packages.Count(); i++)
            {
                var package = rm.Packages.ElementAt(i);
                foreach (var kv in package.FileMapping)
                {
                    resources.Add(new ResourceFile()
                    {
                        Name = kv.Key,
                        PackageIndex = i,
                        Segment = kv.Value
                    });
                }
            }

            ResetPackageNames();
            ResetCellData();
        }

        public static void Render()
        {
            ImGui.SetNextWindowSize(new Vector2(1100, 500), ImGuiCond.Always);

            if (ImGui.Begin("Resource Manager", ref isOpen))
            {
                ImGui.Text("Package:"); ImGui.SameLine();
                ImGui.SetNextItemWidth(200f);
                ImGui.Combo("##PackageList", ref selectedPackage, packageNames, packageNames.Length); ImGui.SameLine();
                if (lastSelectedPackage != selectedPackage)
                {
                    ResetCellData();
                    lastSelectedPackage = selectedPackage;
                }

                if (ImGui.Button("+##AddResourcePackage"))
                {
                    ImGui.OpenPopup("AddResourcePackagePopup");
                }

                if (ImGui.BeginPopup("AddResourcePackagePopup"))
                {
                    ImGui.Text("Name:"); ImGui.SameLine();
                    ImGui.InputText("##Name", ref resourcePackageName, 64);


                    if (ImGui.Button("Confirm##ConfirmAddResourcePackage"))
                    {
                        if (!resourcePackageName.EndsWith(".dat"))
                            resourcePackageName += ".dat";

                        var validPath = Utils.ValidateFilePath(Path.Combine(Application.Instance.ResourceManager.AssetDirectory, resourcePackageName), true, true);
                        if (validPath && !File.Exists(resourcePackageName))
                        {
                            Application.Instance.ResourceManager.CreatePackage(resourcePackageName);
                            selectedPackage = 0;
                            page = 0;
                            ResetPackageNames();
                            ImGui.CloseCurrentPopup();
                        }
                    }

                    ImGui.EndPopup();
                }

                ImGui.SameLine();

                if (!allowBack)
                    ImGui.BeginDisabled();

                if (ImGui.Button("<##PreviousRMPage"))
                {
                    page -= 1;
                    ResetCellData();
                }

                if (!allowBack)
                    ImGui.EndDisabled();

                ImGui.SameLine();

                if (!allowForward)
                    ImGui.BeginDisabled();

                if (ImGui.Button(">##NextRMPage"))
                {
                    page += 1;
                    ResetCellData();
                }

                if (!allowForward)
                    ImGui.EndDisabled();


                ImGui.Separator();

                if (ImGui.BeginTabBar("ResourceResults"))
                {
                    if (ImGui.BeginTabItem("Resources"))
                    {
                        ImGui.BeginTable("ResourceManagerFiles", 6);
                        ImGui.TableSetupColumn("Identifier", ImGuiTableColumnFlags.None, 250f);
                        ImGui.TableSetupColumn("Package", ImGuiTableColumnFlags.None, 150f);
                        ImGui.TableSetupColumn("Original Source File", ImGuiTableColumnFlags.DefaultHide, 300f);
                        ImGui.TableSetupColumn("File Ext", ImGuiTableColumnFlags.None, 90f);
                        ImGui.TableSetupColumn("Size", ImGuiTableColumnFlags.None, 125f);
                        ImGui.TableSetupColumn("Compressed Size", ImGuiTableColumnFlags.None, 125f);
                        ImGui.TableSetupScrollFreeze(0, 1);
                        ImGui.TableHeadersRow();

                        for (int i = 0; i < tableCells.Length; i++)
                        {
                            ImGui.TableNextColumn();
                            ImGui.Text(tableCells[i]);
                        }

                        ImGui.EndTable();

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Add Resources"))
                    {


                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

                ImGui.End();
            }
        }

        static void ResetPackageNames()
        {
            var rm = Application.Instance.ResourceManager;
            packageNames = new string[rm.Packages.Count() + 1];
            for (int i = 0; i < packageNames.Length; i++)
            {
                if (i == 0)
                {
                    packageNames[i] = "All";
                }
                else
                {
                    packageNames[i] = rm.Packages.ElementAt(i - 1).SourceFilePath;
                }
            }
        }

        static void ResetCellData()
        {
            var min = page * maxItems;
            var max = min + maxItems;
            int total = resources.Count - 1;

            allowBack = min > 0;
            allowForward = resources.Count > max;
            if (allowForward)
                total = max;

            var totalCells = 6 * total;
            tableCells = new string[totalCells];

            var rm = Application.Instance.ResourceManager;
            
            var cellIndex = 0;
            int index = min;
            while (index < max)
            {
                if (index >= resources.Count - 1)
                    break;

                var resource = resources[index];
                var filteredPackage = -1;
                if (selectedPackage > 0)
                {
                    filteredPackage = selectedPackage - 1;
                }

                if (filteredPackage > -1 && resource.PackageIndex != filteredPackage)
                {
                    continue;
                }

                var package = rm.Packages.ElementAt(resource.PackageIndex);
                tableCells[cellIndex++] = resource.Name;
                tableCells[cellIndex++] = package.SourceFilePath;
                tableCells[cellIndex++] = resource.Segment.OriginalSourceFile;
                tableCells[cellIndex++] = resource.Segment.FileExt;
                tableCells[cellIndex++] = ConvertBytesToString(resource.Segment.Size);
                tableCells[cellIndex++] = ConvertBytesToString(resource.Segment.CompressedSize);

                index += 1;
            }


        }

        static string ConvertBytesToString(long bytes)
        {
            var result = string.Empty;
            var temp = "" + bytes;
            if (bytes > 1024 * 1024 * 1024)
            {
                var value = bytes / (1024 * 1024 * 1024);
                result = string.Format("{0:.##}", value) + " GB";
            }
            else if (bytes > 1024 * 1024)
            {
                var value = bytes / (1024 * 1024);
                result = string.Format("{0:.##}", value) + " MB";
            }
            else if (bytes > 1024)
            {
                var value = bytes / 1024;
                result = string.Format("{0:.##}", value) + " KB";
            }
            else
            {
                result = string.Format("{0:.##}", bytes) + " bytes";
            }

            return result;
        }

    }
}
