using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupClasses.Library.Datas
{
    public class Data
    {
        private Dictionary<DataValue, object> values =
            new Dictionary<DataValue, object>();

        private bool Verify(DataValue dataValue)
        {
            var result = values.Where(value => value.Key.Id == dataValue.Id);

            if (result?.Count() > 0)
            {
                return false;
            }

            return true;
        }

        public void AddValue(DataValue dataValue, object value)
        {
            if(!Verify(dataValue))
            {
                return;
            }

            values.Add(dataValue, value);
        }

        public void Flush()
        {
            var tempValues = new KeyValuePair<DataValue, object>[values.Select(n => n.Key.Id).Max() + 1];

            foreach(var value in values)
            {
                tempValues[value.Key.Id] = value;
            }

            Values = tempValues;
        }

        public KeyValuePair<DataValue, object>[] Values
        {
            get;
            private set;
        }
    }
}
