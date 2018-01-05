using FluentAssertions;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class ScanPreprocessorTests
    {
        // TODO extend tests for Trim..()

        [Theory]
        [MemberData(nameof(PreprocessData))]
        public void PreprocessTest(
            string text,
            string[] tokens,
            string expectedResult)
        {
            ScanPreprocessor scanPreprocessor = new ScanPreprocessor();

            string result = scanPreprocessor.Preprocess(text, tokens);

            result.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> PreprocessData
        {
            get
            {
                string text = "' IL23ab line IL23eg ' ";

                string[] tokens = new[] { "IL[0-9a-f]*" };

                string expectedResult = "line g";

                yield return new object[] { text, tokens, expectedResult };
            }
        }
    }
}