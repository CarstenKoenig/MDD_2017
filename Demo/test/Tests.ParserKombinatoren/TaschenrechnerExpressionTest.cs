using NUnit.Framework;
using ParserKombinatoren;
using ParserKombinatoren.Taschenrechner;

namespace Tests.ParserKombinatoren
{
    [TestFixture]
    public class TaschenrechnerExpressionTest
    {
        [Test]
        public void ErkenntPositiveZahl42()
        {
            AssertErgbnis("42", 42);
        }

        [Test]
        public void ErkenntNegativeZahlMinus2()
        {
            AssertErgbnis("-2", -2);
        }

        [Test]
        public void BerechnetEinfacheAdditionKorrekt()
        {
            AssertErgbnis("4 + 6", 10);
        }


        [Test]
        public void BerechnetEinfacheSubtraktionKorrekt()
        {
            AssertErgbnis("5 - 25", -20);
        }

        [Test]
        public void BerechnetAdditionMitNegativerZahlKorrekt()
        {
            AssertErgbnis("10 + -5", 5);
        }

        [Test]
        public void BerechnetMultiplikationKorrekt()
        {
            AssertErgbnis("10 * 5", 50);
        }

        [Test]
        public void BerechnetDivisionKorrekt()
        {
            AssertErgbnis("10 / 5", 2);
        }

        [Test]
        public void BeachtetPunktVorStricht()
        {
            AssertErgbnis("5 + 5 * 5", 30);
        }

        [Test]
        public void BerechnetKlammernKorrekt()
        {
            AssertErgbnis("(5 + 5) * 5", 50);
        }


        private void AssertErgbnis(string input, int expected)
        {
            var result = Syntax.Expression.Parse(input);
            Assert.AreEqual(expected, result);
        }
    }
}
