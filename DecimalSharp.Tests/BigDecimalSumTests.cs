using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalSum")]
    public class BigDecimalSumTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Sum()
        {
            var bigDecimalFactory = new BigDecimalFactory();

            BigDecimal expected = null;

            var t = (BigDecimalArgument<BigDecimal>[] arr) =>
            {
                if (expected == null)
                    Assert.Fail();

                BigDecimalTests.assertEqualDecimal(expected, bigDecimalFactory.Sum(arr));
            };

            expected = bigDecimalFactory.BigDecimal(0);

            t(new BigDecimalArgument<BigDecimal>[] {"0"});
            t(new BigDecimalArgument<BigDecimal>[] {"0", bigDecimalFactory.BigDecimal(0)});
            t(new BigDecimalArgument<BigDecimal>[] {1, 0, "-1"});
            t(new BigDecimalArgument<BigDecimal>[] {0, bigDecimalFactory.BigDecimal("-10"), 0, 0, 0, 0, 0, 10});
            t(new BigDecimalArgument<BigDecimal>[] {11, -11});
            t(new BigDecimalArgument<BigDecimal>[] {1, "2", bigDecimalFactory.BigDecimal(3), bigDecimalFactory.BigDecimal("4"), -10});
            t(new BigDecimalArgument<BigDecimal>[] {bigDecimalFactory.BigDecimal(-10), "9", bigDecimalFactory.BigDecimal(0.01), 0.99});

            expected = bigDecimalFactory.BigDecimal(10);

            t(new BigDecimalArgument<BigDecimal>[] {"10"});
            t(new BigDecimalArgument<BigDecimal>[] {"0", bigDecimalFactory.BigDecimal("10")});
            t(new BigDecimalArgument<BigDecimal>[] {10, 0});
            t(new BigDecimalArgument<BigDecimal>[] {0, 0, 0, 0, 0, 0, 10});
            t(new BigDecimalArgument<BigDecimal>[] {11, -1});
            t(new BigDecimalArgument<BigDecimal>[] {1, "2", bigDecimalFactory.BigDecimal(3), bigDecimalFactory.BigDecimal("4")});
            t(new BigDecimalArgument<BigDecimal>[] {"9", bigDecimalFactory.BigDecimal(0.01), 0.99});

            expected = bigDecimalFactory.BigDecimal(600);

            t(new BigDecimalArgument<BigDecimal>[] {100, 200, 300});
            t(new BigDecimalArgument<BigDecimal>[] {"100", "200", "300"});
            t(new BigDecimalArgument<BigDecimal>[] {bigDecimalFactory.BigDecimal(100), bigDecimalFactory.BigDecimal(200), bigDecimalFactory.BigDecimal(300)});
            t(new BigDecimalArgument<BigDecimal>[] {100, "200", bigDecimalFactory.BigDecimal(300)});
            t(new BigDecimalArgument<BigDecimal>[] {99.9, 200.05, 300.05});

            expected = bigDecimalFactory.BigDecimal(double.NaN);

            t(new BigDecimalArgument<BigDecimal>[] {double.NaN});
            t(new BigDecimalArgument<BigDecimal>[] {"1", double.NaN});
            t(new BigDecimalArgument<BigDecimal>[] {100, 200, double.NaN});
            t(new BigDecimalArgument<BigDecimal>[] {double.NaN, 0, "9", bigDecimalFactory.BigDecimal(0), 11, double.PositiveInfinity});
            t(new BigDecimalArgument<BigDecimal>[] {0, bigDecimalFactory.BigDecimal("-Infinity"), "9", bigDecimalFactory.BigDecimal(double.NaN), 11});
            t(new BigDecimalArgument<BigDecimal>[] {4, "-Infinity", 0, "9", bigDecimalFactory.BigDecimal(0), double.PositiveInfinity, 2});

            expected = bigDecimalFactory.BigDecimal(double.PositiveInfinity);

            t(new BigDecimalArgument<BigDecimal>[] {double.PositiveInfinity});
            t(new BigDecimalArgument<BigDecimal>[] {1, "1e10000000000000000000000000000000000000000", "4"});
            t(new BigDecimalArgument<BigDecimal>[] {100, 200, "Infinity"});
            t(new BigDecimalArgument<BigDecimal>[] {0, bigDecimalFactory.BigDecimal("Infinity"), "9", bigDecimalFactory.BigDecimal(0), 11});
            t(new BigDecimalArgument<BigDecimal>[] {0, "9", bigDecimalFactory.BigDecimal(0), 11, double.PositiveInfinity});
            t(new BigDecimalArgument<BigDecimal>[] {4, bigDecimalFactory.BigDecimal(double.PositiveInfinity), 0, "9", bigDecimalFactory.BigDecimal(0), double.PositiveInfinity, 2});

            expected = bigDecimalFactory.BigDecimal(double.NegativeInfinity);

            t(new BigDecimalArgument<BigDecimal>[] {double.NegativeInfinity});
            t(new BigDecimalArgument<BigDecimal>[] {1, "-1e10000000000000000000000000000000000000000", "4"});
            t(new BigDecimalArgument<BigDecimal>[] {100, 200, "-Infinity"});
            t(new BigDecimalArgument<BigDecimal>[] {0, bigDecimalFactory.BigDecimal("-Infinity"), "9", bigDecimalFactory.BigDecimal(0), 11});
            t(new BigDecimalArgument<BigDecimal>[] {0, "9", bigDecimalFactory.BigDecimal(0), 11, double.NegativeInfinity});
            t(new BigDecimalArgument<BigDecimal>[] {4, bigDecimalFactory.BigDecimal(double.NegativeInfinity), 0, "9", bigDecimalFactory.BigDecimal(0), double.NegativeInfinity, 2});

            Assert.Pass();
        }
    }
}