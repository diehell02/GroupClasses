using GroupClasses.Library.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace GroupClasses.Library.Service
{
    public class FilterService: IFilterService
    {
        private List<Filter> filters = new List<Filter>();

        public IReadOnlyList<Filter> Filters
        {
            get => filters;
        }

        public void AddFilter(Filter filter)
        {
            filters.Add(filter);
        }
    }
}
