using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Reflection;

namespace QPROS_APITestingAssignment.Utilities
{
    public class InitializationForExtentReports
    {
        public static ExtentReports? _extent;
        public static ExtentTest? _test;

        public static ExtentReports InitializeExtentReports()
        {
            var htmlReporter = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/extentreport.html";
            var spark = new ExtentSparkReporter(htmlReporter);
            _extent = new ExtentReports();
            _extent.AttachReporter(spark);
            _extent.AddSystemInfo("Project", "Q-PROS API Testing");
            _extent.AddSystemInfo("Environment", "QA");

            return _extent;
        }

        public static string GetProjectRootDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path)!;
        }

        public static void FlushExtentReports()
        {
            _extent?.Flush();
        }
    }
}