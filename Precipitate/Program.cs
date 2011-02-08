using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sprache;
using Precipitate.Parsing;

namespace Precipitate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Pausing for debugger attach, hit enter to continue...");
            Console.ReadLine();

            try
            {

                var projectFile = args[0];

                Document document;
                using (var fileStream = File.Open(projectFile, FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                {
                    var contents = streamReader.ReadToEnd();

                    document = XmlParser.Document.End().Parse(contents);
                }

                Console.WriteLine(document);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //var solutionFile = args[0];
            //var projectName = args[1];

            //var solutionParserFactory = new SolutionParserFactory();

            //var solutionParser = solutionParserFactory.ForFile(solutionFile);

            //var solution = solutionParser.ParseSolution();

            //Console.WriteLine(solution);

            //var project = solution.OpenProject(projectName);

            //Console.WriteLine(project);
        }
    }
}
