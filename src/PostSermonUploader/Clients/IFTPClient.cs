using System.Threading.Tasks;

namespace PostSermonUploader.Clients
{
    public interface IFTPClient
    {
        Task UploadFile(string lLocalPath, string lServerPath);
    }
}