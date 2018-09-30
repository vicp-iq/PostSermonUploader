namespace PostSermonUploader.Models
{
    public class Attachment
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public string DisplayText => $"Attachment: {Name} - {Path}";
    }
}
