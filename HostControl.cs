using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing; // For Point
using Microsoft.Web.WebView2.Core;

namespace BrowserPane
{
    public interface ITaskPaneControl { }

    [ComVisible(true)]
    [ComDefaultInterface(typeof(ITaskPaneControl))] // being explicit is required, reflection based IDispatch does not work
    public partial class HostControl : UserControl, ITaskPaneControl
    {
        public HostControl()
        {
            InitializeComponent();
            InitializeComponentAsync();
        }

        // When the WebView2 control is embedded in a WinForms UserControl, the focus management is not always handled correctly by the WebView2 control
        // However, mouse click events that occur within the bounds of the WebView2 control can be captured by overriding the WndProc method of the UserControl
        // We do this be intercepting the WM_PARENTNOTIFY message that is sent when a mouse button is pressed on a child control of the UserControl
        // Adding an explicit WebView2.Focus() call at that moment sets up the right state in the hosting UserControl for future focus routing to work right
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m); // Call base first to ensure we handle other messages correctly

            // TODO: Fix up this assignment to point to the embedded WebView2 control in your UserControl.
            var webViewControl = webView;

            const int WM_PARENTNOTIFY = 0x0210;
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_MBUTTONDOWN = 0x0207;

            if (m.Msg == WM_PARENTNOTIFY)
            {
                // Extract the event and coordinates
                int childEvent = m.WParam.ToInt32() & 0xFFFF; // LOWORD - the event like WM_LBUTTONDOWN
                if (childEvent == WM_LBUTTONDOWN ||
                    childEvent == WM_RBUTTONDOWN ||
                    childEvent == WM_MBUTTONDOWN)
                {
                    int lParamInt = m.LParam.ToInt32();
                    int xCoord = (short)(lParamInt & 0xFFFF);          // X from LOWORD(lParam)
                    int yCoord = (short)((lParamInt >> 16) & 0xFFFF);  // Y from HIWORD(lParam)
                    Point clickPointInUserControlClient = new Point(xCoord, yCoord);

                    // TODO: Consider DPI if needed

                    // Check if this click point is within the bounds of our webView control
                    if (webViewControl != null && webViewControl.Bounds.Contains(clickPointInUserControlClient))
                    {
                        Debug.WriteLine($"UserControl.WndProc: WM_PARENTNOTIFY (WM_LBUTTONDOWN) received. Click at {clickPointInUserControlClient} is within WebView2's bounds ({webViewControl.Bounds}).");

                        // The click originated within the area occupied by your WebView2 control
                        // Here we add the extra message to set up future focus routing
                        if (webViewControl.CanFocus)
                        {
                            // Set focus to the WebView2 control
                            var focusOK = webViewControl.Focus();
                            if (!focusOK)
                            {
                                Debug.WriteLine("UserControl.WndProc: WebView2 control focus failed. Unexpected after CanFocus == true.");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("UserControl.WndProc: WebView2 control cannot be focused at this time.");
                        }
                    }
                    else
                    {
                        // It might mean the click was on another child control of the UserControl,
                        // or the coordinate interpretation needs adjustment.
                        Debug.WriteLine($"UserControl.WndProc: WM_PARENTNOTIFY (WM_LBUTTONDOWN) at {clickPointInUserControlClient}. Not within WebView2's bounds ({webViewControl?.Bounds}).");
                    }
                }
                else
                {
                    Debug.WriteLine($"UserControl.WndProc: Other WM_PARENTNOTIFY event received: {childEvent} at {m.LParam.ToInt32()}");
                }
            }
            base.WndProc(ref m); // Important to call base.WndProc for other messages
        }

        async void InitializeComponentAsync()
        {
            // Failed workaround for keyboard /mouse focus issues in WebView2 control
            //// msWebView2BrowserHitTransparent allows single-click focus back to the sheet
            //// Makes the mouse click on the webview show the cursor on the webview, but not transfer the keyboard focus
            //// Click in textbox allow click in webview 
            //// Tab etc. does not work inside webview
            //// CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--enable-features=msWebView2BrowserHitTransparent");
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions();

            // TODO: Not sure how to safely wait for this to complete - maybe set a flag?
            var env = await CoreWebView2Environment.CreateAsync(null, Path.GetTempPath(), options: options);
            // var env = await CoreWebView2Environment.CreateAsync(null, Path.GetTempPath());
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            await webView.EnsureCoreWebView2Async(env);
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(textBoxUri.Text);
            }
        }

        private void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            textBoxUri.Text = webView.Source.ToString();
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // Now 'this.webView.CoreWebView2' is not null.
                CoreWebView2 coreWebView2 = webView.CoreWebView2;

                // You can navigate here if you haven't already by setting the Source property
                // or if you called EnsureCoreWebView2Async without setting Source in InitializeWebViewAsync.
                // For example:
                webView.CoreWebView2.Navigate("https://www.bing.com");
            }
            else
            {
                // Handle initialization failure
                MessageBox.Show($"WebView2 CoreWebView2InitializationCompleted failed: {e.InitializationException.ToString()}");
            }
        }
    }
}
