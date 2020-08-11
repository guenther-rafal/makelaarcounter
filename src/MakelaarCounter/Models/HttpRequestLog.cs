using System;
using System.Collections.Generic;
using System.Linq;

namespace MakelaarCounter.Models
{
    public class HttpRequestLog : List<DateTime>
    {
        private readonly int _maxSize;

        public HttpRequestLog(int maxSize)
        {
            _maxSize = maxSize;
        }

        public new void Add(DateTime dateTime)
        {
            base.Add(dateTime);
            var logCountToRemove = Count - _maxSize;
            if (logCountToRemove > 0)
            {
                RemoveRange(0, logCountToRemove);
            }
        }

        public bool IsFull()
        {
            return Count == _maxSize;
        }

        public bool IsOverflooding(DateTime minimumRequestDateTime)
        {
            return this.All(x => x >= minimumRequestDateTime);
        }

        public void DelayLast(TimeSpan delayTime)
        {
            var safetyBuffer = 100;
            var delayedRequestTime = this.Last().Add(delayTime).AddMilliseconds(safetyBuffer);
            RemoveAt(_maxSize - 1);
            Add(delayedRequestTime);
        }
    }
}
