using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public enum FlowAlignment
    {
        /// <summary>
        /// Aligns either to the top or left of the row or column.
        /// </summary>
        Zero,
        /// <summary>
        /// Aligns either vertically or horizontally along the row or column.
        /// </summary>
        Middle,
        /// <summary>
        /// Aligns either to the bottom or right of the row or column.
        /// </summary>
        Full,
        /// <summary>
        /// Forces the element to drop to the lowest possible value without overflowing other elements.
        /// </summary>
        Drop
    }
}
