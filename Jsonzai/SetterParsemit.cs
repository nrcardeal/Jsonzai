using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    class SetterParsemit<T, W> : ISetter2
    {
        public Type Klass => typeof(W);

        public object SetValue(object target, object value)
        {
            ((T)target) = (W);
        }
    }
}
