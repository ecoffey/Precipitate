using System.Collections.Generic;
using System.IO;

namespace Precipitate
{
    public sealed class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Configuration { get; set; }
        public string Platform { get; set; }
        public string OutputType { get; set; }

        public string RootNamespace { get; set; }
        public string AssemblyName { get; set; }
        public string TargetFrameworkVersion { get; set; }

        public IEnumerable<ProjectConfiguration> Configurations { get; set; }
        public IEnumerable<Reference> References { get; set; }
        public IEnumerable<ProjectReference> ProjectReferences { get; set; }

        public override string ToString()
        {
            return this.PrettyPrint(0);
        }
    }
}