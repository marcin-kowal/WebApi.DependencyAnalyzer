
namespace WebApi.DependencyAnalyzer.Engine.Common
{
    public static class StringOperators
    {
        private readonly static Operator<string> _append = new Operator<string>((first, second) => first + second);
        private readonly static Operator<string> _prepend = new Operator<string>((first, second) => second + first);

        public static string Append(this string first, string second) => _append.Execute(first, second);
        public static string Prepend(this string first, string second) => _prepend.Execute(first, second);
    }
}