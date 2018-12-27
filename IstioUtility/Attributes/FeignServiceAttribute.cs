using System;

namespace IstioUtility.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class FeignServiceAttribute : Attribute
    {
        public string ServiceHost { get; }

        public FeignServiceAttribute(string serviceHost)
        {
            if(string.IsNullOrWhiteSpace(serviceHost)) 
            {
                throw new Exception("serviceHost Can't Be Null Or WhiteSpace");
            }
            this.ServiceHost = serviceHost;
        }
    }
}

