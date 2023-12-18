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

        private List<ElementTemplate> elementTemplates;
        /// <summary>
        /// Gets the list of element templates that exist for this state.
        /// </summary>
        public IEnumerable<ElementTemplate> ElementTemplates { get => elementTemplates; }

        private Dictionary<int, ElementState> states;
        /// <summary>
        /// Gets a dictionary of all the states for this template.
        /// </summary>
        public IDictionary<int, ElementState> States { get => states; }
        /// <summary>
        /// Specifies how the container performs layout logic on its children.
        /// </summary>
        public LayoutType LayoutType { get; set; }
        /// <summary>
        /// Specifies the flow behaviour of this container.
        /// </summary>
        public Flow Flow { get; set; }
        /// <summary>
        /// Specifies the grid behaviour of this container.
        /// </summary>
        public GridOptions Grid { get; set; }

        /// <summary>
        /// Specifies whether this element should wrap containers as well as content.
        /// </summary>
        public ElementWrapperType WrapperType { get; set; }

        /// <summary>
        /// Enables scrolling within the container when child elements begin
        /// overflowing. Some flow behaviours are ignored if this is set to <c>true</c>.
        /// </summary>
        public bool AllowAutoScrolling { get; set; }
        /// <summary>
        /// Specifies that the horizontal scrollbar should always show or not.
        /// </summary>
        public bool ShowHorizontalScrollbar { get; set; }
        /// <summary>
        /// Specifies that the vertical scrollbar should always show or not.
        /// </summary>
        public bool ShowVerticalScrollbar { get; set; }

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
        /// <summary>
        /// Gets or sets the index to which this element template is aligned to in its respective container.
        /// </summary>
        public int AlignedTo { get; set; }
        /// <summary>
        /// Specifies the alignment of this element template to the <c>AlignedTo</c> element, if > -1. If no
        /// element alignment is set, align to the container.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }
        /// <summary>
        /// Specifies the alignment of this element template to the <c>AlignedTo</c> element, if > -1. If no
        /// element alignment is set, align to the container.
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; set; }

        #endregion

        public ElementTemplate()
        {
            Name = string.Empty;
            elementTemplates = new List<ElementTemplate>();
            states = new Dictionary<int, ElementState>();
            Flow = new Flow();
            Grid = new GridOptions();
        }


    }
}
