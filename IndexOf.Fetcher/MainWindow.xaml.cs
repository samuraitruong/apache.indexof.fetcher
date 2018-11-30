using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace IndexOf.Fetcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFetch_Click(object sender, RoutedEventArgs e)
        {
            
            txtResults.Document.Blocks.Clear();
            List<string> links = new List<string>();
            using (var client = new WebClient())
            {
                var html = client.DownloadString(txtUrl.Text.Trim());
                var matches = Regex.Matches(html, "href=\\\"([^\"]*)");
                foreach (Match match in matches)
                {
                    var url = match.Groups[1].Value;
                    if (url.Contains("..")) continue;
                    links.Add(txtUrl.Text.Trim() + match.Groups[1].Value);
                }
                var text = String.Join("\r\n", links);
                Clipboard.SetText(text);

                txtResults.AppendText(text);
                lblStatus.Content = $"Copied {links.Count} links to clipboard";
            }
        }
    }
}
