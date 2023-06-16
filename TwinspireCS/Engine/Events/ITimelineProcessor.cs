using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Events
{
    public interface ITimelineProcessor
    {

        List<TimelineEvent> Events { get; set; }
        void StartEvent(TimelineEvent ev);
        int GetCurrentEventIndex();
        TimelineEvent? GetCurrentEvent();
        int DetermineNextEvent();

    }
}
