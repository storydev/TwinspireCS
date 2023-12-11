using Raylib_cs;
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

        public string ID;
        public ElementType Type;
        public int GridIndex;
        public int CellIndex;
        public DrawContext DrawContext;
        public bool IsBaseElement;
        public bool Rendered;
        public bool Visible;
        public Rectangle Dimension;
        public ElementState NextState;
        public ElementState State;
        public ElementState LastState;
        public ComponentShape Shape;

        public Element()
        {

        }

    }
}
