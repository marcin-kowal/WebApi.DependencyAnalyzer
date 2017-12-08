using System.Linq;
using System.Text.RegularExpressions;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    public static class StringOperations
    {
        public static bool StartsWithAny(this string text, params string[] tokens) => tokens
            .Any(token => new Regex($"^{token}").IsMatch(text));

        public static bool ContainsAny(this string text, params string[] tokens) => tokens
            .Any(token => new Regex(token).IsMatch(text));
    }
}