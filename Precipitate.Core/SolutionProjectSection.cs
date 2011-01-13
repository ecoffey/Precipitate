using System;
using System.Collections.Generic;
using System.Text;

namespace Precipitate
{
    public class SolutionProjectSection
    {
        public string Type { get; private set; }
        public string When { get; private set; }
        public IEnumerable<KeyValuePair<string, string>> Pairs { get; private set; }

        public SolutionProjectSection(string type, string when, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            Type = type;
            When = when;
            Pairs = pairs;
        }

        public string PrettyPrint(int depth)
        {
            var d = depth.Depth();

            var pp = new StringBuilder(String.Format("{0}{1} : {2}\n", d, Type, When));

            var d2 = (depth + 1).Depth();
            foreach (var pair in Pairs)
            {
                pp.AppendFormat("{0}{1} : {2}\n", d2, pair.Key, pair.Value);
            }

            return pp.ToString();
        }
    }
}