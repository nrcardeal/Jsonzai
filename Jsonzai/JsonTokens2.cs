using System;
using System.Collections;
using System.IO;


namespace Jsonzai
{
    public class JsonTokens2: Tokens
    {

        public const char OBJECT_OPEN = '{';
        public const char OBJECT_END = '}';
        public const char ARRAY_OPEN = '[';
        public const char ARRAY_END = ']';
        public const char DOUBLE_QUOTES = '"';
        public const char COMMA = ',';
        public const char COLON = ':';

        public StreamReader stream;

        public JsonTokens2(string filename)
        {
            stream = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 2));
        }

        public char Current => (char)stream.Peek();

        public void Trim() {
            while (Current == ' ') stream.Read();
        }

        public char Pop()
        {
            return (char)stream.Read();
        }
        public void Pop(char expected)
        {
            if (Current != expected)
                throw new InvalidOperationException("Expected " + expected + " but found " + Current);
            stream.Read();
        }

        /// <summary>
        /// Consumes all characters until find delimiter and accumulates into a string.
        /// </summary>
        /// <param name="delimiter">May be one of DOUBLE_QUOTES, COLON or COMA</param>
        public string PopWordFinishedWith(char delimiter)
        {
            Trim();
            string acc = "";
            for ( ; Current != delimiter; stream.Read())
            {
                acc += Current;
            }
            stream.Read(); // Discard delimiter
            Trim();
            return acc;
        }
        public string popWordPrimitive()
        {
            Trim();
            string acc = "";
            for( ;  !IsEnd(Current); stream.Read())
            {
                acc += Current;
            }
            Trim();
            return acc;
        }

        public bool IsEnd(char curr)
        {
            return curr == OBJECT_END || curr == ARRAY_END || curr == COMMA;
        }
    }
}
