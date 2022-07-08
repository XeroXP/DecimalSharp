using DecimalSharp.Core;
using DecimalSharp.Core.Extensions;

namespace DecimalSharp
{
    internal class BigDecimalLightDivision : BigDecimalDivisionFunctions
    {
        public static BigDecimalLight Divide(BigDecimalLight x, BigDecimalLight y, long? pr, long? dp)
        {
            long cmp, e, i, k, prodL, remL, rem0, sd, xi, xL, yd0, yL;
            double t;
            BigDecimalLight q;
            int[] qd, prod, rem, yz;
            var Ctor = x.Config;
            var sign = x.s == y.s ? 1 : -1;
            var xd = x.d;
            var yd = y.d;

            // Either 0?
            if (!x.s.IsTrue()) return new BigDecimalLight(x, Ctor);
            if (!y.s.IsTrue()) throw new BigDecimalException(BigDecimalException.DecimalError + "Division by zero");

            e = x.e - y.e;
            yL = yd.LongLength;
            xL = xd.LongLength;
            q = new BigDecimalLight(sign, Ctor);
            qd = q.d = new int[0];

            // Result exponent may be one less than e.
            for (i = 0; i < yd.LongLength && yd[i] == (i < xd.LongLength ? xd[i] : 0);) ++i;
            if (i < yd.LongLength && yd[i] > (i < xd.LongLength ? xd[i] : 0)) --e;

            if (pr == null)
            {
                pr = Ctor.Precision;
                sd = pr.Value;
            }
            else if (dp.IsTrue())
            {
                sd = pr.Value + (BigDecimalLightHelperFunctions.getBase10Exponent(x) - BigDecimalLightHelperFunctions.getBase10Exponent(y)) + 1;
            }
            else
            {
                sd = pr.Value;
            }

            if (sd < 0) return new BigDecimalLight(0, Ctor);


            // Convert precision in number of @base 10 digits to @base 1e7 digits.
            sd = sd / BigDecimalLightHelperFunctions.LOG_BASE + 2 | 0;
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
                    t = (long)k * BigDecimalLightHelperFunctions.BASE + (xd.LongLength > i ? xd[i] : 0);
                    if (qd.LongLength <= i) ArrayExtensions.Resize(ref qd, i + 1);
                    qd[i] = (int)(t / yd0) | 0;
                    k = (long)(t % yd0) | 0;
                }

                // divisor >= 1e7
            }
            else
            {
                // Normalise xd and yd so highest order digit of yd is >= @base/2
                k = BigDecimalLightHelperFunctions.BASE / (yd[0] + 1) | 0;

                if (k > 1)
                {
                    yd = multiplyInteger(yd, k, BigDecimalLightHelperFunctions.BASE);
                    xd = multiplyInteger(xd, k, BigDecimalLightHelperFunctions.BASE);
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

                if (yd[1] >= BigDecimalLightHelperFunctions.BASE / 2) ++yd0;

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
                        if (yL != remL) rem0 = rem0 * BigDecimalLightHelperFunctions.BASE + (rem.LongLength > 1 ? rem[1] : 0);

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
                            if (k >= BigDecimalLightHelperFunctions.BASE) k = BigDecimalLightHelperFunctions.BASE - 1;

                            // product = divisor * trial digit.
                            prod = multiplyInteger(yd, k, BigDecimalLightHelperFunctions.BASE);
                            prodL = prod.LongLength;
                            remL = rem.LongLength;

                            // Compare product and remainder.
                            cmp = compare(prod, rem, prodL, remL);

                            // product > remainder.
                            if (cmp == 1)
                            {
                                k--;

                                // Subtract divisor from product.
                                subtract(ref prod, yL < prodL ? yz : yd, prodL, BigDecimalLightHelperFunctions.BASE);
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
                        subtract(ref rem, prod, remL, BigDecimalLightHelperFunctions.BASE);

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
                                subtract(ref rem, yL < remL ? yz : yd, remL, BigDecimalLightHelperFunctions.BASE);
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
            }

            // Leading zero?
            if (!qd[0].IsTrue()) ArrayExtensions.Shift(ref qd);


            q.d = qd;
            q.e = e;

            return BigDecimalLightHelperFunctions.round(q, dp.IsTrue() ? pr.Value + BigDecimalLightHelperFunctions.getBase10Exponent(q) + 1 : pr.Value);
        }
    }
}
