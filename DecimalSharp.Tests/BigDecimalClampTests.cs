using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalClamp")]
    public class BigDecimalClampTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Clamp()
        {
            var t = (BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> min, BigDecimalArgument<BigDecimal> max, string expected) =>
            {
                BigDecimalTests.assertEqual(expected, new BigDecimal(x).Clamp(min, max).ValueOf());
            };

            t("-0", "0", "0", "-0");
            t("-0", "-0", "0", "-0");
            t("-0", "0", "-0", "-0");
            t("-0", "-0", "-0", "-0");

            t("0", "0", "0", "0");
            t("0", "-0", "0", "0");
            t("0", "0", "-0", "0");
            t("0", "-0", "-0", "0");

            t(0, 0, 1, "0");
            t(-1, 0, 1, "0");
            t(-2, 0, 1, "0");
            t(1, 0, 1, "1");
            t(2, 0, 1, "1");

            t(1, 1, 1, "1");
            t(-1, 1, 1, "1");
            t(-1, -1, 1, "-1");
            t(2, 1, 2, "2");
            t(3, 1, 2, "2");
            t(1, 0, 1, "1");
            t(2, 0, 1, "1");

            t(double.PositiveInfinity, 0, 1, "1");
            t(0, double.NegativeInfinity, 0, "0");
            t(double.NegativeInfinity, 0, 1, "0");
            t(double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, "-Infinity");
            t(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity, "Infinity");
            t(0, 1, double.PositiveInfinity, "1");

            t(0, double.NaN, 1, "NaN");
            t(0, 0, double.NaN, "NaN");
            t(double.NaN, 0, 1, "NaN");

            Assert.Pass();
        }
    }
}