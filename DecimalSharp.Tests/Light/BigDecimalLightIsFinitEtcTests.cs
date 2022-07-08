using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightIsFinitEtc")]
    public class BigDecimalLightIsFinitEtcTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsFinitEtc()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 20,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = -7,
                ToExpPos = 21
            });

            var t = (bool actual) =>
            {
                BigDecimalTests.assert(actual);
            };

            var n = bigDecimalFactory.BigDecimal(1);

            t(!n.IsNegative());
            t(!n.IsZero());
            t(n.IsInteger());
            t(n.Equals(n));
            t(n.Equals(1));
            t(n.Equals("1.0"));
            t(n.Equals("1.00"));
            t(n.Equals("1.000"));
            t(n.Equals("1.0000"));
            t(n.Equals("1.00000"));
            t(n.Equals("1.000000"));
            t(n.Equals(bigDecimalFactory.BigDecimal(1)));
            t(n.GreaterThan(0.99999));
            t(!n.GreaterThanOrEqualTo(1.1));
            t(n.LessThan(1.001));
            t(n.LessThanOrEqualTo(2));
            t(n.ToString() == n.ValueOf());

            n = bigDecimalFactory.BigDecimal("-0.1");

            t(n.IsNeg());
            t(!n.IsZero());
            t(!n.IsInt());
            t(!n.Equals(0.1));
            t(!n.GreaterThan(-0.1));
            t(n.GreaterThanOrEqualTo(-1));
            t(n.LessThan(-0.01));
            t(!n.LessThanOrEqualTo(-1));
            t(n.ToString() == n.ValueOf());

            n = bigDecimalFactory.BigDecimal("0.0000000");

            t(!n.IsNegative());
            t(n.IsZero());
            t(n.IsInteger());
            t(n.Eq("-0"));
            t(n.Gt(-0.000001));
            t(!n.Gte(0.1));
            t(n.Lt(0.0001));
            t(n.Lte("-0"));
            t(n.ToString() == n.ValueOf());

            n = bigDecimalFactory.BigDecimal("-0");

            t(!n.IsNeg());
            t(n.IsZero());
            t(n.IsInt());
            t(n.Equals("0.000"));
            t(n.GreaterThan(-1));
            t(!n.GreaterThanOrEqualTo(0.1));
            t(!n.LessThan(0));
            t(n.LessThan(0.1));
            t(n.LessThanOrEqualTo(0));
            t(n.ValueOf() == "0" && n.ToString() == "0");

            n = bigDecimalFactory.BigDecimal("-1.234e+2");

            t(n.IsNeg());
            t(!n.IsZero());
            t(!n.IsInt());
            t(n.Eq(-123.4));
            t(n.Gte("-1.234e+3"));
            t(n.Lt(-123.39999));
            t(n.Lte("-123.4e+0"));
            t(n.ToString() == n.ValueOf());

            n = bigDecimalFactory.BigDecimal("5e-200");

            t(!n.IsNegative());
            t(!n.IsZero());
            t(!n.IsInteger());
            t(n.Equals(5e-200));
            t(n.GreaterThan(5e-201));
            t(!n.GreaterThanOrEqualTo(1));
            t(n.LessThan(6e-200));
            t(n.LessThanOrEqualTo(5.1e-200));
            t(n.ToString() == n.ValueOf());

            n = bigDecimalFactory.BigDecimal("1");

            t(n.Equals(n));
            t(n.Equals(n.ToString()));
            t(n.Equals(n.ToString()));
            t(n.Equals(n.ValueOf()));
            t(n.Equals(n.ToFixed()));
            t(n.Equals(1));
            t(n.Equals("1e+0"));
            t(!n.Equals(-1));
            t(!n.Equals(0.1));

            t(!bigDecimalFactory.BigDecimal(0.1).Equals(0));
            t(!bigDecimalFactory.BigDecimal(1e9 + 1).Equals(1e9));
            t(!bigDecimalFactory.BigDecimal(1e9 - 1).Equals(1e9));
            t(bigDecimalFactory.BigDecimal(1e9 + 1).Equals(1e9 + 1));
            t(bigDecimalFactory.BigDecimal(1).Equals(1));
            t(!bigDecimalFactory.BigDecimal(1).Equals(-1));

            t(!bigDecimalFactory.BigDecimal(10).GreaterThan(10));
            t(!bigDecimalFactory.BigDecimal(10).LessThan(10));
            t(!bigDecimalFactory.BigDecimal(2).GreaterThan(10));
            t(!bigDecimalFactory.BigDecimal(10).LessThan(2));
            t(bigDecimalFactory.BigDecimal(10).LessThanOrEqualTo(20));
            t(!bigDecimalFactory.BigDecimal(10).GreaterThanOrEqualTo(20));

            t(!bigDecimalFactory.BigDecimal(1.23001e-2).LessThan(1.23e-2));
            t(bigDecimalFactory.BigDecimal(1.23e-2).Lt(1.23001e-2));
            t(!bigDecimalFactory.BigDecimal(1e-2).LessThan(9.999999e-3));
            t(bigDecimalFactory.BigDecimal(9.999999e-3).Lt(1e-2));

            t(!bigDecimalFactory.BigDecimal(1.23001e+2).LessThan(1.23e+2));
            t(bigDecimalFactory.BigDecimal(1.23e+2).Lt(1.23001e+2));
            t(bigDecimalFactory.BigDecimal(9.999999e+2).LessThan(1e+3));
            t(!bigDecimalFactory.BigDecimal(1e+3).Lt(9.9999999e+2));

            t(!bigDecimalFactory.BigDecimal(1.23001e-2).LessThanOrEqualTo(1.23e-2));
            t(bigDecimalFactory.BigDecimal(1.23e-2).Lte(1.23001e-2));
            t(!bigDecimalFactory.BigDecimal(1e-2).LessThanOrEqualTo(9.999999e-3));
            t(bigDecimalFactory.BigDecimal(9.999999e-3).Lte(1e-2));

            t(!bigDecimalFactory.BigDecimal(1.23001e+2).LessThanOrEqualTo(1.23e+2));
            t(bigDecimalFactory.BigDecimal(1.23e+2).Lte(1.23001e+2));
            t(bigDecimalFactory.BigDecimal(9.999999e+2).LessThanOrEqualTo(1e+3));
            t(!bigDecimalFactory.BigDecimal(1e+3).Lte(9.9999999e+2));

            t(bigDecimalFactory.BigDecimal(1.23001e-2).GreaterThan(1.23e-2));
            t(!bigDecimalFactory.BigDecimal(1.23e-2).Gt(1.23001e-2));
            t(bigDecimalFactory.BigDecimal(1e-2).GreaterThan(9.999999e-3));
            t(!bigDecimalFactory.BigDecimal(9.999999e-3).Gt(1e-2));

            t(bigDecimalFactory.BigDecimal(1.23001e+2).GreaterThan(1.23e+2));
            t(!bigDecimalFactory.BigDecimal(1.23e+2).Gt(1.23001e+2));
            t(!bigDecimalFactory.BigDecimal(9.999999e+2).GreaterThan(1e+3));
            t(bigDecimalFactory.BigDecimal(1e+3).Gt(9.9999999e+2));

            t(bigDecimalFactory.BigDecimal(1.23001e-2).GreaterThanOrEqualTo(1.23e-2));
            t(!bigDecimalFactory.BigDecimal(1.23e-2).Gte(1.23001e-2));
            t(bigDecimalFactory.BigDecimal(1e-2).GreaterThanOrEqualTo(9.999999e-3));
            t(!bigDecimalFactory.BigDecimal(9.999999e-3).Gte(1e-2));

            t(bigDecimalFactory.BigDecimal(1.23001e+2).GreaterThanOrEqualTo(1.23e+2));
            t(!bigDecimalFactory.BigDecimal(1.23e+2).Gte(1.23001e+2));
            t(!bigDecimalFactory.BigDecimal(9.999999e+2).GreaterThanOrEqualTo(1e+3));
            t(bigDecimalFactory.BigDecimal(1e+3).Gte(9.9999999e+2));

            t(!bigDecimalFactory.BigDecimal("1.0000000000000000000001").IsInteger());
            t(!bigDecimalFactory.BigDecimal("0.999999999999999999999").IsInteger());
            t(bigDecimalFactory.BigDecimal("4e4").IsInteger());
            t(bigDecimalFactory.BigDecimal("-4e4").IsInteger());

            Assert.Pass();
        }
    }
}