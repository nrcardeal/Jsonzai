using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    class PropertySetterConvert : ISetter
    {
        private PropertyInfo p;
        public Type Klass { get; set; }
        

        public PropertySetterConvert(PropertyInfo prop, Type klass)
        {
            p = prop;
            if (klass.IsArray) klass = klass.GetElementType();
            Klass = klass;
        }

        public void SetValue(object target, object value)
        {
            value = Klass.GetMethod("Parse").Invoke(null, new object[] { value });
            p.SetValue(target, value);
        }
    }
}
