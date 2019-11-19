using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    class PropertySetter : ISetter
    {
        PropertyInfo p;
        public Type Klass { get; set; }
        public PropertySetter(PropertyInfo p)
        {
            this.p = p;
            Klass = p.PropertyType;
            if (Klass.IsArray) Klass = Klass.GetElementType();
        }
        public void SetValue(object target, object value)
        {
            p.SetValue(target, value);
        }

    }
}
