using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Config;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class SingleLineScannerTests
    {
        [Theory]
        [MemberData(nameof(ScanData))]
        public void ScanTest(
            IScannerConfig config, 
            IPreprocessorConfig preprocessorConfig,
            string line, 
            ScanResult[] expectedResult)
        {
            SingleLineScanner scanner = new SingleLineScanner(config, new ScanPreprocessor(preprocessorConfig));

            scanner.AppendLine(line);
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
                
                string line = " line: text to search@";

                ScanResult[] expectedResultSuccess = new[] { "xt to search", "line: text to" }
                    .Select(result => ScanResult.Success(result))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccess = new[] { ScanResult.Success("xt to search") };
                ScanResult[] expectedResultFailure = new[] { ScanResult.Failure() };

                yield return new object[] { configSuccess, preprocessorConfig, line, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, preprocessorConfig, line, expectedResultPartialSuccess };
                yield return new object[] { configFailure, preprocessorConfig, line, expectedResultFailure };
            }
        }
    }
}