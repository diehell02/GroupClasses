using System;
using System.Collections.Generic;
using System.Text;

namespace GroupClasses.Library.Utils
{
    class MathUtil
    {
        public static decimal Variance<T>(List<T> list, Func<T, decimal> func)
        {
            decimal sum = 0;
            decimal avg = 0;
            decimal[] values = new decimal[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                values[i] = func.Invoke(item);
            }

            avg = Average(values);

            foreach (var value in values)
            {
                sum += Convert.ToDecimal(Math.Pow(Convert.ToDouble(value - avg), 2.0d));
            }

            return sum / values.Length;
        }

        public static decimal Variance(decimal[] values)
        {
            decimal sum = 0;
            decimal avg = 0;

            avg = Average(values);

            foreach (var value in values)
            {
                sum += Convert.ToDecimal(Math.Pow(Convert.ToDouble(value - avg), 2.0d));
            }

            return sum / values.Length;
        }

        public static decimal Average(decimal[] array)
        {
            decimal sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum / array.Length;
        }
    }
}
