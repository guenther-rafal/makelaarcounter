using AutoFixture;
using FluentAssertions;
using FluentResults;
using MakelaarCounter.Services;
using MakelaarCounter.Services.Implementation;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MakelaarCounterTests.Services
{
    public class TaskBatcherTests
    {
        private ITaskBatcher _taskBatcher;
        
        [SetUp]
        public void Setup()
        {
            _taskBatcher = new TaskBatcher();
        }

        [Test]
        public async Task Batch_ReturnsDesiredNumberOfResults()
        {
            var fixture = new Fixture();
            var count = fixture.Create<int>();
            var callCounter = 0;
            Func<int, Task<Result>> func = async (z) =>
            {
                callCounter++;
                return await Task.FromResult(Result.Ok());
            };

            var result = await _taskBatcher.BatchExecute(count, func);

            callCounter.Should().Be(count);
        }
    }
}
