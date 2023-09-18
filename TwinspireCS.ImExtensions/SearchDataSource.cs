using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace TwinspireCS.ImExtensions
{
    public class SearchDataSource
    {

        ISearchSource _searchSource;
        public ISearchSource SearchSource
        {
            get { return _searchSource; }
        }

        public object Query { get; set; }

        public SearchDataSource(ISearchSource searchSource)
        {
            _searchSource = searchSource;
        }

        /// <summary>
        /// Returns the search source. If the source is derived from DbConnection,
        /// will return as a DbConnection object.
        /// </summary>
        /// <returns></returns>
        public object GetController()
        {
            if (_searchSource == null)
                return null;

            if (_searchSource.GetType().IsSubclassOf(typeof(DbConnection)))
            {
                return (DbConnection)_searchSource;
            }
            else
            {
                return _searchSource;
            }
        }

    }
}
