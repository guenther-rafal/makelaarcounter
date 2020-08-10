using AutoFixture;
using FluentAssertions;
using FluentResults;
using MakelaarCounter.Extensions;
using MakelaarCounter.Models;
using NUnit.Framework;
using System.Linq;

namespace MakelaarCounterTests.Extensions
{
    public class ListingCountPerAgentResultExtensionTests
    {
        [Test]
        public void GetMostActive_Success_ReturnsMostActive()
        {
            var fixture = new Fixture();
            var topCount = fixture.Create<int>();
            var count = fixture.Create<Generator<int>>().First(z => z > topCount);
            var agents = fixture.CreateMany<Agent>(count).ToList();
            var listingCountPerAgent = new ListingCountPerAgent(agents);
            var expectedResult = listingCountPerAgent.OrderByDescending(z => z.Value.ListingCount).Take(topCount).ToDictionary(z => z.Key, z => z.Value);

            var result = listingCountPerAgent.ToResult().GetMostActive(topCount);

            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void GetMostActive_OriginalResultIsFailure_ReturnsOriginalResult()
        {
            var fixture = new Fixture();
            var topCount = fixture.Create<int>();
            var count = fixture.Create<Generator<int>>().First(z => z > topCount);

            var result = Result.Fail<ListingCountPerAgent>("").GetMostActive(topCount);

            result.IsFailed.Should().BeTrue();
        }
    }
}
