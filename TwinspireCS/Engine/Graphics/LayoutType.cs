using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public enum LayoutType
    {
        /// <summary>
        /// No automatic layout logic.
        /// </summary>
        None,
        /// <summary>
        /// Specifies Flow layout logic. Performs based on Flow options.
        /// </summary>
        Flow,
        /// <summary>
        /// Specifies Grid layout logic. Performs based on Grid options.
        /// </summary>
        Grid,
        /// <summary>
        /// Specifies Table layout logic. Performs based on Table options.
        /// </summary>
        Table
    }
}
