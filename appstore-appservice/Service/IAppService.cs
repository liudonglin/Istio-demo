using System.Collections.Generic;

namespace appstore_appservice
{
    public interface IAppService
    {
        List<AppEntity> GetAllApps();

        AppEntity GetAppByAppID(int appid);
    }
}