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
        private readonly HttpClient httpClient;

        public OrderService(IConfiguration configuration)
        {
            this.httpClient = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 100, UseProxy = false });
            this.httpClient.BaseAddress = new System.Uri(configuration.GetSection("OrderServiceHost").Value);
        }

        public List<OrderInfo> GetOrdersByAppid(int appid)
        {
            var appOrderUrl = $"/api/orders/getordersbyappid/{appid}";
            var responseString = this.httpClient.GetStringAsync(appOrderUrl).Result;
            var orders = JsonConvert.DeserializeObject<List<OrderInfo>>(responseString);
            return orders;
        }
    }
}