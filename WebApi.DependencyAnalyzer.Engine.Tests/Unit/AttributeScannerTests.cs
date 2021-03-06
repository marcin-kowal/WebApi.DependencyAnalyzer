﻿using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class AttributeScannerTests
    {
        [Theory]
        [MemberData(nameof(ScanData))]
        public void ScanTest(
            IScannerConfig config,
            IHashProvider<string> hashProvider,
            string[] lines,
            ScanResult[] expectedResult)
        {
            AttributeScanner scanner = new AttributeScanner(config, new ScanPreprocessor(), hashProvider);

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

                string[] lines = new[]
                {
                    "MyAttribute ( // line: tex",
                    "00 // t to s",
                    "1A) // earch)"
                };

                long[] linesHashes = lines
                    .Select(l => (long)l.Length)
                    .ToArray();

                IHashProvider<string> hashProvider = Substitute.For<IHashProvider<string>>();
                hashProvider.GetHash(Arg.Any<string>()).Returns(args => ((string)args[0]).Length);

                IScannerConfig configSuccess = CreateScannerConfig(textSearchPatterns);
                IScannerConfig configPartialSuccess = CreateScannerConfig(textSearchPatterns, new[] { "ine" });
                IScannerConfig configFailure = CreateScannerConfig(textSearchPatterns, new[] { "xt" });

                ScanResult[] expectedResultSuccess = new[] { "xt to search", "line: text to" }
                    .Select(result => ScanResult.Success(result, linesHashes))
                    .ToArray();
                ScanResult[] expectedResultPartialSuccess = new[] { ScanResult.Success("xt to search", linesHashes) };
                ScanResult[] expectedResultFailure = new[] { ScanResult.Failure() };

                yield return new object[] { configSuccess, hashProvider, lines, expectedResultSuccess };
                yield return new object[] { configPartialSuccess, hashProvider, lines, expectedResultPartialSuccess };
                yield return new object[] { configFailure, hashProvider, lines, expectedResultFailure };
            }
        }

        private static IScannerConfig CreateScannerConfig(
            string[] textSearchPatterns,
            string[] textSearchPatternsExclude = null)
        {
            IScannerConfig config = Substitute.For<IScannerConfig>();
            config.TextSearchPatterns.Returns(textSearchPatterns);

            if (textSearchPatternsExclude != null)
                config.TextSearchPatternsExclude.Returns(textSearchPatternsExclude);

            return config;
        }
    }
}