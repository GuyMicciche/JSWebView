using Xamarin.Forms;
using JSWebView.UWP.Obects;
using JSWebView.Interfaces;

[assembly: Dependency(typeof(BaseUrl))]
namespace JSWebView.UWP.Obects
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "ms-appx-web:///Assets/www/";
        }
    }
}