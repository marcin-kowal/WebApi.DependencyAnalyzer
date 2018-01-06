using System.Collections.Generic;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public interface IScanPreprocessor
    {
        string Preprocess(string text, IReadOnlyCollection<string> tokensToRemove);

        string TrimStart(string text);
        string TrimStart(string text, char[] trimChars);
        string TrimEnd(string text);
        string TrimEnd(string text, char[] trimChars);
        string Trim(string text);
        string Trim(string text, char[] trimChars);

        string TrimStartTo(string text, IReadOnlyCollection<string> tokens);
    }
}