using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Config;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Unit
{
    public class ConfigProviderTests
    {
        [Theory]
        [MemberData(nameof(ReadFromJsonData))]
        public void ReadFromJsonTest(string json, Configuration expectedResult)
        {
            ConfigProvider config = new ConfigProvider();
            config.ReadFromJson(json);

            config.Command.Should().Be(expectedResult.Process.Command);
            config.Arguments.Should().Be(expectedResult.Process.Arguments);

            config.ConfigItems.ShouldAllBeEquivalentTo(expectedResult.Items);
        }

        public static IEnumerable<object[]> ReadFromJsonData
        {
            get
            {
                string[] directories = new[] { @"D:\\dir1", @"C:\\dir2" };
                string[] fileNamePatterns = new[] { "*.dat", "*.bat" };
                string[] fileNamePatternsExclude = new[] { "*.xlsx" };
                bool includeSubdirs = true;
                string[] textSearchPatterns = new[] { "/api/va", "/api/v2" };
                string[] textSearchPatternsExclude = new[] { "swagger" };
                string[] scanners = new[] { "SingleLine", "MultiLine" };
                string command = "command";
                string arguments = "arguments";

                string json =
                    "{" +
                    "'items': [" +
                    "{" +
                    $"'directories': [ {ToJson(directories)} ]," +
                    $"'fileNamePatterns': [ {ToJson(fileNamePatterns)} ]," +
                    $"'fileNamePatternsExclude': [ {ToJson(fileNamePatternsExclude)} ]," +
                    $"'includeSubdirs': '{includeSubdirs}'," +
                    $"'textSearchPatterns': [ {ToJson(textSearchPatterns)} ]," +
                    $"'textSearchPatternsExclude': [{ToJson(textSearchPatternsExclude)} ]," +
                    $"'scanners': [{ToJson(scanners)} ]" +
                    "}" +
                    "]," +
                    "\"process\": {" +
                    $"\"command\": \"{command}\"," +
                    $"\"arguments\": \"{arguments}\"" +
                    "}" +
                    "}";

                ConfigItem[] items = new ConfigItem[]
                {
                    new ConfigItem(
                        directories.Select(d => d.Replace(@"\\", @"\", StringComparison.OrdinalIgnoreCase)).ToArray(),
                        fileNamePatterns,
                        fileNamePatternsExclude,
                        includeSubdirs,
                        textSearchPatterns,
                        textSearchPatternsExclude,
                        scanners)
                };
                ProcessConfig process = new ProcessConfig(command, arguments);
                Configuration expectedResult = new Configuration(items, process);

                yield return new object[] { json, expectedResult };
            }
        }

        private static string ToJson(string[] values)
        {
            return string.Join(',', values.Select(v => "\"" + v + "\""));
        }
    }
}