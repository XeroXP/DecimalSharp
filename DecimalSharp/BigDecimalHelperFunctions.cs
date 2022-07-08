using DecimalSharp.Core;
using DecimalSharp.Core.Extensions;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace DecimalSharp
{
    internal class BigDecimalHelperFunctions
    {
        // -----------------------------------  EDITABLE DEFAULTS  ------------------------------------ //

        /// <summary>
        /// The maximum exponent magnitude.<br />
        /// The limit on the value of `ToExpNeg`, `ToExpPos`, `MinE` and `MaxE`.
        /// </summary>
        internal static long EXP_LIMIT = (long)9e15;                      // 0 to 9e15

        /// <summary>
        /// The limit on the value of `precision`, and on the value of the first argument to<br />
        /// `ToDecimalPlaces`, `ToExponential`, `ToFixed`, `ToPrecision` and `ToSignificantDigits`.
        /// </summary>
        internal static long MAX_DIGITS = (long)1e9;                        // 0 to 1e9

        /// <summary>
        /// Base conversion alphabet.
        /// </summary>
        internal static string NUMERALS = "0123456789abcdef";

        internal static double LN10d = 2.302585092994046;
        /// <summary>
        /// The natural logarithm of 10 (1025 digits).
        /// </summary>
        internal static string LN10 = "2.3025850929940456840179914546843642076011014886287729760333279009675726096773524802359972050895982983419677840422862486334095254650828067566662873690987816894829072083255546808437998948262331985283935053089653777326288461633662222876982198867465436674744042432743651550489343149393914796194044002221051017141748003688084012647080685567743216228355220114804663715659121373450747856947683463616792101806445070648000277502684916746550586856935673420670581136429224554405758925724208241314695689016758940256776311356919292033376587141660230105703089634572075440370847469940168269282808481184289314848524948644871927809676271275775397027668605952496716674183485704422507197965004714951050492214776567636938662976979522110718264549734772662425709429322582798502585509785265383207606726317164309505995087807523710333101197857547331541421808427543863591778117054309827482385045648019095610299291824318237525357709750539565187697510374970888692180205189339507238539205144634197265287286965110862571492198849978748873771345686209167058";

        /// <summary>
        /// Pi (1025 digits).
        /// </summary>
        internal static string PI = "3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491298336733624406566430860213949463952247371907021798609437027705392171762931767523846748184676694051320005681271452635608277857713427577896091736371787214684409012249534301465495853710507922796892589235420199561121290219608640344181598136297747713099605187072113499999983729780499510597317328160963185950244594553469083026425223082533446850352619311881710100031378387528865875332083814206171776691473035982534904287554687311595628638823537875937519577818577805321712268066130019278766111959092164201989380952572010654858632789";

        // ----------------------------------- END OF EDITABLE DEFAULTS ------------------------------- //

        internal static int BASE = (int)1e7;
        internal static int LOG_BASE = 7;
        internal static long MAX_SAFE_INTEGER = 9007199254740991;

        internal static int LN10_PRECISION = LN10.Length - 1;
        internal static int PI_PRECISION = PI.Length - 1;

        internal static string isBinary = @"^0b([01]+(\.[01]*)?|\.[01]+)(p[+-]?\d+)?$";
        internal static string isHex = @"^0x([0-9a-f]+(\.[0-9a-f]*)?|\.[0-9a-f]+)(p[+-]?\d+)?$";
        internal static string isOctal = @"^0o([0-7]+(\.[0-7]*)?|\.[0-7]+)(p[+-]?\d+)?$";
        internal static string isDecimal = @"^(\d+(\.\d*)?|\.\d+)(e[+-]?\d+)?$";

        // Helper functions for BigDecimal.prototype (P) and/or BigDecimal methods, and their callers.


        /*
         *  digitsToString           P.cubeRoot, P.logarithm, P.squareRoot, P.toFraction, P.toPower,
         *                           finiteToString, naturalExponential, naturalLogarithm
         *  checkInt32               P.toDecimalPlaces, P.toExponential, P.toFixed, P.toNearest,
         *                           P.toPrecision, P.toSignificantDigits, toStringBinary, random
         *  checkRoundingDigits      P.logarithm, P.toPower, naturalExponential, naturalLogarithm
         *  convertBase              toStringBinary, parseOther
         *  cos                      P.cos
         *  divide                   P.atanh, P.cubeRoot, P.dividedBy, P.dividedToIntegerBy,
         *                           P.logarithm, P.modulo, P.squareRoot, P.tan, P.tanh, P.toFraction,
         *                           P.toNearest, toStringBinary, naturalExponential, naturalLogarithm,
         *                           taylorSeries, atan2, parseOther
         *  finalise                 P.absoluteValue, P.atan, P.atanh, P.ceil, P.cos, P.cosh,
         *                           P.cubeRoot, P.dividedToIntegerBy, P.floor, P.logarithm, P.minus,
         *                           P.modulo, P.negated, P.plus, P.round, P.sin, P.sinh, P.squareRoot,
         *                           P.tan, P.times, P.toDecimalPlaces, P.toExponential, P.toFixed,
         *                           P.toNearest, P.toPower, P.toPrecision, P.toSignificantDigits,
         *                           P.truncated, divide, getLn10, getPi, naturalExponential,
         *                           naturalLogarithm, ceil, floor, round, trunc
         *  finiteToString           P.toExponential, P.toFixed, P.toPrecision, P.toString, P.valueOf,
         *                           toStringBinary
         *  getBase10Exponent        P.minus, P.plus, P.times, parseOther
         *  getLn10                  P.logarithm, naturalLogarithm
         *  getPi                    P.acos, P.asin, P.atan, toLessThanHalfPi, atan2
         *  getPrecision             P.precision, P.toFraction
         *  getZeroString            digitsToString, finiteToString
         *  intPow                   P.toPower, parseOther
         *  isOdd                    toLessThanHalfPi
         *  maxOrMin                 max, min
         *  naturalExponential       P.naturalExponential, P.toPower
         *  naturalLogarithm         P.acosh, P.asinh, P.atanh, P.logarithm, P.naturalLogarithm,
         *                           P.toPower, naturalExponential
         *  nonFiniteToString        finiteToString, toStringBinary
         *  parseDecimal             BigDecimal
         *  parseOther               BigDecimal
         *  sin                      P.sin
         *  taylorSeries             P.cosh, P.sinh, cos, sin
         *  toLessThanHalfPi         P.cos, P.sin
         *  toStringBinary           P.toBinary, P.toHexadecimal, P.toOctal
         *  truncate                 intPow
         *
         *  Throws:                  P.logarithm, P.precision, P.toFraction, checkInt32, getLn10, getPi,
         *                           naturalLogarithm, config, parseOther, random, BigDecimal
         */


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


        internal static void checkInt32(long i, long min, long max)
        {
            if (i != ~~i || i < min || i > max)
            {
                throw new BigDecimalException(BigDecimalException.InvalidArgument + i);
            }
        }


        /// <summary>
        /// Check 5 rounding digits if `repeating` is null, 4 otherwise.<br />
        /// `repeating == null` if caller is `log` or `pow`,<br />
        /// `repeating != null` if caller is `naturalLogarithm` or `naturalExponential`.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="i"></param>
        /// <param name="rm"></param>
        /// <param name="repeating"></param>
        /// <returns></returns>
        internal static bool checkRoundingDigits(int[] d, long i, RoundingMode rm, bool? repeating = null)
        {
            long di, k, rd;
            bool r;

            // Get the length of the first word of the array d.
            for (k = d[0]; k >= 10; k /= 10) --i;

            // Is the rounding digit in the first word of d?
            if (--i < 0)
            {
                i += LOG_BASE;
                di = 0;
            }
            else
            {
                di = (long)Math.Ceiling((double)(i + 1) / LOG_BASE);
                i %= LOG_BASE;
            }

            // i is the index (0 - 6) of the rounding digit.
            // E.g. if within the word 3487563 the first rounding digit is 5,
            // then i = 4, k = 1000, rd = 3487563 % 1000 = 563
            k = (long)Math.Pow(10, LOG_BASE - i);
            rd = d.LongLength > di ? (d[di] % k) | 0 : 0;

            if (repeating == null)
            {
                if (i < 3)
                {
                    if (i == 0) rd = rd / 100 | 0;
                    else if (i == 1) rd = rd / 10 | 0;
                    r = rm < RoundingMode.ROUND_HALF_UP && rd == 99999 || rm > RoundingMode.ROUND_FLOOR && rd == 49999 || rd == 50000 || rd == 0;
                }
                else
                {
                    r = (rm < RoundingMode.ROUND_HALF_UP && rd + 1 == k || rm > RoundingMode.ROUND_FLOOR && rd + 1 == k / 2) &&
                      (d.LongLength > di + 1 ? d[di + 1] / k / 100 : 0) == Math.Pow(10, i - 2) - 1 ||
                        (rd == k / 2 || rd == 0) && (d.LongLength > di + 1 ? d[di + 1] / k / 100 : 0) == 0;
                }
            }
            else
            {
                if (i < 4)
                {
                    if (i == 0) rd = rd / 1000 | 0;
                    else if (i == 1) rd = rd / 100 | 0;
                    else if (i == 2) rd = rd / 10 | 0;
                    r = (repeating.IsTrue() || rm < RoundingMode.ROUND_HALF_UP) && rd == 9999 || !repeating.IsTrue() && rm > RoundingMode.ROUND_FLOOR && rd == 4999;
                }
                else
                {
                    r = ((repeating.IsTrue() || rm < RoundingMode.ROUND_HALF_UP) && rd + 1 == k ||
                    (!repeating.IsTrue() && rm > RoundingMode.ROUND_FLOOR) && rd + 1 == k / 2) &&
                      (d[di + 1] / k / 1000 | 0) == Math.Pow(10, i - 3) - 1;
                }
            }

            return r;
        }


        /// <summary>
        /// Convert string of `baseIn` to an array of numbers of `baseOut`.<br />
        /// Eg. convertBase("255", 10, 16) returns [15, 15].<br />
        /// Eg. convertBase("ff", 16, 10) returns [2, 5, 5].
        /// </summary>
        /// <param name="str"></param>
        /// <param name="baseIn"></param>
        /// <param name="baseOut"></param>
        /// <returns></returns>
        internal static int[] convertBase(LongString str, int @baseIn, int @baseOut)
        {
            long j;
            var arr = new[] { 0 };
            long arrL;
            var i = 0;
            var strL = str.LongLength;

            for (; i < strL;)
            {
                for (arrL = arr.LongLength; arrL--.IsTrue();) arr[arrL] *= @baseIn;
                arr[0] += NUMERALS.IndexOf(str.ElementAt(i++));
                for (j = 0; j < arr.LongLength; j++)
                {
                    if (arr[j] > @baseOut - 1)
                    {
                        if (arr.LongLength <= j + 1) ArrayExtensions.AddElementAt(ref arr, j + 1, 0);
                        arr[j + 1] += arr[j] / @baseOut | 0;
                        arr[j] %= @baseOut;
                    }
                }
            }

            ArrayExtensions.Reverse(ref arr);
            return arr;
        }


        /// <summary>
        /// Cos(x) = 1 - x^2/2! + x^4/4! - ...<br />
        /// |x| &lt; pi/2
        /// </summary>
        /// <param name="Ctor"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static BigDecimal cosine(BigDecimalConfig Ctor, BigDecimal x)
        {
            long k, len;
            LongString y;

            if (x.IsZero()) return x;

            // Argument reduction: Cos(4x) = 8*(cos^4(x) - cos^2(x)) + 1
            // i.e. Cos(x) = 8*(cos^4(x/4) - cos^2(x/4)) + 1

            // Estimate the optimum number of times to use the argument reduction.
            len = x.d.LongLength;
            if (len < 32)
            {
                k = (long)Math.Ceiling((double)len / 3);
                y = ((double)1 / tinyPow(4, k)).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                k = 16;
                y = "2.3283064365386962890625e-10";
            }

            Ctor.Precision += k;

            x = taylorSeries(Ctor, 1, x.Times(y.ToString()), new BigDecimal(1, Ctor));

            // Reverse argument reduction
            for (var i = k; i--.IsTrue();)
            {
                var cos2x = x.Times(x);
                x = cos2x.Times(cos2x).Minus(cos2x).Times(8).Plus(1);
            }

            Ctor.Precision -= k;

            return x;
        }


        /// <summary>
        /// Perform division in the specified base.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="inexact"></param>
        /// <param name="pr"></param>
        /// <param name="rm"></param>
        /// <param name="dp"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        internal static BigDecimal divide(BigDecimal x, BigDecimal y, out bool inexact, long? pr = null, RoundingMode? rm = null, long? dp = null, int? @base = null)
        {
            return BigDecimalDivision.Divide(x, y, pr, rm, dp, @base, out inexact);
        }
        /// <summary>
        /// Perform division in the specified base.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pr"></param>
        /// <param name="rm"></param>
        /// <param name="dp"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        internal static BigDecimal divide(BigDecimal x, BigDecimal y, long? pr = null, RoundingMode? rm = null, long? dp = null, int? @base = null)
        {
            return BigDecimalDivision.Divide(x, y, pr, rm, dp, @base, out _);
        }


        /// <summary>
        /// Round `x` to `sd` significant digits using rounding mode `rm`.<br />
        /// Check for over/under-flow.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="sd"></param>
        /// <param name="rm"></param>
        /// <param name="isTruncated"></param>
        /// <returns></returns>
        internal static BigDecimal finalise(BigDecimal x, long? sd = null, RoundingMode? rm = null, bool isTruncated = false)
        {
            long digits, i, j, k, rd, w, xdi;
            bool roundUp;
            int[] xd;
            var Ctor = x.Config;

            // Don"t round if sd is null or undefined.
            if (sd != null)
            {
                xd = x.d;

                // Infinity/null.
                if (!xd.IsTrue()) return x;

                // rd: the rounding digit, i.e. the digit after the digit that may be rounded up.
                // w: the word of xd containing rd, a @base 1e7 number.
                // xdi: the index of w within xd.
                // digits: the number of digits of w.
                // i: what would be the index of rd within w if all the numbers were 7 digits long (i.e. if
                // they had leading zeros)
                // j: if > 0, the actual index of rd within w (if < 0, rd is a leading zero).

                // Get the length of the first word of the digits array xd.
                for (digits = 1, k = xd[0]; k >= 10; k /= 10) digits++;
                i = sd.Value - digits;

                // Is the rounding digit in the first word of xd?
                if (i < 0)
                {
                    i += LOG_BASE;
                    j = sd.Value;
                    w = xd[xdi = 0];

                    // Get the rounding digit at index j of w.
                    rd = (long)(w / Math.Pow(10, digits - j - 1) % 10) | 0;
                }
                else
                {
                    xdi = (long)Math.Ceiling((double)(i + 1) / LOG_BASE);
                    k = xd.LongLength;
                    if (xdi >= k)
                    {
                        if (isTruncated)
                        {

                            // Needed by `naturalExponential`, `naturalLogarithm` and `squareRoot`.
                            for (; k++ <= xdi;) ArrayExtensions.Push(ref xd, 0);
                            w = rd = 0;
                            digits = 1;
                            i %= LOG_BASE;
                            j = i - LOG_BASE + 1;
                        }
                        else
                        {
                            goto @out;
                        }
                    }
                    else
                    {
                        w = k = xd[xdi];

                        // Get the number of digits of w.
                        for (digits = 1; k >= 10; k /= 10) digits++;

                        // Get the index of rd within w.
                        i %= LOG_BASE;

                        // Get the index of rd within w, adjusted for leading zeros.
                        // The number of leading zeros of w is given by BigDecimalFactory.LOG_BASE - digits.
                        j = i - LOG_BASE + digits;

                        // Get the rounding digit at index j of w.
                        rd = j < 0 ? 0 : (long)(w / Math.Pow(10, digits - j - 1) % 10) | 0;
                    }
                }

                // Are there any non-zero digits after the rounding digit?
                isTruncated = isTruncated || sd < 0 ||
                  xd.LongLength > xdi + 1 || (j < 0 ? w : w % Math.Pow(10, digits - j - 1)).IsTrue();

                // The expression `w % Math.Pow(10, digits - j - 1)` returns all the digits of w to the right
                // of the digit at (left-to-right) index j, e.g. if w is 908714 and j is 2, the expression
                // will give 714.

                roundUp = rm < RoundingMode.ROUND_HALF_UP
                  ? (rd.IsTrue() || isTruncated) && (rm == RoundingMode.ROUND_UP || rm == (x.s < 0 ? RoundingMode.ROUND_FLOOR : RoundingMode.ROUND_CEIL))
                  : rd > 5 || rd == 5 && (rm == RoundingMode.ROUND_HALF_UP || isTruncated || rm == RoundingMode.ROUND_HALF_EVEN &&

                    // Check whether the digit to the left of the rounding digit is odd.
                    ((long)((i > 0 ? j > 0 ? w / Math.Pow(10, digits - j) : 0 : xd.LongLength > xdi - 1 && xdi >= 1 ? xd[xdi - 1] : 0) % 10) & 1).IsTrue() ||
                      rm == (x.s < 0 ? RoundingMode.ROUND_HALF_FLOOR : RoundingMode.ROUND_HALF_CEIL));

                if (sd < 1 || !xd[0].IsTrue())
                {
                    ArrayExtensions.Resize(ref xd, 1);
                    if (roundUp)
                    {

                        // Convert sd to decimal places.
                        sd -= x.e + 1;

                        // 1, 0.1, 0.01, 0.001, 0.0001 etc.
                        xd[0] = (int)Math.Pow(10, (LOG_BASE - sd.Value % LOG_BASE) % LOG_BASE);
                        x.e = -sd ?? 0;
                    }
                    else
                    {

                        // Zero.
                        x.e = 0;
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
                    xd[xdi] = j > 0 ? ((int)(w / Math.Pow(10, digits - j) % Math.Pow(10, j)) | 0) * (int)k : 0;
                }

                if (roundUp)
                {
                    for (; ; )
                    {

                        // Is the digit to be rounded up in the first word of xd?
                        if (xdi == 0)
                        {

                            // i will be the length of xd[0] before k is added.
                            for (i = 1, j = xd[0]; j >= 10; j /= 10) i++;
                            j = xd[0] += (int)k;
                            for (k = 1; j >= 10; j /= 10) k++;

                            // if i != k the length has increased.
                            if (i != k)
                            {
                                x.e++;
                                if (xd[0] == BASE) xd[0] = 1;
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
                for (i = xd.LongLength; xd[--i] == 0;) ArrayExtensions.Pop(ref xd);

                x.d = xd;
            }

        @out:

            if (Ctor.external)
            {

                // Overflow?
                if (x.e > Ctor.MaxE)
                {

                    // Infinity.
                    x.d = null;
                    x.e = null;

                    // Underflow?
                }
                else if (x.e < Ctor.MinE)
                {

                    // Zero.
                    x.e = 0;
                    x.d = new int[] { 0 };
                    // Ctor.underflow = true;
                } // else Ctor.underflow = false;
            }

            return x;
        }


        internal static LongString finiteToString(BigDecimal x, bool isExp = false, long? sd = null)
        {
            if (!x.IsFinite()) return nonFiniteToString(x);
            long k;
            long e = x.e ?? 0;
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

                str = str + (x.e < 0 ? "e" : "e+") + x.e;
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

            return str;
        }


        /// <summary>
        /// Calculate the base 10 exponent from the base 1e7 exponent.
        /// </summary>
        /// <param name="digits"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        internal static long getBase10Exponent(int[] digits, long e)
        {
            var w = digits[0];

            // Add the number of digits of the first word of the digits array.
            for (e *= LOG_BASE; w >= 10; w /= 10) e++;
            return e;
        }


        internal static BigDecimal getLn10(BigDecimalConfig Ctor, long sd, long? pr = null)
        {
            if (sd > LN10_PRECISION)
            {

                // Reset global state in case the exception is caught.
                Ctor.external = true;
                if (pr.IsTrue()) Ctor.Precision = pr.Value;
                throw new BigDecimalException(BigDecimalException.PrecisionLimitExceeded);
            }
            return finalise(new BigDecimal(LN10, Ctor), sd, RoundingMode.ROUND_DOWN, true);
        }


        internal static BigDecimal getPi(BigDecimalConfig Ctor, long sd, RoundingMode rm)
        {
            if (sd > PI_PRECISION) throw new BigDecimalException(BigDecimalException.PrecisionLimitExceeded);
            return finalise(new BigDecimal(PI, Ctor), sd, rm, true);
        }


        internal static long getPrecision(int[] digits)
        {
            var w = digits.LongLength - 1;
            var len = w * LOG_BASE + 1;

            w = digits[w];

            // If non-zero...
            if (w.IsTrue())
            {

                // Subtract the number of trailing zeros of the last word.
                for (; w % 10 == 0; w /= 10) len--;

                // Add the number of digits of the first word.
                for (w = digits[0]; w >= 10; w /= 10) len++;
            }

            return len;
        }


        internal static LongString getZeroString(long k)
        {
            var zs = "";
            for (; k--.IsTrue();) zs += "0";
            return zs;
        }


        /// <summary>
        /// Implements "exponentiation by squaring". Called by `Pow` and `ParseOther`.
        /// </summary>
        /// <param name="Ctor"></param>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <param name="pr"></param>
        /// <returns>A new BigDecimal whose value is the value of BigDecimal `x` to the power `n`, where `n` is an
        /// integer of type number.</returns>
        internal static BigDecimal intPow(BigDecimalConfig Ctor, BigDecimal x, long n, long pr)
        {
            bool isTruncated = false;
            var r = new BigDecimal(1, Ctor);

            // Max n of 9007199254740991 takes 53 loop iterations.
            // Maximum digits array length; leaves [28, 34] guard digits.
            var k = (long)Math.Ceiling((double)pr / LOG_BASE + 4);

            Ctor.external = false;

            for (; ; )
            {
                if ((n % 2).IsTrue())
                {
                    r = r.Times(x);
                    if (truncate(ref r.d, k)) isTruncated = true;
                }

                n = (long)Math.Floor((double)n / 2);
                if (n == 0)
                {

                    // To ensure correct rounding when r.d is truncated, increment the last word if it is zero.
                    n = r.d.LongLength - 1;
                    if (isTruncated && r.d[n] == 0) ++r.d[n];
                    break;
                }

                x = x.Times(x);
                truncate(ref x.d, k);
            }

            Ctor.external = true;

            return r;
        }


        internal static bool isOdd(BigDecimal n)
        {
            return (n.d[n.d.LongLength - 1] & 1).IsTrue();
        }


        /// <summary>
        /// Handle `max` and `min`.
        /// </summary>
        /// <param name="Ctor"></param>
        /// <param name="ltgt">"Lt" or "Gt".</param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static BigDecimal maxOrMin(BigDecimalConfig Ctor, LtGt ltgt, params BigDecimal[] args)
        {
            BigDecimal y;
            var x = new BigDecimal(args[0], Ctor);
            var i = 0;

            for (; ++i < args.LongLength;)
            {
                y = args[i];
                bool ltgtResult;
                if (ltgt == LtGt.Lt) ltgtResult = x.Lt(y);
                else ltgtResult = x.Gt(y);
                if (!y.s.IsTrue())
                {
                    x = y;
                    break;
                }
                else if (ltgtResult)
                {
                    x = y;
                }
            }

            return x;
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
        /// Max integer argument: Exp("20723265836946413") = 6.3e+9000000000000000<br />
        /// Min integer argument: Exp("-20723265836946411") = 1.2e-9000000000000000<br />
        /// (Math object integer min/max: Math.exp(709) = 8.2e+307, Math.exp(-745) = 5e-324)<br /><br />
        /// Exp(x) is non-terminating for any finite, non-zero x.<br /><br />
        /// The result will always be correctly rounded.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="sd"></param>
        /// <returns>A new BigDecimal whose value is the natural exponential of `x` rounded to `sd` significant
        /// digits.</returns>
        internal static BigDecimal naturalExponential(BigDecimal x, long? sd = null)
        {
            BigDecimal denominator, pow, sum, t;
            long guard, j, wpr;
            var rep = 0;
            var i = 0;
            var k = 0;
            var Ctor = x.Config;
            var rm = Ctor.Rounding;
            var pr = Ctor.Precision;

            // 0/null/Infinity?
            if (!x.d.IsTrue() || !x.d[0].IsTrue() || x.e > 17)
            {
                if (x.d.IsTrue())
                {
                    if (!x.d[0].IsTrue()) return new BigDecimal(1, Ctor);
                    else if (x.s < 0) return new BigDecimal(0, Ctor);
                    else return new BigDecimal(double.PositiveInfinity, Ctor);
                }
                else
                {
                    if (x.s.IsTrue())
                    {
                        if (x.s < 0) return new BigDecimal(0, Ctor);
                        else return new BigDecimal(x, Ctor);
                    }
                    else return new BigDecimal(double.NaN, Ctor);
                }
            }

            if (sd == null)
            {
                Ctor.external = false;
                wpr = pr;
            }
            else
            {
                wpr = sd.Value;
            }

            t = new BigDecimal(0.03125, Ctor);

            // while Abs(x) >= 0.1
            while (x.e > -2)
            {

                // x = x / 2^5
                x = x.Times(t);
                k += 5;
            }

            // Use 2 * Log10(2^k) + 5 (empirically derived) to estimate the increase in precision
            // necessary to ensure the first 4 rounding digits are correct.
            guard = (long)(Math.Log(Math.Pow(2, k)) / LN10d * 2 + 5) | 0;
            wpr += guard;
            denominator = pow = sum = new BigDecimal(1, Ctor);
            Ctor.Precision = wpr;

            for (; ; )
            {
                pow = finalise(pow.Times(x), wpr, RoundingMode.ROUND_DOWN);
                denominator = denominator.Times(++i);
                t = sum.Plus(divide(pow, denominator, wpr, RoundingMode.ROUND_DOWN));

                if (digitsToString(t.d).Slice(0, wpr) == digitsToString(sum.d).Slice(0, wpr))
                {
                    j = k;
                    while (j--.IsTrue()) sum = finalise(sum.Times(sum), wpr, RoundingMode.ROUND_DOWN);

                    // Check to see if the first 4 rounding digits are [49]999.
                    // If so, repeat the summation with a higher precision, otherwise
                    // e.g. with precision: 18, rounding: 1
                    // Exp(18.404272462595034083567793919843761) = 98372560.1229999999 (should be 98372560.123)
                    // `wpr - guard` is the index of first rounding digit.
                    if (sd == null)
                    {

                        if (rep < 3 && checkRoundingDigits(sum.d, wpr - guard, rm, rep.IsTrue()))
                        {
                            Ctor.Precision = wpr += 10;
                            denominator = pow = t = new BigDecimal(1, Ctor);
                            i = 0;
                            rep++;
                        }
                        else
                        {
                            return finalise(sum, Ctor.Precision = pr, rm, Ctor.external = true);
                        }
                    }
                    else
                    {
                        Ctor.Precision = pr;
                        return sum;
                    }
                }

                sum = t;
            }
        }


        /// <summary>
        /// Ln(n) (n != 1) is non-terminating.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="sd"></param>
        /// <returns>A new BigDecimal whose value is the natural logarithm of `x` rounded to `sd` significant
        /// digits.</returns>
        internal static BigDecimal naturalLogarithm(BigDecimal y, long? sd = null)
        {
            LongString c;
            char c0;
            long denominator, e, rep = 0, wpr;
            BigDecimal numerator, sum, t, x1, x2;
            var n = 1;
            var guard = 10;
            var x = y;
            var xd = x.d;
            var Ctor = x.Config;
            var rm = Ctor.Rounding;
            var pr = Ctor.Precision;

            // Is x negative or Infinity, null, 0 or 1?
            if (x.s < 0 || !xd.IsTrue() || !xd[0].IsTrue() || !x.e.IsTrue() && xd[0] == 1 && xd.LongLength == 1)
            {
                if (xd.IsTrue() && !xd[0].IsTrue()) return new BigDecimal(double.NegativeInfinity, Ctor);
                else if (x.s != 1) return new BigDecimal(double.NaN, Ctor);
                else if (xd.IsTrue()) return new BigDecimal(0, Ctor);
                else return new BigDecimal(x, Ctor);
            }

            if (sd == null)
            {
                Ctor.external = false;
                wpr = pr;
            }
            else
            {
                wpr = sd.Value;
            }

            Ctor.Precision = wpr += guard;
            c = digitsToString(xd);
            c0 = c.ElementAt(0);

            if (Math.Abs(e = x.e ?? 0) < 1.5e15)
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

                e = x.e ?? 0;

                if ((int)(c0 - '0') > 1)
                {
                    x = new BigDecimal(("0." + c).ToString(), Ctor);
                    e++;
                }
                else
                {
                    x = new BigDecimal((c0 + "." + c.Slice(1)).ToString(), Ctor);
                }
            }
            else
            {

                // The argument reduction method above may result in overflow if the argument y is a massive
                // number with exponent >= 1500000000000000 (9e15 / 6 = 1.5e15), so instead recall this
                // function using Ln(x*10^e) = Ln(x) + e*ln(10).
                t = getLn10(Ctor, wpr + 2, pr).Times(e.ToString(CultureInfo.InvariantCulture));
                x = naturalLogarithm(new BigDecimal((c0 + "." + c.Slice(1)).ToString(), Ctor), wpr - guard).Plus(t);
                Ctor.Precision = pr;

                return sd == null ? finalise(x, pr, rm, Ctor.external = true) : x;
            }

            // x1 is x reduced to a value near 1.
            x1 = x;

            // Taylor series.
            // Ln(y) = Ln((1 + x)/(1 - x)) = 2(x + x^3/3 + x^5/5 + x^7/7 + ...)
            // where x = (y - 1)/(y + 1)    (|x| < 1)
            sum = numerator = x = divide(x.Minus(1), x.Plus(1), wpr, RoundingMode.ROUND_DOWN);
            x2 = finalise(x.Times(x), wpr, RoundingMode.ROUND_DOWN);
            denominator = 3;

            for (; ; )
            {
                numerator = finalise(numerator.Times(x2), wpr, RoundingMode.ROUND_DOWN);
                t = sum.Plus(divide(numerator, new BigDecimal(denominator, Ctor), wpr, RoundingMode.ROUND_DOWN));

                if (digitsToString(t.d).Slice(0, wpr) == digitsToString(sum.d).Slice(0, wpr))
                {
                    sum = sum.Times(2);

                    // Reverse the argument reduction. Check that e is not 0 because, besides preventing an
                    // unnecessary calculation, -0 + 0 = +0 and to ensure correct rounding -0 needs to stay -0.
                    if (e != 0) sum = sum.Plus(getLn10(Ctor, wpr + 2, pr).Times(e.ToString(CultureInfo.InvariantCulture)));
                    sum = divide(sum, new BigDecimal(n, Ctor), wpr, RoundingMode.ROUND_DOWN);

                    // Is rm > 3 and the first 4 rounding digits 4999, or rm < 4 (or the summation has
                    // been repeated previously) and the first 4 rounding digits 9999?
                    // If so, restart the summation with a higher precision, otherwise
                    // e.g. with precision: 12, rounding: 1
                    // Ln(135520028.6126091714265381533) = 18.7246299999 when it should be 18.72463.
                    // `wpr - guard` is the index of first rounding digit.
                    if (sd == null)
                    {
                        if (checkRoundingDigits(sum.d, wpr - guard, rm, rep.IsTrue()))
                        {
                            Ctor.Precision = wpr += guard;
                            t = numerator = x = divide(x1.Minus(1), x1.Plus(1), wpr, RoundingMode.ROUND_DOWN);
                            x2 = finalise(x.Times(x), wpr, RoundingMode.ROUND_DOWN);
                            denominator = rep = 1;
                        }
                        else
                        {
                            return finalise(sum, Ctor.Precision = pr, rm, Ctor.external = true);
                        }
                    }
                    else
                    {
                        Ctor.Precision = pr;
                        return sum;
                    }
                }

                sum = t;
                denominator += 2;
            }
        }


        /// <summary>
        /// ±Infinity, null.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static LongString nonFiniteToString(BigDecimal x)
        {
            // Unsigned.
            if (x.s == null) return double.NaN.ToString(CultureInfo.InvariantCulture);
            return ((double)x.s * (double)x.s / 0).ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Parse the value of a new BigDecimal `x` from string `str`.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static BigDecimal parseDecimal(BigDecimal x, LongString str)
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
                x.e = e > Ctor.MaxE ? Ctor.MaxE + 1 : e < Ctor.MinE ? Ctor.MinE - 1 : (long)e;
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

                if (Ctor.external)
                {

                    // Overflow?
                    if (x.e > Ctor.MaxE)
                    {

                        // Infinity.
                        x.d = null;
                        x.e = null;

                        // Underflow?
                    }
                    else if (x.e < Ctor.MinE)
                    {

                        // Zero.
                        x.e = 0;
                        x.d = new int[] { 0 };
                        // x.constructor.underflow = true;
                    } // else x.constructor.underflow = false;
                }
            }
            else
            {

                // Zero.
                x.e = 0;
                x.d = new int[] { 0 };
            }

            return x;
        }


        /// <summary>
        /// Parse the value of a new BigDecimal `x` from a string `str`, which is not a decimal value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="BigDecimalException"></exception>
        internal static BigDecimal parseOther(BigDecimal x, LongString str)
        {
            int @base;
            BigDecimalConfig Ctor;
            BigDecimal divisor = null;
            long i, len = 0, xe;
            BigInteger p = 0;
            bool isFloat;
            int[] xd;

            if (str.IndexOf("_") > -1)
            {
                str = Regex.Replace(str, @"(\d)_(?=\d)", "$1");
                if (Regex.IsMatch(str, isDecimal, RegexOptions.IgnoreCase)) return parseDecimal(x, str);
            }
            else if (str == "Infinity" || str == "NaN")
            {
                if (double.IsNaN(double.Parse(str, CultureInfo.InvariantCulture))) x.s = null;
                x.e = null;
                x.d = null;
                return x;
            }

            if (Regex.IsMatch(str, isHex, RegexOptions.IgnoreCase))
            {
                @base = 16;
                str = str.ToLowerInvariant();
            }
            else if (Regex.IsMatch(str, isBinary, RegexOptions.IgnoreCase))
            {
                @base = 2;
            }
            else if (Regex.IsMatch(str, isOctal, RegexOptions.IgnoreCase))
            {
                @base = 8;
            }
            else
            {
                throw new BigDecimalException(BigDecimalException.InvalidArgument + str);
            }

            // Is there a binary exponent part?
            i = str.IndexOf("p", StringComparison.InvariantCultureIgnoreCase);

            if (i > 0)
            {
                p = BigInteger.Parse(str.Slice(i + 1), CultureInfo.InvariantCulture);
                str = str.Slice(2, i);
            }
            else
            {
                str = str.Slice(2);
            }

            // Convert `str` as an integer then divide the result by `@base` raised to a power such that the
            // fraction part will be restored.
            i = str.IndexOf(".");
            isFloat = i >= 0;
            Ctor = x.Config;

            if (isFloat)
            {
                str = str.Replace(".", "");
                len = str.LongLength;
                i = len - i;

                // log[10](16) = 1.2041... , log[10](88) = 1.9444....
                divisor = intPow(Ctor, new BigDecimal(@base, Ctor), i, i * 2);
            }

            xd = convertBase(str, @base, BASE);
            xe = xd.LongLength - 1;

            // Remove trailing zeros.
            for (i = xe; i >= 0 && xd[i] == 0; --i) ArrayExtensions.Pop(ref xd);
            if (i < 0) return new BigDecimal(x.s >= 0 ? "+0" : "-0", Ctor);
            x.e = getBase10Exponent(xd, xe);
            x.d = xd;
            Ctor.external = false;

            // At what precision to perform the division to ensure exact conversion?
            // maxDecimalIntegerPartDigitCount = Ceil(log[10](b) * otherBaseIntegerPartDigitCount)
            // log[10](2) = 0.30103, log[10](8) = 0.90309, log[10](16) = 1.20412
            // E.g. Ceil(1.2 * 3) = 4, so up to 4 decimal digits are needed to represent 3 hex int digits.
            // maxDecimalFractionPartDigitCount = {Hex:4|Oct:3|Bin:1} * otherBaseFractionPartDigitCount
            // Therefore using 4 * the number of digits of str will always be enough.
            if (isFloat) x = divide(x, divisor, len * 4);

            // Multiply by the binary exponent part if present.
            if (p.IsTrue()) x = BigInteger.Abs(p) < 54 ? x.Times(Math.Pow(2, (double)p)) : x.Times(new BigDecimal(2, Ctor).Pow(p));
            Ctor.external = true;

            return x;
        }


        /// <summary>
        /// Sin(x) = x - x^3/3! + x^5/5! - ...<br />
        /// |x| &lt; pi/2
        /// </summary>
        /// <param name="Ctor"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static BigDecimal sine(BigDecimalConfig Ctor, BigDecimal x)
        {
            long k;
            var len = x.d.LongLength;

            if (len < 3)
            {
                return x.IsZero() ? x : taylorSeries(Ctor, 2, x, x);
            }

            // Argument reduction: Sin(5x) = 16*sin^5(x) - 20*sin^3(x) + 5*sin(x)
            // i.e. Sin(x) = 16*sin^5(x/5) - 20*sin^3(x/5) + 5*sin(x/5)
            // and  Sin(x) = Sin(x/5)(5 + sin^2(x/5)(16sin^2(x/5) - 20))

            // Estimate the optimum number of times to use the argument reduction.
            k = (long)(1.4 * Math.Sqrt(len));
            k = k > 16 ? 16 : k | 0;

            x = x.Times((double)1 / tinyPow(5, k));
            x = taylorSeries(Ctor, 2, x, x);

            // Reverse argument reduction
            BigDecimal sin2_x,
              d5 = new BigDecimal(5, Ctor),
              d16 = new BigDecimal(16, Ctor),
              d20 = new BigDecimal(20, Ctor);
            for (; k--.IsTrue();)
            {
                sin2_x = x.Times(x);
                x = x.Times(d5.Plus(sin2_x.Times(d16.Times(sin2_x).Minus(d20))));
            }

            return x;
        }


        /// <summary>
        /// Calculate Taylor series for `Cos`, `Cosh`, `Sin` and `Sinh`.
        /// </summary>
        /// <param name="Ctor"></param>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isHyperbolic"></param>
        /// <returns></returns>
        internal static BigDecimal taylorSeries(BigDecimalConfig Ctor, int n, BigDecimal x, BigDecimal y, bool isHyperbolic = false)
        {
            long j;
            BigDecimal t, u, x2;
            long i = 1;
            var pr = Ctor.Precision;
            var k = (long)Math.Ceiling((double)pr / LOG_BASE);

            Ctor.external = false;
            x2 = x.Times(x);
            u = new BigDecimal(y, Ctor);

            for (; ; )
            {
                t = divide(u.Times(x2), new BigDecimal(n++ * n++, Ctor), pr, RoundingMode.ROUND_DOWN);
                u = isHyperbolic ? y.Plus(t) : y.Minus(t);
                y = divide(t.Times(x2), new BigDecimal(n++ * n++, Ctor), pr, RoundingMode.ROUND_DOWN);
                t = u.Plus(y);

                if (t.d.LongLength > k && u.d.LongLength > k)
                {
                    for (j = k; t.d[j] == u.d[j] && j--.IsTrue();) ;
                    if (j == -1) break;
                }

                var j2 = u;
                u = y;
                y = t;
                t = j2;
                i++;
            }

            Ctor.external = true;
            ArrayExtensions.Resize(ref t.d, k + 1);

            return t;
        }


        /// <summary>
        /// Exponent e must be positive and non-zero.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="e">Must be positive and non-zero</param>
        /// <returns></returns>
        internal static long tinyPow(long b, long e)
        {
            var n = b;
            while ((--e).IsTrue()) n *= b;
            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ctor"></param>
        /// <param name="x"></param>
        /// <param name="quadrant"></param>
        /// <returns>The absolute value of `x` reduced to less than or equal to half pi.</returns>
        internal static BigDecimal toLessThanHalfPi(BigDecimalConfig Ctor, BigDecimal x, out int quadrant)
        {
            BigDecimal t;
            var isNeg = x.s < 0;
            var pi = getPi(Ctor, Ctor.Precision, RoundingMode.ROUND_DOWN);
            var halfPi = pi.Times(0.5);

            x = x.Abs();

            if (x.Lte(halfPi))
            {
                quadrant = isNeg ? 4 : 1;
                return x;
            }

            t = x.DivToInt(pi);

            if (t.IsZero())
            {
                quadrant = isNeg ? 3 : 2;
            }
            else
            {
                x = x.Minus(t.Times(pi));

                // 0 <= x < pi
                if (x.Lte(halfPi))
                {
                    quadrant = isOdd(t) ? (isNeg ? 2 : 3) : (isNeg ? 4 : 1);
                    return x;
                }

                quadrant = isOdd(t) ? (isNeg ? 1 : 4) : (isNeg ? 3 : 2);
            }

            return x.Minus(pi).Abs();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="baseOut"></param>
        /// <param name="sd"></param>
        /// <param name="rm"></param>
        /// <returns>The value of BigDecimal `x` as a string in base `baseOut`.<br />
        /// If the optional `sd` argument is present include a binary exponent suffix.</returns>
        internal static LongString toStringBinary(BigDecimal x, int @baseOut, long? sd, RoundingMode? rm)
        {
            int @base;
            long e, k, len;
            long? i;
            bool roundUp = false;
            LongString str;
            int[] xd;
            BigDecimal y = null;
            var Ctor = x.Config;
            var isExp = sd != null;
            long sdValue;

            if (isExp)
            {
                checkInt32(sd.Value, 1, MAX_DIGITS);
                if (rm == null) rm = Ctor.Rounding;

                sdValue = sd.Value;
            }
            else
            {
                sd = Ctor.Precision;
                rm = Ctor.Rounding;

                sdValue = sd.Value;
            }

            if (!x.IsFinite())
            {
                str = nonFiniteToString(x);
            }
            else
            {
                str = finiteToString(x);
                i = str.IndexOf(".");

                // Use exponential notation according to `toExpPos` and `toExpNeg`? No, but if required:
                // maxBinaryExponent = Floor((decimalExponent + 1) * log[2](10))
                // minBinaryExponent = Floor(decimalExponent * log[2](10))
                // log[2](10) = 3.321928094887362347870319429489390175864

                if (isExp)
                {
                    @base = 2;
                    if (@baseOut == 16)
                    {
                        sdValue = sdValue * 4 - 3;
                    }
                    else if (@baseOut == 8)
                    {
                        sdValue = sdValue * 3 - 2;
                    }
                }
                else
                {
                    @base = @baseOut;
                }

                // Convert the number as an integer then divide the result by its @base raised to a power such
                // that the fraction part will be restored.

                // Non-integer.
                if (i >= 0)
                {
                    str = str.Replace(".", "");
                    y = new BigDecimal(1, Ctor);
                    y.e = str.LongLength - i;
                    y.d = convertBase(finiteToString(y), 10, @base);
                    y.e = y.d.LongLength;
                }

                xd = convertBase(str, 10, @base);
                e = len = xd.LongLength;

                // Remove trailing zeros.
                for (; len > 0 && xd[--len] == 0;) ArrayExtensions.Pop(ref xd);

                if (xd.LongLength == 0 || !xd[0].IsTrue())
                {
                    str = isExp ? "0p+0" : "0";
                }
                else
                {
                    if (i < 0)
                    {
                        e--;
                    }
                    else
                    {
                        x = new BigDecimal(x, Ctor);
                        x.d = xd;
                        x.e = e;
                        x = divide(x, y, out var inexact, sdValue, rm, 0, @base);
                        xd = x.d;
                        e = x.e ?? 0;
                        roundUp = inexact;
                    }

                    // The rounding digit, i.e. the digit after the digit that may be rounded up.
                    i = sdValue < xd.LongLength ? xd[sdValue] : null;
                    k = @base / 2;
                    roundUp = roundUp || sdValue + 1 < xd.LongLength;

                    roundUp = rm < RoundingMode.ROUND_HALF_UP
                      ? (i != null || roundUp) && (rm == RoundingMode.ROUND_UP || rm == (x.s < 0 ? RoundingMode.ROUND_FLOOR : RoundingMode.ROUND_CEIL))
                      : i > k || i == k && (rm == RoundingMode.ROUND_HALF_UP || roundUp || rm == RoundingMode.ROUND_HALF_EVEN && (xd[sdValue - 1] & 1).IsTrue() ||
                        rm == (x.s < 0 ? RoundingMode.ROUND_HALF_FLOOR : RoundingMode.ROUND_HALF_CEIL));

                    ArrayExtensions.Resize(ref xd, sdValue);

                    if (roundUp)
                    {

                        // Rounding up may mean the previous digit has to be rounded up and so on.
                        for (; sdValue > 0 && ++xd[--sdValue] > @base - 1;)
                        {
                            xd[sdValue] = 0;
                            if (!sdValue.IsTrue())
                            {
                                ++e;
                                ArrayExtensions.Unshift(ref xd, 1);
                            }
                        }
                    }

                    // Determine trailing zeros.
                    for (len = xd.LongLength; !xd[len - 1].IsTrue(); --len) ;

                    // E.g. [4, 11, 15] becomes 4bf.
                    for (i = 0, str = ""; i < len; i++)
                    {
                        str += NUMERALS.ElementAt(xd[i.Value]);
                    }

                    // Add binary exponent suffix?
                    if (isExp)
                    {
                        if (len > 1)
                        {
                            if (@baseOut == 16 || @baseOut == 8)
                            {
                                i = @baseOut == 16 ? 4 : 3;
                                for (--len; (len % i).IsTrue(); len++) str += "0";
                                xd = convertBase(str, @base, @baseOut);
                                for (len = xd.LongLength; !xd[len - 1].IsTrue(); --len) ;

                                // xd[0] will always be be 1
                                for (i = 1, str = "1."; i < len; i++) str += NUMERALS.ElementAt(xd[i.Value]);
                            }
                            else
                            {
                                str = str.ElementAt(0) + "." + str.Slice(1);
                            }
                        }

                        str = str + (e < 0 ? "p" : "p+") + e;
                    }
                    else if (e < 0)
                    {
                        for (; (++e).IsTrue();) str = "0" + str;
                        str = "0." + str;
                    }
                    else
                    {
                        if (++e > len) for (e -= len; e--.IsTrue();) str += "0";
                        else if (e < len) str = str.Slice(0, e) + "." + str.Slice(e);
                    }
                }

                str = (@baseOut == 16 ? "0x" : @baseOut == 2 ? "0b" : @baseOut == 8 ? "0o" : "") + str;
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
