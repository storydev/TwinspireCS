using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public class ResourceGroup
    {

        public List<string> RequestedWaves { get; private set; }

        public List<string> RequestedMusic { get; private set; }

        public List<string> RequestedFonts { get; private set; }

        public List<string> RequestedImages { get; private set; }

        public List<string> RequestedBlobs { get; private set; }

        public ResourceGroup()
        {
            RequestedFonts = new List<string>();
            RequestedImages = new List<string>();
            RequestedMusic = new List<string>();
            RequestedWaves = new List<string>();
            RequestedBlobs = new List<string>();
        }

    }
}
