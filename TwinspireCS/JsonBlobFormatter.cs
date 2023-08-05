using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace TwinspireCS
{
    public class JsonBlobFormatter : IBlobFormatter
    {
        public string ForEach(IEnumerable<IBlobObject> items)
        {
            return JsonSerializer.Serialize(items);
        }

        public string ForEach(IEnumerable<object> items)
        {
            return JsonSerializer.Serialize(items);
        }

        public string FromObject(IBlobObject item)
        {
            return JsonSerializer.Serialize(item);
        }

        public string FromObject(object item)
        {
            return JsonSerializer.Serialize(item);
        }

        public IBlobObject ToObject<T>(string text)
        {
            return (IBlobObject)JsonSerializer.Deserialize<T>(text);
        }

        public IEnumerable<IBlobObject> ToObjectEnumerable<T>(string text)
        {
            return (IEnumerable<IBlobObject>)JsonSerializer.Deserialize<IEnumerable<T>>(text);
        }

        public object ToRawObject(string text, Type type)
        {
            return JsonSerializer.Deserialize(text, type);
        }

        public IEnumerable<object> ToRawObjectEnumerable(string text, Type type)
        {
            var generic = typeof(IEnumerable<>).MakeGenericType(type);
            return (IEnumerable<object>)JsonSerializer.Deserialize(text, generic);
        }
    }
}
