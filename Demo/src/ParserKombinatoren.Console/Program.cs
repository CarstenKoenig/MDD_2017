using System;
using ParserKombinatoren.Taschenrechner;

namespace ParserKombinatoren.Console
{
    internal static class Program
    {
        private static void Main()
        {
            var eingaeParser =
                from e in Syntax.Expression
                from eoi in Parsers.EndOfInput()
                select e;

            while (true)
            {
                System.Console.Write("> ");
                var input = System.Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                eingaeParser.TryParse(input,
                    System.Console.WriteLine,
                    (err, pos) =>
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine(Parsers.PrettyPrintError(err, pos));
                        System.Console.ResetColor();
                    });
            }
        }
    }
}
