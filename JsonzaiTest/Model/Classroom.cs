using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class Classroom
    {
        public Classroom()
        {
        }
        public string Class { get; set; }
        public Student[] Student { get; set; }
        //[JsonConvert(typeof(JsonToGuid))]
        public Guid Id { get; set; }

    }
}
