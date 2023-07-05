using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class TableDefinition
    {
        // for columns
        public string[] Columns;
        public float[] ColumnInitialWidths;
        public float[] ColumnWidths;
        public int CurrentColumn;
        public int LookAtColumn;
        public int LookAtRow;

        // for rows
        public float[] RowHeights;

        // for cells
        public bool[] ContentClipped;

        // appearance
        public bool Borders;

        public TableDefinition()
        {
            Columns = Array.Empty<string>();
            ColumnInitialWidths = Array.Empty<float>();
            ColumnWidths = Array.Empty<float>();
            RowHeights = Array.Empty<float>();
            ContentClipped = Array.Empty<bool>();
            CurrentColumn = 0;
            Borders = false;
        }

    }
}
