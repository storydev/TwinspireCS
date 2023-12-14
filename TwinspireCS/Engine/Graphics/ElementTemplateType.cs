using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public enum ElementTemplateType
    {
        /// <summary>
        /// Specifies that the element template contains other elements.
        /// </summary>
        Container,
        /// <summary>
        /// Specifies that the element is considered content and displays more specific types of elements.
        /// </summary>
        Content
    }
}
