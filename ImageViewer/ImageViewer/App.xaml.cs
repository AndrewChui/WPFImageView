using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow win;
            if (e.Args.Length > 0)
            {
                win = new ImageViewer.MainWindow(e.Args[0]);
            }
            else
            {
                win = new ImageViewer.MainWindow();
            }
            win.Show();
        }
    }
}
