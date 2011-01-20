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

            return new ProjectParser().Parse(projectPointer.Filepath, projectPointer.Name);
        }

        public override string ToString()
        {
            return this.PrettyPrint(0);
        }
    }
}
