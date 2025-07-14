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

## TODO

* This sample does not implement task pane management for multiple workbooks.
* It would be nice to add some example of interaction between Excel and the browser.
* Clean up the initialization and revisit https://github.com/MicrosoftEdge/WebView2Feedback/issues/187
