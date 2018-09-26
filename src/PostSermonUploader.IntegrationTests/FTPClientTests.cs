using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using PostSermonUploader.Clients;

namespace PostSermonUploader.IntegrationTests
{
    [TestFixture]
    public class FTPClientTests
    {
        private FTPClient sut;

        [OneTimeSetUp]
        public void ClassInitialize()
        {
            sut = new FTPClient();
        }

        [Test]
        public async Task UploadFile_ShouldNotThrowException_WhenUploadingFileToExistingFolder()
        {
            var sourceFile = TestHelper.GetTestsPath(@"./SourceFile.txt");
            var targetFile = @"tests/SourceFile.txt";

            await sut.UploadFile(sourceFile, targetFile);
        }

        [Test]
        public async Task UploadFile_ShouldNotThrowException_WhenUploadingFileToNonExistentFolder()
        {
            var sourceFile = TestHelper.GetTestsPath(@"./SourceFile.txt");
            var targetFile = @"tests/FTPClientTests/SourceFile.txt";

            //Validate Folder Doesn't Exist

            await sut.UploadFile(sourceFile, targetFile);
        }

        //TODO: Should Send the completion percentage somewhere
    }
}
