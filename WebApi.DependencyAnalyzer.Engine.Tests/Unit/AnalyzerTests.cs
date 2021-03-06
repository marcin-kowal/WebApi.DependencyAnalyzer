﻿using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Decompilation;
using WebApi.DependencyAnalyzer.Engine.FileSystem;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class AnalyzerTests
    {
        [Theory]
        [MemberData(nameof(AnalyzeData))]
        public void AnalyzeTest(
            IFileProvider fileProvider,
            IDecompiler decompiler,
            IScanner scanner,
            string[] expectedResult)
        {
            Analyzer analyzer = new Analyzer(fileProvider, decompiler, scanner);

            IReadOnlyCollection<string> result = analyzer.Analyze();

            result.ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> AnalyzeData
        {
            get
            {
                string[] files = new[] { "file1", "file2" };
                IFileProvider fileProvider = Substitute.For<IFileProvider>();
                fileProvider.GetFiles().Returns(files);

                bool[] endOfStreams = new[]
                {
                    false, false, false, true,
                    false, true
                };
                string[] lines = new[]
                {
                    "file1 line1", "file1 line2",
                    "file2 line3"
                };
                long[] lineHashes = new long[] { lines[0].Length, lines[1].Length, lines[2].Length };

                IDecompiler decompiler1 = Substitute.For<IDecompiler>();
                decompiler1.EndOfStream.Returns(endOfStreams[0], endOfStreams.Skip(1).ToArray());
                decompiler1.ReadLine().Returns(lines[0], lines.Skip(1).ToArray());
                IDecompiler decompiler2 = Substitute.For<IDecompiler>();
                decompiler2.EndOfStream.Returns(endOfStreams[0], endOfStreams.Skip(1).ToArray());
                decompiler2.ReadLine().Returns(lines[0], lines.Skip(1).ToArray());

                IScanner scanner1 = Substitute.For<IScanner>();
                scanner1.GetResult().Returns(new[] {
                    ScanResult.Success("line", lineHashes[0]), ScanResult.Failure(),
                    ScanResult.Success("line", lineHashes[2]) });

                IScanner scanner2 = Substitute.For<IScanner>();
                scanner2.GetResult().Returns(new[] {
                    ScanResult.Success("line1", lineHashes[0]), ScanResult.Success("line2", lineHashes[1]),
                    ScanResult.Success("line3", lineHashes[2]) });

                string[] expectedResult1 = new[] { "line" };
                string[] expectedResult2 = new[] { "line1", "line2", "line3" };

                yield return new object[] { fileProvider, decompiler1, scanner1, expectedResult1 };
                yield return new object[] { fileProvider, decompiler2, scanner2, expectedResult2 };
            }
        }
    }
}