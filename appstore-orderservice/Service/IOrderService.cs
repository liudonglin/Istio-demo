using System.Collections.Generic;

namespace appstore_orderservice
{
    public interface IOrderService
    {
        List<OrderInfo> GetOrderInfosByAppID(int appid);

    }
}