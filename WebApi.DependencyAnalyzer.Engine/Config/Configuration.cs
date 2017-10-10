using Newtonsoft.Json;

namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public class Configuration
    {
        public Configuration(ConfigItem[] items, ProcessConfig process)
        {
            Items = items;
            Process = process;
        }

        [JsonProperty("items")]
        public ConfigItem[] Items { get; }

        [JsonProperty("process")]
        public ProcessConfig Process { get; }
    }
}