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

        /// <summary>
        /// The quick search ID for the current quick search window.
        /// Use this to determine which quick search window is currently in use.
        /// </summary>
        public string QuickSearchID
        {
            get; private set;
        }

        /// <summary>
        /// The name of the asset selected in quick search.
        /// </summary>
        public string QuickSearchAssetName
        {
            get
            {
                var casted = (ResourceManagerEditor)Extension;
                return casted.QuickSearchAssetName;
            }
        }

        /// <summary>
        /// Open a quick search for resources within your project. Does not include resources
        /// added at runtime.
        /// </summary>
        /// <param name="id">The unique ID for this quick search.</param>
        /// <param name="filters">The filters to use. These are the file extensions of the respective resources. E.g. '.png.jpg'</param>
        /// <param name="assetName">The initial asset name to use.</param>
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
