using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace appstore_orderservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController: ControllerBase
    {
        private IOrderService orderService;

        public OrdersController(IOrderService _orderService)
        {
            this.orderService = _orderService;
        }

        // GET api/orders/getordersbyappid/1
        [HttpGet("[action]/{appid}")]
        public ActionResult<List<OrderInfo>> GetOrdersByAppid(int appid)
        {
            var headerStringBuilder = new StringBuilder();
            foreach(var header in Request.Headers)
            {
                headerStringBuilder.AppendLine($"{header.Key} : {header.Value}");
            }
            var headerInfo = headerStringBuilder.ToString();
            Console.Write(headerInfo) ;

            return orderService.GetOrderInfosByAppID(appid);
        }

        private void SetTraceHeaderInfo(HttpClient httpClient)
        {
            foreach (var header in Request.Headers)
            {
                switch (header.Key)
                {
                    case "x-request-id":
                    case "x-b3-traceid":
                    case "x-b3-spanid":
                    case "x-b3-parentspanid":
                    case "x-b3-sampled":
                    case "x-b3-flags":
                        if (!string.IsNullOrWhiteSpace(header.Value.ToString()))
                        {
                            httpClient.DefaultRequestHeaders.Add(header.Key.ToString(), header.Value.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
