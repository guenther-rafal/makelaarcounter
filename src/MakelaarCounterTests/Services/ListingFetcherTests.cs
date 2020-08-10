using AutoFixture;
using FluentAssertions;
using FluentResults;
using MakelaarCounter.ApiRequests;
using MakelaarCounter.Constants;
using MakelaarCounter.Models;
using MakelaarCounter.Parsers;
using MakelaarCounter.Services;
using MakelaarCounter.Services.Implementation;
using MakelaarCounter.Validators;
using Moq;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakelaarCounterTests.Services
{
    public class ListingFetcherTests
    {
        private Mock<IJsonApiGetRequest> _jsonApiGetRequest;
        private Mock<IAgentCollectionResultValidator> _agentCollectionResultValidator;
        private Mock<IHttpResponseMessageJsonParser> _httpResponseMessageJsonParser;
        private string _apiKey;
        private IListingFetcher _listingFetcher;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            _apiKey = fixture.Create<string>();
            _jsonApiGetRequest = new Mock<IJsonApiGetRequest>();
            _agentCollectionResultValidator = new Mock<IAgentCollectionResultValidator>();
            _httpResponseMessageJsonParser = new Mock<IHttpResponseMessageJsonParser>();
            _listingFetcher = new ListingFetcher(_jsonApiGetRequest.Object, _agentCollectionResultValidator.Object,
                _httpResponseMessageJsonParser.Object, _apiKey);
        }

        [Test]
        public async Task Fetch_FirstPage_Succeeds()
        {
            var fixture = new Fixture();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();
            var pageSize = fixture.Create<int>();
            var agentCollection = fixture.Create<AgentCollection>();
            var agentCollectionResult = Result.Ok(agentCollection);
            var responseMessage = Result.Ok(new HttpResponseMessage());
            var query = new ListingFetcherQuery(type, path, pageSize);
            var queryString = query.FirstPageQueryToString();

            _jsonApiGetRequest.Setup(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString)).ReturnsAsync(responseMessage);
            _httpResponseMessageJsonParser.Setup(z => z.Parse<AgentCollection>(responseMessage)).ReturnsAsync(agentCollectionResult);

            var result = await _listingFetcher.Fetch(query);

            _jsonApiGetRequest.Verify(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString), Times.Once);
            _httpResponseMessageJsonParser.Verify(z => z.Parse<AgentCollection>(responseMessage), Times.Once);
            result.Should().Be(agentCollectionResult);
        }

        [Test]
        public async Task Fetch_FirstPageFetchingFails_ReturnsFailure()
        {
            var fixture = new Fixture();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();
            var pageSize = fixture.Create<int>();
            var agentCollectionResult = Result.Fail<AgentCollection>("");
            var responseMessage = Result.Fail<HttpResponseMessage>("");
            var query = new ListingFetcherQuery(type, path, pageSize);
            var queryString = query.FirstPageQueryToString();

            _jsonApiGetRequest.Setup(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString)).ReturnsAsync(responseMessage);
            _httpResponseMessageJsonParser.Setup(z => z.Parse<AgentCollection>(responseMessage)).ReturnsAsync(agentCollectionResult);

            var result = await _listingFetcher.Fetch(query);

            _jsonApiGetRequest.Verify(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString), Times.Once);
            _httpResponseMessageJsonParser.Verify(z => z.Parse<AgentCollection>(responseMessage), Times.Once);
            result.IsFailed.Should().BeTrue();
        }

        [Test]
        public async Task Fetch_NthPage_Succeeds()
        {
            var fixture = new Fixture();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();
            var pageSize = fixture.Create<int>();
            var page = fixture.Create<int>();
            var total = fixture.Create<int>();
            var agentCollection = fixture.Create<AgentCollection>();
            var agentCollectionResult = Result.Ok(agentCollection);
            var responseMessage = Result.Ok(new HttpResponseMessage());
            var validationResult = Result.Ok();
            var query = new ListingFetcherQuery(type, path, pageSize);
            var queryString = query.ToString(page);

            _jsonApiGetRequest.Setup(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString)).ReturnsAsync(responseMessage);
            _httpResponseMessageJsonParser.Setup(z => z.Parse<AgentCollection>(responseMessage)).ReturnsAsync(agentCollectionResult);
            _agentCollectionResultValidator.Setup(z => z.Validate(agentCollectionResult, total, page)).Returns(validationResult);

            var result = await _listingFetcher.Fetch(query, page, total);

            _jsonApiGetRequest.Verify(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString), Times.Once);
            _httpResponseMessageJsonParser.Verify(z => z.Parse<AgentCollection>(responseMessage), Times.Once);
            _agentCollectionResultValidator.Verify(z => z.Validate(agentCollectionResult, total, page), Times.Once);
            result.Should().Be(agentCollectionResult);
        }

        [Test]
        public async Task Fetch_NthPageFails_ReturnsFailure()
        {
            var fixture = new Fixture();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();
            var pageSize = fixture.Create<int>();
            var page = fixture.Create<int>();
            var total = fixture.Create<int>();
            var agentCollectionResult = Result.Fail<AgentCollection>("");
            var responseMessage = Result.Fail<HttpResponseMessage>("");
            var validationResult = Result.Fail("");
            var query = new ListingFetcherQuery(type, path, pageSize);
            var queryString = query.ToString(page);

            _jsonApiGetRequest.Setup(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString)).ReturnsAsync(responseMessage);
            _httpResponseMessageJsonParser.Setup(z => z.Parse<AgentCollection>(responseMessage)).ReturnsAsync(agentCollectionResult);
            _agentCollectionResultValidator.Setup(z => z.Validate(agentCollectionResult, total, page)).Returns(validationResult);

            var result = await _listingFetcher.Fetch(query, page, total);

            _jsonApiGetRequest.Verify(z => z.Execute(HttpClientNames.Funda, _apiKey, queryString), Times.Once);
            _httpResponseMessageJsonParser.Verify(z => z.Parse<AgentCollection>(responseMessage), Times.Once);
            _agentCollectionResultValidator.Verify(z => z.Validate(agentCollectionResult, total, page), Times.Once);
            result.IsFailed.Should().BeTrue();
        }
    }
}
