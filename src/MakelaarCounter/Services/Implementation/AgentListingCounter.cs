using FluentResults;
using MakelaarCounter.Constants;
using MakelaarCounter.Extensions;
using MakelaarCounter.Models;
using MakelaarCounter.Parsers;
using System;
using System.Threading.Tasks;

namespace MakelaarCounter.Services.Implementation
{
    public class AgentListingCounter : IAgentListingCounter
    {
        private readonly IListingFetcher _agentFetcher;
        private readonly ITaskBatcher _taskBatcher;
        private readonly IAgentCollectionResultParser _agentCollectionResultParser;

        public AgentListingCounter(IListingFetcher agentFetcher, ITaskBatcher taskBatcher, 
            IAgentCollectionResultParser agentCollectionResultParser)
        {
            _agentFetcher = agentFetcher;
            _taskBatcher = taskBatcher;
            _agentCollectionResultParser = agentCollectionResultParser;
        }

        public async Task<Result<ListingCountPerAgent>> GetMostActiveAgents(int count, string type, string filterPath)
        {
            var query = new ListingFetcherQuery(type, filterPath, ApiConstants.MaxPageSize);
            var fetchingResult = await _agentFetcher.Fetch(query);
            if (fetchingResult.IsFailed)
            {
                return fetchingResult.ToResult();
            }

            var initialData = fetchingResult.Value;
            var pageCount = (int)Math.Ceiling((double)initialData.TotalFound / ApiConstants.MaxPageSize);
            var results = await _taskBatcher.BatchExecute(pageCount, (page) => _agentFetcher.Fetch(query, page, initialData.TotalFound));
            var parsedResults = _agentCollectionResultParser.Parse(results, initialData.FetchedAgents);
            return parsedResults.GetMostActive(count);
        }
    }
}
