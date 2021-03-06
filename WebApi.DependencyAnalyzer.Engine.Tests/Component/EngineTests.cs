﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Tests.Helpers;
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
        private const string StringFormat = "StringFormat";
        private const string Attribute = "Attribute";

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
                        IReadOnlyCollection<string> result = analyzer.Analyze();

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
                    "'scanners': [ " + ScannerMarker + " ]",
                    "},",
                    "],",
                    "'process': {",
                    "'command': '" + CmdTypeCommand + "',",
                    "'arguments': '" + CmdTypeArguments + "'",
                    "}",
                    "}"
                };

                string[] configDataSingleLine = configData
                    .Select(line => line.Replace(ScannerMarker, ToJson(SingleLine), StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                string[] configDataStringFormat = configData
                    .Select(line => line.Replace(ScannerMarker, ToJson(StringFormat), StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                string[] configDataAttribute = configData
                    .Select(line => line.Replace(ScannerMarker, ToJson(Attribute), StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                string[] configDataComposite = configData
                    .Select(line => line.Replace(ScannerMarker, ToJson(StringFormat, Attribute, SingleLine), StringComparison.OrdinalIgnoreCase))
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
                    "IL_0015: ldstr      \"api/v1/module-management/bids/\"",
                    "+ \"{0}/{1}/versions/{2}/dashboard/settings/hierarchy/\"",
                    "+ \"items\""
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

                string[] fileData4 = new[]
                {
                    "DisplayAttribute::.ctor() = (01 00 01 00 54 0E 0B 44 65 73 63 72 69 70 74 69   // ....T..Descripti",
                    "6F 6E 30 61 70 69 2F 76 31 2F 6D 6F 64 75 6C 65   // on0api/v1/module",
                    "2F 62 69 64 2F 7B 30 7D 2F 75 73 65 72 2F 7B 31   // /bid/{0}/user/{1",
                    "7D 2F 70 65 72 73 6F 6E 61 6C 2F 73 65 74 74 69   // }/personal/setti",
                    "6E 67 73 )                                        // ngs"
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
                    "api/v1/module-management/bids/{0}/{1}/versions/{2}/dashboard/settings/hierarchy/items"
                };

                string[] expectedResult3 = new[]
                {
                    "api/v1/module-management/bids/{0}/versions/{1}/dashboard/settings/hierarchy/items"
                };

                string[] expectedResult4 = new[]
                {
                    "api/v1/module/bid/{0}/user/{1}/personal/settings"
                };

                yield return new object[] { configDataSingleLine, fileData1, expectedResult1 };
                yield return new object[] { configDataSingleLine, fileData2, expectedResult2 };

                yield return new object[] { configDataStringFormat, fileData3, expectedResult3 };

                yield return new object[] { configDataAttribute, fileData4, expectedResult4 };

                yield return new object[] { configDataComposite, fileData1, expectedResult1 };
                yield return new object[] { configDataComposite, fileData2, expectedResult2 };
                yield return new object[] { configDataComposite, fileData3, expectedResult3 };
                yield return new object[] { configDataComposite, fileData4, expectedResult4 };
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

        private static string ToJson(params string[] values)
        {
            return string.Join(',', values.Select(v => "\"" + v + "\""));
        }
    }
}