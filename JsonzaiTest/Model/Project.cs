using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class Project
    {
        public Project()
        {

        }
        [JsonConvert(typeof(JsonToDateTime))] public DateTime DueDate { get; set; }

        [JsonProperty("student_isel")] public Student Student { get; set; }
    }
}
