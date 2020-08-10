using AutoFixture;
using FluentAssertions;
using MakelaarCounter.Models;
using NUnit.Framework;

namespace MakelaarCounterTests.Models
{
    public class ListingFetcherQueryTests
    {
        [Test]
        public void ToString_ReturnsCorrectQuery()
        {
            var fixture = new Fixture();
            var page = fixture.Create<int>();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();
            var pageSize = fixture.Create<int>();
            var query = new ListingFetcherQuery(type, path, pageSize);
            var expectedResult = $"?type={type}&zo={path}&pagesize={pageSize}&page={page}";

            var result = query.ToString(page);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void FirstPageQueryToString_ReturnsCorrectQuery()
        {
            var fixture = new Fixture();
            var type = fixture.Create<string>();
            var path = fixture.Create<string>();
            var pageSize = fixture.Create<int>();
            var query = new ListingFetcherQuery(type, path, pageSize);
            var expectedResult = $"?type={type}&zo={path}&pagesize={pageSize}";

            var result = query.FirstPageQueryToString();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
