using System;
using System.Collections.Generic;
using Raylib_cs;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class ElementState
    {
        /// <summary>
        /// Get the position of element templates within the state.
        /// </summary>
        public Vector2[] Position { get; private set; }
        /// <summary>
        /// Get the scale of element templates within the state.
        /// </summary>
        public Vector2[] Scale { get; private set; }
        /// <summary>
        /// Get the element state types of element templates within the state.
        /// </summary>
        public ElementStateType[] StateType { get; private set; }

        public ElementState()
        {
            Position = Array.Empty<Vector2>();
            Scale = Array.Empty<Vector2>();
            StateType = Array.Empty<ElementStateType>();
        }

        /// <summary>
        /// Set the size of the arrays for this instance. Does not copy existing data.
        /// </summary>
        /// <param name="count">The size of each array.</param>
        public void SetSize(int count)
        {
            Position = new Vector2[count];
            Scale = new Vector2[count];
            StateType = new ElementStateType[count];
        }

    }
}
