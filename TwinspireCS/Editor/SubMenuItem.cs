using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Editor
{
    public class SubMenuItem
    {

        public string Name;
        public Action? OnSelected;
        
        public SubMenuItem()
        {
            Name = string.Empty;
            OnSelected = null;
        }

    }
}
