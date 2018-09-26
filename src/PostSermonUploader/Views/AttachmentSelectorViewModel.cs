using System.Windows;
using Frameworks.Presentation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;

namespace PostSermonUploader.Views
{
    public class AttachmentSelectorViewModel: ViewModel
    {
        public AttachmentSelectorViewModel()
        {
            HookupCommands();
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        private string _file;
        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                RaisePropertyChanged(() => File);
            }
        }

        public bool IsSuccess { get; set; }

        public DelegateCommand FileDialogCommand { get; set; }
        public DelegateCommand<Window> OKCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }

        private void HookupCommands()
        {
            FileDialogCommand = new DelegateCommand(LaunchFileDialog);
            OKCommand = new DelegateCommand<Window>(OK);
            CancelCommand = new DelegateCommand<Window>(Cancel);
        }

        private void OK(Window window)
        {
            IsSuccess = !string.IsNullOrEmpty(File) && !string.IsNullOrEmpty(Title);
            window.Close();
        }

        private void Cancel(Window window)
        {
            IsSuccess = false;
            window.Close();
        }

        private void LaunchFileDialog()
        {
            var openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == true)
            {
                File = openFileDialog.FileName;
            }
        }
    }
}
