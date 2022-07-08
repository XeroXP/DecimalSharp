using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightToNumber")]
    public class BigDecimalLightToNumberTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToNumber()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 20,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = -7,
                ToExpPos = 21
            });

            var t = (BigDecimalArgument<BigDecimalLight> n, double expected) =>
            {
                BigDecimalTests.assertEqual(expected, bigDecimalFactory.BigDecimal(n).ToNumber());
            };

            t(1, 1);
            t("1", 1);
            t("1.0", 1);
            t("1e+0", 1);
            t("1e-0", 1);

            t(-1, -1);
            t("-1", -1);
            t("-1.0", -1);
            t("-1e+0", -1);
            t("-1e-0", -1);

            t("123.456789876543", 123.456789876543);
            t("-123.456789876543", -123.456789876543);

            t("1.1102230246251565e-16", 1.1102230246251565e-16);
            t("-1.1102230246251565e-16", -1.1102230246251565e-16);

            t("9007199254740991", 9007199254740991);
            t("-9007199254740991", -9007199254740991);

            t("5e-324", 5e-324);
            t("1.7976931348623157e+308", 1.7976931348623157e+308);

            t("9.999999e+9000000000000000", double.PositiveInfinity);
            t("-9.999999e+9000000000000000", double.NegativeInfinity);
            t("1e-9000000000000000", 0);
            //t("-1e-9000000000000000", -0);

            Assert.Pass();
        }
    }
}