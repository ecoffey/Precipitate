using System;
using System.Collections.Generic;
using Sprache;

namespace Precipitate.Parsing
{
    internal static class ParseString
    {
        internal static Parser<string> UntilWhitespace()
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from word in Parse.Char(p => !Char.IsWhiteSpace(p), "Consume until we hit whitespace").Many().Text()
                select word;
        }

        internal static Parser<string> UntilCharacter(char endingCharacter)
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from word in Parse.Char(p => p != endingCharacter, "Consume until we hit endingCharacter").Many().Text()
                select word;
        }

        internal static Parser<string> UntilCharacters(params char[] characters)
        {
            var charactersLookup = new HashSet<char>(characters);
            return
                from word in Parse.Char(p => !charactersLookup.Contains(p), "Consume until we hit a specified character").Many().Text()
                select word;
        }
    }
}