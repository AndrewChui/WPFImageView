using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace ImageViewer
{
    enum ShowMode
    {
        Thumbnail,
        SingleWindow,
        SingleFull
    }
    public class Images
    {
        #region private var
        private List<string> fileNames;         //All files' path will be shown
        private List<BitmapImage> bitmapImages = new List<BitmapImage>(13);         //every page will be shown 13 images
        private BitmapImage image;
        private int currentIndex;
        private int beginIndex;       //The first image's index was shown in current page
        private int endIndex;          //endIndex-beginIndex+1=13
        SearchOption searchOption = SearchOption.TopDirectoryOnly;
        ShowMode showMode = ShowMode.Thumbnail;
        #endregion private var
        public List<BitmapImage> BitmapImages
        {
            get { return bitmapImages; }
            set { bitmapImages = value; }
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

        void LoadThumbnail()
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
        }

    }
}
