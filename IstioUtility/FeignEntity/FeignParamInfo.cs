using System.Collections.Generic;
using System;
using IstioUtility.Attributes;
using System.Reflection;

namespace IstioUtility
{
    public class FeignParamInfo
    {
        public FeignParamInfo(string paramName, Type paramType, string alias)
        {
            this.ParamName = paramName;
            this.ParamType = paramType;
            this.Alias = alias;
        }

        public Type ParamType { get; }

        public string ParamName { get; }

        public string Alias { get; }
    }
}