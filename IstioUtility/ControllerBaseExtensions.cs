using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace IstioUtility
{
    public static class ControllerBaseExtensions
    {
        public static HttpClient GetTraceHttpClient(this ControllerBase controller)
        {
            var httpClient = HttpClientFactory.Create();
            SetTraceHeaderInfo(controller,httpClient);
            return httpClient;
        }

        private static void SetTraceHeaderInfo(ControllerBase controller,HttpClient httpClient)
        {
            foreach (var header in controller.Request.Headers)
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
