using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal abstract class MultiLineScanner : IScanner
    {
        protected readonly IScannerConfig _config;
        protected readonly IScanPreprocessor _preprocessor;
        protected readonly IHashProvider<string> _hashProvider;
        protected readonly SingleLineScanner _singleLineScanner;
        protected readonly LimitedStack<Line> _lines;
        protected readonly HashSet<ScanResult> _results;
        protected bool _multilineOperationInProgress;

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

        protected abstract int BufferSize { get; }
        protected abstract char OperandSeparator { get; }
        protected abstract string[] InstructionTokens { get; }
        protected abstract string[] SimpleOperationTokens { get; }
        protected abstract string[] OperandOperationTokens { get; }
        protected abstract string[] AppendTokens { get; }
        protected abstract string[] MultilineOperationBeginTokens { get; }
        protected abstract string[] MultilineOperationEndTokens { get; }

        public void AppendLine(string text)
        {
            Line line = new Line(_preprocessor.TrimStart(text), _hashProvider.GetHash(text));

            if (TryAppendToLastLine(line))
            {
                TryFinishMultilineOperation(line);

                return;
            }

            if (line.Text.StartsWithAny(InstructionTokens))
            {
                text = PrepareToPush(line.Text);

                _lines.Push(line.ReplaceText(text));

                TryStartMultilineOperation(line);
                TryFinishMultilineOperation(line);
            }
        }

        protected abstract string PrepareToPush(string text);

        private bool TryAppendToLastLine(Line line)
        {
            if (_multilineOperationInProgress)
            {
                string text = PrepareToAppend(line.Text);
                AppendToLastLine(line.ReplaceText(text));
                return true;
            }

            string appendToken = AppendTokens
                .FirstOrDefault(token => line.Text.StartsWith(token, StringComparison.OrdinalIgnoreCase));

            if (appendToken != null)
            {
                string text = PrepareToAppend(line.Text, appendToken);
                AppendToLastLine(line.ReplaceText(text));

                return true;
            }

            return false;
        }

        protected abstract string PrepareToAppend(string text, string appendToken = null);

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
            if (line.Text.ContainsAny(MultilineOperationBeginTokens))
            {
                _multilineOperationInProgress = true;

                return true;
            }

            return false;
        }

        private bool TryFinishMultilineOperation(Line line)
        {
            if (line.Text.ContainsAny(MultilineOperationEndTokens))
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
                    l => l.Text.ContainsAny(MultilineOperationBeginTokens));

                if (lineIndex >= 0)
                {
                    Line line = lines[lineIndex];

                    return (lineIndex, line);
                }
            }

            return (null, null);
        }

        protected abstract IReadOnlyCollection<Line> GetMultilineOperands(
            IReadOnlyCollection<Line> lines,
            int multilineOperationLineIndex,
            Line multilineOperationLine);

        protected abstract string BuildCompleteLine(IReadOnlyCollection<Line> operandLines);

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