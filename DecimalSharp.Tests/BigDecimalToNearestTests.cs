using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalToNearest")]
    public class BigDecimalToNearestTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToNearest()
        {
            var isMinusZero = (BigDecimal n) =>
            {
                return n.IsZero() && n.IsNegative();
            };

            var bigDecimalFactory = new BigDecimalFactory(new BigDecimalConfig()
            {
                Precision = 20,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15,
                MaxE = (long)9e15,
                MinE = (long)-9e15
            });

            var t = (bool actual) =>
            {
                BigDecimalTests.assert(actual);
            };

            t(!isMinusZero(bigDecimalFactory.BigDecimal(0).ToNearest(0)));
            t(isMinusZero(bigDecimalFactory.BigDecimal(-1).ToNearest(0)));
            t(isMinusZero(bigDecimalFactory.BigDecimal("-0").ToNearest(0)));
            t(!isMinusZero(bigDecimalFactory.BigDecimal(1).ToNearest(0)));
            t(!isMinusZero(bigDecimalFactory.BigDecimal(1).ToNearest("-0")));
            t(!isMinusZero(bigDecimalFactory.BigDecimal(1).ToNearest(-3)));
            t(isMinusZero(bigDecimalFactory.BigDecimal(-1).ToNearest(-3)));

            var t2 = (string expected, BigDecimalArgument<BigDecimal> n, BigDecimalArgument<BigDecimal> v, RoundingMode? rm) =>
            {
                BigDecimalTests.assertEqual(expected, bigDecimalFactory.BigDecimal(n).ToNearest(v, rm).ValueOf());
            };

            t2("Infinity", double.PositiveInfinity, (string?)null, null);
            t2("-Infinity", double.NegativeInfinity, (string?)null, null);
            t2("NaN", double.NaN, (string?)null, null);
            t2("NaN", double.NaN, double.NaN, null);
            t2("NaN", double.NaN, double.PositiveInfinity, null);
            t2("NaN", double.NaN, double.NegativeInfinity, null);
            t2("NaN", double.NaN, 0, null);
            t2("NaN", double.NaN, "-0", null);

            t2("Infinity", "9.999e+9000000000000000", "1e+9000000000000001", null);
            t2("Infinity", "9.999e+9000000000000000", "-1e+9000000000000001", null);
            t2("-Infinity", "-9.999e+9000000000000000", "1e+9000000000000001", null);
            t2("-Infinity", "-9.999e+9000000000000000", "-1e+9000000000000001", null);
            t2("9.999e+9000000000000000", "9.999e+9000000000000000", (string?)null, null);
            t2("-9.999e+9000000000000000", "-9.999e+9000000000000000", (string?)null, null);

            t2("NaN", 123.456, double.NaN, null);
            t2("Infinity", 123.456, double.PositiveInfinity, null);
            t2("Infinity", 123.456, double.NegativeInfinity, null);
            t2("0", 123.456, 0, null);
            t2("0", 123.456, "-0", null);

            t2("NaN", -123.456, double.NaN, null);
            t2("-Infinity", -123.456, double.PositiveInfinity, null);
            t2("-Infinity", -123.456, double.NegativeInfinity, null);
            t2("-0", -123.456, "-0", null);

            t2("0", 0, 0, null);
            t2("Infinity", 0, double.PositiveInfinity, null);
            t2("Infinity", 0, double.NegativeInfinity, null);
            t2("-Infinity", "-0", double.PositiveInfinity, null);
            t2("-Infinity", "-0", double.NegativeInfinity, null);

            t2("0", 1, -3, null);
            t2("-0", -1, -3, null);
            t2("3", 1.5, -3, RoundingMode.ROUND_UP);
            t2("-0", -1.5, -3, RoundingMode.ROUND_DOWN);
            t2("-3", -1.5, -3, RoundingMode.ROUND_CEIL);

            t2("123", 123.456, (string?)null, null);
            t2("123", 123.456, 1, null);
            t2("123.5", 123.456, 0.1, null);
            t2("123.46", 123.456, 0.01, null);
            t2("123.456", 123.456, 0.001, null);

            t2("123", 123.456, -1, null);
            t2("123.5", 123.456, -0.1, null);
            t2("123.46", 123.456, -0.01, null);
            t2("123.456", 123.456, -0.001, null);

            t2("124", 123.456, "-2", null);
            t2("123.4", 123.456, "-0.2", null);
            t2("123.46", 123.456, "-0.02", null);
            t2("123.456", 123.456, "-0.002", null);

            t2("83105511540", "83105511539.5", 1, RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.499999999999999999999999999999", 1, RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.5", "1", RoundingMode.ROUND_HALF_DOWN);
            t2("83105511540", "83105511539.5000000000000000000001", 1, RoundingMode.ROUND_HALF_DOWN);

            bigDecimalFactory.Config.Precision = 3;

            t2("83105511540", "83105511539.5", bigDecimalFactory.BigDecimal(1), RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.499999999999999999999999999999", 1, RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.5", bigDecimalFactory.BigDecimal("1"), RoundingMode.ROUND_HALF_DOWN);
            t2("83105511540", "83105511539.5000000000000000000001", 1, RoundingMode.ROUND_HALF_DOWN);

            bigDecimalFactory.Config.Precision = 20;

            t2("83105511540", "83105511539.5", -1, RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.499999999999999999999999999999", -1, RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.5", "-1", RoundingMode.ROUND_HALF_DOWN);
            t2("83105511540", "83105511539.5000000000000000000001", -1, RoundingMode.ROUND_HALF_DOWN);

            t2("-83105511540", "-83105511539.5", bigDecimalFactory.BigDecimal(-1), RoundingMode.ROUND_HALF_UP);
            t2("-83105511539", "-83105511539.499999999999999999999999999999", 1, RoundingMode.ROUND_HALF_UP);
            t2("-83105511539", "-83105511539.5", bigDecimalFactory.BigDecimal("-1"), RoundingMode.ROUND_HALF_DOWN);
            t2("-83105511540", "-83105511539.5000000000000000000001", -1, RoundingMode.ROUND_HALF_DOWN);

            t2("83105511540", "83105511539.5", 1, RoundingMode.ROUND_UP);
            t2("83105511539", "83105511539.5", 1, RoundingMode.ROUND_DOWN);
            t2("83105511540", "83105511539.5", 1, RoundingMode.ROUND_CEIL);
            t2("83105511539", "83105511539.5", 1, RoundingMode.ROUND_FLOOR);
            t2("83105511540", "83105511539.5", 1, RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.5", 1, RoundingMode.ROUND_HALF_DOWN);
            t2("83105511540", "83105511539.5", 1, RoundingMode.ROUND_HALF_EVEN);
            t2("83105511540", "83105511539.5", 1, RoundingMode.ROUND_HALF_CEIL);
            t2("83105511539", "83105511539.5", 1, RoundingMode.ROUND_HALF_FLOOR);
            t2("83105511539", "83105511539.499999999999999999999999999999", (string?)null, RoundingMode.ROUND_UP);
            t2("83105511539", "83105511539.499999999999999999999999999999", 1, RoundingMode.ROUND_DOWN);
            t2("83105511539", "83105511539.499999999999999999999999999999", (string?)null, RoundingMode.ROUND_CEIL);
            t2("83105511539", "83105511539.499999999999999999999999999999", 1, RoundingMode.ROUND_FLOOR);
            t2("83105511539", "83105511539.499999999999999999999999999999", (string?)null, RoundingMode.ROUND_HALF_UP);
            t2("83105511539", "83105511539.499999999999999999999999999999", 1, RoundingMode.ROUND_HALF_DOWN);
            t2("83105511539", "83105511539.499999999999999999999999999999", (string?)null, RoundingMode.ROUND_HALF_EVEN);
            t2("83105511539", "83105511539.499999999999999999999999999999", 1, RoundingMode.ROUND_HALF_CEIL);
            t2("83105511539", "83105511539.499999999999999999999999999999", (string?)null, RoundingMode.ROUND_HALF_FLOOR);
            t2("83105511540", "83105511539.5000000000000000000001", (string?)null, RoundingMode.ROUND_UP);
            t2("83105511539", "83105511539.5000000000000000000001", 1, RoundingMode.ROUND_DOWN);
            t2("83105511540", "83105511539.5000000000000000000001", (string?)null, RoundingMode.ROUND_CEIL);
            t2("83105511539", "83105511539.5000000000000000000001", 1, RoundingMode.ROUND_FLOOR);
            t2("83105511540", "83105511539.5000000000000000000001", (string?)null, RoundingMode.ROUND_HALF_UP);
            t2("83105511540", "83105511539.5000000000000000000001", 1, RoundingMode.ROUND_HALF_DOWN);
            t2("83105511540", "83105511539.5000000000000000000001", (string?)null, RoundingMode.ROUND_HALF_EVEN);
            t2("83105511540", "83105511539.5000000000000000000001", 1, RoundingMode.ROUND_HALF_CEIL);
            t2("83105511540", "83105511539.5000000000000000000001", (string?)null, RoundingMode.ROUND_HALF_FLOOR);

            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_UP;
            t2("83105511540", "83105511539.5", (string?)null, null);

            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_DOWN;
            t2("83105511539", "83105511539.5", (string?)null, null);

            t2("3847570", "3847561.00000749", 10, RoundingMode.ROUND_UP);
            t2("42840000000000000", "42835000000000001", "1e+13", RoundingMode.ROUND_UP);
            t2("42830000000000000", "42835000000000001", "1e+13", RoundingMode.ROUND_DOWN);
            t2("42840000000000000", "42835000000000000.0002", "1e+13", RoundingMode.ROUND_UP);
            t2("42830000000000000", "42835000000000000.0002", "1e+13", RoundingMode.ROUND_DOWN);

            t2("500", "449.999", 100, RoundingMode.ROUND_UP);
            t2("400", "449.999", 100, RoundingMode.ROUND_DOWN);
            t2("500", "449.999", 100, RoundingMode.ROUND_CEIL);
            t2("400", "449.999", 100, RoundingMode.ROUND_FLOOR);
            t2("400", "449.999", 100, RoundingMode.ROUND_HALF_UP);
            t2("400", "449.999", 100, RoundingMode.ROUND_HALF_DOWN);
            t2("400", "449.999", 100, RoundingMode.ROUND_HALF_EVEN);
            t2("400", "449.999", 100, RoundingMode.ROUND_HALF_CEIL);
            t2("400", "449.999", 100, RoundingMode.ROUND_HALF_FLOOR);

            t2("-500", "-449.999", 100, RoundingMode.ROUND_UP);
            t2("-400", "-449.999", 100, RoundingMode.ROUND_DOWN);
            t2("-400", "-449.999", 100, RoundingMode.ROUND_CEIL);
            t2("-500", "-449.999", 100, RoundingMode.ROUND_FLOOR);
            t2("-400", "-449.999", 100, RoundingMode.ROUND_HALF_UP);
            t2("-400", "-449.999", 100, RoundingMode.ROUND_HALF_DOWN);
            t2("-400", "-449.999", 100, RoundingMode.ROUND_HALF_EVEN);
            t2("-400", "-449.999", 100, RoundingMode.ROUND_HALF_CEIL);
            t2("-400", "-449.999", 100, RoundingMode.ROUND_HALF_FLOOR);

            t2("500", "450", 100, RoundingMode.ROUND_UP);
            t2("400", "450", 100, RoundingMode.ROUND_DOWN);
            t2("500", "450", 100, RoundingMode.ROUND_CEIL);
            t2("400", "450", 100, RoundingMode.ROUND_FLOOR);
            t2("500", "450", 100, RoundingMode.ROUND_HALF_UP);
            t2("400", "450", 100, RoundingMode.ROUND_HALF_DOWN);
            t2("400", "450", 100, RoundingMode.ROUND_HALF_EVEN);
            t2("500", "450", 100, RoundingMode.ROUND_HALF_CEIL);
            t2("400", "450", 100, RoundingMode.ROUND_HALF_FLOOR);

            t2("-500", "-450", 100, RoundingMode.ROUND_UP);
            t2("-400", "-450", 100, RoundingMode.ROUND_DOWN);
            t2("-400", "-450", 100, RoundingMode.ROUND_CEIL);
            t2("-500", "-450", 100, RoundingMode.ROUND_FLOOR);
            t2("-500", "-450", 100, RoundingMode.ROUND_HALF_UP);
            t2("-400", "-450", 100, RoundingMode.ROUND_HALF_DOWN);
            t2("-400", "-450", 100, RoundingMode.ROUND_HALF_EVEN);
            t2("-400", "-450", 100, RoundingMode.ROUND_HALF_CEIL);
            t2("-500", "-450", 100, RoundingMode.ROUND_HALF_FLOOR);

            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_UP;
            t2("500", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_DOWN;
            t2("400", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_CEIL;
            t2("500", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_FLOOR;
            t2("400", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_UP;
            t2("500", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_DOWN;
            t2("500", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_EVEN;
            t2("500", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_CEIL;
            t2("500", "450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_FLOOR;
            t2("500", "450.001", 100, null);

            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_UP;
            t2("-500", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_DOWN;
            t2("-400", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_CEIL;
            t2("-400", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_FLOOR;
            t2("-500", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_UP;
            t2("-500", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_DOWN;
            t2("-500", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_EVEN;
            t2("-500", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_CEIL;
            t2("-500", "-450.001", 100, null);
            bigDecimalFactory.Config.Rounding = RoundingMode.ROUND_HALF_FLOOR;
            t2("-500", "-450.001", 100, null);

            Assert.Pass();
        }
    }
}