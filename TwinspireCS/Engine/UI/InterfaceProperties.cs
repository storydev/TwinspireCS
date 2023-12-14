using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TwinspireCS.Engine.UI
{
    public class InterfaceProperties
    {
        /// <summary>
        /// Specifies the padding. These values adjust the positional constraints of content
        /// within a container type element.
        /// </summary>
        public Vector4 Padding { get; set; }
        /// <summary>
        /// Specifies the margin. These values adjust the outer distance between container type
        /// elements.
        /// </summary>
        public Vector4 Margin { get; set; }
        /// <summary>
        /// Specifies the thickness of borders around a container type object.
        /// </summary>
        public Vector4 Border { get; set; }



        public InterfaceProperties()
        {

        }

    }
}
