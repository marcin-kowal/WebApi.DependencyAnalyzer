using System;
using System.Collections.Generic;
using System.Linq;
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

        public string TrimStart(string text, IReadOnlyCollection<char> charsToRemove = null)
        {
            char[] trimChars = GetTrimChars(charsToRemove);

            return text.TrimStart(trimChars);
        }

        public string TrimEnd(string text, IReadOnlyCollection<char> charsToRemove = null)
        {
            char[] trimChars = GetTrimChars(charsToRemove);

            return text.TrimEnd(trimChars);
        }

        public string Trim(string text, IReadOnlyCollection<char> charsToRemove = null)
        {
            char[] trimChars = GetTrimChars(charsToRemove);

            return text.Trim(trimChars);
        }

        private char[] GetTrimChars(IReadOnlyCollection<char> charsToRemove)
        {
            char[] trimChars = charsToRemove?.ToArray() 
                ?? Parameters.TrimChars;

            return trimChars;
        }

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