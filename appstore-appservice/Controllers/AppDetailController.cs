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
        private IOrderService orderService;

        public AppDetailController(IAppService _appService
        , IConfiguration _configuration
        , IOrderService orderService)
        {
            this.appService = _appService;
            this.configuration = _configuration;
            this.orderService = orderService;
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
            return appService.GetAllApps();
        }

        // GET api/appdetail/5
        [HttpGet("{id}")]
        public ActionResult<AppEntity> Get(int id)
        {
            var result = appService.GetAppByAppID(id);

            if (result != null)
            {
                result.Orders = orderService.GetOrdersByAppid(id);
            }

            return result;
        }
    }
}
