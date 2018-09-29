using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using PostSermonUploader.Clients;
using PostSermonUploader.Controllers;
using PostSermonUploader.Models;

namespace PostSermonUploader.UnitTests
{
    [TestFixture]
    public class SermonUploaderTests
    {
        private IFTPClient _ftpClient;
        private SermonUploader sut { get; set; }

        [SetUp]
        public void SetUp()
        {
            _ftpClient = A.Fake<IFTPClient>();
            sut = new SermonUploader(_ftpClient)
            {
                FileName = "tbc_sep_29_2018.mp3"
            };
        }

        [Test]
        public async Task AllAttachmentsGetSentToFTPClient()
        {
            var attachments = new[] {new Attachment() {Path = "A"}, new Attachment() {Path = "B"}};

            sut.Attachments = attachments;

            await sut.UploadAttachments();

            A.CallTo(() => _ftpClient.UploadFile("A", A<string>._)).MustHaveHappened();
            A.CallTo(() => _ftpClient.UploadFile("B", A<string>._)).MustHaveHappened();
        }

        [Test]
        public async Task AttachmentHasProperServerPath()
        {
            var attachments = new[] { new Attachment() { Path = "A" } };

            sut.Attachments = attachments;
            sut.FileName = "tbc_sep_29_2018.mp3";

            await sut.UploadAttachments();

            var expectedServerPath = "/wp-content/uploads/2018/09_sep\\A";

            A.CallTo(() => _ftpClient.UploadFile(A<string>._,  expectedServerPath)).MustHaveHappened();
        }


    }
}
