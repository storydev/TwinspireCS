using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public struct SimulationRegion
    {

        public float Hot;
        public float Cold;

        public SimulationRegion(float hot, float cold)
        {
            Hot = hot;
            Cold = cold;
        }

    }
}
