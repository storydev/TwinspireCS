using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinspireCS.Engine.Graphics;
using IG = ImGuiNET;

namespace TwinspireCS.Engine.ImGui
{
    public class Forms
    {

        public static bool FieldCombo(string id, string text, ref int selectedItem, int width, string[] array)
        {
            IG.ImGui.Text(text); IG.ImGui.SameLine();
            IG.ImGui.SetNextItemWidth(width);
            return IG.ImGui.Combo("##" + id, ref selectedItem, array, array.Length);
        }

    }
}
