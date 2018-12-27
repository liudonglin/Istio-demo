using System.Collections.Generic;
using System;
using IstioUtility.Attributes;
using System.Reflection;

namespace IstioUtility
{
    public class FeignServiceInfo
    {
        public FeignServiceInfo(string serviceHost,Type interfaceType)
        {
            this.ServiceHost = serviceHost;
            this.InterfaceType = interfaceType;
            this.MethodInfos = new List<FeignMethodInfo> ();
        }

        public string ServiceHost {get;}

        public List<FeignMethodInfo> MethodInfos {get;}

        public Type InterfaceType {get;}
    }
}