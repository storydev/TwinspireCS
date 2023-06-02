using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public struct ComponentElement
    {

        public ComponentShape Shape;
        public Vector2 Measure;
        public Vector2 Offset;
        public ContentAlignment Alignment;
        public int AlignAgainstIndex;
        public bool AlignmentAgainstOnOutside;
        public ElementType Type;
        public MeasureType HorizontalMeasureType;
        public MeasureType VerticalMeasureType;


        public ComponentElement()
        {
            Shape = ComponentShape.Rectangle;
            Measure = new Vector2(1, 1);
            Offset = new Vector2(0, 0);
            Type = ElementType.NonInteractive;
            HorizontalMeasureType = MeasureType.Percentage;
            VerticalMeasureType = MeasureType.Percentage;
            Alignment = ContentAlignment.TopLeft;
            AlignAgainstIndex = -1;
            AlignmentAgainstOnOutside = false;
        }

    }
}
