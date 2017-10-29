using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proxy
{
    public interface ISharedHttpClient : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
    
    public class SharedHttpClient : ISharedHttpClient
    {
        private readonly HttpClient _sharedHttpClient;

        public SharedHttpClient()
        {
            _sharedHttpClient = new HttpClient(new HttpClientHandler
            {
                UseCookies = false
            });
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _sharedHttpClient.SendAsync(request);
        }

        public void Dispose()
        {
            _sharedHttpClient?.Dispose();
        }
    }
}