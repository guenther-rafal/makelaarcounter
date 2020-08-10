using MakelaarCounter.ApiRequests.Implementation;
using MakelaarCounter.Constants;
using MakelaarCounter.MessageHandlers;
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
            var httpClientFactory = GetHttpClientFactory(HttpClientNames.Funda);
            var jsonRequest = new JsonApiGetRequest(httpClientFactory);
            var fetcher = new ListingFetcher(jsonRequest, new AgentCollectionResultValidator());
            var counter = new AgentListingCounter(fetcher, new TaskBatcher(), new AgentCollectionResultParser());
            var resultsWithGarden = await counter.GetMostActiveAgents(10, "koop", "/amsterdam/tuin/");
            var results = await counter.GetMostActiveAgents(10, "koop", "/amsterdam/");
        }

        private static void ConfigureClient(HttpClient client)
        {
            var apiUrl = ConfigurationManager.AppSettings["fundaApiUrl"];
            client.BaseAddress = new Uri(apiUrl);
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
    }
}
