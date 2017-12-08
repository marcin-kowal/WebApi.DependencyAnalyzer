
namespace WebApi.DependencyAnalyzer.Engine.Config
{
    public interface IScannerConfig
    {
        char OperandSeparator { get; }
        string[] InstructionTokens { get; }
        string[] OperandOperationTokens { get; }
        string[] SimpleOperationTokens { get; }
        string[] MultilineOperationBeginTokens { get; }
        string[] MultilineOperationEndTokens { get; }

        string[] AppendTokens { get; }

        string[] TextSearchPatterns { get; }
        string[] TextSearchPatternsExclude { get; }
    }
}