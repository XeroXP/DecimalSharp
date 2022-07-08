using DecimalSharp.Core;
using DecimalSharp.Core.Extensions;

namespace DecimalSharp
{
    internal class BigDecimalDivision : BigDecimalDivisionFunctions
    {
        public static BigDecimal Divide(BigDecimal x, BigDecimal y, long? pr, RoundingMode? rm, long? dp, int? @base, out bool inexact)
        {
            inexact = false;

            long cmp, e, i, k, logBase, prodL, remL, sd, xi, xL, yd0, yL;
            long rem0;
            double t;
            bool more;
            BigDecimal q;
            int[] qd, prod, rem, yz;
            var Ctor = x.Config;
            var sign = x.s == y.s ? 1 : -1;
            var xd = x.d;
            var yd = y.d;

            // Either null, Infinity or 0?
            if (!xd.IsTrue() || !xd[0].IsTrue() || !yd.IsTrue() || !yd[0].IsTrue())
            {
                if (!x.s.IsTrue() || !y.s.IsTrue() || (xd.IsTrue() ? (yd.IsTrue() && xd[0] == yd[0]) : !yd.IsTrue()))
                    return new BigDecimal(double.NaN, Ctor);
                else if (xd.IsTrue() && xd[0] == 0 || !yd.IsTrue())
                    return new BigDecimal(sign >= 0 ? "+0" : "-0", Ctor);
                else
                    return new BigDecimal((double)sign / 0, Ctor);
            }

            if (@base.IsTrue())
            {
                logBase = 1;
                e = (x.e ?? 0) - (y.e ?? 0);
            }
            else
            {
                @base = BigDecimalHelperFunctions.BASE;
                logBase = BigDecimalHelperFunctions.LOG_BASE;
                e = (long)Math.Floor((double)x.e / logBase) - (long)Math.Floor((double)y.e / logBase);
            }

            yL = yd.LongLength;
            xL = xd.LongLength;
            q = new BigDecimal(sign, Ctor);
            qd = q.d = new int[0];

            // Result exponent may be one less than e.
            // The digit array of a Decimal from toStringBinary may have trailing zeros.
            for (i = 0; i < yd.LongLength && yd[i] == (i < xd.LongLength ? xd[i] : 0); i++) ;

            if (i < yd.LongLength && yd[i] > (i < xd.LongLength ? xd[i] : 0)) e--;

            if (pr == null)
            {
                pr = Ctor.Precision;
                sd = pr.Value;
                rm = Ctor.Rounding;
            }
            else if (dp.IsTrue())
            {
                sd = pr.Value + ((x.e ?? 0) - (y.e ?? 0)) + 1;
            }
            else
            {
                sd = pr.Value;
            }

            if (sd < 0)
            {
                ArrayExtensions.Push(ref qd, 1);
                more = true;
            }
            else
            {

                // Convert precision in number of @base 10 digits to @base 1e7 digits.
                sd = sd / logBase + 2 | 0;
                i = 0;

                // divisor < 1e7
                if (yL == 1)
                {
                    k = 0;
                    yd0 = yd[0];
                    sd++;

                    // k is the carry.
                    for (; (i < xL || k.IsTrue()) && sd--.IsTrue(); i++)
                    {
                        t = (long)k * @base.Value + (xd.LongLength > i ? xd[i] : 0);
                        if (qd.LongLength <= i) ArrayExtensions.Resize(ref qd, i + 1);
                        qd[i] = (int)(t / yd0) | 0;
                        k = (long)(t % yd0) | 0;
                    }

                    more = k.IsTrue() || i < xL;

                    // divisor >= 1e7
                }
                else
                {
                    // Normalise xd and yd so highest order digit of yd is >= @base/2
                    k = @base.Value / (yd[0] + 1) | 0;

                    if (k > 1)
                    {
                        yd = multiplyInteger(yd, k, @base.Value);
                        xd = multiplyInteger(xd, k, @base.Value);
                        yL = yd.LongLength;
                        xL = xd.LongLength;
                    }

                    xi = yL;
                    rem = xd.Slice(0, yL);
                    remL = rem.LongLength;

                    // Add zeros to make remainder as long as divisor.
                    for (; remL < yL;)
                    {
                        if (rem.LongLength <= remL) ArrayExtensions.Resize(ref rem, remL + 1);
                        rem[remL++] = 0;
                    }

                    yz = yd.Slice();
                    ArrayExtensions.Unshift(ref yz, 0);
                    yd0 = yd[0];

                    if (yd[1] >= @base.Value / 2) ++yd0;

                    do
                    {
                        k = 0;

                        // Compare divisor and remainder.
                        cmp = compare(yd, rem, yL, remL);

                        // If divisor < remainder.
                        if (cmp < 0)
                        {

                            // Calculate trial digit, k.
                            rem0 = rem[0];
                            if (yL != remL) rem0 = rem0 * @base.Value + (rem.LongLength > 1 ? rem[1] : 0);

                            // k will be how many times the divisor goes into the current remainder.
                            k = (long)(rem0 / yd0) | 0;

                            //  Algorithm:
                            //  1. product = divisor * trial digit (k)
                            //  2. if product > remainder: product -= divisor, k--
                            //  3. remainder -= product
                            //  4. if product was < remainder at 2:
                            //    5. compare new remainder and divisor
                            //    6. If remainder > divisor: remainder -= divisor, k++

                            if (k > 1)
                            {
                                if (k >= @base.Value) k = @base.Value - 1;

                                // product = divisor * trial digit.
                                prod = multiplyInteger(yd, k, @base.Value);
                                prodL = prod.LongLength;
                                remL = rem.LongLength;

                                // Compare product and remainder.
                                cmp = compare(prod, rem, prodL, remL);

                                // product > remainder.
                                if (cmp == 1)
                                {
                                    k--;

                                    // Subtract divisor from product.
                                    subtract(ref prod, yL < prodL ? yz : yd, prodL, @base.Value);
                                }
                            }
                            else
                            {

                                // cmp is -1.
                                // If k is 0, there is no need to compare yd and rem again below, so change cmp to 1
                                // to avoid it. If k is 1 there is a need to compare yd and rem again below.
                                if (k == 0) cmp = k = 1;
                                prod = yd.Slice();
                            }

                            prodL = prod.LongLength;
                            if (prodL < remL) ArrayExtensions.Unshift(ref prod, 0);

                            // Subtract product from remainder.
                            subtract(ref rem, prod, remL, @base.Value);

                            // If product was < previous remainder.
                            if (cmp == -1)
                            {
                                remL = rem.LongLength;

                                // Compare divisor and new remainder.
                                cmp = compare(yd, rem, yL, remL);

                                // If divisor < new remainder, subtract divisor from remainder.
                                if (cmp < 1)
                                {
                                    k++;

                                    // Subtract divisor from remainder.
                                    subtract(ref rem, yL < remL ? yz : yd, remL, @base.Value);
                                }
                            }

                            remL = rem.LongLength;
                        }
                        else if (cmp == 0)
                        {
                            k++;
                            rem = new[] { 0 };
                        }    // if cmp == 1, k will be 0

                        // Add the next digit, k, to the result array.
                        if (qd.LongLength <= i) ArrayExtensions.Resize(ref qd, i + 1);
                        qd[i++] = (int)k;

                        // Update the remainder.
                        if (cmp.IsTrue() && rem[0].IsTrue())
                        {
                            if (rem.LongLength <= remL) ArrayExtensions.Resize(ref rem, remL + 1);
                            rem[remL++] = xd.LongLength > xi ? xd[xi] : 0;
                        }
                        else
                        {
                            rem = xd.LongLength > xi ? new[] { xd[xi] } : new int[0];
                            remL = 1;
                        }

                    } while ((xi++ < xL || rem.LongLength > 0) && sd--.IsTrue());

                    more = rem.LongLength > 0;
                }

                // Leading zero?
                if (!qd[0].IsTrue()) ArrayExtensions.Shift(ref qd);
            }

            q.d = qd;

            // logBase is 1 when divide is being used for @base conversion.
            if (logBase == 1)
            {
                q.e = e;
                inexact = more;
            }
            else
            {
                // To calculate q.e, first get the number of digits of qd[0].
                for (i = 1, k = qd.LongLength > 0 ? qd[0] : 0; k >= 10; k /= 10) i++;
                q.e = i + e * logBase - 1;

                BigDecimalHelperFunctions.finalise(q, dp.IsTrue() ? pr + q.e + 1 : pr, rm, more);
            }

            return q;
        }
    }
}
