using System.Globalization;

namespace DecimalSharp.Core
{
    public class LongString
    {
        internal char[] chars;

        public int Length
        {
            get { return this.chars.Length; }
        }

        public long LongLength
        {
            get { return this.chars.LongLength; }
        }

        public LongString(char[]? chars = null)
        {
            if (chars != null) this.chars = chars;
            else this.chars = new char[0];
        }

        public LongString Slice(long start = 0, long end = 0)
        {
            if (start >= chars.LongLength) return "";

            if (end == 0) end = chars.LongLength;
            if (end > chars.Length) end = chars.LongLength;

            var length = end - start;

            char[] destfoo = new char[length];
            Array.Copy(chars, start, destfoo, 0, length);
            return new LongString(destfoo);
        }

        public string Replace(string oldValue, string? newValue)
        {
            return ToString().Replace(oldValue, newValue);
        }

        public string ToLowerInvariant()
        {
            return ToString().ToLowerInvariant();
        }

        public long IndexOf(char ch, StringComparison? stringComparison = null)
        {
            return Array.FindIndex(chars, c =>
            {
                if (stringComparison == StringComparison.InvariantCultureIgnoreCase
                    || stringComparison == StringComparison.OrdinalIgnoreCase
                    || stringComparison == StringComparison.CurrentCultureIgnoreCase)
                    return c.ToString().ToLowerInvariant() == ch.ToString().ToLowerInvariant();
                return c == ch;
            });
        }

        public long IndexOf(string ch, StringComparison? stringComparison = null)
        {
            var x = chars;
            var y = ch.ToCharArray();
            return (from i in Enumerable.Range(0, 1 + x.Length - y.Length)
                    where x.Skip(i).Take(y.Length).SequenceEqual(y, new CharComparer(stringComparison))
                    select (int?)i).FirstOrDefault().GetValueOrDefault(-1);
        }

        public char ElementAt(long index)
        {
            return chars[index];
        }

        public override string ToString()
        {
            return new string(chars);
        }

        public override bool Equals(object? obj)
        {
            if (obj != null)
            {
                if (obj is LongString objLongString)
                {
                    return Enumerable.SequenceEqual(chars, objLongString.chars);
                }
                else if (obj is string objString)
                {
                    return Enumerable.SequenceEqual(chars, objString.ToCharArray());
                }
                else if (obj is char[] objCharArr)
                {
                    return Enumerable.SequenceEqual(chars, objCharArr);
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static LongString operator +(LongString a, LongString b)
        {
            LongString newLS = new();
            newLS.chars = new char[a.chars.LongLength + b.chars.LongLength];
            Array.Copy(a.chars, 0, newLS.chars, 0, a.chars.LongLength);
            Array.Copy(b.chars, 0, newLS.chars, a.chars.LongLength, b.chars.LongLength);
            return newLS;
        }

        public static LongString operator +(LongString a, string b0)
        {
            var b = b0.ToCharArray();
            LongString newLS = new();
            newLS.chars = new char[a.chars.LongLength + b.LongLength];
            Array.Copy(a.chars, 0, newLS.chars, 0, a.chars.LongLength);
            Array.Copy(b, 0, newLS.chars, a.chars.LongLength, b.LongLength);
            return newLS;
        }

        public static LongString operator +(LongString a, long? b0) => a + (b0?.ToString(CultureInfo.InvariantCulture) ?? "");
        public static LongString operator +(LongString a, int? b0) => a + (b0?.ToString(CultureInfo.InvariantCulture) ?? "");
        public static LongString operator +(LongString a, char? b0) => a + (b0?.ToString(CultureInfo.InvariantCulture) ?? "");

        public static implicit operator LongString(string? value)
        {
            return new LongString(value?.ToCharArray());
        }
        public static implicit operator string(LongString v) { return v.ToString(); }
        public static bool operator ==(LongString a, LongString b)
        {
            if (a is null)
            {
                return b is null;
            }

            return a.Equals(b);
        }
        public static bool operator !=(LongString a, LongString b)
        {
            return !(a == b);
        }

        internal class CharComparer : IEqualityComparer<char>
        {
            StringComparison? stringComparison;

            public CharComparer(StringComparison? stringComparison = null) : base()
            {
                this.stringComparison = stringComparison;
            }

            public bool Equals(char x, char y)
            {
                if (stringComparison == StringComparison.InvariantCultureIgnoreCase
                    || stringComparison == StringComparison.OrdinalIgnoreCase
                    || stringComparison == StringComparison.CurrentCultureIgnoreCase)
                    return x.ToString().ToLowerInvariant() == y.ToString().ToLowerInvariant();
                return x == y;
            }

            public int GetHashCode(char obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
