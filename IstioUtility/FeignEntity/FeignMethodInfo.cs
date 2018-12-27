using System.Collections.Generic;
using System;
using IstioUtility.Attributes;
using System.Reflection;

namespace IstioUtility
{
    public class FeignMethodInfo
    {
        public FeignMethodInfo(string requestPath,HttpMethod httpMethod,MethodInfo methodType)
        {
            this.RequestPath = requestPath;
            this.HttpMethod = httpMethod;
            this.MethodType = methodType;

            ParamInfos = new List<FeignParamInfo> ();
        }

        public string RequestPath {get;}

        public HttpMethod HttpMethod { get; }

        public List<FeignParamInfo> ParamInfos { get; }

        public MethodInfo MethodType { get; }
    }
}