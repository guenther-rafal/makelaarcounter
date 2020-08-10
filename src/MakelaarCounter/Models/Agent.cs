using Newtonsoft.Json;

namespace MakelaarCounter.Models
{
    [JsonObject]
    public class Agent
    {
        [JsonProperty("MakelaarId")]
        public int Id { get; private set; }
        [JsonProperty("MakelaarNaam")]
        public string Name { get; private set; }
    }
}
