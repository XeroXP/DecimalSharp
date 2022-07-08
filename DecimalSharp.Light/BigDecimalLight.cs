using DecimalSharp.Core;
using DecimalSharp.Core.Extensions;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace DecimalSharp
{
    public class BigDecimalLight
    {
        public BigDecimalLightConfig Config;

        public int[] d;
        public int s;
        public long e;

        protected BigDecimalLight(BigDecimalLightConfig? config = null)
        {
            this.Config = config ?? new BigDecimalLightConfig();

            this.s = 0;
            this.e = 0;
            this.d = new int[0];
        }
        public BigDecimalLight(BigDecimalArgument<BigDecimalLight> v, BigDecimalLightConfig? config = null) : this(config)
        {
            v.Switch(
                @double => _BigDecimalLight(@double),
                @decimal => _BigDecimalLight(@decimal),
                @long => _BigDecimalLight(@long),
                @int => _BigDecimalLight(@int),
                @string => _BigDecimalLight(@string),
                bigInteger => _BigDecimalLight(bigInteger.ToString(CultureInfo.InvariantCulture)),
                bigDecimal => _BigDecimalLight(bigDecimal)
            );
        }
        internal void _BigDecimalLight(string? v)
        {
            if (v == null)
                throw new BigDecimalException(BigDecimalException.InvalidArgument + v);

            int? i = null;
            // Minus sign?
            if (v.Length > 0 && (i = v.ElementAt(0)) == 45)
            {
                v = v.Slice(1);
                this.s = -1;
            }
            else
            {
                this.s = 1;
            }

            var isDec = Regex.IsMatch(v, BigDecimalLightHelperFunctions.isDecimal, RegexOptions.IgnoreCase);
            if (isDec)
            {
                _BigDecimalLight(BigDecimalLightHelperFunctions.parseDecimal(this, v));
            }
            else
            {
                throw new BigDecimalException(BigDecimalException.InvalidArgument + v);
            }
        }
        internal void _BigDecimalLight(int v)
        {
            if (v > 0)
            {
                this.s = 1;
            }
            else if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 0;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            // Fast path for small integers.
            if (v == ~~v && v < 1e7)
            {
                this.e = 0;
                this.d = new int[] { v };
                return;
            }

            _BigDecimalLight(BigDecimalLightHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }
        internal void _BigDecimalLight(long v)
        {
            if (v > 0)
            {
                this.s = 1;
            }
            else if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 0;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            // Fast path for small integers.
            if (v == ~~v && v < 1e7)
            {
                this.e = 0;
                this.d = new int[] { (int)v };
                return;
            }

            _BigDecimalLight(BigDecimalLightHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }

        internal void _BigDecimalLight(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
                throw new BigDecimalException(BigDecimalException.InvalidArgument + v);

            if (v > 0)
            {
                this.s = 1;
            }
            else if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 0;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            _BigDecimalLight(BigDecimalLightHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }

        internal void _BigDecimalLight(decimal v)
        {
            if (v > 0)
            {
                this.s = 1;
            }
            else if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 0;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            _BigDecimalLight(BigDecimalLightHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }
        internal void _BigDecimalLight(BigDecimalLight? v)
        {
            if (v == null)
                throw new BigDecimalException(BigDecimalException.InvalidArgument + v);

            this.s = v.s;
            this.e = v.e;
            this.d = v.d.IsTrue() ? v.d.Slice() : v.d;
        }


        // Decimal prototype methods


        /*
         *  absoluteValue                       abs
         *  comparedTo                          cmp
         *  decimalPlaces                       dp
         *  dividedBy                           div
         *  dividedToIntegerBy                  idiv
         *  equals                              eq
         *  exponent
         *  greaterThan                         gt
         *  greaterThanOrEqualTo                gte
         *  isInteger                           isint
         *  isNegative                          isneg
         *  isPositive                          ispos
         *  isZero
         *  lessThan                            lt
         *  lessThanOrEqualTo                   lte
         *  logarithm                           log
         *  minus                               sub
         *  modulo                              mod
         *  naturalExponential                  exp
         *  naturalLogarithm                    ln
         *  negated                             neg
         *  plus                                add
         *  precision                           sd
         *  squareRoot                          sqrt
         *  times                               mul
         *  toDecimalPlaces                     todp
         *  toExponential
         *  toFixed
         *  toInteger                           toint
         *  toNumber
         *  toPower                             pow
         *  toPrecision
         *  toSignificantDigits                 tosd
         *  toString
         *  valueOf                             val
         */


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimalLight whose value is the absolute value of this BigDecimalLight.</returns>
        public BigDecimalLight Abs()
        {
            var x = new BigDecimalLight(this, this.Config);
            if (x.s.IsTrue()) x.s = 1;
            return x;
        }
        /// <inheritdoc cref="Abs" />
        public BigDecimalLight AbsoluteValue() => Abs();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>1 if the value of this BigDecimalLight is greater than the value of `y`,
        /// <br />-1 if the value of this BigDecimalLight is less than the value of `y`,
        /// <br />0 if they have the same value.</returns>
        public int Cmp(BigDecimalArgument<BigDecimalLight> y) => _cmp(new BigDecimalLight(y, this.Config));
        private int _cmp(BigDecimalLight y)
        {
            long i, j, xdL, ydL;
            var x = this;

            // Signs differ?
            if (x.s != y.s) return x.s.IsTrue() ? x.s : -y.s;

            // Compare exponents.
            if (x.e != y.e) return x.e > y.e ^ x.s < 0 ? 1 : -1;

            xdL = x.d.LongLength;
            ydL = y.d.LongLength;

            // Compare digit by digit.
            for (i = 0, j = xdL < ydL ? xdL : ydL; i < j; ++i)
            {
                if (x.d[i] != y.d[i]) return x.d[i] > y.d[i] ^ x.s < 0 ? 1 : -1;
            }

            // Compare lengths.
            return xdL == ydL ? 0 : xdL > ydL ^ x.s < 0 ? 1 : -1;
        }
        /// <inheritdoc cref="Cmp" />
        public int ComparedTo(BigDecimalArgument<BigDecimalLight> y) => Cmp(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>The number of decimal places of the value of this BigDecimalLight.</returns>
        public long Dp()
        {
            var x = this;
            long w = x.d.LongLength - 1;
            var dp = (w - x.e) * BigDecimalLightHelperFunctions.LOG_BASE;

            // Subtract the number of trailing zeros of the last word.
            w = x.d[w];
            if (w.IsTrue()) for (; w % 10 == 0; w /= 10) dp--;

            return dp < 0 ? 0 : dp;
        }
        /// <inheritdoc cref="Dp" />
        public long DecimalPlaces() => Dp();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimalLight divided by `y`, truncated to
        /// `precision` significant digits.</returns>
        public BigDecimalLight Div(BigDecimalArgument<BigDecimalLight> y) => _div(new BigDecimalLight(y, this.Config));
        private BigDecimalLight _div(BigDecimalLight y)
        {
            return BigDecimalLightHelperFunctions.divide(this, y);
        }
        /// <inheritdoc cref="Div" />
        public BigDecimalLight DividedBy(BigDecimalArgument<BigDecimalLight> y) => Div(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimalLight whose value is the integer part of dividing the value of this BigDecimalLight
        /// by the value of `y`, truncated to `precision` significant digits.</returns>
        public BigDecimalLight Idiv(BigDecimalArgument<BigDecimalLight> y) => _idiv(new BigDecimalLight(y, this.Config));
        private BigDecimalLight _idiv(BigDecimalLight y)
        {
            var x = this;
            var Ctor = x.Config;
            return BigDecimalLightHelperFunctions.round(BigDecimalLightHelperFunctions.divide(x, y, 0, 1), Ctor.Precision);
        }
        /// <inheritdoc cref="Idiv" />
        public BigDecimalLight DividedToIntegerBy(BigDecimalArgument<BigDecimalLight> y) => Idiv(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimalLight is equal to the value of `y`, otherwise false.</returns>
        public bool Eq(BigDecimalArgument<BigDecimalLight> y)
        {
            return !this.Cmp(y).IsTrue();
        }
        /// <inheritdoc cref="Eq" />
        public bool Equals(BigDecimalArgument<BigDecimalLight> y) => Eq(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>The (base 10) exponent value of this BigDecimalLight (this.e is the base 10000000 exponent).</returns>
        public long Exponent()
        {
            return BigDecimalLightHelperFunctions.getBase10Exponent(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimalLight is greater than the value of `y`, otherwise
        /// false.</returns>
        public bool Gt(BigDecimalArgument<BigDecimalLight> y)
        {
            return this.Cmp(y) > 0;
        }
        /// <inheritdoc cref="Gt" />
        public bool GreaterThan(BigDecimalArgument<BigDecimalLight> y) => Gt(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimalLight is greater than or equal to the value of `y`,
        /// otherwise false.</returns>
        public bool Gte(BigDecimalArgument<BigDecimalLight> y)
        {
            return this.Cmp(y) >= 0;
        }
        /// <inheritdoc cref="Gte" />
        public bool GreaterThanOrEqualTo(BigDecimalArgument<BigDecimalLight> y) => Gte(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimalLight is an integer, otherwise false.</returns>
        public bool IsInt()
        {
            return this.e > this.d.LongLength - 2;
        }
        public bool IsInteger() => IsInt();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimalLight is negative, otherwise false.</returns>
        public bool IsNeg()
        {
            return this.s < 0;
        }
        /// <inheritdoc cref="IsNeg" />
        public bool IsNegative() => IsNeg();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimalLight is positive, otherwise false.</returns>
        public bool IsPos()
        {
            return this.s > 0;
        }
        /// <inheritdoc cref="IsPos" />
        public bool IsPositive() => IsPos();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimalLight is 0, otherwise false.</returns>
        public bool IsZero()
        {
            return this.s == 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimalLight is less than `y`, otherwise false.</returns>
        public bool Lt(BigDecimalArgument<BigDecimalLight> y)
        {
            return this.Cmp(y) < 0;
        }
        /// <inheritdoc cref="Lt" />
        public bool LessThan(BigDecimalArgument<BigDecimalLight> y) => Lt(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimalLight is less than or equal to `y`, otherwise false.</returns>
        public bool Lte(BigDecimalArgument<BigDecimalLight> y)
        {
            return this.Cmp(y) < 1;
        }
        /// <inheritdoc cref="Lte" />
        public bool LessThanOrEqualTo(BigDecimalArgument<BigDecimalLight> y) => Lte(y);


        /// <summary>
        /// The maximum error of the result is 1 ulp (unit in the last place).
        /// </summary>
        /// <param name="base">The base of the logarithm. Default = 10.</param>
        /// <returns>The logarithm of the value of this BigDecimalLight to the specified base, truncated to
        /// `precision` significant digits.<br /><br />
        /// If no base is specified, default base = 10 used.</returns>
        public BigDecimalLight Log(BigDecimalArgument<BigDecimalLight> @base)
        {
            return @base.Match(
                @double => _log(new BigDecimalLight(@double, this.Config)),
                @decimal => _log(new BigDecimalLight(@decimal, this.Config)),
                @long => _log(new BigDecimalLight(@long, this.Config)),
                @int => _log(new BigDecimalLight(@int, this.Config)),
                @string =>
                {
                    if (string.IsNullOrEmpty(@string)) return _log(new BigDecimalLight(10, this.Config));
                    return _log(new BigDecimalLight(@string, this.Config));
                },
                bigInteger => _log(new BigDecimalLight(bigInteger, this.Config)),
                bigDecimal => {
                    if (bigDecimal == null) return _log(new BigDecimalLight(10, this.Config));
                    return _log(new BigDecimalLight(bigDecimal, this.Config));
                }
            );
        }
        private BigDecimalLight _log(BigDecimalLight @base)
        {
            BigDecimalLight r;
            var x = this;
            var Ctor = x.Config;
            var pr = Ctor.Precision;
            var wpr = pr + 5;

            // log[-b](x) = NaN
            // log[0](x)  = NaN
            // log[1](x)  = NaN
            if (@base.s < 1 || @base.Eq(BigDecimalLightHelperFunctions.ONE))
                throw new BigDecimalException(BigDecimalException.DecimalError + "NaN");

            // log[b](-x) = NaN
            // log[b](0) = -Infinity
            if (x.s < 1) throw new BigDecimalException(BigDecimalException.DecimalError + (x.s.IsTrue() ? "NaN" : "-Infinity"));

            // log[b](1) = 0
            if (x.Eq(BigDecimalLightHelperFunctions.ONE)) return new BigDecimalLight(0, Ctor);

            Ctor.external = false;
            r = BigDecimalLightHelperFunctions.divide(BigDecimalLightHelperFunctions.ln(x, wpr), BigDecimalLightHelperFunctions.ln(@base, wpr), wpr);
            Ctor.external = true;

            return BigDecimalLightHelperFunctions.round(r, pr);
        }
        /// <inheritdoc cref="Log" />
        public BigDecimalLight Logarithm(BigDecimalArgument<BigDecimalLight> @base) => Log(@base);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimalLight minus `y`, truncated to
        /// `precision` significant digits.</returns>
        public BigDecimalLight Sub(BigDecimalArgument<BigDecimalLight> y) => _sub(new BigDecimalLight(y, this.Config));
        private BigDecimalLight _sub(BigDecimalLight y)
        {
            var x = this;

            if (x.s == y.s) return BigDecimalLightHelperFunctions.subtract(x, y);
            else
            {
                y.s = -y.s;
                return BigDecimalLightHelperFunctions.add(x, y);
            }
        }
        /// <inheritdoc cref="Sub" />
        public BigDecimalLight Minus(BigDecimalArgument<BigDecimalLight> y) => Sub(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimalLight modulo `y`, truncated to
        /// `precision` significant digits.</returns>
        public BigDecimalLight Mod(BigDecimalArgument<BigDecimalLight> y) => _mod(new BigDecimalLight(y, this.Config));
        private BigDecimalLight _mod(BigDecimalLight y)
        {
            BigDecimalLight q;
            var x = this;
            var Ctor = x.Config;
            var pr = Ctor.Precision;

            // x % 0 = NaN
            if (!y.s.IsTrue()) throw new BigDecimalException(BigDecimalException.DecimalError + "NaN");

            // Return x if x is 0.
            if (!x.s.IsTrue()) return BigDecimalLightHelperFunctions.round(new BigDecimalLight(x, Ctor), pr);

            // Prevent rounding of intermediate calculations.
            Ctor.external = false;
            q = BigDecimalLightHelperFunctions.divide(x, y, 0, 1).Times(y);
            Ctor.external = true;

            return x.Minus(q);
        }
        /// <inheritdoc cref="Mod" />
        public BigDecimalLight Modulo(BigDecimalArgument<BigDecimalLight> y) => Mod(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimalLight whose value is the natural exponential of the value of this BigDecimalLight,
        /// i.e. the base e raised to the power the value of this BigDecimalLight, truncated to `precision`
        /// significant digits.</returns>
        public BigDecimalLight Exp()
        {
            return BigDecimalLightHelperFunctions.exp(this);
        }
        /// <inheritdoc cref="Exp" />
        public BigDecimalLight NaturalExponential() => Exp();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimalLight whose value is the natural logarithm of the value of this BigDecimalLight,
        /// truncated to `precision` significant digits.</returns>
        public BigDecimalLight Ln()
        {
            return BigDecimalLightHelperFunctions.ln(this);
        }
        public BigDecimalLight NaturalLogarithm() => Ln();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>a new BigDecimalLight whose value is the value of this BigDecimalLight negated, i.e. as if multiplied by
        /// -1.</returns>
        public BigDecimalLight Neg()
        {
            var x = new BigDecimalLight(this, this.Config);
            x.s = -x.s;
            return x;
        }
        /// <inheritdoc cref="Neg" />
        public BigDecimalLight Negated() => Neg();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimalLight plus `y`, truncated to
        /// `precision` significant digits.</returns>
        public BigDecimalLight Add(BigDecimalArgument<BigDecimalLight> y) => _add(new BigDecimalLight(y, this.Config));
        private BigDecimalLight _add(BigDecimalLight y)
        {
            var x = this;
            if (x.s == y.s) return BigDecimalLightHelperFunctions.add(x, y);
            else
            {
                y.s = -y.s;
                return BigDecimalLightHelperFunctions.subtract(x, y);
            }
        }
        /// <inheritdoc cref="Add" />
        public BigDecimalLight Plus(BigDecimalArgument<BigDecimalLight> y) => Add(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="z">Whether to count integer-part trailing zeros.</param>
        /// <returns>The number of significant digits of the value of this BigDecimalLight.</returns>
        public long Sd(bool? z = null) => Sd(z.HasValue && z.Value ? 1 : 0);
        /// <inheritdoc cref="Sd" />
        public long Sd(long? z)
        {
            long e, sd, w;
            var x = this;

            if (z != null && z != 1 && z != 0) throw new BigDecimalException(BigDecimalException.InvalidArgument + z);

            e = BigDecimalLightHelperFunctions.getBase10Exponent(x) + 1;
            w = x.d.LongLength - 1;
            sd = w * BigDecimalLightHelperFunctions.LOG_BASE + 1;
            w = x.d[w];

            // If non-zero...
            if (w.IsTrue())
            {

                // Subtract the number of trailing zeros of the last word.
                for (; w % 10 == 0; w /= 10) sd--;

                // Add the number of digits of the first word.
                for (w = x.d[0]; w >= 10; w /= 10) sd++;
            }

            return z.IsTrue() && e > sd ? e : sd;
        }
        /// <inheritdoc cref="Sd" />
        public long Precision(bool? z = null) => Sd(z);
        /// <inheritdoc cref="Sd" />
        public long Precision(long? z) => Sd(z);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimalLight whose value is the square root of this BigDecimalLight, truncated to `precision`
        /// significant digits.</returns>
        public BigDecimalLight Sqrt()
        {
            LongString n;
            long e, pr, wpr;
            double s;
            BigDecimalLight r, t;
            var x = this;
            var Ctor = x.Config;

            // Negative or zero?
            if (x.s < 1)
            {
                if (!x.s.IsTrue()) return new BigDecimalLight(0, Ctor);

                // sqrt(-x) = NaN
                throw new BigDecimalException(BigDecimalException.DecimalError + "NaN");
            }

            e = BigDecimalLightHelperFunctions.getBase10Exponent(x);
            Ctor.external = false;

            // Initial estimate.
            s = Math.Sqrt(x.ToNumber());

            // Math.sqrt underflow/overflow?
            // Pass x to Math.sqrt as integer, then adjust the exponent of the result.
            if (s == 0 || s == double.PositiveInfinity)
            {
                n = BigDecimalLightHelperFunctions.digitsToString(x.d);
                if ((n.LongLength + e) % 2 == 0) n += '0';
                s = Math.Sqrt(double.Parse(n, CultureInfo.InvariantCulture));
                e = (long)Math.Floor(((double)e + 1) / 2) - ((e < 0 || (e % 2).IsTrue()) ? 1 : 0);

                if (s == double.PositiveInfinity)
                {
                    n = "5e" + e;
                }
                else
                {
                    n = s.ToExponential();
                    n = n.Slice(0, n.IndexOf('e') + 1) + e;
                }

                r = new BigDecimalLight(n.ToString(), Ctor);
            }
            else
            {
                r = new BigDecimalLight(s.ToString(CultureInfo.InvariantCulture), Ctor);
            }

            pr = Ctor.Precision;
            s = wpr = pr + 3;

            // Newton-Raphson iteration.
            for (; ; )
            {
                t = r;
                r = t.Plus(BigDecimalLightHelperFunctions.divide(x, t, wpr + 2)).Times(0.5);

                if (BigDecimalLightHelperFunctions.digitsToString(t.d).Slice(0, wpr) == (n = BigDecimalLightHelperFunctions.digitsToString(r.d)).Slice(0, wpr))
                {
                    n = n.Slice(wpr - 3, wpr + 1);

                    // The 4th rounding digit may be in error by -1 so if the 4 rounding digits are 9999 or
                    // 4999, i.e. approaching a rounding boundary, continue the iteration.
                    if (s == wpr && n == "4999")
                    {

                        // On the first iteration only, check to see if rounding up gives the exact result as the
                        // nines may infinitely repeat.
                        BigDecimalLightHelperFunctions.round(t, pr + 1, 0);

                        if (t.Times(t).Eq(x))
                        {
                            r = t;
                            break;
                        }
                    }
                    else if (n != "9999")
                    {
                        break;
                    }

                    wpr += 4;
                }
            }

            Ctor.external = true;

            return BigDecimalLightHelperFunctions.round(r, pr);
        }
        /// <inheritdoc cref="Sqrt" />
        public BigDecimalLight SquareRoot() => Sqrt();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimalLight whose value is this BigDecimalLight times `y`, truncated to
        /// `precision` significant digits.</returns>
        public BigDecimalLight Mul(BigDecimalArgument<BigDecimalLight> y) => _mul(new BigDecimalLight(y, this.Config));
        private BigDecimalLight _mul(BigDecimalLight y)
        {
            int carry = 0;
            long e, i, k, rL, t, xdL, ydL;
            int[] r;
            var x = this;
            var Ctor = x.Config;
            var xd = x.d;
            var yd = y.d;

            // Return 0 if either is 0.
            if (!x.s.IsTrue() || !y.s.IsTrue()) return new BigDecimalLight(0, Ctor);

            y.s *= x.s;
            e = x.e + y.e;
            xdL = xd.LongLength;
            ydL = yd.LongLength;

            // Ensure xd points to the longer array.
            if (xdL < ydL)
            {
                r = xd;
                xd = yd;
                yd = r;
                rL = xdL;
                xdL = ydL;
                ydL = rL;
            }

            // Initialise the result array with zeros.
            r = new int[0];
            rL = xdL + ydL;
            for (i = rL; i--.IsTrue();) ArrayExtensions.Push(ref r, 0);

            // Multiply!
            for (i = ydL; --i >= 0;)
            {
                carry = 0;
                for (k = xdL + i; k > i;)
                {
                    t = r[k] + (long)yd[i] * xd[k - i - 1] + carry;
                    r[k--] = (int)(t % BigDecimalLightHelperFunctions.BASE) | 0;
                    carry = (int)(t / BigDecimalLightHelperFunctions.BASE) | 0;
                }

                r[k] = (r[k] + carry) % BigDecimalLightHelperFunctions.BASE | 0;
            }

            // Remove trailing zeros.
            for (; !r[--rL].IsTrue();) ArrayExtensions.Pop(ref r);

            if (carry.IsTrue()) ++e;
            else ArrayExtensions.Shift(ref r);

            y.d = r;
            y.e = e;

            return Ctor.external ? BigDecimalLightHelperFunctions.round(y, Ctor.Precision) : y;
        }
        /// <inheritdoc cref="Mul" />
        public BigDecimalLight Times(BigDecimalArgument<BigDecimalLight> y) => Mul(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp">Decimal places.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimalLight rounded to a maximum of `dp`
        /// decimal places using rounding mode `rm` or `rounding` if `rm` is omitted.<br /><br />
        /// If `dp` is omitted, return a new BigDecimalLight whose value is the value of this BigDecimalLight.</returns>
        public BigDecimalLight ToDP(long? dp = null, RoundingMode? rm = null)
        {
            var x = this;
            var Ctor = x.Config;

            x = new BigDecimalLight(x, Ctor);
            if (dp == null) return x;

            BigDecimalLightHelperFunctions.checkInt32(dp.Value, 0, BigDecimalLightHelperFunctions.MAX_DIGITS);

            if (rm == null) rm = Ctor.Rounding;

            return BigDecimalLightHelperFunctions.round(x, dp.Value + BigDecimalLightHelperFunctions.getBase10Exponent(x) + 1, rm);
        }
        /// <inheritdoc cref="ToDP" />
        public BigDecimalLight ToDecimalPlaces(long? dp = null, RoundingMode? rm = null) => ToDP(dp, rm);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp">Decimal places.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimalLight in exponential notation rounded to
        /// `dp` fixed decimal places using rounding mode `rounding`.</returns>
        public string ToExponential(long? dp = null, RoundingMode? rm = null)
        {
            LongString str;
            var x = this;
            var Ctor = x.Config;

            if (dp == null)
            {
                str = BigDecimalLightHelperFunctions.toString(x, true);
            }
            else
            {
                BigDecimalLightHelperFunctions.checkInt32(dp.Value, 0, BigDecimalLightHelperFunctions.MAX_DIGITS);

                if (rm == null) rm = Ctor.Rounding;

                x = BigDecimalLightHelperFunctions.round(new BigDecimalLight(x, Ctor), dp.Value + 1, rm);
                str = BigDecimalLightHelperFunctions.toString(x, true, dp + 1);
            }

            return str;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp">Decimal places.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimalLight in normal (fixed-point) notation to
        /// `dp` fixed decimal places and rounded using rounding mode `rm` or `rounding` if `rm` is
        /// omitted.</returns>
        public string ToFixed(long? dp = null, RoundingMode? rm = null)
        {
            LongString str;
            BigDecimalLight y;
            var x = this;
            var Ctor = x.Config;

            if (dp == null) return BigDecimalLightHelperFunctions.toString(x);

            BigDecimalLightHelperFunctions.checkInt32(dp.Value, 0, BigDecimalLightHelperFunctions.MAX_DIGITS);

            if (rm == null) rm = Ctor.Rounding;

            y = BigDecimalLightHelperFunctions.round(new BigDecimalLight(x, Ctor), dp.Value + BigDecimalLightHelperFunctions.getBase10Exponent(x) + 1, rm);
            str = BigDecimalLightHelperFunctions.toString(y.Abs(), false, dp.Value + BigDecimalLightHelperFunctions.getBase10Exponent(y) + 1);

            // To determine whether to add the minus sign look at the value before it was rounded,
            // i.e. look at `x` rather than `y`.
            return x.IsNeg() && !x.IsZero() ? "-" + str : str;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimalLight rounded to a whole number using
        /// rounding mode `rounding`.</returns>
        public BigDecimalLight ToInt()
        {
            var x = this;
            var Ctor = x.Config;
            return BigDecimalLightHelperFunctions.round(new BigDecimalLight(x, Ctor), BigDecimalLightHelperFunctions.getBase10Exponent(x) + 1, Ctor.Rounding);
        }
        /// <inheritdoc cref="ToInt" />
        public BigDecimalLight ToInteger() => ToInt();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>The value of this BigDecimalLight converted to a number primitive.</returns>
        public double ToNumber()
        {
            var n = double.Parse(this.ToString(), CultureInfo.InvariantCulture);
            return n;
        }


        /// <summary>
        /// For non-integer or very large exponents Pow(x, y) is calculated using<br /><br />
        /// x^y = Exp(y * ln(x))<br /><br />
        /// The maximum error is 1 ulp (unit in last place).
        /// </summary>
        /// <param name="y">The power to which to raise this BigDecimalLight.</param>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimal raised to the power `y`,
        /// truncated to `precision` significant digits.</returns>
        public BigDecimalLight Pow(BigDecimalArgument<BigDecimalLight> y) => _pow(new BigDecimalLight(y, this.Config));
        private BigDecimalLight _pow(BigDecimalLight y)
        {
            long e, k, pr;
            BigDecimalLight r;
            int sign;
            bool yIsInt;
            var x = this;
            var Ctor = x.Config;
            var guard = 12;
            var yn = y.ToNumber();

            // pow(x, 0) = 1
            if (!y.s.IsTrue()) return new BigDecimalLight(BigDecimalLightHelperFunctions.ONE, Ctor);

            x = new BigDecimalLight(x, Ctor);

            // pow(0, y > 0) = 0
            // pow(0, y < 0) = Infinity
            if (!x.s.IsTrue())
            {
                if (y.s < 1) throw new BigDecimalException(BigDecimalException.DecimalError + "Infinity");
                return x;
            }

            // pow(1, y) = 1
            if (x.Eq(BigDecimalLightHelperFunctions.ONE)) return x;

            pr = Ctor.Precision;

            // pow(x, 1) = x
            if (y.Eq(BigDecimalLightHelperFunctions.ONE)) return BigDecimalLightHelperFunctions.round(x, pr);

            e = y.e;
            k = y.d.LongLength - 1;
            yIsInt = e >= k;
            sign = x.s;

            var k0 = double.IsInfinity(yn) ? BigDecimalLightHelperFunctions.MAX_SAFE_INTEGER + 1 : new BigInteger(yn);
            k0 = (k0 < 0 ? -k0 : k0);

            if (!yIsInt)
            {

                // pow(x < 0, y non-integer) = NaN
                if (sign < 0) throw new BigDecimalException(BigDecimalException.DecimalError + "NaN");

                // If y is a small integer use the 'exponentiation by squaring' algorithm.
            }
            else if (k0 <= BigDecimalLightHelperFunctions.MAX_SAFE_INTEGER)
            {
                k = (long)k0;

                r = new BigDecimalLight(BigDecimalLightHelperFunctions.ONE, Ctor);

                // Max k of 9007199254740991 takes 53 loop iterations.
                // Maximum digits array length; leaves [28, 34] guard digits.
                e = (long)Math.Ceiling((double)pr / BigDecimalLightHelperFunctions.LOG_BASE + 4);

                Ctor.external = false;

                for (; ; )
                {
                    if ((k % 2).IsTrue())
                    {
                        r = r.Times(x);
                        BigDecimalLightHelperFunctions.truncate(ref r.d, e);
                    }

                    k = (long)Math.Floor((double)k / 2);
                    if (k == 0) break;

                    x = x.Times(x);
                    BigDecimalLightHelperFunctions.truncate(ref x.d, e);
                }

                Ctor.external = true;

                return y.s < 0 ? new BigDecimalLight(BigDecimalLightHelperFunctions.ONE, Ctor).Div(r) : BigDecimalLightHelperFunctions.round(r, pr);
            }

            // Result is negative if x is negative and the last digit of integer y is odd.
            sign = sign < 0 && (y.d[Math.Max(e, k)] & 1).IsTrue() ? -1 : 1;

            x.s = 1;
            Ctor.external = false;
            r = y.Times(BigDecimalLightHelperFunctions.ln(x, pr + guard));
            Ctor.external = true;
            r = BigDecimalLightHelperFunctions.exp(r);
            r.s = sign;

            return r;
        }
        /// <inheritdoc cref="Pow" />
        public BigDecimalLight ToPower(BigDecimalArgument<BigDecimalLight> y) => Pow(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimalLight rounded to `sd` significant digits
        /// using rounding mode `rounding`.<br /><br />
        /// Exponential notation if `sd` is less than the number of digits necessary to represent
        /// the integer part of the value in normal notation.</returns>
        public string ToPrecision(long? sd = null, RoundingMode? rm = null)
        {
            long e;
            LongString str;
            var x = this;
            var Ctor = x.Config;

            if (sd == null)
            {
                e = BigDecimalLightHelperFunctions.getBase10Exponent(x);
                str = BigDecimalLightHelperFunctions.toString(x, e <= Ctor.ToExpNeg || e >= Ctor.ToExpPos);
            }
            else
            {
                BigDecimalLightHelperFunctions.checkInt32(sd.Value, 1, BigDecimalLightHelperFunctions.MAX_DIGITS);

                if (rm == null) rm = Ctor.Rounding;

                x = BigDecimalLightHelperFunctions.round(new BigDecimalLight(x, Ctor), sd.Value, rm);
                e = BigDecimalLightHelperFunctions.getBase10Exponent(x);
                str = BigDecimalLightHelperFunctions.toString(x, sd <= e || e <= Ctor.ToExpNeg, sd);
            }

            return str;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A new BigDecimalLight whose value is the value of this BigDecimalLight rounded to a maximum of `sd`
        /// significant digits using rounding mode `rm`, or to `precision` and `rounding` respectively if
        /// omitted.</returns>
        public BigDecimalLight ToSD(long? sd = null, RoundingMode? rm = null)
        {
            var x = this;
            var Ctor = x.Config;

            if (sd == null)
            {
                sd = Ctor.Precision;
                rm = Ctor.Rounding;
            }
            else
            {
                BigDecimalLightHelperFunctions.checkInt32(sd.Value, 1, BigDecimalLightHelperFunctions.MAX_DIGITS);

                if (rm == null) rm = Ctor.Rounding;
            }

            return BigDecimalLightHelperFunctions.round(new BigDecimalLight(x, Ctor), sd.Value, rm);
        }
        /// <inheritdoc cref="ToSD" />
        public BigDecimalLight ToSignificantDigits(long? sd = null, RoundingMode? rm = null) => ToSD(sd, rm);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A string representing the value of this BigDecimalLight.<br /><br />
        /// Exponential notation if this BigDecimalLight has a positive exponent equal to or greater than
        /// `ToExpPos`, or a negative exponent equal to or less than `ToExpNeg`.</returns>
        public override string ToString()
        {
            var x = this;
            var Ctor = x.Config;
            var e = BigDecimalLightHelperFunctions.getBase10Exponent(x);

            return BigDecimalLightHelperFunctions.toString(x, e <= Ctor.ToExpNeg || e >= Ctor.ToExpPos);
        }
        /// <inheritdoc cref="ToString" />
        public string ValueOf() => ToString();
        /// <inheritdoc cref="ToString" />
        public string Val() => ToString();
        /// <inheritdoc cref="ToString" />
        public string ToJSON() => ToString();
    }
}