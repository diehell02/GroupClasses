using System;
using System.Collections.Generic;
using GroupClasses.Library.Datas;

namespace GroupClasses.Library.Service
{
    public interface IDataService
    {
        IReadOnlyList<DataValue> Values
        {
            get;
        }

        void AddValue(DataValue dataValue);
    }
}
