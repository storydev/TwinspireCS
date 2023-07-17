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
            Keyword = "RM";
            Author = "StoryDev";
        }

        public string QuickSearchID
        {
            get; private set;
        }

        public string QuickSearchAssetName
        {
            get
            {
                var casted = (ResourceManagerEditor)Extension;
                return casted.QuickSearchAssetName;
            }
        }

        public void OpenQuickSearch(string id, string filters, string assetName)
        {
            QuickSearchID = id;
            RequireForcedRendering = true;
            var casted = (ResourceManagerEditor)Extension;
            casted.QuickSearch(filters, assetName);
        }

        /// <summary>
        /// Render any extra Resource Manager options.
        /// </summary>
        public override void Render()
        {
            var casted = (ResourceManagerEditor)Extension;
            casted.RenderQuickSearch();
        }
    }
}
