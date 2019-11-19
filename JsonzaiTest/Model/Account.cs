using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class Account
    {
        public Double Balance { get; set; }
        public Double[] Transactions { get; set; }

        public Number Iban { get; set; }
    }
}
