using Android.Content;
using Android.OS;
using Android.Webkit;
using JavaScriptWebView.Android.Renderers;
using JSWebView.Android.Objects;
using JSWebView.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace JavaScriptWebView.Android.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, global::Android.Webkit.WebView>, IWebViewDelegate
    {
        Context context;

        public CustomWebViewRenderer(Context context) : base(context)
        {
            this.context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new global::Android.Webkit.WebView(context);
                SetNativeControl(webView);
            }

            var oldWebView = e.OldElement as CustomWebView;
            if (oldWebView != null)
            {
                oldWebView.EvaluateJavascript = null;
            }

            var newWebView = e.NewElement as CustomWebView;
            if (newWebView != null)
            {
                ManualResetEvent reset = new ManualResetEvent(false);
                var response = "";

                newWebView.EvaluateJavascript = async (js) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("C# function sending this JavaScript => " + js);
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                        {
                            Control?.EvaluateJavascript(js, new JavascriptCallback((r) => { response = r; reset.Set(); }));
                        }
                        else
                        {
                            Control?.LoadUrl("javascript:" + js);
                        }
                    });

                    await Task.Run(() => { reset.WaitOne(); });
                    if (response == "null")
                    {
                        response = string.Empty;
                    }

                    return response;
                };
            }

            if (Control != null && e.NewElement != null)
            {
                SetupControl();
            }

            Element.Source?.Load(this);
        }

        private void SetupControl()
        {
            Control.Settings.JavaScriptEnabled = true;
            Control.Settings.DomStorageEnabled = true;
            //Control.VerticalScrollBarEnabled = true;
            //Control.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
            //Control.Settings.CacheMode = CacheModes.NoCache;

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            //{
            //    Control.SetLayerType(global::Android.Views.LayerType.Hardware, null);
            //}
            //else
            //{
            //    Control.SetLayerType(global::Android.Views.LayerType.Software, null);
            //}

            Control.SetWebViewClient(new CustomWebViewClient());
            Control.SetWebChromeClient(new WebChromeClient());
            Control.AddJavascriptInterface(new CustomJavaScriptInterface(this), "CustomJavaScript");
        }

        public void LoadHtml(string html, string baseUrl)
        {
            Control.LoadDataWithBaseURL(baseUrl ?? "file:///android_asset/", html, "text/html", "UTF-8", null);
        }

        public void LoadUrl(string url)
        {
            Control.LoadUrl(url);
        }
    }


    internal class JavascriptCallback : Java.Lang.Object, IValueCallback
    {
        private Action<string> callback;

        public JavascriptCallback(Action<string> callback)
        {
            this.callback = callback;
        }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            callback?.Invoke(Convert.ToString(value));
        }
    }
}