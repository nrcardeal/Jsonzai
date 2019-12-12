using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    interface Tokens
    {
        char Current { get; }
        void Trim();
        char Pop();
        void Pop(char expected);
        string PopWordFinishedWith(char delimiter);
        string popWordPrimitive();
        bool IsEnd(char curr);
    }
}
