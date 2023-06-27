using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Editor
{
    public class ResourceFile
    {

        public DataSegment Segment;
        public int PackageIndex;
        public string Name;

        public ResourceFile()
        {
            Segment = new DataSegment();
            PackageIndex = -1;
            Name = string.Empty;
        }

    }
}
