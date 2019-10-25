using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class JsonPropertyAttribute : Attribute
    {
        public readonly string PropertyName;
        public JsonPropertyAttribute()
        {

        }
        public JsonPropertyAttribute(string PropertyName)
        {
            this.PropertyName = PropertyName;
        }
        
    }
}
