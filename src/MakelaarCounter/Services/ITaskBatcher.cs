using System;
using System.Threading.Tasks;

namespace MakelaarCounter.Services
{
    public interface ITaskBatcher
    {
        Task<TResult[]> BatchExecute<TResult>(int count, Func<int, Task<TResult>> func);
    }
}