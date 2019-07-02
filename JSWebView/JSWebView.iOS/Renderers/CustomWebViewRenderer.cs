using Foundation;
using JavaScriptCore;
using JSWebView.Controls;
using JSWebView.iOS.Objects;
using JSWebView.iOS.Renderers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRender))]
namespace JSWebView.iOS.Renderers
{
    public class CustomWebViewRender : WebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var webView = e.NewElement as CustomWebView;
            if (webView != null)
            {
                Delegate = new CustomWebViewDelegate(ViewController, webView);

                webView.EvaluateJavascript = (js) =>
                {
                    Debug.WriteLine("C# function sending this JavaScript => " + js);
                    return Task.FromResult(EvaluateJavascript(js));
                };
            }

            if (NativeView != null && e.NewElement != null)
            {
                SetupControl();
            }
        }

        private void SetupControl()
        {
            var control = ((UIWebView)NativeView);
            control.ScalesPageToFit = true;

            var context = (JSContext)control.ValueForKeyPath((NSString)"documentView.webView.mainFrame.javaScriptContext");
            context.ExceptionHandler = (JSContext context2, JSValue exception) =>
            {
                Console.WriteLine("JS exception: {0}", exception);
            };

            context[(NSString)"CustomJavaScript"] = JSValue.From(new CustomJavaScriptExporter(), context);
        }
    }
}