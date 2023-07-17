using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
        /// A short version of the name.
        /// </summary>
        public string Keyword { get; protected set; }

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

        /// <summary>
        /// Gets a value determining if the extension should render without needing
        /// to be in the editor. Must be called with <c>Editor.Render</c> somewhere in your
        /// rendering routine.
        /// </summary>
        public bool RequireForcedRendering { get; protected set; }

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

        /// <summary>
        /// A rendering routine that does not perform anything.
        /// Should be overridden in an inherited class to perform any rendering that this wrapper might require.
        /// </summary>
        public virtual void Render()
        {

        }

    }
}
