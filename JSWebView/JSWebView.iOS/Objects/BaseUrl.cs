using Xamarin.Forms;
using JSWebView.iOS.Obects;
using JSWebView.Interfaces;
using Foundation;

[assembly: Dependency(typeof(BaseUrl))]
namespace JSWebView.iOS.Obects
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath + "/www/";
        }
    }
}