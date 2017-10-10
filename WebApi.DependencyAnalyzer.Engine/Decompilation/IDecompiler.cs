using System;

namespace WebApi.DependencyAnalyzer.Engine.Decompilation
{
    public interface IDecompiler : IDisposable
    {
        bool EndOfStream { get; }

        string ReadLine();

        void Open(string fileName);
        void Close();
    }
}