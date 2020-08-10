using FluentResults;
using MakelaarCounter.ApiRequests;
using MakelaarCounter.Constants;
using MakelaarCounter.Models;
using MakelaarCounter.Validators;
using Newtonsoft.Json;
using System.Configuration;
using System.Threading.Tasks;

namespace MakelaarCounter.Services.Implementation
{
    public class ListingFetcher : IListingFetcher
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["apiKey"];
        private readonly IJsonApiGetRequest _jsonApiGetRequest;
        private readonly IAgentCollectionResultValidator _agentCollectionResultValidator;

        public ListingFetcher(IJsonApiGetRequest jsonApiGetRequest, IAgentCollectionResultValidator agentCollectionResultValidator)
        {
            _jsonApiGetRequest = jsonApiGetRequest;
            _agentCollectionResultValidator = agentCollectionResultValidator;
        }

        public async Task<Result<AgentCollection>> Fetch(ListingFetcherQuery query, int page, int total)
        {
            var fetchingResult = await Fetch(query.Get(page));
            var validationResult = _agentCollectionResultValidator.Validate(fetchingResult, total, page);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }
            return fetchingResult;
        }

        public async Task<Result<AgentCollection>> Fetch(ListingFetcherQuery query)
        {
            var fetchingResult = await Fetch(query.Get());
            var validationResult = _agentCollectionResultValidator.Validate(fetchingResult);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }
            return fetchingResult;
        }

        private async Task<Result<AgentCollection>> Fetch(string query)
        {
            var result = await _jsonApiGetRequest.Execute(HttpClientNames.Funda, _apiKey, query);
            var resultJson = await result.Content.ReadAsStringAsync();
            var xxx = Result.Ok(JsonConvert.DeserializeObject<AgentCollection>(resultJson));
            return xxx;
        }
    }
}
