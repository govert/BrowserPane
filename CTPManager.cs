using ExcelDna.Integration.CustomUI;

namespace BrowserPane
{
    internal static class CTPManager
    {
        // TODO: Since Excel 2013, a CTP is attached only to a single window (one workbook).
        //       So having a single variable here means you can only ever have one CTP in one of the Excel 2013 windows.
        //       Maybe have a map from workbook to CTP, or have a floating one or something...

        static CustomTaskPane ctp;

        public static void ShowCTP()
        {
            if (ctp == null)
            {
                // Make a new one using ExcelDna.Integration.CustomUI.CustomTaskPaneFactory 
                ctp = CustomTaskPaneFactory.CreateCustomTaskPane(typeof(HostControl), "Browser");
                ctp.Visible = true;
                ctp.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
            }
            else
            {
                // Just show it again
                ctp.Visible = true;
            }
        }


        public static void DeleteCTP()
        {
            if (ctp != null)
            {
                // Could hide instead, by calling ctp.Visible = false;
                ctp.Delete();
                ctp = null;
            }
        }
    }
}
