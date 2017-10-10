
namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public interface IScanner
    {
        void AppendLine(string line);

        ScanResult Scan();
    }
}