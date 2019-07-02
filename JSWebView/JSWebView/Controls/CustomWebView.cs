using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace JSWebView.Controls
{
    public class CustomWebView : Xamarin.Forms.WebView
    {
        public static BindableProperty EvaluateJavascriptProperty = BindableProperty.Create(nameof(EvaluateJavascript), typeof(Func<string, Task<string>>), typeof(CustomWebView), null, BindingMode.OneWayToSource);
        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return (Func<string, Task<string>>)GetValue(EvaluateJavascriptProperty); }
            set { SetValue(EvaluateJavascriptProperty, value); }
        }
    }
}
