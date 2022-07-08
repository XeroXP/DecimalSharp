using DecimalSharp.Core.Extensions;

namespace DecimalSharp.Core
{
    public class BigDecimalDivisionFunctions
    {
        // Assumes non-zero x and k, and hence non-zero result.
        protected static int[] multiplyInteger(int[] x, long k, int @base)
        {
            long temp;
            int carry = 0;
            long i = x.LongLength;

            for (x = x.Slice(); i--.IsTrue();)
            {
                temp = (long)x[i] * k + carry;
                x[i] = (int)(temp % @base) | 0;
                carry = (int)(temp / @base) | 0;
            }

            if (carry.IsTrue()) ArrayExtensions.Unshift(ref x, carry);

            return x;
        }

        protected static int compare(int[] a, int[] b, long aL, long bL)
        {
            long i;
            int r;

            if (aL != bL)
            {
                r = aL > bL ? 1 : -1;
            }
            else
            {
                for (i = r = 0; i < aL; i++)
                {
                    if (a[i] != b[i])
                    {
                        r = a[i] > b[i] ? 1 : -1;
                        break;
                    }
                }
            }

            return r;
        }

        protected static void subtract(ref int[] a, int[] b, long aL, int @base)
        {
            var i = 0;

            // Subtract b from a.
            for (; aL--.IsTrue();)
            {
                a[aL] -= i;
                i = a[aL] < b[aL] ? 1 : 0;
                a[aL] = i * @base + a[aL] - b[aL];
            }

            // Remove leading zeros.
            for (; !a[0].IsTrue() && a.LongLength > 1;) ArrayExtensions.Shift(ref a);
        }
    }
}
