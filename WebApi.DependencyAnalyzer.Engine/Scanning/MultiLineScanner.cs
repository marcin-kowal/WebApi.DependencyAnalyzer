using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class MultiLineScanner : IScanner
    {
        private const int BufferSize = 5;

        private readonly IScannerConfig _config;
        private readonly LimitedQueue<string> _lines;
        private readonly SingleLineScanner _singleLineScanner;

        public MultiLineScanner(IScannerConfig config)
        {
            _config = config;
            _lines = new LimitedQueue<string>(BufferSize);
            _singleLineScanner = new SingleLineScanner(config);
        }

        public void AppendLine(string line)
        {
            _lines.Enqueue(line);

            string joinedLines = string.Join(string.Empty, _lines);

            _singleLineScanner.AppendLine(joinedLines);
        }

        public ScanResult Scan()
        {
            return _singleLineScanner.Scan();
        }
    }
}