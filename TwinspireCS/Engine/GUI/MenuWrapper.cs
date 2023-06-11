using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.GUI
{
    public class MenuWrapper
    {

        public string ID;
        public int StartElement;
        public int EndElement;
        public int SelectedElement;
        public string ConfirmKeyName;

        public MenuWrapper()
        {
            ID = string.Empty;
            ConfirmKeyName = string.Empty;
        }

    }
}
