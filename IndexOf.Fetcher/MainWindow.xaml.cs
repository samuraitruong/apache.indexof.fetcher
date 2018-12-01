using MahApps.Metro.Controls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace IndexOf.Fetcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow , INotifyPropertyChanged
    {
        Config config = new Config();
        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        public MainWindow()
        {
            InitializeComponent();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private int _totalLinks = 0;
        public int TotalLinks
        {
            get { return _totalLinks; }
            set
            {
                _totalLinks = value;
                this.NotifyPropertyChange("TotalLinks");
            }
        }

        private double _downloadedKb = 0;
        public double DownloadedKb
        {
            get { return _downloadedKb; }
            set
            {
                _downloadedKb = value;
                this.NotifyPropertyChange("DownloadedKb");
            }
        }
        public double MediaLinks => this.links.Count;
        
        private float _downloadProgress=1;
        public float DownloadProgress { get { return _downloadProgress; } set {
                _downloadProgress = value;
                this.NotifyPropertyChange("DownloadProgress");
            } }
        public int QueueItems => queue.Count;
        private string _url;
        private async Task DownloadSinglePage(string url)
        {
            this.Invoke(() =>
            {
                lblStatus.Content = $"Fetching {url}";
            });
            await Task.Run(() =>
            {
                DownloadProgress = 0;
                using (var client = new WebClient())
                {
                    client.Proxy = null;
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadStringCompleted += Client_DownloadStringCompleted;
                    client.DownloadStringAsync(new Uri(url), url);

                }
            }); }

        private async Task ProcessSinglePage(string html, string root)
        {
            var matches = Regex.Matches(html, "href=\\\"([^\"]*)");
            this.TotalLinks += matches.Count;
            foreach (Match match in matches)
            {
                var url = match.Groups[1].Value;
                if (url.Contains("..")) continue;
                if (url.EndsWith("/"))
                {
                    queue.Enqueue(root + url);
                    continue;
                }
                if (config.AllowExtensions.Any(x => url.ToLower().EndsWith(x)))
                    links.Add(root + match.Groups[1].Value);
            }
            var text = String.Join("\r\n", links);

            this.Invoke(() =>
            {
                Clipboard.SetText(text);

                textEditor.Text = text;
                lblStatus.Content = $"Copied {links.Count} links to clipboard";
            });
            this.NotifyPropertyChange("MediaLinks");

            this.NotifyPropertyChange("QueueItems");
            string newUrl;
            if(queue.TryDequeue(out newUrl))
            {
                await DownloadSinglePage(newUrl);
            } else
            {
                this.Invoke(() =>
                {
                    btnFetch.IsEnabled = true;
                });
            }
        }
        private void btnFetch_Click(object sender, RoutedEventArgs e)
        {
            this.TotalLinks = 0;
            this.links = new List<string>();
            this.btnFetch.IsEnabled = false;
            textEditor.Text = "";
            this.lblStatus.Content = "Fetching ......";
            this._url = txtUrl.Text.Trim();
            DownloadProgress = 0;
            DownloadedKb = 0;
            var url = txtUrl.Text.Trim();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.DownloadSinglePage(url);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        List<string> links = new List<string>();

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            this.ProcessSinglePage(e.Result as string, e.UserState.ToString());
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Debug.WriteLine(e.BytesReceived.ToString() + " ....percent= " +  e.ProgressPercentage + "    ===== " + e.TotalBytesToReceive);
            this.DownloadProgress = e.BytesReceived * 100 / e.TotalBytesToReceive;
            Debug.WriteLine("actual status" + (e.BytesReceived * 100 / e.TotalBytesToReceive));
            this.DownloadedKb += (double)e.BytesReceived / 1000;
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            //this.DataContext = this;
        }

        private void btnFetch_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }
    }
}
