using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class JsonToUri
    {
        public JsonToUri()
        {
        }
        public static object Parse(string json)
        {
            return new Uri(json);
        }
        public static Uri Parse2(string json)
        {
            return new Uri(json);
        }

    }
}
