using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Scanning;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class CompositeScannerTests
    {
        [Theory]
        [MemberData(nameof(AppendLineData))]
        public void AppendLineTest(IScanner[] scanners, string line)
        {
            CompositeScanner compositeScanner = new CompositeScanner();

            foreach (IScanner scanner in scanners)
            { compositeScanner.AddScanner(scanner); }

            compositeScanner.AppendLine(line);

            foreach (IScanner scanner in scanners)
            { scanner.Received(1).AppendLine(line); }
        }

        public static IEnumerable<object[]> AppendLineData
        {
            get
            {
                IScanner[] scanners = new[]
                {
                    Substitute.For<IScanner>(),
                    Substitute.For<IScanner>()
                };

                string line = "line";

                yield return new object[] { scanners, line };
            }
        }

        [Theory]
        [MemberData(nameof(ScanData))]
        public void ScanTest(IScanner[] scanners, ScanResult[] expectedResult)
        {
            CompositeScanner compositeScanner = new CompositeScanner();

            foreach (IScanner scanner in scanners)
            { compositeScanner.AddScanner(scanner); }

            compositeScanner.Scan();

            IReadOnlyCollection<ScanResult> results = compositeScanner.GetResult();

            results.ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> ScanData
        {
            get
            {
                string[] scanResults1 = new[] { "aaa", "bbbb" };
                string[] scanResults2 = new[] { "ccccc" };
                string[] scanResultsTotal = scanResults1
                    .Concat(scanResults2)
                    .ToArray();

                IScanner[] scannersNoSuccess = new[]
                {
                    GetScanner(false, null),
                    GetScanner(false, scanResults2),
                };

                IScanner[] scannersSuccess = new[]
                {
                    GetScanner(true, scanResults1),
                    GetScanner(true, scanResults2),
                };

                ScanResult[] expectedResultNoSuccess = new[] { ScanResult.Failure() };
                ScanResult[] expectedResultSuccess = scanResultsTotal
                    .Select(result => ScanResult.Success(result, result.Length))
                    .ToArray();

                yield return new object[] { scannersNoSuccess, expectedResultNoSuccess };
                yield return new object[] { scannersSuccess, expectedResultSuccess };
            }
        }

        private static IScanner GetScanner(bool success, string[] values)
        {
            IScanner scanner = Substitute.For<IScanner>();

            scanner.GetResult().Returns(success 
                ? values.Select(value => ScanResult.Success(value, value.Length)).ToArray() 
                : new[] { ScanResult.Failure() });

            return scanner;
        }
    }
}