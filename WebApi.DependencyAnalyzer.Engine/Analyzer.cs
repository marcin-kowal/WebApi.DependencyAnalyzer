using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Decompilation;
using WebApi.DependencyAnalyzer.Engine.FileSystem;
using WebApi.DependencyAnalyzer.Engine.Scanning;

namespace WebApi.DependencyAnalyzer.Engine
{
    internal class Analyzer : IAnalyzer
    {
        private readonly IFileProvider _fileProvider;
        private readonly IDecompiler _decompiler;
        private readonly IScanner _scanner;

        public Analyzer(
            IFileProvider fileProvider,
            IDecompiler decompiler,
            IScanner scanner)
        {
            _fileProvider = fileProvider;
            _decompiler = decompiler;
            _scanner = scanner;
        }

        public string[] Analyze()
        {
            SortedSet<string> scanResults = new SortedSet<string>();

            string[] files = _fileProvider.GetFiles();

            foreach (string file in files)
            {
                string[] results = AnalyzeFile(file);

                foreach (string result in results)
                {
                    scanResults.Add(result);
                }
            }

            return scanResults.ToArray();
        }

        private string[] AnalyzeFile(string file)
        {
            List<string> result = new List<string>();

            using (_decompiler)
            {
                _decompiler.Open(file);

                while (!_decompiler.EndOfStream)
                {
                    string line = _decompiler.ReadLine();

                    _scanner.AppendLine(line);
                    ScanResult scanResult = _scanner.Scan();

                    if (scanResult.IsSuccess)
                    {
                        result.AddRange(scanResult.Values);
                    }
                }

                _decompiler.Close();
            }

            return result.ToArray();
        }
    }
}