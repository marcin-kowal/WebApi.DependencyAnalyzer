using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class SingleLineScanner : IScanner
    {
        private readonly IScannerConfig _config;
        private readonly IScanPreprocessor _preprocessor;
        private readonly HashSet<ScanResult> _result;
        private string _line;

        public SingleLineScanner(IScannerConfig config, IScanPreprocessor preprocessor)
        {
            _config = config;
            _preprocessor = preprocessor;
            _result = new HashSet<ScanResult>();
        }

        public void AppendLine(string line)
        {
            line = _preprocessor.Preprocess(line, _config.InstructionTokens);

            _line = line;
        }

        public void Scan()
        {
            IEnumerable<string> matches = _config.TextSearchPatterns
                .Select(pattern => new Regex(pattern).Match(_line))
                .Where(match => match.Success)
                .Select(match => match.Value);

            matches = matches
                .Where(match => !_config.TextSearchPatternsExclude
                    .Any(excludePattern => new Regex(excludePattern).IsMatch(match)));

            foreach (string match in matches)
            {
                _result.Add(ScanResult.Success(match));
            }
        }

        public void Reset()
        {
            _line = null;
            _result.Clear();
        }

        public IReadOnlyCollection<ScanResult> GetResult()
        {
            return _result.Any()
                ? _result.ToArray()
                : new[] { ScanResult.Failure() };
        }
    }
}