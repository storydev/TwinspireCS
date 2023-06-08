using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Engine.Graphics
{
    public class Frame
    {

        public Rectangle Dimension;
        public NPatchInfo Patch;
        public bool UsingPatch;

        public Frame()
        {
            Dimension = new Rectangle(0, 0, 0, 0);
            Patch = new NPatchInfo();
            UsingPatch = false;
        }

    }
}
