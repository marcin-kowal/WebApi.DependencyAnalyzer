using System;
using System.Diagnostics;
using System.IO;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Decompilation
{
    internal class CommandLineDecompiler : IDecompiler
    {
        private readonly IDecompilerConfig _config;
        private bool _isDisposed;
        private Process _process;
        private StreamReader _reader;

        public CommandLineDecompiler(
            IDecompilerConfig config)
        {
            _config = config;
            _isDisposed = true;
        }

        public bool EndOfStream => _reader.EndOfStream;

        public string ReadLine() => _reader.ReadLine();

        public void Open(string libraryPath)
        {
            ProcessStartInfo processInfo = CreateProcessInfo(libraryPath);

            _process = Process.Start(processInfo);
            _reader = _process.StandardOutput;

            _isDisposed = false;
        }

        public void Close()
        {
            _reader.Close();
            _process.Close();
        }

        private ProcessStartInfo CreateProcessInfo(string filePath)
        {
            string arguments = string.Format(_config.Arguments, filePath);

            ProcessStartInfo processInfo = new ProcessStartInfo(_config.Command, arguments)
            {
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            return processInfo;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                _reader.Dispose();
                _reader = null;

                _process.Dispose();
                _process = null;
            }

            _isDisposed = true;
        }

        #endregion
    }
}