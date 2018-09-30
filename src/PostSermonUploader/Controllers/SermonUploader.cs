using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostSermonUploader.Clients;
using PostSermonUploader.Helpers;
using Attachment = PostSermonUploader.Models.Attachment;
using Environment = PostSermonUploader.Helpers.Environment;

namespace PostSermonUploader.Controllers
{
    public class SermonUploader
    { 
        public string FileName { get; set; }
        public string Pastor { get; set; }
        public string Title { get; set; }
        public Attachment[] Attachments { get; set; }

        public Action<string> UpdateStatusMessage { get; set; }

        public static bool IsUploadInProgress { get; set; }

        private IFTPClient _ftpClient;
        private IFTPClient FTPClient
        {
            get => _ftpClient ?? (_ftpClient = new FTPClient(UpdateStatusMessage));
            set => _ftpClient = value;
        }

        public SermonUploader()
        { }

        public SermonUploader(IFTPClient ftpClient)
        {
            FTPClient = ftpClient;
        }

        private bool FileNameIsValid()
        {
            var validationResult = Utilities.ValidateFileName(FileName);
            if (!string.IsNullOrEmpty(validationResult))
            {
                MessageBox.Show(validationResult);
                return false;
            }

            return true;
        }

        public async Task SendEmail()
        {
            if (FileNameIsValid())
            {
                UpdateStatusMessage("Posting");

                string to = ConfigurationManager.AppSettings["EmailDestination"];
                string from = "nthnthnth8374@gmail.com";

                MailMessage message = new MailMessage(from, to)
                {
                    Subject = MakeSubject(),
                    Body = MakeBody()
                };
                var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("nthnthnth8374", "Xhhohohho1234!")
                    };

                try
                {
                    //await client.SendMailAsync(message);

                    UpdateStatusMessage("Posted");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    UpdateStatusMessage("Failed to Post");
                }
            }
        }

        private string MakeBody()
        {
            var result =
                $@"[audio mp3=""http://proceduraltextures.com/trinity{GetPath(
                    Environment.Server)}""][/audio]
<a href=""http://proceduraltextures.com/trinity/wp-content/uploads/downloadfile.php?file={GetPath(
                    Environment.RelativeServer)}"">Download</a>";
            return result;
        }

        private string MakeSubject()
        {
            var result = $"{Pastor} - {Title}";
            return result;
        }

        public async Task PerformUpload()
        {
            if (FileNameIsValid())
            {
                UpdateStatusMessage("Uploading File");

                if (IsUploadInProgress)
                {
                    MessageBox.Show(@"Clicking more won't make it go faster, you know (Proverbs 15:18).");
                    return;
                }

                await PerformUploadCore();
            }
        }

        private async Task PerformUploadCore()
        {
            IsUploadInProgress = true;

            await UploadSermon();
            await UploadAttachments();

            UpdateStatusMessage("Finished Upload");

            IsUploadInProgress = false;
        }

        private async Task UploadSermon()
        {
            var localPath = GetPath(Environment.Local);
            var serverPath = GetPath(Environment.Server);

            await FTPClient.UploadFile(localPath, serverPath);
        }

        public async Task UploadAttachments()
        {
            var serverDirectory = GetDirectory(Environment.Server);

            foreach (var attachment in Attachments)
            {
                var fileName = Path.GetFileName(attachment.Path);
                var serverPath = Path.Combine(serverDirectory, fileName ?? throw new InvalidOperationException());
                await FTPClient.UploadFile(attachment.Path, serverPath);
            }
        }

        private string GetDirectory(Environment environment)
        {
            var timeStamp = Utilities.ParseFilename(FileName);

            string result;

            var month = MonthMapping.Mappings.Single(x => x.Number == timeStamp.Month);

            switch (environment)
            {
                case Environment.Local:
                    result =
                        $@"{ConfigurationManager.AppSettings["RecordingLocation"]}/{timeStamp.Year}/{month.LocalName}";
                    break;
                case Environment.Server:
                    result = $@"/wp-content/uploads/{timeStamp.Year}/{month.ServerName}";
                    break;
                case Environment.RelativeServer:
                    result = $@"{timeStamp.Year}/{month.ServerName}";
                    break;
                default:
                    throw new NotSupportedException();
            }

            return result;
        }

        private string GetPath(Environment environment)
        {
            var directory = GetDirectory(environment);

            return Path.Combine(directory, FileName);
        }
    }
}