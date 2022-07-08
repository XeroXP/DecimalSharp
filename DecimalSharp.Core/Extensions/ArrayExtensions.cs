namespace DecimalSharp.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static void Resize<T>(ref T[] numbers, long newSize)
        {
            var newNumbers = new T[newSize];
            Array.Copy(numbers, 0, newNumbers, 0, Math.Min(numbers.LongLength, newSize));
            numbers = newNumbers;
        }

        public static void Reverse<T>(ref T[] numbers)
        {
            Array.Reverse(numbers);
            /*for (long i = 0; i < numbers.LongLength / 2; i++)
            {
                var tmp = numbers[i];
                numbers[i] = numbers[numbers.LongLength - i - 1];
                numbers[numbers.LongLength - i - 1] = tmp;
            }*/
        }

        public static void Pop<T>(ref T[] numbers)
        {
            Resize(ref numbers, numbers.LongLength - 1);
        }

        public static void Push<T>(ref T[] numbers, T value)
        {
            AddElementAt(ref numbers, numbers.LongLength, value);
        }

        public static T[] Slice<T>(this T[] numbers, long start = 0, long length = 0)
        {
            if (length == 0) length = numbers.LongLength - start;

            if (length > numbers.LongLength - start) length = numbers.LongLength - start;
            T[] destfoo = new T[length];
            Array.Copy(numbers, start, destfoo, 0, length);
            return destfoo;
        }

        public static T[] StringSlice<T>(this T[] numbers, long start = 0, long end = 0)
        {
            if (start >= numbers.LongLength) return new T[0];

            if (end == 0) end = numbers.LongLength;
            if (end > numbers.LongLength) end = numbers.LongLength;

            var length = end - start;

            T[] destfoo = new T[length];
            Array.Copy(numbers, start, destfoo, 0, length);
            return destfoo;
        }

        public static T Shift<T>(ref T[] numbers)
        {
            if (numbers.LongLength == 0)
                return default(T);

            T[] cloneArray = (T[])numbers.Clone();
            Resize(ref numbers, numbers.LongLength - 1);
            for(long i = 1; i < cloneArray.LongLength; i++)
            {
                numbers[i-1] = cloneArray[i];
            }
            return cloneArray[0];
        }

        public static void Unshift<T>(ref T[] numbers, params T[] values)
        {
            foreach (var value in values.Reverse())
            {
                AddElementAt(ref numbers, 0, value);
            }
        }

        /// <summary>    
        /// Add element at nth position in array    
        /// </summary>    
        /// <param name="numbers">Source Array</param>    
        /// <param name="index">Position Number</param>    
        /// <param name="value">the value to be entered</param>    
        public static void AddElementAt<T>(ref T[] numbers, long index, T value)
        {
            //first resize it    
            Resize(ref numbers, numbers.LongLength + 1);

            long orginalLength = numbers.LongLength;
            //clone the array    
            T[] cloneArray = (T[])numbers.Clone();
            for (long i = 0; i < orginalLength; i++)
            {
                if (i == index)
                {
                    //copy element from the position    
                    var element = cloneArray[index];
                    numbers[index] = value;
                    if (i + 1 < orginalLength)
                        numbers[index + 1] = element;
                }
                else if (i > index)
                {
                    if (i + 1 < orginalLength)
                        numbers[i + 1] = cloneArray[i];
                }
                else
                {
                    numbers[i] = cloneArray[i];
                }
            }
        }
    }
}
