using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
namespace ImageViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private Images images;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(string fileName):this()
        {
            images = new Images(fileName, ShowMode.SingleWindow);
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                openCanvs.Visibility = Visibility.Visible;
        }
        private void image01_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = Convert.ToInt32((sender as Image).Tag);
            var singleForm = new SingleForm(images, index);
            singleForm.Show();
         }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(images!=null && images.ShowMode==ShowMode.SingleWindow)
            {
                var singleForm = new SingleForm(images);
                singleForm.Show();
                images.LoadThumbnail();
                gridThumbnail.DataContext = images;
                openCanvs.Visibility = Visibility.Hidden;
            }
            var x = (openCanvs.ActualWidth - openButton.ActualWidth * 2) / 3;
            Canvas.SetLeft(openButton, x);
            var y = (openCanvs.ActualHeight - openButton.ActualHeight) / 2;
            Canvas.SetTop(openButton, y);
            Canvas.SetLeft(exitButton, x * 2 + openButton.ActualWidth);
            Canvas.SetTop(exitButton, y);
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            images.LoadPreThumbnail();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            images.LoadNextThumbnail();
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                images = new Images(fbd.SelectedPath, ShowMode.Thumbnail);
                images.LoadThumbnail();
                gridThumbnail.DataContext = images;
                openCanvs.Visibility = Visibility.Hidden;
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
            }
        }

        //protected的Dispose方法，保证不会被外部调用。
        //传入bool值disposing以确定是否释放托管资源
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                images.Dispose();
                disposed = true;
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }
        ~MainWindow()
        {
            Dispose(false);
        }
    }
}
