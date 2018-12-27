using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IstioUtility
{
    public class HttpClientHelper
    {
        public static string PostData(string url, Dictionary<string, object> paramters)
        {
            var postJson = JsonConvert.SerializeObject(paramters);
            var httpClient = HttpClientFactory.Create();
            var request = new StringContent(postJson, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, request).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            return result;

            //return "123";

            //return "{\"appID\":1,\"appImage\":\"https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1543558871327&di=bf5d9374b2e4e3812d520a6902a5e8c2&imgtype=0&src=http%3A%2F%2Fstatic.open-open.com%2Fnews%2FuploadImg%2F20171120%2F20171120103131_963.png\",\"appName\":\"vscode\",\"appDescription\":\"Visual Studio Code is a lightweight but powerful source code editor which runs on your desktop and is available for Windows, macOS and Linux. It comes with built-in support for JavaScript, TypeScript and Node.js and has a rich ecosystem of extensions for other languages (such as C++, C#, Java, Python, PHP, Go) and runtimes (such as .NET and Unity). \",\"appPrice\":0.00,\"publishDate\":\"2016-10-12T00:00:00\",\"orders\":null}";
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}