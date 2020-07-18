using System;
using GroupClasses.Service;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;

[assembly: Dependency(typeof(GroupClasses.Mac.Service.FilePickerService))]
namespace GroupClasses.Mac.Service
{
    public class FilePickerService : IFilePickerService
    {
        public string PickFilePath()
        {
            try
            {
                FileData fileData = CrossFilePicker.Current.PickFile().Result;

                if (fileData == null)
                    return null; // user canceled file picking

                return fileData.FilePath;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
