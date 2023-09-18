using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.ImExtensions
{
    public class SearchResults
    {

        private SearchConfig _config;

        public string[][] Data;

        public SearchResults(SearchConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Asynchronously obtain data from a supplied source. The subject data can be of a type array, List or SearchDataSource.
        /// For large datasets, it is recommended to use the SearchDataSource type.
        /// </summary>
        /// <param name="data"></param>
        public async void NeedsDataAsync(object data)
        {
            await Task.Run(() => NeedsData(data));
        }

        /// <summary>
        /// Synchronously obtain data from a supplied source. The subject data can be of a type array, List or SearchDataSource.
        /// For large datasets, it is recommended to use the SearchDataSource type.
        /// </summary>
        /// <param name="data"></param>
        public void NeedsData(object data)
        {
            var type = data.GetType();
            if (type.IsArray && type.GetElementType() == _config.InstanceType)
            {
                var instances = (object[])data;
                Data = new string[instances.Length][];
                int index = 0;
                foreach (var instance in instances)
                {
                    Data[index] = new string[_config.FieldNames.Length];
                    for (int i = 0; i < _config.FieldNames.Length; i++)
                    {
                        var field = _config.FieldNames[i];
                        Data[index][i] = _config.InstanceType.GetField(field).GetValue(instance).ToString();
                    }
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == _config.InstanceType)
            {
                var instances = (IEnumerable<object>)data;
                Data = new string[instances.Count()][];
                int index = 0;
                foreach (var instance in instances)
                {
                    Data[index] = new string[_config.FieldNames.Length];
                    for (int i = 0; i < _config.FieldNames.Length; i++)
                    {
                        var field = _config.FieldNames[i];
                        Data[index][i] = _config.InstanceType.GetField(field).GetValue(instance).ToString();
                    }
                }
            }
            else if (type == typeof(SearchDataSource))
            {
                var casted = (SearchDataSource)data;
                var results = casted.SearchSource.Search(casted.Query);
                NeedsData(results);
            }
        }

    }
}
