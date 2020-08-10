using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MakelaarCounter.Services
{
    public interface ITaskBatcher
    {
        Task<IList<TResult>> BatchExecute<TResult>(int count, Func<int, Task<TResult>> func);
    }
}