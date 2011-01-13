using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Precipitate.Parsing;

namespace Precipitate
{
    public sealed class Solution
    {
        public string Name { get; private set; }
        public string Filename { get; private set; }
        public string Version { get; private set; }

        public IEnumerable<SolutionProject> Projects { get; private set; }
        
        public Solution(string name, string filename, string version, IEnumerable<SolutionProject> projects)
        {
            Name = name;
            Filename = filename;
            Version = version;
            Projects = projects;
        }

        public Project OpenProject(string name)
        {
            var projectPointer = Projects.Where(p => p.Name == name).SingleOrDefault();

            if (projectPointer == null)
            {
                throw new InvalidOperationException("Could not find specified Project");
            }

            return Project.Open(projectPointer.Filepath, projectPointer.Name);
        }

        public string PrettyPrint(int depth)
        {
            var d = depth.Depth();

            var pp = new StringBuilder(String.Format("{2}{0} : {1}\n", Name, Filename, d));

            foreach (var project in Projects)
            {
                pp.AppendFormat("{0}\n", project.PrettyPrint(depth + 1));
            }

            return pp.ToString();
        }

        public override string ToString()
        {
            return PrettyPrint(0);
        }
    }
}
