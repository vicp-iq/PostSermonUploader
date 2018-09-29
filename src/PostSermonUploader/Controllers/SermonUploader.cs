using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostSermonUploader.Clients;
using PostSermonUploader.Helpers;

namespace PostSermonUploader.Controllers
{
    public class SermonUploader
    {
        private string FileName { get; set; }
        private string Pastor { get; set; }
        private string Title { get; set; }
        private Action<string> UpdateStatusMessage { get; set; }

        public static bool IsUploadInProgress { get; set; }

        private readonly IFTPClient _ftpClient;

        public SermonUploader(string fileName, string pastor, string title, Action<string> updateStatusMessage)
        {
            FileName = fileName;
            Pastor = pastor;
            Title = title;
            UpdateStatusMessage = updateStatusMessage;

            _ftpClient = new FTPClient(UpdateStatusMessage);
        }

        public async void PostAndUpload()
        {
            if (FileNameIsValid())
            {
                if (IsUploadInProgress)
                {
                    MessageBox.Show(@"Clicking more won't make it go faster, you know (Proverbs 15:18).");
                    return;
                }

                SendEmail();
                await UploadFiles();
            }
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

        public void SendEmail()
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
                    client.Send(message);

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
                $@"[audio mp3=""http://proceduraltextures.com/trinity{GetPath(FileName,
                    Environment.Server)}""][/audio]
<a href=""http://proceduraltextures.com/trinity/wp-content/uploads/downloadfile.php?file={GetPath(FileName,
                    Environment.RelativeServer)}"">Download</a>";
            return result;
        }

        private string MakeSubject()
        {
            var result = $"{Pastor} - {Title}";
            return result;
        }

        public async Task UploadFiles()
        {
            if (FileNameIsValid())
            {
                UpdateStatusMessage("Uploading File");

                if (IsUploadInProgress)
                {
                    MessageBox.Show(@"Clicking more won't make it go faster, you know (Proverbs 15:18).");
                    return;
                }

                await UploadFilesCore(FileName);
            }
        }

        private async Task UploadFilesCore(string fileName)
        {
            IsUploadInProgress = true;
            var lTimeStamp = Utilities.ParseFilename(fileName);
            var lLocalPath = GetPath(lTimeStamp, fileName, Environment.Local);
            var lServerPath = GetPath(lTimeStamp, fileName, Environment.Server);

            await _ftpClient.UploadFile(lLocalPath, lServerPath);

            IsUploadInProgress = false;
        }

        private static string GetPath(DateTime aTimeStamp, string aFileName, Environment lEnvironment)
        {
            string lReturn;

            var lMonth = MonthMapping.Mappings.Single(x => x.Number == aTimeStamp.Month);

            switch (lEnvironment)
            {
                case Environment.Local:
                    lReturn =
                        $@"{ConfigurationManager.AppSettings["RecordingLocation"]}/{aTimeStamp.Year}/{lMonth.LocalName}/{aFileName}";
                    break;
                case Environment.Server:
                    lReturn = $@"/wp-content/uploads//{aTimeStamp.Year}/{lMonth.ServerName}/{aFileName}";
                    break;
                case Environment.RelativeServer:
                    lReturn = $@"{aTimeStamp.Year}/{lMonth.ServerName}/{aFileName}";
                    break;
                default:
                    throw new NotSupportedException();
            }

            return lReturn;
        }

        private static string GetPath(string fileName, Environment environment)
        {
            var lTimeStamp = Utilities.ParseFilename(fileName);
            return GetPath(lTimeStamp, fileName, environment);
        }
    }
}