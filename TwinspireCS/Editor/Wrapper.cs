using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Editor
{
    /// <summary>
    /// A Wrapper is designed to wrap your extension to function
    /// with the main Editor included with Twinspire. To prevent
    /// conflicts with other extensions, wrappers are designed 
    /// only to communicate within itself.
    /// 
    /// To use this class, you must create a class derived from this one.
    /// </summary>
    public abstract class Wrapper
    {

        /// <summary>
        /// The name of this wrapper.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The name of the author publishing this extension.
        /// </summary>
        public string Author { get; protected set; }

        /// <summary>
        /// An interface allowing initialisation and rendering of the extension
        /// itself.
        /// </summary>
        public IExtension Extension { get; private set; }

        private List<SubMenuItem> subMenuItems;
        public IEnumerable<SubMenuItem> SubMenuItems { get { return subMenuItems; } }

        public Wrapper(IExtension extension)
        {
            Extension = extension;
            subMenuItems = new List<SubMenuItem>();
        }

        /// <summary>
        /// Add an item selectable by the user while this module is active.
        /// When the item is selected, call the given action.
        /// </summary>
        /// <param name="name">The name to give this option.</param>
        /// <param name="onSelected">The callback method to call when this item is selected.</param>
        protected void AddSubmenu(string name, Action onSelected)
        {
            subMenuItems.Add(new SubMenuItem()
            {
                Name = name,
                OnSelected = onSelected
            });
        }

    }
}
