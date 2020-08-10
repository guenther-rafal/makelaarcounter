using AutoFixture;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MakelaarCounterTests.MockConfigs
{
    public static class HttpMessageHandlerConfig
    {
        public static void ConfigUriCheck(this Mock<HttpMessageHandler> mock, Action<HttpRequestMessage, CancellationToken> callback)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback(callback)
                .ReturnsAsync(new HttpResponseMessage())
                .Verifiable();
        }

        public static void ConfigSuccess(this Mock<HttpMessageHandler> mock)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();
        }

        public static void ConfigFailure(this Mock<HttpMessageHandler> mock)
        {
            var fixture = new Fixture();
            var statusCodeGenerator = fixture.Create<Generator<HttpStatusCode>>();
            var statusCode = statusCodeGenerator.First(z => z != HttpStatusCode.OK);
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode
                })
                .Verifiable();
        }

        public static void ConfigException(this Mock<HttpMessageHandler> mock)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();
        }
    }
}
