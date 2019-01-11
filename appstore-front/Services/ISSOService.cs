using JWell.FeignNet.Core.Attributes;
using JWell.FeignNet.Extensions;

namespace appstore_front.Services
{
    [FeignClient("AccountServiceHost")]
    public interface ISSOService
    {
        [Headers("Content-Type:application/x-www-form-urlencoded")]
        [Body("grant_type=password&client_id=jw.sso&client_secret=8f6727b0c1504774a407b928e96a197f&username={userName}&password={password}&scope=all")]
        [RequestLine("POST /sso/oauth/token")]
        TokenInfo GetTokenInfo([Param("userName")]string userName,[Param("password")]string password);

        [Headers("Authorization:Bearer {accessToken}")]
        [RequestLine("GET /sso/v1/account/getUserInfo")]
        ResultData<UserInfo> GetUserInfo([Param("accessToken")]string accessToken);
    }
}