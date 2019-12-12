using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
	public interface ISetter
    {
        Type Klass { get; }
        object SetValue(object target, object value);
    }
	
    class DummySetterTVTR : ISetter
    {
		public Type Klass { get => typeof(string); }
		
		public object SetValue(object target, object value){
			TV tv = (TV) target;
			tv.Tr = (string) value;
			return tv;
		}
    }
	
	public struct TV
    {
		public string Tr { get; set; }
    }
	
	
}
