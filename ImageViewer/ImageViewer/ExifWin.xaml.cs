using System.Windows;
using System.Windows.Input;

namespace ImageViewer
{
    /// <summary>
    /// ExifWin.xaml 的交互逻辑
    /// </summary>
    public partial class ExifWin : Window
    {
        public ExifWin()
        {
            InitializeComponent();
        }
        public string ExifInfo { get; set; }
        public void SetLabe()
        {
            exifLabel.Content = ExifInfo;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
