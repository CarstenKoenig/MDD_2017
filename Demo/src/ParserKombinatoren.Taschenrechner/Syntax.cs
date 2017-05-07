/*
 * wir wollen folgende Grammatik parsen:
 * expr   ::= expr addop term | term
 * term   ::= term mulop factor | factor
 * factor ::= int | ( expr )
 * int    ::= - digits | digits
 * digit  ::= (0 | 1 | . . . | 9)*
 * addop  ::= + | -
 * mulop  ::= * | /
 */

using System;
using static ParserKombinatoren.Parsers;

namespace ParserKombinatoren.Taschenrechner
{
    public static class Syntax
    {
        public static Parser<int> Expression => 
            Term.Chainl1(AddOp)
            .TrimLeft();


        private static Parser<int> Term => 
            Factor.Chainl1(MulOp);


        private static Parser<int> Factor
        {
            get
            {
                var inParents =
                        from l in Tokens.Symbol('(')
                        from i in Expression
                        from r in Tokens.Symbol(')')
                        select i;

                return Choice(Tokens.Int, inParents)
                    .ErrorText("( oder Ganzzahl erwartet");
            }
        }


        private static Parser<Func<int, int, int>> MulOp
        {
            get
            {
                return Choice(
                        Tokens.Symbol('*').Const<char, Func<int, int, int>>((x, y) => x * y),
                        Tokens.Symbol('/').Const<char, Func<int, int, int>>((x, y) => x / y)
                    ).ErrorText("* oder / erwartet");
            }
        }


        private static Parser<Func<int, int, int>> AddOp
        {
            get
            {
                return Choice(
                        Tokens.Symbol('+').Const<char, Func<int, int, int>>((x, y) => x + y),
                        Tokens.Symbol('-').Const<char, Func<int, int, int>>((x, y) => x - y)
                    ).ErrorText("+ oder - erwartet");
            }
        }
    }
}

