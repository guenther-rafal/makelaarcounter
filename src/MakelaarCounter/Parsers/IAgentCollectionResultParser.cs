using FluentResults;
using MakelaarCounter.Models;
using System.Collections.Generic;

namespace MakelaarCounter.Parsers
{
    public interface IAgentCollectionResultParser
    {
        Result<ListingCountPerAgent> Parse(IList<Result<AgentCollection>> results, IList<Agent> initialResults);
    }
}