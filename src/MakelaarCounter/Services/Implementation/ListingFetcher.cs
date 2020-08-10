using FluentResults;
using MakelaarCounter.ApiRequests;
using MakelaarCounter.Constants;
using MakelaarCounter.Models;
using MakelaarCounter.Parsers;
using MakelaarCounter.Validators;
using System.Threading.Tasks;

namespace MakelaarCounter.Services.Implementation
{
    public class ListingFetcher : IListingFetcher
    {
        private readonly string _apiKey;
        private readonly IJsonApiGetRequest _jsonApiGetRequest;
        private readonly IAgentCollectionResultValidator _agentCollectionResultValidator;
        private readonly IHttpResponseMessageJsonParser _httpResponseMessageJsonParser;

        public ListingFetcher(IJsonApiGetRequest jsonApiGetRequest, IAgentCollectionResultValidator agentCollectionResultValidator,
            IHttpResponseMessageJsonParser httpResponseMessageJsonParser, string apiKey)
        {
            _jsonApiGetRequest = jsonApiGetRequest;
            _agentCollectionResultValidator = agentCollectionResultValidator;
            _httpResponseMessageJsonParser = httpResponseMessageJsonParser;
            _apiKey = apiKey;
        }

        public async Task<Result<AgentCollection>> Fetch(ListingFetcherQuery query, int page, int total)
        {
            var fetchingResult = await Fetch(query.ToString(page));
            var validationResult = _agentCollectionResultValidator.Validate(fetchingResult, total, page);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }
            return fetchingResult;
        }

        public async Task<Result<AgentCollection>> Fetch(ListingFetcherQuery query)
        {
            return await Fetch(query.FirstPageQueryToString());
        }

        private async Task<Result<AgentCollection>> Fetch(string query)
        {
            var result = await _jsonApiGetRequest.Execute(HttpClientNames.Funda, _apiKey, query);
            return await _httpResponseMessageJsonParser.Parse<AgentCollection>(result);
        }
    }
}
