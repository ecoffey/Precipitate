using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Precipitate
{
    public class ProjectConfiguration
    {
        public string Configuration { get; set; }
        public string Platform { get; set; }
        public string OutputPath { get; set; }

        public override string ToString()
        {
            return this.PrettyPrint(0);
        }
    }

    public class Reference
    {
        public string Assembly { get; set; }
        public string SpecificVersion { get; set; }
        public string HintPath { get; set; }

        public override string ToString()
        {
            return this.PrettyPrint(0);
        }
    }

    public class ProjectReference
    {
        public string AssemblyName { get; set; }
        public string ReferencedProjectId { get; set; }

        public override string ToString()
        {
            return this.PrettyPrint(0);
        }
    }

    public class ProjectParser : IProjectParser
    {
        public Project Parse(string filename, string name)
        {
            XElement fileDocument;
            using (var fileStream = File.OpenRead(filename))
            using (var streamReader = new StreamReader(fileStream))
            {
                fileDocument = XElement.Parse(streamReader.ReadToEnd(), LoadOptions.SetBaseUri);
            }

            var ns = fileDocument.GetDefaultNamespace();

            var id = fileDocument.Descendants(ns + "ProjectGuid").Single().Value;
            var configuration = fileDocument.Descendants(ns + "Configuration").Single().Value;
            var platform = fileDocument.Descendants(ns + "Platform").Single().Value;
            var outputType = fileDocument.Descendants(ns + "OutputType").Single().Value;
            var rootNamespace = fileDocument.Descendants(ns + "RootNamespace").Single().Value;
            var assemblyName = fileDocument.Descendants(ns + "AssemblyName").Single().Value;
            var targetFrameworkVersion = fileDocument.Descendants(ns + "TargetFrameworkVersion").Single().Value;

            var propertyGroups = fileDocument.Descendants(ns + "PropertyGroup").Where(e => e.HasAttributes);
            var configurations = ParseConfigurations(ns, propertyGroups);

            var referenceGroups = fileDocument.Descendants(ns + "Reference");
            var references = ParseReferences(ns, referenceGroups);

            var projectReferenceGroups = fileDocument.Descendants(ns + "ProjectReference");
            var projectReferences = ParseProjectReferences(ns, projectReferenceGroups);

            return new Project
                       {
                           Id = id,
                           Name = name,
                           Configuration = configuration,
                           Platform = platform,
                           OutputType = outputType,
                           RootNamespace = rootNamespace,
                           AssemblyName = assemblyName,
                           TargetFrameworkVersion = targetFrameworkVersion,
                           Configurations = configurations,
                           References = references,
                           ProjectReferences = projectReferences
                       };
        }

        private static IEnumerable<ProjectReference> ParseProjectReferences(XNamespace ns, IEnumerable<XElement> projectReferenceGroups)
        {
            return projectReferenceGroups.Select(projectReference => new ProjectReference
                                                                         {
                                                                             AssemblyName = projectReference.Descendants(ns + "Name").Single().Value,
                                                                             ReferencedProjectId = projectReference.Descendants(ns + "Project").Single().Value.Trim('{', '}')
                                                                         });
        }

        private static IEnumerable<ProjectConfiguration> ParseConfigurations(XNamespace ns, IEnumerable<XElement> propertyGroups)
        {
            return propertyGroups.Select(propertyGroup => new ProjectConfiguration
                                                              {
                                                                  OutputPath = propertyGroup.Descendants(ns + "OutputPath").Single().Value
                                                              });
        }

        private static IEnumerable<Reference> ParseReferences(XNamespace ns, IEnumerable<XElement> referenceGroups)
        {
            foreach (var referenceGroup in referenceGroups)
            {
                var reference = new Reference
                                    {
                                        Assembly = referenceGroup.Attribute("Include").Value
                                    };

                var specificVersion = referenceGroup.Descendants(ns + "SpecificVersion").SingleOrDefault();
                var hintPath = referenceGroup.Descendants(ns + "HintPath").SingleOrDefault();

                reference.SpecificVersion = specificVersion != null ? specificVersion.Value : String.Empty;
                reference.HintPath = hintPath != null ? hintPath.Value : String.Empty;

                yield return reference;
            }
        }
    }
}