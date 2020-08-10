namespace MakelaarCounter.Models
{
    public class AgentListingCount
    {
        public AgentListingCount(Agent agent)
        {
            Agent = agent;
            ListingCount = 1;
        }

        public Agent Agent { get; }
        public int ListingCount { get; set; }
    }
}
