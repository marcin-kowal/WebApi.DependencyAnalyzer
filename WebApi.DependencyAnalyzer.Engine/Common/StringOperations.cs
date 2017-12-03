using System.Linq;
using System.Text.RegularExpressions;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    public static class StringOperations
    {
        private readonly static Operation<string> _append = new Operation<string>((first, second) => first + second);
        private readonly static Operation<string> _prepend = new Operation<string>((first, second) => second + first);

        public static string Append(this string first, string second) => _append.Execute(first, second);
        public static string Prepend(this string first, string second) => _prepend.Execute(first, second);

        public static bool StartsWith(this string text, params string[] tokens) => tokens
            .Any(token => new Regex($"^{token}").IsMatch(text));

        public static bool Contains(this string text, params string[] tokens) => tokens
            .Any(token => new Regex(token).IsMatch(text));
    }
}