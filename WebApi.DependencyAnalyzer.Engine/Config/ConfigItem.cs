using Newtonsoft.Json;

namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public class ConfigItem : IConfigItem
    {
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