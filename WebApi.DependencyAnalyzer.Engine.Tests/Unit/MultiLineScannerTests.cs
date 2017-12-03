using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
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
            ScanResult[] expectedResult)
        {
            MultiLineScanner scanner = new MultiLineScanner(config, new ScanPreprocessor(preprocessorConfig));
            foreach (string line in lines)
            {
                scanner.AppendLine(line);
            }

            scanner.Scan();

            IReadOnlyCollection<ScanResult> result = scanner.GetResult();

            result.ShouldAllBeEquivalentTo(expectedResult);
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

                ScanResult[] expectedResultSuccess = new[] { "xt to search", "line:text to" }
                    .Select(result => ScanResult.Success(result))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccess = new[] { ScanResult.Success("xt to search") };
                ScanResult[] expectedResultFailure = new[] { ScanResult.Failure() };

                yield return new object[] { configSuccess, preprocessorConfig, lines, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, preprocessorConfig, lines, expectedResultPartialSuccess };
                yield return new object[] { configFailure, preprocessorConfig, lines, expectedResultFailure };
            }
        }
    }
}