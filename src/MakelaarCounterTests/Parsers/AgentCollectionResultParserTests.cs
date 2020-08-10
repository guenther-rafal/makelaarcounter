using AutoFixture;
using FluentAssertions;
using FluentResults;
using MakelaarCounter.Models;
using MakelaarCounter.Parsers;
using MakelaarCounter.Parsers.Implementation;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MakelaarCounterTests.Parsers
{
    public class AgentCollectionResultParserTests
    {
        private IAgentCollectionResultParser _agentCollectionResultParser;

        [SetUp]
        public void Setup()
        {
            _agentCollectionResultParser = new AgentCollectionResultParser();
        }

        [Test]
        public void Parse_Success_ReturnsUnionOfAllResults()
        {
            var fixture = new Fixture();
            var results = new List<Result<AgentCollection>>();
            var agentCollectionCount = fixture.Create<int>();
            var agentCollections = fixture.CreateMany<AgentCollection>(agentCollectionCount);
            foreach(var ac in agentCollections)
            {
                results.Add(Result.Ok(ac));
            }
            var initialCount = fixture.Create<int>();
            var initialResults = fixture.CreateMany<Agent>(initialCount).ToList();

            var result = _agentCollectionResultParser.Parse(results, initialResults);

            result.Value.Sum(z => z.Value.ListingCount).Should().Be(initialCount + agentCollections.Sum(z => z.FetchedAgents.Count));
        }

        [Test]
        public void Parse_FailedResultsPresent_ReturnsUnionOfAllResults()
        {
            var fixture = new Fixture();
            var results = new List<Result<AgentCollection>>();
            var agentCollectionCount = fixture.Create<int>();
            var agentCollections = fixture.CreateMany<AgentCollection>(agentCollectionCount);
            var errorMessage = fixture.Create<string>();
            foreach (var ac in agentCollections)
            {
                results.Add(Result.Fail(errorMessage));
            }
            var initialCount = fixture.Create<int>();
            var initialResults = fixture.CreateMany<Agent>(initialCount).ToList();

            var result = _agentCollectionResultParser.Parse(results, initialResults);

            result.Errors.Should().BeEquivalentTo(results.First().Errors);
        }
    }
}
