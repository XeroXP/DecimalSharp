namespace DecimalSharp.Core
{
    public enum RoundingMode
    {
        /// <summary>
        /// Away from zero.
        /// </summary>
        ROUND_UP,
        /// <summary>
        /// Towards zero.
        /// </summary>
        ROUND_DOWN,
        /// <summary>
        /// Towards +Infinity.
        /// </summary>
        ROUND_CEIL,
        /// <summary>
        /// Towards -Infinity.
        /// </summary>
        ROUND_FLOOR,
        /// <summary>
        /// Towards nearest neighbour. If equidistant, up.
        /// </summary>
        ROUND_HALF_UP,
        /// <summary>
        /// Towards nearest neighbour. If equidistant, down.
        /// </summary>
        ROUND_HALF_DOWN,
        /// <summary>
        /// Towards nearest neighbour. If equidistant, towards even neighbour.
        /// </summary>
        ROUND_HALF_EVEN,
        /// <summary>
        /// Towards nearest neighbour. If equidistant, towards +Infinity.
        /// </summary>
        ROUND_HALF_CEIL,
        /// <summary>
        /// Towards nearest neighbour. If equidistant, towards -Infinity.
        /// </summary>
        ROUND_HALF_FLOOR
    }
}
