# BrowserPane Sample
------------------

This example shows how to use the WebView2 control as an embedded borwser in a Custom Task Pane.

![image](https://user-images.githubusercontent.com/414659/153727880-9f359a47-d78c-4ec6-b152-2ccf81e55dc8.png)

## Notes

Reading about the WebView2 control: 
https://docs.microsoft.com/en-us/microsoft-edge/webview2/get-started/winforms

There were some issues to take note of:

* Under .NET 6 there is an error when creating a user control for the Custom Task Pane - " Unable to create specified ActiveX control".
This is because classes do not by default implement IDispatch, which is required for a class to be considered an ActiveX control.
The fix is to add an explicit interface as "ComDefaultInterface" which is then exposed as IDispatch too.
The relevant issue I found on GitHub is here: Hosting UserControl via ActiveX does not work correctly · Issue #4370 · dotnet/winforms (github.com)

* There’s some issue with WebView2 and the temp path that it uses.
I had to add some explicit initialization:

```
	        async void InitializeComponentAsync()
	        {
	            // TODO: Not sure how to safely wait for this to complete - maybe set a flag?
	            var env = await CoreWebView2Environment.CreateAsync(null, Path.GetTempPath());
	            await webView.EnsureCoreWebView2Async(env);
	        }
```

* We implement a workaround for keyboard focus stealing - [WebView2 WinForms control in Excel VSTO Task pane steals and holds on to keyboard focus
](https://github.com/MicrosoftEdge/WebView2Feedback/issues/951)

## Related projects

* [WebView2.DOM](https://github.com/R2D221/WebView2.DOM) - C# DOM bindings to be used with WebView2
* [Westwind.WebView - ](https://github.com/RickStrahl/Westwind.WebView) - A .NET library to aid WebView2 control hosting, .NET/JavaScript interop and Html to Pdf Conversion

## TODO

* This sample does not implement task pane management for multiple workbooks. [This approach to sharing the environment](https://weblog.west-wind.com/posts/2023/Oct/31/Caching-your-WebView-Environment-to-manage-multiple-WebView2-Controls) might help if creating many CTPs 
* It would be nice to add some example of interaction between Excel and the browser.
* Clean up the initialization and revisit https://github.com/MicrosoftEdge/WebView2Feedback/issues/187
* Revisit initialization

```
        private static async Task<CoreWebView2Environment> WebView2EnvironmentAsync()
        {
            if (webView2Environment == null)
            {
                Environment.SetEnvironmentVariable("COREWEBVIEW2_FORCED_HOSTING_MODE", "COREWEBVIEW2_HOSTING_MODE_WINDOW_TO_VISUAL"); // https://github.com/MicrosoftEdge/WebView2Feedback/issues/951#issuecomment-1064624832
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string cacheFolder = Path.Combine(localAppData, "WindowsFormsWebView2");
                LogDisplay.RecordLine($"Available Browser Version String is {CoreWebView2Environment.GetAvailableBrowserVersionString()}, cache folder is {cacheFolder}");
                webView2Environment = await CoreWebView2Environment.CreateAsync(null, cacheFolder);
                LogDisplay.RecordLine($"CoreWebView2Environment is {webView2Environment}");
            }

            return webView2Environment;
        }
            WebView2 webView21;
            var environment = await WebView2EnvironmentAsync();
            await webView21.EnsureCoreWebView2Async(environment);

```

