
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IConfigItem : IFileProviderConfig, IScannerConfig
    {
        string[] Scanners { get; }
    }
}