using DecimalSharp.Core;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimal")]
    public class BigDecimalTests
    {
        [SetUp]
        public void Setup()
        {
        }

        internal static int testNumber = 0, passed = 0;

        /*internal static void write(string str)
        {
            Console.WriteLine(str);
        }*/

        internal static void Fail(object actual)
        {
            /*write(
                "\n  Test number " + testNumber + " failed: assert" +
                "\n  Expected: true" +
                "\n  Actual:   " + actual
            );*/
            Assert.Fail(
                "\n  Test number " + testNumber + " failed: assert" +
                "\n  Expected: true" +
                "\n  Actual:   " + actual
            );
        }

        internal static void assert(bool actual)
        {
            ++testNumber;
            if (actual == true)
            {
                ++passed;
                //write('\n Expected and actual: ' + actual);
            }
            else
            {
                /*write(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: true" +
                    "\n  Actual:   " + actual
                );*/
                Assert.Fail(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: true" +
                    "\n  Actual:   " + actual
                );
            }
        }

        internal static void assertEqual(string expected, string actual)
        {
            ++testNumber;
            // If expected and actual are both NaN, consider them equal.
            if (expected == actual || expected != expected && actual != actual)
            {
                ++passed;
            }
            else
            {
                /*write(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );*/
                Assert.Fail(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );
            }
        }
        internal static void assertEqual(int? expected, int? actual)
        {
            ++testNumber;
            // If expected and actual are both NaN, consider them equal.
            if (expected == actual || expected != expected && actual != actual)
            {
                ++passed;
            }
            else
            {
                /*write(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );*/
                Assert.Fail(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );
            }
        }
        internal static void assertEqual(long? expected, long? actual)
        {
            ++testNumber;
            // If expected and actual are both NaN, consider them equal.
            if (expected == actual || expected != expected && actual != actual)
            {
                ++passed;
            }
            else
            {
                /*write(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );*/
                Assert.Fail(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );
            }
        }
        internal static void assertEqual(double? expected, double? actual)
        {
            ++testNumber;
            // If expected and actual are both NaN, consider them equal.
            if (expected == actual || expected != expected && actual != actual)
            {
                ++passed;
            }
            else
            {
                /*write(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );*/
                Assert.Fail(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );
            }
        }
        internal static void assertEqual(bool expected, bool actual)
        {
            ++testNumber;
            // If expected and actual are both NaN, consider them equal.
            if (expected == actual || expected != expected && actual != actual)
            {
                ++passed;
            }
            else
            {
                /*write(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: true" +
                    "\n  Actual:   " + actual
                );*/
                Assert.Fail(
                    "\n  Test number " + testNumber + " failed: assert" +
                    "\n  Expected: " + expected +
                    "\n  Actual:   " + actual
                );
            }
        }

        internal static void assertEqualDecimal(BigDecimal x, BigDecimal y)
        {
            ++testNumber;
            if (x.Eq(y) || x.IsNaN() && y.IsNaN())
            {
                ++passed;
            }
            else
            {
                /*write(
                  "\n  Test number " + testNumber + " failed: assertEqualDecimal" +
                  "\n  x: " + x.valueOf() +
                  "\n  y: " + y.valueOf()
                );*/
                Assert.Fail(
                  "\n  Test number " + testNumber + " failed: assertEqualDecimal" +
                  "\n  x: " + x.ValueOf() +
                  "\n  y: " + y.ValueOf()
                );
            }
        }

        internal static void assertEqualProps(int[] digits, int exponent, int sign, BigDecimal n)
        {
            int i = 0, len = digits.Length;
            ++testNumber;
            while (i < len && digits[i] == n.d[i]) ++i;
            if (i == len && i == n.d.Length && exponent == n.e && sign == n.s)
            {
                ++passed;
            }
            else
            {
                /*write(
                  "\n  Test number " + testNumber + " failed: assertEqualProps" +
                  "\n  Expected digits:   " + digits +
                  "\n  Expected exponent: " + exponent +
                  "\n  Expected sign:     " + sign +
                  "\n  Actual digits:     " + n.d +
                  "\n  Actual exponent:   " + n.e +
                  "\n  Actual sign:       " + n.s
                );*/
                Assert.Fail(
                  "\n  Test number " + testNumber + " failed: assertEqualProps" +
                  "\n  Expected digits:   " + JsonConvert.SerializeObject(digits) +
                  "\n  Expected exponent: " + exponent +
                  "\n  Expected sign:     " + sign +
                  "\n  Actual digits:     " + JsonConvert.SerializeObject(n.d) +
                  "\n  Actual exponent:   " + n.e +
                  "\n  Actual sign:       " + n.s
                );
            }
        }
        internal static void assertEqualProps(int[] digits, int exponent, int sign, BigDecimalLight n)
        {
            int i = 0, len = digits.Length;
            ++testNumber;
            while (i < len && digits[i] == n.d[i]) ++i;
            if (i == len && i == n.d.Length && exponent == n.e && sign == n.s)
            {
                ++passed;
            }
            else
            {
                /*write(
                  "\n  Test number " + testNumber + " failed: assertEqualProps" +
                  "\n  Expected digits:   " + digits +
                  "\n  Expected exponent: " + exponent +
                  "\n  Expected sign:     " + sign +
                  "\n  Actual digits:     " + n.d +
                  "\n  Actual exponent:   " + n.e +
                  "\n  Actual sign:       " + n.s
                );*/
                Assert.Fail(
                  "\n  Test number " + testNumber + " failed: assertEqualProps" +
                  "\n  Expected digits:   " + JsonConvert.SerializeObject(digits) +
                  "\n  Expected exponent: " + exponent +
                  "\n  Expected sign:     " + sign +
                  "\n  Actual digits:     " + JsonConvert.SerializeObject(n.d) +
                  "\n  Actual exponent:   " + n.e +
                  "\n  Actual sign:       " + n.s
                );
            }
        }

        internal static void assertException(Action func, string msg)
        {
            BigDecimalException? actual = null;
            ++testNumber;
            try
            {
                func();
            }
            catch (BigDecimalException e)
            {
                actual = e;
            }
            if (actual != null && Regex.IsMatch(actual.Message, @"DecimalError"))
            {
                ++passed;
            }
            else
            {
                /*write(
                    "\n  Test number " + testNumber + " failed: assertException" +
                    "\n  Expected: " + msg + " to raise a DecimalError." +
                    "\n  Actual:   " + (actual?.ToString() ?? "no exception")
                );*/
                Assert.Fail(
                    "\n  Test number " + testNumber + " failed: assertException" +
                    "\n  Expected: " + msg + " to raise a DecimalError." +
                    "\n  Actual:   " + (actual?.ToString() ?? "no exception")
                );
            }
        }
    }
}