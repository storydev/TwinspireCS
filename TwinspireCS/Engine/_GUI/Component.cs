using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class Component
    {

        public ComponentElement[] Elements;
        public InterpretedType[] ElementInterpetedTypes;

        public Component(int elementCount)
        {
            Elements = new ComponentElement[elementCount];
            ElementInterpetedTypes = new InterpretedType[elementCount];
        }

    }
}
