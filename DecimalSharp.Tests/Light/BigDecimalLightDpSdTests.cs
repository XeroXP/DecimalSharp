using DecimalSharp.Core;
using NUnit.Framework;
using System;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightDpSd")]
    public class BigDecimalLightDpSdTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DpSd()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 20,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = -7,
                ToExpPos = 21
            });

            var t = (BigDecimalArgument<BigDecimalLight> n, long? dp, long? sd, bool? zs) =>
            {
                BigDecimalTests.assertEqual(dp, bigDecimalFactory.BigDecimal(n).Dp());
                BigDecimalTests.assertEqual(dp, bigDecimalFactory.BigDecimal(n).DecimalPlaces());
                BigDecimalTests.assertEqual(sd, bigDecimalFactory.BigDecimal(n).Sd(zs));
                BigDecimalTests.assertEqual(sd, bigDecimalFactory.BigDecimal(n).Precision(zs));
            };

            var tx = (Action fn, string msg) =>
            {
                BigDecimalTests.assertException(fn, msg);
            };

            t(0, 0, 1, null);
            t(-0, 0, 1, null);
            t(1, 0, 1, null);
            t(-1, 0, 1, null);

            t(100, 0, 1, null);
            t(100, 0, 1, false);
            t(100, 0, 1, false);
            t(100, 0, 3, true);
            t(100, 0, 3, true);

            t("0.0012345689", 10, 8, null);
            t("0.0012345689", 10, 8, false);
            t("0.0012345689", 10, 8, true);

            t("987654321000000.0012345689000001", 16, 31, false);
            t("987654321000000.0012345689000001", 16, 31, true);

            t("1e+123", 0, 1, null);
            t("1e+123", 0, 124, true);
            t("1e-123", 123, 1, null);
            t("1e-123", 123, 1, true);

            t("9.9999e+9000000000000000", 0, 5, false);
            t("9.9999e+9000000000000000", 0, 9000000000000001, true);
            t("-9.9999e+9000000000000000", 0, 5, false);
            t("-9.9999e+9000000000000000", 0, 9000000000000001, true);

            t("1e-9000000000000000", (long)9e15, 1, false);
            t("1e-9000000000000000", (long)9e15, 1, true);
            t("-1e-9000000000000000", (long)9e15, 1, false);
            t("-1e-9000000000000000", (long)9e15, 1, true);

            t("55325252050000000000000000000000.000000004534500000001", 21, 53, null);

            tx(() => { bigDecimalFactory.BigDecimal(1).Sd(2); }, "new Decimal(1).sd(2)");

            Assert.Pass();
        }
    }
}