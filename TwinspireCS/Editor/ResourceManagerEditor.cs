using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static void Init()
        {
            maxItems = 100;
            page = 0;
            resources = new List<ResourceFile>();
            tableCells = Array.Empty<string>();

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

            ResetCellData();
        }

        public static void Render()
        {
            ImGui.SetNextWindowSize(new Vector2(1100, 500), ImGuiCond.Always);

            if (ImGui.Begin("Resource Manager", ref isOpen))
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

                ImGui.End();
            }
        }

        static void ResetCellData()
        {
            var min = page * maxItems;
            var max = min + maxItems;
            int total = resources.Count - 1;

            if (resources.Count > max)
                total = max;

            var totalCells = 6 * total;
            tableCells = new string[totalCells];

            var rm = Application.Instance.ResourceManager;
            
            var cellIndex = 0;
            for (int i = min; min < max; i++)
            {
                if (i >= resources.Count - 1)
                    break;

                var resource = resources[i];
                var package = rm.Packages.ElementAt(resource.PackageIndex);
                tableCells[cellIndex++] = resource.Name;
                tableCells[cellIndex++] = package.SourceFilePath;
                tableCells[cellIndex++] = resource.Segment.OriginalSourceFile;
                tableCells[cellIndex++] = resource.Segment.FileExt;
                tableCells[cellIndex++] = ConvertBytesToString(resource.Segment.Size);
                tableCells[cellIndex++] = ConvertBytesToString(resource.Segment.CompressedSize);
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
