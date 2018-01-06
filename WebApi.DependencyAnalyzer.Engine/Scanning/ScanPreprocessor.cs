using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public class ScanPreprocessor : IScanPreprocessor
    {
        private static class Parameters
        {
            public static readonly char[] TrimChars = new[] { ' ', '\'', '"' };
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

        public string TrimStart(string text) => text.TrimStart(Parameters.TrimChars);

        public string TrimStart(string text, char[] trimChars) => text.TrimStart(trimChars);

        public string TrimEnd(string text) => text.TrimEnd(Parameters.TrimChars);

        public string TrimEnd(string text, char[] trimChars) => text.TrimEnd(trimChars);

        public string Trim(string text) => text.Trim(Parameters.TrimChars);

        public string Trim(string text, char[] trimChars) => text.Trim(trimChars);

        public string TrimStartTo(string text, IReadOnlyCollection<string> tokens)
        {
            int lastIndex = 0;

            foreach (string token in tokens)
            {
                Match match = new Regex(token).Match(text, lastIndex);

                if (match.Success)
                {
                    lastIndex = Math.Max(lastIndex, match.Index);
                }
            }

            return text.Substring(lastIndex);
        }
    }
}