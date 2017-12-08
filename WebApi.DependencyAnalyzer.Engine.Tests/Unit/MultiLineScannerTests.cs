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
    public class MultiLineScannerTests
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
            SingleLineScanner singleLineScanner = new SingleLineScanner(config, new ScanPreprocessor(preprocessorConfig), hashProvider);
            MultiLineScanner scanner = new MultiLineScanner(config, new ScanPreprocessor(preprocessorConfig), hashProvider);

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

                string[] lines = new[] { "INS LD {0} to search", "INS LD line: text", "INS text (,)@" };
                long[] linesHashes = lines
                    .Take(lines.Length - 1)
                    .Select(l => (long)l.Length)
                    .ToArray();

                string[] operandOperationTokens = new[] { "LD" };
                string[] simpleOperationTokens = new[] { "LD" };
                string[] instructionTokens = new[] { "INS" };
                string[] multilineOperationBeginTokens = new[] { @"\(" };
                string[] multilineOperationEndTokens = new[] { @"\)" };
                char operandSeparator = ',';

                IPreprocessorConfig preprocessorConfig = Substitute.For<IPreprocessorConfig>();
                preprocessorConfig.TrimTokens.Returns(trimPatterns);

                IHashProvider<string> hashProvider = Substitute.For<IHashProvider<string>>();
                hashProvider.GetHash(Arg.Any<string>()).Returns(args => ((string)args[0]).Length);

                IScannerConfig configSuccess = CreateScannerConfig(textSearchPatterns, instructionTokens, operandOperationTokens, simpleOperationTokens, null, multilineOperationBeginTokens, multilineOperationEndTokens, operandSeparator);
                IScannerConfig configPartialSuccess = CreateScannerConfig(textSearchPatterns, instructionTokens, operandOperationTokens, simpleOperationTokens, new[] { "ine" }, multilineOperationBeginTokens, multilineOperationEndTokens, operandSeparator);
                IScannerConfig configFailure = CreateScannerConfig(textSearchPatterns, instructionTokens, operandOperationTokens, simpleOperationTokens, new[] { "xt" });

                ScanResult[] expectedResultSuccess = new[] { "xt to search", "line: text to" }
                    .Select(result => ScanResult.Success(result, linesHashes))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccess = new[] { ScanResult.Success("xt to search", linesHashes) };
                ScanResult[] expectedResultFailure = new[] { ScanResult.Failure() };

                yield return new object[] { configSuccess, preprocessorConfig, hashProvider, lines, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, preprocessorConfig, hashProvider, lines, expectedResultPartialSuccess };
                yield return new object[] { configFailure, preprocessorConfig, hashProvider, lines, expectedResultFailure };
            }
        }

        private static IScannerConfig CreateScannerConfig(
            string[] textSearchPatterns,
            string[] instructionTokens,
            string[] operandOperationTokens,
            string[] simpleOperationTokens,
            string[] textSearchPatternsExclude = null,
            string[] multilineOperationBeginTokens = null,
            string[] multilineOperationEndTokens = null,
            char? operandSeparator = null)
        {
            IScannerConfig config = Substitute.For<IScannerConfig>();
            config.InstructionTokens.Returns(instructionTokens);
            config.OperandOperationTokens.Returns(operandOperationTokens);
            config.SimpleOperationTokens.Returns(simpleOperationTokens);
            config.TextSearchPatterns.Returns(textSearchPatterns);

            if (textSearchPatternsExclude != null)
                config.TextSearchPatternsExclude.Returns(textSearchPatternsExclude);

            if (multilineOperationBeginTokens != null)
                config.MultilineOperationBeginTokens.Returns(multilineOperationBeginTokens);

            if (multilineOperationEndTokens != null)
                config.MultilineOperationEndTokens.Returns(multilineOperationEndTokens);

            if (operandSeparator.HasValue)
                config.OperandSeparator.Returns(operandSeparator.Value);

            return config;
        }
    }
}