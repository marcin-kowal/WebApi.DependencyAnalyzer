using System.Collections.Generic;

namespace WebApi.DependencyAnalyzer.Engine
{
    public interface IAnalyzer
    {
        IReadOnlyCollection<string> Analyze();
    }
}