using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace appstore_orderservice
{
    public class OrderService : IOrderService
    {
        private IConfiguration configuration;

        private IDbConnectionFactory connectionFactory;

        public OrderService(IConfiguration _configuration,IDbConnectionFactory _connectionFactory)
        {
            configuration = _configuration;
            connectionFactory = _connectionFactory;
        }

        public List<OrderInfo> GetOrderInfosByAppID(int appid)
        {
            using (var conn = BuildConnection())
            {
                var queryResult = conn.Query<OrderInfo>("select * from test_appstore.OrderInfo where AppID=@AppID ", new { AppID = appid });
                conn.Close();
                return queryResult.ToList();
            }
        }

        private IDbConnection BuildConnection()
        {
            var connectionInfoSection = configuration.GetSection("AppStoreDBConnectionInfo");
            MySqlConnectionStringBuilder msb = new MySqlConnectionStringBuilder();
            msb.Server = connectionInfoSection.GetSection("Server").Value;
            msb.Port = uint.Parse(connectionInfoSection.GetSection("Port").Value);
            msb.Database = connectionInfoSection.GetSection("Database").Value;
            msb.UserID = connectionInfoSection.GetSection("UserID").Value;
            msb.Password = connectionInfoSection.GetSection("Password").Value;
            msb.MaximumPoolSize = 100;
            
            IDbConnection connection = connectionFactory.CreateConnection(msb.ConnectionString);
            return connection;
        }
    }
}