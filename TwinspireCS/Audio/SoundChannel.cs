using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Audio
{
    public struct SoundChannel
    {

        public string? AudioName;
        public float Volume;
        public float Pitch;
        public float Pan;

        public SoundChannel()
        {
            AudioName = null;
            Volume = 0;
            Pitch = 0;
            Pan = 0;
        }

        public SoundChannel(string audioName) : this()
        {
            AudioName = audioName;
        }

    }
}
