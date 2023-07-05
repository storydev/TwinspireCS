using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Editor
{
    public class Editor
    {

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
        }

        /// <summary>
        /// Call this during your main rendering routine as the last thing performed
        /// before requesting the ImGui draw calls.
        /// </summary>
        public static void Render()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(3, 3));
            if (ImGui.Begin("##EditorNavigator", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoTitleBar))
            {
                ImGui.Text("Extension:"); ImGui.SameLine();
                ImGui.SetNextItemWidth(175f);
                ImGui.Combo("##EditorExtensionsList", ref activeWrapper, wrapperNames, wrapperNames.Length);
                ImGui.Text("Author: " + wrapperAuthors[activeWrapper]);

                ImGui.End();
            }

            if (activeWrapper > -1 && activeWrapper < wrappers.Count)
            {
                wrappers[activeWrapper].Extension.Render();
            }
        }

    }
}
