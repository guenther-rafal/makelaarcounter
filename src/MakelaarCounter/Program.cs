using MakelaarCounter.ApiRequests.Implementation;
using MakelaarCounter.Constants;
using MakelaarCounter.MessageHandlers;
using MakelaarCounter.Parsers.Implementation;
using MakelaarCounter.Services.Implementation;
using MakelaarCounter.Validators.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MakelaarCounter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await EnsureApiSanity();
            var counter = CreateAgentListingCounter();
            var results = await counter.GetMostActiveAgents(10, "koop", "/amsterdam/");
            var resultsWithGarden = await counter.GetMostActiveAgents(10, "koop", "/amsterdam/tuin/");
        }

        private static async Task EnsureApiSanity()
        {
            await Task.Delay(60 * 1000);
        }

        private static AgentListingCounter CreateAgentListingCounter()
        {
            var httpClientFactory = GetHttpClientFactory(HttpClientNames.Funda);
            var jsonRequest = new JsonApiGetRequest(httpClientFactory);
            var fetcher = new ListingFetcher(jsonRequest, new AgentCollectionResultValidator(),
                new HttpResponseMessageJsonParser(), ConfigurationManager.AppSettings["apiKey"]);
            return new AgentListingCounter(fetcher, new TaskBatcher(), new AgentCollectionResultParser());
        }

        private static IHttpClientFactory GetHttpClientFactory(string clientName)
        {
            var services = new ServiceCollection();
            services
                .AddHttpClient(clientName, client =>
                {
                    ConfigureClient(client);
                })
                .AddHttpMessageHandler(() => new RateLimitHttpMessageHandler(100, TimeSpan.FromSeconds(60)))
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetService<IHttpClientFactory>();
        }

        private static void ConfigureClient(HttpClient client)
        {
            var apiUrl = ConfigurationManager.AppSettings["fundaApiUrl"];
            client.BaseAddress = new Uri(apiUrl);
        }
    }
}
