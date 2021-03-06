using DecimalSharp.Core;
using NUnit.Framework;

namespace DecimalSharp.Tests.Light
{
    [TestFixture, Category("BigDecimalLightLog")]
    public class BigDecimalLightLogTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Log()
        {
            var bigDecimalFactory = new BigDecimalLightFactory(new BigDecimalLightConfig()
            {
                Precision = 40,
                Rounding = RoundingMode.ROUND_HALF_UP,
                ToExpNeg = (long)-9e15,
                ToExpPos = (long)9e15
            });

            var t = (BigDecimalArgument<BigDecimalLight> n, BigDecimalArgument<BigDecimalLight> @base, string expected, long sd) =>
            {
                var config = bigDecimalFactory.Config.Clone();
                config.Precision = sd;

                BigDecimalTests.assertEqual(expected, new BigDecimalLight(n, config).Log(@base).ValueOf());
            };

            t("243", "3", "5", 7);
            t("243", "9", "2.5", 7);

            /*
             6.166675020000903537297764507632802193308677149
             28^6.16667502 = 839756321.6383567959704282429971526703012698
             28^6.16667503 = 839756349.6207552863005150010387178739013142
             */
            t("32", "4", "2.5", 10);

            //t("316.2277660168379331998893544432645719585553021316022642247511089459022980600999502961482777894980004", "10", "2.49999999999999999999", 21);

            // Base 10 therefore the following tests pass despite 15 or more zeros or nines as the rounding digits.

            // 4.9999999999999999999960913...
            //t("99999.9999999999999991", 10, "4.999", 4);

            // 10^0.1 = 1.258925411794167210423954106395800606093617409466...
            //t("1.258925411794167210423954106395", 10, "0.09999", 4);

            // 3.391702100000000000000000000000000025534271040...
            t("2464.348361986885121671329250344224", 10, "3.3917021", 18);

            // 4.99999999999999999960913...
            //t("99999.99999999999991", 10, "4.999", 4);

            // 4.9999999999999999960913...
            //t("99999.9999999999991", 10, "4.999", 4);

            // 4.99999999999960913...
            //t("99999.99999991", 10, "4.999", 4);

            // 8.959609629999999999999999999999431251938064
            //t("911191437.48166728043529900000", 10, "8.959609629999999999999999", 25);

            //t("6.626757835589191227753975149737456562020794782", 10, "0.8213011002743699999999999999999999999999999", 43);
            //t("64513410281785809574142282220919135969.8537876292904158501590880", 10, "37.80964999999999999999", 22);
            //t("10232.9299228075413096627", 10, "4.009999999999999", 16);
            //t("1.29891281037500", 10, "0.11357", 5);
            //t("0.0000100000000000010000005060000000000800030000000400908", 10, "-4.99", 3);
            //t("8", "4294967296", "0.09375", 4);

            //t("4.20732041199815040736678139715312481859825562145776045079", 10, "0.6240055873352599999999999999999999999999999999999999", 52);
            //t("33.51145738694771448172942314968136067036971739113975569076629", 10, "1.5251933153717162999999999999999999999999999999999999999", 56);
            //t("16.399137225681149762104868844", 10, "1.21482099999999999999999", 24);
            t("0.01", 10, "-2", 17);
            t("0.00001", 10, "-5", 35);
            t("15625", "3125", "1.2", 2);
            //t("268435456", "1048576", "1.4", 2);
            t("8589934592", "65536", "2.0625", 5);
            //t("33554432", "256", "3.125", 4);
            t("4503599627370496", "65536", "3.25", 3);
            t("68630377364883", "59049", "2.9", 2);

            // base 2
            t("18216.8140929585641372", 2, "14.152983050314836771855701", 26);
            t("7408.82842447993", 2, "12.8550297084583087071", 21);
            t("29259.23925137426", 2, "14.83660463902", 13);
            t("31553.4", 2, "14.945507849063278420302384", 26);
            t("6080.97", 2, "12.57008575", 10);
            t("575.881932366571406", 2, "9.16962924962079798", 18);
            t("4573.55560689675", 2, "12.1591004766309775332", 21);
            t("8607.97004888426002071", 2, "13.07145734276093159769689946319", 31);
            t("559.1647711259", 2, "9.12712965971023632", 18);
            t("31897.9889", 2, "14.9611778475691091525075", 24);
            t("9091.859714971663525", 2, "13.1503597085807068872335", 24);
            t("5661.58", 2, "12.466989012642603919950322048", 29);

            Assert.Pass();
        }
    }
}