using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public class ScanPreprocessor : IScanPreprocessor
    {
        private static class Parameters
        {
            public static readonly char[] TrimTokens = new[] { ' ', '\'', '"' };
        }

        public ScanPreprocessor()
        { }

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
            return text.TrimStart(Parameters.TrimTokens);
        }

        public string TrimEnd(string text)
        {
            return text.TrimEnd(Parameters.TrimTokens);
        }

        public string Trim(string text)
        {
            return text.Trim(Parameters.TrimTokens);
        }
    }
}