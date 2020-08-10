using FluentResults;
using MakelaarCounter.Models;
using System.Threading.Tasks;

namespace MakelaarCounter.Services
{
    public interface IAgentListingCounter
    {
        Task<Result<ListingCountPerAgent>> GetMostActiveAgents(int count, string type, string filterPath);
    }
}