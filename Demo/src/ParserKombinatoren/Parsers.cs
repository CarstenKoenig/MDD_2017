/* ***************************************************************
 * adaptiert von G. Hutton, E. Meijer 
 *               "Functional Pearls - Monadic Parsing in Haskell"
 * siehe http://www.cs.nott.ac.uk/~pszgmh/pearl.pdf
 *****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserKombinatoren
{
    public delegate ParserResult<T> Parser<T>(ParserPosition position);

    public static class Parsers
    {

        // ********************* Parsen *****************

        public static T Parse<T>(this Parser<T> parser, string text)
        {
            return
                parser.TryParse(
                    text,
                    x => x,
                    (err, p) => throw new Exception(err));
        }

        public static TRes TryParse<T, TRes>(
            this Parser<T> parser, string text,
            Func<T, TRes> onSuccess,
            Func<string, ParserPosition, TRes> onError)
        {
            var start = ParserPosition.StarteMit(text);
            var result = parser(start);
            return
                result.Match((v, _) => onSuccess(v), onError);
        }

        public static void TryParse<T>(this Parser<T> parser, string text, Action<T> onSuccess,
            Action<string, ParserPosition> onError)
        {
            TryParse(parser, text, res =>
                {
                    onSuccess(res);
                    return 0;
                },
                (err, pos) =>
                {
                    onError(err, pos);
                    return -1;
                });
        }

        // ************************ primitive Parser **************************

        public static Parser<T> Return<T>(T value)
        {
            return pos => ParserResult<T>.Succeed(value, pos);
        }

        public static Parser<T> Fail<T>(string error)
        {
            return pos => ParserResult<T>.Failed(error, pos);
        }

        public static Parser<char> Satisfy(Func<char, bool> property)
        {
            return pos =>
            {
                if (!pos.CurrentChar.HasValue)
                    return ParserResult<char>.Failed("Unerwartetes Ende der Eingabe", pos);

                var zeichen = pos.CurrentChar.Value;

                return !property(zeichen)
                    ? ParserResult<char>.Failed($"Zeichen [{zeichen}] erfüllt Bedingung nicht", pos)
                    : ParserResult<char>.Succeed(zeichen, pos.Next());
            };
        }

        public static Parser<Unit> EndOfInput()
        {
            return pos => pos.IsEndOfInput
                ? ParserResult<Unit>.Succeed(Unit.unit, pos)
                : ParserResult<Unit>.Failed("Ende des Inputs erwartet", pos);
        }

        // ****************************** Kombinatoren*************************

        public static Parser<T> Choice<T>(Parser<T> parserA, Parser<T> parserB)
        {
            return pos => parserA(pos)
                .Match(
                    ParserResult<T>.Succeed
                    , (err, _) => parserB(pos));
        }


        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        {
            return Choice(Many1(parser), Return(Enumerable.Empty<T>()));
        }

        public static Parser<IEnumerable<T>> Many1<T>(this Parser<T> parser)
        {
            return
                from item in parser
                from items in Many(parser)
                select new[] { item }.Concat(items);
        }

        public static Parser<T> Chainl1<T>(this Parser<T> elemParser, Parser<Func<T, T, T>> opParser)
        {
            Parser<T> Rest(T a)
            {
                var more =
                    from f in opParser
                    from b in elemParser
                    from r in Rest(f(a, b))
                    select r;

                return Choice(more, Return(a));
            }

            return
                from x in elemParser
                from y in Rest(x)
                select y;

        }

        // *************************** Hilffunktionen *************************

        public static string PrettyPrintError(string error, ParserPosition atPosition)
        {
            var left = atPosition.Index > 3
                ? "..." + atPosition.Text.Substring(atPosition.Index - 3).TakeMax(30)
                : atPosition.Text.TakeMax(30);

            var indentLen = atPosition.Index > 3
                ? 6
                : atPosition.Index;

            var indent =
                new string(Enumerable.Repeat(' ', indentLen).ToArray());

            return left + "\n" + indent + "^ " + error;
        }

        public static Parser<T> ErrorText<T>(this Parser<T> parser, string error)
        {
            return pos => parser(pos).OverwriteError(error);
        }


        // ********************** Funktor *************************************

        public static Parser<TRes> Map<T, TRes>(this Parser<T> parser, Func<T, TRes> map)
        {
            return pos => parser(pos).Map(map);
        }

        public static Parser<TRes> Const<T, TRes>(this Parser<T> parser, TRes constVal)
        {
            return parser.Map(_ => constVal);
        }

        // ************************ Monade ************************************

        public static Parser<TRes> Bind<T, TRes>(this Parser<T> parser, Func<T, Parser<TRes>> bind)
        {
            return pos => parser(pos)
                .Match(
                    (v, p) => bind(v)(p),
                    ParserResult<TRes>.Failed
                );
        }

        public static Parser<TRes> SelectMany<TSrc, TRes>(this Parser<TSrc> source, Func<TSrc, Parser<TRes>> selector)
        {
            return source.Bind(selector);
        }

        public static Parser<TRes> SelectMany<TSrc, TCol, TRes>(this Parser<TSrc> source,
            Func<TSrc, Parser<TCol>> collectionSelector, Func<TSrc, TCol, TRes> resultSelector)
        {
            return pos => source(pos)
                .Match(
                    (src, p) => collectionSelector(src)(p).Map(col => resultSelector(src, col))
                    , ParserResult<TRes>.Failed);

        }

        // ******************************* Applikative ************************

        public static Parser<TRes> Apply<T, TRes>(this Parser<Func<T, TRes>> fParser, Parser<T> valueParser)
        {
            return fParser.Bind(valueParser.Map);
        }

    }
}