using FluentResults;
using MakelaarCounter.Models;

namespace MakelaarCounter.Validators.Implementation
{
    public class AgentCollectionResultValidator : IAgentCollectionResultValidator
    {
        public Result Validate(Result<AgentCollection> resultToValidate, int expectedTotal, int page)
        {
            if (resultToValidate.IsFailed)
            {
                return resultToValidate.ToResult();
            }

            var fetchedData = resultToValidate.Value;
            if (expectedTotal != fetchedData.TotalFound && page > 1)
            {
                return Result.Fail(new Error("Concurrency issue, the data provided by the API has changed!"));
            }

            return Result.Ok();
        }
    }
}
