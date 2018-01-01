using System;
using System.IO;
using System.Linq;
using System.Text;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;
using WebApi.DependencyAnalyzer.Engine.Decompilation;
using WebApi.DependencyAnalyzer.Engine.FileSystem;
using WebApi.DependencyAnalyzer.Engine.Scanning;

namespace WebApi.DependencyAnalyzer.Engine
{
    public class EngineFactory
    {
        public IAnalyzer[] CreateAnalyzers(string configPath)
        {
            string json = File.ReadAllText(configPath, Encoding.Unicode);
            IConfigProvider configProvider = CreateConfigProvider(json);

            return CreateAnalyzers(configProvider);
        }

        public IAnalyzer[] CreateAnalyzers(IConfigProvider configProvider)
        {
            IDecompiler decompiler = CreateDecompiler(configProvider);

            IAnalyzer[] analyzers = configProvider.ConfigItems
                .Select(configItem => CreateAnalyzer(decompiler, configItem))
                .ToArray();

            return analyzers;
        }

        private IAnalyzer CreateAnalyzer(IDecompiler decompiler, IConfigItem configItem)
        {
            IFileProvider fileProvider = CreateFileProvider(configItem);
            IScanner scanner = CreateScanner(configItem);

            return new Analyzer(fileProvider, decompiler, scanner);
        }

        private IConfigProvider CreateConfigProvider(string json)
        {
            ConfigProvider configProvider = new ConfigProvider();
            configProvider.ReadFromJson(json);

            return configProvider;
        }

        private IDecompiler CreateDecompiler(IDecompilerConfig config)
        {
            return new CommandLineDecompiler(config);
        }

        private IFileProvider CreateFileProvider(IFileProviderConfig config)
        {
            return new FileProvider(config);
        }

        private IScanner CreateScanner(IConfigItem config)
        {
            IScanPreprocessor preprocessor = CreateScanPreprocessor();
            IHashProvider<string> hashProvider = CreateHashProvider();

            string scannerNamespace = typeof(IScanner).Namespace;

            Type[] scannerTypes = config.Scanners
                .Select(scannerName => $"{scannerNamespace}.{scannerName}Scanner")
                .Select(scannerType => Type.GetType(scannerType))
                .ToArray();

            IScanner[] scanners = scannerTypes
                .Select(scannerType => (IScanner)Activator.CreateInstance(scannerType, config, preprocessor, hashProvider))
                .ToArray();

            if (scanners.Length > 1)
            {
                CompositeScanner compositeScanner = new CompositeScanner();

                foreach (IScanner scanner in scanners)
                {
                    compositeScanner.AddScanner(scanner);
                }

                return compositeScanner;
            }

            if (scanners.Length == 1)
            {
                return scanners.First();
            }

            throw new ArgumentOutOfRangeException(nameof(config.Scanners));
        }

        private IScanPreprocessor CreateScanPreprocessor()
        {
            return new ScanPreprocessor();
        }

        private IHashProvider<string> CreateHashProvider()
        {
            return new HashProvider();
        }
    }
}