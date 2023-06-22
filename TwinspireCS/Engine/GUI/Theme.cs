using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class Theme
    {

        public static readonly string BUTTON = "Button";
        public static readonly string BUTTON_HOVER = "ButtonHover";
        public static readonly string BUTTON_DOWN = "ButtonDown";
        public static readonly string DISABLED = "Disabled";
        public static readonly string TOOLTIP = "ToolTip";


        public Dictionary<string, Style> Styles;

        public Theme()
        {
            Styles = new Dictionary<string, Style>();
        }

        private static Theme? _default;
        public static Theme Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Theme();
                }

                return _default;
            }
        }

    }
}
