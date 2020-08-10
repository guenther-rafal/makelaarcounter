using FluentResults;
using MakelaarCounter.Models;
using System.Collections.Generic;

namespace MakelaarCounter.Services
{
    public interface IAgentCollectionResultParser
    {
        Result<ListingCountPerAgent> Parse(IEnumerable<Result<AgentCollection>> results, IList<Agent> initialResults);
    }
}