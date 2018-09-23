using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostSermonUploader
{
    public class Attachment
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public string DisplayText
        {
            get
            {
                return string.Format("Attachment: {0} - {1}", Name, Path);
            }
        }
    }
}
