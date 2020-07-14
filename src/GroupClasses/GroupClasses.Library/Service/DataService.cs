using GroupClasses.Library.Datas;
using System;
using System.Collections.Generic;
using System.Text;

namespace GroupClasses.Library.Service
{
    public class DataService : IDataService
    {
        private List<DataValue> values = new List<DataValue>();

        public IReadOnlyList<DataValue> Values
        {
            get => values;
        }

        public void AddValue(DataValue dataValue)
        {
            values.Add(dataValue);
        }
    }
}
