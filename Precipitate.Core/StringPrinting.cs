using System;
using System.Text;

namespace Precipitate
{
    public static class StringPrinting
    {
        public static string RepeatCharacter(this int depth)
        {
            var depthBuilder = new StringBuilder();

            for (int i = 0; i < depth; i++)
            {
                depthBuilder.Append(' ');
            }

            return depthBuilder.ToString();
        }

        public static string PrettyPrint(this Solution solution, int depth)
        {
            var d = depth.RepeatCharacter();

            var pp = new StringBuilder(String.Format("{2}{0} : {1}\n", solution.Name, solution.Filename, d));

            foreach (var project in solution.Projects)
            {
                pp.AppendFormat("{0}\n", project.PrettyPrint(depth + 1));
            }

            return pp.ToString();
        }

        public static string PrettyPrint(this Project project, int depth)
        {
            var d = depth.RepeatCharacter();

            var pp = new StringBuilder(String.Format("{0}{1} : {2}\n", d, project.Name, project.AssemblyName));

            pp.AppendFormat("{0}References: \n", d);
            foreach (var reference in project.References)
            {
                pp.AppendFormat("{0}\n", reference.PrettyPrint(depth + 1));
            }

            pp.AppendFormat("{0}Project References: \n", d);
            foreach (var projectReference in project.ProjectReferences)
            {
                pp.AppendFormat("{0}\n", projectReference.PrettyPrint(depth + 1));
            }

            return pp.ToString();
        }

        public static string PrettyPrint(this ProjectConfiguration projectConfiguration, int depth)
        {
            var d = depth.RepeatCharacter();
            return String.Format("{0}{1} : {2} : {3}", d, projectConfiguration.Configuration,
                                 projectConfiguration.OutputPath, projectConfiguration.Platform);
        }

        public static string PrettyPrint(this Reference reference, int depth)
        {
            var d = depth.RepeatCharacter();

            return String.Format("{0}{1} {2}", d, reference.AssemblyName, reference.HintPath);
        }

        public static string PrettyPrint(this ProjectReference projectReference, int depth)
        {
            var d = depth.RepeatCharacter();

            return String.Format("{0}{1} {2}", d, projectReference.AssemblyName, projectReference.ReferencedProjectId);
        }
    }
}