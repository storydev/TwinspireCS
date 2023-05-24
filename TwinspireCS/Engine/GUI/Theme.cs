﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class Theme
    {

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