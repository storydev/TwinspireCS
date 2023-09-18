using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.ImExtensions
{
    public interface ISearchSource
    {

        IEnumerable<object> Search(object query);

    }
}
