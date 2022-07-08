using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalToNumber")]
    public class BigDecimalToNumberTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToNumber()
        {
            var bigDecimalFactory = new BigDecimalFactory(new BigDecimalConfig()
            {
                Precision = 20,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = -7,
                ToExpPos = 21,
                MaxE = (long)9e15,
                MinE = (long)-9e15
            });

            var t = (BigDecimalArgument<BigDecimal> n) =>
            {
                BigDecimalTests.assert((double)1 / bigDecimalFactory.BigDecimal(n).ToNumber() == double.PositiveInfinity);
            };

            t("0");
            t("0.0");
            t("0.000000000000");
            t("0e+0");
            t("0e-0");
            t("1e-9000000000000000");

            // Negative zero
            // toNumber can't return negative zero in c#
            /*t = (object n0) =>
            {
                BigDecimal? n = n0.ToBigDecimal(bigDecimalFactory.Config);

                if (n == null)
                    Assert.Fail();

                BigDecimalTests.assert((double)1 / n.toNumber() == double.NegativeInfinity);
            };

            t("-0");
            t("-0.0");
            t("-0.000000000000");
            t("-0e+0");
            t("-0e-0");
            t("-1e-9000000000000000");*/

            var t2 = (BigDecimalArgument<BigDecimal> n, double expected) =>
            {
                BigDecimalTests.assertEqual(expected, bigDecimalFactory.BigDecimal(n).ToNumber());
            };

            t2(double.PositiveInfinity, double.PositiveInfinity);
            t2("Infinity", double.PositiveInfinity);
            t2(double.NegativeInfinity, double.NegativeInfinity);
            t2("-Infinity", double.NegativeInfinity);
            t2(double.NaN, double.NaN);
            t2("NaN", double.NaN);

            t2(1, 1);
            t2("1", 1);
            t2("1.0", 1);
            t2("1e+0", 1);
            t2("1e-0", 1);

            t2(-1, -1);
            t2("-1", -1);
            t2("-1.0", -1);
            t2("-1e+0", -1);
            t2("-1e-0", -1);

            t2("123.456789876543", 123.456789876543);
            t2("-123.456789876543", -123.456789876543);

            t2("1.1102230246251565e-16", 1.1102230246251565e-16);
            t2("-1.1102230246251565e-16", -1.1102230246251565e-16);

            t2("9007199254740991", 9007199254740991);
            t2("-9007199254740991", -9007199254740991);

            t2("5e-324", 5e-324);
            t2("1.7976931348623157e+308", 1.7976931348623157e+308);

            t2("9.999999e+9000000000000000", double.PositiveInfinity);
            t2("-9.999999e+9000000000000000", double.NegativeInfinity);
            t2("1e-9000000000000000", 0);
            //t2("-1e-9000000000000000", -0);

            Assert.Pass();
        }
    }
}