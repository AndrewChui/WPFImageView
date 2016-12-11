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
        SingleWindow,
        SingleFull
    }
    public class Images : INotifyPropertyChanged
    {
        #region private var
        private List<string> fileNames;         //All files' path will be shown
        private List<BitmapImage> bitmapImages = new List<BitmapImage>(13);         //every page will be shown 13 images
        private Bitmap image;
        private int beginIndex;       //The first image's index was shown in current page
        private int endIndex;          //endIndex-beginIndex=13
        private SearchOption searchOption = SearchOption.TopDirectoryOnly;
        private ShowMode showMode = ShowMode.Thumbnail;
        private bool start = false;
        private int currentIndex;
        #endregion private var

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        public int ThumbnailIndex { get; set; }
        public Bitmap Image
        {
            get { return image; }
            set { image = value; }
        }
        public ShowMode ShowMode
        {
            get { return showMode; }
            set { showMode = value; }
        }
        public List<BitmapImage> BitmapImages
        {
            get { return bitmapImages; }
            set
            {
                bitmapImages = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("BitmapImages"));
            }
        }
        public string ImagePath { get; set; }
        public Images(string imagePath)
        {
            ImagePath = imagePath;
            GetImagesFileNames();
            beginIndex = 0;
            endIndex = beginIndex + 13 <= fileNames.Count ? beginIndex + 13 : fileNames.Count;
        }
        public Images()
        {
            ImagePath = null;
            beginIndex = endIndex = currentIndex = 0;
        }
        public void GetImagesFileNames()
        {
            var files = Directory.GetFiles(ImagePath, "*.jpg", searchOption);
            fileNames = new List<string>(files);
        }

       public void LoadThumbnail()
        {
            bitmapImages.Clear();
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
            if (endIndex+13<=fileNames.Count)
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
            if (beginIndex-13>=0)
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
        public void InitSingleImage()
        {
            currentIndex = beginIndex + ThumbnailIndex;
            //var uri = bitmapImages[ThumbnailIndex].UriSource;
            Image = new Bitmap(fileNames[currentIndex]);
        }
    }
}
