using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Audio
{
    public class SoundState
    {

        public string AudioName;
        public int AnimationIndex;
        public float Duration;
        public float ToVolume;
        public bool Complete;

        public SoundState()
        {
            Complete = false;
            AudioName = string.Empty;
            ToVolume = 1.0f;
            Duration = 0.0f;
        }

    }
}
