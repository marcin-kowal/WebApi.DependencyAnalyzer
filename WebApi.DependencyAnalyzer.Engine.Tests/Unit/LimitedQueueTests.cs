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

        [Theory]
        [MemberData(nameof(AmendData))]
        public void AmendTest(int size, string[] values, string amendedValue, string[] expectedResult)
        {
            LimitedQueue<String> queue = new LimitedQueue<string>(size);

            foreach (string value in values)
            {
                queue.Enqueue(value);
            }

            queue.Amend(amendedValue, (s1, s2) => s1 + s2);

            queue.Count.Should().Be(expectedResult.Length);
            queue.ToArray().ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> AmendData
        {
            get
            {
                int size = 3;
                string[] values = new[] { "a", "b", "c", "d" };
                string amendedValue = "1";

                string[] expectedResult = new[] { "b", "c", "d1" };

                yield return new object[] { size, values, amendedValue, expectedResult };
                yield return new object[] { size, new string[0], amendedValue, new[] { amendedValue } };
            }
        }
    }
}