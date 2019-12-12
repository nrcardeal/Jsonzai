using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class JsonToGuid
    {
        public JsonToGuid()
        {
        }
        public static object Parse(string json)
        {
            return new Guid(json);
        }
        public static Guid Parse2(string json)
        {
            return new Guid(json);
        }
    }
}
