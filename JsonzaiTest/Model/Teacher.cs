using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class Teacher : Person
    {
        public Uri GithubUrl { get; set; }
        public DateTime FirstTeachingDay { get; set; }
    }
}
