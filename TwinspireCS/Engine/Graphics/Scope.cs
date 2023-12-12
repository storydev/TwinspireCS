using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class Scope
    {
        /// <summary>
        /// Determines in what render scope this scope applies to.
        /// </summary>
        public RenderScope RenderScope { get; set; }
        /// <summary>
        /// Specifies whether this scope performs layout logic or not.
        /// </summary>
        public WrapperType WrapType { get; set; }
        /// <summary>
        /// The target of this scope. Where should it render and what is its' source, if any.
        /// </summary>
        public RenderTarget Target { get; set; } = new RenderTarget();

        public Scope()
        {

        }

    }
}
