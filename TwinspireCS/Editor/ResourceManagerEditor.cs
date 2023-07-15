using Raylib_cs;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using TwinspireCS.Engine;
using System.Reflection;

namespace TwinspireCS.Editor
{
    public class ResourceManagerEditor : IExtension
    {

        bool isOpen;
        bool isWriting;
        int maxItems;
        int page;
        List<ResourceFile> resources;
        string[] tableCells;
        string[] packageNames;
        int selectedPackage;
        int lastSelectedPackage;
        List<int> selectedRows;
        List<ResourceAddFile> addFiles;
        int writingAllProgressIndex;

        bool allowBack;
        bool allowForward;

        string[] resourceWriteErrorPaths;
        bool resourceWriteHasError;

        string resourcePackageName;

        public void Init()
        {
            maxItems = 100;
            page = 0;
            resources = new List<ResourceFile>();
            tableCells = Array.Empty<string>();
            resourcePackageName = string.Empty;
            resourceWriteErrorPaths = Array.Empty<string>();
            selectedRows = new List<int>();
            addFiles = new List<ResourceAddFile>();
            isWriting = false;
            writingAllProgressIndex = Animate.Create();

            RefreshResources();

            ResetPackageNames();
            ResetCellData();
        }

        void RefreshResources()
        {
            resources.Clear();

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
        }

        public void Render()
        {
            ImGui.SetNextWindowSize(new Vector2(1100, 500), ImGuiCond.Always);

            if (ImGui.Begin("Resource Manager", ref isOpen))
            {
                if (isWriting)
                    ImGui.BeginDisabled();

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
                    ImGui.InputText("##ResourcePackageName", ref resourcePackageName, 64);

                    if (ImGui.Button("Confirm##ConfirmAddResourcePackage"))
                    {
                        if (!resourcePackageName.EndsWith(".dat"))
                            resourcePackageName += ".dat";

                        var validPath = Utils.ValidateFilePath(Path.Combine(Application.Instance.ResourceManager.AssetDirectory, resourcePackageName), true, true, true);
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
                    var offsetY = ImGui.GetCursorPosY();

                    if (ImGui.BeginTabItem("Resources"))
                    {
                        var width = ImGui.GetWindowWidth() - (ImGui.GetStyle().WindowPadding.X * 2);
                        var height = ImGui.GetWindowHeight() - offsetY - (ImGui.GetStyle().WindowPadding.Y * 2) - ImGui.GetTextLineHeight();

                        //ImGui.BeginChild("BrowseResources", new Vector2(width, height), false);

                        // For some inexplicable reason, the flags cause a memory corruption error.
                        // Until fixed, scrolling will have to be done within the window.
                        //
                        //var tableFlags = ImGuiTableFlags.NoHostExtendX |
                          //  ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.RowBg |
                          //  ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY;

                        ImGui.BeginTable("ResourceManagerFiles", 6, ImGuiTableFlags.Borders, new Vector2(width - 5, height - 5));
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
                            int row = (int)Math.Floor((float)(i / 6));
                            var isSelected = selectedRows.IndexOf(row) > -1;

                            if (i % 6 == 0)
                            {
                                if (ImGui.Selectable(tableCells[i], isSelected, ImGuiSelectableFlags.SpanAllColumns))
                                {
                                    if (ImGui.GetIO().KeyCtrl)
                                    {
                                        if (!isSelected)
                                            selectedRows.Add(row);
                                        else
                                            selectedRows.Remove(row);
                                    }
                                    else if (ImGui.GetIO().KeyShift)
                                    {
                                        if (selectedRows.Count > 0)
                                        {
                                            var last = selectedRows[^1];
                                            if (row < last)
                                            {
                                                var temp = last - 1;
                                                while (temp >= row)
                                                {
                                                    if (!selectedRows.Contains(temp))
                                                        selectedRows.Add(temp);
                                                    temp--;
                                                }
                                            }
                                            else if (row > last)
                                            {
                                                var temp = last + 1;
                                                while (temp <= row)
                                                {
                                                    if (!selectedRows.Contains(temp))
                                                        selectedRows.Add(temp);

                                                    temp++;
                                                }
                                            }
                                        }
                                        else if (!isSelected)
                                        {
                                            selectedRows.Add(row);
                                        }
                                    }
                                    else
                                    {
                                        selectedRows.Clear();
                                        selectedRows.Add(row);
                                    }
                                }
                            }
                            else
                            {
                                ImGui.Text(tableCells[i]);
                            }
                        }

                        ImGui.EndTable();
                        //ImGui.EndChild();

                        if (selectedRows.Count == 0)
                        {
                            ImGui.BeginDisabled();
                        }

                        if (ImGui.Button("Edit##BrowseEditResources"))
                        {
                            if (selectedRows.Count > 1)
                                ImGui.OpenPopup("Edit Resources");
                            else if (selectedRows.Count == 1)
                            {
                                var selected = selectedRows[0];
                                editItemName = tableCells[selected * 6];
                                ImGui.OpenPopup("Edit Item");
                            }

                        } ImGui.SameLine();

                        if (ImGui.Button("Delete##BrowseDeleteResources"))
                        {
                            var rm = Application.Instance.ResourceManager;
                            var canDelete = true;
                            foreach (var row in selectedRows)
                            {
                                var itemName = tableCells[row * 6];
                                if (rm.DoesIdentifierExist(itemName))
                                {
                                    canDelete = false;
                                    break;
                                }
                            }

                            if (canDelete)
                                ImGui.OpenPopup("Confirm Delete Items");
                            else
                                ImGui.OpenPopup("Delete Error");

                        } ImGui.SameLine();

                        if (selectedRows.Count == 0)
                        {
                            ImGui.EndDisabled();
                        }

                        //if (selectedRows.Count != 1)
                        //{
                        //    ImGui.BeginDisabled();
                        //}

                        //if (ImGui.Button("Preview##BrowsePreviewSelectedResource"))
                        //{

                        //} ImGui.SameLine();

                        //if (selectedRows.Count != 1)
                        //{
                        //    ImGui.EndDisabled();
                        //}

                        DrawEditPopup();
                        DrawSingleEditPopup();
                        DrawConfirmDeleteSelectionPopup();
                        DrawDeleteErrorPopup();

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Add Resources"))
                    {
                        if (selectedPackage - 1 < 0)
                        {
                            ImGui.Text("Please select a package to add resources to.");
                            // don't let dropped files go through to the next frame
                            if (Raylib.IsFileDropped())
                            {
                                Raylib.GetDroppedFiles();
                            }
                        }
                        else
                        {
                            var packageIndex = selectedPackage - 1;
                            if (Raylib.IsFileDropped() && ImGuiController.IsImGuiInteracted())
                            {
                                var files = Raylib.GetDroppedFiles();
                                AddFiles(files, packageIndex);
                            }

                            int errorCount = 0;
                            foreach (var file in addFiles)
                            {
                                if (file.IdentifierExists && !file.ReplaceExisting)
                                {
                                    errorCount += 1;
                                }
                            }

                            ImGui.BeginTable("ResourcesToAdd", 5);
                            ImGui.TableSetupColumn("File Path", ImGuiTableColumnFlags.None, 300f);
                            ImGui.TableSetupColumn("Identifier", ImGuiTableColumnFlags.None, 150f);
                            ImGui.TableSetupColumn("Exists?", ImGuiTableColumnFlags.None, 120f);
                            ImGui.TableSetupColumn("Replace?", ImGuiTableColumnFlags.None, 90f);
                            ImGui.TableSetupScrollFreeze(0, 1);
                            ImGui.TableHeadersRow();

                            for (int i = 0; i < addFiles.Count; i++)
                            {
                                var file = addFiles[i];

                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0); // file path
                                ImGui.Text(file.FilePath);

                                ImGui.TableSetColumnIndex(1); // identifier
                                if (file.IdentifierExists && !file.ReplaceExisting)
                                {
                                    ImGui.PushID(i);
                                    if (!file.EditingIdentifier)
                                    {
                                        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0, 0, 1f)));
                                        ImGui.Text(file.Identifier);
                                        ImGui.PopStyleColor();
                                        ImGui.SameLine();
                                        if (ImGui.Button("..##ResourceAddEditInput"))
                                        {
                                            file.EditingIdentifier = true;
                                        }
                                    }
                                    else
                                    {
                                        var complete = ImGui.InputText("##ResourceIdentifierEdit", ref file.Identifier, 512, ImGuiInputTextFlags.EnterReturnsTrue);
                                        if (complete)
                                        {
                                            file.IdentifierExists = Application.Instance.ResourceManager.DoesIdentifierExist(file.Identifier);
                                            file.EditingIdentifier = false;
                                        }
                                    }
                                    ImGui.PopID();
                                }
                                else
                                {
                                    if (!file.EditingIdentifier)
                                    {
                                        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0.7f, 0, 1f)));
                                        ImGui.Text(file.Identifier);
                                        ImGui.PopStyleColor();
                                        ImGui.SameLine();
                                        if (ImGui.Button("..##ResourceAddEditInput"))
                                        {
                                            file.EditingIdentifier = true;
                                        }
                                    }
                                    else
                                    {
                                        var complete = ImGui.InputText("##ResourceIdentifierEdit", ref file.Identifier, 512, ImGuiInputTextFlags.EnterReturnsTrue);
                                        if (complete)
                                        {
                                            file.IdentifierExists = Application.Instance.ResourceManager.DoesIdentifierExist(file.Identifier);
                                            file.EditingIdentifier = false;
                                        }
                                    }
                                }

                                ImGui.TableSetColumnIndex(2); // exists
                                ImGui.Text(file.IdentifierExists ? "Identifier Conflicts" : "OK");

                                ImGui.TableSetColumnIndex(3); // replace
                                if (file.IdentifierExists)
                                {
                                    ImGui.PushID(i);
                                    ImGui.Checkbox("##ResourceAddReplace", ref file.ReplaceExisting);
                                    ImGui.PopID();
                                }
                            }

                            ImGui.EndTable();

                            if (selectedPackage > 0 && addFiles.Count > 0)
                            {
                                if (errorCount > 0 || isWriting)
                                    ImGui.BeginDisabled();

                                if (ImGui.Button("Write Data"))
                                {
                                    var rm = Application.Instance.ResourceManager;
                                    foreach (var file in addFiles)
                                    {
                                        if (!file.ReplaceExisting)
                                            rm.AddResource(selectedPackage - 1, file.Identifier, file.FilePath);
                                    }

                                    isWriting = true;
                                    rm.WriteAllAsync(selectedPackage - 1, () =>
                                    {
                                        isWriting = false;
                                        Animate.Reset(writingAllProgressIndex);
                                        page = 0;
                                        addFiles.Clear();

                                        RefreshResources();
                                        ResetCellData();
                                    },
                                    (paths) =>
                                    {
                                        resourceWriteErrorPaths = paths;
                                        resourceWriteHasError = true;
                                    });
                                }

                                if (errorCount > 0 || isWriting)
                                    ImGui.EndDisabled();
                            }
                        }

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Encryption##ResourcesEncryptionOptions"))
                    {
                        for (int i = 0; i < packageNames.Length; i++)
                        {
                            ImGui.PushID(i);
                            ImGui.Text(packageNames[i]); ImGui.SameLine();
                            

                            ImGui.PopID();
                        }

                        ImGui.EndTabItem();
                    }

                    if (resourceWriteHasError)
                    {
                        if (ImGui.BeginTabItem("Errors##ResourceWriteErrors"))
                        {
                            if (ImGui.Button("Clear Errors##ClearResourceWriteErrors"))
                            {
                                resourceWriteHasError = false;
                                resourceWriteErrorPaths = Array.Empty<string>();
                            }

                            foreach (var pathMissing in resourceWriteErrorPaths)
                            {
                                ImGui.TextColored(new Vector4(1f, 0f, 0f, 1f), "Path Missing: ");
                                ImGui.SameLine();
                                ImGui.Text(pathMissing);
                            }

                            ImGui.EndTabItem();
                        }
                    }

                    ImGui.EndTabBar();
                }

                if (isWriting)
                {
                    ImGui.EndDisabled();
                }

                if (isWriting && selectedPackage > 0)
                {
                    var ratio = 0.0f;
                    var rm = Application.Instance.ResourceManager;
                    if (rm.WriteItemsMax > 0)
                    {
                        // forced conversion to float to correct for
                        // integer truncating by the compiler
                        ratio = (float)((float)rm.WriteItemsProgress / (float)rm.WriteItemsMax);
                    }

                    ImGui.ProgressBar(ratio);
                    var package = rm.Packages.ElementAt(selectedPackage - 1);
                    if (rm.WriteItemsProgress >= 0 && rm.WriteItemsProgress < package.FileMapping.Count)
                    {
                        var fileMap = package.FileMapping.ElementAt(rm.WriteItemsProgress);
                        ImGui.Text("Writing " + fileMap.Key + "...");
                    }
                }

                ImGui.End();
            }
        }

        void DrawDeleteErrorPopup()
        {
            if (ImGui.BeginPopupModal("Delete Error"))
            {
                ImGui.Text("You are attempting to delete files that are currently in use.");
                ImGui.Text("You should unload the resources first.");
                ImGui.Text("You may attempt to force unload below, but may cause graphics issues or crash the application.");

                if (ImGui.Button("Force Unload##ForceUnloadResources"))
                {
                    var rm = Application.Instance.ResourceManager;
                    foreach (var row in selectedRows)
                    {
                        var itemName = tableCells[row * 6];
                        rm.Unload(itemName);
                    }
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();

                if (ImGui.Button("OK##DeleteErrorOK"))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        void DrawConfirmDeleteSelectionPopup()
        {
            if (ImGui.BeginPopupModal("Confirm Delete Items"))
            {
                ImGui.Text("Please confirm you would like to delete the current selection?");

                if (ImGui.Button("Confirm##ConfirmDeleteResourceItems"))
                {
                    var packagesAffected = new List<int>();
                    foreach (var selected in selectedRows)
                    {
                        var index = selected;
                        var item = tableCells[index * 6];
                        var resourceIndex = Application.Instance.ResourceManager.GetResourceIndex(item);

                        var found = false;
                        foreach (var package in packagesAffected)
                            if (resourceIndex.PackageIndex == package)
                                found = true;

                        if (!found)
                            packagesAffected.Add(resourceIndex.PackageIndex);

                        Application.Instance.ResourceManager.DeleteItem(resourceIndex.PackageIndex, item);
                    }

                    foreach (var package in packagesAffected)
                        PerformDeletion(package);

                    RefreshResources();
                    ResetCellData();
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        string editPrefix = string.Empty;
        string editSuffix = string.Empty;
        string editTitlePattern = string.Empty;

        string editItemName = string.Empty;

        void DrawSingleEditPopup()
        {
            if (ImGui.BeginPopupModal("Edit Item"))
            {
                ImGui.Text("Name:"); ImGui.SameLine();
                ImGui.InputText("##EditItemName", ref editItemName, 512);

                if (ImGui.Button("Submit##EditItemSubmit"))
                {
                    var index = selectedRows[0];
                    var item = tableCells[index * 6];
                    var resourceIndex = Application.Instance.ResourceManager.GetResourceIndex(item);
                    var package = Application.Instance.ResourceManager.Packages.ElementAt(resourceIndex.PackageIndex);
                    var resource = package.FileMapping.ElementAt(resourceIndex.FileIndex);
                    var temp = package.FileMapping[resource.Key];
                    package.FileMapping.Remove(resource.Key);
                    package.FileMapping[editItemName] = temp;
                    editItemName = string.Empty;
                    SavePackageHeader(resourceIndex.PackageIndex);
                    RefreshResources();
                    ResetCellData();

                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel##EditItemCancel"))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        void DrawEditPopup()
        {
            if (ImGui.BeginPopupModal("Edit Resources"))
            {
                ImGui.Text("Prefix:"); ImGui.SameLine();
                ImGui.SetNextItemWidth(300f);
                ImGui.InputText("##EditItemsPrefix", ref editPrefix, 128);

                ImGui.Text("Suffix:"); ImGui.SameLine();
                ImGui.SetNextItemWidth(300f);
                ImGui.InputText("##EditItemsSuffix", ref editSuffix, 128);

                editTitlePattern = string.Empty;
                if (!string.IsNullOrWhiteSpace(editPrefix))
                {
                    editTitlePattern = editPrefix + "%[0]";
                }

                if (!string.IsNullOrWhiteSpace(editSuffix))
                {
                    editTitlePattern += "%" + editSuffix;
                }

                if (ImGui.Button("Change##BrowseItemsChange"))
                {
                    var packagesToSave = new List<int>();

                    foreach (var index in selectedRows)
                    {
                        var item = tableCells[index * 6];
                        var resourceIndex = Application.Instance.ResourceManager.GetResourceIndex(item);
                        if (resourceIndex.PackageIndex == -1 || resourceIndex.FileIndex == -1)
                            continue;

                        var savePackageFound = false;
                        foreach (var save in packagesToSave)
                            if (save == resourceIndex.PackageIndex)
                                savePackageFound = true;

                        if (!savePackageFound)
                            packagesToSave.Add(resourceIndex.PackageIndex);

                        var package = Application.Instance.ResourceManager.Packages.ElementAt(resourceIndex.PackageIndex);
                        var resource = package.FileMapping.ElementAt(resourceIndex.FileIndex);
                        var finalName = string.Empty;
                        if (editTitlePattern.Contains('%'))
                        {
                            var isPrefix = true;
                            var isSuffix = false;
                            var isReplace = false;
                            for (int i = 0; i < editTitlePattern.Length; i++)
                            {
                                if (editTitlePattern[i] != '%' && editTitlePattern[i] != '[' && editTitlePattern[i] != ']')
                                {
                                    if (isPrefix || isSuffix)
                                    {
                                        finalName += editTitlePattern[i];
                                    }
                                    else if (isReplace)
                                    {
                                        // we don't care what the number is at the moment, just print the name as is.
                                        finalName += resource.Key;
                                    }
                                }
                                else if (editTitlePattern[i] == '%')
                                {
                                    if (isPrefix)
                                    {
                                        isPrefix = false;
                                        isReplace = true;
                                    }
                                    else if (isReplace)
                                    {
                                        isReplace = false;
                                        isSuffix = true;
                                    }
                                }
                            }
                        }

                        var temp = package.FileMapping[resource.Key];
                        package.FileMapping.Remove(resource.Key);
                        package.FileMapping[finalName] = temp;
                    }

                    RefreshResources();
                    ResetCellData();
                    foreach (var save in packagesToSave)
                        SavePackageHeader(save);

                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel##BrowseItemsCancel"))
                {
                    editPrefix = string.Empty;
                    editSuffix = string.Empty;
                    editTitlePattern = string.Empty;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        void AddFiles(string[] files, int inPackage)
        {
            if (addFiles == null)
                addFiles = new List<ResourceAddFile>();

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var identifier = Path.GetFileNameWithoutExtension(file);
                var identifierExists = Application.Instance.ResourceManager.DoesNameExist(identifier);
                addFiles.Add(new ResourceAddFile(file, identifier, inPackage)
                {
                    IdentifierExists = identifierExists
                });
            }
        }

        void ResetPackageNames()
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

        void ResetCellData()
        {
            var min = page * maxItems;
            var max = min + maxItems;
            int total = resources.Count;

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
                if (index >= resources.Count)
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

        string ConvertBytesToString(long bytes)
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

        async void SavePackageHeader(int packageIndex)
        {
            await Task.Run(() =>
            {
                Application.Instance.ResourceManager.RewriteHeader(packageIndex);
            });
        }

        async void PerformDeletion(int packageIndex)
        {
            isWriting = true;
            await Task.Run(() =>
            {
                Application.Instance.ResourceManager.DeleteItemsFromPackage(packageIndex);
            });

            RefreshResources();
            ResetCellData();
            isWriting = false;
        }

    }
}
