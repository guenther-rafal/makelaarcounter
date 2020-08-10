using AutoFixture;
using FluentAssertions;
using MakelaarCounter.ApiRequests.Implementation;
using MakelaarCounterTests.MockConfigs;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MakelaarCounterTests.ApiRequests
{
    public class JsonApiGetRequestTests
    {
        private JsonApiGetRequest _subjectUnderTest;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private Mock<HttpMessageHandler> _handler;

        [SetUp]
        public void Setup()
        {
            _handler = new Mock<HttpMessageHandler>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _subjectUnderTest = new JsonApiGetRequest(_httpClientFactory.Object);
        }

        [Test]
        public async Task Execute_CheckIfCallsCorrectUri_CheckSucceeds()
        {
            var fixture = new Fixture();
            var clientName = fixture.Create<string>();
            var apiKey = fixture.Create<string>();
            var query = fixture.Create<string>();
            var requestUri = "";
            var baseUri = "http://test.com/";
            var expectedRequestUri = $"{baseUri}json/{apiKey}/{query.TrimStart('/')}";

            _handler.ConfigUriCheck((request, cancellationToken) =>
            {
                requestUri = request.RequestUri.AbsoluteUri;
            });
            var httpClient = new HttpClient(_handler.Object);
            httpClient.BaseAddress = new Uri(baseUri);
            _httpClientFactory.Setup(z => z.CreateClient(clientName)).Returns(httpClient);

            await _subjectUnderTest.Execute(clientName, apiKey, query);

            _httpClientFactory.Verify(z => z.CreateClient(clientName), Times.Once);
            _handler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            requestUri.Should().BeEquivalentTo(expectedRequestUri);
        }

        [Test]
        public async Task Execute_Success_ReturnsOkResult()
        {
            var fixture = new Fixture();
            var clientName = fixture.Create<string>();
            var apiKey = fixture.Create<string>();
            var query = fixture.Create<string>();
            var baseUri = "http://test.com/";

            _handler.ConfigSuccess();
            var httpClient = new HttpClient(_handler.Object);
            httpClient.BaseAddress = new Uri(baseUri);
            _httpClientFactory.Setup(z => z.CreateClient(clientName)).Returns(httpClient);

            var result = await _subjectUnderTest.Execute(clientName, apiKey, query);

            _httpClientFactory.Verify(z => z.CreateClient(clientName), Times.Once);
            _handler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task Execute_Failure_ReturnsFailResult()
        {
            var fixture = new Fixture();
            var clientName = fixture.Create<string>();
            var apiKey = fixture.Create<string>();
            var query = fixture.Create<string>();

            _handler.ConfigFailure();
            var httpClient = new HttpClient(_handler.Object);
            httpClient.BaseAddress = new Uri("http://test.com/");
            _httpClientFactory.Setup(z => z.CreateClient(clientName)).Returns(httpClient);

            var result = await _subjectUnderTest.Execute(clientName, apiKey, query);

            _httpClientFactory.Verify(z => z.CreateClient(clientName), Times.Once);
            _handler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            result.IsSuccess.Should().BeFalse();
        }

        [Test]
        public async Task Execute_Exception_ReturnsFailResult()
        {
            var fixture = new Fixture();
            var clientName = fixture.Create<string>();
            var apiKey = fixture.Create<string>();
            var query = fixture.Create<string>();

            _handler.ConfigException();
            var httpClient = new HttpClient(_handler.Object);
            httpClient.BaseAddress = new Uri("http://test.com/");
            _httpClientFactory.Setup(z => z.CreateClient(clientName)).Returns(httpClient);

            var result = await _subjectUnderTest.Execute(clientName, apiKey, query);

            _httpClientFactory.Verify(z => z.CreateClient(clientName), Times.Once);
            _handler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            result.IsSuccess.Should().BeFalse();
        }
    }
}