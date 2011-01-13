using System;
using System.Collections.Generic;
using System.IO;
using Sprache;

namespace Precipitate.Parsing
{
    public sealed class SolutionParserFactory : ISolutionParserFactory
    {
        private static Parser<string> GetSolutionVersion()
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from header in Parse.String("Microsoft Visual Studio Solution File, Format Version ")
                from version in ParseString.UntilWhitespace()
                select version;
        }

        public ISolutionParser ForFile(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                var name = Path.GetFileNameWithoutExtension(filePath);
                var fileContents = reader.ReadToEnd();

                var version = GetSolutionVersion().Parse(fileContents);

                return GetParserFromVersion(version, name, filePath, fileContents);
            }
        }

        private static ISolutionParser GetParserFromVersion(string version, string name, string filepath, string fileContents)
        {
            switch (version.Trim())
            {
                case "11.00":
                    return new Vs2010SolutionParser(name, filepath, fileContents);
                default:
                    throw new InvalidOperationException("Unknown version string");
            }
        }
    }
}