using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace appstore_appservice
{
    public class AppService:IAppService
    {
        private IConfiguration configuration;

        public AppService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public List<AppEntity> GetAllApps()
        {
            using (var conn = BuildConnection())
            {
                var queryResult = conn.Query<AppEntity>("select * from test_appstore.AppEntity");
                conn.Close();
                return queryResult.ToList();
            }
        }

        public AppEntity GetAppByAppID(int appid)
        {
            using (var conn = BuildConnection())
            {
                var queryResult = conn.Query<AppEntity>("select * from test_appstore.AppEntity where AppID=@AppID ", new { AppID = appid }).SingleOrDefault();
                conn.Close();
                return queryResult;
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

            IDbConnection connection = new MySqlConnection(msb.ConnectionString);
            return connection;
        }
    }
}
