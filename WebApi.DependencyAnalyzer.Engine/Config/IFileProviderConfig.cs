
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IFileProviderConfig
    {
        string[] Directories { get; }
        string[] FileNamePatterns { get; }
        string[] FileNamePatternsExclude { get; }
        bool IncludeSubdirs { get; }
    }
}