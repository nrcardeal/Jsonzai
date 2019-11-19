using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    public interface ISetter2
    {
        Type Klass { get; }
        object SetValue(object target, object value);
    }
}
