using System;
using System.Collections.Generic;
using System.IO;
using GroupClasses.Library.Datas;
using GroupClasses.Library.Service;
using GroupClasses.Library.Filters;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Linq;

namespace GroupClasses.ConsoleClient
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

            LoadFilters(workbook.GetSheetAt(1), filterService);
            LoadHeaders(sheet, dataService, filterService);

            return LoadDatas(sheet, dataService);
        }

        private static void LoadHeaders(ISheet sheet, IDataService dataService, IFilterService filterService)
        {
            IRow headerRow = sheet.GetRow(0);

            if (headerRow != null)
            {
                for (var i = 0; i < headerRow.LastCellNum; i++)
                {
                    var cell = headerRow.GetCell(i);
                    var id = i;
                    var name = headerRow.GetCell(i).StringCellValue;

                    var filter = filterService.Filters.Where(_filter => _filter.DataValue.Name == name).FirstOrDefault();
                    DataValue dataValue = null;

                    if (filter != null)
                    {
                        dataValue = new DataValue()
                        {
                            Id = i,
                            Name = headerRow.GetCell(i).StringCellValue,
                            Type = filter.Type == FilterType.Condition ? DataValueType.Binary : DataValueType.Number
                        };
                        filter.DataValue = dataValue;
                    } else
                    {
                        dataValue = new DataValue()
                        {
                            Id = i,
                            Name = headerRow.GetCell(i).StringCellValue,
                            Type = DataValueType.String
                        };
                    }

                    dataService.AddValue(dataValue);
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

                    Data data = new Data();

                    for (int i = 0; i < cells.LastCellNum; i++)
                    {
                        var dataValue = dataService.Values.Where(_dataValue => _dataValue.Id == i).First();

                        switch(dataValue.Type)
                        {
                            case DataValueType.Number:
                                data.AddValue(dataValue, cells.GetCell(i).NumericCellValue);
                                break;
                            case DataValueType.Binary:
                                data.AddValue(dataValue, cells.GetCell(i).ToString());
                                break;
                            case DataValueType.String:
                                data.AddValue(dataValue, cells.GetCell(i).StringCellValue);
                                break;
                        }
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

                filterService.AddFilter(new Filter()
                {
                    DataValue = new DataValue() { Name = row.GetCell(0).StringCellValue },
                    Type = (FilterType)Enum.Parse(typeof(FilterType), row.GetCell(1).StringCellValue),
                    Weighting = Convert.ToDecimal(row.GetCell(2).NumericCellValue),
                    VarianceLimit = Convert.ToInt32(row.GetCell(3).NumericCellValue)
                });
            }
        }

        public static void Save(string path, Data[][] datas, IDataService dataService)
        {
            IWorkbook workbook = null;

            var fs = File.OpenWrite(path);

            if (path.IndexOf(".xlsx") > 0) // 2007版本 
            {
                workbook = new XSSFWorkbook();
            }
            else if (path.IndexOf(".xls") > 0) // 2003版本
            {
                workbook = new HSSFWorkbook();
            }

            for (var i = 0; i < datas.Length; i++)
            {
                var _class = datas[i];
                ISheet sheet = workbook.CreateSheet($"Class{i}");

                int rowIndex = 0;
                int colIndex = 0;

                var row = sheet.CreateRow(rowIndex++);

                foreach(var header in dataService.Values)
                {
                    row.CreateCell(colIndex++).SetCellValue(header.Name);
                }

                foreach (var data in _class)
                {
                    colIndex = 0;
                    row = sheet.CreateRow(rowIndex++);

                    for (int z = 0; z < data.Values.Count(); z++)
                    {
                        var value = data.Values[z];

                        switch (value.Key.Type)
                        {
                            case DataValueType.String:
                                row.CreateCell(colIndex++).SetCellValue(value.Value.ToString());
                                break;
                            case DataValueType.Number:
                                row.CreateCell(colIndex++).SetCellValue(Convert.ToDouble(value.Value));
                                break;
                            default:
                                row.CreateCell(colIndex++).SetCellValue(value.Value.ToString());
                                break;
                        }
                    }
                }
            }

            workbook.Write(fs);

            fs.Close();
        }
    }
}
