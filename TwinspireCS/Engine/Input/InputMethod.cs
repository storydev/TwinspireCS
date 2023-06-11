using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Input
{
    public struct InputMethod
    {

        public InputType Type;
        public int Button;

        // Keyboard-Only
        public bool Control;
        public bool Shift;
        public bool Alt;

        // Console-Only
        public int HoldButton;

        public InputMethod()
        {
            Type = InputType.Keyboard;
            Button = -1;
            Control = false;
            Shift = false;
            Alt = false;
            HoldButton = -1;
        }

    }
}
