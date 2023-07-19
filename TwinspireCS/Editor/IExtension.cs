using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Editor
{
    public interface IExtension
    {

        void Init();
        void RenderImGui();
        void RenderRaylib();

    }
}
