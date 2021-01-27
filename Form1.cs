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

}



