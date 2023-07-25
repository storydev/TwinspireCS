using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public class DefaultBlobFormatter : IBlobFormatter
    {

        public DefaultBlobFormatter()
        {
            

        }

        public string ForEach(IEnumerable<IBlobObject> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("<{0}>", items.Count()));
            foreach (var item in items)
            {
                sb.AppendLine("/:");
                sb.AppendLine(item.ToBlobString());
                sb.AppendLine(":/");
            }
            return sb.ToString();
        }

        public string ForEach(IEnumerable<object> items)
        {
            var sb = new StringBuilder();
            var objectCount = items.Count();
            var itemMap = Blob.TransformObjects(items);
            for (int i = 0; i < objectCount; i++)
            {
                sb.AppendLine("/:");
                foreach (var kv in itemMap)
                {
                    sb.AppendLine(string.Format(".{0} {1}", kv.Key, kv.Value[i].ToString()));
                }
                sb.AppendLine(":/");
            }
            return sb.ToString();
        }

        public string FromObject(IBlobObject item)
        {
            return item.ToBlobString();
        }

        public string FromObject(object item)
        {
            var itemMap = Blob.TransformObject(item);
            var sb = new StringBuilder();
            sb.AppendLine("/:");
            foreach (var kv in itemMap)
            {
                sb.AppendLine(string.Format(".{0} {1}", kv.Key, kv.Value.ToString()));
            }
            sb.AppendLine(":/");
            return sb.ToString();
        }

        public IBlobObject ToObject<T>(string text)
        {
            if (!typeof(T).IsAssignableFrom(typeof(IBlobObject)))
                return null;

            var instance = (IBlobObject)Activator.CreateInstance(typeof(T));
            instance.FromString(text);
            return instance;
        }

        public IEnumerable<IBlobObject> ToObjectEnumerable<T>(string text)
        {
            if (!text.StartsWith('<'))
                return null;

            var success = int.TryParse(text[1..2], out int count);
            if (count > 0 && success)
            {
                int index = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        index = i + 1;
                        break;
                    }
                }

                var result = new List<IBlobObject>();
                for (int i = 0; i < count; i++)
                {
                    var itemText = "";
                    for (int cursor = index; cursor < text.Length; cursor++)
                    {
                        index += 1;
                        if (text[cursor] == '/' && text[cursor + 1] == ':')
                        {
                            continue;
                        }
                        else if (text[cursor] == ':' && text[cursor + 1] == '/')
                        {
                            break;
                        }
                        else if (text[cursor] == ':' && (text[cursor + 1] == '\r' || text[cursor + 1] == '\n'))
                        {
                            continue;
                        }
                        else
                        {
                            itemText += text[cursor];
                        }
                    }
                    if (!string.IsNullOrEmpty(itemText))
                    {
                        result.Add(ToObject<T>(itemText));
                    }
                }
                return result;
            }

            return new List<IBlobObject>();
        }


        /// <summary>
        /// WIP. Do not use.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ToRawObject(string text, Type type)
        {
            var itemText = "";
            for (int cursor = 0; cursor < text.Length; cursor++)
            {
                if (text[cursor] == '/' && text[cursor + 1] == ':')
                {
                    continue;
                }
                else if (text[cursor] == ':' && text[cursor + 1] == '/')
                {
                    break;
                }
                else if (text[cursor] == ':' && (text[cursor + 1] == '\r' || text[cursor + 1] == '\n'))
                {
                    continue;
                }
                else
                {
                    itemText += text[cursor];
                }
            }

            if (!string.IsNullOrEmpty(itemText))
            {
                // @TODO

                var instance = Activator.CreateInstance(type);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {

                }
            }

            return null;
        }

        /// <summary>
        /// WIP. Do not use.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<object> ToRawObjectEnumerable(string text, Type type)
        {
            throw new NotImplementedException();
        }


        private IDictionary<string, object> CreateTextValueMap(string text, Type type)
        {
            // only parses body of an object
            var result = new Dictionary<string, object>();
            

            return result;
        }
    }
}
