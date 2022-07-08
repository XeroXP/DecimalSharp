using DecimalSharp.Core;
using DecimalSharp.Core.Extensions;
using System.Globalization;
using System.Numerics;

namespace DecimalSharp
{
    internal class BigDecimalLightHelperFunctions
    {
        // -----------------------------------  EDITABLE DEFAULTS  ------------------------------------ //

        /// <summary>
        /// The limit on the value of `precision`, and on the value of the first argument to<br />
        /// `ToDecimalPlaces`, `ToExponential`, `ToFixed`, `ToPrecision` and `ToSignificantDigits`.
        /// </summary>
        internal static long MAX_DIGITS = (long)1e9;                        // 0 to 1e9

        internal static double LN10d = 2.302585092994046;
        /// <summary>
        /// The natural logarithm of 10 (1025 digits).
        /// </summary>
        internal static string LN10 = "2.302585092994045684017991454684364207601101488628772976033327900967572609677352480235997205089598298341967784042286";

        // ----------------------------------- END OF EDITABLE DEFAULTS ------------------------------- //

        internal static BigDecimalLight ONE = new BigDecimalLight(1);
        internal static int BASE = (int)1e7;
        internal static int LOG_BASE = 7;
        internal static long MAX_SAFE_INTEGER = 9007199254740991;
        internal static long MAX_E = (long)9e15;//(long)Math.Floor((double)MAX_SAFE_INTEGER / LOG_BASE);

        internal static string isDecimal = @"^(\d+(\.\d*)?|\.\d+)(e[+-]?\d+)?$";

        // Helper functions for BigDecimalLight.prototype (P) and/or BigDecimalLight methods, and their callers.


        /*
           *  add                 P.minus, P.plus
           *  checkInt32          P.todp, P.toExponential, P.toFixed, P.toPrecision, P.tosd
           *  digitsToString      P.log, P.sqrt, P.pow, toString, exp, ln
           *  divide              P.div, P.idiv, P.log, P.mod, P.sqrt, exp, ln
           *  exp                 P.exp, P.pow
           *  getBase10Exponent   P.exponent, P.sd, P.toint, P.sqrt, P.todp, P.toFixed, P.toPrecision,
           *                      P.toString, divide, round, toString, exp, ln
           *  getLn10             P.log, ln
           *  getZeroString       digitsToString, toString
           *  ln                  P.log, P.ln, P.pow, exp
           *  parseDecimal        BigDecimalLight
           *  round               P.abs, P.idiv, P.log, P.minus, P.mod, P.neg, P.plus, P.toint, P.sqrt,
           *                      P.times, P.todp, P.toExponential, P.toFixed, P.pow, P.toPrecision, P.tosd,
           *                      divide, getLn10, exp, ln
           *  subtract            P.minus, P.plus
           *  toString            P.toExponential, P.toFixed, P.toPrecision, P.toString, P.valueOf
           *  truncate            P.pow
           *
           *  Throws:             P.log, P.mod, P.sd, P.sqrt, P.pow,  checkInt32, divide, round,
           *                      getLn10, exp, ln, parseDecimal, BigDecimalLight, config
           */


        internal static BigDecimalLight add(BigDecimalLight x, BigDecimalLight y)
        {
            int carry;
            long e, i, k, len;
            int[] d, xd, yd;
            var Ctor = x.Config;
            var pr = Ctor.Precision;

            // If either is zero...
            if (!x.s.IsTrue() || !y.s.IsTrue())
            {

                // Return x if y is zero.
                // Return y if y is non-zero.
                if (!y.s.IsTrue()) y = new BigDecimalLight(x, Ctor);
                return Ctor.external ? round(y, pr) : y;
            }

            xd = x.d;
            yd = y.d;

            // x and y are finite, non-zero numbers with the same sign.

            k = x.e;
            e = y.e;
            xd = xd.Slice();
            i = k - e;

            // If base 1e7 exponents differ...
            if (i.IsTrue())
            {
                bool isYd = false;
                if (i < 0)
                {
                    d = xd;
                    i = -i;
                    len = yd.LongLength;
                }
                else
                {
                    d = yd;
                    e = k;
                    len = xd.LongLength;
                    isYd = true;
                }

                // Limit number of zeros prepended to max(ceil(pr / LOG_BASE), len) + 1.
                k = (long)Math.Ceiling((double)pr / LOG_BASE);
                len = k > len ? k + 1 : len + 1;

                if (i > len)
                {
                    i = len;
                    ArrayExtensions.Resize(ref d, 1);
                }

                // Prepend zeros to equalise exponents. Note: Faster to use reverse then do unshifts.
                ArrayExtensions.Reverse(ref d);
                for (; i--.IsTrue();) ArrayExtensions.Push(ref d, 0);
                ArrayExtensions.Reverse(ref d);

                if (isYd) yd = d;
                else xd = d;
            }

            len = xd.LongLength;
            i = yd.LongLength;

            // If yd is longer than xd, swap xd and yd so xd points to the longer array.
            if (len - i < 0)
            {
                i = len;
                d = yd;
                yd = xd;
                xd = d;
            }

            // Only start adding at yd.length - 1 as the further digits of xd can be left as they are.
            for (carry = 0; i.IsTrue();)
            {
                carry = (xd[--i] = xd[i] + yd[i] + carry) / BASE | 0;
                xd[i] %= BASE;
            }

            if (carry.IsTrue())
            {
                ArrayExtensions.Unshift(ref xd, carry);
                ++e;
            }

            // Remove trailing zeros.
            // No need to check for zero, as +x + +y != 0 && -x + -y != 0
            for (len = xd.LongLength; xd[--len] == 0;) ArrayExtensions.Pop(ref xd);

            y.d = xd;
            y.e = e;

            return Ctor.external ? round(y, pr) : y;
        }


        internal static void checkInt32(long i, long min, long max)
        {
            if (i != ~~i || i < min || i > max)
            {
                throw new BigDecimalException(BigDecimalException.InvalidArgument + i);
            }
        }

        internal static LongString digitsToString(int[] d)
        {
            long i, k;
            LongString ws;
            var indexOfLastWord = d.LongLength - 1;
            LongString str = "";
            var w = d[0];

            if (indexOfLastWord > 0)
            {
                str += w;
                for (i = 1; i < indexOfLastWord; i++)
                {
                    ws = d[i] + "";
                    k = LOG_BASE - ws.LongLength;
                    if (k.IsTrue()) str += getZeroString(k);
                    str += ws;
                }

                w = d[i];
                ws = w + "";
                k = LOG_BASE - ws.LongLength;
                if (k.IsTrue()) str += getZeroString(k);
            }
            else if (w == 0)
            {
                return "0";
            }

            // Remove trailing zeros of last w.
            for (; w % 10 == 0;) w /= 10;

            return str + w;
        }


        /// <summary>
        /// Perform division in the specified base.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pr"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        internal static BigDecimalLight divide(BigDecimalLight x, BigDecimalLight y, long? pr = null, long? dp = null)
        {
            return BigDecimalLightDivision.Divide(x, y, pr, dp);
        }


        /// <summary>
        /// Taylor/Maclaurin series.<br /><br />
        /// Exp(x) = x^0/0! + x^1/1! + x^2/2! + x^3/3! + ...<br /><br />
        /// Argument reduction:<br />
        /// Repeat x = x / 32, k += 5, until |x| &lt; 0.1<br />
        /// Exp(x) = Exp(x / 2^k)^(2^k)<br /><br />
        /// Previously, the argument was initially reduced by<br />
        /// Exp(x) = Exp(r) * 10^k  where r = x - k * ln10, k = Floor(x / ln10)<br />
        /// to first put r in the range [0, ln10], before dividing by 32 until |x| &lt; 0.1, but this was<br />
        /// found to be slower than just dividing repeatedly by 32 as above.<br /><br />
        /// (Math object integer min/max: Math.exp(709) = 8.2e+307, Math.exp(-745) = 5e-324)<br /><br />
        /// Exp(x) is non-terminating for any finite, non-zero x.<br /><br />
        /// </summary>
        /// <param name="x"></param>
        /// <param name="sd"></param>
        /// <returns>A new BigDecimalLight whose value is the natural exponential of `x` rounded to `sd` significant
        /// digits.</returns>
        internal static BigDecimalLight exp(BigDecimalLight x, long? sd = null)
        {
            long guard, wpr,
              i = 0,
              k = 0;
            BigDecimalLight denominator, sum, t, pow;
            var Ctor = x.Config;
            var pr = Ctor.Precision;

            if (getBase10Exponent(x) > 16) throw new BigDecimalException(BigDecimalException.ExponentOutOfRange + getBase10Exponent(x));

            // exp(0) = 1
            if (!x.s.IsTrue()) return new BigDecimalLight(ONE, Ctor);

            if (sd == null)
            {
                Ctor.external = false;
                wpr = pr;
            }
            else
            {
                wpr = sd.Value;
            }

            t = new BigDecimalLight(0.03125, Ctor);

            while (x.Abs().Gte(0.1))
            {
                x = x.Times(t);    // x = x / 2^5
                k += 5;
            }

            // Estimate the precision increase necessary to ensure the first 4 rounding digits are correct.
            guard = (long)(Math.Log(Math.Pow(2, k)) / LN10d * 2 + 5) | 0;
            wpr += guard;
            denominator = pow = sum = new BigDecimalLight(ONE, Ctor);
            Ctor.Precision = wpr;

            for (; ; )
            {
                pow = round(pow.Times(x), wpr);
                denominator = denominator.Times(++i);
                t = sum.Plus(divide(pow, denominator, wpr));

                if (digitsToString(t.d).Slice(0, wpr) == digitsToString(sum.d).Slice(0, wpr))
                {
                    while (k--.IsTrue()) sum = round(sum.Times(sum), wpr);
                    Ctor.Precision = pr;
                    if (sd == null)
                    {
                        Ctor.external = true; return round(sum, pr);
                    }
                    else return sum;
                }

                sum = t;
            }
        }


        /// <summary>
        /// Calculate the base 10 exponent from the base 1e7 exponent.
        /// </summary>
        /// <param name="digits"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        internal static long getBase10Exponent(BigDecimalLight x)
        {
            var e = x.e * LOG_BASE;
            var w = x.d[0];

            // Add the number of digits of the first word of the digits array.
            for (; w >= 10; w /= 10) e++;
            return e;
        }


        internal static BigDecimalLight getLn10(BigDecimalLightConfig Ctor, long sd, long? pr = null)
        {
            if (sd > LN10.Length - 1)
            {
                // Reset global state in case the exception is caught.
                Ctor.external = true;
                if (pr.IsTrue()) Ctor.Precision = pr.Value;
                throw new BigDecimalException(BigDecimalException.DecimalError + "LN10 precision limit exceeded");
            }

            return round(new BigDecimalLight(LN10, Ctor), sd);
        }


        internal static LongString getZeroString(long k)
        {
            var zs = "";
            for (; k--.IsTrue();) zs += "0";
            return zs;
        }


        /// <summary>
        /// Ln(n) (n != 1) is non-terminating.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="sd"></param>
        /// <returns>A new BigDecimalLight whose value is the natural logarithm of `x` truncated to `sd` significant
        /// digits.</returns>
        /// <exception cref="BigDecimalException"></exception>
        internal static BigDecimalLight ln(BigDecimalLight y, long? sd = null)
        {
            LongString c;
            char c0;
            long denominator, e, wpr;
            BigDecimalLight numerator, sum, t, x2;
            var n = 1;
            var guard = 10;
            var x = y;
            var xd = x.d;
            var Ctor = x.Config;
            var pr = Ctor.Precision;

            // ln(-x) = NaN
            // ln(0) = -Infinity
            if (x.s < 1) throw new BigDecimalException(BigDecimalException.DecimalError + (x.s.IsTrue() ? "NaN" : "-Infinity"));

            // ln(1) = 0
            if (x.Eq(ONE)) return new BigDecimalLight(0, Ctor);

            if (sd == null)
            {
                Ctor.external = false;
                wpr = pr;
            }
            else
            {
                wpr = sd.Value;
            }

            if (x.Eq(10))
            {
                if (sd == null) Ctor.external = true;
                return getLn10(Ctor, wpr);
            }

            wpr += guard;
            Ctor.Precision = wpr;
            c = digitsToString(xd);
            c0 = c.ElementAt(0);
            e = getBase10Exponent(x);

            if (Math.Abs(e) < 1.5e15)
            {

                // Argument reduction.
                // The series converges faster the closer the argument is to 1, so using
                // Ln(a^b) = b * Ln(a),   Ln(a) = Ln(a^b) / b
                // multiply the argument by itself until the leading digits of the significand are 7, 8, 9,
                // 10, 11, 12 or 13, recording the number of multiplications so the sum of the series can
                // later be divided by this number, then separate out the power of 10 using
                // Ln(a*10^b) = Ln(a) + b*ln(10).

                // max n is 21 (gives 0.9, 1.0 or 1.1) (9e15 / 21 = 4.2e14).
                //while (c0 < 9 && c0 != 1 || c0 == 1 && c.charAt(1) > 1) {
                // max n is 6 (gives 0.7 - 1.3)
                while ((int)(c0 - '0') < 7 && (int)(c0 - '0') != 1 || (int)(c0 - '0') == 1 && c.LongLength > 1 && (int)(c.ElementAt(1) - '0') > 3)
                {
                    x = x.Times(y);
                    c = digitsToString(x.d);
                    c0 = c.ElementAt(0);
                    n++;
                }

                e = getBase10Exponent(x);

                if ((int)(c0 - '0') > 1)
                {
                    x = new BigDecimalLight(("0." + c).ToString(), Ctor);
                    e++;
                }
                else
                {
                    x = new BigDecimalLight((c0 + "." + c.Slice(1)).ToString(), Ctor);
                }
            }
            else
            {

                // The argument reduction method above may result in overflow if the argument y is a massive
                // number with exponent >= 1500000000000000 (9e15 / 6 = 1.5e15), so instead recall this
                // function using Ln(x*10^e) = Ln(x) + e*ln(10).
                t = getLn10(Ctor, wpr + 2, pr).Times(e.ToString(CultureInfo.InvariantCulture));
                x = ln(new BigDecimalLight((c0 + "." + c.Slice(1)).ToString(), Ctor), wpr - guard).Plus(t);

                Ctor.Precision = pr;
                if (sd == null) {
                    Ctor.external = true;
                    return round(x, pr);
                }
                else return x;
            }

            // x reduced to a value near 1.

            // Taylor series.
            // Ln(y) = Ln((1 + x)/(1 - x)) = 2(x + x^3/3 + x^5/5 + x^7/7 + ...)
            // where x = (y - 1)/(y + 1)    (|x| < 1)
            sum = numerator = x = divide(x.Minus(ONE), x.Plus(ONE), wpr);
            x2 = round(x.Times(x), wpr);
            denominator = 3;

            for (; ; )
            {
                numerator = round(numerator.Times(x2), wpr);
                t = sum.Plus(divide(numerator, new BigDecimalLight(denominator, Ctor), wpr));

                if (digitsToString(t.d).Slice(0, wpr) == digitsToString(sum.d).Slice(0, wpr))
                {
                    sum = sum.Times(2);

                    // Reverse the argument reduction. Check that e is not 0 because, besides preventing an
                    // unnecessary calculation, -0 + 0 = +0 and to ensure correct rounding -0 needs to stay -0.
                    if (e != 0) sum = sum.Plus(getLn10(Ctor, wpr + 2, pr).Times(e.ToString(CultureInfo.InvariantCulture)));
                    sum = divide(sum, new BigDecimalLight(n, Ctor), wpr);


                    Ctor.Precision = pr;
                    if (sd == null)
                    {
                        Ctor.external = true;
                        return round(sum, pr);
                    }
                    else return sum;
                }

                sum = t;
                denominator += 2;
            }
        }

        /// <summary>
        /// Parse the value of a new BigDecimalLight `x` from string `str`.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static BigDecimalLight parseDecimal(BigDecimalLight x, LongString str)
        {
            BigInteger e;
            long i, len;
            var Ctor = x.Config;

            // Decimal point?
            if ((e = str.IndexOf(".")) > -1) str = str.Replace(".", "");

            // Exponential form?
            if ((i = str.IndexOf("e", StringComparison.InvariantCultureIgnoreCase)) > 0)
            {

                // Determine exponent.
                if (e < 0) e = i;
                e += BigInteger.Parse(str.Slice(i + 1));
                str = str.Slice(0, i);
            }
            else if (e < 0)
            {

                // Integer.
                e = str.LongLength;
            }

            // Determine leading zeros.
            for (i = 0; i < str.LongLength && str.ElementAt(i) == 48; i++) ;

            // Determine trailing zeros.
            for (len = str.LongLength; (len - 1) >= 0 && str.ElementAt(len - 1) == 48; --len) ;
            str = str.Slice(i, len);

            if (!string.IsNullOrEmpty(str))
            {
                len -= i;
                e = e - i - 1;
                x.e = (long)Math.Floor((double)e / LOG_BASE) > MAX_E ? MAX_E + 1 : (long)Math.Floor((double)e / LOG_BASE) < -MAX_E ? -MAX_E - 1 : (long)Math.Floor((double)e / LOG_BASE);
                x.d = new int[0];

                // Transform @base

                // e is the @base 10 exponent.
                // i is where to slice str to get the first word of the digits array.
                i = (long)((e + 1) % LOG_BASE);
                if (e < 0) i += LOG_BASE;

                if (i < len)
                {
                    if (i.IsTrue()) ArrayExtensions.Push(ref x.d, int.Parse(str.Slice(0, i), CultureInfo.InvariantCulture));
                    for (len -= LOG_BASE; i < len;) ArrayExtensions.Push(ref x.d, int.Parse(str.Slice(i, i += LOG_BASE), CultureInfo.InvariantCulture));
                    str = str.Slice(i);
                    i = LOG_BASE - str.LongLength;
                }
                else
                {
                    i -= len;
                }

                for (; i--.IsTrue();) str += "0";
                ArrayExtensions.Push(ref x.d, int.Parse(str, CultureInfo.InvariantCulture));

                if (Ctor.external && (x.e > MAX_E || x.e < -MAX_E)) throw new BigDecimalException(BigDecimalException.ExponentOutOfRange + e);
            }
            else
            {

                // Zero.
                x.s = 0;
                x.e = 0;
                x.d = new int[] { 0 };
            }

            return x;
        }


        /// <summary>
        /// Round `x` to `sd` significant digits, using rounding mode `rm` if present (truncate otherwise).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="sd"></param>
        /// <param name="rm"></param>
        /// <returns></returns>
        /// <exception cref="BigDecimalException"></exception>
        internal static BigDecimalLight round(BigDecimalLight x, long sd, RoundingMode? rm = null)
        {
            long i, j, k, n, rd, w, xdi;
            bool doRound = false;
            var Ctor = x.Config;
            var xd = x.d;

            // rd: the rounding digit, i.e. the digit after the digit that may be rounded up.
            // w: the word of xd which contains the rounding digit, a base 1e7 number.
            // xdi: the index of w within xd.
            // n: the number of digits of w.
            // i: what would be the index of rd within w if all the numbers were 7 digits long (i.e. if
            // they had leading zeros)
            // j: if > 0, the actual index of rd within w (if < 0, rd is a leading zero).

            // Get the length of the first word of the digits array xd.
            for (n = 1, k = xd[0]; k >= 10; k /= 10) n++;
            i = sd - n;

            // Is the rounding digit in the first word of xd?
            if (i < 0)
            {
                i += LOG_BASE;
                j = sd;
                w = xd[xdi = 0];
            }
            else
            {
                xdi = (long)Math.Ceiling(((double)i + 1) / LOG_BASE);
                k = xd.LongLength;
                if (xdi >= k) return x;
                w = k = xd[xdi];

                // Get the number of digits of w.
                for (n = 1; k >= 10; k /= 10) n++;

                // Get the index of rd within w.
                i %= LOG_BASE;

                // Get the index of rd within w, adjusted for leading zeros.
                // The number of leading zeros of w is given by LOG_BASE - n.
                j = i - LOG_BASE + n;
            }

            if (rm != null) {
                k = (long)Math.Pow(10, n - j - 1);

                // Get the rounding digit at index j of w.
                rd = w / k % 10 | 0;

                // Are there any non-zero digits after the rounding digit?
                doRound = sd < 0 || xd.LongLength > xdi + 1 || (w % k).IsTrue();

                // The expression `w % mathpow(10, n - j - 1)` returns all the digits of w to the right of the
                // digit at (left-to-right) index j, e.g. if w is 908714 and j is 2, the expression will give
                // 714.

                doRound = rm < RoundingMode.ROUND_HALF_UP
                  ? (rd.IsTrue() || doRound) && (rm == RoundingMode.ROUND_UP || rm == (x.s < 0 ? RoundingMode.ROUND_FLOOR : RoundingMode.ROUND_CEIL))
                  : rd > 5 || rd == 5 && (rm == RoundingMode.ROUND_HALF_UP || doRound || rm == RoundingMode.ROUND_HALF_EVEN &&

                    // Check whether the digit to the left of the rounding digit is odd.
                    (((i > 0 ? j > 0 ? (long)(w / Math.Pow(10, n - j)) : 0 : xd.LongLength > xdi - 1 && xdi >= 1 ? xd[xdi - 1] : 0) % 10) & 1).IsTrue() ||
                      rm == (x.s < 0 ? RoundingMode.ROUND_HALF_FLOOR : RoundingMode.ROUND_HALF_CEIL));
            }

            if (sd < 1 || !xd[0].IsTrue())
            {
                if (doRound)
                {
                    k = getBase10Exponent(x);
                    ArrayExtensions.Resize(ref xd, 1);

                    // Convert sd to decimal places.
                    sd = sd - k - 1;

                    // 1, 0.1, 0.01, 0.001, 0.0001 etc.
                    xd[0] = (int)Math.Pow(10, (LOG_BASE - sd % LOG_BASE) % LOG_BASE);
                    x.e = (long)Math.Floor((double)-sd / LOG_BASE);
                }
                else
                {
                    ArrayExtensions.Resize(ref xd, 1);

                    // Zero.
                    x.e = x.s = 0;
                    xd[0] = 0;
                }
                x.d = xd;

                return x;
            }

            // Remove excess digits.
            if (i == 0)
            {
                ArrayExtensions.Resize(ref xd, xdi);
                k = 1;
                xdi--;
            }
            else
            {
                ArrayExtensions.Resize(ref xd, xdi + 1);
                k = (long)Math.Pow(10, LOG_BASE - i);

                // E.g. 56700 becomes 56000 if 7 is the rounding digit.
                // j > 0 means i > number of leading zeros of w.
                xd[xdi] = j > 0 ? (int)(((long)(w / Math.Pow(10, n - j) % Math.Pow(10, j)) | 0) * k) : 0;
            }

            if (doRound)
            {
                for (; ; )
                {

                    // Is the digit to be rounded up in the first word of xd?
                    if (xdi == 0)
                    {
                        if ((xd[0] += (int)k) == BASE)
                        {
                            xd[0] = 1;
                            ++x.e;
                        }

                        break;
                    }
                    else
                    {
                        xd[xdi] += (int)k;
                        if (xd[xdi] != BASE) break;
                        xd[xdi--] = 0;
                        k = 1;
                    }
                }
            }

            // Remove trailing zeros.
            for (i = xd.LongLength; i >= 0 && xd[--i] == 0;) ArrayExtensions.Pop(ref xd);

            if (Ctor.external && (x.e > MAX_E || x.e < -MAX_E))
            {
                throw new BigDecimalException(BigDecimalException.ExponentOutOfRange + getBase10Exponent(x));
            }

            x.d = xd;

            return x;
        }


        internal static BigDecimalLight subtract(BigDecimalLight x, BigDecimalLight y)
        {
            long e, i, j, k, len, xe;
            int[] d, xd, yd;
            bool xLTy;
            var Ctor = x.Config;
            var pr = Ctor.Precision;

            // Return y negated if x is zero.
            // Return x if y is zero and x is non-zero.
            if (!x.s.IsTrue() || !y.s.IsTrue())
            {
                if (y.s.IsTrue()) y.s = -y.s;
                else y = new BigDecimalLight(x, Ctor);
                return Ctor.external ? round(y, pr) : y;
            }

            xd = x.d;
            yd = y.d;

            // x and y are non-zero numbers with the same sign.

            e = y.e;
            xe = x.e;
            xd = xd.Slice();
            k = xe - e;

            // If exponents differ...
            if (k.IsTrue())
            {
                xLTy = k < 0;

                if (xLTy)
                {
                    d = xd;
                    k = -k;
                    len = yd.LongLength;
                }
                else
                {
                    d = yd;
                    e = xe;
                    len = xd.LongLength;
                }

                // Numbers with massively different exponents would result in a very high number of zeros
                // needing to be prepended, but this can be avoided while still ensuring correct rounding by
                // limiting the number of zeros to `Math.ceil(pr / LOG_BASE) + 2`.
                i = Math.Max((long)Math.Ceiling((double)pr / LOG_BASE), len) + 2;

                if (k > i)
                {
                    k = i;
                    ArrayExtensions.Resize(ref d, 1);
                }

                // Prepend zeros to equalise exponents.
                ArrayExtensions.Reverse(ref d);
                for (i = k; i--.IsTrue();) ArrayExtensions.Push(ref d, 0);
                ArrayExtensions.Reverse(ref d);

                if (xLTy) xd = d;
                else yd = d;
                // Base 1e7 exponents equal.
            }
            else
            {

                // Check digits to determine which is the bigger number.

                i = xd.LongLength;
                len = yd.LongLength;
                xLTy = i < len;
                if (xLTy) len = i;

                for (i = 0; i < len; i++)
                {
                    if (xd[i] != yd[i])
                    {
                        xLTy = xd[i] < yd[i];
                        break;
                    }
                }

                k = 0;
            }

            if (xLTy)
            {
                d = xd;
                xd = yd;
                yd = d;
                y.s = -y.s;
            }

            len = xd.LongLength;

            // Append zeros to xd if shorter.
            // Don't add zeros to yd if shorter as subtraction only needs to start at yd length.
            for (i = yd.LongLength - len; i > 0; --i)
            {
                if (xd.LongLength <= len) ArrayExtensions.Resize(ref xd, len + 1);
                xd[len++] = 0;
            }

            // Subtract yd from xd.
            for (i = yd.LongLength; i > k;)
            {
                if (xd[--i] < yd[i])
                {
                    for (j = i; j.IsTrue() && xd[--j] == 0;) xd[j] = BASE - 1;
                    --xd[j];
                    xd[i] += BASE;
                }

                xd[i] -= yd[i];
            }

            // Remove trailing zeros.
            for (; len > 0 && xd[--len] == 0;) ArrayExtensions.Pop(ref xd);

            // Remove leading zeros and adjust exponent accordingly.
            for (; xd.LongLength > 0 && xd[0] == 0; ArrayExtensions.Shift(ref xd)) --e;

            // Zero?
            if (xd.LongLength == 0 || !xd[0].IsTrue()) return new BigDecimalLight(0, Ctor);

            y.d = xd;
            y.e = e;

            //return external && xd.length >= pr / LOG_BASE ? round(y, pr) : y;
            return Ctor.external ? round(y, pr) : y;
        }


        internal static string toString(BigDecimalLight x, bool isExp = false, long? sd = null)
        {
            long k, e = getBase10Exponent(x);
            var str = digitsToString(x.d);
            var len = str.LongLength;

            if (isExp)
            {
                if (sd.IsTrue() && (k = sd.Value - len) > 0)
                {
                    str = str.ElementAt(0) + "." + str.Slice(1) + getZeroString(k);
                }
                else if (len > 1)
                {
                    str = str.ElementAt(0) + "." + str.Slice(1);
                }

                str = str + (e < 0 ? "e" : "e+") + e;
            }
            else if (e < 0)
            {
                str = "0." + getZeroString(-e - 1) + str;
                if (sd.IsTrue() && (k = sd.Value - len) > 0) str += getZeroString(k);
            }
            else if (e >= len)
            {
                str += getZeroString(e + 1 - len);
                if (sd.IsTrue() && (k = sd.Value - e - 1) > 0) str = str + "." + getZeroString(k);
            }
            else
            {
                if ((k = e + 1) < len) str = str.Slice(0, k) + "." + str.Slice(k);
                if (sd.IsTrue() && (k = sd.Value - len) > 0)
                {
                    if (e + 1 == len) str += ".";
                    str += getZeroString(k);
                }
            }

            return x.s < 0 ? "-" + str : str;
        }


        /// <summary>
        /// Does not strip trailing zeros.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        internal static bool truncate(ref int[] arr, long len)
        {
            if (arr.LongLength > len)
            {
                ArrayExtensions.Resize(ref arr, len);
                return true;
            }
            return false;
        }
    }
}
