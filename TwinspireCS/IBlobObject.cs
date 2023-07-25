using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public interface IBlobObject
    {

        string ToBlobString();

        void FromString(string text);

    }
}
