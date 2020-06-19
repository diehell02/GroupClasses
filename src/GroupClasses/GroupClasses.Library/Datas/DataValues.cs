using System;
using System.Collections.Generic;

namespace GroupClasses.Library.Datas
{
    public static class DataValues
    {
        private static List<DataValue> values = new List<DataValue>();

        public static IReadOnlyList<DataValue> Values
        {
            get => values;
        }

        public static void AddValue(DataValue dataValue)
        {
            values.Add(dataValue);
        }
    }
}
