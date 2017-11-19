using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [MemberData(nameof(UnenqueueData))]
        public void UnenqueueTest(
            int size,
            string[] values,
            string expectedResult,
            string[] expectedValues)
        {
            LimitedQueue<String> queue = new LimitedQueue<string>(size);

            foreach (string value in values)
            {
                queue.Enqueue(value);
            }

            string result = queue.Unenqueue();

            result.ShouldBeEquivalentTo(expectedResult);
            queue.Count.Should().Be(expectedValues.Length);
            queue.ToArray().ShouldAllBeEquivalentTo(expectedValues);
        }

        public static IEnumerable<object[]> UnenqueueData
        {
            get
            {
                int size = 3;

                string[] values0 = Array.Empty<string>();
                string[] values1 = new[] { "a" };
                string[] values3 = new[] { "a", "b", "c", "d" };

                string expectedResult0 = null;
                string expectedResult1 = values1.Last();
                string expectedResult3 = values3.Last();

                string[] expectedValues0 = Array.Empty<string>();
                string[] expectedValues1 = Array.Empty<string>();
                string[] expectedValues3 = new[] { "b", "c" };

                yield return new object[] { size, values0, expectedResult0, expectedValues0 };
                yield return new object[] { size, values1, expectedResult1, expectedValues1 };
                yield return new object[] { size, values3, expectedResult3, expectedValues3 };
            }
        }

        [Theory]
        [MemberData(nameof(PeekLastData))]
        public void PeekLastTest(
            int size,
            string[] values,
            string expectedResult,
            string[] expectedValues)
        {
            LimitedQueue<String> queue = new LimitedQueue<string>(size);

            foreach (string value in values)
            {
                queue.Enqueue(value);
            }

            string result = queue.PeekLast();

            result.ShouldBeEquivalentTo(expectedResult);
            queue.Count.Should().Be(expectedValues.Length);
            queue.ToArray().ShouldAllBeEquivalentTo(expectedValues);
        }

        public static IEnumerable<object[]> PeekLastData
        {
            get
            {
                int size = 3;

                string[] values0 = Array.Empty<string>();
                string[] values1 = new[] { "a" };
                string[] values3 = new[] { "a", "b", "c", "d" };

                string expectedResult0 = null;
                string expectedResult1 = values1.Last();
                string expectedResult3 = values3.Last();

                string[] expectedValues0 = Array.Empty<string>();
                string[] expectedValues1 = new[] { "a" };
                string[] expectedValues3 = new[] { "b", "c", "d" };

                yield return new object[] { size, values0, expectedResult0, expectedValues0 };
                yield return new object[] { size, values1, expectedResult1, expectedValues1 };
                yield return new object[] { size, values3, expectedResult3, expectedValues3 };
            }
        }
    }
}