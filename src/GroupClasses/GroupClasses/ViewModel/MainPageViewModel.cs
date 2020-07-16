using System;
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

        public async void ImportClickCommand()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

                Console.WriteLine("File name chosen: " + fileName);
                Console.WriteLine("File data: " + contents);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        public void ExportClickCommand()
        {

        }
    }
}
