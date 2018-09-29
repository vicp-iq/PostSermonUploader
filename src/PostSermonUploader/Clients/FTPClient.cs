using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PostSermonUploader.Controllers;

namespace PostSermonUploader.Clients
{
    public class FTPClient:IFTPClient
    {
        public Action<string> UpdateStatusMessage { get; }

        public FTPClient(Action<string> updateStatusMessage)
        {
            UpdateStatusMessage = updateStatusMessage;
        }

        private string FTPServerAddress
        {
            get { return ConfigurationManager.AppSettings["FTPServerAddress"]; }
        }

        private string Username
        {
            get { return ConfigurationManager.AppSettings["FTPServerUserName"]; }
        }

        private string Password
        {
            get { return ConfigurationManager.AppSettings["FTPServerPassword"]; }
        }

        public async Task UploadFile(string lLocalPath, string lServerPath)
        {
            var tempPath = CopyFileToTempFolder(lLocalPath);

            await CreateFolderIfNecessary(lServerPath);
            await PerformUpload(lServerPath, tempPath);
        }

        private static string CopyFileToTempFolder(string lLocalPath)
        {
            var tempPath = Path.GetTempFileName();
            File.Copy(lLocalPath, tempPath, true);
            return tempPath;
        }

        private async Task PerformUpload(string lServerPath, string tempPath)
        {
            var state = new FtpState();
            var request = (FtpWebRequest) (WebRequest.Create(FTPServerAddress + lServerPath));
            request.Credentials = new NetworkCredential(Username, Password);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            state.Request = request;
            state.FileName = tempPath;

            using (var requestStream = await state.Request.GetRequestStreamAsync())
            {
                const int bufferLength = 10000;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes;
                using (FileStream stream = File.OpenRead(state.FileName))
                {
                    do
                    {
                        readBytes = await stream.ReadAsync(buffer, 0, bufferLength);
                        await requestStream.WriteAsync(buffer, 0, readBytes);
                        count += readBytes;
                        var percentageComplete = (int) (((double) count / stream.Length) * 100);
                        UpdateStatusMessage($"Uploading Sermon ({percentageComplete}% complete)");
                    } while (readBytes != 0);
                }
            }

            using (FtpWebResponse response = (FtpWebResponse) await state.Request.GetResponseAsync())
            {
                state.StatusDescription = response.StatusDescription;
            }
        }

        private async Task CreateFolderIfNecessary(string lServerPath)
        {
            FtpWebRequest request;
            FtpWebResponse response;
            StreamReader reader;
            string directoryContents = null;

            try
            {
                request = GetDownloadsFileClient(FTPServerAddress + Path.GetDirectoryName(lServerPath));
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                using (response = (FtpWebResponse) await request.GetResponseAsync())
                {
                    using (reader = new StreamReader(response.GetResponseStream()))
                    {
                        directoryContents = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException)
            {
                //Swallow this exception because there is a bug that causes it if the directory is empty
            }

            if (string.IsNullOrEmpty(directoryContents))
            {
                request = GetDownloadsFileClient(FTPServerAddress + Path.GetDirectoryName(lServerPath));
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                response = (FtpWebResponse) await request.GetResponseAsync();
                if (response.StatusCode != FtpStatusCode.PathnameCreated)
                {
                    throw new Exception("Failed to create new folder");
                }
            }
        }

        private FtpWebRequest GetDownloadsFileClient(string fileURL)
        {
            var client = (FtpWebRequest)(WebRequest.Create(fileURL));
            client.Credentials = new NetworkCredential(Username, Password);

            return client;
        }
    }
}