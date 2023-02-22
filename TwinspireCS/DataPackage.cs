using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public class DataPackage
    {

        public string SourceFilePath;
        public long FileBufferCount;
        public long FileCursor;
        public Dictionary<string, long[]> FileMapping;
        public FileStream? RawStream;
        public int Version;

        public DataPackage()
        {
            SourceFilePath = string.Empty;
            FileMapping = new Dictionary<string, long[]>();
            Version = 0;
        }

    }
}
