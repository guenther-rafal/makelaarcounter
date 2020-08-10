using Newtonsoft.Json;
using System.Collections.Generic;

namespace MakelaarCounter.Models
{
    [JsonObject]
    public class AgentCollection
    {
        [JsonProperty("Objects")]
        public List<Agent> FetchedAgents { get; set; }

        [JsonProperty("TotaalAantalObjecten")]
        public int TotalFound { get; set; }
    }
}
