using DecimalSharp.Core;
using NUnit.Framework;
using System;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightConstructor")]
    public class BigDecimalLightConstructorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 40,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15
            });

            var t = (int[] coefficient, int exponent, int sign, BigDecimalArgument<BigDecimalLight> n) =>
            {
                BigDecimalTests.assertEqualProps(coefficient, exponent, sign, bigDecimalFactory.BigDecimal(n));
            };

            t(new int[] {0}, 0, 0, 0);
            t(new int[] {0}, 0, 0, -0);
            t(new int[] {1}, 0, -1, -1);
            t(new int[] {10}, 0, -1, -10);

            t(new int[] {1}, 0, 1, 1);
            t(new int[] {10}, 0, 1, 10);
            t(new int[] {100}, 0, 1, 100);
            t(new int[] {1000}, 0, 1, 1000);
            t(new int[] {10000}, 0, 1, 10000);
            t(new int[] {100000}, 0, 1, 100000);
            t(new int[] {1000000}, 0, 1, 1000000);

            t(new int[] {1}, 1, 1, 10000000);
            t(new int[] {10}, 1, 1, 100000000);
            t(new int[] {100}, 1, 1, 1000000000);
            t(new int[] {1000}, 1, 1, 10000000000);
            t(new int[] {10000}, 1, 1, 100000000000);
            t(new int[] {100000}, 1, 1, 1000000000000);
            t(new int[] {1000000}, 1, 1, 10000000000000);

            t(new int[] {1}, 2, -1, -100000000000000);
            t(new int[] {10}, 2, -1, -1000000000000000);
            t(new int[] {100}, 2, -1, -10000000000000000);
            t(new int[] {1000}, 2, -1, -100000000000000000);
            t(new int[] {10000}, 2, -1, -1000000000000000000);
            //t(new int[] {100000}, 2, -1, -10000000000000000000);
            //t(new int[] {1000000}, 2, -1, -100000000000000000000);

            t(new int[] {1000000}, -1, 1, 1e-1);
            t(new int[] {100000}, -1, -1, -1e-2);
            t(new int[] {10000}, -1, 1, 1e-3);
            t(new int[] {1000}, -1, -1, -1e-4);
            t(new int[] {100}, -1, 1, 1e-5);
            t(new int[] {10}, -1, -1, -1e-6);
            t(new int[] {1}, -1, 1, 1e-7);

            t(new int[] {1000000}, -2, 1, 1e-8);
            t(new int[] {100000}, -2, -1, -1e-9);
            t(new int[] {10000}, -2, 1, 1e-10);
            t(new int[] {1000}, -2, -1, -1e-11);
            t(new int[] {100}, -2, 1, 1e-12);
            t(new int[] {10}, -2, -1, -1e-13);
            t(new int[] {1}, -2, 1, 1e-14);

            t(new int[] {1000000}, -3, 1, 1e-15);
            t(new int[] {100000}, -3, -1, -1e-16);
            t(new int[] {10000}, -3, 1, 1e-17);
            t(new int[] {1000}, -3, -1, -1e-18);
            t(new int[] {100}, -3, 1, 1e-19);
            t(new int[] {10}, -3, -1, -1e-20);
            t(new int[] {1}, -3, 1, 1e-21);

            t(new int[] {9}, 0, 1, "9");
            t(new int[] {99}, 0, -1, "-99");
            t(new int[] {999}, 0, 1, "999");
            t(new int[] {9999}, 0, -1, "-9999");
            t(new int[] {99999}, 0, 1, "99999");
            t(new int[] {999999}, 0, -1, "-999999");
            t(new int[] {9999999}, 0, 1, "9999999");

            t(new int[] {9, 9999999}, 1, -1, "-99999999");
            t(new int[] {99, 9999999}, 1, 1, "999999999");
            t(new int[] {999, 9999999}, 1, -1, "-9999999999");
            t(new int[] {9999, 9999999}, 1, 1, "99999999999");
            t(new int[] {99999, 9999999}, 1, -1, "-999999999999");
            t(new int[] {999999, 9999999}, 1, 1, "9999999999999");
            t(new int[] {9999999, 9999999}, 1, -1, "-99999999999999");

            t(new int[] {9, 9999999, 9999999}, 2, 1, "999999999999999");
            t(new int[] {99, 9999999, 9999999}, 2, -1, "-9999999999999999");
            t(new int[] {999, 9999999, 9999999}, 2, 1, "99999999999999999");
            t(new int[] {9999, 9999999, 9999999}, 2, -1, "-999999999999999999");
            t(new int[] {99999, 9999999, 9999999}, 2, 1, "9999999999999999999");
            t(new int[] {999999, 9999999, 9999999}, 2, -1, "-99999999999999999999");
            t(new int[] {9999999, 9999999, 9999999}, 2, 1, "999999999999999999999");

            // Test base conversion.

            var t2 = (string expected, BigDecimalArgument<BigDecimalLight> n) => {
                BigDecimalTests.assertEqual(expected, bigDecimalFactory.BigDecimal(n).ValueOf());
            };

            var tx = (Action fn, string msg) => {
                BigDecimalTests.assertException(fn, msg);
            };

            tx(() => { bigDecimalFactory.BigDecimal(double.NaN); }, "NaN");
            tx(() => { bigDecimalFactory.BigDecimal("NaN"); }, "'NaN'");
            tx(() => { bigDecimalFactory.BigDecimal("-NaN"); }, "'-NaN'");
            tx(() => { bigDecimalFactory.BigDecimal(" NaN"); }, "' NaN'");
            tx(() => { bigDecimalFactory.BigDecimal("NaN "); }, "'NaN '");
            tx(() => { bigDecimalFactory.BigDecimal(" NaN "); }, "' NaN '");
            tx(() => { bigDecimalFactory.BigDecimal("+NaN"); }, "'+NaN'");
            tx(() => { bigDecimalFactory.BigDecimal(" +NaN"); }, "' +NaN'");
            tx(() => { bigDecimalFactory.BigDecimal(".NaN"); }, "'.NaN'");
            tx(() => { bigDecimalFactory.BigDecimal("NaN."); }, "'NaN.'");

            tx(() => { bigDecimalFactory.BigDecimal(double.PositiveInfinity); }, "Infinity");
            tx(() => { bigDecimalFactory.BigDecimal(double.NegativeInfinity); }, "-Infinity");
            tx(() => { bigDecimalFactory.BigDecimal("Infinity"); }, "'Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal("-Infinity"); }, "'-Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal(" Infinity"); }, "' Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal("Infinity "); }, "'Infinity '");
            tx(() => { bigDecimalFactory.BigDecimal(" Infinity "); }, "' Infinity '");
            tx(() => { bigDecimalFactory.BigDecimal("+Infinity"); }, "'+Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal(" +Infinity"); }, "' +Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal(".Infinity"); }, "'.Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal("Infinity."); }, "'Infinity.'");

            t2("0", 0);
            t2("0", "-0");
            t2("0", "0");
            t2("0", "-0");
            t2("0", "0.");
            t2("0", "-0.");
            t2("0", "0.0");
            t2("0", "-0.0");
            t2("0", "0.00000000");
            t2("0", "-0.0000000000000000000000");

            tx(() => { bigDecimalFactory.BigDecimal(" 0"); }, "' 0'");
            tx(() => { bigDecimalFactory.BigDecimal("0 "); }, "'0 '");
            tx(() => { bigDecimalFactory.BigDecimal(" 0 "); }, "' 0 '");
            tx(() => { bigDecimalFactory.BigDecimal("0-"); }, "'0-'");
            tx(() => { bigDecimalFactory.BigDecimal(" -0"); }, "' -0'");
            tx(() => { bigDecimalFactory.BigDecimal("-0 "); }, "'-0 '");
            tx(() => { bigDecimalFactory.BigDecimal("+0"); }, "'+0'");
            tx(() => { bigDecimalFactory.BigDecimal(" +0"); }, "' +0'");
            tx(() => { bigDecimalFactory.BigDecimal(" .0"); }, "' .0'");
            tx(() => { bigDecimalFactory.BigDecimal("0. "); }, "'0. '");
            tx(() => { bigDecimalFactory.BigDecimal("+-0"); }, "'+-0'");
            tx(() => { bigDecimalFactory.BigDecimal("-+0"); }, "'-+0'");
            tx(() => { bigDecimalFactory.BigDecimal("--0"); }, "'--0'");
            tx(() => { bigDecimalFactory.BigDecimal("++0"); }, "'++0'");
            tx(() => { bigDecimalFactory.BigDecimal(".-0"); }, "'.-0'");
            tx(() => { bigDecimalFactory.BigDecimal(".+0"); }, "'.+0'");
            tx(() => { bigDecimalFactory.BigDecimal("0 ."); }, "'0 .'");
            tx(() => { bigDecimalFactory.BigDecimal(". 0"); }, "'. 0'");
            tx(() => { bigDecimalFactory.BigDecimal("..0"); }, "'..0'");
            tx(() => { bigDecimalFactory.BigDecimal("+.-0"); }, "'+.-0'");
            tx(() => { bigDecimalFactory.BigDecimal("-.+0"); }, "'-.+0'");
            tx(() => { bigDecimalFactory.BigDecimal("+. 0"); }, "'+. 0'");
            tx(() => { bigDecimalFactory.BigDecimal(".0."); }, "'.0.'");

            t2("1", 1);
            t2("-1", -1);
            t2("1", "1");
            t2("-1", "-1");
            t2("0.1", ".1");
            t2("0.1", ".1");
            t2("-0.1", "-.1");
            t2("1", "1.");
            t2("1", "1.0");
            t2("-1", "-1.");
            t2("-1", "-1.0000");
            t2("1", "1.00000000");
            t2("-1", "-1.000000000000000000000000");

            tx(() => { bigDecimalFactory.BigDecimal(" 1"); }, "' 1'");
            tx(() => { bigDecimalFactory.BigDecimal("1 "); }, "'1 '");
            tx(() => { bigDecimalFactory.BigDecimal(" 1 "); }, "' 1 '");
            tx(() => { bigDecimalFactory.BigDecimal("1-"); }, "'1-'");
            tx(() => { bigDecimalFactory.BigDecimal(" -1"); }, "' -1'");
            tx(() => { bigDecimalFactory.BigDecimal("-1 "); }, "'-1 '");
            tx(() => { bigDecimalFactory.BigDecimal("+1"); }, "'+1'");
            tx(() => { bigDecimalFactory.BigDecimal(" +1"); }, "' +1'");
            tx(() => { bigDecimalFactory.BigDecimal(".1."); }, "'.1.'");
            tx(() => { bigDecimalFactory.BigDecimal("+-1"); }, "'+-1'");
            tx(() => { bigDecimalFactory.BigDecimal("-+1"); }, "'-+1'");
            tx(() => { bigDecimalFactory.BigDecimal("--1"); }, "'--1'");
            tx(() => { bigDecimalFactory.BigDecimal("++1"); }, "'++1'");
            tx(() => { bigDecimalFactory.BigDecimal(".-1"); }, "'.-1'");
            tx(() => { bigDecimalFactory.BigDecimal(".+1"); }, "'.+1'");
            tx(() => { bigDecimalFactory.BigDecimal("1 ."); }, "'1 .'");
            tx(() => { bigDecimalFactory.BigDecimal(". 1"); }, "'. 1'");
            tx(() => { bigDecimalFactory.BigDecimal("..1"); }, "'..1'");
            tx(() => { bigDecimalFactory.BigDecimal("+.-1"); }, "'+.-1'");
            tx(() => { bigDecimalFactory.BigDecimal("-.+1"); }, "'-.+1'");
            tx(() => { bigDecimalFactory.BigDecimal("+. 1"); }, "'+. 1'");
            tx(() => { bigDecimalFactory.BigDecimal("-. 1"); }, "'-. 1'");
            tx(() => { bigDecimalFactory.BigDecimal("1.."); }, "'1..'");
            tx(() => { bigDecimalFactory.BigDecimal("+1.."); }, "'+1..'");
            tx(() => { bigDecimalFactory.BigDecimal("-1.."); }, "'-1..'");
            tx(() => { bigDecimalFactory.BigDecimal("-.1."); }, "'-.1.'");
            tx(() => { bigDecimalFactory.BigDecimal("+.1."); }, "'+.1.'");
            tx(() => { bigDecimalFactory.BigDecimal(".-10."); }, "'.-10.'");
            tx(() => { bigDecimalFactory.BigDecimal(".+10."); }, "'.+10.'");
            tx(() => { bigDecimalFactory.BigDecimal(". 10."); }, "'. 10.'");

            t2("123.456789", 123.456789);
            t2("-123.456789", -123.456789);
            t2("123.456789", "123.456789");
            t2("-123.456789", "-123.456789");

            tx(() => { bigDecimalFactory.BigDecimal((string?)null); }, "null");
            tx(() => { bigDecimalFactory.BigDecimal((BigDecimalLight?)null); }, "null");
            tx(() => { bigDecimalFactory.BigDecimal("null"); }, "'null'");
            tx(() => { bigDecimalFactory.BigDecimal(""); }, "''");
            tx(() => { bigDecimalFactory.BigDecimal(" "); }, "' '");
            tx(() => { bigDecimalFactory.BigDecimal("nan"); }, "'nan'");
            tx(() => { bigDecimalFactory.BigDecimal("23e"); }, "'23e'");
            tx(() => { bigDecimalFactory.BigDecimal("e4"); }, "'e4'");
            tx(() => { bigDecimalFactory.BigDecimal("ff"); }, "'ff'");
            tx(() => { bigDecimalFactory.BigDecimal("0xg"); }, "'oxg'");
            tx(() => { bigDecimalFactory.BigDecimal("0Xfi"); }, "'0Xfi'");
            tx(() => { bigDecimalFactory.BigDecimal("--45"); }, "'--45'");
            tx(() => { bigDecimalFactory.BigDecimal("9.99--"); }, "'9.99--'");
            tx(() => { bigDecimalFactory.BigDecimal("0 0"); }, "'0 0'");

            Assert.Pass();
        }
    }
}