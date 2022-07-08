using DecimalSharp.Core;
using DecimalSharp.Core.Extensions;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace DecimalSharp
{
    public class BigDecimal
    {
        public BigDecimalConfig Config;

        public int[]? d;
        public int? s;
        public long? e;

        protected BigDecimal(BigDecimalConfig? config = null)
        {
            this.Config = config ?? new BigDecimalConfig();

            this.s = null;
            this.e = null;
            this.d = null;
        }
        public BigDecimal(BigDecimalArgument<BigDecimal> v, BigDecimalConfig? config = null) : this(config)
        {
            v.Switch(
                @double => _BigDecimal(@double),
                @decimal => _BigDecimal(@decimal),
                @long => _BigDecimal(@long),
                @int => _BigDecimal(@int),
                @string => _BigDecimal(@string),
                bigInteger => _BigDecimal(bigInteger.ToString(CultureInfo.InvariantCulture)),
                bigDecimal => _BigDecimal(bigDecimal)
            );
        }
        internal void _BigDecimal(string? v)
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
                // Plus sign?
                if (i == 43) v = v.Slice(1);
                this.s = 1;
            }

            var isDec = Regex.IsMatch(v, BigDecimalHelperFunctions.isDecimal, RegexOptions.IgnoreCase);
            if (isDec)
            {
                _BigDecimal(BigDecimalHelperFunctions.parseDecimal(this, v));
            }
            else
            {
                _BigDecimal(BigDecimalHelperFunctions.parseOther(this, v));
            }
        }
        internal void _BigDecimal(int v)
        {
            int e, i;

            if (v == 0)
            {
                this.s = (double)1 / v < 0 ? -1 : 1;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 1;
            }

            // Fast path for small integers.
            if (v == ~~v && v < 1e7)
            {
                for (e = 0, i = v; i >= 10; i /= 10) e++;

                if (this.Config.external)
                {
                    if (e > this.Config.MaxE)
                    {
                        this.e = null;
                        this.d = null;
                    }
                    else if (e < this.Config.MinE)
                    {
                        this.e = 0;
                        this.d = new int[] { 0 };
                    }
                    else
                    {
                        this.e = e;
                        this.d = new int[] { v };
                    }
                }
                else
                {
                    this.e = e;
                    this.d = new int[] { v };
                }

                return;
            }

            _BigDecimal(BigDecimalHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }
        internal void _BigDecimal(long v)
        {
            long e, i;

            if (v == 0)
            {
                this.s = (double)1 / v < 0 ? -1 : 1;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 1;
            }

            // Fast path for small integers.
            if (v == ~~v && v < 1e7)
            {
                for (e = 0, i = v; i >= 10; i /= 10) e++;

                if (this.Config.external)
                {
                    if (e > this.Config.MaxE)
                    {
                        this.e = null;
                        this.d = null;
                    }
                    else if (e < this.Config.MinE)
                    {
                        this.e = 0;
                        this.d = new int[] { 0 };
                    }
                    else
                    {
                        this.e = e;
                        this.d = new int[] { (int)v };
                    }
                }
                else
                {
                    this.e = e;
                    this.d = new int[] { (int)v };
                }

                return;
            }

            _BigDecimal(BigDecimalHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }

        internal void _BigDecimal(double v)
        {
            if (v == 0)
            {
                this.s = 1 / v < 0 ? -1 : 1;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 1;
            }

            // Infinity, NaN.
            if (v * 0 != 0)
            {
                if (double.IsNaN(v))
                {
                    this.s = null; //if nan
                }
                this.e = null;
                this.d = null;
                return;
            }

            _BigDecimal(BigDecimalHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }

        internal void _BigDecimal(decimal v)
        {
            if (v == 0)
            {
                this.s = 1 / v < 0 ? -1 : 1;
                this.e = 0;
                this.d = new int[] { 0 };
                return;
            }

            if (v < 0)
            {
                v = -v;
                this.s = -1;
            }
            else
            {
                this.s = 1;
            }

            _BigDecimal(BigDecimalHelperFunctions.parseDecimal(this, v.ToString(CultureInfo.InvariantCulture)));
        }
        internal void _BigDecimal(BigDecimal? v)
        {
            if (v == null)
                throw new BigDecimalException(BigDecimalException.InvalidArgument + v);

            this.s = v.s;

            if (this.Config.external)
            {
                if (!v.d.IsTrue() || v.e > this.Config.MaxE)
                {

                    // Infinity.
                    this.e = null;
                    this.d = null;
                }
                else if (v.e < this.Config.MinE)
                {

                    // Zero.
                    this.e = 0;
                    this.d = new int[] { 0 };
                }
                else
                {
                    this.e = v.e;
                    this.d = v.d.Slice();
                }
            }
            else
            {
                this.e = v.e;
                this.d = v.d.IsTrue() ? v.d.Slice() : v.d;
            }
        }


        // Decimal prototype methods


        /*
         *  absoluteValue             abs
         *  ceil
         *  clampedTo                 clamp
         *  comparedTo                cmp
         *  cosine                    cos
         *  cubeRoot                  cbrt
         *  decimalPlaces             dp
         *  dividedBy                 div
         *  dividedToIntegerBy        divToInt
         *  equals                    eq
         *  floor
         *  greaterThan               gt
         *  greaterThanOrEqualTo      gte
         *  hyperbolicCosine          cosh
         *  hyperbolicSine            sinh
         *  hyperbolicTangent         tanh
         *  inverseCosine             acos
         *  inverseHyperbolicCosine   acosh
         *  inverseHyperbolicSine     asinh
         *  inverseHyperbolicTangent  atanh
         *  inverseSine               asin
         *  inverseTangent            atan
         *  isFinite
         *  isInteger                 isInt
         *  isnull
         *  isNegative                isNeg
         *  isPositive                isPos
         *  isZero
         *  lessThan                  lt
         *  lessThanOrEqualTo         lte
         *  logarithm                 log
         *  [maximum]                 [max]
         *  [minimum]                 [min]
         *  minus                     sub
         *  modulo                    mod
         *  naturalExponential        exp
         *  naturalLogarithm          ln
         *  negated                   neg
         *  plus                      add
         *  precision                 sd
         *  round
         *  sine                      sin
         *  squareRoot                sqrt
         *  tangent                   tan
         *  times                     mul
         *  toBinary
         *  toDecimalPlaces           toDP
         *  toExponential
         *  toFixed
         *  toFraction
         *  toHexadecimal             toHex
         *  toNearest
         *  toNumber
         *  toOctal
         *  toPower                   pow
         *  toPrecision
         *  toSignificantDigits       toSD
         *  toString
         *  truncated                 trunc
         *  valueOf                   toJSON
         */


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the absolute value of this BigDecimal.</returns>
        public BigDecimal Abs()
        {
            var x = new BigDecimal(this, this.Config);
            if (x.s < 0) x.s = 1;
            return BigDecimalHelperFunctions.finalise(x);
        }
        /// <inheritdoc cref="Abs" />
        public BigDecimal AbsoluteValue() => Abs();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal rounded to a whole number in the
        /// direction of positive Infinity.</returns>
        public BigDecimal Ceil() {
            return BigDecimalHelperFunctions.finalise(new BigDecimal(this, this.Config), this.e + 1, RoundingMode.ROUND_CEIL);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal clamped to the range
        /// delineated by `min` and `max`.</returns>
        public BigDecimal Clamp(BigDecimalArgument<BigDecimal> min, BigDecimalArgument<BigDecimal> max) => _clamp(new BigDecimal(min, this.Config), new BigDecimal(max, this.Config));
        private BigDecimal _clamp(BigDecimal min, BigDecimal max) {
            int? k;
            var x = this;
            if (!min.s.IsTrue() || !max.s.IsTrue()) return new BigDecimal(double.NaN, this.Config);
            if (min.Gt(max)) throw new BigDecimalException(BigDecimalException.InvalidArgument + max);
            k = x.Cmp(min);
            return k < 0 ? min : x.Cmp(max) > 0 ? max : new BigDecimal(x, this.Config);
        }
        /// <inheritdoc cref="Clamp" />
        public BigDecimal ClampedTo(BigDecimalArgument<BigDecimal> min, BigDecimalArgument<BigDecimal> max) => Clamp(min, max);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>1 if the value of this BigDecimal is greater than the value of `y`,
        /// <br />-1 if the value of this BigDecimal is less than the value of `y`,
        /// <br />0 if they have the same value,
        /// <br />null if the value of either BigDecimal is null.</returns>
        public int? Cmp(BigDecimalArgument<BigDecimal> y) => _cmp(new BigDecimal(y, this.Config));
        private int? _cmp(BigDecimal y)
        {
            long i, j, xdL, ydL;
            var x = this;
            var xd = x.d;
            var yd = y.d;
            var xs = x.s;
            var ys = y.s;

            // Either null or ±Infinity?
            if (!xd.IsTrue() || !yd.IsTrue())
            {
                return !xs.IsTrue() || !ys.IsTrue() ? null : xs != ys ? xs : xd == yd ? 0 : !xd.IsTrue() ^ xs < 0 ? 1 : -1;
            }

            // Either zero?
            if (!xd[0].IsTrue() || !yd[0].IsTrue()) return xd[0].IsTrue() ? xs : yd[0].IsTrue() ? -ys : 0;

            // Signs differ?
            if (xs != ys) return xs;

            // Compare exponents.
            if (x.e != y.e) return x.e > y.e ^ xs < 0 ? 1 : -1;

            xdL = xd.LongLength;
            ydL = yd.LongLength;

            // Compare digit by digit.
            for (i = 0, j = xdL < ydL ? xdL : ydL; i < j; ++i)
            {
                if (xd[i] != yd[i]) return xd[i] > yd[i] ^ xs < 0 ? 1 : -1;
            }

            // Compare lengths.
            return xdL == ydL ? 0 : xdL > ydL ^ xs < 0 ? 1 : -1;
        }
        /// <inheritdoc cref="Cmp" />
        public int? ComparedTo(BigDecimalArgument<BigDecimal> y) => Cmp(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the cosine of the value in radians of this BigDecimal.</returns>
        public BigDecimal Cos() {
            long pr;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (!x.d.IsTrue()) return new BigDecimal(double.NaN, this.Config);

            // Cos(0) = Cos(-0) = 1
            if (!x.d[0].IsTrue()) return new BigDecimal(1, this.Config);

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + Math.Max(x.e ?? 0, x.Sd() ?? 0) + BigDecimalHelperFunctions.LOG_BASE;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            x = BigDecimalHelperFunctions.cosine(Ctor, BigDecimalHelperFunctions.toLessThanHalfPi(Ctor, x, out var quadrant));

            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return BigDecimalHelperFunctions.finalise(quadrant == 2 || quadrant == 3 ? x.Neg() : x, pr, rm, true);
        }
        /// <inheritdoc cref="Cos" />
        public BigDecimal Cosine() => Cos();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the cube root of the value of this BigDecimal, rounded to
        /// `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Cbrt() {
            long e, sd;
            int rep = 0;
            bool m = false;
            double s;
            LongString n;
            BigDecimal r, t, t3, t3plusx;
            var x = this;
            var Ctor = x.Config;

            if (!x.IsFinite() || x.IsZero()) return new BigDecimal(x, this.Config);
            Ctor.external = false;

            // Initial estimate.
            s = (x.s ?? 0) * Math.Pow((x.s ?? 0) * x.ToNumber(), (double)1 / 3);

            // Math.cbrt underflow/overflow?
            // Pass x to Math.pow as integer, then adjust the exponent of the result.
            if (!s.IsTrue() || Math.Abs(s) == double.PositiveInfinity) {
                n = BigDecimalHelperFunctions.digitsToString(x.d);
                e = x.e ?? 0;

                // Adjust n exponent so it is a multiple of 3 away from x exponent.
                if ((s = (e - n.LongLength + 1) % 3).IsTrue()) n += (s == 1 || s == -2 ? "0" : "00");
                s = Math.Pow(double.Parse(n, CultureInfo.InvariantCulture), (double)1 / 3);

                // Rarely, e may be one less than the result exponent value.
                e = (long)Math.Floor(((double)e + 1) / 3) - ((e % 3 == (e < 0 ? -1 : 2)) ? 1 : 0);

                if (s == double.PositiveInfinity) {
                    n = "5e" + e;
                } else {
                    n = s.ToExponential();
                    n = n.Slice(0, n.IndexOf("e") + 1) + e;
                }

                r = new BigDecimal(n.ToString(), Ctor);
                r.s = x.s;
            } else {
                r = new BigDecimal(s.ToString(CultureInfo.InvariantCulture), Ctor);
            }

            sd = (e = Ctor.Precision) + 3;

            // Halley"s method.
            // TODO? Compare Newton"s method.
            for (; ; ) {
                t = r;
                t3 = t.Times(t).Times(t);
                t3plusx = t3.Plus(x);
                r = BigDecimalHelperFunctions.divide(t3plusx.Plus(x).Times(t), t3plusx.Plus(t3), sd + 2, RoundingMode.ROUND_DOWN);

                // TODO? Replace with for-loop and checkRoundingDigits.
                if (BigDecimalHelperFunctions.digitsToString(t.d).Slice(0, sd) == (n = BigDecimalHelperFunctions.digitsToString(r.d)).Slice(0, sd)) {
                    n = n.Slice(sd - 3, sd + 1);

                    // The 4th rounding digit may be in error by -1 so if the 4 rounding digits are 9999 or 4999
                    // , i.e. approaching a rounding boundary, continue the iteration.
                    if (n == "9999" || !rep.IsTrue() && n == "4999") {

                        // On the first iteration only, check to see if rounding up gives the exact result as the
                        // nines may infinitely repeat.
                        if (!rep.IsTrue()) {
                            BigDecimalHelperFunctions.finalise(t, e + 1, RoundingMode.ROUND_UP);

                            if (t.Times(t).Times(t).Eq(x)) {
                                r = t;
                                break;
                            }
                        }

                        sd += 4;
                        rep = 1;
                    } else {

                        // If the rounding digits are null, 0{0,4} or 50{0,3}, check for an exact result.
                        // If not, then there are further digits and m will be truthy.
                        double doubleN = 0;
                        double.TryParse(n, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleN);
                        double doubleN2 = 0;
                        double.TryParse(n.Slice(1), NumberStyles.Any, CultureInfo.InvariantCulture, out doubleN2);
                        if (!doubleN.IsTrue() || !doubleN2.IsTrue() && n.ElementAt(0) == '5') {

                            // Truncate to the first rounding digit.
                            BigDecimalHelperFunctions.finalise(r, e + 1, RoundingMode.ROUND_DOWN);
                            m = !r.Times(r).Times(r).Eq(x);
                        }

                        break;
                    }
                }
            }

            Ctor.external = true;

            return BigDecimalHelperFunctions.finalise(r, e, Ctor.Rounding, m);
        }
        /// <inheritdoc cref="Cbrt" />
        public BigDecimal CubeRoot() => Cbrt();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>The number of decimal places of the value of this BigDecimal.</returns>
        public long? Dp() {
            long w;
            var d = this.d;
            long? n = null;

            if (d.IsTrue()) {
                w = d.LongLength - 1;
                n = (w - (long)Math.Floor((double)this.e / BigDecimalHelperFunctions.LOG_BASE)) * BigDecimalHelperFunctions.LOG_BASE;

                // Subtract the number of trailing zeros of the last word.
                w = d[w];
                if (w.IsTrue()) for (; w % 10 == 0; w /= 10) n--;
                if (n < 0) n = 0;
            }

            return n;
        }
        /// <inheritdoc cref="Dp" />
        public long? DecimalPlaces() => Dp();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal divided by `y`, rounded to
        /// `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Div(BigDecimalArgument<BigDecimal> y) => _div(new BigDecimal(y, this.Config));
        private BigDecimal _div(BigDecimal y) {
            return BigDecimalHelperFunctions.divide(this, y);
        }
        /// <inheritdoc cref="Div" />
        public BigDecimal DividedBy(BigDecimalArgument<BigDecimal> y) => Div(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is the integer part of dividing the value of this BigDecimal
        /// by the value of `y`, rounded to `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal DivToInt(BigDecimalArgument<BigDecimal> y) => _divToInt(new BigDecimal(y, this.Config));
        private BigDecimal _divToInt(BigDecimal y) {
            var x = this;
            var Ctor = x.Config;
            return BigDecimalHelperFunctions.finalise(BigDecimalHelperFunctions.divide(x, y, 0, RoundingMode.ROUND_DOWN, 1), Ctor.Precision, Ctor.Rounding);
        }
        /// <inheritdoc cref="DivToInt" />
        public BigDecimal DividedToIntegerBy(BigDecimalArgument<BigDecimal> y) => DivToInt(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimal is equal to the value of `y`, otherwise false.</returns>
        public bool Eq(BigDecimalArgument<BigDecimal> y)
        {
            return this.Cmp(y) == 0;
        }
        /// <inheritdoc cref="Eq" />
        public bool Equals(BigDecimalArgument<BigDecimal> y) => Eq(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal rounded to a whole number in the
        /// direction of negative Infinity.</returns>
        public BigDecimal Floor() {
            return BigDecimalHelperFunctions.finalise(new BigDecimal(this, this.Config), this.e + 1, RoundingMode.ROUND_FLOOR);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimal is greater than the value of `y`, otherwise
        /// false.</returns>
        public bool Gt(BigDecimalArgument<BigDecimal> y)
        {
            return this.Cmp(y) > 0;
        }
        /// <inheritdoc cref="Gt" />
        public bool GreaterThan(BigDecimalArgument<BigDecimal> y) => Gt(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimal is greater than or equal to the value of `y`,
        /// otherwise false.</returns>
        public bool Gte(BigDecimalArgument<BigDecimal> y)
        {
            var k = this.Cmp(y);
            return k == 1 || k == 0;
        }
        /// <inheritdoc cref="Gte" />
        public bool GreaterThanOrEqualTo(BigDecimalArgument<BigDecimal> y) => Gte(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the hyperbolic cosine of the value in radians of this
        /// BigDecimal.</returns>
        public BigDecimal Cosh() {
            long pr, len, k;
            LongString n;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;
            var one = new BigDecimal(1, Ctor);

            if (!x.IsFinite()) return x.s.IsTrue() ? new BigDecimal(double.PositiveInfinity, Ctor) : new BigDecimal(double.NaN, Ctor);
            if (x.IsZero()) return one;

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + Math.Max(x.e ?? 0, x.Sd() ?? 0) + 4;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;
            len = x.d.LongLength;

            // Argument reduction: Cos(4x) = 1 - 8cos^2(x) + 8cos^4(x) + 1
            // i.e. Cos(x) = 1 - cos^2(x/4)(8 - 8cos^2(x/4))

            // Estimate the optimum number of times to use the argument reduction.
            // TODO? Estimation reused from cosine() and may not be optimal here.
            if (len < 32) {
                k = (long)Math.Ceiling((double)len / 3);
                n = ((double)1 / BigDecimalHelperFunctions.tinyPow(4, k)).ToString(CultureInfo.InvariantCulture);
            } else {
                k = 16;
                n = "2.3283064365386962890625e-10";
            }

            x = BigDecimalHelperFunctions.taylorSeries(Ctor, 1, x.Times(n.ToString()), new BigDecimal(1, Ctor), true);

            // Reverse argument reduction
            BigDecimal cosh2_x;
            var i = k;
            var d8 = new BigDecimal(8, Ctor);
            for (; i--.IsTrue();) {
                cosh2_x = x.Times(x);
                x = one.Minus(cosh2_x.Times(d8.Minus(cosh2_x.Times(d8))));
            }

            return BigDecimalHelperFunctions.finalise(x, Ctor.Precision = pr, Ctor.Rounding = rm, true);
        }
        /// <inheritdoc cref="Cosh" />
        public BigDecimal HyperbolicCosine() => Cosh();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the hyperbolic sine of the value in radians of this
        /// BigDecimal.</returns>
        public BigDecimal Sinh() {
            long pr, len, k;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (!x.IsFinite() || x.IsZero()) return new BigDecimal(x, Ctor);

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + Math.Max(x.e ?? 0, x.Sd() ?? 0) + 4;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;
            len = x.d.LongLength;

            if (len < 3) {
                x = BigDecimalHelperFunctions.taylorSeries(Ctor, 2, x, x, true);
            } else {

                // Alternative argument reduction: Sinh(3x) = Sinh(x)(3 + 4sinh^2(x))
                // i.e. Sinh(x) = Sinh(x/3)(3 + 4sinh^2(x/3))
                // 3 multiplications and 1 addition

                // Argument reduction: Sinh(5x) = Sinh(x)(5 + sinh^2(x)(20 + 16sinh^2(x)))
                // i.e. Sinh(x) = Sinh(x/5)(5 + sinh^2(x/5)(20 + 16sinh^2(x/5)))
                // 4 multiplications and 2 additions

                // Estimate the optimum number of times to use the argument reduction.
                k = (long)(1.4 * Math.Sqrt(len));
                k = k > 16 ? 16 : k | 0;

                x = x.Times((double)1 / BigDecimalHelperFunctions.tinyPow(5, k));
                x = BigDecimalHelperFunctions.taylorSeries(Ctor, 2, x, x, true);

                // Reverse argument reduction
                BigDecimal sinh2_x,
                d5 = new BigDecimal(5, Ctor),
                d16 = new BigDecimal(16, Ctor),
                d20 = new BigDecimal(20, Ctor);
                for (; k--.IsTrue();) {
                    sinh2_x = x.Times(x);
                    x = x.Times(d5.Plus(sinh2_x.Times(d16.Times(sinh2_x).Plus(d20))));
                }
            }

            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return BigDecimalHelperFunctions.finalise(x, pr, rm, true);
        }
        /// <inheritdoc cref="Sinh" />
        public BigDecimal HyperbolicSine() => Sinh();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the hyperbolic tangent of the value in radians of this
        /// BigDecimal.</returns>
        public BigDecimal Tanh() {
            long pr;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (!x.IsFinite()) return new BigDecimal(x.s.HasValue ? (double)x.s.Value : double.NaN, Ctor);
            if (x.IsZero()) return new BigDecimal(x, Ctor);

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + 7;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            return BigDecimalHelperFunctions.divide(x.Sinh(), x.Cosh(), Ctor.Precision = pr, Ctor.Rounding = rm);
        }
        /// <inheritdoc cref="Tanh" />
        public BigDecimal HyperbolicTangent() => Tanh();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the arccosine (inverse cosine) in radians of the value of
        /// this BigDecimal.</returns>
        public BigDecimal Acos() {
            BigDecimal halfPi;
            var x = this;
            var Ctor = x.Config;
            var k = x.Abs().Cmp(1);
            var pr = Ctor.Precision;
            var rm = Ctor.Rounding;

            if (k != -1) {
                return k == 0
                  // |x| is 1
                  ? x.IsNeg() ? BigDecimalHelperFunctions.getPi(Ctor, pr, rm) : new BigDecimal(0, Ctor)
                  // |x| > 1 or x is null
                  : new BigDecimal(double.NaN, Ctor);
            }

            if (x.IsZero()) return BigDecimalHelperFunctions.getPi(Ctor, pr + 4, rm).Times(0.5);

            // TODO? Special case Acos(0.5) = pi/3 and Acos(-0.5) = 2*pi/3

            Ctor.Precision = pr + 6;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            x = x.Asin();
            halfPi = BigDecimalHelperFunctions.getPi(Ctor, pr + 4, rm).Times(0.5);

            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return halfPi.Minus(x);
        }
        /// <inheritdoc cref="Acos" />
        public BigDecimal InverseCosine() => Acos();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the inverse of the hyperbolic cosine in radians of the
        /// value of this BigDecimal.</returns>
        public BigDecimal Acosh() {
            long pr;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (x.Lte(1)) return new BigDecimal(x.Eq(1) ? 0 : double.NaN, Ctor);
            if (!x.IsFinite()) return new BigDecimal(x, Ctor);

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + Math.Max(Math.Abs(x.e ?? 0), x.Sd() ?? 0) + 4;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;
            Ctor.external = false;

            x = x.Times(x).Minus(1).Sqrt().Plus(x);

            Ctor.external = true;
            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return x.Ln();
        }
        /// <inheritdoc cref="Acosh" />
        public BigDecimal InverseHyperbolicCosine() => Acosh();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the inverse of the hyperbolic sine in radians of the value
        /// of this BigDecimal.</returns>
        public BigDecimal Asinh() {
            long pr;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (!x.IsFinite() || x.IsZero()) return new BigDecimal(x, Ctor);

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + 2 * Math.Max(Math.Abs(x.e ?? 0), x.Sd() ?? 0) + 6;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;
            Ctor.external = false;

            x = x.Times(x).Plus(1).Sqrt().Plus(x);

            Ctor.external = true;
            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return x.Ln();
        }
        /// <inheritdoc cref="Asinh" />
        public BigDecimal InverseHyperbolicSine() => Asinh();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the inverse of the hyperbolic tangent in radians of the
        /// value of this BigDecimal.</returns>
        public BigDecimal Atanh() {
            long pr;
            RoundingMode rm;
            long wpr, xsd;
            var x = this;
            var Ctor = x.Config;

            if (!x.IsFinite()) return new BigDecimal(double.NaN, Ctor);
            if (x.e >= 0)
            {
                if (x.Abs().Eq(1)) return new BigDecimal((double)x.s / 0, Ctor);
                else if (x.IsZero()) return new BigDecimal(x, Ctor);
                else return new BigDecimal(double.NaN, Ctor);
            }

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            xsd = x.Sd() ?? 0;

            if (Math.Max(xsd, pr) < 2 * -x.e - 1) return BigDecimalHelperFunctions.finalise(new BigDecimal(x, Ctor), pr, rm, true);

            Ctor.Precision = wpr = xsd - (x.e ?? 0);

            x = BigDecimalHelperFunctions.divide(x.Plus(1), new BigDecimal(1, Ctor).Minus(x), wpr + pr, RoundingMode.ROUND_DOWN);

            Ctor.Precision = pr + 4;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            x = x.Ln();

            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return x.Times(0.5);
        }
        /// <inheritdoc cref="Atanh" />
        public BigDecimal InverseHyperbolicTangent() => Atanh();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the arcsine (inverse sine) in radians of the value of this
        /// BigDecimal.</returns>
        public BigDecimal Asin() {
            BigDecimal halfPi;
            int? k;
            long pr;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (x.IsZero()) return new BigDecimal(x, Ctor);

            k = x.Abs().Cmp(1);
            pr = Ctor.Precision;
            rm = Ctor.Rounding;

            if (k != -1) {

                // |x| is 1
                if (k == 0) {
                    halfPi = BigDecimalHelperFunctions.getPi(Ctor, pr + 4, rm).Times(0.5);
                    halfPi.s = x.s;
                    return halfPi;
                }

                // |x| > 1 or x is null
                return new BigDecimal(double.NaN, Ctor);
            }

            // TODO? Special case Asin(1/2) = pi/6 and Asin(-1/2) = -pi/6

            Ctor.Precision = pr + 6;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            x = x.Div(new BigDecimal(1, Ctor).Minus(x.Times(x)).Sqrt().Plus(1)).Atan();

            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return x.Times(2);
        }
        /// <inheritdoc cref="Asin" />
        public BigDecimal InverseSine() => Asin();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the arctangent (inverse tangent) in radians of the value
        /// of this BigDecimal.</returns>
        public BigDecimal Atan() {
            int k;
            long i, j, n, wpr;
            BigDecimal px, t, r, x2;
            var x = this;
            var Ctor = x.Config;
            var pr = Ctor.Precision;
            var rm = Ctor.Rounding;

            if (!x.IsFinite()) {
                if (!x.s.IsTrue()) return new BigDecimal(double.NaN, Ctor);
                if (pr + 4 <= BigDecimalHelperFunctions.PI_PRECISION) {
                    r = BigDecimalHelperFunctions.getPi(Ctor, pr + 4, rm).Times(0.5);
                    r.s = x.s;
                    return r;
                }
            } else if (x.IsZero()) {
                return new BigDecimal(x, Ctor);
            } else if (x.Abs().Eq(1) && pr + 4 <= BigDecimalHelperFunctions.PI_PRECISION) {
                r = BigDecimalHelperFunctions.getPi(Ctor, pr + 4, rm).Times(0.25);
                r.s = x.s;
                return r;
            }

            Ctor.Precision = wpr = pr + 10;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            // TODO? if (x >= 1 && pr <= BigDecimalFactory.PI_PRECISION) Atan(x) = halfPi * x.s - Atan(1 / x);

            // Argument reduction
            // Ensure |x| < 0.42
            // Atan(x) = 2 * Atan(x / (1 + Sqrt(1 + x^2)))

            k = Math.Min(28, (int)(wpr / BigDecimalHelperFunctions.LOG_BASE + 2) | 0);

            for (i = k; i != 0; --i) x = x.Div(x.Times(x).Plus(1).Sqrt().Plus(1));

            Ctor.external = false;

            j = (long)Math.Ceiling((double)wpr / BigDecimalHelperFunctions.LOG_BASE);
            n = 1;
            x2 = x.Times(x);
            r = new BigDecimal(x, Ctor);
            px = x;

            // Atan(x) = x - x^3/3 + x^5/5 - x^7/7 + ...
            for (; i != -1;) {
                px = px.Times(x2);
                t = r.Minus(px.Div(n += 2));

                px = px.Times(x2);
                r = t.Plus(px.Div(n += 2));

                if (r.d.LongLength > j && t.d.LongLength > j) for (i = j; r.d[i] == t.d[i] && i--.IsTrue();) ;
            }

            if (k.IsTrue()) r = r.Times(2 << (k - 1));

            Ctor.external = true;

            return BigDecimalHelperFunctions.finalise(r, Ctor.Precision = pr, Ctor.Rounding = rm, true);
        }
        /// <inheritdoc cref="Atan" />
        public BigDecimal InverseTangent() => Atan();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimal is a finite number, otherwise false.</returns>
        public bool IsFinite() {
            return this.d.IsTrue();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimal is an integer, otherwise false.</returns>
        public bool IsInt() {
            return this.d.IsTrue() && Math.Floor((double)this.e / BigDecimalHelperFunctions.LOG_BASE) > this.d.LongLength - 2;
        }
        public bool IsInteger() => IsInt();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimal is NaN(null), otherwise false.</returns>
        public bool IsNaN() {
            return !this.s.IsTrue();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimal is negative, otherwise false.</returns>
        public bool IsNeg() {
            return this.s < 0;
        }
        /// <inheritdoc cref="IsNeg" />
        public bool IsNegative() => IsNeg();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimal is positive, otherwise false.</returns>
        public bool IsPos() {
            return this.s > 0;
        }
        /// <inheritdoc cref="IsPos" />
        public bool IsPositive() => IsPos();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the value of this BigDecimal is 0 or -0, otherwise false.</returns>
        public bool IsZero() {
            return this.d.IsTrue() && this.d[0] == 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimal is less than `y`, otherwise false.</returns>
        public bool Lt(BigDecimalArgument<BigDecimal> y)
        {
            return this.Cmp(y) < 0;
        }
        /// <inheritdoc cref="Lt" />
        public bool LessThan(BigDecimalArgument<BigDecimal> y) => Lt(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>True if the value of this BigDecimal is less than or equal to `y`, otherwise false.</returns>
        public bool Lte(BigDecimalArgument<BigDecimal> y)
        {
            return this.Cmp(y) < 1;
        }
        /// <inheritdoc cref="Lte" />
        public bool LessThanOrEqualTo(BigDecimalArgument<BigDecimal> y) => Lte(y);


        /// <summary>
        /// The result will always be correctly rounded if the base of the log is 10, and "almost always"
        /// otherwise:<br /><br />
        /// Depending on the rounding mode, the result may be incorrectly rounded if the first fifteen
        /// rounding digits are [49]99999999999999 or [50]00000000000000. In that case, the maximum error
        /// between the result and the correctly rounded result will be one ulp (unit in the last place).
        /// </summary>
        /// <param name="base">The base of the logarithm. Default = 10.</param>
        /// <returns>The logarithm of the value of this BigDecimal to the specified base, rounded to `precision`
        /// significant digits using rounding mode `rounding`.<br /><br />
        /// If no base is specified, default base = 10 used.</returns>
        public BigDecimal Log(BigDecimalArgument<BigDecimal> @base)
        {
            return @base.Match(
                @double => _log(new BigDecimal(@double, this.Config)),
                @decimal => _log(new BigDecimal(@decimal, this.Config)),
                @long => _log(new BigDecimal(@long, this.Config)),
                @int => _log(new BigDecimal(@int, this.Config)),
                @string =>
                {
                    if (string.IsNullOrEmpty(@string)) return _log(new BigDecimal(10, this.Config));
                    return _log(new BigDecimal(@string, this.Config));
                },
                bigInteger => _log(new BigDecimal(bigInteger, this.Config)),
                bigDecimal => {
                    if (bigDecimal == null) return _log(new BigDecimal(10, this.Config));
                    return _log(new BigDecimal(bigDecimal, this.Config));
                }
            );
        }
        private BigDecimal _log(BigDecimal @base) {
            bool isBase10, inf = false;
            int[] d;
            BigDecimal denominator, num, r;
            long k, sd;
            var arg = this;
            var Ctor = arg.Config;
            var pr = Ctor.Precision;
            var rm = Ctor.Rounding;
            var guard = 5;


            d = @base.d;

            // Return null if @base is negative, or non-finite, or is 0 or 1.
            if (@base.s < 0 || !d.IsTrue() || !d[0].IsTrue() || @base.Eq(1)) return new BigDecimal(double.NaN, Ctor);

            isBase10 = @base.Eq(10);

            d = arg.d;

            // Is arg negative, non-finite, 0 or 1?
            if (arg.s < 0 || !d.IsTrue() || !d[0].IsTrue() || arg.Eq(1)) {
                return new BigDecimal(d.IsTrue() && !d[0].IsTrue() ? double.NegativeInfinity : arg.s != 1 ? double.NaN : d.IsTrue() ? 0 : double.PositiveInfinity, Ctor);
            }

            // The result will have a non-terminating decimal expansion if @base is 10 and arg is not an
            // integer power of 10.
            if (isBase10) {
                if (d.LongLength > 1) {
                    inf = true;
                } else {
                    for (k = d[0]; k % 10 == 0;) k /= 10;
                    inf = k != 1;
                }
            }

            Ctor.external = false;
            sd = pr + guard;
            num = BigDecimalHelperFunctions.naturalLogarithm(arg, sd);
            denominator = isBase10 ? BigDecimalHelperFunctions.getLn10(Ctor, sd + 10) : BigDecimalHelperFunctions.naturalLogarithm(@base, sd);

            // The result will have 5 rounding digits.
            r = BigDecimalHelperFunctions.divide(num, denominator, sd, RoundingMode.ROUND_DOWN);

            // If at a rounding boundary, i.e. the result"s rounding digits are [49]9999 or [50]0000,
            // calculate 10 further digits.
            //
            // If the result is known to have an infinite decimal expansion, repeat this until it is clear
            // that the result is above or below the boundary. Otherwise, if after calculating the 10
            // further digits, the last 14 are nines, round up and assume the result is exact.
            // Also assume the result is exact if the last 14 are zero.
            //
            // Example of a result that will be incorrectly rounded:
            // log[1048576](4503599627370502) = 2.60000000000000009610279511444746...
            // The above result correctly rounded using ROUND_CEIL to 1 decimal place should be 2.7, but it
            // will be given as 2.6 as there are 15 zeros immediately after the requested decimal place, so
            // the exact result would be assumed to be 2.6, which rounded using ROUND_CEIL to 1 decimal
            // place is still 2.6.
            if (BigDecimalHelperFunctions.checkRoundingDigits(r.d, k = pr, rm)) {

                do {
                    sd += 10;
                    num = BigDecimalHelperFunctions.naturalLogarithm(arg, sd);
                    denominator = isBase10 ? BigDecimalHelperFunctions.getLn10(Ctor, sd + 10) : BigDecimalHelperFunctions.naturalLogarithm(@base, sd);
                    r = BigDecimalHelperFunctions.divide(num, denominator, sd, RoundingMode.ROUND_DOWN);

                    if (!inf) {

                        // Check for 14 nines from the 2nd rounding digit, as the first may be 4.
                        var sliceToParse = BigDecimalHelperFunctions.digitsToString(r.d).Slice(k + 1, k + 15);
                        long parsedSlice = string.IsNullOrEmpty(sliceToParse) ? 0 : long.Parse(sliceToParse, CultureInfo.InvariantCulture);
                        if (parsedSlice + 1 == 1e14) {
                            r = BigDecimalHelperFunctions.finalise(r, pr + 1, 0);
                        }

                        break;
                    }
                } while (BigDecimalHelperFunctions.checkRoundingDigits(r.d, k += 10, rm));
            }

            Ctor.external = true;

            return BigDecimalHelperFunctions.finalise(r, pr, rm);
        }
        /// <inheritdoc cref="Log" />
        public BigDecimal Logarithm(BigDecimalArgument<BigDecimal> @base) => Log(@base);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal minus `y`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Sub(BigDecimalArgument<BigDecimal> y) => _sub(new BigDecimal(y, this.Config));
        private BigDecimal _sub(BigDecimal y) {
            int[] d, xd, yd;
            long i, j, k, e, xe, len, pr;
            RoundingMode rm;
            bool xLTy;
            var x = this;
            var Ctor = x.Config;

            // If either is not finite...
            if (!x.d.IsTrue() || !y.d.IsTrue()) {

                // Return null if either is null.
                if (!x.s.IsTrue() || !y.s.IsTrue()) y = new BigDecimal(double.NaN, Ctor);

                // Return y negated if x is finite and y is ±Infinity.
                else if (x.d.IsTrue()) y.s = -y.s;

                // Return x if y is finite and x is ±Infinity.
                // Return x if both are ±Infinity with different signs.
                // Return null if both are ±Infinity with the same sign.
                else
                {
                    if (y.d.IsTrue() || x.s != y.s)
                        y = new BigDecimal(x, Ctor);
                    else
                        y = new BigDecimal(double.NaN, Ctor);
                }

                return y;
            }

            // If signs differ...
            if (x.s != y.s) {
                y.s = -y.s;
                return x.Plus(y);
            }

            xd = x.d;
            yd = y.d;
            pr = Ctor.Precision;
            rm = Ctor.Rounding;

            // If either is zero...
            if (!xd[0].IsTrue() || !yd[0].IsTrue()) {

                // Return y negated if x is zero and y is non-zero.
                if (yd[0].IsTrue()) y.s = -y.s;

                // Return x if y is zero and x is non-zero.
                else if (xd[0].IsTrue()) y = new BigDecimal(x, Ctor);

                // Return zero if both are zero.
                // From IEEE 754 (2008) 6.3: 0 - 0 = -0 - -0 = -0 when rounding to -Infinity.
                else return new BigDecimal(rm == RoundingMode.ROUND_FLOOR ? "-0" : "0", Ctor);

                return Ctor.external ? BigDecimalHelperFunctions.finalise(y, pr, rm) : y;
            }

            // x and y are finite, non-zero numbers with the same sign.

            // Calculate @base 1e7 exponents.
            e = (long)Math.Floor((double)y.e / BigDecimalHelperFunctions.LOG_BASE);
            xe = (long)Math.Floor((double)x.e / BigDecimalHelperFunctions.LOG_BASE);

            xd = xd.Slice();
            k = xe - e;

            // If @base 1e7 exponents differ...
            if (k.IsTrue()) {
                xLTy = k < 0;

                if (xLTy) {
                    d = xd;
                    k = -k;
                    len = yd.LongLength;
                } else {
                    d = yd;
                    e = xe;
                    len = xd.LongLength;
                }

                // Numbers with massively different exponents would result in a very high number of
                // zeros needing to be prepended, but this can be avoided while still ensuring correct
                // rounding by limiting the number of zeros to `Math.ceil(pr / BigDecimalFactory.LOG_BASE) + 2`.
                i = (long)Math.Max(Math.Ceiling((double)pr / BigDecimalHelperFunctions.LOG_BASE), len) + 2;

                if (k > i) {
                    k = i;
                    ArrayExtensions.Resize(ref d, 1);
                }

                // Prepend zeros to equalise exponents.
                ArrayExtensions.Reverse(ref d);
                for (i = k; i--.IsTrue();) ArrayExtensions.Push(ref d, 0);
                ArrayExtensions.Reverse(ref d);

                if (xLTy) xd = d;
                else yd = d;

                // BigDecimalFactory.BASE 1e7 exponents equal.
            } else {

                // Check digits to determine which is the bigger number.

                i = xd.LongLength;
                len = yd.LongLength;
                xLTy = i < len;
                if (xLTy) len = i;

                for (i = 0; i < len; i++) {
                    if (xd[i] != yd[i]) {
                        xLTy = xd[i] < yd[i];
                        break;
                    }
                }

                k = 0;
            }

            if (xLTy) {
                d = xd;
                xd = yd;
                yd = d;
                y.s = -y.s;
            }

            len = xd.LongLength;

            // Append zeros to `xd` if shorter.
            // Don"t add zeros to `yd` if shorter as subtraction only needs to start at `yd` length.
            for (i = yd.LongLength - len; i > 0; --i)
            {
                if (xd.LongLength <= len) ArrayExtensions.Resize(ref xd, len + 1);
                xd[len++] = 0;
            }

            // Subtract yd from xd.
            for (i = yd.LongLength; i > k;) {

                if (xd[--i] < yd[i]) {
                    for (j = i; j != 0 && xd[--j] == 0;) xd[j] = BigDecimalHelperFunctions.BASE - 1;
                    --xd[j];
                    xd[i] += BigDecimalHelperFunctions.BASE;
                }

                xd[i] -= yd[i];
            }

            // Remove trailing zeros.
            for (; len > 0 && xd[--len] == 0;) ArrayExtensions.Pop(ref xd);

            // Remove leading zeros and adjust exponent accordingly.
            for (; xd.LongLength > 0 && xd[0] == 0; ArrayExtensions.Shift(ref xd)) --e;

            // Zero?
            if (xd.LongLength == 0 || !xd[0].IsTrue()) return new BigDecimal(rm == RoundingMode.ROUND_FLOOR ? "-0" : "0", Ctor);

            y.d = xd;
            y.e = BigDecimalHelperFunctions.getBase10Exponent(xd, e);

            return Ctor.external ? BigDecimalHelperFunctions.finalise(y, pr, rm) : y;
        }
        /// <inheritdoc cref="Sub" />
        public BigDecimal Minus(BigDecimalArgument<BigDecimal> y) => Sub(y);


        /// <summary>
        /// The result depends on the modulo mode.
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal modulo `y`, rounded to
        /// `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Mod(BigDecimalArgument<BigDecimal> y) => _mod(new BigDecimal(y, this.Config));
        private BigDecimal _mod(BigDecimal y) {
            BigDecimal q;
            var x = this;
            var Ctor = x.Config;

            // Return null if x is ±Infinity or null, or y is null or ±0.
            if (!x.d.IsTrue() || !y.s.IsTrue() || y.d.IsTrue() && !y.d[0].IsTrue()) return new BigDecimal(double.NaN, Ctor);

            // Return x if y is ±Infinity or x is ±0.
            if (!y.d.IsTrue() || x.d.IsTrue() && !x.d[0].IsTrue()) {
                return BigDecimalHelperFunctions.finalise(new BigDecimal(x, Ctor), Ctor.Precision, Ctor.Rounding);
            }

            // Prevent rounding of intermediate calculations.
            Ctor.external = false;

            if (Ctor.Modulo == 9) {

                // Euclidian division: q = Sign(y) * Floor(x / Abs(y))
                // result = x - q * y    where  0 <= result < Abs(y)
                q = BigDecimalHelperFunctions.divide(x, y.Abs(), 0, RoundingMode.ROUND_FLOOR, 1);
                q.s *= y.s;
            } else {
                q = BigDecimalHelperFunctions.divide(x, y, 0, (RoundingMode)Ctor.Modulo, 1);
            }

            q = q.Times(y);

            Ctor.external = true;

            return x.Minus(q);
        }
        /// <inheritdoc cref="Mod" />
        public BigDecimal Modulo(BigDecimalArgument<BigDecimal> y) => Mod(y);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the natural exponential of the value of this BigDecimal,
        /// i.e. the base e raised to the power the value of this BigDecimal, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Exp() {
            return BigDecimalHelperFunctions.naturalExponential(this);
        }
        /// <inheritdoc cref="Exp" />
        public BigDecimal NaturalExponential() => Exp();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the natural logarithm of the value of this BigDecimal,
        /// rounded to `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Ln() {
            return BigDecimalHelperFunctions.naturalLogarithm(this);
        }
        public BigDecimal NaturalLogarithm() => Ln();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>a new BigDecimal whose value is the value of this BigDecimal negated, i.e. as if multiplied by
        /// -1.</returns>
        public BigDecimal Neg() {
            var x = new BigDecimal(this, this.Config);
            x.s = -x.s;
            return BigDecimalHelperFunctions.finalise(x);
        }
        /// <inheritdoc cref="Neg" />
        public BigDecimal Negated() => Neg();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal plus `y`, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Add(BigDecimalArgument<BigDecimal> y) => _add(new BigDecimal(y, this.Config));
        private BigDecimal _add(BigDecimal y) {
            int carry;
            long pr, len, i, e, k;
            int[] d, xd, yd;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            // If either is not finite...
            if (!x.d.IsTrue() || !y.d.IsTrue()) {

                // Return null if either is null.
                if (!x.s.IsTrue() || !y.s.IsTrue()) y = new BigDecimal(double.NaN, Ctor);

                // Return x if y is finite and x is ±Infinity.
                // Return x if both are ±Infinity with the same sign.
                // Return null if both are ±Infinity with different signs.
                // Return y if x is finite and y is ±Infinity.
                else if (!x.d.IsTrue())
                {
                    if (y.d.IsTrue() || x.s == y.s)
                        y = new BigDecimal(x, Ctor);
                    else
                        y = new BigDecimal(double.NaN, Ctor);
                }

                return y;
            }

            // If signs differ...
            if (x.s != y.s) {
                y.s = -y.s;
                return x.Minus(y);
            }

            xd = x.d;
            yd = y.d;
            pr = Ctor.Precision;
            rm = Ctor.Rounding;

            // If either is zero...
            if (!xd[0].IsTrue() || !yd[0].IsTrue()) {

                // Return x if y is zero.
                // Return y if y is non-zero.
                if (!yd[0].IsTrue()) y = new BigDecimal(x, Ctor);

                return Ctor.external ? BigDecimalHelperFunctions.finalise(y, pr, rm) : y;
            }

            // x and y are finite, non-zero numbers with the same sign.

            // Calculate @base 1e7 exponents.
            k = (long)Math.Floor((double)x.e / BigDecimalHelperFunctions.LOG_BASE);
            e = (long)Math.Floor((double)y.e / BigDecimalHelperFunctions.LOG_BASE);

            xd = xd.Slice();
            i = k - e;

            // If @base 1e7 exponents differ...
            if (i.IsTrue()) {
                bool isXd = false;

                if (i < 0) {
                    d = xd;
                    i = -i;
                    len = yd.LongLength;
                    isXd = true;
                } else {
                    d = yd;
                    e = k;
                    len = xd.LongLength;
                }

                // Limit number of zeros prepended to Max(ceil(pr / BigDecimalFactory.LOG_BASE), len) + 1.
                k = (long)Math.Ceiling((double)pr / BigDecimalHelperFunctions.LOG_BASE);
                len = k > len ? k + 1 : len + 1;

                if (i > len) {
                    i = len;
                    ArrayExtensions.Resize(ref d, 1);
                }

                // Prepend zeros to equalise exponents. Note: Faster to use reverse then do unshifts.
                ArrayExtensions.Reverse(ref d);
                for (; i--.IsTrue();) ArrayExtensions.Push(ref d, 0);
                ArrayExtensions.Reverse(ref d);

                if (isXd) xd = d;
                else yd = d;
            }

            len = xd.LongLength;
            i = yd.LongLength;

            // If yd is longer than xd, swap xd and yd so xd points to the longer array.
            if (len - i < 0) {
                i = len;
                d = yd;
                yd = xd;
                xd = d;
            }

            // Only start adding at yd.LongLength - 1 as the further digits of xd can be left as they are.
            for (carry = 0; i.IsTrue();) {
                carry = ((xd[--i] = xd[i] + yd[i] + carry) / BigDecimalHelperFunctions.BASE) | 0;
                xd[i] %= BigDecimalHelperFunctions.BASE;
            }

            if (carry != 0) {
                ArrayExtensions.Unshift(ref xd, carry);
                ++e;
            }

            // Remove trailing zeros.
            // No need to check for zero, as +x + +y != 0 && -x + -y != 0
            for (len = xd.LongLength; xd[--len] == 0;) ArrayExtensions.Pop(ref xd);

            y.d = xd;
            y.e = BigDecimalHelperFunctions.getBase10Exponent(xd, e);

            return Ctor.external ? BigDecimalHelperFunctions.finalise(y, pr, rm) : y;
        }
        /// <inheritdoc cref="Add" />
        public BigDecimal Plus(BigDecimalArgument<BigDecimal> y) => Add(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="z">Whether to count integer-part trailing zeros.</param>
        /// <returns>The number of significant digits of the value of this BigDecimal.</returns>
        public long? Sd(bool? z = null) => Sd(z.HasValue && z.Value ? 1 : 0);
        /// <inheritdoc cref="Sd" />
        public long? Sd(long? z) {
            long? k;
            var x = this;

            if (z != null && z != 1 && z != 0) throw new BigDecimalException(BigDecimalException.InvalidArgument + z);

            if (x.d.IsTrue()) {
                k = BigDecimalHelperFunctions.getPrecision(x.d);
                if (z.IsTrue() && x.e + 1 > k) k = x.e + 1;
            } else {
                k = null;
            }

            return k;
        }
        /// <inheritdoc cref="Sd" />
        public long? Precision(bool? z = null) => Sd(z);
        /// <inheritdoc cref="Sd" />
        public long? Precision(long? z) => Sd(z);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal rounded to a whole number using
        /// rounding mode `rounding`.</returns>
        public BigDecimal Round() {
            var x = this;
            var Ctor = x.Config;

            return BigDecimalHelperFunctions.finalise(new BigDecimal(x, Ctor), x.e + 1, Ctor.Rounding);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the sine of the value in radians of this BigDecimal.</returns>
        public BigDecimal Sin() {
            return sin(out _);
        }
        internal BigDecimal sin(out int quadrant)
        {
            quadrant = 0;

            long pr;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (!x.IsFinite()) return new BigDecimal(double.NaN, Ctor);
            if (x.IsZero()) return new BigDecimal(x, Ctor);

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + Math.Max(x.e ?? 0, x.Sd() ?? 0) + BigDecimalHelperFunctions.LOG_BASE;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            x = BigDecimalHelperFunctions.sine(Ctor, BigDecimalHelperFunctions.toLessThanHalfPi(Ctor, x, out quadrant));

            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return BigDecimalHelperFunctions.finalise(quadrant > 2 ? x.Neg() : x, pr, rm, true);
        }
        /// <inheritdoc cref="Sin" />
        public BigDecimal Sine() => Sin();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the square root of this BigDecimal, rounded to `precision`
        /// significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Sqrt() {
            bool m = false;
            LongString n;
            long sd, rep = 0;
            BigDecimal r, t;
            var x = this;
            var d = x.d;
            long e = x.e ?? 0;
            var Ctor = x.Config;

            // Negative/null/Infinity/zero?
            if (x.s != 1 || !d.IsTrue() || !d[0].IsTrue()) {
                if (!x.s.IsTrue() || x.s < 0 && (!d.IsTrue() || d[0].IsTrue()))
                    return new BigDecimal(double.NaN, Ctor);
                else if (d.IsTrue())
                    return new BigDecimal(x, Ctor);
                else
                    return new BigDecimal(double.PositiveInfinity, Ctor);
            }

            double s = (double)x.s;

            Ctor.external = false;

            // Initial estimate.
            s = Math.Sqrt(x.ToNumber());

            // Math.sqrt underflow/overflow?
            // Pass x to Math.sqrt as integer, then adjust the exponent of the result.
            if (s == 0 || s == double.PositiveInfinity) {
                n = BigDecimalHelperFunctions.digitsToString(d);

                if ((n.LongLength + e) % 2 == 0) n += "0";
                s = Math.Sqrt(double.Parse(n, CultureInfo.InvariantCulture));
                e = (long)Math.Floor(((double)e + 1) / 2) - ((e < 0 || e % 2 != 0) ? 1 : 0);

                if (s == double.PositiveInfinity) {
                    n = "5e" + e;
                } else {
                    n = s.ToExponential();
                    n = n.Slice(0, n.IndexOf("e") + 1) + e;
                }

                r = new BigDecimal(n.ToString(), Ctor);
            } else {
                r = new BigDecimal(s.ToString(CultureInfo.InvariantCulture), Ctor);
            }

            sd = (e = Ctor.Precision) + 3;

            // Newton-Raphson iteration.
            for (; ; ) {
                t = r;
                r = t.Plus(BigDecimalHelperFunctions.divide(x, t, sd + 2, RoundingMode.ROUND_DOWN)).Times(0.5);

                // TODO? Replace with for-loop and checkRoundingDigits.
                if (BigDecimalHelperFunctions.digitsToString(t.d).Slice(0, sd) == (n = BigDecimalHelperFunctions.digitsToString(r.d)).Slice(0, sd)) {
                    n = n.Slice(sd - 3, sd + 1);

                    // The 4th rounding digit may be in error by -1 so if the 4 rounding digits are 9999 or
                    // 4999, i.e. approaching a rounding boundary, continue the iteration.
                    if (n == "9999" || !rep.IsTrue() && n == "4999") {

                        // On the first iteration only, check to see if rounding up gives the exact result as the
                        // nines may infinitely repeat.
                        if (!rep.IsTrue()) {
                            BigDecimalHelperFunctions.finalise(t, e + 1, RoundingMode.ROUND_UP);

                            if (t.Times(t).Eq(x)) {
                                r = t;
                                break;
                            }
                        }

                        sd += 4;
                        rep = 1;
                    } else {

                        // If the rounding digits are null, 0{0,4} or 50{0,3}, check for an exact result.
                        // If not, then there are further digits and m will be truthy.
                        double doubleN = 0;
                        double.TryParse(n, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleN);
                        double doubleN2 = 0;
                        double.TryParse(n.Slice(1), NumberStyles.Any, CultureInfo.InvariantCulture, out doubleN2);
                        if (!doubleN.IsTrue() || !doubleN2.IsTrue() && n.ElementAt(0) == '5') {

                            // Truncate to the first rounding digit.
                            BigDecimalHelperFunctions.finalise(r, e + 1, RoundingMode.ROUND_DOWN);
                            m = !r.Times(r).Eq(x);
                        }

                        break;
                    }
                }
            }

            Ctor.external = true;

            return BigDecimalHelperFunctions.finalise(r, e, Ctor.Rounding, m);
        }
        /// <inheritdoc cref="Sqrt" />
        public BigDecimal SquareRoot() => Sqrt();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the tangent of the value in radians of this BigDecimal.</returns>
        public BigDecimal Tan() {
            long pr;
            RoundingMode rm;
            var x = this;
            var Ctor = x.Config;

            if (!x.IsFinite()) return new BigDecimal(double.NaN, Ctor);
            if (x.IsZero()) return new BigDecimal(x, Ctor);

            pr = Ctor.Precision;
            rm = Ctor.Rounding;
            Ctor.Precision = pr + 10;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            x = x.sin(out var quadrant);
            x.s = 1;
            x = BigDecimalHelperFunctions.divide(x, new BigDecimal(1, Ctor).Minus(x.Times(x)).Sqrt(), pr + 10, RoundingMode.ROUND_UP);

            Ctor.Precision = pr;
            Ctor.Rounding = rm;

            return BigDecimalHelperFunctions.finalise(quadrant == 2 || quadrant == 4 ? x.Neg() : x, pr, rm, true);
        }
        /// <inheritdoc cref="Tan" />
        public BigDecimal Tangent() => Tan();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns>A new BigDecimal whose value is this BigDecimal times `y`, rounded to `precision` significant
        /// digits using rounding mode `rounding`.</returns>
        public BigDecimal Mul(BigDecimalArgument<BigDecimal> y) => _mul(new BigDecimal(y, this.Config));
        private BigDecimal _mul(BigDecimal y) {
            int carry = 0;
            long e, i, k, rL, xdL, ydL;
            long t;
            int[] r;
            var x = this;
            var Ctor = x.Config;
            var xd = x.d;
            var yd = y.d;

            y.s *= x.s;

            // If either is null, ±Infinity or ±0...
            if (!xd.IsTrue() || !xd[0].IsTrue() || !yd.IsTrue() || !yd[0].IsTrue()) {
                if (!y.s.IsTrue() || xd.IsTrue() && !xd[0].IsTrue() && !yd.IsTrue() || yd.IsTrue() && !yd[0].IsTrue() && !xd.IsTrue())
                {
                    // Return null if either is null.
                    // Return null if x is ±0 and y is ±Infinity, or y is ±0 and x is ±Infinity.
                    return new BigDecimal(double.NaN, Ctor);
                }
                else if (!xd.IsTrue() || !yd.IsTrue())
                {
                    // Return ±Infinity if either is ±Infinity.
                    return new BigDecimal((double)y.s / 0, Ctor);
                }
                else
                {
                    // Return ±0 if either is ±0.
                    return new BigDecimal(y.s >= 0 ? "+0" : "-0", Ctor);
                }
            }

            e = (long)Math.Floor((double)x.e / BigDecimalHelperFunctions.LOG_BASE) + (long)Math.Floor((double)y.e / BigDecimalHelperFunctions.LOG_BASE);
            xdL = xd.LongLength;
            ydL = yd.LongLength;

            // Ensure xd points to the longer array.
            if (xdL < ydL) {
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
            for (i = ydL; --i >= 0;) {
                carry = 0;
                for (k = xdL + i; k > i;) {
                    t = r[k] + (long)yd[i] * xd[k - i - 1] + carry;
                    r[k--] = (int)(t % BigDecimalHelperFunctions.BASE) | 0;
                    carry = (int)(t / BigDecimalHelperFunctions.BASE) | 0;
                }

                r[k] = (int)((r[k] + carry) % BigDecimalHelperFunctions.BASE) | 0;
            }

            // Remove trailing zeros.
            for (; rL > 0 && !r[--rL].IsTrue();) ArrayExtensions.Pop(ref r);

            if (carry.IsTrue()) ++e;
            else ArrayExtensions.Shift(ref r);

            y.d = r;
            y.e = BigDecimalHelperFunctions.getBase10Exponent(r, e);

            return Ctor.external ? BigDecimalHelperFunctions.finalise(y, Ctor.Precision, Ctor.Rounding) : y;
        }
        /// <inheritdoc cref="Mul" />
        public BigDecimal Times(BigDecimalArgument<BigDecimal> y) => Mul(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimal in base 2, round to `sd` significant
        /// digits using rounding mode `rm`.<br /><br />
        /// If the optional `sd` argument is present then return binary exponential notation.</returns>
        public string ToBinary(long? sd = null, RoundingMode? rm = null) {
            return BigDecimalHelperFunctions.toStringBinary(this, 2, sd, rm);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp">Decimal places.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal rounded to a maximum of `dp`
        /// decimal places using rounding mode `rm` or `rounding` if `rm` is omitted.<br /><br />
        /// If `dp` is omitted, return a new BigDecimal whose value is the value of this Decimal.</returns>
        public BigDecimal ToDP(long? dp = null, RoundingMode? rm = null) {
            var x = this;
            var Ctor = x.Config;

            x = new BigDecimal(x, Ctor);
            if (dp == null) return x;

            BigDecimalHelperFunctions.checkInt32(dp.Value, 0, BigDecimalHelperFunctions.MAX_DIGITS);

            if (rm == null) rm = Ctor.Rounding;

            return BigDecimalHelperFunctions.finalise(x, dp + x.e + 1, rm);
        }
        /// <inheritdoc cref="ToDP" />
        public BigDecimal ToDecimalPlaces(long? dp = null, RoundingMode? rm = null) => ToDP(dp, rm);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp">Decimal places.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimal in exponential notation rounded to
        /// `dp` fixed decimal places using rounding mode `rounding`.</returns>
        public string ToExponential(long? dp = null, RoundingMode? rm = null) {
            LongString str;
            var x = this;
            var Ctor = x.Config;

            if (dp == null) {
                str = BigDecimalHelperFunctions.finiteToString(x, true);
            } else {
                BigDecimalHelperFunctions.checkInt32(dp.Value, 0, BigDecimalHelperFunctions.MAX_DIGITS);

                if (rm == null) rm = Ctor.Rounding;

                x = BigDecimalHelperFunctions.finalise(new BigDecimal(x, Ctor), dp + 1, rm);
                str = BigDecimalHelperFunctions.finiteToString(x, true, dp + 1);
            }

            return x.IsNeg() && !x.IsZero() ? "-" + str : str;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp">Decimal places.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimal in normal (fixed-point) notation to
        /// `dp` fixed decimal places and rounded using rounding mode `rm` or `rounding` if `rm` is
        /// omitted.</returns>
        public string ToFixed(long? dp = null, RoundingMode? rm = null) {
            LongString str;
            BigDecimal y;
            var x = this;
            var Ctor = x.Config;

            if (dp == null) {
                str = BigDecimalHelperFunctions.finiteToString(x);
            } else {
                BigDecimalHelperFunctions.checkInt32(dp.Value, 0, BigDecimalHelperFunctions.MAX_DIGITS);

                if (rm == null) rm = Ctor.Rounding;

                y = BigDecimalHelperFunctions.finalise(new BigDecimal(x, Ctor), dp + x.e + 1, rm);
                str = BigDecimalHelperFunctions.finiteToString(y, false, dp + y.e + 1);
            }

            // To determine whether to add the minus sign look at the value before it was rounded,
            // i.e. look at `x` rather than `y`.
            return x.IsNeg() && !x.IsZero() ? "-" + str : str;
        }


        /// <summary>
        /// The denominator will be a positive non-zero value less than or equal to the specified maximum
        /// denominator. If a maximum denominator is not specified, the denominator will be the lowest
        /// value necessary to represent the number exactly.
        /// </summary>
        /// <param name="maxD">Maximum denominator.</param>
        /// <returns>An array representing the value of this BigDecimal as a simple fraction with a
        /// numerator and a denominator.</returns>
        /// <exception cref="BigDecimalException"></exception>
        public BigDecimal[] ToFraction(BigDecimalArgument<BigDecimal> maxD)
        {
            BigDecimal d, d1, n, n1, maxDr = null;
            long e, k;
            var x = this;
            var xd = x.d;
            var Ctor = x.Config;

            if (!xd.IsTrue()) return new[] { new BigDecimal(x, Ctor) };

            n1 = new BigDecimal(1, Ctor);
            d1 = new BigDecimal(0, Ctor);

            d = new BigDecimal(d1, Ctor);
            d.e = BigDecimalHelperFunctions.getPrecision(xd) - x.e - 1;
            e = d.e ?? 0;
            k = e % BigDecimalHelperFunctions.LOG_BASE;
            d.d[0] = (int)Math.Pow(10, k < 0 ? BigDecimalHelperFunctions.LOG_BASE + k : k);

            maxD.Switch(
                @double => {
                    n = new BigDecimal(@double, Ctor);
                    if (!n.IsInt() || n.Lt(n1)) throw new BigDecimalException(BigDecimalException.InvalidArgument + n);
                    maxDr = n.Gt(d) ? (e > 0 ? d : n1) : n;
                },
                @decimal => {
                    n = new BigDecimal(@decimal, Ctor);
                    if (!n.IsInt() || n.Lt(n1)) throw new BigDecimalException(BigDecimalException.InvalidArgument + n);
                    maxDr = n.Gt(d) ? (e > 0 ? d : n1) : n;
                },
                @long => {
                    n = new BigDecimal(@long, Ctor);
                    if (!n.IsInt() || n.Lt(n1)) throw new BigDecimalException(BigDecimalException.InvalidArgument + n);
                    maxDr = n.Gt(d) ? (e > 0 ? d : n1) : n;
                },
                @int => {
                    n = new BigDecimal(@int, Ctor);
                    if (!n.IsInt() || n.Lt(n1)) throw new BigDecimalException(BigDecimalException.InvalidArgument + n);
                    maxDr = n.Gt(d) ? (e > 0 ? d : n1) : n;
                },
                @string => {
                    if (string.IsNullOrEmpty(@string))
                    {
                        // d is 10**e, the minimum max-denominator needed.
                        maxDr = e > 0 ? d : n1;
                    }
                    else
                    {
                        n = new BigDecimal(@string, Ctor);
                        if (!n.IsInt() || n.Lt(n1)) throw new BigDecimalException(BigDecimalException.InvalidArgument + n);
                        maxDr = n.Gt(d) ? (e > 0 ? d : n1) : n;
                    }
                },
                bigInteger => {
                    n = new BigDecimal(bigInteger, Ctor);
                    if (!n.IsInt() || n.Lt(n1)) throw new BigDecimalException(BigDecimalException.InvalidArgument + n);
                    maxDr = n.Gt(d) ? (e > 0 ? d : n1) : n;
                },
                bigDecimal => {
                    if (bigDecimal == null)
                    {
                        // d is 10**e, the minimum max-denominator needed.
                        maxDr = e > 0 ? d : n1;
                    }
                    else
                    {
                        n = new BigDecimal(bigDecimal, Ctor);
                        if (!n.IsInt() || n.Lt(n1)) throw new BigDecimalException(BigDecimalException.InvalidArgument + n);
                        maxDr = n.Gt(d) ? (e > 0 ? d : n1) : n;
                    }
                }
            );

            return _toFraction(maxDr);
        }
        private BigDecimal[] _toFraction(BigDecimal maxD) {
            BigDecimal d, d0, d1, d2, n, n0, n1, q;
            long e, k, pr;
            BigDecimal[] r;
            var x = this;
            var xd = x.d;
            var Ctor = x.Config;

            if (!xd.IsTrue()) return new[] { new BigDecimal(x, Ctor) };

            n1 = d0 = new BigDecimal(1, Ctor);
            d1 = n0 = new BigDecimal(0, Ctor);

            d = new BigDecimal(d1, Ctor);
            d.e = BigDecimalHelperFunctions.getPrecision(xd) - x.e - 1;
            e = d.e ?? 0;
            k = e % BigDecimalHelperFunctions.LOG_BASE;
            d.d[0] = (int)Math.Pow(10, k < 0 ? BigDecimalHelperFunctions.LOG_BASE + k : k);

            Ctor.external = false;
            n = new BigDecimal(BigDecimalHelperFunctions.digitsToString(xd).ToString(), Ctor);
            pr = Ctor.Precision;
            Ctor.Precision = e = xd.LongLength * BigDecimalHelperFunctions.LOG_BASE * 2;

            for (; ; ) {
                q = BigDecimalHelperFunctions.divide(n, d, 0, RoundingMode.ROUND_DOWN, 1);
                d2 = d0.Plus(q.Times(d1));
                if (d2.Cmp(maxD) == 1) break;
                d0 = d1;
                d1 = d2;
                d2 = n1;
                n1 = n0.Plus(q.Times(d2));
                n0 = d2;
                d2 = d;
                d = n.Minus(q.Times(d2));
                n = d2;
            }

            d2 = BigDecimalHelperFunctions.divide(maxD.Minus(d0), d1, 0, RoundingMode.ROUND_DOWN, 1);
            n0 = n0.Plus(d2.Times(n1));
            d0 = d0.Plus(d2.Times(d1));
            n0.s = n1.s = x.s;

            // Determine which fraction is closer to x, n0/d0 or n1/d1?
            r = BigDecimalHelperFunctions.divide(n1, d1, e, RoundingMode.ROUND_DOWN).Minus(x).Abs().Cmp(BigDecimalHelperFunctions.divide(n0, d0, e, RoundingMode.ROUND_DOWN).Minus(x).Abs()) < 1
                ? new[] { n1, d1 } : new[] { n0, d0 };

            Ctor.Precision = pr;
            Ctor.external = true;

            return r;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimal in base 16, round to `sd` significant
        /// digits using rounding mode `rm`.<br /><br />
        /// If the optional `sd` argument is present then return binary exponential notation.</returns>
        public string ToHex(long? sd = null, RoundingMode? rm = null) {
            return BigDecimalHelperFunctions.toStringBinary(this, 16, sd, rm);
        }
        /// <inheritdoc cref="ToHex" />
        public string ToHexadecimal(long? sd = null, RoundingMode? rm = null) => ToHex(sd, rm);


        /// <summary>
        /// The return value will always have the same sign as this BigDecimal, unless either this BigDecimal
        /// or `y` is null, in which case the return value will be also be null.<br /><br />
        /// The return value is not affected by the value of `precision`.
        /// </summary>
        /// <param name="y">The magnitude to round to a multiple of. Default = 1.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A new BigDecimal whose value is the nearest multiple of `y` in the direction of rounding
        /// mode `rm`, or `BigDecimal.Config.Rounding` if `rm` is omitted, to the value of this BigDecimal.</returns>
        public BigDecimal ToNearest(BigDecimalArgument<BigDecimal> y, RoundingMode? rm = null)
        {
            return y.Match(
                @double => _toNearest(new BigDecimal(@double, this.Config), rm),
                @decimal => _toNearest(new BigDecimal(@decimal, this.Config), rm),
                @long => _toNearest(new BigDecimal(@long, this.Config), rm),
                @int => _toNearest(new BigDecimal(@int, this.Config), rm),
                @string =>
                {
                    if (string.IsNullOrEmpty(@string)) return _toNearest(new BigDecimal(1, this.Config), this.Config.Rounding);
                    return _toNearest(new BigDecimal(@string, this.Config), rm);
                },
                bigInteger => _toNearest(new BigDecimal(bigInteger, this.Config), rm),
                bigDecimal => {
                    if (bigDecimal == null) return _toNearest(new BigDecimal(1, this.Config), this.Config.Rounding);
                    return _toNearest(new BigDecimal(bigDecimal, this.Config), rm);
                }
            );
        }
        private BigDecimal _toNearest(BigDecimal y, RoundingMode? rm = null) {
            var x = this;
            var Ctor = x.Config;

            x = new BigDecimal(x, Ctor);

            if (rm == null) {
                rm = Ctor.Rounding;
            }

            // If x is not finite, return x if y is not null, else null.
            if (!x.d.IsTrue()) return y.s.IsTrue() ? x : y;

            // If y is not finite, return Infinity with the sign of x if y is Infinity, else null.
            if (!y.d.IsTrue()) {
                if (y.s.IsTrue()) y.s = x.s;
                return y;
            }

            // If y is not zero, calculate the nearest multiple of y to x.
            if (y.d[0].IsTrue()) {
                Ctor.external = false;
                x = BigDecimalHelperFunctions.divide(x, y, 0, rm, 1).Times(y);
                Ctor.external = true;
                BigDecimalHelperFunctions.finalise(x);

                // If y is zero, return zero with the sign of x.
            } else {
                y.s = x.s;
                x = y;
            }

            return x;
        }


        /// <summary>
        /// Zero is not keep its sign. (C# has not -0)
        /// </summary>
        /// <returns>The value of this BigDecimal converted to a number primitive.</returns>
        public double ToNumber() {
            var n = double.Parse(this.ToString(), CultureInfo.InvariantCulture);
            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimal in base 8, round to `sd` significant
        /// digits using rounding mode `rm`.<br /><br />
        /// If the optional `sd` argument is present then return binary exponential notation.</returns>
        public string ToOctal(long? sd = null, RoundingMode? rm = null) {
            return BigDecimalHelperFunctions.toStringBinary(this, 8, sd, rm);
        }


        /// <summary>
        /// For non-integer or very large exponents Pow(x, y) is calculated using<br /><br />
        /// x^y = Exp(y * ln(x))<br /><br />
        /// Assuming the first 15 rounding digits are each equally likely to be any digit 0-9, the
        /// probability of an incorrectly rounded result<br /><br />
        /// P([49]9{14} | [50]0{14}) = 2 * 0.2 * 10^-14 = 4e-15 = 1/2.5e+14<br />
        /// i.e. 1 in 250,000,000,000,000<br /><br />
        /// If a result is incorrectly rounded the maximum error will be 1 ulp (unit in last place).
        /// </summary>
        /// <param name="y">The power to which to raise this BigDecimal.</param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal raised to the power `y`, rounded
        /// to `precision` significant digits using rounding mode `rounding`.</returns>
        public BigDecimal Pow(BigDecimalArgument<BigDecimal> y) => _pow(new BigDecimal(y, this.Config));
        private BigDecimal _pow(BigDecimal y) {
            long e, k, pr;
            BigDecimal r;
            RoundingMode rm;
            int s;
            var x = this;
            var Ctor = x.Config;
            var yn = y.ToNumber();

            var xn = x.ToNumber();
            int? sign = xn == 0 && Math.Abs(yn) % 2 == 1 ? x.s : null;
            // Either ±Infinity, null or ±0?
            if (!x.d.IsTrue() || !y.d.IsTrue() || !x.d[0].IsTrue() || !y.d[0].IsTrue()) return new BigDecimal((sign.HasValue ? sign.Value : 1) * Math.Pow(xn, yn), Ctor);

            x = new BigDecimal(x, Ctor);

            if (x.Eq(1)) return x;

            pr = Ctor.Precision;
            rm = Ctor.Rounding;

            if (y.Eq(1)) return BigDecimalHelperFunctions.finalise(x, pr, rm);

            // y exponent
            e = (long)Math.Floor((double)y.e / BigDecimalHelperFunctions.LOG_BASE);

            //BigInteger
            var k0 = double.IsInfinity(yn) ? BigDecimalHelperFunctions.MAX_SAFE_INTEGER + 1 : new BigInteger(yn);
            k0 = (k0 < 0 ? -k0 : k0);
            // If y is a small integer use the "exponentiation by squaring" algorithm.
            if (e >= y.d.LongLength - 1 && k0 <= BigDecimalHelperFunctions.MAX_SAFE_INTEGER) {
                r = BigDecimalHelperFunctions.intPow(Ctor, x, (long)k0, pr);
                return y.s < 0 ? new BigDecimal(1, Ctor).Div(r) : BigDecimalHelperFunctions.finalise(r, pr, rm);
            }

            s = x.s ?? 0;

            // if x is negative
            if (s < 0) {

                // if y is not an integer
                if (e < y.d.LongLength - 1) return new BigDecimal(double.NaN, Ctor);

                // Result is positive if x is negative and the last digit of integer y is even.
                if (y.d.LongLength <= e || (y.d[e] & 1) == 0) s = 1;

                // if x.eq(-1)
                if (x.e == 0 && x.d[0] == 1 && x.d.LongLength == 1) {
                    x.s = s;
                    return x;
                }
            }

            // Estimate result exponent.
            // x^y = 10^e,  where e = y * Log10(x)
            // Log10(x) = Log10(x_significand) + x_exponent
            // Log10(x_significand) = Ln(x_significand) / Ln(10)
            var kd = Math.Pow(x.ToNumber(), yn);
            e = kd == 0 || !double.IsFinite(kd)
              ? (long)Math.Floor(yn * (Math.Log(double.Parse("0." + BigDecimalHelperFunctions.digitsToString(x.d), CultureInfo.InvariantCulture)) / BigDecimalHelperFunctions.LN10d + (x.e ?? 0) + 1))
              : new BigDecimal(kd.ToString(CultureInfo.InvariantCulture), Ctor).e ?? 0;

            // Exponent estimate may be incorrect e.g. x: 0.999999999999999999, y: 2.29, e: 0, r.e: -1.

            // Overflow/underflow?
            if (e > Ctor.MaxE + 1 || e < Ctor.MinE - 1) return new BigDecimal(e > 0 ? (double)s / 0 : 0, Ctor);

            Ctor.external = false;
            x.s = 1;
            Ctor.Rounding = RoundingMode.ROUND_DOWN;

            // Estimate the extra guard digits needed to ensure five correct rounding digits from
            // naturalLogarithm(x). Example of failure without these extra digits (precision: 10):
            // new Decimal(2.32456).pow("2087987436534566.46411")
            // should be 1.162377823e+764914905173815, but is 1.162355823e+764914905173815
            k = Math.Min(12, e.ToString(CultureInfo.InvariantCulture).Length);

            // r = x^y = Exp(y*ln(x))
            r = BigDecimalHelperFunctions.naturalExponential(y.Times(BigDecimalHelperFunctions.naturalLogarithm(x, pr + k)), pr);

            // r may be Infinity, e.g. (0.9999999999999999).pow(-1e+40)
            if (r.d.IsTrue()) {

                // Truncate to the required precision plus five rounding digits.
                r = BigDecimalHelperFunctions.finalise(r, pr + 5, RoundingMode.ROUND_DOWN);

                // If the rounding digits are [49]9999 or [50]0000 increase the precision by 10 and recalculate
                // the result.
                if (BigDecimalHelperFunctions.checkRoundingDigits(r.d, pr, rm)) {
                    e = pr + 10;

                    // Truncate to the increased precision plus five rounding digits.
                    r = BigDecimalHelperFunctions.finalise(BigDecimalHelperFunctions.naturalExponential(y.Times(BigDecimalHelperFunctions.naturalLogarithm(x, e + k)), e), e + 5, RoundingMode.ROUND_DOWN);

                    // Check for 14 nines from the 2nd rounding digit (the first rounding digit may be 4 or 9).
                    var sliceToParse = BigDecimalHelperFunctions.digitsToString(r.d).Slice(pr + 1, pr + 15);
                    long parsedSlice = string.IsNullOrEmpty(sliceToParse) ? 0 : long.Parse(sliceToParse, CultureInfo.InvariantCulture);
                    if (parsedSlice + 1 == 1e14) {
                        r = BigDecimalHelperFunctions.finalise(r, pr + 1, RoundingMode.ROUND_UP);
                    }
                }
            }

            r.s = s;
            Ctor.external = true;
            Ctor.Rounding = rm;

            return BigDecimalHelperFunctions.finalise(r, pr, rm);
        }
        /// <inheritdoc cref="Pow" />
        public BigDecimal ToPower(BigDecimalArgument<BigDecimal> y) => Pow(y);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A string representing the value of this BigDecimal rounded to `sd` significant digits
        /// using rounding mode `rounding`.<br /><br />
        /// Exponential notation if `sd` is less than the number of digits necessary to represent
        /// the integer part of the value in normal notation.</returns>
        public string ToPrecision(long? sd = null, RoundingMode? rm = null) {
            LongString str;
            var x = this;
            var Ctor = x.Config;

            if (sd == null) {
                str = BigDecimalHelperFunctions.finiteToString(x, x.e <= Ctor.ToExpNeg || x.e >= Ctor.ToExpPos);
            } else {
                BigDecimalHelperFunctions.checkInt32(sd.Value, 1, BigDecimalHelperFunctions.MAX_DIGITS);

                if (rm == null) rm = Ctor.Rounding;

                x = BigDecimalHelperFunctions.finalise(new BigDecimal(x, Ctor), sd, rm);
                str = BigDecimalHelperFunctions.finiteToString(x, sd <= x.e || x.e <= Ctor.ToExpNeg, sd);
            }

            return x.IsNeg() && !x.IsZero() ? "-" + str : str;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd">Significant digits.</param>
        /// <param name="rm">Rounding mode.</param>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal rounded to a maximum of `sd`
        /// significant digits using rounding mode `rm`, or to `precision` and `rounding` respectively if
        /// omitted.</returns>
        public BigDecimal ToSD(long? sd = null, RoundingMode? rm = null) {
            var x = this;
            var Ctor = x.Config;

            if (sd == null) {
                sd = Ctor.Precision;
                rm = Ctor.Rounding;
            } else {
                BigDecimalHelperFunctions.checkInt32(sd.Value, 1, BigDecimalHelperFunctions.MAX_DIGITS);

                if (rm == null) rm = Ctor.Rounding;
            }

            return BigDecimalHelperFunctions.finalise(new BigDecimal(x, Ctor), sd, rm);
        }
        /// <inheritdoc cref="ToSD" />
        public BigDecimal ToSignificantDigits(long? sd = null, RoundingMode? rm = null) => ToSD(sd, rm);


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A string representing the value of this BigDecimal.<br /><br />
        /// Exponential notation if this BigDecimal has a positive exponent equal to or greater than
        /// `ToExpPos`, or a negative exponent equal to or less than `ToExpNeg`.</returns>
        public override string ToString() {
            var x = this;
            var Ctor = x.Config;
            var str = BigDecimalHelperFunctions.finiteToString(x, x.e <= Ctor.ToExpNeg || x.e >= Ctor.ToExpPos);

            return x.IsNeg() && !x.IsZero() ? "-" + str : str;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimal whose value is the value of this BigDecimal truncated to a whole number.</returns>
        public BigDecimal Trunc() {
            return BigDecimalHelperFunctions.finalise(new BigDecimal(this, this.Config), this.e + 1, RoundingMode.ROUND_DOWN);
        }
        /// <inheritdoc cref="Trunc" />
        public BigDecimal Truncated() => Trunc();


        /// <summary>
        /// Unlike `ToString`, negative zero will include the minus sign.
        /// </summary>
        /// <returns>A string representing the value of this BigDecimal.</returns>
        public string ToJSON() {
            var x = this;
            var Ctor = x.Config;
            var str = BigDecimalHelperFunctions.finiteToString(x, x.e <= Ctor.ToExpNeg || x.e >= Ctor.ToExpPos);

            return x.IsNeg() ? "-" + str : str;
        }
        /// <inheritdoc cref="ToJSON" />
        public string ValueOf() => ToJSON();


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
        /// <returns>A new BigDecimal whose value and Config have been cloned.</returns>
        public BigDecimal Clone()
        {
            return new BigDecimal(this, this.Config.Clone());
        }
    }
}