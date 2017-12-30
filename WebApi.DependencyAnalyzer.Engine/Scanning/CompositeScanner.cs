using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;

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

        public void Scan()
        {
            Parallel.ForEach(_scanners, scanner => scanner.Scan());
        }

        public IReadOnlyCollection<ScanResult> GetResult()
        {
            HashSet<ScanResult> results = new HashSet<ScanResult>();

            foreach (IScanner scanner in _scanners)
            {
                IEnumerable<long> previousResultsHashes = results.SelectMany(res => res.LineHashes);

                var scannerResultsToAdd = scanner.GetResult()
                    .Where(scan => scan.IsSuccess && !previousResultsHashes.Any(hash => scan.LineHashes.Contains(hash)));

                foreach (ScanResult scannerResult in scannerResultsToAdd)
                {
                    results.Add(scannerResult);
                }
            }

            ScanResult[] result = results.Any()
                ? results.ToArray()
                : new[] { ScanResult.Failure() };

            return result;
        }
    }
}