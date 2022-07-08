namespace DecimalSharp.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Slice(this string str, int start = 0, int end = 0)
        {
            if (start >= str.Length) return "";

            if (end == 0) end = str.Length;
            if (end > str.Length) end = str.Length;

            return str.Substring(start, end - start);
        }
    }
}
