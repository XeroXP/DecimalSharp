using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalSign")]
    public class BigDecimalSignTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Sign()
        {
            var bigDecimalFactory = new BigDecimalFactory();

            var t = (BigDecimalArgument<BigDecimal> n, int? expected) =>
            {
                BigDecimalTests.assertEqual(expected, bigDecimalFactory.Sign(n));
            };

            t(double.NaN, null);
            t("NaN", null);
            t(double.PositiveInfinity, 1);
            t(double.NegativeInfinity, -1);
            t("Infinity", 1);
            t("-Infinity", -1);

            BigDecimalTests.assert((double)1 / bigDecimalFactory.Sign("0") == double.PositiveInfinity);
            BigDecimalTests.assert((double)1 / bigDecimalFactory.Sign(bigDecimalFactory.BigDecimal("0")) == double.PositiveInfinity);
            //BigDecimalTests.assert((double)1 / bigDecimalFactory.sign("-0") == double.NegativeInfinity);
            //BigDecimalTests.assert((double)1 / bigDecimalFactory.sign(bigDecimalFactory.BigDecimal("-0")) == double.NegativeInfinity);

            t("0", 0);
            //t("-0", -0);
            t("1", 1);
            t("-1", -1);
            t("9.99", 1);
            t("-9.99", -1);

            Assert.Pass();
        }
    }
}