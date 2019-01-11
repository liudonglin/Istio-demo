using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using IstioUtility;
using Microsoft.Extensions.Logging;

namespace appstore_appservice
{

    [Route("api/[controller]")]
    [ApiController]
    public class AppDetailController: ControllerBase
    {
        private IAppService appService;
        private IConfiguration configuration;

        private readonly  ILogger logger;

        public AppDetailController(IAppService _appService
        ,IConfiguration _configuration
        ,ILogger<AppDetailController> logger)
        {
            this.appService = _appService;
            this.configuration = _configuration;
            this.logger = logger;
        }

        // GET api/appdetail
        [HttpGet]
        public ActionResult<List<AppEntity>> Get()
        {
            var headerStringBuilder = new StringBuilder();
            foreach(var header in Request.Headers)
            {
                headerStringBuilder.AppendLine($"{header.Key} : {header.Value}");
            }
            var headerInfo = headerStringBuilder.ToString();
            logger.LogInformation(headerInfo);
            logger.LogWarning(headerInfo);
            return appService.GetAllApps();
        }

        // GET api/appdetail/5
        [HttpGet("{id}")]
        public ActionResult<AppEntity> Get(int id)
        {
            logger.LogInformation("根据id获取AppEntity的信息");
            logger.LogWarning("根据id获取AppEntity的信息");

            var result = appService.GetAppByAppID(id);

            if (result != null)
            {
                var appOrderHost = configuration.GetSection("OrderServiceHost").Value;
                var appOrderUrl = appOrderHost + $"/api/orders/getordersbyappid/{id}";

                var httpClient = this.GetTraceHttpClient();
                var task = httpClient.GetAsync(appOrderUrl).Result;
                result.Orders = task.Content.ReadAsAsync<List<OrderInfo>>().Result;
            }

            return result;
        }

        // post api/appdetail/test_get_entity
        [HttpPost("test_get_entity")]
        public ActionResult<AppEntity> TestGetEntity(dynamic app)
        {
            var result = appService.GetAppByAppID(1);
            return result;
        }

        [HttpPost("test_get_string")]
        public ActionResult<string> TestGetString()
        {
            return "api TestGetString result";
        }
    }
}
