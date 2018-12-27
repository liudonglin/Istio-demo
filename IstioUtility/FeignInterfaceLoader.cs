using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IstioUtility.Attributes;

namespace IstioUtility
{
    public class FeignInterfaceLoader
    {
        private static Type feignServiceAttrType = typeof(FeignServiceAttribute);
        private static Type feignMethodAttribute = typeof(FeignMethodAttribute);
        private static Type feignParamAttribute = typeof(FeignParamAttribute);

        public static Dictionary<Type,FeignServiceInfo> Load(string[] assemblyNames)
        {
            Dictionary<Type,FeignServiceInfo> result = new Dictionary<Type, FeignServiceInfo> ();

            if (assemblyNames == null) return result;

            foreach (var assemblyName in assemblyNames)
            {
                Assembly myAssembly = Assembly.Load(assemblyName);

                if (myAssembly == null) continue;

                var interfaceTypes = myAssembly.GetTypes().Where(x => x.IsInterface && x.IsPublic);
                foreach (var interfaceType in interfaceTypes)
                {
                    FeignServiceInfo info = AnalysisFeignServiceInfo(interfaceType);
                    if(info==null) break;
                    result.Add(interfaceType,info);
                }
            }
            return result;
        }

        private static FeignServiceInfo AnalysisFeignServiceInfo(Type interfaceType)
        {
            var feignServiceAttr = interfaceType.GetCustomAttribute(feignServiceAttrType) as FeignServiceAttribute;
            if(feignServiceAttr==null) return null;

            FeignServiceInfo result = new FeignServiceInfo (feignServiceAttr.ServiceHost,interfaceType);

            var methodinfos = interfaceType.GetMethods();
            foreach(var methodinfo in methodinfos)
            {
                FeignMethodInfo feignMethod = AnalysisFeignMethodInfo(methodinfo);
                result.MethodInfos.Add(feignMethod);
            }

            return result;
        }

        private static FeignMethodInfo AnalysisFeignMethodInfo(MethodInfo methodinfo)
        {
            var feignMethodAttr = methodinfo.GetCustomAttribute(feignMethodAttribute) as FeignMethodAttribute;
            if(feignMethodAttr==null) 
            throw new Exception($"The Method {methodinfo.DeclaringType.Name}.{methodinfo.Name} In A Feign Interface Must Have FeignMethodAttribute");
            
            FeignMethodInfo result = new FeignMethodInfo
            (feignMethodAttr.RequestPath
            ,feignMethodAttr.HttpMethod
            ,methodinfo);

            var pramainfos = methodinfo.GetParameters();
            foreach(var pramainfo in pramainfos)
            {
                FeignParamInfo feignParam= AnalysisFeignParamInfo(pramainfo);
                result.ParamInfos.Add(feignParam);
            }

            return result;
        }

        private static FeignParamInfo AnalysisFeignParamInfo(ParameterInfo paramInfo)
        {
            var feignMethodAttr = paramInfo.GetCustomAttribute(feignParamAttribute) as FeignParamAttribute;
            var alias = feignMethodAttr==null?null:feignMethodAttr.ParamName;
            return new FeignParamInfo(paramInfo.Name,paramInfo.ParameterType,alias);
        }
    }
}