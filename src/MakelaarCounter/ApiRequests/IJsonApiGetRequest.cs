using FluentResults;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakelaarCounter.ApiRequests
{
    public interface IJsonApiGetRequest
    {
        Task<Result<HttpResponseMessage>> Execute(string clientName, string apiKey, string query);
    }
}