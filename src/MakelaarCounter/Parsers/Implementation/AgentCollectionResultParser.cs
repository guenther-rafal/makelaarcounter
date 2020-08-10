using FluentResults;
using MakelaarCounter.Models;
using System.Collections.Generic;
using System.Linq;

namespace MakelaarCounter.Parsers.Implementation
{
    public class AgentCollectionResultParser : IAgentCollectionResultParser
    {
        public Result<ListingCountPerAgent> Parse(IList<Result<AgentCollection>> results, IList<Agent> initialResults)
        {
            var listingCountPerAgent = new ListingCountPerAgent(initialResults);
            if (results.Any(z => z.IsFailed))
            {
                return results.First(z => z.IsFailed).ToResult();
            }
            foreach (var r in results)
            {
                listingCountPerAgent.Update(r.Value.FetchedAgents);
            }
            return Result.Ok(listingCountPerAgent);
        }
    }
}
