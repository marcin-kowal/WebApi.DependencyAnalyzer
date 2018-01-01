using System;
using System.IO;
using System.Text;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Helpers
{
    internal sealed class FileData : IDisposable
    {
        private FileData(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            FilePath = filePath;
            DirectoryPath = Path.GetDirectoryName(filePath);
        }

        public static FileData Create(string[] data)
        {
            string filePath = Path.GetTempFileName();

            File.AppendAllLines(filePath, data, Encoding.Unicode);

            return new FileData(filePath);
        }

        public string FileName { get; }
        public string FilePath { get; }
        public string DirectoryPath { get; }

        public void Dispose()
        {
            File.Delete(FilePath);
        }
    }
}