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
        public List<string> ToDelete;
        public Dictionary<string, DataSegment> FileMapping;
        public int Version;
        public int HeaderSize;

        public DataPackage()
        {
            SourceFilePath = string.Empty;
            FileMapping = new Dictionary<string, DataSegment>();
            ToDelete = new List<string>();
            Version = 0;
        }

    }

    public class DataSegment
    {

        public string OriginalSourceFile;
        public string FileExt;
        public long Cursor;
        public long Size;
        public long CompressedSize;
        public byte[] Data;

        public DataSegment()
        {
            OriginalSourceFile = string.Empty;
            FileExt = string.Empty;
            Cursor = 0;
            Size = 0;
            CompressedSize = 0;
            Data = Array.Empty<byte>();
        }

    }
}
