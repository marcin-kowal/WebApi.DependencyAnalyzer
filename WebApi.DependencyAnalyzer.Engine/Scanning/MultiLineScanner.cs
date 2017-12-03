using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly Dictionary<string, IReadOnlyCollection<ScanResult>> _results;
        private bool _multilineOperationInProgress;

        public MultiLineScanner(IScannerConfig config, IScanPreprocessor preprocessor)
        {
            _config = config;
            _preprocessor = preprocessor;
            _singleLineScanner = new SingleLineScanner(config, preprocessor);

            _lines = new LimitedQueue<string>(BufferSize);
            _results = new Dictionary<string, IReadOnlyCollection<ScanResult>>();
        }

        public void AppendLine(string line)
        {
            line = _preprocessor.Trim(line);

            if (TryAppendToLastLine(line))
            {
                if (line.Contains(_config.MultilineOperationEndTokens))
                {
                    _multilineOperationInProgress = false;
                }

                return;
            }

            if (line.StartsWith(_config.InstructionTokens))
            {
                line = _preprocessor.Preprocess(line, _config.InstructionTokens);

                if (line.StartsWith(_config.SimpleOperationTokens))
                {
                    _lines.Enqueue(line);
                }
                else if (line.Contains(_config.MultilineOperationBeginTokens))
                {
                    _multilineOperationInProgress = true;
                    _lines.Enqueue(line);
                }
            }
        }

        public void Scan()
        {
            string[] lines = _lines.ToArray();

            foreach (string line in lines)
            {
                if (_results.ContainsKey(line))
                {
                    continue;
                }

                _singleLineScanner.Reset();
                _singleLineScanner.AppendLine(line);
                _singleLineScanner.Scan();
                IReadOnlyCollection<ScanResult> result = _singleLineScanner.GetResult();

                if (result.Any(res => res.IsSuccess))
                {
                    _results.Add(line, result.Where(res => res.IsSuccess).ToArray());
                }
            }

            if (!_multilineOperationInProgress && lines.Any())
            {
                int multilineOperationLineIndex = Array.FindLastIndex(lines,
                    line => line.Contains(_config.MultilineOperationBeginTokens));

                if (multilineOperationLineIndex >= 0)
                {
                    string multilineOperationLine = lines[multilineOperationLineIndex];

                    int numberOfOperands = 1 + multilineOperationLine
                        .Count(chr => chr == _config.OperandSeparator);

                    if (multilineOperationLineIndex < numberOfOperands)
                    {
                        throw new InvalidOperationException("Not enough operands for multiline operation. Buffer too small. " +
                            $"Number of available operands: {multilineOperationLineIndex}. Number of operation arguments: {numberOfOperands}.");
                    }

                    IEnumerable<string> operandLines = lines
                        .Take(multilineOperationLineIndex)
                        .Reverse()
                        .Take(numberOfOperands)
                        .Reverse();

                    foreach (string operandLine in operandLines)
                    {
                        _results.Remove(operandLine);
                    }

                    operandLines = operandLines
                        .Select((line, index) => line.StartsWith("ldstr", StringComparison.Ordinal) 
                            ? _preprocessor.Preprocess(line, _config.SimpleOperationTokens)
                            : $"{{{index - 1}}}");

                    string result = string.Format(CultureInfo.InvariantCulture, operandLines.First(), operandLines.Skip(1).ToArray());

                    _results.Add(multilineOperationLine, new[] { ScanResult.Success(result) });
                }
            }
        }

        private bool TryAppendToLastLine(string text)
        {
            if (_multilineOperationInProgress)
            {
                AppendToLastLine(text);
                return true;
            }

            string appendToken = _config.AppendTokens
                .FirstOrDefault(token => text.StartsWith(token, StringComparison.OrdinalIgnoreCase));

            if (appendToken != null)
            {
                text = text.Remove(0, appendToken.Length);
                text = _preprocessor.TrimStart(text);

                AppendToLastLine(text);
                return true;
            }

            return false;
        }

        private void AppendToLastLine(string text)
        {
            string lastLine = _lines.Unenqueue();

            if (!string.IsNullOrEmpty(lastLine))
            {
                _results.Remove(lastLine);
                _lines.Enqueue(lastLine.Append(text));
            }
        }

        public void Reset()
        {
            _lines.Clear();
            _singleLineScanner.Reset();
            _results.Clear();
        }

        public IReadOnlyCollection<ScanResult> GetResult()
        {
            IReadOnlyCollection<ScanResult> result = new HashSet<ScanResult>(_results.Values
                .SelectMany(value => value))
                .ToArray();

            return result;
        }
    }
}