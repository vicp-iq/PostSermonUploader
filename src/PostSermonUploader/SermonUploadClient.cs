using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PostSermonUploader
{
    public class SermonUploadClient
    {
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

        private static SermonUploadClient _client;

        private SermonUploadClient()
        {
            InitializeUploadSermonBackgroundWorker();
        }

        private void InitializeUploadSermonBackgroundWorker()
        {
            UploadSermonBackgroundWorker = new BackgroundWorker();
            UploadSermonBackgroundWorker.DoWork += UploadSermonAsync;
            UploadSermonBackgroundWorker.ProgressChanged += OnSermonUploadProgressChanged;
            UploadSermonBackgroundWorker.RunWorkerCompleted += OnSermonUploadComplete;
            UploadSermonBackgroundWorker.WorkerReportsProgress = true;
        }

        public static async Task PerformUpload(FtpState ftpState)
        {
            Stream requestStream;
            // End the asynchronous call to get the request stream.

            requestStream = ftpState.Request.GetRequestStream();
            // Copy the file contents to the request stream.
            const int bufferLength = 10000;
            byte[] buffer = new byte[bufferLength];
            int count = 0;
            int readBytes;
            FileStream stream = File.OpenRead(ftpState.FileName);
            do
            {
                readBytes = stream.Read(buffer, 0, bufferLength);
                requestStream.Write(buffer, 0, readBytes);
                count += readBytes;
                //worker.ReportProgress((int)(((double)count / stream.Length) * 100));
            } while (readBytes != 0);

            // IMPORTANT: Close the request stream before sending the request.
            requestStream.Close();
            stream.Close();

            FtpWebResponse response = null;
            response = (FtpWebResponse)ftpState.Request.GetResponse();
            response.Close();
            ftpState.StatusDescription = response.StatusDescription;

            //e.Result = ftpState;
        }

        public static SermonUploadClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new SermonUploadClient();
                }

                return _client;
            }
        }

        public bool IsUploadInProgress
        {
            get
            {
                return UploadSermonBackgroundWorker.IsBusy;
            }
        }

        private BackgroundWorker UploadSermonBackgroundWorker { get; set; }
        public Action<string> UpdateStatusMessage { get; set; }

        private FtpWebRequest GetDownloadsFileClient(string fileURL)
        {
            var client = (FtpWebRequest)(WebRequest.Create(fileURL));
            client.Credentials = new NetworkCredential(Username, Password);

            return client;
        }

        public void UploadSermon(string fileName)
        {
            var lTimeStamp = Utilities.ParseFilename(fileName);
            var lLocalPath = GetPath(lTimeStamp, fileName, Environment.Local);
            var lServerPath = GetPath(lTimeStamp, fileName, Environment.Server);

            var tempPath = Path.GetTempFileName();
            File.Copy(lLocalPath, tempPath, true);

            //Verify that the path actually exists
            FtpWebRequest request;
            FtpWebResponse response;
            StreamReader reader;
            string directoryContents = null;

            try
            {
                request = GetDownloadsFileClient(FTPServerAddress + Path.GetDirectoryName(lServerPath));
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                using (response = (FtpWebResponse)request.GetResponse())
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
                //create a new directory
                request = GetDownloadsFileClient(FTPServerAddress + Path.GetDirectoryName(lServerPath));
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                response = (FtpWebResponse)request.GetResponse();
                if (response.StatusCode != FtpStatusCode.PathnameCreated)
                {
                    throw new Exception("Failed to create new folder");
                }
            }

            var state = new FtpState();
            request = (FtpWebRequest)(WebRequest.Create(FTPServerAddress + lServerPath));
            request.Credentials = new NetworkCredential(Username, Password);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            state.Request = request;
            state.FileName = tempPath;

            UploadSermonBackgroundWorker.RunWorkerAsync(state);
        }

        private void OnSermonUploadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            var state = e.Result as FtpState;
            UpdateStatusMessage("Sermon upload complete");
            File.Delete(state.FileName);
        }

        private void OnSermonUploadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateStatusMessage($"Uploading Sermon ({e.ProgressPercentage}% complete)");
        }

        private static void UploadSermonAsync(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            FtpState state = (FtpState)e.Argument;

            Stream requestStream;
            // End the asynchronous call to get the request stream.

            requestStream = state.Request.GetRequestStream();
            // Copy the file contents to the request stream.
            const int bufferLength = 10000;
            byte[] buffer = new byte[bufferLength];
            int count = 0;
            int readBytes;
            FileStream stream = File.OpenRead(state.FileName);
            do
            {
                readBytes = stream.Read(buffer, 0, bufferLength);
                requestStream.Write(buffer, 0, readBytes);
                count += readBytes;
                worker.ReportProgress((int)(((double)count / stream.Length) * 100));
            } while (readBytes != 0);

            // IMPORTANT: Close the request stream before sending the request.
            requestStream.Close();
            stream.Close();

            FtpWebResponse response = null;
            response = (FtpWebResponse)state.Request.GetResponse();
            response.Close();
            state.StatusDescription = response.StatusDescription;

            e.Result = state;
        }

        public static string GetPath(DateTime aTimeStamp, string aFileName, Environment lEnvironment)
        {
            string lReturn;

            var lMonth = MonthMapping.Mappings.FirstOrDefault(x => x.Number == aTimeStamp.Month);

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

        public static string GetPath(string fileName, Environment environment)
        {
            var lTimeStamp = Utilities.ParseFilename(fileName);
            return GetPath(lTimeStamp, fileName, environment);
        }

        public enum Environment { Local, Server, RelativeServer };
    }

    public class FtpState
    {
        public FtpWebRequest Request { get; set; }
        public string FileName { get; set; }
        public string StatusDescription { get; set; }
    }
}