using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.Enums;
using CefSharp.WinForms;

namespace Optimade
{
    public class CefSharp
    {
        public ChromiumWebBrowser chromeBrowser;

        public event EventHandler<JavascriptMessageChangedArgs> JavascriptMessageChanged;

        protected virtual void OnJavascriptMessage(JavascriptMessageChangedArgs e)
        {
            EventHandler<JavascriptMessageChangedArgs> handler = JavascriptMessageChanged;
            handler?.Invoke(this, e);
        }

        public CefSharp()
        {
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser(@"file:///C:/C%23/Optimade/Resources/Website/Website.html");

            chromeBrowser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;
        }

        public enum Messages
        {
            Minimized,
            Maximized,
            Hidden
        }

        private Messages JavaScriptMessage;

        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            JavaScriptMessage = (Messages)(int)e.Message;

            JavascriptMessageChangedArgs args = new JavascriptMessageChangedArgs();
            args.msg = JavaScriptMessage;

            OnJavascriptMessage(args);
        }


    }
    public class JavascriptMessageChangedArgs : EventArgs
    {
        public CefSharp.Messages msg { get; set; }
    }
}
