using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PostSermonUploader.Helpers;

namespace PostSermonUploader.Clients
{
    //Tracks whether there is an upload in progress
    //Knows how to figure out paths
    public class SermonUploadClient
    {
        public SermonUploadClient()
        {
            _ftpClient = new FTPClient();
        }

        private static SermonUploadClient _client;
        private IFTPClient _ftpClient;

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

        public bool IsUploadInProgress { get; set; }

        public Action<string> UpdateStatusMessage { get; set; }

        public async Task UploadSermon(string fileName)
        {
            IsUploadInProgress = true;
            var lTimeStamp = Utilities.ParseFilename(fileName);
            var lLocalPath = GetPath(lTimeStamp, fileName, Environment.Local);
            var lServerPath = GetPath(lTimeStamp, fileName, Environment.Server);

            await _ftpClient.UploadFile(lLocalPath, lServerPath);

            IsUploadInProgress = false;
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