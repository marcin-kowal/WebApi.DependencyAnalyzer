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
        public void ScanTest(IScannerConfig config, string[] lines, ScanResult expectedResult)
        {
            MultiLineScanner scanner = new MultiLineScanner(config);
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

                IScannerConfig configSuccess = Substitute.For<IScannerConfig>();
                configSuccess.TextSearchPatterns.Returns(textSearchPatterns);
                configSuccess.TrimPatterns.Returns(trimPatterns);

                IScannerConfig configPartialSuccess = Substitute.For<IScannerConfig>();
                configPartialSuccess.TextSearchPatterns.Returns(textSearchPatterns);
                configPartialSuccess.TextSearchPatternsExclude.Returns(new[] { "ine" });
                configPartialSuccess.TrimPatterns.Returns(trimPatterns);

                IScannerConfig configFailure = Substitute.For<IScannerConfig>();
                configFailure.TextSearchPatterns.Returns(new[] { "xt.*", ".* to" });
                configFailure.TextSearchPatternsExclude.Returns(new[] { "xt" });

                string[] lines = new[] { " line: ", "text t", "o search@" };

                ScanResult expectedResultSuccess = ScanResult.Success("xt to search", "line: text to");
                ScanResult expectedResultPartialSuccess = ScanResult.Success("xt to search");
                ScanResult expectedResultFailure = ScanResult.Failure();

                yield return new object[] { configSuccess, lines, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, lines, expectedResultPartialSuccess };
                yield return new object[] { configFailure, lines, expectedResultFailure };
            }
        }
    }
}