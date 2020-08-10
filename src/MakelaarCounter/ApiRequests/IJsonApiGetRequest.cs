using System.Net.Http;
using System.Threading.Tasks;

namespace MakelaarCounter.ApiRequests
{
    public interface IJsonApiGetRequest
    {
        Task<HttpResponseMessage> Execute(string clientName, string apiKey, string query);
    }
}