using FluentResults;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakelaarCounter.Parsers.Implementation
{
    public class HttpResponseMessageJsonParser : IHttpResponseMessageJsonParser
    {
        public async Task<Result<TDestination>> Parse<TDestination>(Result<HttpResponseMessage> result)
        {
            if (result.IsFailed)
            {
                return result.ToResult();
            }
            var resultJson = await result.Value.Content.ReadAsStringAsync();
            return Result.Ok(JsonConvert.DeserializeObject<TDestination>(resultJson));
        }
    }
}
