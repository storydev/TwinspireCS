using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.ImGui
{
    public class SearchOptions
    {

        private Dictionary<string, OptionData> options;
        private bool noOrdering;

        public bool CanOrderColumns { get => !noOrdering; }

        public IDictionary<string, OptionData> Options { get => options; }

        public SearchOptions()
        {
            options = new Dictionary<string, OptionData>();
        }

        /// <summary>
        /// Set the width of the specified column.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="width">The display width of the given field in search.</param>
        public void SetColumnWidth(string fieldName, float width)
        {
            if (options.ContainsKey(fieldName))
            {
                options[fieldName].Width = width;
            }
            else
            {
                options.Add(fieldName, new OptionData() { Width = width });
            }
        }

        /// <summary>
        /// Setup a relationship between a given field and the field to display from another type.
        /// </summary>
        /// <param name="fieldName">The name of the field to assign the relationship.</param>
        /// <param name="relType">The type that represents the relationship.</param>
        /// <param name="relFieldName">The field used as the display value from the relationship type.</param>
        public void SetRelationship(string fieldName, Type relType, string relFieldName)
        {
            if (options.ContainsKey(fieldName))
            {
                options[fieldName].RelationshipType = relType;
                options[fieldName].RelationshipFieldName = relFieldName;
            }
            else
            {
                options.Add(fieldName, new OptionData() { RelationshipType = relType, RelationshipFieldName = relFieldName });
            }
        }

        /// <summary>
        /// Prevent manual ordering of columns.
        /// </summary>
        public void DisableOrdering()
        {
            noOrdering = true;
        }

    }
}
