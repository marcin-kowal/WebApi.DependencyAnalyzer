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
        private string _line;

        public SingleLineScanner(IScannerConfig config, IScanPreprocessor preprocessor)
        {
            _config = config;
            _preprocessor = preprocessor;
        }

        public void AppendLine(string line)
        {
            line = _preprocessor.Preprocess(line);

            _line = line;
        }

        public ScanResult Scan()
        {
            IEnumerable<string> matches = _config.TextSearchPatterns
                .Select(pattern => new Regex(pattern).Match(_line))
                .Where(match => match.Success)
                .Select(match => _preprocessor.Preprocess(match.Value));

            matches = matches
                .Where(match => !_config.TextSearchPatternsExclude
                    .Any(excludePattern => new Regex(excludePattern).IsMatch(match)));

            return matches.Any()
                ? ScanResult.Success(matches.ToArray())
                : ScanResult.Failure();
        }
    }
}