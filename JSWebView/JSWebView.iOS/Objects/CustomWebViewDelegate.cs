using Foundation;
using JSWebView.Controls;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using UIKit;

namespace JSWebView.iOS
{
    public class CustomWebViewDelegate : UIWebViewDelegate
    {
        CustomWebView customWebView;
        UIViewController controller;

        public CustomWebViewDelegate(UIViewController controller, CustomWebView customWebView)
        {
            this.customWebView = customWebView;
            this.controller = controller;
        }

        public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            string result = WebUtility.UrlDecode(request.ToString());

            if (result.StartsWith("myapp://"))
            {
                Debug.WriteLine("JavaScript function calling C# function. The result is => " + result);

                return false;
            }

            return true;
        }

        public override async void LoadingFinished(UIWebView webView)
        {
            // Sometimes doesn't work, so delay 500ms before starting
            await Task.Delay(500);

            EvalJS(webView, "alert('Called from OnPageFinished()');");
        }

        private void EvalJS(UIWebView webView, string js)
        {
            webView.EvaluateJavascript(new NSString(js));
        }
    }
}
