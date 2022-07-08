using DecimalSharp.Core;
using DecimalSharp.Core.Extensions;
using System.Numerics;
using System.Security.Cryptography;

namespace DecimalSharp
{
    public class BigDecimalFactory
    {
        public BigDecimalConfig Config { get; set; }

        public BigDecimalFactory(BigDecimalConfig? config = null)
        {
            this.Config = config ?? new BigDecimalConfig();
        }

        public BigDecimal BigDecimal(BigDecimalArgument<BigDecimal> v)
        {
            return new BigDecimal(v, this.Config);
        }

        // BigDecimalFactory methods


        /*
         *  abs
         *  acos
         *  acosh
         *  add
         *  asin
         *  asinh
         *  atan
         *  atanh
         *  atan2
         *  cbrt
         *  ceil
         *  clamp
         *  clone
         *  cos
         *  cosh
         *  div
         *  exp
         *  floor
         *  hypot
         *  ln
         *  log
         *  log2
         *  log10
         *  max
         *  min
         *  mod
         *  mul
         *  pow
         *  random
         *  round
         *  set
         *  sign
         *  sin
         *  sinh
         *  sqrt
         *  sub
         *  sum
         *  tan
         *  tanh
         *  trunc
         */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the absolute value of `x`.</returns>
        public BigDecimal Abs(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Abs();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the arccosine in radians of `x`.</returns>
        public BigDecimal Acos(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Acos();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">A value in radians.</param>
        /// <returns>A new BigDecimal whose value is the inverse of the hyperbolic cosine of `x`, rounded to
        /// `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Acosh(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Acosh();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is the sum of `x` and `y`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Add(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> y) => BigDecimal(x).Plus(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the arcsine in radians of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Asin(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Asin();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the inverse of the hyperbolic sine of `x`, rounded to
        /// `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Asinh(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Asinh();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the arctangent in radians of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Atan(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Atan();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">A value in radians.</param>
        /// <returns>A new BigDecimal whose value is the inverse of the hyperbolic tangent of `x`, rounded to
        /// `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Atanh(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Atanh();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <returns>A new BigDecimal whose value is the arctangent in radians of `y/x` in the range -pi to pi
        /// (inclusive), rounded to `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Atan2(BigDecimalArgument<BigDecimal> y, BigDecimalArgument<BigDecimal> x) => _atan2(BigDecimal(y), BigDecimal(x));
        private BigDecimal _atan2(BigDecimal y, BigDecimal x)
        {
            BigDecimal r;
            var pr = this.Config.Precision;
            var rm = this.Config.Rounding;
            var wpr = pr + 4;

            // Either null
            if (!y.s.IsTrue() || !x.s.IsTrue())
            {
                r = BigDecimal(double.NaN);

                // Both ±Infinity
            }
            else if (!y.d.IsTrue() && !x.d.IsTrue())
            {
                r = BigDecimalHelperFunctions.getPi(this.Config, wpr, RoundingMode.ROUND_DOWN).Times(x.s > 0 ? 0.25 : 0.75);
                r.s = y.s;

                // x is ±Infinity or y is ±0
            }
            else if (!x.d.IsTrue() || y.IsZero())
            {
                r = x.s < 0 ? BigDecimalHelperFunctions.getPi(this.Config, pr, rm) : BigDecimal(0);
                r.s = y.s;

                // y is ±Infinity or x is ±0
            }
            else if (!y.d.IsTrue() || x.IsZero())
            {
                r = BigDecimalHelperFunctions.getPi(this.Config, wpr, RoundingMode.ROUND_DOWN).Times(0.5);
                r.s = y.s;

                // Both non-zero and finite
            }
            else if (x.s < 0)
            {
                this.Config.Precision = wpr;
                this.Config.Rounding = RoundingMode.ROUND_DOWN;
                r = this.Atan(BigDecimalHelperFunctions.divide(y, x, wpr, RoundingMode.ROUND_DOWN));
                x = BigDecimalHelperFunctions.getPi(this.Config, wpr, RoundingMode.ROUND_DOWN);
                this.Config.Precision = pr;
                this.Config.Rounding = rm;
                r = y.s < 0 ? r.Minus(x) : r.Plus(x);
            }
            else
            {
                r = this.Atan(BigDecimalHelperFunctions.divide(y, x, wpr, RoundingMode.ROUND_DOWN));
            }

            return r;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the cube root of `x`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Cbrt(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Cbrt();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is `x` rounded to an integer using `ROUND_CEIL`.</returns>
        public BigDecimal Ceil(BigDecimalArgument<BigDecimal> x) => _ceil(BigDecimal(x));
        private BigDecimal _ceil(BigDecimal x)
        {
            return BigDecimalHelperFunctions.finalise(x, x.e + 1, RoundingMode.ROUND_CEIL);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>A new BigDecimal whose value is `x` clamped to the range delineated by `min` and `max`.</returns>
        public BigDecimal Clamp(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> min, BigDecimalArgument<BigDecimal> max) => BigDecimal(x).Clamp(min, max);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">A value in radians.</param>
        /// <returns>A new BigDecimal whose value is the cosine of `x`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Cos(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Cos();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">A value in radians.</param>
        /// <returns>A new BigDecimal whose value is the hyperbolic cosine of `x`, rounded to precision
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Cosh(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Cosh();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimalFactory with the same configuration properties as this BigDecimalFactory.</returns>
        public BigDecimalFactory Clone()
        {
            return new BigDecimalFactory(this.Config.Clone());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is `x` divided by `y`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Div(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> y) => BigDecimal(x).Div(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The power to which to raise the base of the natural log.</param>
        /// <returns>A new BigDecimal whose value is the natural exponential of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Exp(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Exp();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is `x` round to an integer using `ROUND_FLOOR`.</returns>
        public BigDecimal Floor(BigDecimalArgument<BigDecimal> x) => _floor(BigDecimal(x));
        private BigDecimal _floor(BigDecimal x)
        {
            return BigDecimalHelperFunctions.finalise(x, x.e + 1, RoundingMode.ROUND_FLOOR);
        }


        /// <summary>
        /// Hypot(a, b, ...) = Sqrt(a^2 + b^2 + ...)
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A new BigDecimal whose value is the square root of the sum of the squares of the arguments,
        /// rounded to `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Hypot(params BigDecimalArgument<BigDecimal>[] args) => _hypot(args.Select(arg => BigDecimal(arg)).ToArray());
        private BigDecimal _hypot(params BigDecimal[] args)
        {
            long i;
            BigDecimal n, t = BigDecimal(0);

            this.Config.external = false;

            for (i = 0; i < args.LongLength;)
            {
                n = args[i++];
                if (!n.d.IsTrue())
                {
                    if (n.s.IsTrue())
                    {
                        this.Config.external = true;
                        return BigDecimal(double.PositiveInfinity);
                    }
                    t = n;
                }
                else if (t.d.IsTrue())
                {
                    t = t.Plus(n.Times(n));
                }
            }

            this.Config.external = true;

            return t.Sqrt();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if object is a BigDecimal instance, otherwise false.</returns>
        public static bool IsDecimalInstance(object obj)
        {
            return obj is BigDecimal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the natural logarithm of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Ln(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Ln();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The argument of the logarithm.</param>
        /// <param name="y">The base of the logarithm.</param>
        /// <returns>A new BigDecimal whose value is the log of `x` to the base `y`, or to base 10 if no base
        /// is specified, rounded to `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Log(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> y) => BigDecimal(x).Log(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the base 2 logarithm of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Log2(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Log(2);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the base 10 logarithm of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Log10(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Log(10);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A new BigDecimal whose value is the maximum of the arguments.</returns>
        public BigDecimal Max(params double[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Max" />
        public BigDecimal Max(params decimal[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Max" />
        public BigDecimal Max(params long[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Max" />
        public BigDecimal Max(params int[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Max" />
        public BigDecimal Max(params string[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Max" />
        public BigDecimal Max(params BigInteger[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Max" />
        public BigDecimal Max(params BigDecimal[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Max" />
        public BigDecimal Max(params BigDecimalArgument<BigDecimal>[] args) => _max(args.Select(arg => BigDecimal(arg)).ToArray());
        private BigDecimal _max(params BigDecimal[] args)
        {
            return BigDecimalHelperFunctions.maxOrMin(this.Config, LtGt.Lt, args);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A new BigDecimal whose value is the minimum of the arguments.</returns>
        public BigDecimal Min(params double[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Min" />
        public BigDecimal Min(params decimal[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Min" />
        public BigDecimal Min(params long[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Min" />
        public BigDecimal Min(params int[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Min" />
        public BigDecimal Min(params string[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Min" />
        public BigDecimal Min(params BigInteger[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Min" />
        public BigDecimal Min(params BigDecimal[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Min" />
        public BigDecimal Min(params BigDecimalArgument<BigDecimal>[] args) => _min(args.Select(arg => BigDecimal(arg)).ToArray());
        private BigDecimal _min(params BigDecimal[] args)
        {
            return BigDecimalHelperFunctions.maxOrMin(this.Config, LtGt.Gt, args);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is `x` modulo `y`, rounded to `precision` significant digits
        /// using rounding mode `rounding`.</returns>
        public BigDecimal Mod(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> y) => BigDecimal(x).Mod(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is `x` multiplied by `y`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Mul(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> y) => BigDecimal(x).Mul(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The base.</param>
        /// <param name="y">The exponent.</param>
        /// <returns>A new BigDecimal whose value is `x` raised to the power `y`, rounded to precision
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Pow(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> y) => BigDecimal(x).Pow(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <returns>A new BigDecimal with a random value equal to or greater than 0 and less than 1, and with
        /// `sd`, or `BigDecimalFactory.Config.Precision` if `sd` is omitted, significant digits(or less if trailing zeros
        /// are produced).</returns>
        public BigDecimal Random(long? sd = null)
        {
            byte[] d;
            long e, k, n;
            long i = 0;
            var r = BigDecimal(1);
            var rd = new int[0];

            if (sd == null) sd = this.Config.Precision;
            else BigDecimalHelperFunctions.checkInt32(sd.Value, 1, BigDecimalHelperFunctions.MAX_DIGITS);

            k = (long)Math.Ceiling((double)sd.Value / BigDecimalHelperFunctions.LOG_BASE);

            if (!this.Config.Crypto)
            {
                Random random = new Random();
                for (; i < k;)
                {
                    if (rd.LongLength <= i) ArrayExtensions.Resize(ref rd, i + 1);
                    rd[i++] = (int)(random.NextDouble() * 1e7) | 0;
                }
            }
            else
            {
                // buffer
                d = new byte[k *= 4];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(d);

                    for (; i < k;)
                    {

                        // 0 <= n < 2147483648
                        n = d[i] + (d[i + 1] << 8) + (d[i + 2] << 16) + ((d[i + 3] & 0x7f) << 24);

                        // Probability n >= 2.14e9, is 7483648 / 2147483648 = 0.0035 (1 in 286).
                        if (n >= 2.14e9)
                        {
                            byte[] temp = new byte[4];
                            rng.GetBytes(temp);
                            Array.Copy(temp, 0, d, i, 4);
                        }
                        else
                        {

                            // 0 <= n <= 2139999999
                            // 0 <= (n % 1e7) <= 9999999
                            ArrayExtensions.Push(ref rd, (int)(n % 1e7));
                            i += 4;
                        }
                    }
                }

                i = k / 4;
            }

            k = rd[--i];
            sd %= BigDecimalHelperFunctions.LOG_BASE;

            // Convert trailing digits to zeros according to sd.
            if (k.IsTrue() && sd.IsTrue())
            {
                n = (long)Math.Pow(10, BigDecimalHelperFunctions.LOG_BASE - sd.Value);
                rd[i] = (int)(((k / n) | 0) * n);
            }

            // Remove trailing words which are zero.
            for (; i >= 0 && rd[i] == 0; i--) ArrayExtensions.Pop(ref rd);

            // Zero?
            if (i < 0)
            {
                e = 0;
                rd = new[] { 0 };
            }
            else
            {
                e = -1;

                // Remove leading words which are zero and adjust exponent accordingly.
                for (; rd[0] == 0; e -= BigDecimalHelperFunctions.LOG_BASE) ArrayExtensions.Shift(ref rd);

                // Count the digits of the first word of rd to determine leading zeros.
                for (k = 1, n = rd[0]; n >= 10; n /= 10) k++;

                // Adjust the exponent for leading zeros of the first word of rd.
                if (k < BigDecimalHelperFunctions.LOG_BASE) e -= BigDecimalHelperFunctions.LOG_BASE - k;
            }

            r.e = e;
            r.d = rd;

            return r;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is `x` rounded to an integer using rounding mode `rounding`.</returns>
        public BigDecimal Round(BigDecimalArgument<BigDecimal> x) => _round(BigDecimal(x));
        private BigDecimal _round(BigDecimal x)
        {
            return BigDecimalHelperFunctions.finalise(x, x.e + 1, this.Config.Rounding);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>1 if x &gt; 0,
        /// <br />-1 if x &lt; 0,
        /// <br />0 if x is 0,
        /// <br />-0 if x is -0,
        /// <br />null otherwise.</returns>
        public int? Sign(BigDecimalArgument<BigDecimal> x) => _sign(BigDecimal(x));
        private int? _sign(BigDecimal x)
        {
            return x.d.IsTrue() ? (x.d[0].IsTrue() ? x.s : 0 * x.s) : x.s ?? null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the sine of `x`, rounded to `precision` significant digits
        /// using rounding mode `rounding`.</returns>
        public BigDecimal Sin(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Sin();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">A value in radians.</param>
        /// <returns>A new BigDecimal whose value is the hyperbolic sine of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Sinh(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Sinh();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is the square root of `x`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Sqrt(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Sqrt();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is `x` minus `y`, rounded to `precision` significant digits
        /// using rounding mode `rounding`.</returns>
        public BigDecimal Sub(BigDecimalArgument<BigDecimal> x, BigDecimalArgument<BigDecimal> y) => BigDecimal(x).Sub(y);


        /// <summary>
        /// Only the result is rounded, not the intermediate calculations.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A new BigDecimal whose value is the sum of the arguments, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Sum(params double[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Sum" />
        public BigDecimal Sum(params decimal[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Sum" />
        public BigDecimal Sum(params long[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Sum" />
        public BigDecimal Sum(params int[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Sum" />
        public BigDecimal Sum(params string[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Sum" />
        public BigDecimal Sum(params BigInteger[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Sum" />
        public BigDecimal Sum(params BigDecimal[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        /// <inheritdoc cref="Sum" />
        public BigDecimal Sum(params BigDecimalArgument<BigDecimal>[] args) => _sum(args.Select(arg => BigDecimal(arg)).ToArray());
        private BigDecimal _sum(params BigDecimal[] args)
        {
            long i = 0;
            var x = args[i];

            this.Config.external = false;
            for (; x.s.IsTrue() && ++i < args.LongLength;) x = x.Plus(args[i]);
            this.Config.external = true;

            return BigDecimalHelperFunctions.finalise(x, this.Config.Precision, this.Config.Rounding);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">A value in radians.</param>
        /// <returns>A new BigDecimal whose value is the tangent of `x`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Tan(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Tan();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">A value in radians.</param>
        /// <returns>A new BigDecimal whose value is the hyperbolic tangent of `x`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Tanh(BigDecimalArgument<BigDecimal> x) => BigDecimal(x).Tanh();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns>A new BigDecimal whose value is `x` truncated to an integer.</returns>
        public BigDecimal Trunc(BigDecimalArgument<BigDecimal> x) => _trunc(BigDecimal(x));
        private BigDecimal _trunc(BigDecimal x)
        {
            return BigDecimalHelperFunctions.finalise(x, x.e + 1, RoundingMode.ROUND_DOWN);
        }
    }
}
