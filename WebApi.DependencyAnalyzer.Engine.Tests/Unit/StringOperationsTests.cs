using FluentAssertions;
using System;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Common;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class StringOperationsTests
    {
        [Theory]
        [MemberData(nameof(StartsWithAnyData))]
        public void StartsWithAnyTest(string text, string[] tokens, bool expectedResult)
        {
            bool result = text.StartsWithAny(tokens);

            result.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> StartsWithAnyData
        {
            get
            {
                string text = "text to use";

                yield return new object[] { text, Array.Empty<string>(), false };
                yield return new object[] { text, new[] { "ext", "to" }, false };
                yield return new object[] { text, new[] { "tex", "to" }, true };
                yield return new object[] { text, new[] { "[a-z]+" }, true };
                yield return new object[] { text, new[] { "[A-Z]+" }, false };
            }
        }

        [Theory]
        [MemberData(nameof(ContainsAnyData))]
        public void ContainsAnyTest(string text, string[] tokens, bool expectedResult)
        {
            bool result = text.ContainsAny(tokens);

            result.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> ContainsAnyData
        {
            get
            {
                string text = "text to use";

                yield return new object[] { text, Array.Empty<string>(), false };
                yield return new object[] { text, new[] { "qwe", "to" }, true };
                yield return new object[] { text, new[] { "[a-z]+" }, true };
                yield return new object[] { text, new[] { "[A-Z]+" }, false };
            }
        }
    }
}