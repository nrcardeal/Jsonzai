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
        static Dictionary<string, Delegate> funcs = new Dictionary<string, Delegate>(); 
        static void Cache(Type klass)
        {
            ISetter2 setter;
            JsonPropertyAttribute attr;
            properties.Add(klass, new Dictionary<string, ISetter2>());
            foreach (PropertyInfo prop in klass.GetProperties())
            {
                setter = (ISetter2)Activator.CreateInstance(BuildSetter(klass, prop));
                attr = (JsonPropertyAttribute)prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
                if (attr != null)
                    properties[klass].Add(attr.PropertyName, setter);
                else
                    properties[klass].Add(prop.Name, setter);
            }
            
        }
        public static Type BuildSetter(Type klass, PropertyInfo p)
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
            buildGetTypeProperty(myTypeBuilder, klassProp, p);
            buildSetValueMethod(myTypeBuilder, klass, p);


            // Create the Setter<klass.name><p.name>
            Type t = myTypeBuilder.CreateType();

            // The following line saves the single-module assembly. This
            // requires AssemblyBuilderAccess to include Save. You can now
            // type "ildasm MyDynamicAsm.dll" at the command prompt, and 
            // examine the assembly. You can also write a program that has
            // a reference to the assembly, and use the MyDynamicType type.
            // 
            myAssemblyBuilder.Save(myAssemblyName.Name + ".dll");
            return t;
        }
        static void buildSetValueMethod(TypeBuilder myTypeBuilder, Type klass, PropertyInfo p)
        {
            MethodBuilder setValue = myTypeBuilder.DefineMethod(
                "SetValue",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.HideBySig | MethodAttributes.Virtual,
                CallingConventions.Standard,
                typeof(object), // Return type
                new Type[] { typeof(object), typeof(object) });

            // Add IL to SetValue body
            JsonConvertAttribute convert = (JsonConvertAttribute)p.GetCustomAttribute(typeof(JsonConvertAttribute));
            ILGenerator methodIL = setValue.GetILGenerator();
            
            if (!klass.IsValueType)
            {
                methodIL.Emit(OpCodes.Ldarg_1); // ldarg.1
                methodIL.Emit(OpCodes.Castclass, klass); // castclass  Student
                methodIL.Emit(OpCodes.Ldarg_2); // ldarg.2
                if (convert != null)
                {
                    methodIL.Emit(OpCodes.Castclass, typeof(string));
                    methodIL.Emit(OpCodes.Call, convert.klass.GetMethod("Parse"));
                }
                if (p.PropertyType.IsValueType)
                    methodIL.Emit(OpCodes.Unbox_Any, p.PropertyType);
                else
                    methodIL.Emit(OpCodes.Castclass, p.PropertyType);
                methodIL.Emit(OpCodes.Callvirt, p.GetSetMethod());
                methodIL.Emit(OpCodes.Ldarg_1); // ldarg.1
            }
            else
            {
                LocalBuilder var = methodIL.DeclareLocal(klass);
                methodIL.Emit(OpCodes.Ldarg_1); // ldarg.1
                methodIL.Emit(OpCodes.Unbox_Any, klass);
                methodIL.Emit(OpCodes.Stloc_0);
                methodIL.Emit(OpCodes.Ldloca_S, var);
                methodIL.Emit(OpCodes.Ldarg_2);
                if (convert != null)
                {
                    methodIL.Emit(OpCodes.Castclass, typeof(string));
                    methodIL.Emit(OpCodes.Call, convert.klass.GetMethod("Parse"));
                }
                if (p.PropertyType.IsValueType)
                    methodIL.Emit(OpCodes.Unbox_Any, p.PropertyType);
                else
                    methodIL.Emit(OpCodes.Castclass, p.PropertyType);
                methodIL.Emit(OpCodes.Call, p.GetSetMethod());
                methodIL.Emit(OpCodes.Ldloc_0);
                methodIL.Emit(OpCodes.Box, klass);
            }
            methodIL.Emit(OpCodes.Ret);// ret
        }
        static void buildGetTypeProperty(TypeBuilder myTypeBuilder, Type klass, PropertyInfo p)
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
            getmethodIL.Emit(OpCodes.Ldtoken, klass); //IL_0000: ldtoken Jsonzai.Test.Model.Student
            getmethodIL.Emit(OpCodes.Call, 
                typeof(Type).GetMethod("GetTypeFromHandle", new Type[1] { typeof(RuntimeTypeHandle) }));//IL_0005:  call       class [mscorlib] System.Type[mscorlib] System.Type::GetTypeFromHandle(valuetype[mscorlib] System.RuntimeTypeHandle)
            getmethodIL.Emit(OpCodes.Ret); //IL_000a:  ret        

            getType.SetGetMethod(getKlass);
        }

        private static object FillObject(Tokens tokens, object target)
        {
            Type klass = target.GetType();
            if (!properties.ContainsKey(klass)) Cache(klass);
            while (tokens.Current != JsonTokens.OBJECT_END)
            {
                string propName = tokens.PopWordFinishedWith(JsonTokens.COLON).Replace("\"", "");
                ISetter2 s = properties[klass][propName];
                target = s.SetValue(target, Parse(tokens, s.Klass));

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

        public static IEnumerable<T> SequenceFrom<T>(string filename)
        {
            Tokens tokens = new JsonTokens2(filename);
            tokens.Pop(JsonTokens2.ARRAY_OPEN);
            while (tokens.Current != JsonTokens2.ARRAY_END)
            {
                yield return (T) Parse(tokens, typeof(T));
                if (tokens.Current != JsonTokens2.ARRAY_END)
                {
                    tokens.Pop(JsonTokens2.COMMA);
                    tokens.Trim();
                }
            }
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
        public static void AddConfiguration<T, W>(string propName, Func<String, W> convert)
        {
            ISetter2 setter;
            properties.Add(typeof(T), new Dictionary<string, ISetter2>());
            funcs.Add(propName, convert);
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                PropertyInfo p = typeof(T).GetProperty(propName);
                if (p == prop)
                    setter = new SetterConvertDelegate<W>(p, convert);
                else
                    setter = (ISetter2)Activator.CreateInstance(BuildSetter(typeof(T), prop));
                JsonPropertyAttribute attr = (JsonPropertyAttribute)prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
                if (attr != null)
                    properties[typeof(T)].Add(attr.PropertyName, setter);
                else
                    properties[typeof(T)].Add(prop.Name, setter);
            }
        }
    }
}
