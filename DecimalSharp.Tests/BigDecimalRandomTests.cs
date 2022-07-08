using NUnit.Framework;
using System;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalRandom")]
    public class BigDecimalRandomTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Random()
        {
            var bigDecimalFactory = new BigDecimalFactory();

            Random random = new Random();

            var maxDigits = 100;

            for (var i = 0; i < 996; i++)
            {
                var sd = (long)(random.NextDouble() * maxDigits + 1) | 0;

                BigDecimal r;
                if (random.NextDouble() > 0.5)
                {
                    bigDecimalFactory.Config.Precision = sd;
                    r = bigDecimalFactory.Random();
                }
                else
                {
                    r = bigDecimalFactory.Random(sd);
                }

                BigDecimalTests.assert(r.Sd() <= sd && r.Gte(0) && r.Lt(1) && r.Eq(r) && r.Eq(r.ValueOf()));
            }

            Assert.Pass();
        }
    }
}