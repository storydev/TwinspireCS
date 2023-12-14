using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.Graphics
{
    public class RenderTarget
    {
        /// <summary>
        /// Specifies where this target should be rendered.
        /// </summary>
        public RenderTargetType Type { get; set; }
        /// <summary>
        /// Specifies the source for this target, if any.
        /// </summary>
        public Rectangle SourceRect { get; set; }
        /// <summary>
        /// A callback that obtains all the elements drawn to this render target.
        /// </summary>
        public OnGetElements GetElements { get; set; }

        internal RenderScope ExpectedScope { get; set; }

        public RenderTarget()
        {

        }

    }
}
