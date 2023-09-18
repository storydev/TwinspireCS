using System.Reflection;

namespace TwinspireCS.ImExtensions
{
    public class SearchConfig
    {

        public Type InstanceType;
        public string[] FieldNames;
        public float[] FieldWidths;
        
        public SearchConfig(Type instanceType)
        {
            InstanceType = instanceType;

            if (InstanceType == null)
            {
                throw new ArgumentNullException("instanceType cannot be null");
            }

            var fields = InstanceType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            FieldNames = new string[fields.Length];
            FieldWidths = new float[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                FieldNames[i] = fields[i].FieldType.Name;
                var fieldWidthAttr = fields[i].GetCustomAttribute<SearchFieldWidth>();
                if (fieldWidthAttr != null)
                {
                    FieldWidths[i] = fieldWidthAttr.Width;
                }
                else
                {
                    FieldWidths[i] = 120f;
                }
            }
        }

    }
}