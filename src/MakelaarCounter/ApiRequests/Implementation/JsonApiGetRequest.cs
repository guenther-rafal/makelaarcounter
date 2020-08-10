using FluentResults;
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

        public async Task<Result<HttpResponseMessage>> Execute(string clientName, string apiKey, string query)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(clientName);
                var response = await client.GetAsync($"json/{apiKey}/{query.TrimStart('/')}");
                if (response.IsSuccessStatusCode)
                {
                    return Result.Ok(response);
                }
                return Result.Fail<HttpResponseMessage>(new Error($"{response.StatusCode} - {response.ReasonPhrase}"));
            }
            catch (HttpRequestException e)
            {
                return Result.Fail<HttpResponseMessage>(new Error($"{e.Message}"));
            }
        }
    }
}
