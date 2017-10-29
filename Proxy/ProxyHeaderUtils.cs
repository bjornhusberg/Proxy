using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Proxy
{
    public static class ProxyHeaderUtil
    {
        private static readonly string[] BlackListedHeaders = { "Host", "Connection", "Transfer-Encoding" };

        public static void CopyProxyHeaders(HttpResponseMessage from, HttpResponse to)
        {
            to.Headers.Clear();
            from.Headers.Concat(from.Content.Headers)
                .Where(header => !BlackListedHeaders.Contains(header.Key))
                .ToList()
                .ForEach(header => to.Headers.Add(header.Key, header.Value.ToArray()));
        }

        public static void CopyProxyHeaders(HttpRequest from, HttpRequestMessage to)
        {
            to.Headers.Clear();
            from.Headers
                .Where(header => !BlackListedHeaders.Contains(header.Key))
                .ToList()
                .ForEach(header => to.Headers.Add(header.Key, header.Value.ToList()));
        }
    }
}