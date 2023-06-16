using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinspireCS.Engine.GUI;
using Raylib_cs;

namespace TwinspireCS.Engine.Events
{
    public class TimelineManager
    {

        static bool showInitError;
        static Canvas targetCanvas;
        static int targetErrorMessageLayoutIndex;

        static List<TimelineEvent> events;

        public static ITimelineProcessor? TimelineProcessor { get; set; }

        /// <summary>
        /// Initialise the TimelineManager with the given target canvas to render any information
        /// into.
        /// </summary>
        /// <param name="target">The canvas to render into.</param>
        public static void Init(Canvas target)
        {
            events = new List<TimelineEvent>();

            if (TimelineProcessor == null)
            {
                showInitError = true;
            }

            targetCanvas = target;
            targetErrorMessageLayoutIndex = targetCanvas.CreateLayout(new Rectangle(0, 0, Raylib.GetRenderWidth() * .5f, 40), 1, 1);
        }

        /// <summary>
        /// Render any information required by the TimelineManager.
        /// </summary>
        public static void Render()
        {
            if (targetCanvas != null)
            {
                targetCanvas.DrawTo(targetErrorMessageLayoutIndex, 0, LayoutFlags.SpanColumn | LayoutFlags.SpanRow);
                targetCanvas.Label("TimelineErrorMessage", "No timeline has been assigned. Could not begin game.");
            }
        }

        public static void StartGame()
        {
            var ev = events.First();
            if (ev != null)
            {
                TimelineProcessor?.StartEvent(ev);
            }
        }

    }
}
