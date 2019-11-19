using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Jsonzai
{
    public class JsonParser
	{
		static Dictionary<Type,Dictionary<string,ISetter>> properties = new Dictionary<Type,Dictionary<string,ISetter>>();

        static void Cache(Type klass)
		{
            ISetter setter;
            properties.Add(klass, new Dictionary<string, ISetter>());
            foreach (PropertyInfo prop in klass.GetProperties()) 
            {
                JsonConvertAttribute convert = (JsonConvertAttribute)prop.GetCustomAttribute(typeof(JsonConvertAttribute));
                if (convert != null)
                    setter = new PropertySetterConvert(prop, convert.klass);
                else
                    setter = new PropertySetter(prop);
                JsonPropertyAttribute attr = (JsonPropertyAttribute)prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
                if (attr != null)           
                    properties[klass].Add(attr.PropertyName, setter);
                else
                    properties[klass].Add(prop.Name, setter);
            }
        }

		public static object Parse(String source, Type klass)
		{
			return Parse(new JsonTokens(source), klass);
		}

		static object Parse(JsonTokens tokens, Type klass) {
			switch (tokens.Current) {
				case JsonTokens.OBJECT_OPEN:
					return ParseObject(tokens, klass);
				case JsonTokens.ARRAY_OPEN:
					return ParseArray(tokens, klass);
				case JsonTokens.DOUBLE_QUOTES:
					return ParseString(tokens);
				default:
					return ParsePrimitive(tokens, klass);
			}
		}

		private static string ParseString(JsonTokens tokens)
		{
			tokens.Pop(JsonTokens.DOUBLE_QUOTES); // Discard double quotes "
			return tokens.PopWordFinishedWith(JsonTokens.DOUBLE_QUOTES);
		}

        private static object ParsePrimitive(JsonTokens tokens, Type klass)
        {
            string word = tokens.popWordPrimitive();
            if (!klass.IsPrimitive || typeof(string).IsAssignableFrom(klass))
				if (word.ToLower().Equals("null"))
					return null;
				else
					throw new InvalidOperationException("Looking for a primitive but requires instance of " + klass);
			return klass.GetMethod("Parse", new Type[] { typeof(String), typeof(IFormatProvider) }).Invoke(null, new object[] { word, CultureInfo.InvariantCulture });
		}

		private static object ParseObject(JsonTokens tokens, Type klass)
		{
			tokens.Pop(JsonTokens.OBJECT_OPEN); // Discard bracket { OBJECT_OPEN
			object target = Activator.CreateInstance(klass);
			return FillObject(tokens, target);
		}

        private static object FillObject(JsonTokens tokens, object target)
        {
            Type klass = target.GetType();
            if (!properties.ContainsKey(klass)) Cache(klass); 
            while (tokens.Current != JsonTokens.OBJECT_END)
            {                
                string propName = tokens.PopWordFinishedWith(JsonTokens.COLON).Replace("\"","");
                ISetter s = properties[klass][propName];
                s.SetValue(target, Parse(tokens, s.Klass));
                     
                tokens.Trim();
                if (tokens.Current != JsonTokens.OBJECT_END)
                    tokens.Pop(JsonTokens.COMMA);
                
            }
            tokens.Pop(JsonTokens.OBJECT_END); // Discard bracket } OBJECT_END
            return target;
        }

		private static object ParseArray(JsonTokens tokens, Type klass)
		{
			ArrayList list = new ArrayList();
			tokens.Pop(JsonTokens.ARRAY_OPEN); // Discard square brackets [ ARRAY_OPEN
			while (tokens.Current != JsonTokens.ARRAY_END)
			{
				list.Add(Parse(tokens,klass));
				if (tokens.Current != JsonTokens.ARRAY_END)
				{
					tokens.Pop(JsonTokens.COMMA);
					tokens.Trim();
				}

			}
			tokens.Pop(JsonTokens.ARRAY_END); // Discard square bracket ] ARRAY_END
			return list.ToArray(klass);
		}
	}
}
