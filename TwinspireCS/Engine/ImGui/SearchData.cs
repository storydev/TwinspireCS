using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.ImGui
{
    public class SearchData
    {

        public string[] fieldNames;
        public float[] fieldWidths;
        public Type[] fieldTypes;
        public Type[] fieldRelationship;
        public string[] fieldRelationshipFieldName;
        public object[] fieldFilters;

        public SearchData()
        {
            fieldNames = Array.Empty<string>();
            fieldWidths = Array.Empty<float>();
            fieldTypes = Array.Empty<Type>();
            fieldRelationship = Array.Empty<Type>();
            fieldRelationshipFieldName = Array.Empty<string>();
            fieldFilters = Array.Empty<object>();
        }


    }

    public class OptionData
    {

        public float Width;
        public Type? RelationshipType;
        public string RelationshipFieldName;

        public OptionData()
        {
            RelationshipFieldName = string.Empty;
            Width = 150f;
        }

    }
}
