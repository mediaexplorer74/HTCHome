using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using HTCHome.Core;
using E = HTCHome.Core.Environment;

namespace NewsWidget
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private IntPtr _handle;
        private News widget;

        //private NumberFormatInfo f = new NumberFormatInfo();

        public Options(News w)
        {
            InitializeComponent();

            widget = w;
        }

        private void WindowSourceInitialized(object sender, EventArgs e)
        {
            _handle = new WindowInteropHelper(this).Handle;

            var margins = new WinAPI.MARGINS {cyTopHeight = 24};

            HwndSource.FromHwnd(_handle).CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            WinAPI.ExtendGlassFrame(_handle, ref margins);

            GeneralTab.Header = Widget.Instance.LocaleManager.GetString("General");
            AboutTab.Header = Widget.Instance.LocaleManager.GetString("About");

            NewsCountTextBlock.Text = Widget.Instance.LocaleManager.GetString("NewsCount");
            IntervalTextBlock.Text = Widget.Instance.LocaleManager.GetString("Interval");

            OkButton.Content = Widget.Instance.LocaleManager.GetString("OK");
            CancelButton.Content = Widget.Instance.LocaleManager.GetString("Cancel");
            ApplyButton.Content = Widget.Instance.LocaleManager.GetString("Apply");

            AddSourceTextBlock.Text = Widget.Instance.LocaleManager.GetString("AddSource");

            Image1.Source = new BitmapImage(new Uri(E.Path + "\\News\\Resources\\icon.png"));

            System.Reflection.Assembly _AsmObj = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName _CurrAsmName = _AsmObj.GetName();
            string _Major = _CurrAsmName.Version.Major.ToString();
            string _Minor = _CurrAsmName.Version.Minor.ToString();
            string _Build = _CurrAsmName.Version.Build.ToString();
            VersionString.Text = string.Format("Version {0}.{1} Build {2}", _Major, _Minor, _Build);

            IntervalComboBox.Text = Properties.Settings.Default.Interval.ToString();
            NewsCountComboBox.Text = Properties.Settings.Default.NewsCount.ToString();

            if (Properties.Settings.Default.Sources != null)
            {
                foreach (string s in Properties.Settings.Default.Sources)
                {
                    var item = new SourceItem();
                    item.Header = s;
                    item.ItemRemoved += ItemItemRemoved;
                    SourcesPanel.Children.Add(item);
                }
            }
            
            ApplyButton.IsEnabled = false;
        }

        void ItemItemRemoved(int index)
        {
            //widget.Sources.RemoveAt(index);
            widget.NewsTimeline.RemoveSource(Properties.Settings.Default.Sources[index]);
            Properties.Settings.Default.Sources.RemoveAt(index);
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            if (ApplyButton.IsEnabled)
            {
                ApplySettings();
            }

            this.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            ApplyButton.IsEnabled = false;
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void ApplySettings()
        {

            Properties.Settings.Default.Interval = Convert.ToInt32(IntervalComboBox.Text);
            Properties.Settings.Default.NewsCount = Convert.ToInt32(NewsCountComboBox.Text);

            //widget.UpdateSettings();
        }

        private void AddSourceBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(AddSourceBox.Text) && AddSourceBox.Text.StartsWith("http://"))
            {
                //var source = new Source {Url = AddSourceBox.Text};
                //source.GetNewsFinished += widget.GetNewsFinished;
                //source.RefreshFinished += widget.SourceRefreshFinished;
                //widget.Sources.Add(source);   
                widget.NewsTimeline.AddSource(AddSourceBox.Text);
                if (Properties.Settings.Default.Sources == null)
                    Properties.Settings.Default.Sources = new StringCollection();
                Properties.Settings.Default.Sources.Add(AddSourceBox.Text);

                var item = new SourceItem {Header = AddSourceBox.Text};
                item.ItemRemoved += ItemItemRemoved;
                SourcesPanel.Children.Add(item);

                AddSourceBox.Text = "";

            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AddSourceBox.Text) && AddSourceBox.Text.StartsWith("http://"))
            {
                //var source = new Source();
                //source.Url = AddSourceBox.Text;
                //source.GetNewsFinished += widget.GetNewsFinished;
                //source.RefreshFinished += widget.SourceRefreshFinished;
                //widget.Sources.Add(source);
                widget.NewsTimeline.AddSource(AddSourceBox.Text);
                if (Properties.Settings.Default.Sources == null)
                    Properties.Settings.Default.Sources = new StringCollection();
                Properties.Settings.Default.Sources.Add(AddSourceBox.Text);

                SourceItem item = new SourceItem();
                item.Header = AddSourceBox.Text;
                item.ItemRemoved += new SourceItem.ItemRemovedHandler(ItemItemRemoved);
                SourcesPanel.Children.Add(item);
                AddSourceBox.Text = "";

                //source.GetNews();
            }
        }

        private void NewsCountComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }


        private void ContactStringMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", ContactString.Text, string.Empty, string.Empty, 0);
        }
    }
}
