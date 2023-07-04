using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public struct ResourceIndex
    {

        public int PackageIndex;
        public int FileIndex;

        public ResourceIndex(int packageIndex, int fileIndex)
        {
            PackageIndex = packageIndex;
            FileIndex = fileIndex;
        }
    }
}
