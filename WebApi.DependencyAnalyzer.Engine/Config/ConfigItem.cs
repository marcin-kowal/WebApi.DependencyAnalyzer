using Newtonsoft.Json;

namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public class ConfigItem : IConfigItem
    {
        private static readonly char[] _trimPatterns = new[] { ' ', '\'', '"' };
        private static readonly string[] _appendOperators = new[] { "+" };
        private static readonly string[] _prependOperators = new[] { "ldstr" };

        public ConfigItem(
            string[] directories,
            string[] fileNamePatterns,
            string[] fileNamePatternsExclude,
            bool includeSubdirs,
            string[] textSearchPatterns,
            string[] textSearchPatternsExclude,
            string[] scanners)
        {
            Directories = directories;
            FileNamePatterns = fileNamePatterns;
            FileNamePatternsExclude = fileNamePatternsExclude;
            IncludeSubdirs = includeSubdirs;
            TextSearchPatterns = textSearchPatterns;
            TextSearchPatternsExclude = textSearchPatternsExclude;
            Scanners = scanners;
        }

        public char[] TrimPatterns { get; } = _trimPatterns;
        public string[] AppendOperators { get; } = _appendOperators;
        public string[] PrependOperators { get; } = _prependOperators;

        [JsonProperty("directories")]
        public string[] Directories { get; }

        [JsonProperty("fileNamePatterns")]
        public string[] FileNamePatterns { get; }

        [JsonProperty("fileNamePatternsExclude")]
        public string[] FileNamePatternsExclude { get; }

        [JsonProperty("includeSubdirs")]
        public bool IncludeSubdirs { get; }

        [JsonProperty("textSearchPatterns")]
        public string[] TextSearchPatterns { get; }

        [JsonProperty("textSearchPatternsExclude")]
        public string[] TextSearchPatternsExclude { get; }

        [JsonProperty("scanners")]
        public string[] Scanners { get; }
    }
}