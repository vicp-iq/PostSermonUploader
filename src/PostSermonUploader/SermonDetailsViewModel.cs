using Frameworks.Presentation;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PostSermonUploader
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
            UploadFileCommand = new DelegateCommand(UploadFile);
            AddAttachmentCommand = new DelegateCommand(AddAttachment);
        }

        private void AddAttachment()
        {
            var attachmentSelectorViewModel = new AttachmentSelectorViewModel();
            var attachmentSelectorView = new AttachmentSelectorView {DataContext = attachmentSelectorViewModel};

            attachmentSelectorView.ShowDialog();

            if (attachmentSelectorViewModel.IsSuccess)
            {
                Attachments.Add(new Attachment { Name = attachmentSelectorViewModel.Title, Path = attachmentSelectorViewModel.File });
            }
        }

        private void SendEmail()
        {
            try
            {
                var uploader = new SermonUploadManager(Filename, Pastor, Title, UpdateStatusMessage);
                uploader.SendEmail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UploadFile()
        {
            try
            {
                var ftpClient = SermonUploadClient.Client;

                if (ftpClient.IsUploadInProgress)
                {
                    MessageBox.Show("Clicking more won't make it go faster, you know (Proverbs 15:18).");
                    return;
                }

                ftpClient.UpdateStatusMessage = UpdateStatusMessage;

                var uploader = new SermonUploadManager(Filename, Pastor, Title, UpdateStatusMessage);
                uploader.UploadFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void PostAndUpload()
        {
            try
            {
                var uploader = new SermonUploadManager(Filename, Pastor, Title, UpdateStatusMessage);
                uploader.PostAndUpload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateStatusMessage(string message)
        {
            Status = message;
        }
    }
}