using FluentResults;
using MakelaarCounter.Models;
using System.Threading.Tasks;

namespace MakelaarCounter.Services
{
    public interface IListingFetcher
    {
        Task<Result<AgentCollection>> Fetch(ListingFetcherQuery query, int page, int total);
        Task<Result<AgentCollection>> Fetch(ListingFetcherQuery query);
    }
}