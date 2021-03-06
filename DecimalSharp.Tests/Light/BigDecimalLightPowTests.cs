using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightPow")]
    public class BigDecimalLightPowTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Pow()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 40,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15
            });

            var t = (BigDecimalArgument<BigDecimalLight> @base, BigDecimalArgument<BigDecimalLight> exp, string expected, long sd) =>
            {
                var config = bigDecimalFactory.Config.Clone();
                config.Precision = sd;

                BigDecimalTests.assertEqual(expected, new BigDecimalLight(@base, config).Pow(exp).ValueOf());
            };

            //t("2.56", "6.5", "450.3599627370496", 16);
            //t("1.96", "1.5", "2.744", 15);
            //t("2.25", "9.5", "2216.8378200531005859375", 23);
            t("48.9262695992662373981", "1.0", "48.926269599266237", 17);
            //t("1.21", "0.5", "1.1", 2);
            //t("3.24", "0.5", "1.8", 2);
            //t("5344.87762641765349023882127126550721", "1.0625", "9139.7407411741874683083843738640173291", 38);
            t("91.180153837", "0.5", "9.54882997214842023704943457512609", 33);
            t("91.145", "23.8479557348417627", "54402923894673605836306983589686900000000000000", 33);
            //t("65536", "2.5", "1099511627776", 13);

            bigDecimalFactory.Config.ToExpNeg = 0;
            bigDecimalFactory.Config.ToExpPos = 0;

            t("9.9999999999999", "2220.75", "5.623413251778e+2220", 13);
            t("0.9999999999999999991999999999019999949909999999", "2220.75", "9.999999999999982233999997e-1", 25);
            t("-2", "1001", "-2.1430172143725346418e+301", 20);
            t("5.0771598579583468811E-101844", "7064449.87442997380369702938801116641723585825702571602", "3.907934864857193219594361275098983e-719466848189", 34);
            t("93720986.7819907489497420190553708041564963922285117", "39.580", "3.3e+315", 2);

            Assert.Pass();
        }
    }
}