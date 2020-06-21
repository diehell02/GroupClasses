using GroupClasses.Library.Datas;
using System;
using System.Collections.Generic;
using System.Text;

namespace GroupClasses.Library.Service
{
    class DataService
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
