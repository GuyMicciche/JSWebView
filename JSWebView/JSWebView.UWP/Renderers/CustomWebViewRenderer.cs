using CustomJavaScriptRuntimeComponent;
using JSWebView.Controls;
using JSWebView.UWP.Renderers;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace JSWebView.UWP.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, Windows.UI.Xaml.Controls.WebView>, IWebViewDelegate
    {
        private readonly HtmlCommunicator communicator = new HtmlCommunicator();

        protected override async void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new Windows.UI.Xaml.Controls.WebView() { });
            }

            var oldWebView = e.OldElement as CustomWebView;
            if (oldWebView != null)
            {
                oldWebView.EvaluateJavascript = null;
            }

            var newWebView = e.NewElement as CustomWebView;
            if (newWebView != null)
            {
                newWebView.EvaluateJavascript = async (js) =>
                {
                    Debug.WriteLine("C# function sending this JavaScript => " + js);
                    return await Control.InvokeScriptAsync("eval", new[] { js });
                };
            }

            if (Control != null && e.NewElement != null)
            {
                SetupControl();
            }

            Element.Source?.Load(this);
        }

        private async void SetupControl()
        {
            Control.Settings.IsJavaScriptEnabled = true;

            Control.NavigationStarting += Control_NavigationStarting;
            Control.ScriptNotify += Control_ScriptNotify;
            Control.NavigationCompleted += Control_NavigationCompleted;
        }

        private async void Control_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
        {
            await sender.InvokeScriptAsync("eval", new string[] { "window.alert = function(message){ window.external.notify(message); };" });
        }

        private async void Control_ScriptNotify(object sender, Windows.UI.Xaml.Controls.NotifyEventArgs e)
        {
            string result = e.Value;

            Debug.WriteLine("JavaScript function calling C# function. The result is => " + result);

            if(!result.Contains("appx://"))
            {
                await new Windows.UI.Popups.MessageDialog(result).ShowAsync();
            }
        }

        private void Control_NavigationStarting(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationStartingEventArgs args)
        {
            sender.AddWebAllowedObject("CustomJavaScript", communicator);
        }

        const string LocalScheme = "ms-appx-web:///";

        // Script to insert a <base> tag into an HTML document
        const string BaseInsertionScript = @"
var head = document.getElementsByTagName('head')[0];
var bases = head.getElementsByTagName('base');
if(bases.length == 0){
    head.innerHTML = 'baseTag' + head.innerHTML;
}";
        public void LoadHtml(string html, string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = LocalScheme;
            }

            // Generate a base tag for the document
            var baseTag = $"<base href=\"{baseUrl}\"></base>";

            string htmlWithBaseTag;

            // Set up an internal WebView we can use to load and parse the original HTML string
            var internalWebView = new Windows.UI.Xaml.Controls.WebView();

            // When the 'navigation' to the original HTML string is done, we can modify it to include our <base> tag
            internalWebView.NavigationCompleted += async (sender, args) =>
            {
                // Generate a version of the <base> script with the correct <base> tag
                var script = BaseInsertionScript.Replace("baseTag", baseTag);

                // Run it and retrieve the updated HTML from our WebView
                await sender.InvokeScriptAsync("eval", new[] { script });
                htmlWithBaseTag = await sender.InvokeScriptAsync("eval", new[] { "document.documentElement.outerHTML;" });

                // Set the HTML for the 'real' WebView to the updated HTML
                Control.NavigateToString(!string.IsNullOrEmpty(htmlWithBaseTag) ? htmlWithBaseTag : html);
            };

            // Kick off the initial navigation
            internalWebView.NavigateToString(html);
        }

        public void LoadUrl(string url)
        {
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(LocalScheme + url, UriKind.RelativeOrAbsolute);
            }

            Control.Source = uri;
        }
    }
}