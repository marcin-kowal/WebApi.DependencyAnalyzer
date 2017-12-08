using System.Collections.Generic;
using System.Linq;

namespace WebApi.DependencyAnalyzer.Engine.Scanning
{
    internal class Line
    {
        public Line(string text, long hash)
            :this(text, new[] { hash })
        { }

        public Line(string text, IReadOnlyCollection<long> hashes)
        {
            Text = text;
            Hashes = hashes;
        }

        public string Text { get; }
        public IReadOnlyCollection<long> Hashes { get; }

        public Line Append(Line line)
        {
            string newText = Text + line.Text;

            IReadOnlyCollection<long> newHashes = Hashes
                .Concat(line.Hashes)
                .Distinct()
                .ToArray();

            return new Line(newText, newHashes);
        }
    }
}