using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebApi.DependencyAnalyzer.Engine.Tests.Helpers;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Integration
{
    public class DependencyAnalyzerTests
    {
        private const string Marker = "##";
        private const string DirectoryMarker = Marker + "DIR" + Marker;
        private const string FileMarker = Marker + "FILE" + Marker;
        private const string TestClassPath = @"Integration\TestClass.cs";
        private const string ConfigJsonPath = @"Integration\config.json";

        [Fact]
        public void DependencyAnalyzerTest()
        {
            string[] expectedResult = new[]
            {
                "api/v1/module/bid/{0}/user/{1}/dashboard/settings",
                "api/v1/module-management/bids/{0}/versions/{1}/dashboard/settings/hierarchy/",
                "api/v1/module-management/bids/{0}/{1}/versions/{2}/dashboard/settings/hierarchy/items",
                "api/v1/module/bid/{0}/user/{1}/personal/settings"
            };

            List<string> results = new List<string>();

            using (FileData assemblyFile = FileData.Create(Array.Empty<string>()))
            {
                Compiler.Run(assemblyFile.FilePath, TestClassPath);
                UpdateConfig(ConfigJsonPath, assemblyFile);

                IAnalyzer[] analyzers = new EngineFactory().CreateAnalyzers(ConfigJsonPath);

                foreach (IAnalyzer analyzer in analyzers)
                {
                    IReadOnlyCollection<string> result = analyzer.Analyze();

                    results.AddRange(result);
                }
            }

            results.ShouldAllBeEquivalentTo(expectedResult);
        }

        private void UpdateConfig(string configJsonPath, FileData assemblyFile)
        {
            string config = File.ReadAllText(configJsonPath, Encoding.Unicode);

            config = config.Replace(DirectoryMarker, DuplicateBackslash(assemblyFile.DirectoryPath), StringComparison.Ordinal);
            config = config.Replace(FileMarker, assemblyFile.FileName, StringComparison.Ordinal);

            File.WriteAllText(configJsonPath, config, Encoding.Unicode);
        }

        private static string DuplicateBackslash(string source)
        {
            return source.Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase);
        }
    }
}