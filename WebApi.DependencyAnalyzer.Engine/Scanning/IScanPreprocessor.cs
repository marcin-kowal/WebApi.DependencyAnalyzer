
namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public interface IScanPreprocessor
    {
        string Preprocess(string text);
    }
}