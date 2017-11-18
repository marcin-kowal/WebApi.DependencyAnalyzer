
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IConfigItem : 
        IFileProviderConfig, 
        IScannerConfig,
        IPreprocessorConfig
    {
        string[] Scanners { get; }
    }
}