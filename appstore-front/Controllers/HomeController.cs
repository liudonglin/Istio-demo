﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appstore_front.Models;
using System.Net.Http;
using System.Net.Http.Formatting;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using appstore_front.Services;

namespace appstore_front.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration configuration;
        private IAppDetailService appDetailService;

        private ISSOService ssoService;

        private const string USERCOOKIENAME = "appstore_front_user_cookie";

        public HomeController(IConfiguration configuration
        ,IAppDetailService appDetailService)
        {
            this.configuration = configuration;
            this.appDetailService = appDetailService;
            //this.appDetailService = appDetailService;
            //this.ssoService = ssoService;
        }

        public IActionResult Index()
        {
            var headerStringBuilder = new StringBuilder();
            foreach(var header in Request.Headers)
            {
                headerStringBuilder.AppendLine($"{header.Key} : {header.Value}");
            }
            ViewData["HeaderInfo"] = headerStringBuilder.ToString();
            try
            {
                var apps = appDetailService.Get();

                ViewData["Apps"] = apps;
                ViewData["Appservice_Error"] = string.Empty;
            }
            catch (Exception e)
            {
                ViewData["Apps"] = new List<AppEntity>();
                ViewData["Appservice_Error"] = e.Message;
            }

            return View();
        }

        public IActionResult Account()
        {
            var user = ReadCookieUserinfo();
            ViewData["Userinfo"] = user;
            return View();
        }

        public IActionResult DoLogin(string userName,string password)
        {
            var token = ssoService.GetTokenInfo(userName,password);
            if(token==null||string.IsNullOrWhiteSpace(token.access_token))
            {
                ViewData["SSOError"] = "用户名或密码错误！";
            }
            else
            {
                var result = ssoService.GetUserInfo(token.access_token);
                var userinfo = result.data;
                if(userinfo==null||string.IsNullOrWhiteSpace(userinfo.userSysNo))
                {
                    ViewData["SSOError"] = "用户获取失败请稍后再试！";
                }
                else
                {
                    ViewData["Userinfo"] = userinfo;
                    SaveCookieUserinfo(userinfo);
                }
            }

            // var accountServiceHost = configuration.GetSection("AccountServiceHost").Value;
            // var accountTokenlUrl = accountServiceHost + "/sso/oauth/token";
            // var httpClient = HttpClientFactory.Create();

            // var postData = $"grant_type=password&client_id=jw.sso&client_secret=8f6727b0c1504774a407b928e96a197f&username={userName}&password={password}&scope=all";
            // var request = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
            // var task = httpClient.PostAsync(accountTokenlUrl, request).Result;
            // var token = task.Content.ReadAsAsync<TokenInfo>().Result;

            // if (string.IsNullOrWhiteSpace(token.access_token))
            // {
            //     ViewData["SSOError"] = "用户名或密码错误！";
            // }

            // var accountUserlUrl = accountServiceHost + $"/sso/v1/account/getUserInfo";
            // httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
            // task = httpClient.GetAsync(accountUserlUrl).Result;
            // var userinfo = task.Content.ReadAsAsync<ResultData<UserInfo>>().Result;

            // if (userinfo.data == null)
            // {
            //     ViewData["SSOError"] = "用户获取失败请稍后再试！";
            // }

            // ViewData["Userinfo"] = userinfo.data;
            // SaveCookieUserinfo(userinfo.data);
            return View("Account");
        }

        public IActionResult LoginOut()
        {
            HttpContext.Response.Cookies.Append(USERCOOKIENAME,string.Empty,new CookieOptions() { IsEssential = true });
            return View("Account");
        }

        private void SaveCookieUserinfo(UserInfo user)
        {
            if(user==null) return ;
            var userStr = JsonConvert.SerializeObject(user);
            userStr = Base64Encode(userStr);
            HttpContext.Response.Cookies.Append(USERCOOKIENAME,userStr,new CookieOptions() { IsEssential = true });
        }

        private UserInfo ReadCookieUserinfo()
        {
            var userStr = HttpContext.Request.Cookies[USERCOOKIENAME];

            if (string.IsNullOrWhiteSpace(userStr))
            {
                return null;
            }
            try
            {
                userStr = Base64Decode(userStr);
                var user = JsonConvert.DeserializeObject<UserInfo>(userStr);
                return user;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        public static string Base64Encode(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static string Base64Decode(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }

        public IActionResult Detail(int? appID)
        {
            if(!appID.HasValue)
            {
                return View("index");
            }

            try
            {
                var app = appDetailService.Get(appID.Value);
                if(app!=null)
                {
                    ViewData["AppInfo"] = app;
                    ViewData["Appservice_Error"] = string.Empty;
                }
                else
                {
                    ViewData["AppInfo"] = new AppEntity();
                    ViewData["Appservice_Error"] = $"AppID为{appID.Value}的应用不存在";
                }
            }
            catch (Exception e)
            {
                ViewData["AppInfo"] = new AppEntity();
                ViewData["Appservice_Error"] = e.Message;
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SetTraceHeaderInfo(HttpClient httpClient)
        {
            foreach (var header in this.Request.Headers)
            {
                switch (header.Key)
                {
                    case "x-request-id":
                    case "x-b3-traceid":
                    case "x-b3-spanid":
                    case "x-b3-parentspanid":
                    case "x-b3-sampled":
                    case "x-b3-flags":
                        if (!string.IsNullOrWhiteSpace(header.Value.ToString()))
                        {
                            httpClient.DefaultRequestHeaders.Add(header.Key.ToString(), header.Value.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
