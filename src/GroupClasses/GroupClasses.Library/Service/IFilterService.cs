using System;
using System.Collections.Generic;
using GroupClasses.Library.Filters;

namespace GroupClasses.Library.Service
{
    public interface IFilterService
    {
        IReadOnlyList<Filter> Filters
        {
            get;
        }

        void AddFilter(Filter filter);
    }
}
