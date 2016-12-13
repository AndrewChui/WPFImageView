using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Windows;

namespace ImageViewer
{
    public enum ShowMode
    {
        Thumbnail,
        SingleWindow
    }
    public class Images : INotifyPropertyChanged
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
        private int thumbnailIndex = 0; //the index in List bitmapImages of which image has been shown in the single windows
        private FileSystemWatcher fileWatcher;
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
        public ObservableCollection<BitmapImage> BitmapImages
        {
            get { return bitmapImages; }
            set
            {
                bitmapImages = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BitmapImages"));
            }
        }
        public string ImagePath { get; set; }    //path of images' folder
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
            fileWatcher = new FileSystemWatcher(imagePath, "*.jpg");
            fileWatcher.Created += new FileSystemEventHandler(OnCreate);
            //fileWatcher.Deleted += new FileSystemEventHandler(OnDelete);
            //fileWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
            | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            fileWatcher.IncludeSubdirectories = true;
            endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
            this.showMode = showMode;
        }
        //Watch Files changed
        private void OnCreate(object sender,FileSystemEventArgs e)
        {
            fileNames.Add(e.FullPath);
            if(endIndex-beginIndex<13)
            {
                endIndex++;
                if (showMode == ShowMode.Thumbnail)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.DecodePixelHeight = 300;
                        img.UriSource = new Uri(fileNames[fileNames.Count - 1]);
                        img.EndInit();
                        bitmapImages.Add(img);
                        LoadThumbnail();
                    }));

                }
            }
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
        public void GetImagesFileNames()
        {
            var files = Directory.GetFiles(ImagePath, "*.jpg", searchOption);
            fileNames = new List<string>(files);
        }

        public void LoadThumbnail()
        {
            bitmapImages.Clear();
            if(showMode==ShowMode.SingleWindow)
            {
                beginIndex = currentIndex - 6 > 0 ? currentIndex - 6 : 0;
                endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
                showMode = ShowMode.Thumbnail;
            }
            for (int i = beginIndex; i < endIndex; i++)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.DecodePixelHeight = 300;
                img.UriSource = new Uri(fileNames[i]);
                img.EndInit();
                bitmapImages.Add(img);
            }
            start = true;
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
            thumbnailIndex = index;
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
    }
}
