using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public enum FlowWrap
    {
        /// <summary>
        /// Do not enable flow-wrap. Causes clipping in the container.
        /// </summary>
        None,
        /// <summary>
        /// Child elements are justified and along the row or column.
        /// </summary>
        Justify,
        /// <summary>
        /// Child elements retain margin distance between each other when wrapping.
        /// </summary>
        MarginOnly
    }
}
