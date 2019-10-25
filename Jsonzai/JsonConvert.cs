using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class JsonConvertAttribute : Attribute
    {
        public JsonConvertAttribute()
        {

        }
        public readonly Type klass;
        public JsonConvertAttribute(Type klass)
        {
            this.klass = klass;
        }

    }
}
