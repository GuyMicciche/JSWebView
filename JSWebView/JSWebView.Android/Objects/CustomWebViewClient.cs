using Android.OS;
using Android.Webkit;
using System;

namespace JSWebView.Android.Objects
{
    public class CustomWebViewClient : WebViewClient
    {
        public CustomWebViewClient() { }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            //TODO

            return base.ShouldOverrideUrlLoading(view, request);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            EvalJS(view, "alert('Called from OnPageFinished()');");
        }

        public void EvalJS(WebView webview, string js)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                webview.EvaluateJavascript(js, null);
            }
            else
            {
                webview.LoadUrl("javascript:" + js);
            }
        }
    }
}