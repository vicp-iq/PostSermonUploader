using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostSermonUploader.IntegrationTests
{
    [TestClass]
    public class FTPClientTests
    {
        private FTPClient sut;

        [TestInitialize]
        public void ClassInitialize()
        {
            sut = new FTPClient();
        }

        [TestMethod]
        public void UploadFile_ShouldNotThrowException_WhenUploadingFileToRootFolder()
        {
            var sourceFile = @"./SourceFile.txt";
            var targetFile = @"SourceFile.txt";

            sut.UploadFile(sourceFile, targetFile);
        }

        [TestMethod]
        public void UploadFile_ShouldNotThrowException_WhenUploadingFileToNonExistentFolder()
        {
            var sourceFile = @"./SourceFile.txt";
            var targetFile = @"FTPClientTests/SourceFile.txt";

            //Validate Folder Doesn't Exist

            sut.UploadFile(sourceFile, targetFile);
        }



        //var fileNameInTarget = Path.Combine(targetFolder, Path.GetFileName(sourceFile));

        //var actual = sut.DownloadFile(fileNameInTarget);
        //var expected = File.ReadAllText(sourceFile);

        //Assert.AreEqual(expected, actual);
    }
}
