using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserKombinatoren
{
    public static class Helpers
    {
        private static readonly Parser<string> _whitespace =
            Parsers.Satisfy(char.IsWhiteSpace).Many().Map(ToString);

        public static Parser<T> IgnoreWhitespaceRight<T>(this Parser<T> parser)
        {
            return
                from p in parser
                from s in _whitespace
                select p;
        }

        public static Parser<T> IgnoreWhitespaceLeft<T>(this Parser<T> parser)
        {
            return
                from s in _whitespace
                from p in parser
                select p;
        }

        public static string TakeMax(this string text, int length)
        {
            return length <= text.Length ? text.Substring(0, length) : text;
        }

        public static string ToString(IEnumerable<char> chars)
        {
            return new string(chars.ToArray());
        }
    }
}