using DecimalSharp.Core;

namespace DecimalSharp
{
    public class BigDecimalConfig
    {
        // 1 to MAX_DIGITS
        private long _precision = 20;
        /// <summary>
        /// The maximum number of significant digits of the result of a calculation or base conversion.
        /// </summary>
        public long Precision {
            get { return _precision; }
            set {
                if (value < 1 || value > BigDecimalHelperFunctions.MAX_DIGITS)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(Precision) + ": " + value);
                _precision = value;
            }
        }

        /// <summary>
        /// The rounding mode used when rounding to `precision`.
        /// </summary>
        public RoundingMode Rounding { get; set; } = RoundingMode.ROUND_HALF_UP;

        // 0 to 9
        private int _modulo = 1;
        /// <summary>
        /// The modulo mode used when calculating the modulus: a mod n.<br />
        /// The quotient (q = a / n) is calculated according to the corresponding rounding mode.<br />
        /// The remainder (r) is calculated as: r = a - n * q.<br />
        /// <br />
        /// UP         0 The remainder is positive if the dividend is negative, else is negative.<br />
        /// DOWN       1 The remainder has the same sign as the dividend (JavaScript %).<br />
        /// FLOOR      3 The remainder has the same sign as the divisor (Python %).<br />
        /// HALF_EVEN  6 The IEEE 754 remainder function.<br />
        /// EUCLID     9 Euclidian division. q = Sign(n) * Floor(a / Abs(n)). Always positive.<br />
        /// <br />
        /// Truncated division (1), floored division (3), the IEEE 754 remainder (6), and Euclidian<br />
        /// division (9) are commonly used for the modulus operation. The other rounding modes can also<br />
        /// be used, but they may not give useful results.<br />
        /// </summary>
        public int Modulo {
            get { return _modulo; }
            set {
                if (value < 0 || value > 9)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(Modulo) + ": " + value);
                _modulo = value;
            }
        }

        // 0 to -EXP_LIMIT
        private long _toExpNeg = -7;
        /// <summary>
        /// The exponent value at and beneath which `ToString` returns exponential notation.
        /// </summary>
        public long ToExpNeg {
            get { return _toExpNeg; }
            set {
                if (value < -BigDecimalHelperFunctions.EXP_LIMIT || value > 0)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(ToExpNeg) + ": " + value);
                _toExpNeg = value;
            }
        }

        // 0 to EXP_LIMIT
        private long _toExpPos = 21;
        /// <summary>
        /// The exponent value at and above which `ToString` returns exponential notation.
        /// </summary>
        public long ToExpPos {
            get { return _toExpPos; }
            set {
                if (value < 0 || value > BigDecimalHelperFunctions.EXP_LIMIT)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(ToExpPos) + ": " + value);
                _toExpPos = value;
            }
        }

        // -1 to -EXP_LIMIT
        private long _minE = -BigDecimalHelperFunctions.EXP_LIMIT;
        public long MinE {
            get { return _minE; }
            set {
                if (value < -BigDecimalHelperFunctions.EXP_LIMIT || value > 0)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(MinE) + ": " + value);
                _minE = value;
            }
        }

        // 1 to EXP_LIMIT
        private long _maxE = BigDecimalHelperFunctions.EXP_LIMIT;
        /// <summary>
        /// The maximum exponent value, above which overflow to Infinity occurs.
        /// </summary>
        public long MaxE {
            get { return _maxE; }
            set {
                if (value < 0 || value > BigDecimalHelperFunctions.EXP_LIMIT)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(MaxE) + ": " + value);
                _maxE = value;
            }
        }

        /// <summary>
        /// Whether to use cryptographically-secure random number generation.
        /// </summary>
        public bool Crypto { get; set; } = false;

        internal bool external = true;

        public BigDecimalConfig Clone()
        {
            return new BigDecimalConfig
            {
                Precision = Precision,
                Rounding = Rounding,
                Modulo = Modulo,
                ToExpNeg = ToExpNeg,
                ToExpPos = ToExpPos,
                MinE = MinE,
                MaxE = MaxE,
                Crypto = Crypto,
                external = external
            };
        }
    }
}
