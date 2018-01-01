
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IScannerConfig
    {
        string[] TextSearchPatterns { get; }
        string[] TextSearchPatternsExclude { get; }
    }
}