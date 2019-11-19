using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public struct Number
    {
        public Number(string prefix, string digits, Guid id) : this()
        {
            Prefix = prefix;
            Digits = digits;
            Id = id;
        }

        public string Prefix { get; set; }
        public string Digits { get; set; }
        [JsonConvert(typeof(JsonToGuid))] public Guid Id { get; set; }
    }
}
