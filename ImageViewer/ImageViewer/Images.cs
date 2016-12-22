using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Threading;

namespace ImageViewer
{
    public enum ShowMode
    {
        Thumbnail,
        SingleWindow
    }
    public class Images : INotifyPropertyChanged, IDisposable
    {
        #region private var
        private List<string> fileNames;         //All files' path will be shown
        private ObservableCollection<BitmapImage> bitmapImages = new ObservableCollection<BitmapImage>();         //every page will be shown 13 images
        private Bitmap image;         //Single mode, this image will be shown
        private int beginIndex;       //The first image's index was shown in current page
        private int endIndex;          //endIndex-beginIndex=13
        private SearchOption searchOption = SearchOption.TopDirectoryOnly;
        private ShowMode showMode;
        private bool start = false;    //images have been loaded when start=true 
        private int currentIndex;     // the index in List filenames of which image has been shown in the single windows
        //private int thumbnailIndex = 0; //the index in List bitmapImages of which image has been shown in the single windows
        private FileSystemWatcher fileWatcher;
        private bool leftPage;
        private bool rightPage;
        #endregion private var

        /// <summary>
        /// Implementation INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        #region Property
        /// <summary>
        /// property binding to thumbnail windows
        /// </summary>
        public bool LeftPage
        {
            get { return leftPage; }
            set
            {
                leftPage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LeftPage"));
            }
        }

        public bool RightPage
        {
            get { return rightPage; }
            set
            {
                rightPage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RightPage"));
            }
        }
        public ObservableCollection<BitmapImage> BitmapImages
        {
            get { return bitmapImages; }
            set { bitmapImages = value; }
        }
        public string ImagePath { get; set; }    //path of images' folder
        public ShowMode ShowMode
        {
            get { return showMode; }
        }
        #endregion Property
        #region Construct method
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="imagePath">path of images' folder</param>
        public Images(string imagePath, ShowMode showMode)
        {
            if (showMode == ShowMode.Thumbnail)
            {
                ImagePath = imagePath;
                GetImagesFileNames();
                beginIndex = 0;
                currentIndex = 0;
                LeftPage = RightPage = false;
            }
            if (showMode == ShowMode.SingleWindow)
            {
                ImagePath = Path.GetDirectoryName(imagePath);
                GetImagesFileNames();
                for(int i=0;i<fileNames.Count;i++)
                {
                    if(imagePath==fileNames[i])
                    {
                        currentIndex = i;
                        break;
                    }
                }
                beginIndex = currentIndex - 6 > 0 ? currentIndex - 6 : 0;
            }
            //watch filesystem changed
            fileWatcher = new FileSystemWatcher(ImagePath, "*.jpg");
            fileWatcher.Created += new FileSystemEventHandler(OnCreate);
            //fileWatcher.Deleted += new FileSystemEventHandler(OnDelete);
            fileWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
            | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            fileWatcher.IncludeSubdirectories = true;
            endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
            this.showMode = showMode;
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            fileNames.Add(e.FullPath);
            endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
            LoadThumbnail();
        }

        /// <summary>
        /// Construct
        /// </summary>
        public Images()
        {
            ImagePath = null;
            beginIndex = endIndex = currentIndex = 0;
        }
        #endregion Method
        /// <summary>
        /// Watch File System, add new files in list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreate(object sender, FileSystemEventArgs e)
        {
            fileNames.Add(e.FullPath);
            endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
            LoadThumbnail();
        }
        public void GetImagesFileNames()
        {
            var files = Directory.GetFiles(ImagePath, "*.jpg", searchOption);
            fileNames = new List<string>(files);
        }

        public void LoadThumbnail()
        {
            bitmapImages.Clear();
            if (showMode == ShowMode.SingleWindow && currentIndex != beginIndex)
            {
                beginIndex = currentIndex;
                endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
                showMode = ShowMode.Thumbnail;
            }
            var t = new Thread(() =>
            {
                for (int i = beginIndex; i < endIndex; i++)
                {
                    try
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.DecodePixelHeight = 300;
                        img.UriSource = new Uri(fileNames[i]);
                        img.EndInit();
                        img.Freeze();
                        BitmapImages.Add(img);
                    }
                    catch { }
                  }
            });
            t.Start();
            start = true;
            LeftPage = beginIndex > 0 ? true : false;
            RightPage = endIndex < fileNames.Count ? true : false;
        }

        public void LoadNextThumbnail()
        {
            if (!start || endIndex==fileNames.Count)
                return;
            beginIndex = endIndex;
            endIndex = endIndex + 13 <= fileNames.Count ? endIndex + 13 : fileNames.Count;
            LoadThumbnail();
        }
        public void LoadPreThumbnail()
        {
            if (!start || beginIndex==0)
                return;
            beginIndex = beginIndex - 13 >= 0 ? beginIndex - 13 : 0;
            endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
            LoadThumbnail();
        }
        public Bitmap InitSingleImage(int index)
        {
            currentIndex = beginIndex +index;
            if(currentIndex>=0 && currentIndex<fileNames.Count)
            {
                image = new Bitmap(fileNames[currentIndex]);
            }
            else
            {
                image = null;
                currentIndex = 0;
            }
            showMode = ShowMode.SingleWindow;
            return image;
        }
        public Bitmap InitSingleImage()
        {
            image = new Bitmap(fileNames[currentIndex]);
            return image;
        }
        public Bitmap NextSingleImage()
        {
            if (currentIndex + 1 < fileNames.Count)
            {
                image.Dispose();
                image = new Bitmap(fileNames[++currentIndex]);
            }
            else
            {
                image = null;
            }
            return image;
        }
        public Bitmap PreSingleImage()
        {
            if (currentIndex > 0)
            {
                image?.Dispose();
                image = new Bitmap(fileNames[--currentIndex]);
            }
            else
            {
                image = null;
            }
            return image;
        }
        //资源清理
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
            if(!disposed)
            {
                fileWatcher.Dispose();
                image?.Dispose();
                disposed = true;
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
          }

        ~Images()
        {
            Dispose(false);
        }
      }
}
