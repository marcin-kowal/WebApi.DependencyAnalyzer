using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Integration
{
    public static class InstantCompiler
    {
        public static void Run(string resultAssemblyPath, params string[] sourceFilePaths)
        {
            string resultAssemblyName = Path.GetFileName(resultAssemblyPath);

            CSharpCompilation compilation = CreateCompilation(resultAssemblyName, sourceFilePaths);
            CompileAssembly(compilation, resultAssemblyPath);
        }

        private static CSharpCompilation CreateCompilation(string resultAssemblyName, string[] sourceFilePaths)
        {
            IReadOnlyCollection<SyntaxTree> syntaxTrees = CreateSyntaxTrees(sourceFilePaths);
            IReadOnlyCollection<MetadataReference> references = CreateReferences();

            CSharpCompilation compilation = CSharpCompilation.Create(
                resultAssemblyName,
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation;
        }

        private static IReadOnlyCollection<SyntaxTree> CreateSyntaxTrees(IReadOnlyCollection<string> sourceFilePaths)
        {
            IReadOnlyCollection<SyntaxTree> syntaxTrees = sourceFilePaths
                .Select(sourceFilePath =>
                {
                    string csCode = File.ReadAllText(sourceFilePath);
                    return CSharpSyntaxTree.ParseText(csCode);
                })
                .ToArray();

            return syntaxTrees;
        }

        private static IReadOnlyCollection<MetadataReference> CreateReferences()
        {
            return new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=0.0.0.0").Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DisplayAttribute).Assembly.Location)
            };
        }

        private static void CompileAssembly(CSharpCompilation compilation, string resultAssemblyPath)
        {
            EmitResult emitResult = compilation.Emit(resultAssemblyPath);

            if (!emitResult.Success)
            {
                Diagnostic diagnostic = emitResult.Diagnostics.First(diag => diag.IsWarningAsError
                    || diag.Severity == DiagnosticSeverity.Error);

                throw new InvalidOperationException(diagnostic.GetMessage());
            }
        }
    }
}