using System.Text.RegularExpressions;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public class ScanPreprocessor : IScanPreprocessor
    {
        private readonly IPreprocessorConfig _config;

        public ScanPreprocessor(IPreprocessorConfig config)
        {
            _config = config;
        }

        public string Preprocess(string text)
        {
            foreach (string token in _config.RemoveTokens)
            {
                text = new Regex(token).Replace(text, string.Empty);
            }

            text = text.Trim(_config.TrimTokens);

            return text;
        }
    }
}