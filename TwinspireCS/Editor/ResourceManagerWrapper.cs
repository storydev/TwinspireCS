using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Editor
{
    public class ResourceManagerWrapper : Wrapper
    {
        public ResourceManagerWrapper(IExtension extension) : base(extension)
        {
            Name = "Resource Manager";
            Author = "StoryDev";
        }
    }
}
