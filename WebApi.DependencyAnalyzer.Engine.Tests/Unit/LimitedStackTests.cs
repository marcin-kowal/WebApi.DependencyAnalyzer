using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Common;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class LimitedStackTests
    {
        [Theory]
        [MemberData(nameof(PushData))]
        public void PushTest(
            int size,
            string[] values,
            string[] expectedResult)
        {
            LimitedStack<String> stack = new LimitedStack<string>(size);

            foreach (string value in values)
            {
                stack.Push(value);
            }

            stack.Count.Should().Be(expectedResult.Length);
            stack.ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> PushData
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
        [MemberData(nameof(PopData))]
        public void PopTest(
            int size,
            string[] values,
            string expectedResult,
            string[] expectedValues)
        {
            LimitedStack<String> stack = new LimitedStack<string>(size);

            foreach (string value in values)
            {
                stack.Push(value);
            }

            string result = stack.Pop();

            result.ShouldBeEquivalentTo(expectedResult);
            stack.Count.Should().Be(expectedValues.Length);
            stack.ShouldAllBeEquivalentTo(expectedValues);
        }

        public static IEnumerable<object[]> PopData
        {
            get
            {
                int size = 3;

                string[] values1 = new[] { "a" };
                string[] values3 = new[] { "a", "b", "c", "d" };

                string expectedResult1 = values1.Last();
                string expectedResult3 = values3.Last();

                string[] expectedValues1 = Array.Empty<string>();
                string[] expectedValues3 = new[] { "b", "c" };

                yield return new object[] { size, values1, expectedResult1, expectedValues1 };
                yield return new object[] { size, values3, expectedResult3, expectedValues3 };
            }
        }

        [Fact]
        public void CtorIncorrectSizeTest()
        {
            Exception exception = Record.Exception(() => new LimitedStack<string>(0));

            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void PopFromEmptyStackTest()
        {
            LimitedStack<string> stack = new LimitedStack<string>(1);

            Exception exception = Record.Exception(() => stack.Pop());

            exception.Should().BeOfType<InvalidOperationException>();
        }
    }
}