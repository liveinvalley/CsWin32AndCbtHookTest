using System.Globalization;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI;
using System.Diagnostics;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private Windows.Win32.UI.WindowsAndMessaging.HOOKPROC proc;
        Windows.Win32.UI.WindowsAndMessaging.HHOOK hook;

        private Windows.Win32.UI.Shell.SUBCLASSPROC subproc;

        public Form1()
        {
            // set WH_CBT hook
            proc = new Windows.Win32.UI.WindowsAndMessaging.HOOKPROC(this.CbtHookProc);

            uint currentThread = PInvoke.GetCurrentThreadId();
            hook = PInvoke.SetWindowsHookEx(
                Windows.Win32.UI.WindowsAndMessaging.WINDOWS_HOOK_ID.WH_CBT,
                proc,
                Windows.Win32.Foundation.HINSTANCE.Null,
                currentThread);

            // create proc;
            subproc = new Windows.Win32.UI.Shell.SUBCLASSPROC(this.SubclassProc);


            InitializeComponent();
        }

        public Windows.Win32.Foundation.LRESULT CbtHookProc(int code, Windows.Win32.Foundation.WPARAM wParam, Windows.Win32.Foundation.LPARAM lParam)
        {
            if (code == 3)
            {
                Debug.WriteLine("Window created. HWND = {0}", wParam);
                Windows.Win32.Foundation.BOOL r2 = PInvoke.SetWindowSubclass(new Windows.Win32.Foundation.HWND((nint)wParam.Value), subproc, 0, 0);

                Windows.Win32.Foundation.BOOL ret = PInvoke.UnhookWindowsHookEx(hook);
                if (ret == false)
                {
                    Debug.WriteLine("UnhookWindowsHookEx failed", wParam);
                }
            }
            return PInvoke.CallNextHookEx(hook, code, wParam, lParam);
        }

        public Windows.Win32.Foundation.LRESULT SubclassProc(Windows.Win32.Foundation.HWND hWnd, uint uMsg, Windows.Win32.Foundation.WPARAM wParam, Windows.Win32.Foundation.LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
        {
            return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.DefWndProc(ref m);
        }
    }


}
