using System;
using System.Collections;
using System.IO;


namespace Jsonzai
{
    public class JsonTokens2: Tokens, IDisposable
    {

        public const char OBJECT_OPEN = '{';
        public const char OBJECT_END = '}';
        public const char ARRAY_OPEN = '[';
        public const char ARRAY_END = ']';
        public const char DOUBLE_QUOTES = '"';
        public const char COMMA = ',';
        public const char COLON = ':';

        public FileStream stream;
        private char peek;

        public JsonTokens2(string filename) 
        {
            stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 2);
            peek = (char)stream.ReadByte();
        }

        public char Current => peek;

        public void Trim() {
            while (Current == ' ') MoveNext();
        }
        public bool MoveNext()
        {
            peek = (char)stream.ReadByte();
            return peek != 0;
        }
        public char Pop()
        {
            MoveNext();
            return Current;
        }
        public void Pop(char expected)
        {
            if (Current != expected)
                throw new InvalidOperationException("Expected " + expected + " but found " + Current);
            MoveNext();
        }

        /// <summary>
        /// Consumes all characters until find delimiter and accumulates into a string.
        /// </summary>
        /// <param name="delimiter">May be one of DOUBLE_QUOTES, COLON or COMA</param>
        public string PopWordFinishedWith(char delimiter)
        {
            Trim();
            string acc = "";
            for ( ; Current != delimiter; MoveNext())
            {
                acc += Current;
            }
            MoveNext(); // Discard delimiter
            Trim();
            return acc;
        }
        public string popWordPrimitive()
        {
            Trim();
            string acc = "";
            for( ;  !IsEnd(Current); MoveNext())
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

        public void Dispose()
        {
            stream.Close();
        }
    }
}
