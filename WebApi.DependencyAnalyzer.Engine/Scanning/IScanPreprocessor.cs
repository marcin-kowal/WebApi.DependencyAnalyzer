using System.Collections.Generic;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public interface IScanPreprocessor
    {
        string Preprocess(string text, IReadOnlyCollection<string> tokensToRemove);

        string TrimStart(string text);
        string TrimEnd(string text);
        string Trim(string text);
    }
}