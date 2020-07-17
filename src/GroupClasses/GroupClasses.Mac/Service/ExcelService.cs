using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GroupClasses.Library.Datas;
using GroupClasses.Library.Service;
using OfficeOpenXml;
using Xamarin.Forms;

[assembly: Dependency(typeof(GroupClasses.Mac.Service.ExcelService))]
namespace GroupClasses.Mac.Service
{
    public class ExcelService
    {
        public Data[] Load(string path, IDataService dataService, IFilterService filterService)
        {
            var fs = File.OpenRead(path);
            Data[] datas = null;

            using (ExcelPackage package = new ExcelPackage(fs))
            {
                var dataSheet = package.Workbook.Worksheets[0];
                var filterSheet = package.Workbook.Worksheets[1];

                LoadFilters(filterSheet, filterService);
                LoadHeaders(dataSheet, dataService, filterService);

                datas = LoadDatas(dataSheet, dataService);
            }

            return datas;
        }

        private void LoadHeaders(ExcelWorksheet workSheet, IDataService dataService, IFilterService filterService)
        {
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            for (var i = 0; i < end.Column; i++)
            {
                var id = i;
                var name = workSheet.Cells[0, i].Value.ToString();

                var filter = filterService.Filters.Where(_filter => _filter.DataValue.Name == name).FirstOrDefault();
                DataValue dataValue = null;

                if (filter != null)
                {
                    dataValue = new DataValue()
                    {
                        Id = i,
                        Name = name,
                        Type = DataValueType.Number
                    };
                    filter.DataValue = dataValue;
                }
                else
                {
                    dataValue = new DataValue()
                    {
                        Id = i,
                        Name = name,
                        Type = DataValueType.String
                    };
                }

                dataService.AddValue(dataValue);
            }
        }

        private Data[] LoadDatas(ExcelWorksheet workSheet, IDataService dataService)
        {
            List<Data> datas = new List<Data>();

            if (workSheet != null)
            {
                var start = workSheet.Dimension.Start;
                var end = workSheet.Dimension.End;
                int index = start.Row + 1;

                while (index <= end.Row)
                {
                    Data data = new Data();

                    for (int i = 0; i < end.Column; i++)
                    {
                        var dataValue = dataService.Values.Where(_dataValue => _dataValue.Id == i).First();

                        switch (dataValue.Type)
                        {
                            case DataValueType.Number:
                                data.AddValue(dataValue, Convert.ToDecimal(workSheet.Cells[index, i].Value));
                                break;
                            case DataValueType.String:
                                data.AddValue(dataValue, workSheet.Cells[index, i].Value.ToString());
                                break;
                        }
                    }

                    data.Flush();

                    datas.Add(data);

                    index++;
                }
            }

            return datas.ToArray();
        }

        private void LoadFilters(ExcelWorksheet workSheet, IFilterService filterService)
        {
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            for (var i = 1; i <= end.Row; i++)
            {
                filterService.AddFilter(new Library.Filters.Filter()
                {
                    DataValue = new DataValue() { Name = workSheet.Cells[i, 0].Value.ToString() },
                    Type = Library.Filters.FilterType.Average,
                    Weighting = Convert.ToDecimal(workSheet.Cells[i, 2].Value),
                    VarianceLimit = Convert.ToInt32(workSheet.Cells[i, 3].Value)
                });
            }
        }

        public void Save(string path, Data[][] datas, IDataService dataService)
        {
            var fs = File.OpenWrite(path);

            using (ExcelPackage package = new ExcelPackage(fs))
            {
                for (var i = 0; i < datas.Length; i++)
                {
                    var _class = datas[i];
                    var worksheet = package.Workbook.Worksheets.Add($"Class{i}");

                    int rowIndex = 0;
                    int colIndex = 0;

                    foreach (var header in dataService.Values)
                    {
                        worksheet.Cells[rowIndex, colIndex++].Value = header.Name;
                    }

                    rowIndex++;

                    foreach (var data in _class)
                    {
                        colIndex = 0;

                        for (int z = 0; z < data.Values.Count(); z++)
                        {
                            var value = data.Values[z];

                            switch (value.Key.Type)
                            {
                                case DataValueType.String:
                                    worksheet.Cells[z, colIndex++].Value = value.Value.ToString();
                                    break;
                                case DataValueType.Number:
                                    worksheet.Cells[z, colIndex++].Value = Convert.ToDouble(value.Value);
                                    break;
                                default:
                                    worksheet.Cells[z, colIndex++].Value = value.Value.ToString();
                                    break;
                            }
                        }

                        rowIndex++;
                    }
                }

                package.Save();
            }

            fs.Close();
        }
    }
}
