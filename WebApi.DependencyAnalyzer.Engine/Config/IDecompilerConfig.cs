
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IDecompilerConfig
    {
        string Command { get; }
        string Arguments { get; }
    }
}