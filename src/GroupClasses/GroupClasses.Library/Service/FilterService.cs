using GroupClasses.Library.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace GroupClasses.Library.Service
{
    class FilterService
    {
        private static List<Filter> filters = new List<Filter>();

        public static IReadOnlyList<Filter> Filters
        {
            get => filters;
        }

        public static void AddFilter(Filter filter)
        {
            filters.Add(filter);
        }
    }
}
