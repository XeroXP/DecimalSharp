using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightExp")]
    public class BigDecimalLightExpTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Exp()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 40,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15
            });

            var t = (BigDecimalArgument<BigDecimalLight> n, string expected, long pr) =>
            {
                var config = bigDecimalFactory.Config.Clone();
                config.Precision = pr;

                BigDecimalTests.assertEqual(expected, new BigDecimalLight(n, config).Exp().ValueOf());
            };

            t("0.9", "2.45960311115694966380012656360247069542177230644", 48);
            t("-0.0000000000000005", "0.999999999999999500000000000000124", 33);

            // Initial exponent estimate incorrect by -1
            t("20.72326583694641116", "1000000000.0000000038", 20);

            // Initial exponent estimate incorrect by +1
            t("-27.6310211159285483", "0.000000000000999", 3);

            t("-0.9", "0.40656965974059911188345423964562", 32);
            t("-0.0000000000000001", "0.9999999999999999", 26);
            t("-0.9", "0.4", 2);
            t("-0.00000000009", "0.99999", 5);
            t("0.9", "2.45960311115694966380012656360247069542177230644", 48);
            t("40.95984262795251", "614658133673303019.41715", 23);

            t("24.429", "40679902037.5", 12);
            t("0.8747", "2.39815573", 9);
            t("2", "7.389", 5);
            t("2.13349", "8.44428600324102919", 19);
            t("1", "2.7182818284590452353602874713", 29);
            t("1.839758663", "6.2950188567239", 14);
            t("6.23103", "508.27874", 8);

            bigDecimalFactory.Config.ToExpNeg = 0;
            bigDecimalFactory.Config.ToExpPos = 0;

            t("1e-9000000000000000", "1e+0", 10);

            // Initial exponent estimate incorrect by +1
            t("2302585095.29663062096", "9.999998439e+1000000000", 10);

            // Initial exponent estimate incorrect by -1
            t("557.22559250455906", "1.0000000000000044e+242", 17);

            t("-7.2204571E-4550853476128405", "9.99999e-1", 6);

            Assert.Pass();
        }
    }
}