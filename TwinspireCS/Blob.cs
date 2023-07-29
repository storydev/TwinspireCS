using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{

    /// <summary>
    /// A resource containing text data.
    /// </summary>
    public class Blob
    {

        private byte[] data;

        /// <summary>
        /// Get the raw data source.
        /// </summary>
        public byte[] Data
        {
            get => data;
        }

        /// <summary>
        /// Gets a UTF-8 formatted string from the raw data source.
        /// </summary>
        public string UTF8Text
        {
            get
            {
                return Encoding.UTF8.GetString(data);
            }
        }

        /// <summary>
        /// Gets an ASCII formatted string from the raw data source.
        /// </summary>
        public string ASCIIText
        {
            get
            {
                return Encoding.ASCII.GetString(data);
            }
        }

        public Blob()
        {
            
        }

        /// <summary>
        /// Load a Blob from text data.
        /// </summary>
        /// <param name="text">The text to use for this blob.</param>
        /// <param name="encoding">The encoding of the text. If <c>null</c>, the default encoder is used.</param>
        public void LoadFromMemory(string text, Encoding? encoding = null)
        {
            encoding ??= Encoding.Default;
            data = encoding.GetBytes(text);
        }

        /// <summary>
        /// Loads a blob from a generic object, using the specified formatter for reference.
        /// 
        /// Only primitive types, arrays and <c>IEnumerable</c> types are supported.
        /// </summary>
        /// <remarks>
        /// Use <c>LoadFromMemory</c> taking a <c>byte[]</c> if you need something unformatted.
        /// </remarks>
        /// <param name="item">The object to load as a blob.</param>
        /// <param name="formatter">The required formatter to use to format the <c>item</c> into a storage format.</param>
        /// <param name="encoding">(Optional) The specified encoding to use for formatting the resulting text.</param>
        public void LoadFromMemory(object item, IBlobFormatter formatter, Encoding? encoding = null)
        {
            encoding ??= Encoding.Default;

            var typeInfo = item.GetType().GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var genericType = typeInfo.GetGenericArguments()[0].GetType();
                if (genericType == typeof(IBlobObject))
                {
                    var casted = (IEnumerable<IBlobObject>)item;
                    var result = formatter.ForEach(casted);
                    data = encoding.GetBytes(result);
                }
                else if (!IsBasicType(genericType))
                {
                    var casted = (IEnumerable<object>)item;
                    var result = formatter.ForEach(casted);
                    data = encoding.GetBytes(result);
                }
            }
            else if (typeInfo.IsArray)
            {
                var arrayType = typeInfo.GetElementType();
                if (arrayType == typeof(IBlobObject))
                {
                    var casted = (IEnumerable<IBlobObject>)item;
                    var result = formatter.ForEach(casted);
                    data = encoding.GetBytes(result);
                }
                else if (!IsBasicType(arrayType))
                {
                    var casted = (IEnumerable<object>)item;
                    var result = formatter.ForEach(casted);
                    data = encoding.GetBytes(result);
                }
            }
            else if (!IsBasicType(item.GetType()))
            {
                if (item.GetType() == typeof(IBlobObject))
                {
                    var casted = (IBlobObject)item;
                    var result = formatter.FromObject(casted);
                    data = encoding.GetBytes(result);
                }
                else
                {
                    var result = formatter.FromObject(item);
                    data = encoding.GetBytes(result);
                }
            }
        }

        /// <summary>
        /// Loads a blob from raw byte data. No formatting is used.
        /// </summary>
        /// <param name="data"></param>
        public void LoadFromMemory(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Parse the current blob data into a typed object using the given formatter. It is possible for this
        /// function to fail if the source data does not exactly match the structure of the generic type.
        /// 
        /// To guarantee its success, it's recommended to use a type derived from <c>IBlobObject</c>.
        /// </summary>
        /// <remarks>
        /// This function does not process enumerables. Use <c>ToObjectEnumerable</c> for dealing with enumerable types.
        /// </remarks>
        /// <param name="formatter">The formatter to use to create the subject object.</param>
        /// <param name="encoding">The encoder to use for encoding the raw data. If null, the default encoder is used.</param>
        /// <returns></returns>
        public T ToObject<T>(IBlobFormatter formatter, Encoding? encoding = null)
        {
            if (data == null)
                return default(T);
            
            if (typeof(T).IsGenericType || typeof(T).IsArray)
                return default(T);

            encoding ??= Encoding.Default;
            var text = encoding.GetString(data);

            if (typeof(T) == typeof(IBlobObject))
            {
                var result = (IBlobObject)Activator.CreateInstance(typeof(T));
                result.FromString(text);
                return (T)result;
            }
            else
            {
                var result = formatter.ToRawObject(text, typeof(T));
                return (T)result;
            }
        }

        /// <summary>
        /// Parse the current blob data into a typed enumerable object using the given formatter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter to use to create the subject object.</param>
        /// <param name="encoding">The encoder to use for encoding the raw data. If null, the default encoder is used.</param>
        /// <returns></returns>
        public IEnumerable<T> ToObjectEnumerable<T>(IBlobFormatter formatter, Encoding? encoding = null)
        {
            if (data == null)
                return new List<T>();

            encoding ??= Encoding.Default;
            var text = encoding.GetString(data);

            if (typeof(T) == typeof(IBlobObject))
            {
                var result = formatter.ToObjectEnumerable<T>(text);
                return (IEnumerable<T>)result;
            }
            else
            {
                var result = formatter.ToRawObjectEnumerable(text, typeof(T));
                return (IEnumerable<T>)result;
            }
        }



        /// <summary>
        /// Create a blob from text data.
        /// </summary>
        /// <param name="text">The text to convert to a blob.</param>
        /// <returns></returns>
        public static Blob FromString(string text, Encoding? encoding = null)
        {
            var blob = new Blob();
            blob.LoadFromMemory(text, encoding);
            return blob;
        }

        /// <summary>
        /// Transforms a singular object instance to a type <c>IDictionary</c> containing
        /// a list of fields/properties and their current values.
        /// </summary>
        /// <param name="instance">An instance of a class or struct type.</param>
        /// <returns></returns>
        public static IDictionary<string, object> TransformObject(object instance)
        {
            var result = new Dictionary<string, object>();
            var type = instance.GetType();
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsBasicType(field.FieldType))
                {
                    result[field.Name] = field.GetValue(instance);
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsBasicType(property.PropertyType))
                {
                    result[property.Name] = property.GetValue(instance);
                }
            }

            return result;
        }

        /// <summary>
        /// Transforms an object enumerable to a type <c>IDictionary</c> containing
        /// a list of fields/properties and their current values.
        /// 
        /// Unlike <c>TransformObject</c>, the value of each field is an array of
        /// <c>object</c> representing all the values for the respective field from the given
        /// enumerable.
        /// </summary>
        /// <param name="instance">The enumerable object list.</param>
        /// <returns></returns>
        public static IDictionary<string, object[]> TransformObjects(IEnumerable<object> instance)
        {
            var result = new Dictionary<string, object[]>();
            var type = instance.GetType().GetGenericTypeDefinition();
            var objectCount = instance.Count();

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsBasicType(field.FieldType))
                {
                    result[field.Name] = new object[objectCount];
                    for (int i = 0; i < objectCount; i++)
                    {
                        result[field.Name][i] = field.GetValue(instance.ElementAt(i));
                    }
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsBasicType(property.PropertyType))
                {
                    result[property.Name] = new object[objectCount];
                    for (int i = 0; i < objectCount; i++)
                    {
                        result[property.Name][i] = property.GetValue(instance.ElementAt(i));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determine if the given type is a basic type or the generic
        /// type is derived of a basic type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns></returns>
        public static bool IsBasicType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsBasicType(typeInfo.GetGenericArguments()[0]);
            }

            return typeInfo.IsPrimitive
                || typeInfo.IsEnum
                || type.Equals(typeof(string))
                || type.Equals(typeof(decimal))
                || type.Equals(typeof(DateTime));
        }

        private static DefaultBlobFormatter _default;
        /// <summary>
        /// The default formatter for parsing and reading text data.
        /// </summary>
        public static DefaultBlobFormatter Default
        {
            get
            {
                return _default ??= new DefaultBlobFormatter();
            }
        }

    }
}
