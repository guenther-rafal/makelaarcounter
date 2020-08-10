using AutoFixture;
using FluentAssertions;
using MakelaarCounter.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MakelaarCounterTests.Models
{
    public class HttpRequestLogTests
    {
        [Test]
        public void Add_NotOverMaxSize_Adds()
        {
            var fixture = new Fixture();
            var maxSize = fixture.Create<int>();
            var httpRequestLog = new HttpRequestLog(maxSize);
            var now = DateTime.Now;

            httpRequestLog.Add(now);

            httpRequestLog.Should().BeEquivalentTo(new List<DateTime> { now });
        }

        [Test]
        public void Add_OverMaxSize_RemovesOldest()
        {
            var fixture = new Fixture();
            var maxSize = fixture.Create<int>();
            var httpRequestLog = new HttpRequestLog(maxSize);
            var overMaxSizeInsertionValue = DateTime.Now;
            for (var i = 0; i < maxSize; i++)
            {
                httpRequestLog.Add(DateTime.Now.AddSeconds(i));
            }
            var expectedResult = httpRequestLog.Skip(1).ToList();
            expectedResult.Add(overMaxSizeInsertionValue);

            httpRequestLog.Add(overMaxSizeInsertionValue);

            httpRequestLog.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void IsFull_NotFull_ReturnsFalse()
        {
            var fixture = new Fixture();
            var maxSize = fixture.Create<int>();
            var lessThanMaxSize = fixture.Create<Generator<int>>().First(z => z < maxSize);
            var httpRequestLog = new HttpRequestLog(maxSize);
            for (var i = 0; i < lessThanMaxSize; i++)
            {
                httpRequestLog.Add(DateTime.Now);
            }

            var result = httpRequestLog.IsFull();

            result.Should().BeFalse();
        }

        [Test]
        public void IsFull_Full_ReturnsTrue()
        {
            var fixture = new Fixture();
            var maxSize = fixture.Create<int>();
            var httpRequestLog = new HttpRequestLog(maxSize);
            for (var i = 0; i < maxSize ; i++)
            {
                httpRequestLog.Add(DateTime.Now);
            }

            var result = httpRequestLog.IsFull();

            result.Should().BeTrue();
        }

        [Test]
        public void IsOverflooding_IsOverflooding_ReturnsTrue()
        {
            var fixture = new Fixture();
            var maxSize = fixture.Create<int>();
            var httpRequestLog = new HttpRequestLog(maxSize);
            for (var i = 0; i < maxSize; i++)
            {
                httpRequestLog.Add(DateTime.Now);
            }

            var result = httpRequestLog.IsOverflooding(DateTime.MinValue);

            result.Should().BeTrue();
        }

        [Test]
        public void IsOverflooding_IsNotOverflooding_ReturnsFalse()
        {
            var fixture = new Fixture();
            var maxSize = fixture.Create<int>();
            var httpRequestLog = new HttpRequestLog(maxSize);
            for (var i = 0; i < maxSize; i++)
            {
                httpRequestLog.Add(DateTime.Now);
            }

            var result = httpRequestLog.IsOverflooding(DateTime.Now.AddMilliseconds(1));

            result.Should().BeFalse();
        }
    }
}
