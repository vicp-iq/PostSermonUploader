using System.Net;

namespace PostSermonUploader.Helpers
{
    public class FtpState
    {
        public FtpWebRequest Request { get; set; }
        public string FileName { get; set; }
        public string StatusDescription { get; set; }
    }
}