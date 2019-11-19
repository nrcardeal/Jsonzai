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

    class DummySetterProperty : ISetter
    {
        public Type Klass { get => typeof(JsonToDateTime); }

        public object SetValue(object target, object value)
        {
            
			((Project)target).DueDate = (DateTime)JsonToDateTime.Parse((string)value);
			return target;
        }
    }
	public class JsonToDateTime
    {
        public JsonToDateTime(){}
        public static object Parse(string json){
            String[] arr = json.Split('-', ':', 'T');
            return new DateTime(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), int.Parse(arr[3]), int.Parse(arr[4]), int.Parse(arr[5]));
        }
    }
	public class Project
    {
        public Project(){}
		public DateTime DueDate { get; set; }
    }

}