
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IPreprocessorConfig
    {
        char[] TrimTokens { get; }
        string[] RemoveTokens { get; }
    }
}