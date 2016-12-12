using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
namespace ImageViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Images images;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void gridThumbnail_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta<-2)
            {
                images.LoadNextThumbnail();
            }
            if(e.Delta>2)
            {
                images.LoadPreThumbnail();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                images = new ImageViewer.Images(fbd.SelectedPath,ShowMode.Thumbnail);
                images.LoadThumbnail();
                gridThumbnail.DataContext = images;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                System.Windows.Application.Current.Shutdown();

        }
        private void image01_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = Convert.ToInt32((sender as Image).Tag);
            var singleForm = new SingleForm(images, index);
            singleForm.Show();
         }
    }
}
