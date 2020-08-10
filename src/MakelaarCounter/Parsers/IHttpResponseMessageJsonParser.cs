using FluentResults;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakelaarCounter.Parsers
{
    public interface IHttpResponseMessageJsonParser
    {
        Task<Result<TDestination>> Parse<TDestination>(Result<HttpResponseMessage> result);
    }
}