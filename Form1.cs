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

using Xilium.CefGlue;
using Xilium.CefGlue.WindowsForms;

namespace Optimade
{
    public partial class Optimade : Form
    {
        //Wndproc Messages
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_MOUSELEAVE = 0x02A3;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;

        //The initialized browser instance.
        public CefSharp Browser;

        public Optimade()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            Browser = new CefSharp();

            Browser.chromeBrowser.DragHandler = new DragDropHandler();

            // Add it to the form and fill it to the form window.
            this.Controls.Add(Browser.chromeBrowser);
            Browser.chromeBrowser.Dock = DockStyle.Fill;

            Browser.chromeBrowser.IsBrowserInitializedChanged += ChromeBrowser_IsBrowserInitializedChanged;
            Browser.JavascriptMessageChanged += OnJavascriptMessage;
        }

        //Intercepts Javascript messages and acts accordingly.
        private void OnJavascriptMessage(object sender, JavascriptMessageChangedArgs args)
        {
            Console.WriteLine(args.msg);

            switch (args.msg)
            {
                case CefSharp.Messages.Maximized:
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.WindowState = FormWindowState.Maximized;
                        }));
                        break;
                    }
                case CefSharp.Messages.Minimized:
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.WindowState = FormWindowState.Normal;
                        }));
                        break;
                    }
                case CefSharp.Messages.Hidden:
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.WindowState = FormWindowState.Minimized;
                        }));
                        break;
                    }
                case CefSharp.Messages.Close:
                    {
                        Application.Exit();
                        break;
                    }

            }
        }

        private void ChromeBrowser_IsBrowserInitializedChanged(object sender, EventArgs args)
        {

            ChromeWidgetMessageInterceptor.SetupLoop(Browser.chromeBrowser, (message) =>
            {
                if (message.Msg == WM_LBUTTONDOWN)
                {
                    Point point = new Point(message.LParam.ToInt32());

                    if (((DragDropHandler)Browser.chromeBrowser.DragHandler).draggableRegion.IsVisible(point))
                    {
                        ReleaseCapture();
                        SendHandleMessage();
                    }
                }
            });

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

        //Makes the window have it's elliptical shape.
        protected override void OnActivated(EventArgs e)
        {
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        //Updates the window and browser's shape and size.
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            //If statement is present here because OnSizeChanged triggers before CefSharp is initialized.
            if (Browser != null)
            {
                Browser.chromeBrowser.Size = this.Size;

                //Compensates for automatic resizing upon dragging the window when in fullscreen.
                if (this.WindowState == FormWindowState.Normal)
                {
                    Browser.chromeBrowser.ExecuteScriptAsyncWhenPageLoaded(@"
                    document.getElementById('size_img').src = 'MaximizeButton.png';
                    maximized = false;");
                }
            }

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

}



