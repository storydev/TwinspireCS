using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Events
{
    public class TimelineEvent
    {

        public uint Type;
        public int Subtype;
        public int[] Data;

        public TimelineEvent()
        {
            Data = Array.Empty<int>();
        }

    }
}
