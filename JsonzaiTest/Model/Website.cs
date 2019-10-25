using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class Website
    {
        public Website()
        {

        }
        [JsonConvert(typeof(JsonToUri))] public Uri Uri { get; set; }
    }
}
