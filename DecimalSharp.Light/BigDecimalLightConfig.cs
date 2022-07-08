using DecimalSharp.Core;

namespace DecimalSharp
{
    public class BigDecimalLightConfig
    {
        // 1 to MAX_DIGITS
        private long _precision = 20;
        /// <summary>
        /// The maximum number of significant digits of the result of a calculation or base conversion.
        /// </summary>
        public long Precision {
            get { return _precision; }
            set {
                if (value < 1 || value > BigDecimalLightHelperFunctions.MAX_DIGITS)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(Precision) + ": " + value);
                _precision = value;
            }
        }

        /// <summary>
        /// The rounding mode used when rounding to `precision`.
        /// </summary>
        public RoundingMode Rounding { get; set; } = RoundingMode.ROUND_HALF_UP;

        // 0 to -MAX_E
        private long _toExpNeg = -7;
        /// <summary>
        /// The exponent value at and beneath which `ToString` returns exponential notation.
        /// </summary>
        public long ToExpNeg {
            get { return _toExpNeg; }
            set {
                if (value < -BigDecimalLightHelperFunctions.MAX_E || value > 0)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(ToExpNeg) + ": " + value);
                _toExpNeg = value;
            }
        }

        // 0 to MAX_E
        private long _toExpPos = 21;
        /// <summary>
        /// The exponent value at and above which `ToString` returns exponential notation.
        /// </summary>
        public long ToExpPos {
            get { return _toExpPos; }
            set {
                if (value < 0 || value > BigDecimalLightHelperFunctions.MAX_E)
                    throw new BigDecimalException(BigDecimalException.InvalidArgument + nameof(ToExpPos) + ": " + value);
                _toExpPos = value;
            }
        }

        internal bool external = true;

        public BigDecimalLightConfig Clone()
        {
            return new BigDecimalLightConfig
            {
                Precision = Precision,
                Rounding = Rounding,
                ToExpNeg = ToExpNeg,
                ToExpPos = ToExpPos,
                external = external
            };
        }
    }
}
