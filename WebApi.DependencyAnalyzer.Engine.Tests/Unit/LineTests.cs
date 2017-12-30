using FluentAssertions;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class LineTests
    {
        [Theory]
        [MemberData(nameof(AppendData))]
        internal void AppendTest(Line firstLine, Line secondLine, Line expectedResult)
        {
            Line result = firstLine.Append(secondLine);

            result.ShouldBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> AppendData
        {
            get
            {
                string first = "first";
                string second = "second";

                Line firstLine = new Line(first, first.Length);
                Line secondLine = new Line(second, second.Length);

                Line expectedResult = new Line(first + second, new long[] { first.Length, second.Length });

                yield return new object[] { firstLine, secondLine, expectedResult };
            }
        }
    }
}