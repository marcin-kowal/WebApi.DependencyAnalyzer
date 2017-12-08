using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Common;
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
            IHashProvider<string> hashProvider,
            string[] lines, 
            ScanResult[] expectedResult)
        {
            SingleLineScanner scanner = new SingleLineScanner(config, new ScanPreprocessor(preprocessorConfig), hashProvider);

            foreach (string line in lines)
            {
                scanner.AppendLine(line);
                scanner.Scan();
            }

            IReadOnlyCollection<ScanResult> result = scanner.GetResult();

            result.ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> ScanData
        {
            get
            {
                string[] textSearchPatterns = new[] { "xt.*", ".* to" };
                char[] trimPatterns = new[] { ' ', '@' };
                string[] appendTokens = new[] { "+" };

                string line = " line: text to search@";
                string[] lines = new[] { " line: text to s", "+ earch@" };

                long lineHash = line.Length;
                long[] linesHashes = lines
                    .Select(l => (long)l.Length)
                    .ToArray();

                IPreprocessorConfig preprocessorConfig = Substitute.For<IPreprocessorConfig>();
                preprocessorConfig.TrimTokens.Returns(trimPatterns);

                IHashProvider<string> hashProvider = Substitute.For<IHashProvider<string>>();
                hashProvider.GetHash(Arg.Any<string>()).Returns(args => ((string)args[0]).Length);

                IScannerConfig configSuccess = CreateScannerConfig(textSearchPatterns, appendTokens);
                IScannerConfig configPartialSuccess = CreateScannerConfig(textSearchPatterns, appendTokens, new[] { "ine" });
                IScannerConfig configFailure = CreateScannerConfig(textSearchPatterns, appendTokens, new[] { "xt" });

                ScanResult[] expectedResultSuccess = new[] { "xt to search", "line: text to" }
                    .Select(result => ScanResult.Success(result, lineHash))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccess = new[] { ScanResult.Success("xt to search", lineHash) };
                ScanResult[] expectedResultFailure = new[] { ScanResult.Failure() };

                ScanResult[] expectedResultSuccessLines = new[] { "xt to search", "line: text to" }
                    .Select(result => ScanResult.Success(result, linesHashes))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccessLines = new[] { ScanResult.Success("xt to search", linesHashes) };

                yield return new object[] { configSuccess, preprocessorConfig, hashProvider, new[] { line }, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, preprocessorConfig, hashProvider, new[] { line }, expectedResultPartialSuccess };
                yield return new object[] { configFailure, preprocessorConfig, hashProvider, new[] { line }, expectedResultFailure };

                yield return new object[] { configSuccess, preprocessorConfig, hashProvider, lines, expectedResultSuccessLines };
                yield return new object[] { configPartialSuccess, preprocessorConfig, hashProvider, lines, expectedResultPartialSuccessLines };
                yield return new object[] { configFailure, preprocessorConfig, hashProvider, lines, expectedResultFailure };
            }
        }

        private static IScannerConfig CreateScannerConfig(
            string[] textSearchPatterns, 
            string[] appendTokens,
            string[] textSearchPatternsExclude = null)
        {
            IScannerConfig scannerConfig = Substitute.For<IScannerConfig>();
            scannerConfig.TextSearchPatterns.Returns(textSearchPatterns);
            scannerConfig.AppendTokens.Returns(appendTokens);

            if (textSearchPatternsExclude != null)
            {
                scannerConfig.TextSearchPatternsExclude.Returns(textSearchPatternsExclude);
            }

            return scannerConfig;
        }
    }
}