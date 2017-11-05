using System.Linq;
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
            _singleLineScanner = new SingleLineScanner(config);

            _lines = new LimitedQueue<string>(BufferSize)
                .WithAmendOperator((s1, s2) => s1 + s2);
        }

        public void AppendLine(string line)
        {
            line = Preprocess(line);

            if (TryAmend(line) || TryEnqueue(line))
            {
                string joinedLines = string.Join(string.Empty, _lines);

                _singleLineScanner.AppendLine(joinedLines);
            }
        }

        public ScanResult Scan()
        {
            ScanResult result = _singleLineScanner.Scan();

            if (result.IsSuccess)
            {
                _lines.Clear();
            }

            return result;
        }

        private string Preprocess(string line)
        {
            line = line.Trim(_config.TrimPatterns);

            return line;
        }

        private bool TryAmend(string line)
        {
            string concatOperator = _config.ConcatOperators
                .FirstOrDefault(concatOper => line.StartsWith(concatOper));

            if (concatOperator != null)
            {
                line = line.Remove(0, concatOperator.Length);
                line = Preprocess(line);

                _lines.Amend(line);

                return true;
            }

            return false;
        }

        private bool TryEnqueue(string line)
        {
            _lines.Enqueue(line);

            return true;
        }
    }
}