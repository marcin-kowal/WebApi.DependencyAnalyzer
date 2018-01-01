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
        private static class Parameters
        {
            public const char OperandSeparator = ',';
            public static readonly string[] AppendTokens = new[] { "+" };
            public static readonly string[] InstructionTokens = new[] { "IL_[0-9a-fA-F]+:" };
            public static readonly string[] MultilineOperationBeginTokens = new[] { @"System.String::Format\(" };
            public static readonly string[] MultilineOperationEndTokens = new[] { @"\)" };
            public static readonly string[] OperandOperationTokens = new[] { "ldstr" };
            public static readonly string[] SimpleOperationTokens = new[] { "ldstr", "ldfld", "ldc" };
        }

        private const int BufferSize = 12;

        private readonly IScannerConfig _config;
        private readonly IScanPreprocessor _preprocessor;
        private readonly IHashProvider<string> _hashProvider;
        private readonly SingleLineScanner _singleLineScanner;
        private readonly LimitedStack<Line> _lines;
        private readonly HashSet<ScanResult> _results;
        private bool _multilineOperationInProgress;

        public MultiLineScanner(
            IScannerConfig config, 
            IScanPreprocessor preprocessor,
            IHashProvider<string> hashProvider)
        {
            _config = config;
            _preprocessor = preprocessor;
            _hashProvider = hashProvider;
            _singleLineScanner = new SingleLineScanner(config, preprocessor, hashProvider);

            _lines = new LimitedStack<Line>(BufferSize);
            _results = new HashSet<ScanResult>();
        }

        public void AppendLine(string text)
        {
            Line line = new Line(_preprocessor.TrimStart(text), _hashProvider.GetHash(text));

            if (TryAppendToLastLine(line))
            {
                TryFinishMultilineOperation(line);

                return;
            }

            if (line.Text.StartsWithAny(Parameters.InstructionTokens))
            {
                text = _preprocessor.Preprocess(line.Text, Parameters.InstructionTokens);
                line = new Line(text, line.Hashes);

                _lines.Push(line);

                TryStartMultilineOperation(line);
                TryFinishMultilineOperation(line);
            }
        }

        private bool TryAppendToLastLine(Line line)
        {
            if (_multilineOperationInProgress)
            {
                AppendToLastLine(line);
                return true;
            }

            string appendToken = Parameters.AppendTokens
                .FirstOrDefault(token => line.Text.StartsWith(token, StringComparison.OrdinalIgnoreCase));

            if (appendToken != null)
            {
                string text = line.Text.Remove(0, appendToken.Length);
                text = _preprocessor.TrimStart(text);

                AppendToLastLine(new Line(text, line.Hashes));

                return true;
            }

            return false;
        }

        private void AppendToLastLine(Line line)
        {
            Line lastLine = _lines.Pop();

            if (!string.IsNullOrEmpty(lastLine.Text))
            {
                Line newLine = lastLine.Append(line);

                _lines.Push(newLine);
            }
        }

        private bool TryStartMultilineOperation(Line line)
        {
            if (line.Text.ContainsAny(Parameters.MultilineOperationBeginTokens))
            {
                _multilineOperationInProgress = true;

                return true;
            }

            return false;
        }

        private bool TryFinishMultilineOperation(Line line)
        {
            if (line.Text.ContainsAny(Parameters.MultilineOperationEndTokens))
            {
                _multilineOperationInProgress = false;

                return true;
            }

            return false;
        }

        public void Scan()
        {
            Line[] lines = _lines.ToArray();

            if (!_multilineOperationInProgress)
            {
                (int? lineIndex, Line line) multilineOperation = TryGetMultilineOperation(lines);

                if (multilineOperation.lineIndex >= 0 && multilineOperation.line != null)
                {
                    IReadOnlyCollection<Line> operandLines = GetMultilineOperands(
                        lines,
                        multilineOperation.lineIndex.Value,
                        multilineOperation.line);

                    long[] operandLineHashes = operandLines
                        .SelectMany(line => line.Hashes)
                        .Distinct()
                        .ToArray();

                    string completeLine = BuildCompleteLine(operandLines);
                    IReadOnlyCollection<ScanResult> scanResults = RunSingleLineScanner(completeLine);

                    UpdateResults(_results, scanResults, operandLineHashes);
                }
            }
        }

        private (int? lineIndex, Line line) TryGetMultilineOperation(Line[] lines)
        {
            if (lines.Any())
            {
                int lineIndex = Array.FindLastIndex(lines,
                    l => l.Text.ContainsAny(Parameters.MultilineOperationBeginTokens));

                if (lineIndex >= 0)
                {
                    Line line = lines[lineIndex];

                    return (lineIndex, line);
                }
            }

            return (null, null);
        }

        private IReadOnlyCollection<Line> GetMultilineOperands(
            Line[] lines, 
            int multilineOperationLineIndex,
            Line multilineOperationLine)
        {
            int numberOfOperands = 1 
                + multilineOperationLine.Text.Count(chr => chr == Parameters.OperandSeparator);

            if (multilineOperationLineIndex < numberOfOperands)
            {
                throw new InvalidOperationException("Not enough operands for multiline operation. Buffer too small. " +
                    $"Number of available operands: {multilineOperationLineIndex}. Number of operation arguments: {numberOfOperands}.");
            }

            Line[] operandLines = lines
                .Take(multilineOperationLineIndex)
                .Reverse()
                .Where(line => line.Text.StartsWithAny(Parameters.SimpleOperationTokens))
                .Take(numberOfOperands)
                .Reverse()
                .ToArray();

            return operandLines;
        }

        private string BuildCompleteLine(IReadOnlyCollection<Line> operandLines)
        {
            int index = 0;

            IEnumerable<string> operands = operandLines
                .Select((line) => line.Text.StartsWithAny(Parameters.OperandOperationTokens)
                    ? _preprocessor.Preprocess(line.Text, Parameters.SimpleOperationTokens)
                    : $"{{{index++}}}");

            string completeLine = string.Format(CultureInfo.InvariantCulture, operands.First(), operands.Skip(1).ToArray());

            return completeLine;
        }

        private IReadOnlyCollection<ScanResult> RunSingleLineScanner(string line)
        {
            _singleLineScanner.AppendLine(line);

            _singleLineScanner.Scan();

            IReadOnlyCollection<ScanResult> results = _singleLineScanner.GetResult()
                .Where(result => result.IsSuccess)
                .ToArray();

            return results;
        }

        private void UpdateResults(
            HashSet<ScanResult> resultsToUpdate, 
            IReadOnlyCollection<ScanResult> scanResults,
            long[] lineHashes)
        {
            foreach (long hash in lineHashes)
            {
                resultsToUpdate.RemoveWhere(result => result.LineHashes.Contains(hash));
            }

            foreach (ScanResult result in scanResults)
            {
                resultsToUpdate.Add(ScanResult.Success(result.Value, lineHashes));
            }
        }

        public IReadOnlyCollection<ScanResult> GetResult()
        {
            return _results.Any()
                ? _results.ToArray()
                : new[] { ScanResult.Failure() };
        }
    }
}