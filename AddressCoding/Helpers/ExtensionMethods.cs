using System.Collections.Generic;
using System.Linq;

namespace AddressCoding
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Первый символ строки в верхнем регистре
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpperFistChar(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 0)
            {
                str = str.ToLower();
                var chars = str.ToCharArray();
                chars[0] = char.ToUpper(chars[0]);
                return new string(chars);
            }
            return null;
        }

        /// <summary>
        /// Метод расширения для IEnumerable, для разбиения на части с заданым количеством элементов внутри одной части
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">Перечислитель</param>
        /// <param name="partitionSize">Количество элементов в одной части</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> instance, int partitionSize)
        {
            return instance
                .Select((value, index) => new { Index = index, Value = value })
                .GroupBy(i => i.Index / partitionSize)
                .Select(i => i.Select(i2 => i2.Value));
        }
    }
}