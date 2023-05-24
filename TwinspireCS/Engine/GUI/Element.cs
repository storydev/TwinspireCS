﻿using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class Element
    {

        public ElementType Type;
        public int GridIndex;
        public int CellIndex;
        public int StyleIndex;
        public Rectangle Dimension;
        public ElementState State;

        public Element()
        {

        }

    }
}