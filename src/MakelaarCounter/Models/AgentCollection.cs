using Newtonsoft.Json;
using System.Collections.Generic;

namespace MakelaarCounter.Models
{
    [JsonObject]
    public class AgentCollection
    {
        [JsonProperty("Objects")]
        public List<Agent> FetchedAgents { get; private set; }

        [JsonProperty("TotaalAantalObjecten")]
        public int TotalFound { get; private set; }

        [JsonProperty("$.Paging.AantalPaginas")]
        public int PageCount { get; private set; }
    }
}
