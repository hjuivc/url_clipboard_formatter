// ClipboardMonitor.cs in the Services folder
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace url_clipboard_formatter.Services
{
    public static class ClipboardMonitor
    {
        // Define the delegate for the Win32 API window message handler
        private delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        // Import user32.dll functions that will be used to interact with the clipboard
        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll")]
        private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        // Define the Clipboard Update event
        public static event EventHandler ClipboardUpdate;

        // Handle to the next clipboard viewer window
        private static IntPtr _nextClipboardViewer;

        // Handle to our window that will receive clipboard messages
        private static HwndSource _hwndSource;

        // Message ID for the clipboard update message
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        // Method to start monitoring the clipboard
        public static void Start()
        {
            // Create a new window (invisible) to receive the clipboard messages
            var wndHelper = new WindowInteropHelper(new Window());
            var hWnd = wndHelper.EnsureHandle();

            // Set this window to be the next clipboard viewer
            _nextClipboardViewer = SetClipboardViewer(hWnd);

            // Create an HwndSource to use as our message pump
            _hwndSource = HwndSource.FromHwnd(hWnd);
            _hwndSource.AddHook(WndProc);
        }

        // Method to stop monitoring the clipboard
        public static void Stop()
        {
            // Remove our window from the clipboard viewer chain
            ChangeClipboardChain(_hwndSource.Handle, _nextClipboardViewer);

            // Remove the message hook
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }

        // This is our window procedure that will listen for clipboard update messages
        private static IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                OnClipboardUpdate();
            }
            else
            {
                // Pass the message on to the next clipboard viewer
                SendMessage(_nextClipboardViewer, msg, wParam, lParam);
            }

            return IntPtr.Zero;
        }

        // Method to raise the ClipboardUpdate event
        private static void OnClipboardUpdate()
        {
            ClipboardUpdate?.Invoke(null, EventArgs.Empty);
        }
    }
}
