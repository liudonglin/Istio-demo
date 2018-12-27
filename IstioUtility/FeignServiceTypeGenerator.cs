using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IstioUtility
{
    public class FeignServiceTypeGenerator
    {
        private static readonly object typeBuildLock = new object();
        private static ModuleBuilder moduleBuilder;
        public static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (moduleBuilder == null)
                {
                    var assemblyName = new AssemblyName { Name = "FeignServiceTypes" };
                    var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                    moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
                }
                return moduleBuilder;
            }
        }

        public static Type GetOrCreateFeignServiceType(FeignServiceInfo feignServiceInfo)
        {
            string genericTypeName = $"FeignServiceTypes_{feignServiceInfo.InterfaceType.FullName}_Implement";
            Type genericType = null;
            lock (typeBuildLock)
            {
                genericType = ModuleBuilder.GetType(genericTypeName);
                if (genericType == null)
                {
                    genericType = CreateFeignServiceType(feignServiceInfo, genericTypeName);
                }
            }

            return genericType;
        }

        private static Type CreateFeignServiceType(FeignServiceInfo feignServiceInfo, string genericTypeName)
        {
            TypeBuilder typeBuilder = ModuleBuilder.DefineType(
             genericTypeName,
             TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
             typeof(object),
             new Type[] { feignServiceInfo.InterfaceType });

            CreateDefaultConstructor(typeBuilder);

            foreach (var feignMethod in feignServiceInfo.MethodInfos)
            {
                CreateImplementMethod(typeBuilder, feignServiceInfo, feignMethod);
            }

            return typeBuilder.CreateTypeInfo();
        }

        private static void CreateDefaultConstructor(TypeBuilder typeBuilder)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                attributes: MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                callingConvention: CallingConventions.Standard | CallingConventions.HasThis,
                parameterTypes: Type.EmptyTypes
            );

            var constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            constructorIlGenerator.Emit(OpCodes.Call, typeof(object).GetConstructors().Single());

            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        private static void CreateImplementMethod(TypeBuilder typeBuilder, FeignServiceInfo feignServiceInfo, FeignMethodInfo feignMethod)
        {

            var paramters = new Type[feignMethod.ParamInfos.Count];

            for (int i = 0; i < feignMethod.ParamInfos.Count; i++)
            {
                paramters[i] = feignMethod.ParamInfos[i].ParamType;
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
            feignMethod.MethodType.Name,
            MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual,
            CallingConventions.Standard | CallingConventions.HasThis,
            feignMethod.MethodType.ReturnType,
            paramters);

            ILGenerator methIL = methodBuilder.GetILGenerator();
            LocalBuilder _local_dictionary = methIL.DeclareLocal(DictionaryType);
            methIL.Emit(OpCodes.Nop);
            /* Dictionary<string, object> paramters = new Dictionary<string, object>(); */
            methIL.Emit(OpCodes.Newobj, DictionaryConstructor);
            methIL.Emit(OpCodes.Stloc,_local_dictionary);

            /* paramters.Add("argName",argValue); */
            for (int i = 0; i < feignMethod.ParamInfos.Count; i++)
            {
                var paramInfo = feignMethod.ParamInfos[i];
                methIL.Emit(OpCodes.Ldloc,_local_dictionary);
                methIL.Emit(OpCodes.Ldstr, string.IsNullOrWhiteSpace(paramInfo.Alias) ? paramInfo.ParamName : paramInfo.Alias);
                methIL.Emit(OpCodes.Ldarg,i+1);
                if(paramInfo.ParamType.IsValueType)
                {
                    methIL.Emit(OpCodes.Box,paramInfo.ParamType);
                }
                methIL.Emit(OpCodes.Callvirt, DictionaryAddMethod);
                methIL.Emit(OpCodes.Nop);
            }

            var url = CreateCompletUrl(feignServiceInfo.ServiceHost, feignMethod.RequestPath);
            /* HttpClientHelper.PostData("http://localhost:5001", paramters); */
            methIL.Emit(OpCodes.Ldstr, url);
            methIL.Emit(OpCodes.Ldloc, _local_dictionary);
            methIL.Emit(OpCodes.Call, PostDataMethod);

            if (feignMethod.MethodType.ReturnType == VoidType)
            {
                methIL.Emit(OpCodes.Pop);
                methIL.Emit(OpCodes.Ret);
            }
            else if(feignMethod.MethodType.ReturnType == StringType)
            {
                methIL.Emit(OpCodes.Ret);
            }
            else
            {
                LocalBuilder _local_result_str = methIL.DeclareLocal(StringType);
                methIL.Emit(OpCodes.Stloc, _local_result_str);

                var label_EndTry = methIL.DefineLabel();

                /* try begain */
                methIL.BeginExceptionBlock();
                methIL.Emit(OpCodes.Ldloc, _local_result_str);

                LocalBuilder _local_result = methIL.DeclareLocal(feignMethod.MethodType.ReturnType);

                if (IsSimpleType(feignMethod.MethodType.ReturnType))
                {
                    methIL.Emit(OpCodes.Call, feignMethod.MethodType.ReturnType.GetMethod("Parse", new Type[] { StringType }));
                    methIL.Emit(OpCodes.Stloc, _local_result);
                    methIL.Emit(OpCodes.Leave_S, label_EndTry);
                }
                else
                {
                    /* if  _local_result_str = null || _local_result_str = "" return null ; else return parse value */
                    var label_StringEqualsTrue = methIL.DefineLabel();
                    var label_NullEqualsBegain = methIL.DefineLabel();
                    var label_ElseBegin = methIL.DefineLabel();

                    methIL.Emit(OpCodes.Brfalse_S, label_StringEqualsTrue);
                    methIL.Emit(OpCodes.Ldloc, _local_result_str);
                    methIL.Emit(OpCodes.Ldstr, "");
                    methIL.Emit(OpCodes.Call, StringType.GetMethod("Equals", new Type[] { StringType, StringType }));
                    methIL.Emit(OpCodes.Br_S, label_NullEqualsBegain);
                    methIL.MarkLabel(label_StringEqualsTrue);
                    methIL.Emit(OpCodes.Ldc_I4, 1);
                    LocalBuilder _local_str_equals_flag = methIL.DeclareLocal(IntType);
                    methIL.MarkLabel(label_NullEqualsBegain);
                    methIL.Emit(OpCodes.Stloc, _local_str_equals_flag);
                    methIL.Emit(OpCodes.Ldloc, _local_str_equals_flag);
                    methIL.Emit(OpCodes.Brfalse_S, label_ElseBegin);
                    methIL.Emit(OpCodes.Nop);
                    methIL.Emit(OpCodes.Ldnull);
                    LocalBuilder _local_null_equals_flag = methIL.DeclareLocal(IntType);
                    methIL.Emit(OpCodes.Stloc, _local_null_equals_flag);
                    methIL.Emit(OpCodes.Leave_S, label_EndTry);

                    methIL.MarkLabel(label_ElseBegin);
                    methIL.Emit(OpCodes.Nop);
                    methIL.Emit(OpCodes.Ldloc, _local_result_str);

                    if(IsNullableType(feignMethod.MethodType.ReturnType)
                    && IsSimpleType(feignMethod.MethodType.ReturnType.GetGenericArguments()[0]))
                    {
                        var returnSimpleType = feignMethod.MethodType.ReturnType.GetGenericArguments()[0];
                        methIL.Emit(OpCodes.Call, returnSimpleType.GetMethod("Parse", new Type[] { StringType }));
                        methIL.Emit(OpCodes.Newobj, typeof(Nullable<>).MakeGenericType(returnSimpleType).GetConstructor(new Type[] { returnSimpleType }));
                    }
                    else
                    {
                        var deserializeObjectGenericMethod = DeserializeObjectMethod.MakeGenericMethod(feignMethod.MethodType.ReturnType);
                        methIL.Emit(OpCodes.Call, deserializeObjectGenericMethod);
                    }

                    methIL.Emit(OpCodes.Stloc, _local_result);
                }

                /* catch begain */
                methIL.BeginCatchBlock(ExceptionType);
                LocalBuilder _local_exception = methIL.DeclareLocal(ExceptionType);
                methIL.Emit(OpCodes.Stloc, _local_exception);
                methIL.Emit(OpCodes.Nop);
                methIL.Emit(OpCodes.Ldstr, "返回结果类型转化失败，");
                methIL.Emit(OpCodes.Ldloc, _local_result_str);
                methIL.Emit(OpCodes.Ldstr, $"不是有效的{feignMethod.MethodType.ReturnType.FullName}的值!");
                methIL.Emit(OpCodes.Call, StringType.GetMethod("Concat", new Type[] { StringType, StringType, StringType }));
                methIL.Emit(OpCodes.Newobj, ExceptionType.GetConstructor(new Type[] { StringType }));
                methIL.Emit(OpCodes.Throw);
                methIL.EndExceptionBlock();
                methIL.MarkLabel(label_EndTry);
                methIL.Emit(OpCodes.Ldloc, _local_result);
                methIL.Emit(OpCodes.Ret);
            }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static bool IsSimpleType(Type type)
        {
            TypeCode typeCode = Type.GetTypeCode(type);

            return typeCode == TypeCode.Char
                 || typeCode == TypeCode.SByte
                 || typeCode == TypeCode.Byte
                 || typeCode == TypeCode.Int16
                 || typeCode == TypeCode.Int32
                 || typeCode == TypeCode.Int64
                 || typeCode == TypeCode.UInt16
                 || typeCode == TypeCode.UInt32
                 || typeCode == TypeCode.UInt64
                 || typeCode == TypeCode.Boolean
                 || typeCode == TypeCode.Decimal
                 || typeCode == TypeCode.Double
                 || typeCode == TypeCode.Single
                 || typeCode == TypeCode.DateTime;
        }

        private static string CreateCompletUrl(string serviceHost, string requestPath)
        {
            return serviceHost.TrimEnd('/') + "/" + requestPath.TrimStart('/');
        }

        private static Type _dictionaryType;
        public static Type DictionaryType
        {
            get
            {
                if(_dictionaryType==null)
                {
                    _dictionaryType = typeof(System.Collections.Generic.Dictionary<string,object>);
                }
                return _dictionaryType;
            }
        }
        
        private static MethodInfo _dictionaryAddMethod;
        public static MethodInfo DictionaryAddMethod
        {
            get
            {
                if (_dictionaryAddMethod == null)
                {
                    _dictionaryAddMethod = DictionaryType.GetMethod("Add");
                }
                return _dictionaryAddMethod;
            }
        }

        private static ConstructorInfo _dictionaryConstructor;
        public static ConstructorInfo DictionaryConstructor
        {
            get
            {
                if(_dictionaryConstructor==null)
                {
                    _dictionaryConstructor = DictionaryType.GetConstructor(Type.EmptyTypes);
                }
                return _dictionaryConstructor;
            }
        }

        private static Type _voidType;
        private static Type VoidType
        {
            get
            {
                if(_voidType==null)
                {
                    _voidType = typeof(void);
                }
                return _voidType;
            }
        }

        private static Type _stringType;
        public static Type StringType
        {
            get
            {
                if (_stringType == null)
                {
                    _stringType = typeof(string);
                }
                return _stringType;
            }
        }

        private static Type _exceptionType;
        public static Type ExceptionType
        {
            get
            {
                if (_exceptionType == null)
                {
                    _exceptionType = typeof(Exception);
                }
                return _exceptionType;
            }
        }

        private static Type _intType;
        public static Type IntType
        {
            get
            {
                if (_intType == null)
                {
                    _intType = typeof(int);
                }
                return _intType;
            }
        }

        private static MethodInfo _deserializeObjectMethod;
        public static MethodInfo DeserializeObjectMethod
        {
            get
            {
                if (_deserializeObjectMethod == null)
                {
                    _deserializeObjectMethod = typeof(HttpClientHelper).GetMethod("DeserializeObject");
                }
                return _deserializeObjectMethod;
            }
        }

        private static MethodInfo _postDataMethod;
        public static MethodInfo PostDataMethod
        {
            get
            {
                if(_postDataMethod==null)
                {
                    _postDataMethod = typeof(HttpClientHelper).GetMethod("PostData");
                }
                return _postDataMethod;
            }
        }

        

    }
}