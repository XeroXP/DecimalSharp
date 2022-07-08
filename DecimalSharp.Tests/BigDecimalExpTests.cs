using DecimalSharp.Core;
using NUnit.Framework;
using System;
using System.Globalization;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalExp")]
    public class BigDecimalExpTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Exp()
        {
            var bigDecimalFactory = new BigDecimalFactory(new BigDecimalConfig()
            {
                Precision = 40,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15,
                MaxE = (long)9e15,
                MinE = (long)-9e15
            });

            var t = (BigDecimalArgument<BigDecimal> n, string expected, long pr, RoundingMode rm) =>
            {
                var config = bigDecimalFactory.Config.Clone();
                config.Precision = pr;
                config.Rounding = rm;

                BigDecimalTests.assertEqual(expected, new BigDecimal(n, config).Exp().ValueOf());
            };

            t("0", "1", 40, RoundingMode.ROUND_HALF_UP);
            t("0", Math.Exp(0).ToString(CultureInfo.InvariantCulture), 40, RoundingMode.ROUND_HALF_UP);
            t("-0", "1", 40, RoundingMode.ROUND_HALF_UP);
            //t("-0", Math.Exp("-0").ToString(CultureInfo.InvariantCulture), 40, 4);
            t("Infinity", "Infinity", 40, RoundingMode.ROUND_HALF_UP);
            t("Infinity", Math.Exp(double.PositiveInfinity).ToString(CultureInfo.InvariantCulture), 40, RoundingMode.ROUND_HALF_UP);
            t("-Infinity", "0", 40, RoundingMode.ROUND_HALF_UP);
            t("-Infinity", Math.Exp(double.NegativeInfinity).ToString(CultureInfo.InvariantCulture), 40, RoundingMode.ROUND_HALF_UP);
            t("NaN", "NaN", 40, RoundingMode.ROUND_HALF_UP);
            t("NaN", Math.Exp(double.NaN).ToString(CultureInfo.InvariantCulture), 40, RoundingMode.ROUND_HALF_UP);
            t("1", "2.718281828459045235360287471352662497757", 40, RoundingMode.ROUND_HALF_UP);

            t("4.504575", "90.42990317191332252519829", 25, RoundingMode.ROUND_HALF_DOWN);
            t("6.3936622751479561979", "598.04277120550571020949043340838952845520759628012", 50, RoundingMode.ROUND_HALF_UP);
            t("0.000000000000000004", "1.000000000000000004000000000000000008001", 40, RoundingMode.ROUND_CEIL);
            t("0.9", "2.45960311115694966380012656360247069542177230644", 48, RoundingMode.ROUND_DOWN);
            t("-0.0000000000000005", "0.999999999999999500000000000000124", 33, RoundingMode.ROUND_DOWN);
            t("-0.00000000000000000001", "0.99999999999999999999000000000000000000004999", 44, RoundingMode.ROUND_FLOOR);
            t("-0.000000000000004", "0.999999999999996000000000000008", 31, RoundingMode.ROUND_UP);
            t("-0.0000000000000000006", "0.99999", 5, RoundingMode.ROUND_FLOOR);
            t("0.0000000000000000006", "1", 5, RoundingMode.ROUND_FLOOR);
            t("-0.0000003", "1", 1, RoundingMode.ROUND_HALF_UP);

            // Initial exponent estimate incorrect by -1
            t("20.72326583694641116", "1000000000.0000000038", 20, RoundingMode.ROUND_DOWN);

            // Initial exponent estimate incorrect by +1
            t("-27.6310211159285483", "0.000000000000999", 3, RoundingMode.ROUND_DOWN);

            t("-0.9", "0.40656965974059911188345423964562", 32, RoundingMode.ROUND_DOWN);
            t("-0.00000000000005", "0.999", 3, RoundingMode.ROUND_FLOOR);
            t("-0.9999999999999999", "0.367879441171442358383467887305694866395394004", 45, RoundingMode.ROUND_HALF_UP);
            t("-0.99999", "0.36788311998424806939070532012638041", 35, RoundingMode.ROUND_FLOOR);
            t("-0.00000000001", "0.99999999999000000000004999999999983333333333375", 49, RoundingMode.ROUND_CEIL);
            t("-0.9999999999999", "0.367879441171479109539640916233017625680100198337", 48, RoundingMode.ROUND_UP);
            t("-0.999999999", "0.36787944153932176295090581241", 29, RoundingMode.ROUND_UP);
            t("-0.0000000003", "0.9999999997000000001", 19, RoundingMode.ROUND_CEIL);
            t("-0.0000001", "0.99999990000000499999983333333749999991667", 41, RoundingMode.ROUND_CEIL);
            t("-0.0000000000000001", "0.9999999999999999", 26, RoundingMode.ROUND_DOWN);
            t("-0.999999999999999", "0.36788", 5, RoundingMode.ROUND_CEIL);
            t("-0.999999999", "0.367879441539321762951", 21, RoundingMode.ROUND_HALF_UP);
            t("-0.000000000001", "0.9999999999990000000000005", 31, RoundingMode.ROUND_UP);
            t("-0.1", "0.9048374180359595731642491", 25, RoundingMode.ROUND_UP);
            t("-0.99999999", "0.36787944485", 12, RoundingMode.ROUND_FLOOR);
            t("-0.99999999", "0.36787944485023675170391910600205499737", 38, RoundingMode.ROUND_UP);
            t("-0.1", "0.9048374180359595731642491", 25, RoundingMode.ROUND_UP);
            t("-0.9", "0.4065696597", 10, RoundingMode.ROUND_FLOOR);
            t("-0.9999999999999", "0.367879441171479", 15, RoundingMode.ROUND_FLOOR);
            t("-0.99", "0.371576691022045690531524119908201386918028", 42, RoundingMode.ROUND_FLOOR);
            t("-0.999999999999999", "0.3678794411714426894749649417", 28, RoundingMode.ROUND_UP);
            t("-0.9", "0.4", 2, RoundingMode.ROUND_DOWN);
            t("-0.00000000009", "0.99999", 5, RoundingMode.ROUND_DOWN);
            t("0.9", "2.45960311115694966380012656360247069542177230644", 48, RoundingMode.ROUND_DOWN);
            t("40.95984262795251", "614658133673303019.41715", 23, RoundingMode.ROUND_DOWN);
            t("50.57728", "9234930123395249855007.64784227728909958776637", 45, RoundingMode.ROUND_UP);
            t("-9.295952106254287693", "0.00009179505707794839996147521992", 28, RoundingMode.ROUND_FLOOR);

            t("24.429", "40679902037.5", 12, RoundingMode.ROUND_DOWN);
            t("3.085347", "21.875056169741656067", 20, RoundingMode.ROUND_CEIL);
            t("6.079163", "436.663554324263441178", 21, RoundingMode.ROUND_UP);
            t("0.89588138", "2.4494937731", 11, RoundingMode.ROUND_HALF_DOWN);
            t("3.06", "21.3", 3, RoundingMode.ROUND_HALF_UP);
            t("0.828620743", "2.2901578446832146", 17, RoundingMode.ROUND_HALF_EVEN);
            t("0.8747", "2.39815573", 9, RoundingMode.ROUND_DOWN);
            t("4", "54.5", 3, RoundingMode.ROUND_FLOOR);
            t("1.74023", "5.698653962365493026791", 22, RoundingMode.ROUND_FLOOR);
            t("0.3178134", "1.37411982654", 12, RoundingMode.ROUND_HALF_DOWN);
            t("1.0212228", "2.77658790066265475", 18, RoundingMode.ROUND_UP);
            t("2.8", "16.444646771097049871498016", 26, RoundingMode.ROUND_HALF_EVEN);
            t("2", "7.389", 5, RoundingMode.ROUND_DOWN);
            t("2.13349", "8.44428600324102919", 19, RoundingMode.ROUND_DOWN);
            t("1.0306766", "2.8029617", 8, RoundingMode.ROUND_HALF_UP);
            t("1.38629371", "3.99999739553", 12, RoundingMode.ROUND_UP);
            t("2.140864956", "8.5", 2, RoundingMode.ROUND_HALF_UP);
            t("1", "2.7182818284590452353602874713", 29, RoundingMode.ROUND_DOWN);
            t("2.8", "16.4446467711", 13, RoundingMode.ROUND_HALF_UP);
            t("1.7923271", "6.0034067514286690274238254973", 29, RoundingMode.ROUND_UP);
            t("2", "7.38905609893065", 15, RoundingMode.ROUND_HALF_UP);
            t("1.839758663", "6.2950188567239", 14, RoundingMode.ROUND_DOWN);
            t("3.1541", "23.4319388536798", 15, RoundingMode.ROUND_FLOOR);
            t("6.23103", "508.27874", 8, RoundingMode.ROUND_DOWN);
            t("0.15", "1.16183424272828312261663", 24, RoundingMode.ROUND_UP);
            t("3.6454", "38.298089", 8, RoundingMode.ROUND_HALF_UP);
            t("2.8086602", "16.5877", 6, RoundingMode.ROUND_CEIL);
            t("1", "2.71828182845904523536", 22, RoundingMode.ROUND_HALF_UP);
            t("3.712", "40.9355959021562903", 19, RoundingMode.ROUND_CEIL);
            t("1.742336005", "5.71066800248", 12, RoundingMode.ROUND_HALF_DOWN);

            t("67.0234090932359557332", "129000000000000000000000000000", 3, RoundingMode.ROUND_CEIL);
            t("6.4350484439574", "623.3127778698531510658792212713024749828103299", 46, RoundingMode.ROUND_FLOOR);
            t("-90.6147801309103528", "0.0000000000000000000000000000000000000004430992452392223286671364132586", 31, RoundingMode.ROUND_UP);
            t("52.6735295600", "75131702520984694212520.839", 26, RoundingMode.ROUND_HALF_DOWN);
            t("4.91754742409", "136.667015585278752656929641054712859399847337855456678258883", 60, RoundingMode.ROUND_UP);
            t("-8.291786018917236430647515856", "0.0002505", 4, RoundingMode.ROUND_FLOOR);

            bigDecimalFactory.Config.ToExpNeg = 0;
            bigDecimalFactory.Config.ToExpPos = 0;

            // Max integer argument
            t("20723265836946413", "6.3207512951460243608e+9000000000000000", 20, RoundingMode.ROUND_HALF_UP);

            // Min integer argument
            t("-20723265836946411", "1.1690154783664756563e-9000000000000000", 20, RoundingMode.ROUND_HALF_UP);

            t("2.08E+16", "Infinity", 10, RoundingMode.ROUND_DOWN);
            t("9.99999999e+9000000000000000", "Infinity", 100, RoundingMode.ROUND_HALF_UP);

            t("-2.08E+16", "0e+0", 10, RoundingMode.ROUND_DOWN);
            t("1e-9000000000000000", "1e+0", 10, RoundingMode.ROUND_DOWN);

            // Initial exponent estimate incorrect by +1
            t("2302585095.29663062096", "9.999998439e+1000000000", 10, RoundingMode.ROUND_DOWN);

            // Initial exponent estimate incorrect by -1
            t("557.22559250455906", "1.0000000000000044e+242", 17, RoundingMode.ROUND_DOWN);

            t("-7.2204571E-4550853476128405", "9.99999e-1", 6, RoundingMode.ROUND_DOWN);
            t("-1.239848698043325450682384840", "2.894280056499551869832955260486309228756785711877e-1", 49, RoundingMode.ROUND_CEIL);
            t("-358219354.0214584957674057041104824439823073474823", "1.7279578060422445345064581640966e-155572689", 32, RoundingMode.ROUND_FLOOR);
            t("8.82661445434039879925209590467500361019097244359748402", "6.813181388774733211e+3", 19, RoundingMode.ROUND_HALF_EVEN);
            t("9.02366224E-9", "1.00000000902366228071324023326175156155718e+0", 43, RoundingMode.ROUND_HALF_EVEN);
            t("-4.4768686752786086271180252E+574398129049502", "0e+0", 15, RoundingMode.ROUND_FLOOR);

            Assert.Pass();
        }
    }
}