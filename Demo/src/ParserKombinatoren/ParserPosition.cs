using System;

namespace ParserKombinatoren
{
    public class ParserPosition
    {
        public static ParserPosition StarteMit(string text)
        {
            return new ParserPosition(text, 0);
        }

        public static ParserPosition Next(ParserPosition aktuell)
        {
            return aktuell.Next();
        }

        public ParserPosition Next()
        {
            return new ParserPosition(Text, Index + 1);
        }

        private ParserPosition(string text, int index)
        {
            Text = text;
            Index = index;
        }

        public int Index { get; }
        public string Text { get; }
        public char? CurrentChar
        {
            get
            {
                if (IsEndOfInput)
                    return null;
                return Text[Index];
            }
        }

        public bool IsEndOfInput => 
            Index >= Text.Length || Index < 0;
    }
}