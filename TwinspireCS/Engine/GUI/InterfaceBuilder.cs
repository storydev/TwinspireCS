using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.GUI
{
    public class InterfaceBuilder
    {

        private Dictionary<string, Component> components;
        public IDictionary<string, Component> Components => components;

        public InterfaceBuilder()
        {
            components = new Dictionary<string, Component>();
        }

        public void AddComponent(string name, Component component)
        {
            components[name] = component;
        }

    }
}
