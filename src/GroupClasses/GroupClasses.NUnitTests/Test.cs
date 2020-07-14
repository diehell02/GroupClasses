using GroupClasses.Library.GroupCore;
using GroupClasses.Library.Service;
using NUnit.Framework;
using System;
namespace GroupClasses.NUnitTests
{
    [TestFixture()]
    public class Test
    {
        //[Test()]
        //public void TestLoad()
        //{
        //    FileUtil.Load("TestDatas/Group Test.xlsx");
        //}

        [Test()]
        public void TestGroup()
        {
            var dataService = new DataService();
            var filterService = new FilterService();
            var datas = FileUtil.Load("TestDatas/Group Test.xlsx", dataService, filterService);

            var group = new Group(dataService, filterService);

            group.Grouping(datas, 3);
        }
    }
}
