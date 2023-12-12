using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public enum RenderTargetType
    {
        /// <summary>
        /// Apply to the whole client.
        /// </summary>
        Screen,
        /// <summary>
        /// Apply to the primary scope containing the canvas.
        /// </summary>
        Canvas,
        /// <summary>
        /// Apply to a frame identified in interface scope.
        /// </summary>
        Frame
    }
}
