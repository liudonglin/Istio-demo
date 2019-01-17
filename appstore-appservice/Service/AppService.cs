using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MySql.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text;
using System;
using Microsoft.Extensions.Logging;

namespace appstore_appservice
{
    public class AppService:IAppService
    {
        private IConfiguration configuration;
        private IDbConnectionFactory connectionFactory;

        private readonly  ILogger logger;

        public AppService(IConfiguration _configuration
        ,IDbConnectionFactory _connectionFactory
        ,ILogger<AppDetailController> logger)
        {
            configuration = _configuration;
            connectionFactory = _connectionFactory;
            this.logger = logger;
        }

        public List<AppEntity> GetAllApps()
        {
            using (var conn = BuildConnection())
            {
                try
                {
                    var queryResult = conn.Query<AppEntity>("select * from test_appstore.AppEntity");
                    conn.Close();
                    return queryResult.ToList();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    return new List<AppEntity>();
                }
            }
        }

        public AppEntity GetAppByAppID(int appid)
        {
            using (var conn = BuildConnection())
            {
                try
                {
                    var queryResult = conn.Query<AppEntity>("select * from test_appstore.AppEntity where AppID=@AppID ", new { AppID = appid }).SingleOrDefault();
                    conn.Close();
                    return queryResult;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    return null;
                }
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
