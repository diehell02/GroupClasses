using System;
using System.Collections.Generic;
using System.IO;
using GroupClasses.Library.Datas;
using GroupClasses.Library.Service;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Linq;

namespace GroupClasses.NUnitTests
{
    public class FileUtil
    {
        public static Data[] Load(string path, IDataService dataService, IFilterService filterService)
        {
            IWorkbook workbook = null;

            var fs = File.OpenRead(path);

            if (path.IndexOf(".xlsx") > 0) // 2007版本 
            {
                workbook = new XSSFWorkbook(fs);
            }
            else if (path.IndexOf(".xls") > 0) // 2003版本
            {
                workbook = new HSSFWorkbook(fs);
            }

            ISheet sheet = workbook.GetSheetAt(0);

            LoadHeaders(sheet, dataService);
            LoadFilters(workbook.GetSheetAt(1), filterService);

            return LoadDatas(sheet, dataService);
        }

        private static void LoadHeaders(ISheet sheet, IDataService dataService)
        {
            IRow headerRow = sheet.GetRow(0);

            if (headerRow != null)
            {
                for (var i = 1; i < headerRow.LastCellNum; i++)
                {
                    dataService.AddValue(new DataValue()
                    {
                        Id = i,
                        Name = headerRow.GetCell(i).StringCellValue,
                        Type = DataValueType.Number
                    });
                }
            }
        }

        private static Data[] LoadDatas(ISheet sheet, IDataService dataService)
        {
            List<Data> datas = new List<Data>();

            if (sheet != null)
            {
                int index = 1;

                while (index <= sheet.LastRowNum)
                {
                    IRow cells = sheet.GetRow(index++);

                    if (cells is null)
                    {
                        continue;
                    }

                    Data data = new Data(cells.GetCell(0).StringCellValue);

                    for (int i = 1; i < cells.LastCellNum; i++)
                    {
                        data.AddValue(new DataValue()
                        {
                            Id = i,
                            Name = dataService.Values.Where(value => value.Id == i).First().Name,
                            Type = DataValueType.Number
                        }, cells.GetCell(i).NumericCellValue);
                    }

                    data.Flush();

                    datas.Add(data);
                }
            }

            return datas.ToArray();
        }

        private static void LoadFilters(ISheet sheet, IFilterService filterService)
        {
            for(var i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                filterService.AddFilter(new Library.Filters.Filter()
                {
                    DataValue = new DataValue() { Id = Convert.ToInt32(row.GetCell(0).NumericCellValue) },
                    Type = Library.Filters.FilterType.Average,
                    Weighting = 1,
                    VarianceLimit = 10
                });
            }
        }

        public void Save(string path)
        {

        }
    }
}
