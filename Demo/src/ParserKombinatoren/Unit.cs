namespace ParserKombinatoren
{
    public class Unit
    {
        private Unit()
        {

        }

        static Unit()
        {
            _unit = new Unit();
        }

        private static readonly Unit _unit;

        public static Unit unit => _unit;
    }
}