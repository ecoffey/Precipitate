using System;
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
    }
}