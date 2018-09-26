using System.IO;
using System.Reflection;

namespace PostSermonUploader.IntegrationTests
{
    public static class TestHelper
    {
        public static string GetTestsPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)?.Replace(@"", string.Empty);
        }

        public static string GetTestsPath(string relativePath)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            var directory = new System.Uri(path).LocalPath;

            return Path.Combine(directory, relativePath);
        }
    }
}