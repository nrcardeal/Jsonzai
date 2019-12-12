using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    class SetterConvertDelegate<T> : ISetter
    {
        PropertyInfo prop;
        public Type Klass => typeof(T);
        Func<string, T> conv;
        public SetterConvertDelegate(PropertyInfo prop, Func<string, T> convert)
        {
            conv = convert;
            this.prop = prop;
        }

        public void SetValue(object target, object value)
        {
            prop.SetValue(target, conv((string)value));
        }
    }
}
