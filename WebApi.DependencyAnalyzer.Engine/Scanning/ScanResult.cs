
namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public class ScanResult
    {
        public static ScanResult Success(params string[] values)
        {
            return new ScanResult(true, values);
        }

        public static ScanResult Failure()
        {
            return new ScanResult(false, null);
        }

        private ScanResult(bool success, string[] values)
        {
            IsSuccess = success;
            Values = values;
        }

        public bool IsSuccess { get; }
        public string[] Values { get; }
    }
}