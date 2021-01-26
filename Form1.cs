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
using CefSharp.Enums;
using CefSharp.WinForms;

namespace Optimade
{
    public partial class Optimade : Form
    {
        // Messages
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_MOUSELEAVE = 0x02A3;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;

        public Optimade()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            CefSharp cef = new CefSharp();

            CefSharp.chromeBrowser.DragHandler = new DragDropHandler();

            CefSharp.chromeBrowser.IsBrowserInitializedChanged += ChromeBrowser_IsBrowserInitializedChanged;
            CefSharp.chromeBrowser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;

            // Add it to the form and fill it to the form window.
            this.Controls.Add(CefSharp.chromeBrowser);
            CefSharp.chromeBrowser.Dock = DockStyle.Fill;
        }

        private void ChromeBrowser_IsBrowserInitializedChanged(object sender, EventArgs args)
        {


            ChromeWidgetMessageInterceptor.SetupLoop(CefSharp.chromeBrowser, (message) =>
            {
                if (message.Msg == WM_LBUTTONDOWN)
                {
                    Point point = new Point(message.LParam.ToInt32());

                    if (((DragDropHandler)CefSharp.chromeBrowser.DragHandler).draggableRegion.IsVisible(point))
                    {
                        ReleaseCapture();
                        SendHandleMessage();
                    }
                }
            });

        }
        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            if (e.Message.ToString() == "drag")
            {
                Console.WriteLine("Clicked");
            }

            if (e.Message.ToString() == "move")
            { 
                Console.WriteLine("Dragging");
            }

            if (e.Message.ToString() == "stop")
            {
                Console.WriteLine("Stop");
            }
             
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Console.WriteLine("Mouse moved");
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

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public void SendHandleMessage()
        {
            if (InvokeRequired) { Invoke(new SendHandleMessageDelegate(SendHandleMessage), new object[] { }); return; }

            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        public delegate void SendHandleMessageDelegate();
    
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

            CefSharp.chromeBrowser.FrameLoadEnd += OnFrameLoadEnd;
        }

        private void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                //In the main frame we inject some javascript that's run on mouseUp
                //You can hook any javascript event you like.
                CefSharp.chromeBrowser.ExecuteScriptAsync(@"
                var dragging = false;
                var caption = document.getElementById('caption');
                
                function stopDrag()
                {
                    if (dragging == true) {
                        dragging = false;
                        CefSharp.PostMessage('stop');
                    }
                }

                caption.onmousedown = function () {
                    dragging = true;
                    CefSharp.PostMessage('drag');

                    caption.addEventListener('mouseleave',stopDrag)
                }

                caption.onmousemove = function () {
                        if (dragging == true) {
                            CefSharp.PostMessage('move');
                        }
                }
                caption.addEventListener('mouseup',stopDrag)");
            }
        }

             
    }
    

}



