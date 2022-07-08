using DecimalSharp.Tests.Extensions;
using NUnit.Framework;
using System;
using System.Globalization;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightPowSqrt")]
    public class BigDecimalLightPowSqrtTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PowSqrt()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                ToExpNeg = -7,
                ToExpPos = 21
            });

            Random random = new Random();

            string n;
            for (var i = 0; i < 1000; i++)
            {
                
                // Get a random value in the range [0,1) with a random number of significant digits
                // in the range [1, 40], as a string in exponential format.
                var e = random.NextDouble().ToExponential();

                // Change exponent to a non-zero value of random length in the range (-9e15, 9e15).
                var r = bigDecimalFactory.BigDecimal(e.Slice(0, e.IndexOf('e') + 1) + (random.NextDouble() < 0.5 ? '-' : "") +
                  (n = Math.Floor(random.NextDouble() * 9e15).ToString(CultureInfo.InvariantCulture)).Substring((int)(random.NextDouble() * n.Length) | 0));
                //console.log(' r:          ' + r);

                // Random precision in the range [1, 40].
                bigDecimalFactory.Config.Precision = (long)(random.NextDouble() * 40 + 1) | 0;

                var p = r.Pow(0.5);
                //console.log(' r.pow(0.5): ' + p);

                // sqrt is much faster than pow(0.5)
                var s = r.Sqrt();
                //console.log(' r.sqrt():   ' + s);

                BigDecimalTests.assertEqual(p.ValueOf(), s.ValueOf());
            }

            Assert.Pass();
        }
    }
}