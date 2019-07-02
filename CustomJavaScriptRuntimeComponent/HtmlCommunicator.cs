using System.Diagnostics;
using Windows.Foundation.Metadata;

namespace CustomJavaScriptRuntimeComponent
{
    [AllowForWeb]
    public sealed class HtmlCommunicator
    {
        public void notify(string result)
        {
            Debug.WriteLine("JavaScript function calling C# function. The result is => " + result);
        }
    }
}
