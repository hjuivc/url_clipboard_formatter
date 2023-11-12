using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using url_clipboard_formatter.Services;

namespace url_clipboard_formatter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string copiedUrl = string.Empty;
        private bool isUrlCopied = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ClipboardMonitor.Start();
            ClipboardMonitor.ClipboardUpdate += ClipboardMonitor_ClipboardUpdate;
        }

        private void ClipboardMonitor_ClipboardUpdate(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                if (isUrlCopied)
                {
                    // Format the URL with the new text
                    string formattedUrl = $"[{clipboardText}]({copiedUrl})";
                    Clipboard.SetText(formattedUrl);
                    isUrlCopied = false;
                }
                else if (Uri.IsWellFormedUriString(clipboardText, UriKind.Absolute))
                {
                    copiedUrl = clipboardText;
                    isUrlCopied = true;
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ClipboardMonitor.Stop();
            base.OnExit(e);
        }
    }
}


