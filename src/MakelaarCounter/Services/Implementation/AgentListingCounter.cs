using FluentResults;
using MakelaarCounter.Extensions;
using MakelaarCounter.Models;
using System;
using System.Threading.Tasks;

namespace MakelaarCounter.Services.Implementation
{
    public class AgentListingCounter : IAgentListingCounter
    {
        private readonly IListingFetcher _agentFetcher;
        private readonly ITaskBatcher _taskBatcher;
        private readonly IAgentCollectionResultParser _agentCollectionResultParser;
        private const int MaxPageSize = 25;

        public AgentListingCounter(IListingFetcher agentFetcher, ITaskBatcher taskBatcher, 
            IAgentCollectionResultParser agentCollectionResultParser)
        {
            _agentFetcher = agentFetcher;
            _taskBatcher = taskBatcher;
            _agentCollectionResultParser = agentCollectionResultParser;
        }

        //public async Task<Result<ListingCountPerAgent>> GetMostActiveAgents(int count, string type, string filterPath)
        //{
        //    var listingsCountPerAgent = new ListingCountPerAgent();
        //    var pageNumber = 1;
        //    var fetchedCount = 0;
        //    var total = 0;
        //    var query = new ListingFetcherQuery(type, filterPath, MaxPageSize);
        //    do
        //    {
        //        var fetchingResult = await _agentFetcher.Fetch(HttpClientNames.Funda, query.Get(pageNumber));
        //        var validationResult = _agentCollectionResultValidator.Validate(fetchingResult, total, pageNumber);
        //        if (validationResult.IsFailed)
        //        {
        //            return validationResult;
        //        }

        //        var fetchedData = fetchingResult.Value;
        //        listingsCountPerAgent.Update(fetchedData.FetchedAgents);
        //        fetchedCount += fetchedData.FetchedAgents.Count;
        //        total = fetchedData.TotalFound;
        //        pageNumber++;
        //    }
        //    while (fetchedCount < total);
        //    return Result.Ok(listingsCountPerAgent.GetMostActive(count));
        //}

        public async Task<Result<ListingCountPerAgent>> GetMostActiveAgents(int count, string type, string filterPath)
        {
            var query = new ListingFetcherQuery(type, filterPath, MaxPageSize);
            var fetchingResult = await _agentFetcher.Fetch(query);
            if (fetchingResult.IsFailed)
            {
                return fetchingResult.ToResult();
            }

            var initialData = fetchingResult.Value;
            var pageCount = (int)Math.Ceiling((double)initialData.TotalFound / MaxPageSize);
            var results = await _taskBatcher.BatchExecute(pageCount, (page) => _agentFetcher.Fetch(query, page, initialData.TotalFound));
            var parsedResults = _agentCollectionResultParser.Parse(results, initialData.FetchedAgents);
            return parsedResults.GetMostActive(count);
        }
    }
}
