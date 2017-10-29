using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Proxy
{
    /// <summary>
    /// Example code for an ActionResult which proxies a HttpClient response.
    /// </summary>
    /// <inheritdoc cref="IDisposable" />
    /// <inheritdoc cref="ActionResult" />
    public class ProxyResult : ActionResult, IDisposable
    {
        private readonly HttpResponseMessage _proxiedResponse;
        
        public ProxyResult(HttpResponseMessage proxiedResponse)
        {
            _proxiedResponse = proxiedResponse;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            
            response.StatusCode = (int) _proxiedResponse.StatusCode;

            ProxyHeaderUtil.CopyProxyHeaders(_proxiedResponse, response);
            
            using (var inStream = await _proxiedResponse.Content.ReadAsStreamAsync())
            {
                var outStream = response.Body;
                await inStream.CopyToAsync(outStream);
            }
        }

        public void Dispose()
        {
            _proxiedResponse?.Dispose();
        }
    }
}
