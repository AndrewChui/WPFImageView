using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
