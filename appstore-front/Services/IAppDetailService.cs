using System.Collections.Generic;
using IstioUtility.Attributes;

namespace appstore_front.Services
{
    [FeignService("http://localhost:5001/")]
    public interface IAppDetailService
    {
        [FeignMethod("api/appdetail",HttpMethod.Get)]
        List<AppEntity> Get();

        [FeignMethod("api/appdetail",HttpMethod.Get)]
        AppEntity Get(int id);

        [FeignMethod("/api/appdetail/test_get_entity",HttpMethod.Post)]
        string TestGetEntity(int sysno,[FeignParam("tname")]string name);

        [FeignMethod("/api/appdetail/test_get_string",HttpMethod.Post)]
        string TestGetString();

    }
}