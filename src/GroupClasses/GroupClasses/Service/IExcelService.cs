using System;
using GroupClasses.Library.Datas;
using GroupClasses.Library.Service;

namespace GroupClasses.Service
{
    public interface IExcelService
    {
        Data[] Load(string path, IDataService dataService, IFilterService filterService);

        void Save(string path, Data[][] datas, IDataService dataService);
    }
}
