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
        public Dictionary<string, DataSegment> FileMapping;
        public int Version;
        public int HeaderSize;

        public DataPackage()
        {
            SourceFilePath = string.Empty;
            FileMapping = new Dictionary<string, DataSegment>();
            Version = 0;
        }

    }

    public class DataSegment
    {

        public long Cursor;
        public long Size;
        public byte[] Data;

        public DataSegment()
        {
            Cursor = 0;
            Size = 0;
            Data = Array.Empty<byte>();
        }

    }
}
