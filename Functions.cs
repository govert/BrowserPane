using ExcelDna.Integration;

namespace BrowserPane
{
    public class Functions
    {
        public static object SayHello() => $"Hello from {ExcelDnaUtil.XllPath}";

    }
}