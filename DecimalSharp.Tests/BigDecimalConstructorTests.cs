using DecimalSharp.Core;
using DecimalSharp.Tests.Extensions;
using NUnit.Framework;
using System;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalConstructor")]
    public class BigDecimalConstructorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor()
        {
            var bigDecimalFactory = new BigDecimalFactory(new BigDecimalConfig()
            {
                Precision = 40,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15,
                MaxE = (long)9e15,
                MinE = (long)-9e15,
                Crypto = false,
                Modulo = 1
            });

            var t = (int[] coefficient, int exponent, int sign, BigDecimalArgument<BigDecimal> n) =>
            {
                BigDecimalTests.assertEqualProps(coefficient, exponent, sign, bigDecimalFactory.BigDecimal(n));
            };

            t(new int[] {0}, 0, 1, 0);
            t(new int[] {0}, 0, -1, "-0");
            t(new int[] {1}, 0, -1, -1);
            t(new int[] {10}, 1, -1, -10);

            t(new int[] {1}, 0, 1, 1);
            t(new int[] {10}, 1, 1, 10);
            t(new int[] {100}, 2, 1, 100);
            t(new int[] {1000}, 3, 1, 1000);
            t(new int[] {10000}, 4, 1, 10000);
            t(new int[] {100000}, 5, 1, 100000);
            t(new int[] {1000000}, 6, 1, 1000000);

            t(new int[] {1}, 7, 1, 10000000);
            t(new int[] {10}, 8, 1, 100000000);
            t(new int[] {100}, 9, 1, 1000000000);
            t(new int[] {1000}, 10, 1, 10000000000);
            t(new int[] {10000}, 11, 1, 100000000000);
            t(new int[] {100000}, 12, 1, 1000000000000);
            t(new int[] {1000000}, 13, 1, 10000000000000);

            t(new int[] {1}, 14, -1, -100000000000000);
            t(new int[] {10}, 15, -1, -1000000000000000);
            t(new int[] {100}, 16, -1, -10000000000000000);
            t(new int[] {1000}, 17, -1, -100000000000000000);
            t(new int[] {10000}, 18, -1, -1000000000000000000);
            //t(new int[] {100000}, 19, -1, -10000000000000000000);
            //t(new int[] {1000000}, 20, -1, -100000000000000000000);

            t(new int[] {1000000}, -1, 1, 1e-1);
            t(new int[] {100000}, -2, -1, -1e-2);
            t(new int[] {10000}, -3, 1, 1e-3);
            t(new int[] {1000}, -4, -1, -1e-4);
            t(new int[] {100}, -5, 1, 1e-5);
            t(new int[] {10}, -6, -1, -1e-6);
            t(new int[] {1}, -7, 1, 1e-7);

            t(new int[] {1000000}, -8, 1, 1e-8);
            t(new int[] {100000}, -9, -1, -1e-9);
            t(new int[] {10000}, -10, 1, 1e-10);
            t(new int[] {1000}, -11, -1, -1e-11);
            t(new int[] {100}, -12, 1, 1e-12);
            t(new int[] {10}, -13, -1, -1e-13);
            t(new int[] {1}, -14, 1, 1e-14);

            t(new int[] {1000000}, -15, 1, 1e-15);
            t(new int[] {100000}, -16, -1, -1e-16);
            t(new int[] {10000}, -17, 1, 1e-17);
            t(new int[] {1000}, -18, -1, -1e-18);
            t(new int[] {100}, -19, 1, 1e-19);
            t(new int[] {10}, -20, -1, -1e-20);
            t(new int[] {1}, -21, 1, 1e-21);

            t(new int[] {9}, 0, 1, "9");
            t(new int[] {99}, 1, -1, "-99");
            t(new int[] {999}, 2, 1, "999");
            t(new int[] {9999}, 3, -1, "-9999");
            t(new int[] {99999}, 4, 1, "99999");
            t(new int[] {999999}, 5, -1, "-999999");
            t(new int[] {9999999}, 6, 1, "9999999");

            t(new int[] {9, 9999999}, 7, -1, "-99999999");
            t(new int[] {99, 9999999}, 8, 1, "999999999");
            t(new int[] {999, 9999999}, 9, -1, "-9999999999");
            t(new int[] {9999, 9999999}, 10, 1, "99999999999");
            t(new int[] {99999, 9999999}, 11, -1, "-999999999999");
            t(new int[] {999999, 9999999}, 12, 1, "9999999999999");
            t(new int[] {9999999, 9999999}, 13, -1, "-99999999999999");

            t(new int[] {9, 9999999, 9999999}, 14, 1, "999999999999999");
            t(new int[] {99, 9999999, 9999999}, 15, -1, "-9999999999999999");
            t(new int[] {999, 9999999, 9999999}, 16, 1, "99999999999999999");
            t(new int[] {9999, 9999999, 9999999}, 17, -1, "-999999999999999999");
            t(new int[] {99999, 9999999, 9999999}, 18, 1, "9999999999999999999");
            t(new int[] {999999, 9999999, 9999999}, 19, -1, "-99999999999999999999");
            t(new int[] {9999999, 9999999, 9999999}, 20, 1, "999999999999999999999");

            // Test base conversion.

            var t2 = (string expected, BigDecimalArgument<BigDecimal> n) => {
                BigDecimalTests.assertEqual(expected, bigDecimalFactory.BigDecimal(n).ValueOf());
            };

            long randInt()
            {
                Random random = new Random();
                return (long)Math.Floor(random.NextDouble() * 0x20000000000000 / Math.Pow(10, (long)(random.NextDouble() * 16) | 0));
            }

            // Test random integers against Number.prototype.toString(base).
            for (long k, i = 0; i < 127; i++)
            {
                k = randInt();
                t2(k.ToString(), "0b" + k.ToString(2));
                k = randInt();
                t2(k.ToString(), "0B" + k.ToString(2));
                k = randInt();
                t2(k.ToString(), "0o" + k.ToString(8));
                k = randInt();
                t2(k.ToString(), "0O" + k.ToString(8));
                k = randInt();
                t2(k.ToString(), "0x" + k.ToString(16));
                k = randInt();
                t2(k.ToString(), "0X" + k.ToString(16));
            }

            // Binary.
            t2("0", "0b0");
            t2("0", "0B0");
            t2("-5", "-0b101");
            t2("5", "+0b101");
            t2("1.5", "0b1.1");
            t2("-1.5", "-0b1.1");

            t2("18181", "0b100011100000101.00");
            t2("-12.5", "-0b1100.10");
            t2("343872.5", "0b1010011111101000000.10");
            t2("-328.28125", "-0b101001000.010010");
            t2("-341919.144535064697265625", "-0b1010011011110011111.0010010100000000010");
            t2("97.10482025146484375", "0b1100001.000110101101010110000");
            t2("-120914.40625", "-0b11101100001010010.01101");
            t2("8080777260861123367657", "0b1101101100000111101001111111010001111010111011001010100101001001011101001");
            
            // Octal.
            t2("8", "0o10");
            t2("-8.5", "-0O010.4");
            t2("8.5", "+0O010.4");
            t2("-262144.000000059604644775390625", "-0o1000000.00000001");
            t2("572315667420.390625", "0o10250053005734.31");

            // Hex.
            t2("1", "0x00001");
            t2("255", "0xff");
            t2("-15.5", "-0Xf.8");
            t2("15.5", "+0Xf.8");
            t2("-16777216.00000000023283064365386962890625", "-0x1000000.00000001");
            t2("325927753012307620476767402981591827744994693483231017778102969592507", "0xc16de7aa5bf90c3755ef4dea45e982b351b6e00cd25a82dcfe0646abb");
            
            // Test parsing.

            var tx = (Action fn, string msg) => {
                BigDecimalTests.assertException(fn, msg);
            };

            t2("NaN", double.NaN);
            t2("NaN", -double.NaN);
            t2("NaN", "NaN");
            t2("NaN", "-NaN");
            t2("NaN", "+NaN");

            tx(() => { bigDecimalFactory.BigDecimal(" NaN"); }, "' NaN'");
            tx(() => { bigDecimalFactory.BigDecimal("NaN "); }, "'NaN '");
            tx(() => { bigDecimalFactory.BigDecimal(" NaN "); }, "' NaN '");
            tx(() => { bigDecimalFactory.BigDecimal(" -NaN"); }, "' -NaN'");
            tx(() => { bigDecimalFactory.BigDecimal(" +NaN"); }, "' +NaN'");
            tx(() => { bigDecimalFactory.BigDecimal("-NaN "); }, "'-NaN '");
            tx(() => { bigDecimalFactory.BigDecimal("+NaN "); }, "'+NaN '");
            tx(() => { bigDecimalFactory.BigDecimal(".NaN"); }, "'.NaN'");
            tx(() => { bigDecimalFactory.BigDecimal("NaN."); }, "'NaN.'");

            t2("Infinity", double.PositiveInfinity);
            t2("-Infinity", double.NegativeInfinity);
            t2("Infinity", "Infinity");
            t2("-Infinity", "-Infinity");
            t2("Infinity", "+Infinity");

            tx(() => { bigDecimalFactory.BigDecimal(" Infinity"); }, "' Infinity '");
            tx(() => { bigDecimalFactory.BigDecimal("Infinity "); }, "'Infinity '");
            tx(() => { bigDecimalFactory.BigDecimal(" Infinity "); }, "' Infinity '");
            tx(() => { bigDecimalFactory.BigDecimal(" -Infinity"); }, "' -Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal(" +Infinity"); }, "' +Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal(".Infinity"); }, "'.Infinity'");
            tx(() => { bigDecimalFactory.BigDecimal("Infinity."); }, "'Infinity.'");

            t2("0", 0);
            t2("0", "0");
            t2("-0", "-0");
            t2("0", "0.");
            t2("-0", "-0.");
            t2("0", "0.0");
            t2("-0", "-0.0");
            t2("0", "0.00000000");
            t2("-0", "-0.0000000000000000000000");

            tx(() => { bigDecimalFactory.BigDecimal(" 0"); }, "' 0'");
            tx(() => { bigDecimalFactory.BigDecimal("0 "); }, "'0 '");
            tx(() => { bigDecimalFactory.BigDecimal(" 0 "); }, "' 0 '");
            tx(() => { bigDecimalFactory.BigDecimal("0-"); }, "'0-'");
            tx(() => { bigDecimalFactory.BigDecimal(" -0"); }, "' -0'");
            tx(() => { bigDecimalFactory.BigDecimal("-0 "); }, "'-0 '");
            tx(() => { bigDecimalFactory.BigDecimal("+0 "); }, "'+0 '");
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
            t2("0.1", "+.1");
            t2("1", "1.");
            t2("1", "1.0");
            t2("-1", "-1.");
            t2("1", "+1.");
            t2("-1", "-1.0000");
            t2("1", "1.0000");
            t2("1", "1.00000000");
            t2("-1", "-1.000000000000000000000000");
            t2("1", "+1.000000000000000000000000");

            tx(() => { bigDecimalFactory.BigDecimal(" 1"); }, "' 1'");
            tx(() => { bigDecimalFactory.BigDecimal("1 "); }, "'1 '");
            tx(() => { bigDecimalFactory.BigDecimal(" 1 "); }, "' 1 '");
            tx(() => { bigDecimalFactory.BigDecimal("1-"); }, "'1-'");
            tx(() => { bigDecimalFactory.BigDecimal(" -1"); }, "' -1'");
            tx(() => { bigDecimalFactory.BigDecimal("-1 "); }, "'-1 '");
            tx(() => { bigDecimalFactory.BigDecimal(" +1"); }, "' +1'");
            tx(() => { bigDecimalFactory.BigDecimal("+1 "); }, "'+1'");
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
            t2("-123.456789", "-123.456789");
            t2("123.456789", "123.456789");
            t2("123.456789", "+123.456789");

            tx(() => { bigDecimalFactory.BigDecimal((string)null); }, "null");
            tx(() => { bigDecimalFactory.BigDecimal("undefined"); }, "'undefined'");
            tx(() => { bigDecimalFactory.BigDecimal((BigDecimal)null); }, "null");
            tx(() => { bigDecimalFactory.BigDecimal("null"); }, "'null'");
            tx(() => { bigDecimalFactory.BigDecimal(""); }, "''");
            tx(() => { bigDecimalFactory.BigDecimal(" "); }, "' '");
            tx(() => { bigDecimalFactory.BigDecimal("nan"); }, "'nan'");
            tx(() => { bigDecimalFactory.BigDecimal("23e"); }, "'23e'");
            tx(() => { bigDecimalFactory.BigDecimal("e4"); }, "'e4'");
            tx(() => { bigDecimalFactory.BigDecimal("ff"); }, "'ff'");
            tx(() => { bigDecimalFactory.BigDecimal("0xg"); }, "'oxg'");
            tx(() => { bigDecimalFactory.BigDecimal("0Xfi"); }, "'0Xfi'");
            tx(() => { bigDecimalFactory.BigDecimal("++45"); }, "'++45'");
            tx(() => { bigDecimalFactory.BigDecimal("--45"); }, "'--45'");
            tx(() => { bigDecimalFactory.BigDecimal("9.99--"); }, "'9.99--'");
            tx(() => { bigDecimalFactory.BigDecimal("9.99++"); }, "'9.99++'");
            tx(() => { bigDecimalFactory.BigDecimal("0 0"); }, "'0 0'");

            Assert.Pass();
        }
    }
}