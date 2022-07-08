using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalMinAndMax")]
    public class BigDecimalMinAndMaxTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MinAndMax()
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

            var t = (BigDecimalArgument<BigDecimal> min, BigDecimalArgument<BigDecimal> max, BigDecimalArgument<BigDecimal>[] arr) =>
            {
                BigDecimalTests.assertEqual(bigDecimalFactory.BigDecimal(max).ValueOf(), bigDecimalFactory.Max(arr).ValueOf());
                BigDecimalTests.assertEqual(bigDecimalFactory.BigDecimal(min).ValueOf(), bigDecimalFactory.Min(arr).ValueOf());
            };

            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {double.NaN});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {-2, 0, -1, double.NaN});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {-2, double.NaN, 0, -1});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {double.NaN, -2, 0, -1});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {double.NaN, -2, 0, -1});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {-2, 0, -1, bigDecimalFactory.BigDecimal(double.NaN)});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {-2, 0, -1, bigDecimalFactory.BigDecimal(double.NaN)});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {double.PositiveInfinity, -2, "NaN", 0, -1, double.NegativeInfinity});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {"NaN", double.PositiveInfinity, -2, 0, -1, double.NegativeInfinity});
            t(double.NaN, double.NaN, new BigDecimalArgument<BigDecimal>[] {double.PositiveInfinity, -2, double.NaN, 0, -1, double.NegativeInfinity});

            t(0, 0, new BigDecimalArgument<BigDecimal>[] {0, 0, 0});
            t(-2, double.PositiveInfinity, new BigDecimalArgument<BigDecimal>[] {-2, 0, -1, double.PositiveInfinity});
            t(double.NegativeInfinity, 0, new BigDecimalArgument<BigDecimal>[] {-2, 0, -1, double.NegativeInfinity});
            t(double.NegativeInfinity, double.PositiveInfinity, new BigDecimalArgument<BigDecimal>[] {double.NegativeInfinity, -2, 0, -1, double.PositiveInfinity});
            t(double.NegativeInfinity, double.PositiveInfinity, new BigDecimalArgument<BigDecimal>[] {double.PositiveInfinity, -2, 0, -1, double.NegativeInfinity});
            t(double.NegativeInfinity, double.PositiveInfinity, new BigDecimalArgument<BigDecimal>[] {double.NegativeInfinity, -2, 0, bigDecimalFactory.BigDecimal(double.PositiveInfinity)});

            t(-2, 0, new BigDecimalArgument<BigDecimal>[] {-2, 0, -1});
            t(-2, 0, new BigDecimalArgument<BigDecimal>[] {-2, -1, 0});
            t(-2, 0, new BigDecimalArgument<BigDecimal>[] {0, -2, -1});
            t(-2, 0, new BigDecimalArgument<BigDecimal>[] {0, -1, -2});
            t(-2, 0, new BigDecimalArgument<BigDecimal>[] {-1, -2, 0});
            t(-2, 0, new BigDecimalArgument<BigDecimal>[] {-1, 0, -2});

            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {-1, 0, 1});
            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {-1, 1, 0});
            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {0, -1, 1});
            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {0, 1, -1});
            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {1, -1, 0});
            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {1, 0, -1});

            t(0, 2, new BigDecimalArgument<BigDecimal>[] {0, 1, 2});
            t(0, 2, new BigDecimalArgument<BigDecimal>[] {0, 2, 1});
            t(0, 2, new BigDecimalArgument<BigDecimal>[] {1, 0, 2});
            t(0, 2, new BigDecimalArgument<BigDecimal>[] {1, 2, 0});
            t(0, 2, new BigDecimalArgument<BigDecimal>[] {2, 1, 0});
            t(0, 2, new BigDecimalArgument<BigDecimal>[] {2, 0, 1});

            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {"-1", 0, bigDecimalFactory.BigDecimal(1)});
            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {"-1", bigDecimalFactory.BigDecimal(1)});
            t(-1, 1, new BigDecimalArgument<BigDecimal>[] {0, "-1", bigDecimalFactory.BigDecimal(1)});
            t(0, 1, new BigDecimalArgument<BigDecimal>[] {0, bigDecimalFactory.BigDecimal(1)});
            t(1, 1, new BigDecimalArgument<BigDecimal>[] {bigDecimalFactory.BigDecimal(1)});
            t(-1, -1, new BigDecimalArgument<BigDecimal>[] {bigDecimalFactory.BigDecimal(-1)});

            t(0.0009999, 0.0010001, new BigDecimalArgument<BigDecimal>[] {0.001, 0.0009999, 0.0010001});
            t(-0.0010001, -0.0009999, new BigDecimalArgument<BigDecimal>[] {-0.001, -0.0009999, -0.0010001});
            t(-0.000001, 999.001, new BigDecimalArgument<BigDecimal>[] {2, -0, "1e-9000000000000000", 324.32423423, -0.000001, "999.001", 10});
            t("-9.99999e+9000000000000000", double.PositiveInfinity, new BigDecimalArgument<BigDecimal>[] {10, "-9.99999e+9000000000000000", bigDecimalFactory.BigDecimal(double.PositiveInfinity), "9.99999e+9000000000000000", 0});
            t("-9.999999e+9000000000000000", "1.01e+9000000000000000", new BigDecimalArgument<BigDecimal>[] {"-9.99998e+9000000000000000", "-9.999999e+9000000000000000", "9e+8999999999999999", "1.01e+9000000000000000", 1e+300});
            t(1, double.PositiveInfinity, new BigDecimalArgument<BigDecimal>[] {1, "1e+9000000000000001", 1e200});
            t(double.NegativeInfinity, 1, new BigDecimalArgument<BigDecimal>[] {1, "-1e+9000000000000001", -1e200});
            t(0, 1, new BigDecimalArgument<BigDecimal>[] {1, "1e-9000000000000001", 1e-200});
            t("-0", 1, new BigDecimalArgument<BigDecimal>[] {1, "-1e-9000000000000001", 1e-200});
            t(-3, 3, new BigDecimalArgument<BigDecimal>[] {1, "2", 3, "-1", -2, "-3"});

            Assert.Pass();
        }
    }
}