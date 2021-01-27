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

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        static extern IntPtr CreateRoundRectRgn
            (int nLeftRect,     // x-coordinate of upper-left corner.
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
            );

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

        public Messages JavaScriptMessage;

        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            JavaScriptMessage = (Messages)(int)e.Message;

            switch (JavaScriptMessage)
            {
                case Messages.Maximized:
                    {
                        Console.WriteLine("Maximized");
                        break;
                    }
                case Messages.Minimized:
                    {
                        Console.WriteLine("Minimized");
                        break;
                    }
                case Messages.Hidden:
                    {
                        Console.WriteLine("Hidden");
                        break;
                    }

            }
        }


    }
}
