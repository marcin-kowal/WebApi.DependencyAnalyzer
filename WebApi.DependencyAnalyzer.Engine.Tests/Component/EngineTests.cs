using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Tests.Common;
using Xunit;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Component
{
    public class EngineTests
    {
        private const string Marker = "##";
        private const string DirectoryMarker = Marker + "DIR" + Marker;
        private const string FileMarker = Marker + "FILE" + Marker;
        private const string SingleLine = "SingleLine";

        private const string IldasmPath = "C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v10.0A\\bin\\NETFX 4.6.1 Tools\\ildasm.exe";
        private string IldasmCommand => "\\\"" + DuplicateBackslash(IldasmPath) + "\\\"\",";
        private const string IldasmArguments = "/text \\\"{0}\\\" /unicode /metadata";

        private const string CmdTypeCommand = "cmd";
        private const string CmdTypeArguments = "/k type {0} & exit";

        [Theory]
        [MemberData(nameof(EngineData))]
        public void EngineTest(string[] configData, string[] fileData, string[] expectedResult)
        {
            List<string> results = new List<string>();

            using (FileData dataFile = FileData.Create(fileData))
            {
                configData = PrepareConfigData(configData, dataFile);

                using (FileData configFile = FileData.Create(configData))
                {
                    IAnalyzer[] analyzers = new EngineFactory().CreateAnalyzers(configFile.FilePath);

                    foreach (IAnalyzer analyzer in analyzers)
                    {
                        string[] result = analyzer.Analyze();

                        results.AddRange(result);
                    }
                }
            }

            results.ShouldAllBeEquivalentTo(expectedResult);
        }

        public static IEnumerable<object[]> EngineData
        {
            get
            {
                string[] configDataSingleLine = new[]
                {
                    "{",
                    "'items': [",
                    "{",
                    "'directories': [ '" + DirectoryMarker + "' ],",
                    "'fileNamePatterns': [ '" + FileMarker + "' ],",
                    "'fileNamePatternsExclude': [],",
                    "'includeSubdirs': 'false',",
                    "'textSearchPatterns': [ 'api/v1.*' ],",
                    "'textSearchPatternsExclude': [ '/swagger' ],",
                    "'scanners': [ '" + SingleLine + "' ]",
                    "},",
                    "],",
                    "'process': {",
                    "'command': '" + CmdTypeCommand + "',",
                    "'arguments': '" + CmdTypeArguments + "'",
                    "}",
                    "}"
                };

                string[] configDataMultiLine = new[]
                {
                    ""
                };

                string[] fileData1 = new[]
                {
                    "// 70000001 : (25) L\"api/v1/module/currencies\"",
                    "// 70000035 : (33) L\"api/v1/module/currencies/default\"",
                    "// 70000079 : (48) L\"api/v1/module/bids/{0}/assigned_tag_collections\"",
                    "// 70000119 : (50) L\"api/v1/module/bid/{0}/user/{1}/dashboard/settings\"",
                    "IL_0015:  ldstr      \"api/v1/module/bid/{0}/user/{1}/dashboard/settings\"",
                    "IL_0002:  ldstr      \"api/v1/module/currencies\"",
                    "IL_0002:  ldstr      \"api/v1/module/currencies/default\"",
                    "IL_0002:  ldstr      \"api/v1/module/bids/{0}/assigned_tag_collections\"",
                };

                string[] fileData2 = new[]
                {
                    "IL_0015:  ldstr      \"{0}/users\"",
                    "IL_001a: ldstr      \"api/v1/module-management\"",

                    "IL_0002:  ldstr      \"{0}/{1}/tags/commodities/all\"",
                    "IL_0007: ldstr      \"api/v1/module/bids\"",
                };

                string[] fileData3 = new[]
                {
                    "IL_0010:  ldstr      \"{0}/{1}/versions/{2}/dashboard/settings/hierarchy/\"",
                    "+ \"items\"",
                    "IL_0015: ldstr      \"api/v1/module-management/bids\"",
                };

                string[] expectedResult1 = new[]
                {
                    "api/v1/module/bid/{0}/user/{1}/dashboard/settings",
                    "api/v1/module/bids/{0}/assigned_tag_collections",
                    "api/v1/module/currencies",
                    "api/v1/module/currencies/default",
                };

                string[] expectedResult2 = new[]
                {
                    "api/v1/module-management/{0}/users",

                    "api/v1/module/bids/{0}/{1}/tags/commodities/all",
                };

                string[] expectedResult3 = new[]
                {
                    "api/v1/module-management/bids/{0}/{1}/versions/{2}/dashboard/settings/hierarchy/items"
                };

                yield return new object[] { configDataSingleLine, fileData1, expectedResult1 };
            }
        }

        private string[] PrepareConfigData(string[] configData, FileData dataFile)
        {
            return configData
                .Select(configLine => configLine.Contains(Marker)
                    ? configLine
                        .Replace(DirectoryMarker, DuplicateBackslash(dataFile.DirectoryPath))
                        .Replace(FileMarker, dataFile.FileName)
                    : configLine)
                .ToArray();
        }

        private static string DuplicateBackslash(string source)
        {
            return source.Replace(@"\", @"\\");
        }
    }
}