using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class ElementTemplate
    {

        /// <summary>
        /// Gets or sets the name of this template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies the type for this template.
        /// </summary>
        public ElementTemplateType Type { get; set; }

        #region Container Data

        /// <summary>
        /// Determines if this element allows borders to be drawn around it.
        /// </summary>
        public bool HasBorders { get; set; }

        private List<ElementTemplate> elements;
        /// <summary>
        /// Gets the list of element templates that exist for this state.
        /// </summary>
        public IEnumerable<ElementTemplate> Elements { get => elements; }

        private Dictionary<int, ElementState> states;
        /// <summary>
        /// Gets a dictionary of all the states for this template.
        /// </summary>
        public IDictionary<int, ElementState> States { get => states; }
        /// <summary>
        /// Specifies the flow behaviour of this container.
        /// </summary>
        public Flow Flow { get; set; }

        #endregion

        #region Content Data
        /// <summary>
        /// Specifies that the content for this template can be auto-sized.
        /// Only applies to text.
        /// </summary>
        public bool CanAutoSize { get; set; }
        /// <summary>
        /// Specifies the shape for this content.
        /// </summary>
        public ElementTemplateShape Shape { get; set; }
        /// <summary>
        /// Specifies the initial size of this template. Ignored if <c>CanAutoSize</c> is set to <c>true</c>.
        /// </summary>
        public Vector2 Size { get; set; }



        #endregion


    }
}
