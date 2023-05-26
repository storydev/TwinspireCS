using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine
{
    public class Animate
    {

        public static int Max;
        private static float[]? animateTicks;
        private static float[]? animateTicksDelays;
        private static bool[]? animateTickReset;
        private static int[]? animateTickLoopDir;
        private static int animateIndex;
        private static float lastAnimateSecondsValue;

        public static void Init()
        {
            animateIndex = -1;
            Clear();
        }

        public static void Clear()
        {
            animateTicks = new float[Max];
            animateTicksDelays = new float[Max];
            animateTickReset = new bool[Max];
            animateTickLoopDir = new int[Max];
        }

        public static int Create()
        {
            return ++animateIndex;
        }

        public static void ResetTicks()
        {
            for (int i = 0; i < Max; i++)
            {
                if (animateTickReset[i])
                {
                    animateTicks[i] = 0;
                    animateTickReset[i] = false;
                }
            }
        }

        public static bool Tick(int index, float seconds, float delay = 0.0f)
        {
            var result = false;
            lastAnimateSecondsValue = seconds;

            if (delay > 0.0f)
            {
                if (animateTicks[index] + Raylib.GetFrameTime() > seconds)
                {
                    result = true;
                    if (animateTicksDelays[index] + Raylib.GetFrameTime() > delay)
                    {
                        result = false;
                        animateTicksDelays[index] = 0.0f;
                        animateTickReset[index] = true;
                    }
                    else
                    {
                        animateTicksDelays[index] += Raylib.GetFrameTime();
                    }
                }
                else
                {
                    animateTicks[index] += Raylib.GetFrameTime();
                }
            }
            else
            {
                if (animateTicks[index] + Raylib.GetFrameTime() > seconds)
                {
                    result = true;
                }
                else
                {
                    animateTicks[index] += Raylib.GetFrameTime();
                }
            }

            return result;
        }

        public static bool TickCondition(int index, float seconds, bool conditional)
        {
            if (conditional)
            {
                return Tick(index, seconds);
            }
            else
            {
                return false;
            }
        }

        public static bool TickLoop(int index, float seconds, float delay = 0.0f)
        {
            var result = false;
            lastAnimateSecondsValue = seconds;

            if (animateTickLoopDir[index] == 1)
            {
                if (animateTicks[index] + Raylib.GetFrameTime() > seconds)
                {
                    result = true;
                    animateTickLoopDir[index] = -1;
                }
                else
                {
                    animateTicks[index] += Raylib.GetFrameTime();
                }
            }
            else if (animateTickLoopDir[index] == -1)
            {
                if (animateTicksDelays[index] + Raylib.GetFrameTime() > delay)
                {
                    if (animateTicks[index] - Raylib.GetFrameTime() < 0)
                    {
                        result = true;
                        animateTickLoopDir[index] = 1;
                    }
                    else
                    {
                        animateTicks[index] -= Raylib.GetFrameTime();
                    }

                    animateTicksDelays[index] = 0.0f;
                }
                else
                {
                    animateTicksDelays[index] += Raylib.GetFrameTime();
                }
            }

            return result;
        }

        public static float GetRatio(int index)
        {
            return animateTicks[index] / lastAnimateSecondsValue;
        }

        public static void Reset(int index)
        {
            animateTickReset[index] = true;
        }

    }
}
