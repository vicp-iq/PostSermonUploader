using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace PostSermonUploader
{
    public class FTPClient
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

        public void UploadFile(string sourceFile, string targetFolder)
        {
            var targetUploadPath = FTPServerAddress + targetFolder;

            if (!DoesFolderExistForFile(targetUploadPath))
            {
                CreateFolderForFile(targetUploadPath);
            }

            PerformUpload(sourceFile, targetUploadPath);
        }

        private bool DoesFolderExistForFile(string targetFilePath)
        {
            var targetFolderPath = Path.GetDirectoryName(targetFilePath);
            if (targetFolderPath == null)
                return true;

            var ftpWebRequest = GenerateWebRequestForUri(targetFolderPath);

            string directoryContents;
            ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            using (var response = (FtpWebResponse) ftpWebRequest.GetResponse())
            {
                var responseStream = response.GetResponseStream();

                Debug.Assert(responseStream != null, "responseStream != null");
                using (var reader = new StreamReader(responseStream))
                {
                    directoryContents = reader.ReadToEnd();
                }
            }

            return !string.IsNullOrEmpty(directoryContents);
        }

        private void CreateFolderForFile(string targetFilePath)
        {
            var targetFolderPath = Path.GetDirectoryName(targetFilePath);
            if (targetFolderPath == null)
                throw new NotSupportedException("Should not have hit this code for files in the root path");

            var ftpWebRequest = GenerateWebRequestForUri(targetFolderPath);
            ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            using (var response = (FtpWebResponse) ftpWebRequest.GetResponse())
            {
                if (response.StatusCode != FtpStatusCode.PathnameCreated)
                {
                    throw new Exception($"Failed to create folder: {response.StatusCode}");
                }
            }
        }

        private void PerformUpload(string sourceFile, string targetUploadPath)
        {
            var ftpWebRequest = GenerateWebRequestForUri(targetUploadPath);

            ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

            using (var sourceFileStream = File.OpenRead(sourceFile))
            {
                const int bufferLength = 10000;
                byte[] buffer = new byte[bufferLength];

                using (var ftpRequestStream = ftpWebRequest.GetRequestStream())
                {
                    int bytesRead;
                    do
                    {
                        bytesRead = sourceFileStream.Read(buffer, 0, bufferLength);
                        ftpRequestStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
            }
        }

        private FtpWebRequest GenerateWebRequestForUri(string targetUploadPath)
        {
            var ftpWebRequest = (FtpWebRequest) WebRequest.Create(targetUploadPath);

            if (Username != null)
            {
                ftpWebRequest.Credentials = new NetworkCredential(Username, Password);
            }
            return ftpWebRequest;
        }

        public string DownloadFile(string fileNameInTarget)
        {
            throw new NotImplementedException();
        }
    }
}