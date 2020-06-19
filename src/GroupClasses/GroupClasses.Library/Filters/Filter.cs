using System;
using GroupClasses.Library.Datas;

namespace GroupClasses.Library.Filters
{
    public class Filter
    {
        public DataValue DataValue
        {
            get;
            set;
        }

        public FilterType Type
        {
            get;
            set;
        }

        public int Weight
        {
            get;
            set;
        }
    }
}
