using System.Collections.Generic;
using System.Linq;

namespace MakelaarCounter.Models
{
    public class ListingCountPerAgent : Dictionary<int, AgentListingCount>
    {
        public ListingCountPerAgent() { }

        public ListingCountPerAgent(IList<Agent> agents)
        {
            Update(agents);
        }

        public ListingCountPerAgent(IDictionary<int, AgentListingCount> listingCountPerAgent)
            : base(listingCountPerAgent) { }

        public void Update(IList<Agent> agents)
        {
            foreach(var a in agents)
            {
                if (ContainsKey(a.Id))
                {
                    this[a.Id].ListingCount++;
                }
                else
                {
                    Add(a.Id, new AgentListingCount(a));
                }
            }
        }
    }
}
