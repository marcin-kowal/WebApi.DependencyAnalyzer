
namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public class ScanResult
    {
        public static ScanResult Success(string value)
        {
            return new ScanResult(true, value);
        }

        public static ScanResult Failure()
        {
            return new ScanResult(false, null);
        }

        private ScanResult(bool success, string value)
        {
            IsSuccess = success;
            Value = value;
        }

        public bool IsSuccess { get; }
        public string Value { get; }
    }
}