using FluentResults;
using MakelaarCounter.Models;
using System;
using System.Linq;

namespace MakelaarCounter.Validators.Implementation
{
    public class AgentCollectionResultValidator : IAgentCollectionResultValidator
    {
        public Result Validate(Result<AgentCollection> resultToValidate, int expectedTotal, int page)
        {
            var validationResult = Validate(resultToValidate);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var fetchedData = resultToValidate.Value;
            if (expectedTotal != fetchedData.TotalFound && page > 1)
            {
                return Result.Fail(new Error("Concurrency issue, the data provided by the API has changed!"));
            }

            return Result.Ok();
        }

        public Result Validate(Result<AgentCollection> resultToValidate)
        {
            if (resultToValidate.IsFailed)
            {
                var newResult = Result.Fail(resultToValidate.Errors.First());
                newResult.Errors.AddRange(resultToValidate.Errors.Skip(1));
                return newResult;
            }

            return Result.Ok();
        }
    }
}
