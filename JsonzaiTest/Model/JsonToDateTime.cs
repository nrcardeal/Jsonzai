using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    class JsonToDateTime
    {
        JsonToDateTime()
        {

        }
        public static object Parse(string json)
        {
            String[] arr = json.Split('-', ':', 'T');
            return new DateTime(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), int.Parse(arr[3]), int.Parse(arr[4]), int.Parse(arr[5]));
        }
    }
}
