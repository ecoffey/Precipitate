using System;
using System.Collections.Generic;
using System.Text;

namespace Precipitate
{
    public static class SolutionProjectType
    {
        public static readonly string Project = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
        public static readonly string SolutionFolder = "2150E333-8FDC-42A3-9474-1A3956D46DE8";
    }

    public sealed class SolutionProject
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string ProjectTypeId { get; set; }
        public string Filepath { get; private set; }
        public IEnumerable<SolutionProjectSection> Sections { get; private set; }

        public SolutionProject(string id, string name, string projectTypeId, string filepath, IEnumerable<SolutionProjectSection> sections)
        {
            Id = id;
            Name = name;
            ProjectTypeId = projectTypeId;
            Filepath = filepath;
            Sections = sections;
        }

        public string PrettyPrint(int depth)
        {
            var d = depth.RepeatCharacter();

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