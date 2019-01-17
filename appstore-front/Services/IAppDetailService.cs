using System.Collections.Generic;
using System.Net.Http;
using JWell.FeignNet.Core.Attributes;
using JWell.FeignNet.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace appstore_front.Services
{
    [FeignClient("AppServiceHost")]
    public interface IAppDetailService
    {
        [RequestLine("GET api/appdetail")]
        List<AppEntity> Get();

        [RequestLine("GET api/appdetail/{id}")]
        AppEntity Get([Param("id")] int id);


    }

    public class AppDetailService : IAppDetailService
    {
        private readonly HttpClient httpClient;

        public AppDetailService(IConfiguration configuration)
        {
            this.httpClient = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 100, UseProxy = false });
            this.httpClient.BaseAddress = new System.Uri(configuration.GetSection("AppServiceHost").Value);
        }

        public List<AppEntity> Get()
        {
            var appDetailUrl = "/api/appdetail";
            var responseString = this.httpClient.GetStringAsync(appDetailUrl).Result;
            var apps = JsonConvert.DeserializeObject<List<AppEntity>>(responseString);
            return apps;
        }

        public AppEntity Get(int id)
        {
            var appDetailUrl = $"/api/appdetail/{id}";
            var responseString = this.httpClient.GetStringAsync(appDetailUrl).Result;
            var app = JsonConvert.DeserializeObject<AppEntity>(responseString);
            return app;
        }
    }
}