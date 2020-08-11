using MakelaarCounter.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MakelaarCounter.MessageHandlers
{
    public class RateLimitHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpRequestLog _requestLog;
        private readonly TimeSpan _limitTime;
        private readonly SemaphoreSlim _semaphoreSlim;

        public RateLimitHttpMessageHandler(int limitCount, TimeSpan limitTime)
        {
            _limitTime = limitTime;
            _requestLog = new HttpRequestLog(limitCount);
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync();
            var now = DateTime.Now;
            _requestLog.Add(now);
            if (_requestLog.IsFull())
            {
                await Delay(now);
            }
            _semaphoreSlim.Release();
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task Delay(DateTime now)
        {
            var minimumRequestDateTime = now.Add(-_limitTime);
            var delayTime = _requestLog.IsOverflooding(minimumRequestDateTime) ? _limitTime : TimeSpan.Zero;
            if (delayTime > TimeSpan.Zero)
            {
                await Task.Delay(delayTime);
                _requestLog.DelayLast(delayTime);
            }
        }
    }
}
