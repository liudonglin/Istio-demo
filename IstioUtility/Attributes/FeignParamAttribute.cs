using System;

namespace IstioUtility.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class FeignParamAttribute : Attribute
    {
        public string ParamName { get; }

        public FeignParamAttribute(string paramName)
        {
            if(string.IsNullOrWhiteSpace(paramName)) 
            {
                throw new Exception("paramName Can't Be Null Or WhiteSpace");
            }
            this.ParamName = paramName;
        }
    }
}