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
        private readonly IHttpClientFactory httpClientFactory;

        public AppDetailService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public List<AppEntity> Get()
        {
            var appDetailUrl = "/api/appdetail";
            var apiClient = httpClientFactory.CreateClient("app-service");
            var responseString = apiClient.GetStringAsync(appDetailUrl).Result;
            var apps = JsonConvert.DeserializeObject<List<AppEntity>>(responseString);
            return apps;
        }

        public AppEntity Get(int id)
        {
            var appDetailUrl = $"/api/appdetail/{id}";
            var apiClient = httpClientFactory.CreateClient("app-service");
            var responseString = apiClient.GetStringAsync(appDetailUrl).Result;
            var app = JsonConvert.DeserializeObject<AppEntity>(responseString);
            return app;
        }
    }
}