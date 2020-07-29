using GroupClasses.Library.GroupCore;
using GroupClasses.Library.Service;
using NPOI.OpenXmlFormats.Dml.Diagram;
using System;
using System.IO;

namespace GroupClasses.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Group();
            }
        }

        private static void Group()
        {
            Console.WriteLine("Please input the data:");
            string path = Console.ReadLine().Trim('\"');
            Console.WriteLine("Please input the class number:");
            string groupCount = Console.ReadLine();

            var dataService = new DataService();
            var filterService = new FilterService();
            var datas = FileUtil.Load(path, dataService, filterService);

            var group = new Group(dataService, filterService);

            var directoryInfo = new DirectoryInfo(path);
            string savePath = $"{directoryInfo.Parent.FullName}//output.xlsx";

            FileUtil.Save(savePath, group.Grouping(datas, int.Parse(groupCount)).Result, dataService);

            Console.WriteLine($"Have saved into {savePath}");
        }
    }
}
