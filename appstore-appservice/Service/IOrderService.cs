using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace appstore_appservice
{
    public interface IOrderService
    {
        List<OrderInfo> GetOrdersByAppid(int appid);
    }

    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public OrderService(IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public List<OrderInfo> GetOrdersByAppid(int appid)
        {
            var appOrderUrl = $"/api/orders/getordersbyappid/{appid}";
            var apiClient = httpClientFactory.CreateClient("order-service");
            var responseString = apiClient.GetStringAsync(appOrderUrl).Result;
            var orders = JsonConvert.DeserializeObject<List<OrderInfo>>(responseString);
            return orders;
        }
    }
}