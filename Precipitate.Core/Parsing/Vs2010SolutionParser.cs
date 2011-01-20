using System;
using System.Collections.Generic;
using System.IO;
using Sprache;

namespace Precipitate.Parsing
{
    public sealed class Vs2010SolutionParser : ISolutionParser
    {
        private readonly string _name;
        private readonly string _filePath;
        private readonly Lazy<string> _fileContents;

        internal Vs2010SolutionParser(string name, string filePath, string fileContents)
        {
            _name = name;
            _filePath = filePath;
            _fileContents = new Lazy<string>(() => fileContents);
        }

        public Vs2010SolutionParser(string name, string filePath)
        {
            _name = name;
            _filePath = filePath;
            _fileContents = new Lazy<string>(() =>
                                                 {
                                                     using (var fileStream = File.Open(filePath, FileMode.Open))
                                                     using (var reader = new StreamReader(fileStream))
                                                     {
                                                         return reader.ReadToEnd();
                                                     }
                                                 });
        }

        public Solution ParseSolution()
        {
            return Solution(_name, _filePath).End().Parse(_fileContents.Value);
        }

        private static Parser<T> Quoted<T>(Parser<T> inner)
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from openQuote in Parse.Char('"')
                from i in inner
                from closeQuote in Parse.Char('"')
                select i;
        }
        
        private static readonly Parser<string> QuotedString = Quoted(ParseString.UntilCharacter('"'));

        private static readonly Parser<string> Guid =
            from leading in Parse.WhiteSpace.Many()
            from openBrace in Parse.Char('{')
            from guidText in Parse.LetterOrDigit.XOr(Parse.Char('-')).Many().Text()
            from closeBrace in Parse.Char('}')
            select guidText;

        private static readonly Parser<string> QuotedGuid = Quoted(Guid);
        
        private static readonly Parser<KeyValuePair<string, string>> KeyValue =
            from leading in Parse.WhiteSpace.Many()
            from key in ParseString.UntilWhitespace()
            from ws in Parse.WhiteSpace.Many()
            from equal in Parse.Char('=')
            from value in ParseString.UntilWhitespace()
            select new KeyValuePair<string, string>(key, value);
        
        private static readonly Parser<SolutionProjectSection> ProjectSection =
            from leading in Parse.WhiteSpace.Many()
            from openKeyWord in Parse.String("ProjectSection(")
            from type in ParseString.UntilCharacter(')')
            from closeParen in Parse.Char(')')
            from ws1 in Parse.WhiteSpace.Many().Until(Parse.Char('='))
            from ws2 in Parse.WhiteSpace.Many()
            from preOrPost in ParseString.UntilWhitespace()
            from keyValuePairs in KeyValue.Many()
            from ws3 in Parse.WhiteSpace.Many()
            from endKeyWord in Parse.String("EndProjectSection")
            select new SolutionProjectSection(type, preOrPost, keyValuePairs);

        private static readonly Parser<SolutionProject> Project =
            from leading in Parse.WhiteSpace.Many()
            from projectKeyWord in Parse.String("Project(")
            from id in QuotedGuid
            from rightParen in Parse.Char(')')
            from ws in Parse.WhiteSpace.Many().Until(Parse.Char('='))
            from projectName in QuotedString
            from comma1 in Parse.Char(',')
            from filePath in QuotedString
            from comman2 in Parse.Char(',')
            from projectId in QuotedGuid
            from ws4 in Parse.WhiteSpace.Many()
            from sections in ProjectSection.Many()
            from ws3 in Parse.WhiteSpace.Many()
            from endKeyWord in Parse.String("EndProject")
            select new SolutionProject(projectId, projectName, filePath, sections);

        private static Parser<Solution> Solution(string name, string filename)
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from header in Parse.String("Microsoft Visual Studio Solution File, Format Version ")
                from versionString in ParseString.UntilWhitespace()
                from newLine in Parse.WhiteSpace.Many()
                from pound in Parse.AnyChar.Until(Parse.Char('\n'))
                from projects in Project.Many()
                from throwAway in Parse.AnyChar.Many()
                select new Solution(name, filename, versionString, projects);
        }


        
    }
}