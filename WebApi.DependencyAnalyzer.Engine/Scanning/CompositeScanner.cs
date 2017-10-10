using System.Collections.Generic;
using System.Linq;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class CompositeScanner : IScanner
    {
        private List<IScanner> _scanners;

        public CompositeScanner()
        {
            _scanners = new List<IScanner>();
        }

        public void AddScanner(IScanner scanner)
        {
            _scanners.Add(scanner);
        }

        public void AppendLine(string line)
        {
            _scanners
                .AsParallel()
                .ForAll(scanner => scanner.AppendLine(line));
        }

        public ScanResult Scan()
        {
            ScanResult[] results = _scanners
                .AsParallel()
                .Select(scanner => scanner.Scan())
                .Where(scan => scan.IsSuccess)
                .ToArray();

            ScanResult result = results.Any()
                ? ScanResult.Success(results.SelectMany(res => res.Values).Distinct().ToArray())
                : ScanResult.Failure();

            return result;
        }
    }
}