// LocaleDownloadWindow (Updates Downloader)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace HTCHome
{
    /// <summary>
    /// Interaction logic for LocaleDownloadWindow.xaml
    /// </summary>
    public partial class LocaleDownloadWindow : Window
    {
        private string locale;
        private WebClient downloader;

        public LocaleDownloadWindow(string locale)
        {
            InitializeComponent();

            this.locale = locale;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            MessageText.Text = string.Format(MessageText.Text, CultureInfo.GetCultureInfo(locale).DisplayName);

            DownloadProgress.Visibility = System.Windows.Visibility.Visible;

            downloader = new WebClient();
            if (HTCHome.Properties.Settings.Default.UseProxy)
            {
                downloader.Proxy = HTCHome.Core.GeneralHelper.Proxy;
            }

            downloader.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloader_DownloadProgressChanged);
            downloader.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(downloader_DownloadFileCompleted);
            downloader.DownloadFileAsync(new Uri("http://store.htchome.org/localization/home2/" + locale + ".zip"), App.Path + "\\" + locale + ".zip");
        }

        void downloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Unpack(App.Path, App.Path + "\\" + locale + ".zip");
            this.Close();
        }

        void downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Value = e.ProgressPercentage;
        }


        public static void Unpack(string path, string file)
        {
            using (FileStream fileStreamIn = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (ZipInputStream zipInStream = new ZipInputStream(fileStreamIn))
                {
                    ZipEntry entry;
                    FileInfo info = new FileInfo(file);
                    while (true)
                    {
                        entry = zipInStream.GetNextEntry();
                        if (entry == null)
                            break;
                        if (!entry.IsDirectory)
                        {
                            if (File.Exists(path + "\\" + entry.Name))
                            {
                                File.Delete(path + "\\" + entry.Name);
                            }

                            using (FileStream fileStreamOut = new FileStream(string.Format(@"{0}\{1}", path, entry.Name), FileMode.Create, FileAccess.Write))
                            {
                                int size;
                                byte[] buffer = new byte[1024];
                                do
                                {
                                    size = zipInStream.Read(buffer, 0, buffer.Length);
                                    fileStreamOut.Write(buffer, 0, size);
                                } while (size > 0);
                                fileStreamOut.Close();
                            }
                        }
                        else
                            if (!Directory.Exists(string.Format(@"{0}\{1}", path, entry.Name)))
                                Directory.CreateDirectory(string.Format(@"{0}\{1}", path, entry.Name));
                    }
                    zipInStream.Close();
                }
                fileStreamIn.Close();
            }
        }
    }
}
