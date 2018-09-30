using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PostSermonUploader.Clients;
using PostSermonUploader.Controllers;
using PostSermonUploader.Helpers;
using PostSermonUploader.Models;
using WPFFramework;

namespace PostSermonUploader.Views
{
    public class SermonDetailsViewModel : ViewModel
    {
        public SermonDetails SermonDetails { get; set; }

        public SermonDetailsViewModel()
        {
            SermonDetails = new SermonDetails();
            SetDefaults();
            HookupCommands();
        }

        private void SetDefaults()
        {
            Pastor = "Brad Warkentin";
            Title = DateTime.Today.ToShortDateString();
            Filename =
                $"tbc_{MonthMapping.Mappings.First(x => x.Number == DateTime.Today.Month).Shorthand}_{DateTime.Today.Day:00}_{DateTime.Today.Year}.mp3";
            Attachments = new ObservableCollection<Attachment>();
        }

        public string Pastor
        {
            get { return Get(SermonDetails, sermonDetails => sermonDetails.Pastor); }
            set
            {
                if (SermonDetails != null && SermonDetails.Pastor != value)
                {
                    SermonDetails.Pastor = value;
                    RaisePropertyChanged(() => Pastor);
                }
            }
        }

        public string Title
        {
            get { return Get(SermonDetails, sermonDetails => sermonDetails.Title); }
            set
            {
                if (SermonDetails != null && SermonDetails.Title != value)
                {
                    SermonDetails.Title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        public string Filename
        {
            get { return Get(SermonDetails, sermonDetails => sermonDetails.Filename); }
            set
            {
                if (SermonDetails != null && SermonDetails.Filename != value)
                {
                    SermonDetails.Filename = value;
                    RaisePropertyChanged(() => Filename);
                }
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public ObservableCollection<Attachment> Attachments { get; set; }

        public DelegateCommand PostAndUploadCommand { get; set; }
        public DelegateCommand SendEmailCommand { get; set; }
        public DelegateCommand UploadFileCommand { get; set; }
        public DelegateCommand AddAttachmentCommand { get; set; }

        private void HookupCommands()
        {
            PostAndUploadCommand = new DelegateCommand(PostAndUpload);
            SendEmailCommand = new DelegateCommand(SendEmail);
            UploadFileCommand = new DelegateCommand(UploadFiles);
            AddAttachmentCommand = new DelegateCommand(AddAttachment);
        }

        private Task AddAttachment()
        {
            var attachmentSelectorViewModel = new AttachmentSelectorViewModel();
            var attachmentSelectorView = new AttachmentSelectorView {DataContext = attachmentSelectorViewModel};

            attachmentSelectorView.ShowDialog();

            if (attachmentSelectorViewModel.IsSuccess)
            {
                Attachments.Add(new Attachment { Name = attachmentSelectorViewModel.Title, Path = attachmentSelectorViewModel.File });
            }

            return null;
        }

        private async Task SendEmail()
        {
            try
            {
                var sermonUploader = CreateSermonUploader();
                await sermonUploader.SendEmail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private SermonUploader CreateSermonUploader()
        {
            var sermonUploader = new SermonUploader
            {
                FileName = Filename,
                Pastor = Pastor,
                UpdateStatusMessage = UpdateStatusMessage,
                Title = Title,
                Attachments = Attachments.ToArray()
            };
            return sermonUploader;
        }

        private async Task UploadFiles()
        {
            try
            {
                var sermonUploader = CreateSermonUploader();

                await sermonUploader.PerformUpload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task PostAndUpload()
        {
            await SendEmail();
            await UploadFiles();
        }

        private void UpdateStatusMessage(string message)
        {
            Status = message;
        }
    }
}