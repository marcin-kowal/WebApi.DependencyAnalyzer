using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class SingleLineScanner : IScanner
    {
        private static class Parameters
        {
            public static readonly string[] AppendTokens = new[] { "+" };
            public static readonly string[] InstructionTokens = new[] { "IL_[0-9a-fA-F]+:" };
        }

        private readonly IScannerConfig _config;
        private readonly IScanPreprocessor _preprocessor;
        private readonly IHashProvider<string> _hashProvider;
        private readonly HashSet<ScanResult> _results;

        private Line _line;

        public SingleLineScanner(
            IScannerConfig config,
            IScanPreprocessor preprocessor,
            IHashProvider<string> hashProvider)
        {
            _config = config;
            _preprocessor = preprocessor;
            _hashProvider = hashProvider;
            _results = new HashSet<ScanResult>();
        }

        public void AppendLine(string text)
        {
            Line line = new Line(_preprocessor.Trim(text), _hashProvider.GetHash(text));

            if (TryAppendToCurrentLine(line))
            {
                return;
            }

            text = _preprocessor.Preprocess(line.Text, Parameters.InstructionTokens);

            _line = line.ReplaceText(text);
        }

        public void Scan()
        {
            IEnumerable<string> matches = _config.TextSearchPatterns
                .Select(pattern => new Regex(pattern).Match(_line.Text))
                .Where(match => match.Success)
                .Select(match => match.Value);

            matches = matches
                .Where(match => !_config.TextSearchPatternsExclude
                    .Any(excludePattern => new Regex(excludePattern).IsMatch(match)));

            foreach (long hash in _line.Hashes)
            {
                _results.RemoveWhere(result => result.LineHashes.Contains(hash));
            }

            foreach (string match in matches)
            {
                _results.Add(ScanResult.Success(match, _line.Hashes));
            }
        }

        public IReadOnlyCollection<ScanResult> GetResult()
        {
            return _results.Any()
                ? _results.ToArray()
                : new[] { ScanResult.Failure() };
        }

        private bool TryAppendToCurrentLine(Line line)
        {
            string appendToken = Parameters.AppendTokens
                .FirstOrDefault(token => line.Text.StartsWith(token, StringComparison.OrdinalIgnoreCase));

            if (appendToken != null)
            {
                string text = line.Text.Remove(0, appendToken.Length);
                text = _preprocessor.TrimStart(text);

                _line = _line.Append(line.ReplaceText(text));

                return true;
            }

            return false;
        }
    }
}