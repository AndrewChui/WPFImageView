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
        private List<BitmapImage> bitmapImages = new List<BitmapImage>(13);         //every page will be shown 13 images
        private Bitmap image;         //Single mode, this image will be shown
        private int beginIndex;       //The first image's index was shown in current page
        private int endIndex;          //endIndex-beginIndex=13
        private SearchOption searchOption = SearchOption.TopDirectoryOnly;
        private ShowMode showMode;
        private bool start = false;    //images have been loaded when start=true 
        private int currentIndex;     // the index in List filenames of which image has been shown in the single windows
        private int thumbnailIndex = 0; //the index in List bitmapImages of which image has been shown in the single windows
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
        public List<BitmapImage> BitmapImages
        {
            get { return bitmapImages; }
            set
            {
                bitmapImages = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("BitmapImages"));
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
            endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
            this.showMode = showMode;
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
            OnPropertyChanged(new PropertyChangedEventArgs("BitmapImages"));
            start = true;
        }
        public void LoadNextThumbnail()
        {
            if (!start)
                return;
            if (endIndex + 13 <= fileNames.Count)
            {
                endIndex += 13;
                beginIndex += 13;
            }
            else
            {
                endIndex = fileNames.Count;
                beginIndex = endIndex - 13;
            }
            LoadThumbnail();
        }
        public void LoadPreThumbnail()
        {
            if (!start)
                return;
            if (beginIndex - 13 >= 0)
            {
                beginIndex -= 13;
                endIndex -= 13;
            }
            else
            {
                beginIndex = 0;
                endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
            }
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
                image.Dispose();
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
