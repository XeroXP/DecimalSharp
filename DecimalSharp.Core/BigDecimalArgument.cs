using System.Numerics;

namespace DecimalSharp.Core
{
    public struct BigDecimalArgument<TBigDecimal>
    {
        readonly double _value0;
        readonly decimal _value1;
        readonly long _value2;
        readonly int _value3;
        readonly string? _value4;
        readonly BigInteger _value5;
        readonly TBigDecimal? _value6;
        readonly int _index;

        BigDecimalArgument(int index, double value0 = default, decimal value1 = default, long value2 = default, int value3 = default, string? value4 = default, BigInteger value5 = default, TBigDecimal? value6 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
        }

        public static implicit operator BigDecimalArgument<TBigDecimal>(double t) => new(0, value0: t);
        public static implicit operator BigDecimalArgument<TBigDecimal>(decimal t) => new(1, value1: t);
        public static implicit operator BigDecimalArgument<TBigDecimal>(long t) => new(2, value2: t);
        public static implicit operator BigDecimalArgument<TBigDecimal>(int t) => new(3, value3: t);
        public static implicit operator BigDecimalArgument<TBigDecimal>(string? t) => new(4, value4: t);
        public static implicit operator BigDecimalArgument<TBigDecimal>(BigInteger t) => new(5, value5: t);
        public static implicit operator BigDecimalArgument<TBigDecimal>(TBigDecimal? t) => new(6, value6: t);

        public void Switch(Action<double> f0, Action<decimal> f1, Action<long> f2, Action<int> f3, Action<string?> f4, Action<BigInteger> f5, Action<TBigDecimal?> f6)
        {
            if (_index == 0 && f0 != null)
            {
                f0(_value0);
                return;
            }
            if (_index == 1 && f1 != null)
            {
                f1(_value1);
                return;
            }
            if (_index == 2 && f2 != null)
            {
                f2(_value2);
                return;
            }
            if (_index == 3 && f3 != null)
            {
                f3(_value3);
                return;
            }
            if (_index == 4 && f4 != null)
            {
                f4(_value4);
                return;
            }
            if (_index == 5 && f5 != null)
            {
                f5(_value5);
                return;
            }
            if (_index == 6 && f6 != null)
            {
                f6(_value6);
                return;
            }
            throw new InvalidOperationException();
        }

        public TResult Match<TResult>(Func<double, TResult> f0, Func<decimal, TResult> f1, Func<long, TResult> f2, Func<int, TResult> f3, Func<string?, TResult> f4, Func<BigInteger, TResult> f5, Func<TBigDecimal?, TResult> f6)
        {
            if (_index == 0 && f0 != null)
            {
                return f0(_value0);
            }
            if (_index == 1 && f1 != null)
            {
                return f1(_value1);
            }
            if (_index == 2 && f2 != null)
            {
                return f2(_value2);
            }
            if (_index == 3 && f3 != null)
            {
                return f3(_value3);
            }
            if (_index == 4 && f4 != null)
            {
                return f4(_value4);
            }
            if (_index == 5 && f5 != null)
            {
                return f5(_value5);
            }
            if (_index == 6 && f6 != null)
            {
                return f6(_value6);
            }
            throw new InvalidOperationException();
        }
    }
}
