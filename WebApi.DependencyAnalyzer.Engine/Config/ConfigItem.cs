using Newtonsoft.Json;

namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public class ConfigItem : IConfigItem
    {
        private const char _operandSeparator = ',';
        private static readonly string[] _instructionTokens = new[] { "IL_[0-9a-fA-F]+:" };
        private static readonly string[] _simpleOperationTokens = new[] { "ldstr", "ldfld" };
        private static readonly string[] _multilineOperationBeginTokens = new[] { @"System.String::Format\(" };
        private static readonly string[] _multilineOperationEndTokens = new[] { @"\)" };
        private static readonly char[] _trimTokens = new[] { ' ', '\'', '"' };
        private static readonly string[] _appendTokens = new[] { "+" };

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

        public char OperandSeparator { get; } = _operandSeparator;
        public string[] InstructionTokens { get; } = _instructionTokens;
        public string[] SimpleOperationTokens { get; } = _simpleOperationTokens;
        public string[] MultilineOperationBeginTokens { get; } = _multilineOperationBeginTokens;
        public string[] MultilineOperationEndTokens { get; } = _multilineOperationEndTokens;
        public char[] TrimTokens { get; } = _trimTokens;
        public string[] AppendTokens { get; } = _appendTokens;

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