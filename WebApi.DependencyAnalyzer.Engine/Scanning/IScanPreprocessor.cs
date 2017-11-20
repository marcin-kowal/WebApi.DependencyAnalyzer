
namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public interface IScanPreprocessor
    {
        string Preprocess(string text);

        string TrimStart(string text);
        string TrimEnd(string text);
        string Trim(string text);
    }
}