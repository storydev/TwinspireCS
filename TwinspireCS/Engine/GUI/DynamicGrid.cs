using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class DynamicGrid : Grid
    {

        public int StartElement;
        public int EndElement;

        public DynamicGrid()
        {
            StartElement = -1;
            EndElement = -1;
        }

    }
}
