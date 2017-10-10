using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.FileSystem
{
    internal class FileProvider : IFileProvider
    {
        private readonly IFileProviderConfig _fileProviderConfig;

        public FileProvider(IFileProviderConfig fileProviderConfig)
        {
            _fileProviderConfig = fileProviderConfig;
        }

        public string[] GetFiles()
        {
            SearchOption searchOption = _fileProviderConfig.IncludeSubdirs
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            IEnumerable<string> fileNames = _fileProviderConfig.Directories
                .SelectMany(directory => _fileProviderConfig.FileNamePatterns
                    .SelectMany(fileNamePattern => Directory.GetFiles(directory, fileNamePattern, searchOption))
                );

            IEnumerable<string> excludeFileNames = _fileProviderConfig.Directories
                .SelectMany(directory => _fileProviderConfig.FileNamePatternsExclude
                    .SelectMany(excludePattern => Directory.GetFiles(directory, excludePattern, searchOption))
                );

            fileNames = fileNames
                .Except(excludeFileNames);

            return fileNames.ToArray();
        }
    }
}