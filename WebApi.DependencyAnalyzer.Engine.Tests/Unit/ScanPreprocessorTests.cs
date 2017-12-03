using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Config;
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
            IPreprocessorConfig config,
            string[] tokens,
            string expectedResult)
        {
            ScanPreprocessor scanPreprocessor = new ScanPreprocessor(config);

            string result = scanPreprocessor.Preprocess(text, tokens);

            result.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> PreprocessData
        {
            get
            {
                IPreprocessorConfig config = Substitute.For<IPreprocessorConfig>();
                config.TrimTokens.Returns(new[] { ' ', '\'' });

                string text = "' IL23ab line IL23eg ' ";

                string[] tokens = new[] { "IL[0-9a-f]*" };

                string expectedResult = "line g";

                yield return new object[] { text, config, tokens, expectedResult };
            }
        }
    }
}