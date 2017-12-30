using System.Collections.Generic;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public interface IScanner
    {
        void AppendLine(string line);
        void Scan();
        IReadOnlyCollection<ScanResult> GetResult();
    }
}