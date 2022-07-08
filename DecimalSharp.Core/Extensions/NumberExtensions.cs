using System.Globalization;

namespace DecimalSharp.Core.Extensions
{
    public static class NumberExtensions
    {
        public static string ToExponential(this double? value, int? fractionDigits = null)
        {
            if (!value.HasValue) return "";

            return value.Value.ToExponential(fractionDigits);
        }

        public static string ToExponential(this double value, int? fractionDigits = null)
        {
            if (!fractionDigits.HasValue)
            {
                return value.ToString("0.####################e+0", CultureInfo.InvariantCulture);
            }

            return value.ToString("0." + string.Empty.PadRight(fractionDigits.Value, '0') + "e+0", CultureInfo.InvariantCulture);
        }

        /*public static bool IsTrue<T>(this T[] value)
        {
            return value.LongLength != 0;
        }*/

        public static bool IsTrue<T>(this T[]? value)
        {
            return value != null && value.LongLength != 0;
        }

        public static bool IsTrue<T>(this T value) where T : struct
        {
            return !value.Equals(default(T));
        }

        public static bool IsTrue<T>(this T? value) where T : struct
        {
            if (value == null) return false;

            return value.HasValue && !value.Value.Equals(default(T));
        }
    }
}
