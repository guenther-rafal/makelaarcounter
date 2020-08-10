using AutoFixture;
using FluentAssertions;
using FluentResults;
using MakelaarCounter.Models;
using MakelaarCounter.Validators;
using MakelaarCounter.Validators.Implementation;
using NUnit.Framework;
using System.Linq;

namespace MakelaarCounterTests.Validators
{
    public class AgentCollectionResultValidatorTests
    {
        private IAgentCollectionResultValidator _agentCollectionResultValidator;

        [SetUp]
        public void Setup()
        {
            _agentCollectionResultValidator = new AgentCollectionResultValidator();
        }

        [Test]
        public void Validate_NoConcurrencyIssues_ReturnsSuccessResult()
        {
            var fixture = new Fixture();
            var expectedTotal = fixture.Create<int>();
            var page = fixture.Create<int>();
            var agentCollection = fixture.Create<AgentCollection>();
            agentCollection.TotalFound = expectedTotal;
            var agentCollectionResult = Result.Ok(agentCollection);

            var result = _agentCollectionResultValidator.Validate(agentCollectionResult, expectedTotal, page);

            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validate_FirstPage_ReturnsSuccessResult()
        {
            var fixture = new Fixture();
            var expectedTotal = fixture.Create<int>();
            var page = 1;
            var agentCollection = fixture.Create<AgentCollection>();
            agentCollection.TotalFound = 0;
            var agentCollectionResult = Result.Ok(agentCollection);

            var result = _agentCollectionResultValidator.Validate(agentCollectionResult, expectedTotal, page);

            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validate_OriginalResultIsFailure_ReturnsFailureResult()
        {
            var fixture = new Fixture();
            var expectedTotal = fixture.Create<int>();
            var page = fixture.Create<int>();
            var errorMessage = fixture.Create<string>();
            var agentCollectionResult = Result.Fail<AgentCollection>(errorMessage);

            var result = _agentCollectionResultValidator.Validate(agentCollectionResult, expectedTotal, page);

            result.IsSuccess.Should().BeFalse();
        }

        [Test]
        public void Validate_ExpectedTotalIncorrect_ReturnsFailureResult()
        {
            var fixture = new Fixture();
            var expectedTotal = fixture.Create<int>();
            var page = fixture.Create<int>();
            var errorMessage = fixture.Create<string>();
            var agentCollection = fixture.Create<AgentCollection>();
            agentCollection.TotalFound = fixture.Create<Generator<int>>().First(z => z != expectedTotal);
            var agentCollectionResult = Result.Ok(agentCollection);

            var result = _agentCollectionResultValidator.Validate(agentCollectionResult, expectedTotal, page);

            var error = result.Errors.Single();
            error.Message.Should().BeEquivalentTo("Concurrency issue, the data provided by the API has changed!");
        }
    }
}
