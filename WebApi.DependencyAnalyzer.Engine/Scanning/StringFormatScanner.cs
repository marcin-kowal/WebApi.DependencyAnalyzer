using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebApi.DependencyAnalyzer.Engine.Common;
using WebApi.DependencyAnalyzer.Engine.Config;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class StringFormatScanner : MultiLineScanner
    {
        private static class Parameters
        {
            public const int BufferSize = 12;
            public const char OperandSeparator = ',';
            public static readonly string[] AppendTokens = new[] { "+" };
            public static readonly string[] InstructionTokens = new[] { "IL_[0-9a-fA-F]+:" };
            public static readonly string[] MultilineOperationBeginTokens = new[] { @"System.String::Format\(" };
            public static readonly string[] MultilineOperationEndTokens = new[] { @"\)" };
            public static readonly string[] OperandOperationTokens = new[] { "ldstr" };
            public static readonly string[] SimpleOperationTokens = new[] { "ldstr", "ldfld", "ldc" };
        }

        public StringFormatScanner(
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
            int numberOfOperands = 1
                + multilineOperationLine.Text.Count(chr => chr == OperandSeparator);

            if (multilineOperationLineIndex < numberOfOperands)
            {
                throw new InvalidOperationException("Not enough operands for String.Format operation. Buffer too small. " +
                    $"Number of available operands: {multilineOperationLineIndex}. Number of operation arguments: {numberOfOperands}.");
            }

            Line[] operandLines = lines
                .Take(multilineOperationLineIndex)
                .Reverse()
                .Where(line => line.Text.StartsWithAny(SimpleOperationTokens))
                .Take(numberOfOperands)
                .Reverse()
                .ToArray();

            return operandLines;
        }

        protected override string PrepareToPush(string text)
        {
            text = _preprocessor.Preprocess(text, InstructionTokens);

            return text;
        }

        protected override string PrepareToAppend(string text, string appendToken = null)
        {
            if (appendToken != null)
            {
                text = text.Remove(0, appendToken.Length);
            }

            text = _preprocessor.TrimStart(text);

            return text;
        }

        protected override string BuildCompleteLine(IReadOnlyCollection<Line> operandLines)
        {
            int index = 0;

            IEnumerable<string> operands = operandLines
                .Select((line) => line.Text.StartsWithAny(OperandOperationTokens)
                    ? _preprocessor.Preprocess(line.Text, SimpleOperationTokens)
                    : $"{{{index++}}}");

            string completeLine = string.Format(CultureInfo.InvariantCulture, operands.First(), operands.Skip(1).ToArray());

            return completeLine;
        }
    }
}