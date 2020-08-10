namespace MakelaarCounter.Models
{
    public class AgentListingCount
    {
        public AgentListingCount(Agent agent)
        {
            Agent = agent;
        }

        public Agent Agent { get; }
        public int ListingCount { get; set; }
    }
}
