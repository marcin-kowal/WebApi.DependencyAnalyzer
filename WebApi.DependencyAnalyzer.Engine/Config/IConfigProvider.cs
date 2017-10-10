
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IConfigProvider : IDecompilerConfig
    {
        IConfigItem[] ConfigItems { get; }
    }
}