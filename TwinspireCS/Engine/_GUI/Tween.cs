using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public struct Tween
    {

        public Style From;
        public Style To;
        public float Opacity;
        public float Scale;
        public float Duration;
        public float Delay;

        public Tween()
        {
            From = new Style();
            To = new Style();
            Opacity = 1.0f;
            Scale = 1.0f;
            Duration = 1.0f;
            Delay = 0.0f;
        }

        public Tween(Style from, Style to) : this()
        {
            From = from;
            To = to;
        }

    }
}
