using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightValueOf")]
    public class BigDecimalLightValueOfTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValueOf()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 20,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15
            });

            var t = (string expected, BigDecimalArgument<BigDecimalLight> n) =>
            {
                BigDecimalTests.assertEqual(expected, bigDecimalFactory.BigDecimal(n).ValueOf());
            };

            t("0", 0);
            t("0", "0");
            t("1", 1);
            t("9", 9);
            t("90", 90);
            t("90.12", 90.12);
            t("0.1", 0.1);
            t("0.01", 0.01);
            t("0.0123", 0.0123);
            t("111111111111111111111", "111111111111111111111");
            t("0.00001", 0.00001);

            t("0", "-0");
            t("-1", -1);
            t("-9", -9);
            t("-90", -90);
            t("-90.12", -90.12);
            t("-0.1", -0.1);
            t("-0.01", -0.01);
            t("-0.0123", -0.0123);
            t("-111111111111111111111", "-111111111111111111111");
            t("-0.00001", -0.00001);

            // Exponential format
            bigDecimalFactory.Config.ToExpNeg = 0;
            bigDecimalFactory.Config.ToExpPos = 0;

            t("1e-7", 0.0000001);
            t("1.23e-7", 0.000000123);
            t("1.2e-8", 0.000000012);
            t("-1e-7", -0.0000001);
            t("-1.23e-7", -0.000000123);
            t("-1.2e-8", -0.000000012);

            t("5.73447902457635174479825134e+14", "573447902457635.174479825134");
            t("1.07688e+1", "10.7688");
            t("3.171194102379077141557759899307946350455841e+27", "3171194102379077141557759899.307946350455841");
            t("4.924353466898191177698653319742594890634579e+37", "49243534668981911776986533197425948906.34579");
            t("6.85558243926569397328633907445409866949445343654692955e+18", "6855582439265693973.28633907445409866949445343654692955");
            t("1e+0", "1");

            Assert.Pass();
        }
    }
}