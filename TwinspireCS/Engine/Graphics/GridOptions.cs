using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class GridOptions
    {
        /// <summary>
        /// Number of columns in the grid.
        /// </summary>
        public int Columns { get; set; }
        /// <summary>
        /// Number of rows in the grid.
        /// </summary>
        public int Rows { get; set; }

        public GridOptions()
        {
            Columns = 1;
            Rows = 1;
        }

    }
}
