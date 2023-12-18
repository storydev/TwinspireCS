using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class Flow
    {
        /// <summary>
        /// Specifies how elements flow in the container.
        /// </summary>
        public FlowDirection Direction { get; set; }
        /// <summary>
        /// Specifies how child elements should wrap once overflow is reached.
        /// </summary>
        public FlowWrap WrappingBehaviour { get; set; }
        /// <summary>
        /// Specifies how child elements should align in each row/column.
        /// </summary>
        public FlowAlignment Alignment { get; set; }

        public Flow()
        {
            Direction = FlowDirection.LeftToRight;
            WrappingBehaviour = FlowWrap.MarginOnly;
            Alignment = FlowAlignment.Zero;
        }

    }
}
