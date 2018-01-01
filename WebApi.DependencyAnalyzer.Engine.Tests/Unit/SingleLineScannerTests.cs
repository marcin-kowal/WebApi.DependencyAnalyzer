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
            IHashProvider<string> hashProvider,
            string[] lines, 
            ScanResult[] expectedResult)
        {
            SingleLineScanner scanner = new SingleLineScanner(config, new ScanPreprocessor(), hashProvider);

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

                string line = " line: text to search ' ";
                string[] lines = new[] { " line: text to s", "+ earch ' " };

                long lineHash = line.Length;
                long[] linesHashes = lines
                    .Select(l => (long)l.Length)
                    .ToArray();

                IHashProvider<string> hashProvider = Substitute.For<IHashProvider<string>>();
                hashProvider.GetHash(Arg.Any<string>()).Returns(args => ((string)args[0]).Length);

                IScannerConfig configSuccess = CreateScannerConfig(textSearchPatterns);
                IScannerConfig configPartialSuccess = CreateScannerConfig(textSearchPatterns, new[] { "ine" });
                IScannerConfig configFailure = CreateScannerConfig(textSearchPatterns, new[] { "xt" });

                ScanResult[] expectedResultSuccess = new[] { "xt to search", "line: text to" }
                    .Select(result => ScanResult.Success(result, lineHash))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccess = new[] { ScanResult.Success("xt to search", lineHash) };
                ScanResult[] expectedResultFailure = new[] { ScanResult.Failure() };

                ScanResult[] expectedResultSuccessLines = new[] { "xt to search", "line: text to" }
                    .Select(result => ScanResult.Success(result, linesHashes))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccessLines = new[] { ScanResult.Success("xt to search", linesHashes) };

                yield return new object[] { configSuccess, hashProvider, new[] { line }, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, hashProvider, new[] { line }, expectedResultPartialSuccess };
                yield return new object[] { configFailure, hashProvider, new[] { line }, expectedResultFailure };

                yield return new object[] { configSuccess, hashProvider, lines, expectedResultSuccessLines };
                yield return new object[] { configPartialSuccess, hashProvider, lines, expectedResultPartialSuccessLines };
                yield return new object[] { configFailure, hashProvider, lines, expectedResultFailure };
            }
        }

        private static IScannerConfig CreateScannerConfig(
            string[] textSearchPatterns, 
            string[] textSearchPatternsExclude = null)
        {
            IScannerConfig scannerConfig = Substitute.For<IScannerConfig>();
            scannerConfig.TextSearchPatterns.Returns(textSearchPatterns);

            if (textSearchPatternsExclude != null)
            {
                scannerConfig.TextSearchPatternsExclude.Returns(textSearchPatternsExclude);
            }

            return scannerConfig;
        }
    }
}