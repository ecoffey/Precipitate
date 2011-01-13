using System;
using System.Collections.Generic;
using System.Text;

namespace Precipitate
{
    public sealed class SolutionProject
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Filepath { get; private set; }
        public IEnumerable<SolutionProjectSection> Sections { get; private set; }

        public SolutionProject(string id, string name, string filepath, IEnumerable<SolutionProjectSection> sections)
        {
            Id = id;
            Name = name;
            Filepath = filepath;
            Sections = sections;
        }

        public string PrettyPrint(int depth)
        {
            var d = depth.Depth();

            var pp = new StringBuilder(String.Format("{0}{1} : {2} : {3}\n", d, Name, Id, Filepath));

            foreach (var section in Sections)
            {
                pp.AppendFormat("{0}\n", section.PrettyPrint(depth + 1));
            }

            return pp.ToString();
        }

        public override string ToString()
        {
            return PrettyPrint(0);
        }
    }
}