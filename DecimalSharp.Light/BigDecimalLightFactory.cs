using DecimalSharp.Core;

namespace DecimalSharp
{
    public class BigDecimalLightFactory
    {
        public BigDecimalLightConfig Config { get; set; }

        public BigDecimalLightFactory(BigDecimalLightConfig? config = null)
        {
            this.Config = config ?? new BigDecimalLightConfig();
        }

        public BigDecimalLight BigDecimal(BigDecimalArgument<BigDecimalLight> v)
        {
            return new BigDecimalLight(v, this.Config);
        }

        // BigDecimalLightFactory methods


        /*
         *  clone
         */


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new BigDecimalLightFactory with the same configuration properties as this BigDecimalLightFactory.</returns>
        public BigDecimalLightFactory Clone()
        {
            return new BigDecimalLightFactory(this.Config.Clone());
        }
    }
}
