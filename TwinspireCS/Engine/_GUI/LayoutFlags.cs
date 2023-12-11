using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    [Flags]
    public enum LayoutFlags
    {
        /// <summary>
        /// Specifies that rows can grow to the total height of its content.
        /// </summary>
        DynamicRows = 0x01,
        /// <summary>
        /// Specifies that columns can grow to the total width of its content.
        /// </summary>
        DynamicColumns = 0x02,
        /// <summary>
        /// Specifies that rows have specific heights. Content will be clipped. This option overrides DynamicRows.
        /// </summary>
        StaticRows = 0x04,
        /// <summary>
        /// Specifies that columns have specific widths. Content will be clipped. This option overrides DynamicColumns.
        /// </summary>
        StaticColumns = 0x08,
        /// <summary>
        /// Specifies that the height of content will match the height of the row. Only works if StaticRows are used.
        /// </summary>
        FixedComponentHeights = 0x10,
        /// <summary>
        /// Specifies that the width of content will match the width of the row. Only works if StaticRows are used.
        /// </summary>
        FixedComponentWidths = 0x20,
        /// <summary>
        /// When used, components too large to be displayed correctly in rows will not wrap, and will instead fill the current row.
        /// The contents of the component will be clipped.
        /// </summary>
        FillRowsAlways = 0x40,
        /// <summary>
        /// Each individual element will span the full width of the row within its container.
        /// </summary>
        SpanRow = 0x80,
        /// <summary>
        /// Each individual element will span the full height of the column within its container. Unaffected by the maximum content height of the container.
        /// </summary>
        SpanColumn = 0x100,
    }
}
