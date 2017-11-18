using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using WebApi.DependencyAnalyzer.Engine.Config;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class MultiLineScannerTests
    {
        [Theory]
        [MemberData(nameof(ScanData))]
        public void ScanTest(
            IScannerConfig config,
            IPreprocessorConfig preprocessorConfig,
            string[] lines, 
            ScanResult expectedResult)
        {
            MultiLineScanner scanner = new MultiLineScanner(config, new ScanPreprocessor(preprocessorConfig));
            foreach (string line in lines)
            {
                scanner.AppendLine(line);
            }

            ScanResult result = scanner.Scan();

            result.ShouldBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> ScanData
        {
            get
            {
                string[] textSearchPatterns = new[] { "xt.*", ".* to" };
                char[] trimPatterns = new[] { ' ', '@' };

                IPreprocessorConfig preprocessorConfig = Substitute.For<IPreprocessorConfig>();
                preprocessorConfig.TrimTokens.Returns(trimPatterns);

                IScannerConfig configSuccess = Substitute.For<IScannerConfig>();
                configSuccess.TextSearchPatterns.Returns(textSearchPatterns);

                IScannerConfig configPartialSuccess = Substitute.For<IScannerConfig>();
                configPartialSuccess.TextSearchPatterns.Returns(textSearchPatterns);
                configPartialSuccess.TextSearchPatternsExclude.Returns(new[] { "ine" });

                IScannerConfig configFailure = Substitute.For<IScannerConfig>();
                configFailure.TextSearchPatterns.Returns(new[] { "xt.*", ".* to" });
                configFailure.TextSearchPatternsExclude.Returns(new[] { "xt" });

                string[] lines = new[] { " line: ", "text t", "o search@" };

                ScanResult expectedResultSuccess = ScanResult.Success("xt to search", "line:text to");
                ScanResult expectedResultPartialSuccess = ScanResult.Success("xt to search");
                ScanResult expectedResultFailure = ScanResult.Failure();

                yield return new object[] { configSuccess, preprocessorConfig, lines, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, preprocessorConfig, lines, expectedResultPartialSuccess };
                yield return new object[] { configFailure, preprocessorConfig, lines, expectedResultFailure };
            }
        }
    }
}