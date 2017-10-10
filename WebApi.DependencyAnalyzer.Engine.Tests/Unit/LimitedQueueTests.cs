using FluentAssertions;
using System;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Common;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class LimitedQueueTests
    {
        [Theory]
        [MemberData(nameof(EnqueueData))]
        public void EnqueueTest(int size, string[] values, string[] expectedResult)
        {
            LimitedQueue<String> queue = new LimitedQueue<string>(size);

            foreach (string value in values)
            {
                queue.Enqueue(value);
            }

            queue.Count.Should().Be(expectedResult.Length);
            queue.ToArray().ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> EnqueueData
        {
            get
            {
                int size = 2;
                string[] values = new[] { "a", "b", "c" };

                string[] expectedResult = new[] { "b", "c" };

                yield return new object[] { size, values, expectedResult };
            }
        }
    }
}