
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IScannerConfig
    {
        char[] TrimPatterns { get; }
        string[] ConcatOperators { get; }
        string[] TextSearchPatterns { get; }
        string[] TextSearchPatternsExclude { get; }
    }
}