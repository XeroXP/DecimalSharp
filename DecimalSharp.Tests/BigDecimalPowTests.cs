using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests
{
    [TestFixture, Category("BigDecimalPow")]
    public class BigDecimalPowTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Pow()
        {
            var bigDecimalFactory = new BigDecimalFactory(new BigDecimalConfig()
            {
                Precision = 40,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15,
                MaxE = (long)9e15,
                MinE = (long)-9e15
            });

            var t = (BigDecimalArgument<BigDecimal> @base, BigDecimalArgument<BigDecimal> exp, string expected, long sd, RoundingMode rm) =>
            {
                var config = bigDecimalFactory.Config.Clone();
                config.Precision = sd;
                config.Rounding = rm;

                BigDecimalTests.assertEqual(expected, new BigDecimal(@base, config).Pow(exp).ValueOf());
            };

            t("9", "0.5", "3", 7, RoundingMode.ROUND_HALF_UP);
            t("9", "0.5", "3", 26, RoundingMode.ROUND_HALF_UP);
            t("0.9999999999", "6", "0.999999999400000000149999999980000000001", 39, RoundingMode.ROUND_HALF_UP);
            t("2.56", "6.5", "450.3599627370496", 16, RoundingMode.ROUND_DOWN);
            t("1.96", "1.5", "2.744", 15, RoundingMode.ROUND_DOWN);
            t("2.25", "9.5", "2216.8378200531005859375", 23, RoundingMode.ROUND_DOWN);
            t("11.05", "2.00000000000000007", "122.103", 6, RoundingMode.ROUND_HALF_UP);
            t("10.5", "3.000000000000000002", "1157.63", 6, RoundingMode.ROUND_HALF_UP);
            t("1.00000000000000000003", "4.00000005", "1.000000000000000000120000002", 28, RoundingMode.ROUND_HALF_UP);
            t("6.0000005", "1.00000000000000006", "6.000001", 7, RoundingMode.ROUND_HALF_UP);
            t("1.0000000000000000000005", "49.0000000000000000000002", "1.000000000000000000025", 22, RoundingMode.ROUND_HALF_UP);
            t("15.333333333333333333", "28.33333333333333", "3917746643938779840069598486694964.98308568625045", 49, RoundingMode.ROUND_HALF_UP);
            t("7.537714", "7.9", "8515169.08260507715975", 21, RoundingMode.ROUND_HALF_UP);
            t("6.951", "9.225", "58598464.57", 10, RoundingMode.ROUND_HALF_UP);
            t("6.01093", "9.8911", "50651225.3819968681522216250662534915", 36, RoundingMode.ROUND_HALF_UP);
            t("8.7587", "4.23", "9694.37298592397372", 18, RoundingMode.ROUND_HALF_UP);
            t("5.1749", "7.7267995", "328229.2815443039852", 19, RoundingMode.ROUND_HALF_UP);
            t("0.16", "-0.9999999999999", "6.2", 2, RoundingMode.ROUND_HALF_UP);
            t("0.4", "-20", "90949470.1772928237915039063", 27, RoundingMode.ROUND_HALF_UP);
            t("0.5", "22", "0.000000238418579101563", 15, RoundingMode.ROUND_HALF_UP);
            t("32", "0.4", "4", 1, RoundingMode.ROUND_HALF_UP);
            t("4", "2.5", "32", 11, RoundingMode.ROUND_HALF_UP);
            t("4", "5.5", "2048", 27, RoundingMode.ROUND_HALF_UP);
            t("16", "23.5", "19807040628566084398385987584", 29, RoundingMode.ROUND_HALF_UP);
            t("16", "26.5", "81129638414606681695789005144064", 35, RoundingMode.ROUND_HALF_UP);
            t("25", "13.5", "7450580596923828125", 39, RoundingMode.ROUND_HALF_UP);
            t("32", "28.2", "2787593149816327892691964784081045188247552", 43, RoundingMode.ROUND_HALF_UP);
            t("32", "3.6", "262144", 35, RoundingMode.ROUND_HALF_UP);
            t("25", "21.5", "1136868377216160297393798828125", 31, RoundingMode.ROUND_HALF_UP);
            t("9", "8.5", "129140163", 19, RoundingMode.ROUND_HALF_UP);
            t("4", "7.5", "32768", 13, RoundingMode.ROUND_HALF_UP);
            t("4", "6.5", "8192", 10, RoundingMode.ROUND_HALF_UP);
            t("6.034", "0.25964", "2", 1, RoundingMode.ROUND_HALF_UP);
            t("9", "4.5", "19683", 16, RoundingMode.ROUND_HALF_UP);
            t("9", "1.5", "27", 5, RoundingMode.ROUND_HALF_UP);
            t("9.61", "3.5", "2751.2614111", 12, RoundingMode.ROUND_HALF_UP);
            t("4", "6.5", "8192", 8, RoundingMode.ROUND_HALF_UP);
            t("4", "7.5", "32768", 11, RoundingMode.ROUND_HALF_UP);
            t("9", "4.5", "19683", 5, RoundingMode.ROUND_HALF_UP);

            t("48.9262695992662373981", "1.0", "48.926269599266237", 17, RoundingMode.ROUND_DOWN);
            t("1.21", "0.5", "1.1", 2, RoundingMode.ROUND_DOWN);
            t("12.96", "0.5", "3.6", 2, RoundingMode.ROUND_FLOOR);
            t("3.24", "0.5", "1.8", 2, RoundingMode.ROUND_DOWN);
            t("70.56", "0.5", "8.4", 2, RoundingMode.ROUND_FLOOR);
            t("4.41", "6.5", "15447.2377739119461", 32, RoundingMode.ROUND_FLOOR);
            t("11.05", "2.00000000000000007", "122.103", 6, RoundingMode.ROUND_HALF_UP);
            t("10.5", "3.000000000000000002", "1157.63", 6, RoundingMode.ROUND_HALF_UP);
            t("1.00000000000000000003", "4.00000005", "1.000000000000000000120000002", 28, RoundingMode.ROUND_HALF_UP);
            t("6.0000005", "1.00000000000000006", "6.000001", 7, RoundingMode.ROUND_HALF_UP);
            t("1.0000000000000000000005", "49.0000000000000000000002", "1.000000000000000000025", 22, RoundingMode.ROUND_HALF_UP);
            t("5344.87762641765349023882127126550721", "1.0625", "9139.7407411741874683083843738640173291", 38, RoundingMode.ROUND_DOWN);
            t("28", "6.166675020000903537297764507632802193308677149", "839756321.64088511", 17, RoundingMode.ROUND_UP);
            t("91.180153837", "0.5", "9.54882997214842023704943457512609", 33, RoundingMode.ROUND_DOWN);
            t("16", "26.5", "81129638414606681695789005144064", 35, RoundingMode.ROUND_HALF_UP);
            t("25", "13.5", "7450580596923828125", 39, RoundingMode.ROUND_HALF_UP);
            t("4.3985903", "20.9956530307", "32120869378609.033520730996715368034448124619", 44, RoundingMode.ROUND_CEIL);
            t("2.858368", "48.97", "21682301291468972839895.193017121528607658932", 44, RoundingMode.ROUND_HALF_DOWN);
            t("91.145", "23.8479557348417627", "54402923894673605836306983589686900000000000000", 33, RoundingMode.ROUND_DOWN);

            t("5.379973182", "2.65", "86.4", 3, RoundingMode.ROUND_HALF_EVEN);
            t("625", "4.5", "3814697265625", 13, RoundingMode.ROUND_UP);
            t("65536", "1.25", "1048576", 7, RoundingMode.ROUND_HALF_DOWN);
            t("9", "1.5", "27", 2, RoundingMode.ROUND_HALF_FLOOR);
            t("256", "1.625", "8192", 4, RoundingMode.ROUND_HALF_FLOOR);
            t("65536", "1.875", "1073741824", 10, RoundingMode.ROUND_HALF_EVEN);
            t("65536", "2.5", "1099511627776", 13, RoundingMode.ROUND_DOWN);
            t("625", "5.25", "476837158203125", 15, RoundingMode.ROUND_HALF_UP);

            t("0.16", "-0.9999999999999", "6.2", 2, RoundingMode.ROUND_HALF_UP);
            t("3.6361", "-0.06", "0.92547", 5, RoundingMode.ROUND_HALF_UP);
            t("8.7881541", "-0.00000006", "0.999999869595727123998", 22, RoundingMode.ROUND_HALF_UP);
            t("5.812", "-0.99999", "0.17206083953928505581714758136682954", 35, RoundingMode.ROUND_HALF_UP);
            t("6.06737421654397", "-0.000000000001", "0.99999999999819707407228698", 26, RoundingMode.ROUND_HALF_UP);
            t("5.57197470953405387", "-0.9", "0.213", 3, RoundingMode.ROUND_HALF_UP);
            t("8.4297580531324", "-0.000000000000002", "0.99999999999999573646385819", 26, RoundingMode.ROUND_HALF_UP);
            t("1.746122696164", "-0.9", "0.605526", 6, RoundingMode.ROUND_HALF_UP);
            t("5.74274073282643192871", "-0.000000000000004", "0.999999999999993008253696156596264156", 36, RoundingMode.ROUND_HALF_UP);
            t("9.66306878602393217324", "-0.999", "0.1037217997755957147", 19, RoundingMode.ROUND_HALF_UP);
            t("5", "-0.9999999", "0.200000032188760838972540436", 28, RoundingMode.ROUND_HALF_UP);

            t("21.8005326254960840089", "14.99999999999999999999", "119400615273418803650.1362563340821916898208389", 46, RoundingMode.ROUND_HALF_UP);
            t("46.80102307015", "4.000001", "4797589.19437982876031", 21, RoundingMode.ROUND_HALF_UP);
            t("29.255206217375", "9.9999999999999999999999999", "459231465846284.22207", 20, RoundingMode.ROUND_HALF_UP);
            t("0.72591761772", "6.999999999999999999999", "0.106221237503302998", 18, RoundingMode.ROUND_HALF_UP);
            t("0.3928066161887", "32.0002", "0.00000000000010319062643292561810866879769121849802", 38, RoundingMode.ROUND_HALF_UP);
            t("24.798046085018648753453", "5.9999999999999999", "232543806.207", 12, RoundingMode.ROUND_HALF_UP);
            t("20.485568584242", "18.99999999999999999999999", "8270131718672851271097903.87621818958353436851582", 48, RoundingMode.ROUND_HALF_UP);
            t("969.0", "-1", "0.0010319917440660474716202", 23, RoundingMode.ROUND_HALF_UP);
            t("8.97", "-1", "0.111482720178", 12, RoundingMode.ROUND_FLOOR);
            t("61766796871807246.3278075", "-1", "0.00000000000000001618993", 7, RoundingMode.ROUND_UP);

            t("-1", "101", "-1", 100, RoundingMode.ROUND_DOWN);
            t("-1", "9999999999999999999999999999999999999999999999999999999999999999999999999", "-1", 100, RoundingMode.ROUND_DOWN);
            t("-1", "1e307", "1", 100, RoundingMode.ROUND_DOWN);
            t("-1", "1e309", "1", 100, RoundingMode.ROUND_DOWN);

            bigDecimalFactory.Config.ToExpNeg = 0;
            bigDecimalFactory.Config.ToExpPos = 0;

            t("9.9999999999999", "2220.75", "5.623413251778e+2220", 13, RoundingMode.ROUND_DOWN);
            t("0.9999999999999999991999999999019999949909999999", "2220.75", "9.999999999999982233999997e-1", 25, RoundingMode.ROUND_DOWN);
            t("987504387560932846509387650789.49807365", "981459.4903857", "9.876e+29438424", 4, RoundingMode.ROUND_HALF_UP);

            t("-2", "1001", "-2.1430172143725346418e+301", 20, RoundingMode.ROUND_DOWN);
            t("-2", 1e6, "9.9006562292958982506979236164e+301029", 29, RoundingMode.ROUND_UP);

            t("5.0771598579583468811E-101844", "7064449.87442997380369702938801116641723585825702571602", "3.907934864857193219594361275098983e-719466848189", 34, RoundingMode.ROUND_DOWN);
            t("5.80246472674775E+21125581", "0.00077726506294426495082193497633668602085", "1.5018938138904125617523547e+16420", 26, RoundingMode.ROUND_FLOOR);
            t("1.66630944E+74", "6980757669.9081156729942256", "3.74152e+518124090060", 6, RoundingMode.ROUND_HALF_DOWN);
            t("5.9E+6", "3456.7700", "1.8971788927235700943477592799711063194e+23405", 38, RoundingMode.ROUND_UP);
            t("93720986.7819907489497420190553708041564963922285117", "39.580", "3.3e+315", 2, RoundingMode.ROUND_DOWN);
            t("908948247.896330216349750387912923575076135766138", "11.38907521122213262858256836", "1.0702278292293091784680297675223031e+102", 35, RoundingMode.ROUND_FLOOR);
            t("4.485925762349120387154391E+47", "1677945.16766265206929939", "8.53959030215133943e+79957194", 18, RoundingMode.ROUND_HALF_DOWN);
            t("2.8448989811706207675566E+89", "2.368592228588521845032068137267440272102614", "7.58940197453762187722508511706932e+211", 33, RoundingMode.ROUND_HALF_DOWN);

            /*t("0.9999999999999999", "-1e+30", "1.530863912e+43429448190325", 10, RoundingMode.ROUND_DOWN);
            t("0.9999999999999999999999999999999999999999999999999", "-1e+32", "1.00000000000000001000000000000000005e+0", 36, RoundingMode.ROUND_DOWN);
            t("0.9999999999999999", "-1e+50", "Infinity", 40, RoundingMode.ROUND_DOWN);
            t("0.9999999999999999999999999999999899999999999999994403269002375809806554775739676251993670310626872684", "-1.49181945463118148622657269735650603014891811120124843379694396257337810020127409048127397077199569e+271", "Infinity", 100, RoundingMode.ROUND_DOWN);
            */
            Assert.Pass();
        }
    }
}