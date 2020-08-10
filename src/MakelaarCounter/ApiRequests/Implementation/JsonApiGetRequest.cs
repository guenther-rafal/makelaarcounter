using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakelaarCounter.ApiRequests.Implementation
{
    public class JsonApiGetRequest : IJsonApiGetRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public JsonApiGetRequest(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> Execute(string clientName, string apiKey, string query)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            return await client.GetAsync($"json/{apiKey}/{query.TrimStart('/')}");
        }
    }
}
