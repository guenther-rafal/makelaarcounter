using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MakelaarCounter.Services.Implementation
{
    public class TaskBatcher : ITaskBatcher
    {
        public async Task<TResult[]> BatchExecute<TResult>(int count, Func<int, Task<TResult>> func)
        {
            var tasks = new List<Task<TResult>>();
            for (var page = 0; page < count; page++)
            {
                tasks.Add(func(page));
            }
            return await Task.WhenAll(tasks);
        }
    }
}
