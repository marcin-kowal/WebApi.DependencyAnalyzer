using Newtonsoft.Json;

namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public class ProcessConfig
    {
        public ProcessConfig(string command, string arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        [JsonProperty("command")]
        public string Command { get; }

        [JsonProperty("arguments")]
        public string Arguments { get; }
    }
}