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
        [MemberData(nameof(ModifyLastData))]
        public void ModifyLastTest(
            int size, 
            string[] values, 
            string modifierValue, 
            Func<string, string, string> modifyOperator, 
            string[] expectedResult)
        {
            LimitedQueue<String> queue = new LimitedQueue<string>(size)
                .WithAppendOperator(modifyOperator)
                .WithPrependOperator(modifyOperator);

            foreach (string value in values)
            {
                queue.Enqueue(value);
            }

            queue.ModifyLast(modifierValue, modifyOperator);

            queue.Count.Should().Be(expectedResult.Length);
            queue.ToArray().ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> ModifyLastData
        {
            get
            {
                int size = 3;
                string[] values = new[] { "a", "b", "c", "d" };
                string modifierValue = "1";

                string[] appendExpectedResult = new[] { "b", "c", "d1" };
                string[] prependExpectedResult = new[] { "b", "c", "1d" };

                Func<string, string, string> appendModifyOperator = (s1, s2) => s1 + s2;
                Func<string, string, string> prependModifyOperator = (s1, s2) => s2 + s1;
                Func<string, string, string> emptyModifyOperator = (s1, s2) => string.Empty;

                yield return new object[] { size, values, modifierValue, appendModifyOperator, appendExpectedResult };
                yield return new object[] { size, values, modifierValue, prependModifyOperator, prependExpectedResult };
                yield return new object[] { size, new string[0], modifierValue, emptyModifyOperator, new[] { modifierValue } };
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
                string[] values0 = new string [0];
                string[] values1 = new[] { "a" };
                string[] values3 = new[] { "a", "b", "c", "d" };

                string expectedResult0 = null;
                string expectedResult1 = values1.Last();
                string expectedResult3 = values3.Last();

                string[] expectedValues0 = new string[0];
                string[] expectedValues1 = new string[0];
                string[] expectedValues3 = new[] { "b", "c" };

                yield return new object[] { size, values0, expectedResult0, expectedValues0 };
                yield return new object[] { size, values1, expectedResult1, expectedValues1 };
                yield return new object[] { size, values3, expectedResult3, expectedValues3 };
            }
        }
    }
}