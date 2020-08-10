using FluentResults;
using MakelaarCounter.Models;

namespace MakelaarCounter.Validators
{
    public interface IAgentCollectionResultValidator
    {
        Result Validate(Result<AgentCollection> resultToParse, int expectedTotal, int page);
        Result Validate(Result<AgentCollection> resultToValidate);
    }
}