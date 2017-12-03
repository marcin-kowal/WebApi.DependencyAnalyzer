using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public void Reset()
        {
            Parallel.ForEach(_scanners, scanner => scanner.Reset());
        }

        public IReadOnlyCollection<ScanResult> GetResult()
        {
            HashSet<ScanResult> results = new HashSet<ScanResult>(_scanners
                .AsParallel()
                .SelectMany(scanner => scanner.GetResult())
                .Where(scan => scan.IsSuccess)
                .Select(scan => ScanResult.Success(scan.Value)));

            ScanResult[] result = results.Any()
                ? results.ToArray()
                : new[] { ScanResult.Failure() };

            return result;
        }
    }
}