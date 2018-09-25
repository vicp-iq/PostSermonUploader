using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostSermonUploader
{
    public class SermonUploadManager
    {
        private string FileName { get; set; }
        private string Pastor { get; set; }
        private string Title { get; set; }
        private Action<string> UpdateStatusMessage { get; set; }

        public SermonUploadManager(string fileName, string pastor, string title, Action<string> updateStatusMessage)
        {
            FileName = fileName;
            Pastor = pastor;
            Title = title;
            UpdateStatusMessage = updateStatusMessage;
        }

        public async void PostAndUpload()
        {
            if (FileNameIsValid())
            {
                var ftpClient = SermonUploadClient.Client;

                if (ftpClient.IsUploadInProgress)
                {
                    MessageBox.Show(@"Clicking more won't make it go faster, you know (Proverbs 15:18).");
                    return;
                }

                ftpClient.UpdateStatusMessage = UpdateStatusMessage;

                SendEmail();
                await UploadFile();
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
                $@"[audio mp3=""http://proceduraltextures.com/trinity{SermonUploadClient.GetPath(FileName,
                    SermonUploadClient.Environment.Server)}""][/audio]
<a href=""http://proceduraltextures.com/trinity/wp-content/uploads/downloadfile.php?file={SermonUploadClient.GetPath(FileName,
                    SermonUploadClient.Environment.RelativeServer)}"">Download</a>";
            return result;
        }

        private string MakeSubject()
        {
            var result = $"{Pastor} - {Title}";
            return result;
        }

        public async Task UploadFile()
        {
            if (FileNameIsValid())
            {
                UpdateStatusMessage("Uploading File");

                var ftpClient = SermonUploadClient.Client;

                if (ftpClient.IsUploadInProgress)
                {
                    MessageBox.Show(@"Clicking more won't make it go faster, you know (Proverbs 15:18).");
                    return;
                }

                ftpClient.UpdateStatusMessage = UpdateStatusMessage;

                var client = SermonUploadClient.Client;
                await client.UploadSermon(FileName);
            }
        }
    }
}