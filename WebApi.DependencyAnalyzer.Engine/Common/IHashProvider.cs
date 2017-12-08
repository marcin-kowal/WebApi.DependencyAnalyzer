
namespace WebApi.DependencyAnalyzer.Engine.Common
{
    public interface IHashProvider<TInput>
    {
        long GetHash(TInput input);
    }
}