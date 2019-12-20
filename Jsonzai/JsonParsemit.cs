using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai
{
    public class JsonParsemit
    {
        static Dictionary<Type, Dictionary<string, ISetter2>> properties = new Dictionary<Type, Dictionary<string, ISetter2>>();

        /**
         * This method fills the Dictionary properties with the different Properties of the class given
         * and for each property creates a .dll that will have the IL to set the value of the property
         */
        static void Cache(Type klass)
        {
            properties.Add(klass, new Dictionary<string, ISetter2>());
            foreach (PropertyInfo prop in klass.GetProperties())
            {
                ISetter2 setter = (ISetter2)Activator.CreateInstance(BuildSetter(klass, prop, null));
                // Links the setter with the appropriated name
                JsonPropertyAttribute attr = (JsonPropertyAttribute)prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
                if (attr != null)
                    properties[klass].Add(attr.PropertyName, setter);
                else
                    properties[klass].Add(prop.Name, setter);
            }
        }
        /**
         * This method will replace the setter associated with the property name given, by one with the delegate code
         */
        public static void AddConfiguration<T, W>(string propName, Func<String, W> convert)
        {
            if (!properties.ContainsKey(typeof(T)))
                Cache(typeof(T)); // Fills the properties Dictionary with all the properties of T

            // Removes and substitues the property referenced in the parameters
            // and the linked setter for one with a setter done with the convert delegate
            PropertyInfo p = typeof(T).GetProperty(propName);
            JsonPropertyAttribute attr = (JsonPropertyAttribute)p.GetCustomAttribute(typeof(JsonPropertyAttribute));
            if (attr != null)
                propName = attr.PropertyName; // Substitute the name for the name that is associated on the dictionary when filled by the Cache
            properties[typeof(T)].Remove(propName);
            properties[typeof(T)].Add(propName, (ISetter2)Activator.CreateInstance(BuildSetter(typeof(T), p, convert)));
        }
        #region IL Generation code
        /**
         * This method builds the .dll Library to set the value for the property given, 
         * if a Delegate is given, the code will set the value with the code from the Delegate,
         * if the property has an JsonConvertAttribute the value will be set according to its Parse method,
         * if the property has an JsonConvertAttribute and a Delegate is given the Delegate is prioritised
         */
        public static Type BuildSetter(Type klass, PropertyInfo p, Delegate del)
        {
            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = "LibSetter" + klass.Name + p.Name;

            // Define a dynamic assembly in the current application domain.
            AssemblyBuilder myAssemblyBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.RunAndSave);

            // Define a dynamic module in this assembly.
            ModuleBuilder myModuleBuilder = myAssemblyBuilder.
                                            DefineDynamicModule(myAssemblyName.Name, myAssemblyName.Name + ".dll");

            // Define a runtime class with specified name and attributes.
            // <=> class Setter<klass.name><p.Name> : ISetter2
            TypeBuilder myTypeBuilder = myModuleBuilder.DefineType(
                "Setter" + klass.Name + p.Name,
                TypeAttributes.Public,
                typeof(object),
                new Type[] { typeof(ISetter2) });

            Type klassProp = p.PropertyType.IsArray ? p.PropertyType.GetElementType() : p.PropertyType;
            BuildGetTypeProperty(myTypeBuilder, klassProp);
            BuildSetValueMethod(myTypeBuilder, klass, p, del);


            // Create the Setter<klass.name><p.name>
            Type t = myTypeBuilder.CreateType();

            // The following line saves the single-module assembly. This
            // requires AssemblyBuilderAccess to include Save. You can now
            // type "ildasm MyDynamicAsm.dll" at the command prompt, and 
            // examine the assembly. You can also write a program that has
            // a reference to the assembly, and use the MyDynamicType type.
            myAssemblyBuilder.Save(myAssemblyName.Name + ".dll");
            return t;
        }
        static void BuildSetValueMethod(TypeBuilder myTypeBuilder, Type klass, PropertyInfo p, Delegate del)
        {
            //Creates the builder for the method 
            MethodBuilder setValue = myTypeBuilder.DefineMethod(
                "SetValue",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.HideBySig | MethodAttributes.Virtual,
                CallingConventions.Standard,
                typeof(object), // Return type
                new Type[] { typeof(object), typeof(object) });

            if (!klass.IsValueType)
                ReferenceTypeClassILEmit(klass, del, p, setValue);
            else
                ValueTypeClassILEmit(klass, del, p, setValue);
        }

        static void ReferenceTypeClassILEmit(Type klass, Delegate del, PropertyInfo p, MethodBuilder setValue)
        {
            //Emits the body for a reference type class
            ILGenerator methodIL = setValue.GetILGenerator();
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Castclass, klass);
            methodIL.Emit(OpCodes.Ldarg_2);
            PrepareValue(methodIL, del, p);
            methodIL.Emit(OpCodes.Callvirt, p.GetSetMethod());
            methodIL.Emit(OpCodes.Ldarg_1);
            methodIL.Emit(OpCodes.Ret);
        }
        static void ValueTypeClassILEmit(Type klass, Delegate del, PropertyInfo p, MethodBuilder setValue)
        {
            //Emits th body for a value type class
            ILGenerator methodIL = setValue.GetILGenerator();
            LocalBuilder var = methodIL.DeclareLocal(klass);
            methodIL.Emit(OpCodes.Ldarg_1); 
            methodIL.Emit(OpCodes.Unbox_Any, klass);
            methodIL.Emit(OpCodes.Stloc_0);
            methodIL.Emit(OpCodes.Ldloca_S, var);
            methodIL.Emit(OpCodes.Ldarg_2);
            PrepareValue(methodIL, del, p);
            methodIL.Emit(OpCodes.Call, p.GetSetMethod());
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Box, klass);
            methodIL.Emit(OpCodes.Ret);// ret
        }
        /**
         * Generates the IL code to prepare the value to be set in the property
         * prioritising the Delegate code over the JsonConvertAtribute
         */
        static void PrepareValue(ILGenerator methodIL, Delegate del, PropertyInfo p)
        {
            if (del != null)
            {
                methodIL.Emit(OpCodes.Castclass, typeof(string));
                methodIL.Emit(OpCodes.Call, del.Method);
            }
            else
            {
                JsonConvertAttribute convert = (JsonConvertAttribute)p.GetCustomAttribute(typeof(JsonConvertAttribute));
                if (convert != null)
                {
                    methodIL.Emit(OpCodes.Castclass, typeof(string));
                    methodIL.Emit(OpCodes.Call, convert.klass.GetMethod("Parse"));
                }
                if (p.PropertyType.IsValueType)
                    methodIL.Emit(OpCodes.Unbox_Any, p.PropertyType);
                else
                    methodIL.Emit(OpCodes.Castclass, p.PropertyType);
            }
        }

        /**
         * This method builds in the .dll a Property of the type Type with the value given
         */
        static void BuildGetTypeProperty(TypeBuilder myTypeBuilder, Type klass)
        {
            PropertyBuilder getType = myTypeBuilder.DefineProperty(
                "Type", PropertyAttributes.HasDefault, typeof(Type), new Type[0]);
            MethodBuilder getKlass = myTypeBuilder.DefineMethod(
                "get_Klass",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.HideBySig | MethodAttributes.Virtual,
                CallingConventions.Standard,
                typeof(Type),
                new Type[0]);

            ILGenerator getmethodIL = getKlass.GetILGenerator();

            getmethodIL.Emit(OpCodes.Ldtoken, klass);
            getmethodIL.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new Type[1] { typeof(RuntimeTypeHandle) }));
            getmethodIL.Emit(OpCodes.Ret);       

            getType.SetGetMethod(getKlass);
        }
        #endregion
        private static object FillObject(Tokens tokens, object target)
        {
            Type klass = target.GetType();
            if (!properties.ContainsKey(klass)) Cache(klass); // Only fills the dictionary one time
            while (tokens.Current != JsonTokens.OBJECT_END)
            {
                string propName = tokens.PopWordFinishedWith(JsonTokens.COLON).Replace("\"", "");
                ISetter2 s = properties[klass][propName];
                target = s.SetValue(target, Parse(tokens, s.Klass)); // Set the value to the target, the value is the result from the Parse method

                tokens.Trim();
                if (tokens.Current != JsonTokens.OBJECT_END)
                    tokens.Pop(JsonTokens.COMMA);

            }
            tokens.Pop(JsonTokens.OBJECT_END); // Discard bracket } OBJECT_END
            return target;
        }

        public static T Parse<T>(String source)
        {
            if (typeof(T).IsArray)
                return (T)Parse(new JsonTokens(source), typeof(T).GetElementType());
            return (T)Parse(new JsonTokens(source), typeof(T));
        }       
        public static IEnumerable<T> SequenceFrom<T>(string filename)
        {
            using (JsonTokens2 tokens = new JsonTokens2(filename))
            {
                tokens.Pop(JsonTokens2.ARRAY_OPEN);
                while (tokens.Current != JsonTokens2.ARRAY_END)
                {
                    yield return (T)Parse(tokens, typeof(T)); // Return the results by Lazy (only returns one element when is needed)
                    if (tokens.Current != JsonTokens2.ARRAY_END)
                    {
                        tokens.Pop(JsonTokens2.COMMA);
                        tokens.Trim();
                    }
                }
            }
        }
        

        #region Code Disponibilized By The Teacher
        static object Parse(Tokens tokens, Type klass)
        {
            switch (tokens.Current)
            {
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
        private static string ParseString(Tokens tokens)
        {
            tokens.Pop(JsonTokens.DOUBLE_QUOTES); // Discard double quotes "
            return tokens.PopWordFinishedWith(JsonTokens.DOUBLE_QUOTES);
        }
        private static object ParsePrimitive(Tokens tokens, Type klass)
        {
            string word = tokens.popWordPrimitive();
            if (!klass.IsPrimitive || typeof(string).IsAssignableFrom(klass))
                if (word.ToLower().Equals("null"))
                    return null;
                else
                    throw new InvalidOperationException("Looking for a primitive but requires instance of " + klass);
            return klass.GetMethod("Parse", new Type[] { typeof(String), typeof(IFormatProvider) }).Invoke(null, new object[] { word, CultureInfo.InvariantCulture });
        }
        private static object ParseObject(Tokens tokens, Type klass)
        {
            tokens.Pop(JsonTokens.OBJECT_OPEN); // Discard bracket { OBJECT_OPEN
            object target = Activator.CreateInstance(klass);
            return FillObject(tokens, target);
        }
        private static object ParseArray(Tokens tokens, Type klass)
        {
            ArrayList list = new ArrayList();
            tokens.Pop(JsonTokens.ARRAY_OPEN); // Discard square brackets [ ARRAY_OPEN
            while (tokens.Current != JsonTokens.ARRAY_END)
            {
                list.Add(Parse(tokens, klass));
                if (tokens.Current != JsonTokens.ARRAY_END)
                {
                    tokens.Pop(JsonTokens.COMMA);
                    tokens.Trim();
                }

            }
            tokens.Pop(JsonTokens.ARRAY_END); // Discard square bracket ] ARRAY_END
            return list.ToArray(klass);
        }
        #endregion
    }
}