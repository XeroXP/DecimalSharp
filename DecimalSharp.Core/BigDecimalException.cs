namespace DecimalSharp.Core;

public class BigDecimalException : Exception
{
    public static string DecimalError = "[DecimalError] ";
    public static string InvalidArgument = DecimalError + "Invalid argument: ";
    public static string PrecisionLimitExceeded = DecimalError + "Precision limit exceeded";
    public static string ExponentOutOfRange = DecimalError + "Exponent out of range: ";

    public BigDecimalException() : base() { }
    public BigDecimalException(string message) : base(message) { }
}
