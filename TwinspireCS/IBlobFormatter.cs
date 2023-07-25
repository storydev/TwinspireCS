using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public interface IBlobFormatter
    {

        string FromObject(IBlobObject item);

        string FromObject(object item);

        string ForEach(IEnumerable<IBlobObject> items);

        string ForEach(IEnumerable<object> items);

        IBlobObject ToObject<T>(string text);

        object ToRawObject(string text, Type type);

        IEnumerable<IBlobObject> ToObjectEnumerable<T>(string text);

        IEnumerable<object> ToRawObjectEnumerable(string text, Type type);

    }
}
