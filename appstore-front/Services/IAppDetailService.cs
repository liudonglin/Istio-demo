using System.Collections.Generic;
using JWell.FeignNet.Core.Attributes;
using JWell.FeignNet.Extensions;

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