using Foundation;
using JSWebView.iOS.Interfaces;
using System.Diagnostics;

namespace JSWebView.iOS.Objects
{
    class CustomJavaScriptExporter : NSObject, ICustomJavaScriptVisibleProtocol
    {
        public void notify(string result)
        {
            Debug.WriteLine("JavaScript function calling C# function. The result is => " + result);
        }
    }
}