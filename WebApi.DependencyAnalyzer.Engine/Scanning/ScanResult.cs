using System.Collections.Generic;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    public class ScanResult
    {
        public static ScanResult Success(string value, long lineHash) =>
            Success(value, new[] { lineHash });

        public static ScanResult Success(string value, IReadOnlyCollection<long> lineHashes)
        {
            return new ScanResult(true, value, lineHashes);
        }

        public static ScanResult Failure()
        {
            return new ScanResult(false, null, null);
        }

        private ScanResult(bool success, string value, IReadOnlyCollection<long> lineHashes)
        {
            IsSuccess = success;
            Value = value;

            LineHashes = lineHashes;
        }

        public bool IsSuccess { get; }
        public string Value { get; }

        internal IReadOnlyCollection<long> LineHashes { get; }
    }
}