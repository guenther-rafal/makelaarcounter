using FluentResults;
using MakelaarCounter.Models;
using System.Linq;

namespace MakelaarCounter.Extensions
{
    public static class ListingCountPerAgentResultExtension
    {
        public static Result<ListingCountPerAgent> GetMostActive(this Result<ListingCountPerAgent> result, int count)
        {
            if (result.IsFailed)
            {
                return result;
            }
            var mostActive = result.Value.OrderByDescending(z => z.Value.ListingCount).Take(count).ToDictionary(z => z.Key, z => z.Value);
            return Result.Ok(new ListingCountPerAgent(mostActive));
        }
    }
}
