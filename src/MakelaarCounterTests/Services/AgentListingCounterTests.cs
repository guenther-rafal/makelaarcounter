using AutoFixture;
using FluentAssertions;
using FluentResults;
using MakelaarCounter.Constants;
using MakelaarCounter.Models;
using MakelaarCounter.Parsers;
using MakelaarCounter.Services;
using MakelaarCounter.Services.Implementation;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakelaarCounterTests.Services
{
    public class AgentListingCounterTests
    {
        private IAgentListingCounter _agentListingCounter;
        private Mock<IListingFetcher> _listingFetcher;
        private Mock<ITaskBatcher> _taskBatcher;
        private Mock<IAgentCollectionResultParser> _agentCollectionResultParser;

        [SetUp]
        public void Setup()
        {
            _listingFetcher = new Mock<IListingFetcher>();
            _taskBatcher = new Mock<ITaskBatcher>();
            _agentCollectionResultParser = new Mock<IAgentCollectionResultParser>();
            _agentListingCounter = new AgentListingCounter(_listingFetcher.Object, 
                _taskBatcher.Object, _agentCollectionResultParser.Object);
        }

        [Test]
        public async Task GetMostActiveAgents_FetchingFailed_ReturnsFailure()
        {
            var fixture = new Fixture();
            var count = fixture.Create<int>();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();

            _listingFetcher.Setup(z => z.Fetch(It.IsAny<ListingFetcherQuery>())).ReturnsAsync(Result.Fail<AgentCollection>(""));

            var result = await _agentListingCounter.GetMostActiveAgents(count, type, path);

            _listingFetcher.Verify(z => z.Fetch(It.IsAny<ListingFetcherQuery>()), Times.Once);
            result.IsFailed.Should().BeTrue();
        }

        [Test]
        public async Task GetMostActiveAgents_FetchingSuccessful_ReturnsDesiredNumberOfResults()
        {
            var fixture = new Fixture();
            var count = fixture.Create<int>();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();
            var initialFetchingResult = Result.Ok(fixture.Create<AgentCollection>());
            var pageCount = (int)Math.Ceiling((double)initialFetchingResult.Value.TotalFound / ApiConstants.MaxPageSize);
            var batchedFetchingResults = new List<Result<AgentCollection>>();
            for (var i = 0; i < pageCount; i++)
            {
                var chunk = Result.Ok(fixture.Create<AgentCollection>());
                batchedFetchingResults.Add(chunk);
            }
            var listingCountPerAgentResult = Result.Ok(new ListingCountPerAgent(fixture.CreateMany<Agent>(count).ToList()));

            _listingFetcher.Setup(z => z.Fetch(It.IsAny<ListingFetcherQuery>())).ReturnsAsync(initialFetchingResult);
            _taskBatcher
                .Setup(z => z.BatchExecute(pageCount, It.IsAny<Func<int, Task<Result<AgentCollection>>>>()))
                .ReturnsAsync(batchedFetchingResults);
            _agentCollectionResultParser
                .Setup(z => z.Parse(batchedFetchingResults, initialFetchingResult.Value.FetchedAgents))
                .Returns(listingCountPerAgentResult);

            var result = await _agentListingCounter.GetMostActiveAgents(count, type, path);

            _listingFetcher.Verify(z => z.Fetch(It.IsAny<ListingFetcherQuery>()), Times.Once);
            _taskBatcher.Verify(z => z.BatchExecute(pageCount, It.IsAny<Func<int, Task<Result<AgentCollection>>>>()), Times.Once);
            _agentCollectionResultParser.Verify(z => z.Parse(batchedFetchingResults, initialFetchingResult.Value.FetchedAgents), Times.Once);
            result.Value.Count.Should().Be(count);
        }
    }
}
