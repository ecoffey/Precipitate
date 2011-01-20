using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Precipitate.Parsing;

namespace Precipitate
{
    class Program
    {
        static void Main(string[] args)
        {
            var solutionFile = args[0];
            var projectName = args[1];

            var solutionParserFactory = new SolutionParserFactory();

            var solutionParser = solutionParserFactory.ForFile(solutionFile);

            var solution = solutionParser.ParseSolution();

            Console.WriteLine(solution);

            var project = solution.OpenProject(projectName);

            Console.WriteLine(project);
        }
    }
}
