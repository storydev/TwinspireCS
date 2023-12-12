using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class Element
    {
        /// <summary>
        /// Determines in which context this element is rendered.
        /// </summary>
        public RenderScope Scope { get; set; }
        /// <summary>
        /// Specifies what template to use for this element for rendering.
        /// </summary>
        public int TemplateIndex { get; set; }
        /// <summary>
        /// Specifies the size of the element.
        /// </summary>
        public Rectangle Dimension { get; set; }

        public Element()
        {

        }

    }
}
