using FluentAssertions;
using FluentResults;
using MakelaarCounter.Parsers;
using MakelaarCounter.Parsers.Implementation;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakelaarCounterTests.Parsers
{
    public class HttpResponseMessageJsonParserTests
    {
        private IHttpResponseMessageJsonParser _httpResponseMessageJsonParser;

        [SetUp]
        public void Setup()
        {
            _httpResponseMessageJsonParser = new HttpResponseMessageJsonParser();
        }

        [Test]
        public async Task Parse_ErrorResponse_ReturnsFailure()
        {
            var responseResult = Result.Fail<HttpResponseMessage>("");

            var result = await _httpResponseMessageJsonParser.Parse<object>(responseResult);

            result.IsFailed.Should().BeTrue();
        }
    }
}
