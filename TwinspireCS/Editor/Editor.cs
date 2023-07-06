using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Raylib_cs;

namespace TwinspireCS.Editor
{
    public class Editor
    {

        private static List<string> messages;
        private static bool showMessages;
        private static List<Wrapper> wrappers;
        private static int activeWrapper;

        static string[] wrapperNames;
        static string[] wrapperAuthors;

        /// <summary>
        /// Adds a wrapper to an extension.
        /// </summary>
        /// <param name="wrapper">The wrapper to add to the editor.</param>
        public static void AddWrapper(Wrapper wrapper)
        {
            wrappers ??= new List<Wrapper>();
            wrappers.Add(wrapper);
        }

        /// <summary>
        /// Initialise the editor and all extensions. Any extensions must be added
        /// prior to calling this function.
        /// </summary>
        public static void Init()
        {
            wrapperNames = new string[wrappers.Count];
            wrapperAuthors = new string[wrappers.Count];
            for (int i = 0; i < wrappers.Count; i++)
            {
                var wrap = wrappers[i];
                wrap.Extension.Init();
                wrapperNames[i] = wrap.Name;
                wrapperAuthors[i] = wrap.Author;
            }

            messages = new List<string>();
        }

        /// <summary>
        /// Requests a message to be displayed to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void BroadcastMessage(string message)
        {
            messages.Add(message);
            showMessages = true;
        }

        internal static void ExecuteExtensionAssemblies()
        {
            var localAppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var files = Directory.GetFiles(Path.Combine(localAppPath, "Extensions"), "*.dll");
            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file);
                var projectType = assembly.GetType(assembly.GetName().Name + ".Project");
                if (projectType != null)
                {
                    var initPath = projectType.GetMethod("GetWrapper", BindingFlags.Static | BindingFlags.Public);
                    var wrapper = (Wrapper)initPath?.Invoke(null, null);
                    if (wrapper != null)
                    {
                        wrappers.Add(wrapper);
                    }
                }
            }
        }

        /// <summary>
        /// Call this during your main rendering routine as the last thing performed
        /// before requesting the ImGui draw calls.
        /// </summary>
        public static void Render()
        {
            ImGui.SetNextWindowPos(new Vector2(3, 3));
            if (ImGui.Begin("##EditorNavigator", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoTitleBar))
            {
                ImGui.Text("Extension:"); ImGui.SameLine();
                ImGui.SetNextItemWidth(175f);
                ImGui.Combo("##EditorExtensionsList", ref activeWrapper, wrapperNames, wrapperNames.Length);
                ImGui.Text("Author: " + wrapperAuthors[activeWrapper]);

                ImGui.End();
            }

            if (showMessages)
            {
                var messagesSize = new Vector2(300, 250);
                ImGui.SetNextWindowSize(messagesSize, ImGuiCond.Always);
                ImGui.SetNextWindowPos(new Vector2(Raylib.GetRenderWidth() - messagesSize.X - 3, 3), ImGuiCond.Always);
                if (ImGui.Begin("Messages##EditorMessages", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoBringToFrontOnFocus))
                {
                    if (ImGui.Button("Clear##ClearEditorMessages"))
                    {
                        messages.Clear();
                    } ImGui.SameLine();
                    if (ImGui.Button("Close##CloseEditorMessagesWindow"))
                    {
                        showMessages = false;
                    }

                    ImGui.Separator();

                    foreach (var message in messages)
                    {
                        ImGui.Text(message);
                    }

                    ImGui.End();
                }
            }

            if (activeWrapper > -1 && activeWrapper < wrappers.Count)
            {
                wrappers[activeWrapper].Extension.Render();
            }
        }

    }
}
