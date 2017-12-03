using System.Collections.Generic;
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

        public string Preprocess(string text, IReadOnlyCollection<string> tokensToRemove)
        {
            text = TrimStart(text);
            
            foreach (string token in tokensToRemove)
            {
                text = new Regex(token).Replace(text, string.Empty);
            }

            text = Trim(text);

            return text;
        }

        public string TrimStart(string text)
        {
            return text.TrimStart(_config.TrimTokens);
        }

        public string TrimEnd(string text)
        {
            return text.TrimEnd(_config.TrimTokens);
        }

        public string Trim(string text)
        {
            return text.Trim(_config.TrimTokens);
        }
    }
}