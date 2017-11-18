using System;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class MultiLineScanner : IScanner
    {
        private const int BufferSize = 5;

        private readonly IScannerConfig _config;
        private readonly IScanPreprocessor _preprocessor;
        private readonly LimitedQueue<string> _lines;
        private readonly SingleLineScanner _singleLineScanner;

        public MultiLineScanner(IScannerConfig config, IScanPreprocessor preprocessor)
        {
            _config = config;
            _preprocessor = preprocessor;
            _singleLineScanner = new SingleLineScanner(config, preprocessor);

            _lines = new LimitedQueue<string>(BufferSize);
        }

        public void AppendLine(string line)
        {
            line = _preprocessor.Preprocess(line);

            if (TryAppendToLastLine(line) || TryEnqueue(line))
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
                bool inProgress;
                bool repeatScan = false;
                string lastLine = _lines.Unenqueue();

                do
                {
                    inProgress = false;

                    string previousLine = _lines.PeekLast();

                    if (!string.IsNullOrEmpty(previousLine)
                        && _config.PrependTokens.Any(token => previousLine.StartsWith(token, StringComparison.OrdinalIgnoreCase)))
                    {
                        TryPrependToLastLine(lastLine);

                        if (_lines.Count > 1)
                        {
                            inProgress = true;
                        }
                        repeatScan = true;
                    }
                }
                while (inProgress);

                if (repeatScan)
                {
                    lastLine = _lines.Unenqueue();
                    _singleLineScanner.AppendLine(lastLine);
                    result = _singleLineScanner.Scan();
                }

                _lines.Clear();
            }

            return result;
        }

        private bool TryAppendToLastLine(string text)
        {
            string appendToken = _config.AppendTokens
                .FirstOrDefault(token => text.StartsWith(token, StringComparison.OrdinalIgnoreCase));

            if (appendToken != null)
            {
                text = text.Remove(0, appendToken.Length);
                text = _preprocessor.Preprocess(text);

                string lastLine = _lines.Unenqueue();
                _lines.Enqueue(lastLine.Append(text));

                return true;
            }

            return false;
        }

        private bool TryPrependToLastLine(string text)
        {
            string lastLine = _lines.Unenqueue();

            string prependToken = _config.PrependTokens
                .FirstOrDefault(token => lastLine.StartsWith(token, StringComparison.OrdinalIgnoreCase));

            if (prependToken != null)
            {
                lastLine = lastLine.Remove(0, prependToken.Length);
                lastLine = _preprocessor.Preprocess(lastLine);
                
                _lines.Enqueue(lastLine.Prepend(text));

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