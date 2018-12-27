using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using appstore_front.Services;
using IstioUtility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace appstore_front
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // var services = FeignInterfaceLoader.Load(new string[]{"appstore-front"});
            // var temp = services[typeof(IAppDetailService)];
            // var implType = FeignServiceTypeGenerator.GetOrCreateFeignServiceType(temp);
            // object ptInstance = Activator.CreateInstance(implType);

            // var instance = ptInstance as IAppDetailService;
            // var result1 = instance.TestGetEntity(1001,"gavin.d.liu");
            //var result2 = instance.TestGetString();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000")
                .UseStartup<Startup>();

    }
}
