using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Editor
{
    public class ResourceAddFile
    {

        public string FilePath;
        public string Identifier;
        public int PackageIndex;
        public bool ReplaceExisting;
        public bool IdentifierExists;
        public bool EditingIdentifier;

        public ResourceAddFile(string path, string identifier, int packageIndex)
        {
            FilePath = path;
            Identifier = identifier;
            PackageIndex = packageIndex;
            ReplaceExisting = false;
            IdentifierExists = false;
            EditingIdentifier = false;
        }

    }
}
