using System.Collections.Generic;
using OF.FeignCore.Attributes;
using OF.FeignCore.Extensions;

namespace appstore_front.Services
{
    [FeignClient("AppServiceHost")]
    public interface IAppDetailService
    {
        [RequestLine("GET api/appdetail")]
        List<AppEntity> Get();

        [RequestLine("GET api/appdetail/{id}")]
        AppEntity Get([Param("id")] int id);


    }
}