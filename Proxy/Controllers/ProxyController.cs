using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

namespace Proxy.Controllers
{
    public class ProxyController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ISharedHttpClient _httpClient;
        private readonly ProxyConfiguration _proxyConfiguration;

        public ProxyController(ISharedHttpClient httpClient, ProxyConfiguration proxyConfiguration)
        {
            _httpClient = httpClient;
            _proxyConfiguration = proxyConfiguration;
        }

        [Route("/")]
        public async Task<IActionResult> Proxy()
        {
            var message = new HttpRequestMessage
            {
                Method = new HttpMethod(HttpContext.Request.Method),
                RequestUri = GetProxyRequestUri(),
                Content = new StreamContent(HttpContext.Request.Body),
            };
            ProxyHeaderUtil.CopyProxyHeaders(HttpContext.Request, message);
            return new ProxyResult(await _httpClient.SendAsync(message));
        }

        private Uri GetProxyRequestUri()
        {
            var baseUri = new Uri(_proxyConfiguration.BaseUrl);
            return new UriBuilder(HttpContext.Request.GetUri())
            {
                Scheme = baseUri.Scheme,
                Host = baseUri.Host,
                Port = baseUri.Port
            }.Uri;
        }
    }
}