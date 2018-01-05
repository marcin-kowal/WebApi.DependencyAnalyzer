using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class AttributeScanner : MultiLineScanner
    {
        private static class Parameters
        {
            public const int BufferSize = 12;
            public const char OperandSeparator = new char();
            public static readonly char[] TrimChars = new[] { '/', ')' };
            public static readonly string[] AppendTokens = Array.Empty<string>();
            public static readonly string[] InstructionTokens = new[] { @".+Attribute.*\(" };
            public static readonly string[] MultilineOperationBeginTokens = new[] { "//" };
            public static readonly string[] MultilineOperationEndTokens = new[] { @"[^\(]\)" };
            public static readonly string[] OperandOperationTokens = Array.Empty<string>();
            public static readonly string[] SimpleOperationTokens = new[] { ".*//" };
        }

        public AttributeScanner(
            IScannerConfig config,
            IScanPreprocessor preprocessor,
            IHashProvider<string> hashProvider)
            : base(config, preprocessor, hashProvider)
        { }

        protected override int BufferSize => Parameters.BufferSize;
        protected override char OperandSeparator => Parameters.OperandSeparator;
        protected override string[] AppendTokens => Parameters.AppendTokens;
        protected override string[] InstructionTokens => Parameters.InstructionTokens;
        protected override string[] MultilineOperationBeginTokens => Parameters.MultilineOperationBeginTokens;
        protected override string[] MultilineOperationEndTokens => Parameters.MultilineOperationEndTokens;
        protected override string[] OperandOperationTokens => Parameters.OperandOperationTokens;
        protected override string[] SimpleOperationTokens => Parameters.SimpleOperationTokens;

        protected override IReadOnlyCollection<Line> GetMultilineOperands(
            IReadOnlyCollection<Line> lines, 
            int multilineOperationLineIndex, 
            Line multilineOperationLine)
        {
            return lines;
        }

        protected override string PrepareToPush(string text)
        {
            text = _preprocessor.TrimStartTo(text, InstructionTokens);
            text = _preprocessor.TrimStart(text, Parameters.TrimChars);
            text = _preprocessor.Trim(text);

            return text;
        }

        protected override string PrepareToAppend(string text, string appendToken = null)
        {
            if (appendToken != null)
            {
                text = text.Remove(0, appendToken.Length);
            }

            text = _preprocessor.TrimStartTo(text, MultilineOperationBeginTokens);
            text = _preprocessor.Trim(text, Parameters.TrimChars);
            text = _preprocessor.Trim(text);

            return text;
        }

        protected override string BuildCompleteLine(IReadOnlyCollection<Line> operandLines)
        {
            string text = operandLines.Last().Text;

            text = _preprocessor.Preprocess(text, InstructionTokens);
            text = _preprocessor.TrimStart(text, Parameters.TrimChars);

            return text;
        }
    }
}