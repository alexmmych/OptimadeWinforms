using System;
using CefSharp;
using CefSharp.WinForms;
using System.IO;
using System.Reflection;

namespace Optimade
{
    public class CefSharp
    {
        public ChromiumWebBrowser chromeBrowser;

        //Activates when Javascript posts a message to CefSharp.
        public event EventHandler<JavascriptMessageChangedArgs> JavascriptMessageChanged;

        //Retrieves the Javascript message.
        protected virtual void OnJavascriptMessage(JavascriptMessageChangedArgs e)
        {
            EventHandler<JavascriptMessageChangedArgs> handler = JavascriptMessageChanged;
            handler?.Invoke(this, e);
        }

        //Contains the types of messages Javascript can send to CefSharp.
        public enum Messages
        {
            Minimized,
            Maximized,
            Hidden,
            Close
        }

        public CefSharp()
        {
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component

            string executablePath = Assembly.GetEntryAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);
            string htmlSite = "\\Resources\\Website\\Website.html";

            string uriHTML = executableDirectory + htmlSite;

            chromeBrowser = new ChromiumWebBrowser(uriHTML);

            chromeBrowser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;
        }

        //Loads the message into the event argument.
        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            JavascriptMessageChangedArgs args = new JavascriptMessageChangedArgs();

            //Gets the message and transforms it to the 'Messages' enum.
            args.msg = (Messages)(int)e.Message;
            OnJavascriptMessage(args);
        }


    }

    //JavascriptMessageChanged arguments.
    public class JavascriptMessageChangedArgs : EventArgs
    {
        //Gets the message sent by Javascript.
        public CefSharp.Messages msg { get; set; }
    }
}