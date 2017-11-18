
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IScannerConfig
    {
        string[] AppendTokens { get; }
        string[] PrependTokens { get; }

        string[] TextSearchPatterns { get; }
        string[] TextSearchPatternsExclude { get; }
    }
}