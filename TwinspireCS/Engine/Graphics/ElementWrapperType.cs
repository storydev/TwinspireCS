using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public enum ElementWrapperType
    {
        /// <summary>
        /// An element containing only content elements within it.
        /// </summary>
        Static,
        /// <summary>
        /// An element containing a combination of content and container type elements.
        /// </summary>
        Wrapper
    }
}
