using System;

namespace ParserKombinatoren.Taschenrechner
{
    public static class Tokens
    {
        public static Parser<char> Symbol(char symbol)
        {
            return Parsers
                .Satisfy(c => symbol == c)
                .TrimRight()
                .ErrorText($"{symbol} erwartet");
        }

        public static Parser<int> Int
        {
            get
            {
                var sign = Parsers
                    .Choice(
                    Symbol('-').Const<char, Func<int, int>>(x => -x),
                    Parsers.Return<Func<int, int>>(x => x))
                    .ErrorText("- oder Ziffer erwartet");

                var digits = Parsers
                    .Satisfy(char.IsDigit)
                    .Many1()
                    .Map(Helpers.ToString)
                    .Map(int.Parse)
                    .ErrorText("Ziffer erwartet");

                return sign.Apply(digits).TrimRight();
            }
        }
    }
}