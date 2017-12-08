using FluentAssertions;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Common;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Common
{
    public class HashProviderTests
    {
        [Theory]
        [MemberData(nameof(GetHashData))]
        public void GetHashTest(string input, long expectedValue, bool checkIfNonZero)
        {
            HashProvider hashProvider = new HashProvider();

            long result = hashProvider.GetHash(input);

            result.Should().Be(expectedValue);
            if (checkIfNonZero)
            {
                result.Should().NotBe(0L);
            }
        }

        public static IEnumerable<object[]> GetHashData
        {
            get
            {
                string text = "qwe ASD 123 @#$";
                long expectedResult = new HashProvider().GetHash(text);

                yield return new object[] { string.Empty, 0, false };
                yield return new object[] { text, expectedResult, true };
            }
        }
    }
}