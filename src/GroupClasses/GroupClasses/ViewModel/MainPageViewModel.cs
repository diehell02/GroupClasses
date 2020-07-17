using System;
using System.Threading.Tasks;
using GroupClasses.Library.Datas;
using GroupClasses.Library.GroupCore;
using GroupClasses.Library.Service;
using GroupClasses.Service;
using Xamarin.Forms;

namespace GroupClasses.ViewModel
{
    public class MainPageViewModel : AbstractNotifyPropertyChanged
    {
        private string dataPath;

        public string DataPath
        {
            get => dataPath;
            set
            {
                dataPath = value;
                InvokePropertyChanged(nameof(DataPath));
            }
        }

        public string GroupResultText
        {
            get;
            set;
        }

        public int ClassNumber
        {
            get;
            set;
        }

        private DataService dataService;
        private FilterService filterService;
        private Data[][] dataResults;

        public async void ImportClickCommand()
        {
            try
            {
                string filePath =
                    await Task.Run(() =>
                    DependencyService.Get<IFilePickerService>()?.PickFilePath());

                Console.WriteLine("File name chosen: " + filePath);

                dataService = new DataService();
                filterService = new FilterService();
                var datas = DependencyService.Get<IExcelService>()?.Load(filePath, dataService, filterService);

                var group = new Group(dataService, filterService);

                dataResults = await group.Grouping(datas, ClassNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        public async void ExportClickCommand()
        {
            string filePath =
                    await Task.Run(() =>
                    DependencyService.Get<IFilePickerService>()?.PickFilePath());

            Console.WriteLine("File name chosen: " + filePath);

            DependencyService.Get<IExcelService>()?.Save(filePath, dataResults, dataService);
        }
    }
}
