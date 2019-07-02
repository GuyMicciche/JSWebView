using Xamarin.Forms;
using JSWebView.Android.Obects;
using JSWebView.Interfaces;

[assembly: Dependency(typeof(BaseUrl))]
namespace JSWebView.Android.Obects
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/www/";
        }
    }
}