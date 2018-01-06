using FluentAssertions;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class ScanPreprocessorTests
    {
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

        [Theory]
        [MemberData(nameof(TrimStartToData))]
        public void TrimStartToTest(
            string text,
            IReadOnlyCollection<string> tokens,
            string expectedResult)
        {
            ScanPreprocessor scanPreprocessor = new ScanPreprocessor();

            string result = scanPreprocessor.TrimStartTo(text, tokens);

            result.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> TrimStartToData
        {
            get
            {
                string text = "' IL23ab line IL23eg ' ";

                string[] tokens1 = new[] { "li" };
                string[] tokens2 = new[] { "lin.", "23" };

                string expectedResult1 = "line IL23eg ' ";
                string expectedResult2 = "23eg ' ";

                yield return new object[] { text, tokens1, expectedResult1 };
                yield return new object[] { text, tokens2, expectedResult2 };
            }
        }
    }
}