using FluentAssertions;
using System;
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
        private const string ScannerMarker = Marker + "SCANNER" + Marker;
        private const string SingleLine = "SingleLine";
        private const string MultiLine = "MultiLine";

        //private const string IldasmPath = "C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v10.0A\\bin\\NETFX 4.6.1 Tools\\ildasm.exe";
        //private string IldasmCommand => "\\\"" + DuplicateBackslash(IldasmPath) + "\\\"\",";
        //private const string IldasmArguments = "/text \\\"{0}\\\" /unicode /metadata";

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
                string[] configData = new[]
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
                    "'scanners': [ '" + ScannerMarker + "' ]",
                    "},",
                    "],",
                    "'process': {",
                    "'command': '" + CmdTypeCommand + "',",
                    "'arguments': '" + CmdTypeArguments + "'",
                    "}",
                    "}"
                };

                string[] configDataSingleLine = configData
                    .Select(line => line.Replace(ScannerMarker, SingleLine, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                string[] configDataMultiLine = configData
                    .Select(line => line.Replace(ScannerMarker, MultiLine, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

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
                    "IL_0010:  ldstr      \"{0}/{1}/versions/{2}/dashboard/settings/hierarchy/\"",
                    "+ \"items\"",
                    "IL_0015: ldstr      \"api/v1/module-management/bids/\"",
                };

                //string.Format("{0}/{1}/versions/{2}/dashboard/settings/hierarchy/items", "api/v1/module-management/bids", cVer, pVer);
                string[] fileData3 = new[]
                {
                    "IL_000e: nop",
                    "IL_000f:  ldarg.0",
                    "        IL_0010: ldstr      \"{0}/{1}/versions/{2}/dashboard/settings/hierarchy/\"",
                    "       + \"items\"",
                    "        IL_0015: ldstr      \"api/v1/module-management/bids\"",
                    "        IL_001a: ldarg.0",
                    "        IL_001b: ldfld int32 RestfulService / '<Get>d__3'::cVer",
                    "        IL_0020: box[mscorlib]System.Int32",
                    "IL_0025:  ldarg.0",
                    "        IL_0026: ldfld valuetype[mscorlib]System.Guid RestfulService / '<Get>d__3'::pVer",
                    "        IL_002b: box[mscorlib]System.Guid",
                    "IL_0030:  call       string[mscorlib] System.String::Format(string,",
                    "                                                           object,",
                    "                                                           object,",
                    "                                                           object)"
                };

                string[] expectedResult1 = new[]
                {
                    "api/v1/module/bid/{0}/user/{1}/dashboard/settings",
                    "api/v1/module/bids/{0}/assigned_tag_collections",
                    "api/v1/module/currencies",
                    "api/v1/module/currencies/default",
                };

                string[] expectedResult2_3 = new[]
                {
                    "api/v1/module-management/bids/{0}/{1}/versions/{2}/dashboard/settings/hierarchy/items"
                };

                yield return new object[] { configDataSingleLine, fileData1, expectedResult1 };
                yield return new object[] { configDataMultiLine, fileData1, expectedResult1 };
                yield return new object[] { configDataMultiLine, fileData2, expectedResult2_3 };
                //yield return new object[] { configDataMultiLine, fileData3, expectedResult2_3 };
            }
        }

        private string[] PrepareConfigData(string[] configData, FileData dataFile)
        {
            return configData
                .Select(configLine => configLine.Contains(Marker)
                    ? configLine
                        .Replace(DirectoryMarker, DuplicateBackslash(dataFile.DirectoryPath), StringComparison.OrdinalIgnoreCase)
                        .Replace(FileMarker, dataFile.FileName, StringComparison.OrdinalIgnoreCase)
                    : configLine)
                .ToArray();
        }

        private static string DuplicateBackslash(string source)
        {
            return source.Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase);
        }
    }
}