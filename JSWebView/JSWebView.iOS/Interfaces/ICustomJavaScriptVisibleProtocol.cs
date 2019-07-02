using Foundation;
using JavaScriptCore;

namespace JSWebView.iOS.Interfaces
{
    [Protocol()]
    interface ICustomJavaScriptVisibleProtocol : IJSExport
    {
        [Export("notify:")]
        void notify(string result);
    }
}