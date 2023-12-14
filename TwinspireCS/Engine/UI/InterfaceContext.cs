using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinspireCS.Engine.Graphics;

namespace TwinspireCS.Engine.UI
{
    public class InterfaceContext
    {

        static RenderTarget currentTarget;

        public static void SetRenderTarget(RenderTarget target)
        {
            if (currentTarget != null)
            {
                // @TODO: Do something with elements
                
            }

            currentTarget = target;
        }

        

    }
}
