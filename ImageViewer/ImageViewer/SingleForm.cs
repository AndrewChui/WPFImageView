using System;
using System.Drawing;
using System.Windows.Forms;


namespace ImageViewer
{
    public partial class SingleForm : Form
    {
        private Bitmap currentImage;
        private Rectangle formRect;
        private Rectangle imageRect;
        private bool mouseButtonDown;
        private Point lastMousePos;
        private Images images;
        private ExifWin exifWin;
        private void IinitExifWin()
        {
            exifWin = new ExifWin();
            exifWin.ExifInfo = Util.GetProperty(currentImage);
            exifWin.SetLabe();
            exifWin.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            exifWin.Left = 0;
            exifWin.Top = 0;
            InitializeComponent();
        }
        public SingleForm(Images imgs,int index)
        {
            images = imgs;
            currentImage = images.InitSingleImage(index);
            IinitExifWin();
        }
        public SingleForm(Images imgs)
        {
            images = imgs;
            currentImage = images.InitSingleImage();
            IinitExifWin();
        }
        private void SingleForm_Load(object sender, EventArgs e)
        {
            //Size = Screen.PrimaryScreen.Bounds.Size;
            MouseWheel += SingleForm_MouseWheel;
            LoadFullImage();
        }
        private void SingleForm_MouseWheel(object sender, MouseEventArgs e)
        {
            var img = e.Delta < 0 ? images.NextSingleImage() : images.PreSingleImage();
            if(img!=null)
            {
                currentImage = img;
                LoadFullImage();
            }
        }
        private void SingleForm_Paint(object sender, PaintEventArgs e)
        {
            if (currentImage != null)
                e.Graphics.DrawImage(currentImage, formRect, imageRect, System.Drawing.GraphicsUnit.Pixel);
        }
        private void LoadFullImage()
        {
            formRect = Util.CalcImageRegion(currentImage, Size);
            imageRect = new Rectangle(Point.Empty, currentImage.Size);
            Invalidate();
        }

        private void SingleForm_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Middle)
            {
                Close();
            }
            else
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.None;
                if (e.Button == MouseButtons.Left && currentImage != null)
                {
                    mouseButtonDown = true;
                    lastMousePos = e.Location;
                    Util.CalcShowRegion(currentImage, Size, e.Location, 1, ref formRect, out imageRect);
                    Invalidate();
                }
                if (e.Button == MouseButtons.Right && currentImage != null)
                {
                    mouseButtonDown = true;
                    lastMousePos = e.Location;
                    Util.CalcShowRegion(currentImage, Size, e.Location, 2, ref formRect, out imageRect);
                    Invalidate();
                }
            }
        }

        private void SingleForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void SingleForm_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left || e.Button==MouseButtons.Right) && currentImage != null)
            {
                LoadFullImage();
                mouseButtonDown = false;
            }
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private void SingleForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseButtonDown && currentImage != null)
            {
                var deltX = (e.Location.X - lastMousePos.X) * 7;
                var deltY = (e.Location.Y - lastMousePos.Y) * 7;
                int imageX;
                int imageY;
                Size size = currentImage.Size;
                if (deltX < 0)
                    imageX = (imageRect.X + deltX < 0 ? 0 : imageRect.X + deltX);
                else
                    imageX = ((imageRect.X + deltX + imageRect.Width) > size.Width ? size.Width - imageRect.Width : imageRect.X + deltX);
                if (deltY < 0)
                    imageY = (imageRect.Y + deltY < 0 ? 0 : imageRect.Y + deltY);
                else
                    imageY = ((imageRect.Y + deltY + imageRect.Height) > size.Height ? size.Height - imageRect.Height : imageRect.Y + deltY);
                imageRect = new Rectangle(imageX, imageY, imageRect.Width, imageRect.Height);
                Invalidate();
                lastMousePos = e.Location;
            }
            else
            {
                if(e.Y<20 && exifWin.Visibility==System.Windows.Visibility.Hidden && e.X<20)
                {
                    exifWin.Show();
                }
                else
                {
                    exifWin.Hide();
                }
            }
        }

        private void SingleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            exifWin?.Close();
            images.LoadThumbnail();
        }
    }
}
