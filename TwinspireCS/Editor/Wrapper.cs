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

        public Wrapper(IExtension extension)
        {
            Extension = extension;
        }

    }
}
