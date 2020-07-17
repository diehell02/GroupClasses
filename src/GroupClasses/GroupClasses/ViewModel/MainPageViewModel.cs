using System;
using GroupClasses.Library.Datas;
using GroupClasses.Library.GroupCore;
using GroupClasses.Library.Service;
using GroupClasses.Utils;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;

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
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string filePath = fileData.FilePath;

                Console.WriteLine("File name chosen: " + filePath);

                dataService = new DataService();
                filterService = new FilterService();
                var datas = FileUtil.Load(filePath, dataService, filterService);

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
            FileData fileData = await CrossFilePicker.Current.PickFile();
            if (fileData == null)
                return; // user canceled file picking

            string filePath = fileData.FilePath;

            FileUtil.Save(filePath, dataResults, dataService);
        }
    }
}
