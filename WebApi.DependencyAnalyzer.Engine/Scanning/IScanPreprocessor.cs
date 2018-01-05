using System.Collections.Generic;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public interface IScanPreprocessor
    {
        string Preprocess(string text, IReadOnlyCollection<string> tokensToRemove);

        string TrimStart(string text, IReadOnlyCollection<char> charsToRemove = null);
        string TrimEnd(string text, IReadOnlyCollection<char> charsToRemove = null);
        string Trim(string text, IReadOnlyCollection<char> charsToRemove = null);

        string TrimStartTo(string text, IReadOnlyCollection<string> tokens);
    }
}