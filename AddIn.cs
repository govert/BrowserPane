using System;
using ExcelDna.Integration;

namespace BrowserPane
{
    public class AddIn : IExcelAddIn
    {
        public void AutoOpen()
        {
            // I'm not sure whether this does anything
            // System.Windows.Forms.Application.EnableVisualStyles();

            // Failed workaround for keyboard / mouse focus issues in WebView2 control
            // SetEnvironmentVariable(L"COREWEBVIEW2_FORCED_HOSTING_MODE", L"COREWEBVIEW2_HOSTING_MODE_WINDOW_TO_VISUAL");
            // Environment.SetEnvironmentVariable("COREWEBVIEW2_FORCED_HOSTING_MODE", "COREWEBVIEW2_HOSTING_MODE_WINDOW_TO_VISUAL");
        }

        public void AutoClose()
        {
        }

    }
}
