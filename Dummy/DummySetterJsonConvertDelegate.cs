using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jsonzai;

namespace Jsonzai.Test.Model
{
    public interface ISetter
    {
        Type Klass { get; }
        object SetValue(object target, object value);
    }

    class DummySetterJsonConvertDelegate: ISetter
    {
        public Type Klass => typeof(Guid);
		
		Func<string, Guid> conv;

        public object SetValue(object target, object value)
        {            
			((Classroom)target).Id = (Guid)JsonToGuid.Parse((string)value);
			return target;
        }
    }
	
	public class JsonToGuid
    {
        public JsonToGuid()
        {
        }
		
        public static Guid Parse(string json)
        {
            return new Guid(json);
        }
    }
	
	public class Classroom
	{
        public Classroom()
        {
        }
        public string Class { get; set; }
        public Guid Id { get; set; }

    }

}