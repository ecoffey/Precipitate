using System;
using System.IO;
using Sprache;

namespace Precipitate.Parsing
{
    internal static class StringUntil
    {
        internal static Parser<string> Whitespace()
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from word in Parse.Char(p => !Char.IsWhiteSpace(p), "Consume until we hit whitespace").Many().Text()
                select word;
        }

        internal static Parser<string> Character(char endingCharacter)
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from word in Parse.Char(p => p != endingCharacter, "Consume until we hit endingCharacter").Many().Text()
                select word;
        }
    }
    public interface ISolutionParserFactory
    {
        ISolutionParser ForFile(string filePath);
    }

    public sealed class SolutionParserFactory : ISolutionParserFactory
    {
        private static Parser<string> GetSolutionVersion()
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from header in Parse.String("Microsoft Visual Studio Solution File, Format Version ")
                from version in StringUntil.Whitespace()
                select version;
        }

        public ISolutionParser ForFile(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                var fileContents = reader.ReadToEnd();

                var version = GetSolutionVersion().Parse(fileContents);

                return new Vs2010SolutionParser(String.Empty, filePath, fileContents);
            }
        }
    }
}