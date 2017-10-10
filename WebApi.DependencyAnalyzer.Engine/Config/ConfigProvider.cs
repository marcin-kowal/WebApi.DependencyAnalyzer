using Newtonsoft.Json;

namespace WebApi.DependencyAnalyzer.Engine.Config
{
    internal class ConfigProvider : IConfigProvider
    {
        private Configuration _configuration;

        public string Command => _configuration.Process.Command;
        public string Arguments => _configuration.Process.Arguments;

        public IConfigItem[] ConfigItems => _configuration.Items;

        public void ReadFromJson(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };

            _configuration = JsonConvert.DeserializeObject<Configuration>(json, settings);
        }
    }
}