using System;

namespace IstioUtility.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class FeignMethodAttribute : Attribute
    {
        public string RequestPath { get; }

        public HttpMethod HttpMethod { get; }

        public FeignMethodAttribute(string requestPath,HttpMethod httpMethod)
        {
            if(string.IsNullOrWhiteSpace(requestPath)) 
            {
                throw new Exception("requestPath Can't Be Null Or WhiteSpace");
            }
            this.RequestPath = requestPath;
            this.HttpMethod = httpMethod;
        }
    }

    public enum HttpMethod
    {
        Post = 0,

        Get = 1

    }
}
