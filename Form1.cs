using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.WinForms;

namespace Optimade
{
    public partial class Optimade : Form
    {
        public Optimade()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            CefSharp cef = new CefSharp();

            // Add it to the form and fill it to the form window.
            this.Controls.Add(CefSharp.chromeBrowser);
            CefSharp.chromeBrowser.Dock = DockStyle.Fill;
        }


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        static extern IntPtr CreateRoundRectRgn
            (int nLeftRect,     // x-coordinate of upper-left corner.
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
            );

        protected override void OnActivated(EventArgs e)
        {
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

    }

    public class CefSharp
    {
        public static ChromiumWebBrowser chromeBrowser;

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

            CefSharp.chromeBrowser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;
            CefSharp.chromeBrowser.FrameLoadEnd += OnFrameLoadEnd;
        }

        private void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                //In the main frame we inject some javascript that's run on mouseUp
                //You can hook any javascript event you like.
                CefSharp.chromeBrowser.ExecuteScriptAsync(@"
                document.getElementById('caption').onmousedown = function () {
                    CefSharp.PostMessage('drag');
                }
                document.getElementById('caption').onmouseup = function () {
                    CefSharp.PostMessage('stop');
                }");
            }
        }
        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            if (e.Message.ToString() == "drag")
            {
                Console.WriteLine("Dragging");
            }

            if (e.Message.ToString() == "stop")
            {
                Console.WriteLine("Stop");
            }
        }
    }
}



